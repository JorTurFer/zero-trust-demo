using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace zero_trust_demo;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ServiceBusOptions();
        var sbSection = configuration.GetSection(ServiceBusOptions.SectionName);
        sbSection.Bind(options);
        services.Configure<ServiceBusOptions>(sbSection);

        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClientWithNamespace(options.Namespace)
                .WithCredential(new DefaultAzureCredential());
            builder.AddServiceBusAdministrationClientWithNamespace(options.Namespace)
                .WithCredential(new DefaultAzureCredential());
        });

        return services;
    }
}