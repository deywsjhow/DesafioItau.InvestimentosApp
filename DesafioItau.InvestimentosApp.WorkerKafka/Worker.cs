using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace DesafioItau.InvestimentosApp.CotacoesConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
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
    }
}
