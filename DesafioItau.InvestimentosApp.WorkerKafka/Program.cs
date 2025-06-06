using Confluent.Kafka;
using DesafioItau.InvestimentosApp.CotacoesConsumer;

var builder = Host.CreateApplicationBuilder(args);

var kafkaSection = builder.Configuration.GetSection("Kafka");

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = kafkaSection.GetValue<string>("BootstrapServers"),
    GroupId = kafkaSection.GetValue<string>("GroupId"),
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false //Para n�o processar mensagens duplicadas
};

// Registra o ConsumerConfig como singleton para inje��o
builder.Services.AddSingleton(consumerConfig);

// Registra o Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
