using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using DesafioItau.InvestimentosApp.Repository.DbCotacoesContext;
using System.Net.Http;
using System.Text.Json;

public class B3ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ICotacoesContext _cotacoesContext;

    public B3ApiClient(ICotacoesContext cotacoesContext)
    {
        _cotacoesContext = cotacoesContext;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://b3api.vercel.app/api/Assets/")
        };
    }

    public async Task<AssetInfo?> GetAssetAsync(string codigo)
    {
        var response = await _httpClient.GetAsync(codigo);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        return new AssetInfo
        {
            Price = root.GetProperty("price").GetDecimal(),
            TradeTime = root.GetProperty("updatedAt").GetDateTime()
        };
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
                        await _cotacoesContext.InsereNovaCotacaoNaBaseInsUpd(id, asset.Price, asset.TradeTime);
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

