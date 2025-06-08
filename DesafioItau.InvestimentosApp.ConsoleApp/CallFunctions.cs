using System;
using System.Threading.Tasks;
using DesafioItau.InvestimentosApp.Domain.Ativos;
using DesafioItau.InvestimentosApp.Domain.Usuarios;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using DesafioItau.InvestimentosApp.Repository.DbCotacoesContext;
using DesafioItau.InvestimentosApp.Repository.DbUsuariosContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

class CallFunctions
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(configuration.GetValue<string>("UrlApiB3"))
        };        

        var loggerAtivos = loggerFactory.CreateLogger<DbAtivosContext>();
        var loggerCotacoes = loggerFactory.CreateLogger<DbCotacoesContext>();
        var loggerUsuarios = loggerFactory.CreateLogger<DbUsuariosContext>();

        // Instantiate the contexts
        IAtivosContext ativosContext = new DbAtivosContext(configuration, loggerAtivos);
        ICotacoesContext cotacoesContext = new DbCotacoesContext(configuration, loggerCotacoes);
        IUsuariosContext usuariosContext = new DbUsuariosContext(configuration, loggerUsuarios);

        // Instantiate services
        IAtivosService ativosService = new AtivosService(ativosContext, cotacoesContext);
        IUsuariosService usuariosService = new UsuariosService(usuariosContext);

        var b3ApiClient = new B3ApiClient(cotacoesContext, httpClient, loggerFactory.CreateLogger<string>());

        //Toda vez que rodar, popular a tabela de cotações.
        await b3ApiClient.InserirCotacoesAsync();


        while (true)
        {
            Console.WriteLine("\nEscolha uma opção:");
            Console.WriteLine("1 - Consultar último preço do ativo");
            Console.WriteLine("2 - Consultar preço médio do usuário por ativo");
            Console.WriteLine("3 - Consultar posição do usuário");
            Console.WriteLine("4 - Consultar corretagem total do usuário");
            Console.WriteLine("5 - Consultar Top 10 posições");
            Console.WriteLine("6 - Consultar Top 10 corretagens");
            Console.WriteLine("0 - Sair");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    Console.Write("Digite o código do ativo: ");
                    var codigoAtivo = Console.ReadLine();
                    var resultadoAtivo = await ativosService.GetAtivo(codigoAtivo);
                    if (resultadoAtivo.Success)
                    {
                        var ativo = resultadoAtivo.Data;
                        Console.WriteLine($"Ativo: {ativo.Ativo}, Preço: {ativo.Preco}, DataHora: {ativo.DataHora}");
                    }
                    else
                    {
                        Console.WriteLine($"Erro: {resultadoAtivo.ErrorMessage}");
                    }
                    break;

                case "2":
                    Console.Write("Digite o ID do usuário: ");
                    if (int.TryParse(Console.ReadLine(), out int usuarioIdPrecoMedio))
                    {
                        Console.Write("Digite o código do ativo: ");
                        var codigoAtivoPrecoMedio = Console.ReadLine();
                        var precoMedioResult = await usuariosService.GetPrecoMedioAsync(usuarioIdPrecoMedio, codigoAtivoPrecoMedio);
                        if (precoMedioResult.Success)
                        {
                            var precoMedio = precoMedioResult.Data;
                            Console.WriteLine($"Preço médio do usuário {usuarioIdPrecoMedio} para ativo {codigoAtivoPrecoMedio}: {precoMedio.PrecoMedio}");
                        }
                        else
                        {
                            Console.WriteLine($"Erro: {precoMedioResult.ErrorMessage}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID do usuário inválido.");
                    }
                    break;

                case "3":
                    Console.Write("Digite o ID do usuário: ");
                    if (int.TryParse(Console.ReadLine(), out int usuarioIdPosicao))
                    {
                        var posicaoResult = await usuariosService.GetPosicao(usuarioIdPosicao);
                        if (posicaoResult.Success)
                        {
                            foreach (var pos in posicaoResult.Data)
                            {
                                Console.WriteLine($"Ativo: {pos.NomeAtivo}, Quantidade: {pos.Quantidade}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Erro: {posicaoResult.ErrorMessage}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID do usuário inválido.");
                    }
                    break;

                case "4":
                    Console.Write("Digite o ID do usuário: ");
                    if (int.TryParse(Console.ReadLine(), out int usuarioIdCorretagem))
                    {
                        var corretagemResult = await usuariosService.GetCorretagemTotal(usuarioIdCorretagem);
                        if (corretagemResult.Success)
                        {
                            var corretagem = corretagemResult.Data;
                            Console.WriteLine($"Corretagem total do usuário {usuarioIdCorretagem}: {corretagem.TotalCorretagem}");
                        }
                        else
                        {
                            Console.WriteLine($"Erro: {corretagemResult.ErrorMessage}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID do usuário inválido.");
                    }
                    break;

                case "5":
                    var topPosicoesResult = await usuariosService.GetPosicaoTotal();
                    if (topPosicoesResult.Success)
                    {
                        Console.WriteLine("Top 10 clientes com maiores posições:");
                        foreach (var pos in topPosicoesResult.Data)
                        {
                            Console.WriteLine($"Cliente: {pos.NomUser}, Total Posição: {pos.TotalPosicao}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Erro: {topPosicoesResult.ErrorMessage}");
                    }
                    break;

                case "6":
                    var topCorretagensResult = await usuariosService.GetCorretagemTotal();
                    if (topCorretagensResult.Success)
                    {
                        Console.WriteLine("Top 10 clientes que mais pagaram corretagem:");
                        foreach (var c in topCorretagensResult.Data)
                        {
                            Console.WriteLine($"Cliente: {c.NomUser}, Total Corretagem: {c.TotalCorretagem}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Erro: {topCorretagensResult.ErrorMessage}");
                    }
                    break;

                case "0":
                    Console.WriteLine("Saindo...");
                    return;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }
}
