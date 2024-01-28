using poc.api.sqlserver.Model;

namespace poc.api.sqlserver.Service.Producer;

public interface ICriarProdutoProducer
{
    Task Publish(Produto model);
}