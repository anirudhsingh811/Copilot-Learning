using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;

// This is the main entry point for the Azure Functions application on Azure cloud and local development.

var host = new HostBuilder()// host for Azure Functions
    .ConfigureFunctionsWorkerDefaults() // configure worker defaults on the host  including logging, etc.
    .ConfigureServices(services => // configure additional services
    {
        services.AddApplicationInsightsTelemetryWorkerService(); // add Application Insights telemetry for worker service
        services.ConfigureFunctionsApplicationInsights();// configure Application Insights for Azure Functions
    })
    .Build();

await host.RunAsync();// run the host
