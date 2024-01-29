using Microsoft.EntityFrameworkCore;
using poc.api.sqlserver.Configuration;
using poc.api.sqlserver.EndPoints;
using poc.api.sqlserver.Service.Persistence;
using poc.api.sqlserver.Service.Producer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddConnections();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig(builder.Configuration);

// Sql Server
builder.Services.AddDbContext<SqlServerDb>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// Service 
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// Bus
builder.Services.AddSingleton<IProdutoProducer, ProdutoProducer>();

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.RegisterProdutosEndpoints();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();