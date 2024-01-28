using poc.api.redis.Model;
using poc.api.redis.Service.Persistence;
using System.Text.Json;
using Confluent.Kafka;

namespace poc.api.redis.Service.Consumers;

//public class CriarProdutoConsumer : BackgroundService
//{
//    private readonly string _topic;
//    private readonly ConsumerConfig _config;
//    private readonly IProdutoService _produtoService;
//    private readonly ILogger<CriarProdutoConsumer> _logger;

//    public CriarProdutoConsumer(IConfiguration configuration, ILogger<CriarProdutoConsumer> logger, IProdutoService produtoService)
//    {
//        _config = new ConsumerConfig
//        {
//            BootstrapServers = configuration["Kafka:BootstrapServers"],
//            GroupId = configuration["Kafka:GroupId:Produto"],
//            AutoOffsetReset = AutoOffsetReset.Earliest
//        };
//        _topic = configuration["Kafka:Topic:Produto:CriarProduto"];
//        _logger = logger;
//        _produtoService = produtoService;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("Consumer > ExecuteAsync > Produto > CRIAR_PRODUTO > Redis...");
//        using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
//        consumer.Subscribe(_topic);

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            var consumeResult = consumer.Consume(stoppingToken);
//            var model = JsonSerializer.Deserialize<Produto>(consumeResult.Message.Value);
//            await _produtoService.Post(model);
//            _logger.LogInformation($"Mensagem consumida do tópico {_topic}: {consumeResult.Message.Value}");
//        }
//        consumer.Close();
//    }
//}


public class CriarProdutoConsumer : BackgroundService
{
    private readonly string _topic;
    private readonly ConsumerConfig _config;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CriarProdutoConsumer> _logger;

    public CriarProdutoConsumer(IConfiguration configuration, ILogger<CriarProdutoConsumer> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId:Produto"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _topic = configuration["Kafka:Topic:Produto:CriarProduto"];
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consumer > ExecuteAsync > Produto > CRIAR_PRODUTO > Redis...");
        using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            var model = JsonSerializer.Deserialize<Produto>(consumeResult.Message.Value);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();
                await produtoService.Post(model);
            }

            _logger.LogInformation($"Mensagem consumida do tópico {_topic}: {consumeResult.Message.Value}");
        }
        consumer.Close();
    }
}

