using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using DesafioItau.InvestimentosApp.Repository.DbCotacoesContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

public class B3ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ICotacoesContext _cotacoesContext;
    private readonly ILogger _logger;

    public B3ApiClient(ICotacoesContext cotacoesContext, HttpClient httpClient, ILogger logger)
    {
        _cotacoesContext = cotacoesContext;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AssetInfo?> GetAssetAsync(string codigo)
    {
        try
        {
            var response = await _httpClient.GetAsync(codigo);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jsonDeserialize = JsonSerializer.Deserialize<AssetInfo>(json);

            if (jsonDeserialize is null)
                return null;

            return new AssetInfo
            {
                price = jsonDeserialize.price,
                tradetime = jsonDeserialize.tradetime
            };

        }
        catch(HttpRequestException ex)
        {
            //_logger.LogError(ex, "Erro ao consultar a api da B3");
            Console.WriteLine("Erro ao consultar a api da B3: {erro}", ex);
            return null;
        }
        
    }

    public async Task InserirCotacoesAsync()
    {
        var codigos = new List<string> { "PETR4", "VALE3", "ITUB4", "ABEV3", "BBAS3" };
        var id = 0;

        foreach (var item in codigos)
        {
            {
                try
                {
                    var asset = await GetAssetAsync(item);
                    if (asset != null)
                    {
                        id = id + 1;                        
                        await _cotacoesContext.InsereNovaCotacaoNaBaseInsUpd(id, asset.price, asset.tradetime);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao consultar {item}: {ex.Message}");
                }
            }
        }
    }
}

