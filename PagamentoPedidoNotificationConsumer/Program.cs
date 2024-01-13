using Amazon.S3;
using Amazon.SQS;
using Application.UseCases;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PagamentoPedidoNotificationConsumer.Service;
using PagamentoPedidoNotificationConsumer.HealthCheck;


await Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<PagamentoPedidoConsumer>();
            services.AddHostedService(provider => provider.GetService<PagamentoPedidoConsumer>());
            services.AddHealthChecks().AddCheck<CustomHealthCheck>("custom_check");
            services.AddHostedService<ConsumerBackgroundService>();

            services.AddTransient<IPedidoUseCase, PedidoUseCase>();
            services.AddTransient<ICarrinhoRepository, CarrinhoRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();

            services.Configure<DatabaseConfig>(hostContext.Configuration.GetSection(nameof(DatabaseConfig)));
            services.AddSingleton<IDatabaseConfig>(sp => sp.GetRequiredService<IOptions<DatabaseConfig>>().Value);

            services.AddLocalStack(hostContext.Configuration);
            services.AddAWSServiceLocalStack<IAmazonSQS>();
            services.AddAWSServiceLocalStack<IAmazonS3>();
            services.AddLogging();
        })
        .RunConsoleAsync();
