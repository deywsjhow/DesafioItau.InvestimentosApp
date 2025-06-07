using Confluent.Kafka;
using Polly;
using Polly.Retry;

using DesafioItau.InvestimentosApp.Repository.DbCotacoesContext;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using System.Text.Json;
using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
namespace DesafioItau.InvestimentosApp.CotacoesConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncPolicy _policyExecutora;
        private readonly IAtivosContext _ativosContext;
        private readonly ICotacoesContext _cotacoesContext;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IAtivosContext ativosContext, ICotacoesContext cotacoesContext)
        {
            _logger = logger;
            _ativosContext = ativosContext;
            _cotacoesContext = cotacoesContext;

            var kafkaConfig = configuration.GetSection("Kafka");

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaConfig.GetValue<string>("BootstrapServers"),
                GroupId = kafkaConfig.GetValue<string>("GroupId"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = kafkaConfig.GetValue<bool>("EnableAutoCommit")
            };

            _topic = kafkaConfig.GetValue<string>("Topic");

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _consumer.Subscribe(_topic);

            // Retry 3 tentativas
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, delay, count, ctx) => _logger.LogWarning("Tentativa {tentativa} falhou: {mensagem}. Retry em {tempo}.", count, ex.Message, delay.TotalSeconds));

            var fallbackPolicy = Policy
                .Handle<Exception>()
                .FallbackAsync(
                     fallbackAction: async (ct) =>
                     {
                         _logger.LogError("Falha definitiva no processamento da mensagem após todas as tentativas. Executando fallback.");
                         await Task.CompletedTask;
                     },
                     onFallbackAsync: async (ex) => 
                     {
                         _logger.LogError(ex.Message); 
                         await Task.CompletedTask;
                     });

            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) =>
                    {
                        _logger.LogWarning("Circuit breaker aberto por {BreakDelay} devido a: {Exception}", breakDelay, ex.Message);
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit breaker resetado. Tentando novamente.");
                    });                  


            var combinedCircuitRetry = circuitBreakerPolicy.WrapAsync(_retryPolicy);
            _policyExecutora = fallbackPolicy.WrapAsync(combinedCircuitRetry);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker iniciado...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);
                        _logger.LogInformation("Mensagem recebida {msg}:", consumeResult.Message.Value);

                        await _policyExecutora.ExecuteAsync(async () =>
                        {
                            await ProcessarMensagemAsync(consumeResult.Message.Value);
                        });

                        //commitando o resultado!
                        _consumer.Commit(consumeResult);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError("Erro ao consumir mensagem {mensagem}:", ex.Error.Reason);
                    }
                }

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Worker finalizado pelo cancelamento");
            }
            finally
            {
                _consumer.Close();
            }

            await Task.CompletedTask;
        }

        private async Task ProcessarMensagemAsync(string mensagem)
        {
            _logger.LogInformation("Processando cotação {cotacao}:", mensagem);

            //deserealizando a mensagem
            var cotacao = JsonSerializer.Deserialize<CotacoesResponse>(mensagem);

            //Idempotência - verificando se o ativo já existe na base
            var ativo = await _ativosContext.GetAtivo(cotacao.Ativo);

            if(ativo.codigo is null)
            {
                _logger.LogWarning("Ativo {Ativo} não encontrado. Ignorando mensagem.", ativo.codigo);
                return;
            }

            //Verificando se já existe uma cotação desse ativo 
            var cotacaoVerificada = await _cotacoesContext.GetCotacaoByAtivoAndDateTime(ativo.id, cotacao.DataHora);

            if (cotacaoVerificada != null)
            {
                _logger.LogInformation("Cotação já registrada para o ativo {Ativo} em {Data}. Ignorando...", cotacao.Ativo, cotacao.DataHora);
                return;
            }

            //Processando a mensagem na base
            await _cotacoesContext.InsereNovaCotacaoNaBase(ativo.id, cotacao.Preco, DateTime.Now);

            _logger.LogInformation("Cotação registrada com sucesso para o ativo {Ativo}", cotacao.Ativo);
        }
    }

}
