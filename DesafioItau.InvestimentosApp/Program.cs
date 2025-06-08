using DesafioItau.InvestimentosApp.Domain.Ativos;
using DesafioItau.InvestimentosApp.Domain.Usuarios;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using DesafioItau.InvestimentosApp.Repository.DbCotacoesContext;
using DesafioItau.InvestimentosApp.Repository.DbUsuariosContext;

var builder = WebApplication.CreateBuilder(args);

// Injeção de dependências do contexto e serviços
builder.Services.AddScoped<IAtivosContext, DbAtivosContext>();
builder.Services.AddScoped<ICotacoesContext, DbCotacoesContext>();
builder.Services.AddScoped<IUsuariosContext, DbUsuariosContext>();

builder.Services.AddScoped<IAtivosService, AtivosService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();


// Registro do B3ApiClient com HttpClient configurado
builder.Services.AddHttpClient<B3ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlApiB3"));
});

// Demais configurações padrão
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
