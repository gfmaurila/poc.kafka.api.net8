using Confluent.Kafka;
using Newtonsoft.Json;
using poc.api.sqlserver.Model;

namespace poc.api.sqlserver.Service.Producer;

public class CriarProdutoProducer : ICriarProdutoProducer
{
    private readonly ProducerConfig _config;
    private readonly string _topic;
    private readonly ILogger<CriarProdutoProducer> _logger;

    public CriarProdutoProducer(IConfiguration configuration, ILogger<CriarProdutoProducer> logger)
    {
        _config = new ProducerConfig { BootstrapServers = configuration["Kafka:BootstrapServers"] };
        _topic = configuration["Kafka:Topic:Produto:CriarProduto"];
        _logger = logger;
    }

    public async Task Publish(Produto model)
    {
        _logger.LogInformation($"Producer > Publish > Produto > CRIAR_PRODUTO - SQL Server...");
        using var producer = new ProducerBuilder<Null, string>(_config).Build();
        var message = JsonConvert.SerializeObject(model);
        await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
        producer.Flush(TimeSpan.FromSeconds(10));
    }
}