using LabAnalyzerConnector.Application.Normalization;
using LabAnalyzerConnector.Application.Orders;
using LabAnalyzerConnector.Application.Processing;
using LabAnalyzerConnector.Application.Services;
using LabAnalyzerConnector.Communication.Factories;
using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Core.Configuration.Repositories;
using LabAnalyzerConnector.Core.Configuration.Storage;
using LabAnalyzerConnector.Core.Services;
using LabAnalyzerConnector.Infrastructure.Persistence;
using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Services;
using LabAnalyzerConnector.Protocols.ASTM;
using LabAnalyzerConnector.Protocols.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LabAnalyzerConnector.Domain.Abstractions;
using LabAnalyzerConnector.Application.Results;
using LabAnalyzerConnector.Application.Transmission;
using LabAnalyzerConnector.Protocols.HL7.Parsing;
using LabAnalyzerConnector.Protocols.HL7;
using LabAnalyzerConnector.Protocols.HL7.Filtering;
using LabAnalyzerConnector.Application.Events;
using LabAnalyzerConnector.Application.ViewModels;
using LabAnalyzerConnector.Application.Catalog;
using LabAnalyzerConnector.Protocols.HL7.Framing;

namespace LabAnalyzerConnector.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLabAnalyzerConnector(
        this IServiceCollection services)
    {
        // =====================================================
        // DATABASE
        // =====================================================

        string dataDirectory =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data");

        Directory.CreateDirectory(
            dataDirectory);

        string databasePath =
            Path.Combine(
                dataDirectory,
                "LabAnalyzerConnector.db");

        string connectionString =
            $"Data Source={databasePath}";


        services.AddDbContextFactory<
            LabAnalyzerDbContext>(
            options =>
            {
                options.UseSqlite(
                    connectionString);
            });


        services.AddSingleton<
            LabAnalyzerDatabaseInitializer>();


        // =====================================================
        // ANALYZER CONFIGURATION STORAGE
        // =====================================================

        services.AddSingleton<
            IAnalyzerConfigurationStorage,
            SqliteAnalyzerConfigurationStorage>();


        services.AddSingleton<
            IAnalyzerConfigurationRepository,
            AnalyzerConfigurationRepository>();


        services.AddSingleton<
            AnalyzerConfigurationService>();


        services.AddSingleton<
            IAnalyzerConfigurationService>(
            provider =>
                provider.GetRequiredService<
                    AnalyzerConfigurationService>());

        services.AddSingleton<
    ILabResultRepository,
    SqliteLabResultRepository>();

        services.AddSingleton<
    LabResultPersistenceService>();

       


        // =====================================================
        // ANALYZER MANAGEMENT
        // =====================================================

        services.AddSingleton<
            AnalyzerManager>();


        services.AddSingleton<
            IAnalyzerManager>(
            provider =>
                provider.GetRequiredService<
                    AnalyzerManager>());


        // =====================================================
        // ORDER REPOSITORY
        // =====================================================

        // Production application uses SQLite.
        //
        // Do NOT register InMemoryOrderRepository here.
        // InMemoryOrderRepository should only be used by tests.

        services.AddSingleton<
            IOrderRepository,
            SqliteOrderRepository>();


        // =====================================================
        // ORDER SERVICES
        // =====================================================

        services.AddSingleton<
            OrderService>();

        services.AddSingleton<
    OrderTransmissionService>();

        services.AddSingleton<
            AstmOrderMessageBuilder>();

        services.AddSingleton<
            IAnalyzerOrderSender,
            AstmOrderSender>();


        services.AddSingleton<
            BidirectionalOrderService>();


        services.AddSingleton<
            OrderWorkflowService>();


        services.AddSingleton<
            AnalyzerOrderQueryService>();


        services.AddSingleton<
            AnalyzerOrderResponseService>();


        // =====================================================
        // COMMUNICATION
        // =====================================================

        services.AddSingleton<
            ConnectionFactory>();


        services.AddSingleton<
            ConnectionManager>();


        services.AddSingleton<
            IAnalyzerConnectionManager>(
            provider =>
                provider.GetRequiredService<
                    ConnectionManager>());


        services.AddSingleton<
            AnalyzerConnectionCoordinator>();


        // =====================================================
        // ASTM FRAMING AND PARSING
        // =====================================================

        services.AddTransient<
            AstmMessageFramer>();


        services.AddTransient<
            AstmRecordParser>();


        // =====================================================
        // ASTM PROTOCOL PROCESSOR
        // =====================================================

        services.AddTransient<
            AstmProtocolProcessor>();


        services.AddTransient<
            IProtocolProcessor,
            AstmProtocolProcessor>();


        // =====================================================
        // ASTM ORDER MESSAGES
        // =====================================================

        services.AddTransient<
            AstmOrderMessageBuilder>();


        services.AddTransient<
            AstmOrderResponseBuilder>();


        services.AddTransient<
            AstmOrderQueryHandler>();


        // =====================================================
        // PROTOCOL ROUTING
        // =====================================================

        services.AddSingleton<
            ProtocolRouter>();
        services.AddSingleton<AnalyzerCatalogService>();

        services.AddSingleton<IAnalyzerCatalogService>(
            provider => provider.GetRequiredService<AnalyzerCatalogService>());

        // =====================================================
        // NORMALIZATION
        // =====================================================

        services.AddTransient<
            INormalizer,
            AstmNormalizer>();


        services.AddTransient<
    INormalizer,
    Hl7Normalizer>();


        services.AddTransient<AstmMessageFramer>();
        services.AddTransient<AstmRecordParser>();
        services.AddTransient<AstmProtocolProcessor>();
        services.AddTransient<IProtocolProcessor, AstmProtocolProcessor>();

        // HL7
        services.AddTransient<Hl7MessageFramer>();
        services.AddTransient<Hl7Parser>();
        services.AddSingleton<Hl7ResultFilter>();
        services.AddTransient<Hl7ProtocolProcessor>();
        services.AddTransient<IProtocolProcessor, Hl7ProtocolProcessor>();

        // =====================================================
        // MAPPING SERVICES
        // =====================================================

        services.AddSingleton<
            IAnalyzerMappingProfileService,
            AnalyzerMappingProfileService>();

        services.AddSingleton<
     AnalyzerMappingInitializer>();

        services.AddSingleton<DashboardEventBus>();

        services.AddTransient<AnalyzerManagementViewModel>();

        services.AddSingleton<AnalyzerConnectionTestService>();

        services.AddTransient<AnalyzerManagementService>();


        services.AddSingleton<
            IFieldMappingEngine,
            FieldMappingEngine>();


        services.AddSingleton<
            ITestCodeMapper,
            TestCodeMapper>();

        services.AddSingleton<
    ITestCodeMappingRepository,
    SqliteTestCodeMappingRepository>();

      


        services.AddSingleton<
            IResultTransformationEngine,
            ResultTransformationEngine>();


        services.AddSingleton<
            IUnitConversionEngine,
            UnitConversionEngine>();


        services.AddSingleton<
            IMappingValidationService,
            MappingValidationService>();

        services.AddSingleton<DashboardViewModel>();

        services.AddTransient<ResultsViewModel>();


        // =====================================================
        // MAPPING PIPELINE
        // =====================================================

        services.AddSingleton<
            IMappingPipeline,
            MappingPipeline>();


        services.AddSingleton<
            NormalizedMessageProcessingService>();


        // =====================================================
        // PROTOCOL MESSAGE PROCESSING
        // =====================================================

        services.AddSingleton<
            ProtocolMessageProcessingService>();


        // =====================================================
        // PROTOCOL PROCESSING COORDINATOR
        // =====================================================

        services.AddSingleton<
            ProtocolProcessingCoordinator>();


        // =====================================================
        // CONNECTION PROCESSING
        // =====================================================

        services.AddSingleton<
            ConnectionProcessingCoordinator>();

        services.AddSingleton<ResultMatchingService>();


        return services;
    }
}