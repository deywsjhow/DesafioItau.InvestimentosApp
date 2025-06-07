using Confluent.Kafka;
using DesafioItau.InvestimentosApp.CotacoesConsumer;

var builder = Host.CreateApplicationBuilder(args);

var kafkaSection = builder.Configuration.GetSection("Kafka");

builder.Services.AddSingleton(sp =>
{
    var config = new ConsumerConfig
    {
        BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers"),
        GroupId = builder.Configuration.GetValue<string>("Kafka:GroupId"),
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = builder.Configuration.GetValue<bool> ("Kafka:EnableAutoCommit")
    };

    return new ConsumerBuilder<Ignore, string>(config).Build();
});


builder.Services.AddSingleton<IConsumer<Ignore, string>>(sp =>
{
    var config = sp.GetRequiredService<ConsumerConfig>();
    return new ConsumerBuilder<Ignore, string>(config).Build();
});

// Registra o Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
