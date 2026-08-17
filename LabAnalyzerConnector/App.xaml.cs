using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

using LabAnalyzerConnector.Application.Processing;
using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Infrastructure.DependencyInjection;
using LabAnalyzerConnector.Infrastructure.Persistence;
using LabAnalyzerConnector.Mapping.Services;
using LabAnalyzerConnector.Application.Services;
using LabAnalyzerConnector.Core.Services;

namespace LabAnalyzerConnector;

public partial class App : System.Windows.Application
{
    public IServiceProvider Services { get; }

    public App()
    {
        IServiceCollection serviceCollection =
            new ServiceCollection();

        // Register all application services
        serviceCollection.AddLabAnalyzerConnector();

        // Build dependency injection container
        Services =
            serviceCollection.BuildServiceProvider();
    }

    protected override async void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);

        // ================================================
        // 1. Initialize database
        // ================================================

        LabAnalyzerDatabaseInitializer databaseInitializer =
            Services.GetRequiredService<
                LabAnalyzerDatabaseInitializer>();

        await databaseInitializer.InitializeAsync();

        // ================================================
        // 2. Get Analyzer Manager
        // ================================================

        IAnalyzerManager analyzerManager =
            Services.GetRequiredService<
                IAnalyzerManager>();

        // ================================================
        // 3. Load saved analyzer configurations
        // ================================================

        await analyzerManager.LoadAsync();

        // ================================================
        // 4. Initialize analyzer mapping profiles
        // ================================================

        AnalyzerMappingInitializer mappingInitializer =
            Services.GetRequiredService<
                AnalyzerMappingInitializer>();

        foreach (AnalyzerConfiguration analyzer
                 in analyzerManager.GetAnalyzers())
        {
            mappingInitializer.Initialize(
                analyzer.AnalyzerId,
                analyzer.Name);
        }

        // ================================================
        // 5. Initialize event-driven processing pipeline
        // ================================================

        _ =
            Services.GetRequiredService<
                ConnectionProcessingCoordinator>();

        _ =
            Services.GetRequiredService<
                ProtocolProcessingCoordinator>();

        // ================================================
        // 6. Initialize connection coordinator
        // ================================================

        AnalyzerConnectionCoordinator connectionCoordinator =
            Services.GetRequiredService<
                AnalyzerConnectionCoordinator>();

        // ================================================
        // 7. Automatically connect analyzers
        // ================================================

        try
        {
            await connectionCoordinator.ConnectAllAsync();
        }
        catch (OperationCanceledException)
        {
            // A connection attempt was cancelled.
            // The application can still start normally.
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"One or more analyzers could not connect during startup.\n\n{ex.Message}",
                "Analyzer Connection Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        // ================================================
        // 8. Create Main Dashboard
        // ================================================

        MainWindow mainWindow =
            ActivatorUtilities.CreateInstance<
                MainWindow>(
                    Services);

        DashboardWindow dashboard =
            ActivatorUtilities.CreateInstance<
                DashboardWindow>(
                    Services);

        // ================================================
        // 9. Set application MainWindow
        // ================================================

        MainWindow = dashboard;

        // ================================================
        // 10. Show Dashboard
        // ================================================

        dashboard.Show();
    }
}

