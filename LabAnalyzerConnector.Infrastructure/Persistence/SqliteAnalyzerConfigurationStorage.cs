using System.Text.Json;
using System.Text.Json.Serialization;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Configuration.Storage;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class SqliteAnalyzerConfigurationStorage
    : IAnalyzerConfigurationStorage
{
    private readonly IDbContextFactory<LabAnalyzerDbContext>
        _dbContextFactory;

    private readonly JsonSerializerOptions _jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

    public SqliteAnalyzerConfigurationStorage(
        IDbContextFactory<LabAnalyzerDbContext> dbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(
            dbContextFactory);

        _dbContextFactory =
            dbContextFactory;
    }

    // =========================================================
    // Load
    // =========================================================

    public async Task<
        IReadOnlyCollection<AnalyzerConfiguration>>
        LoadAsync()
    {
        await using LabAnalyzerDbContext db =
            await _dbContextFactory
                .CreateDbContextAsync();

        List<AnalyzerConfigurationEntity>
            entities =
                await db.AnalyzerConfigurations
                    .AsNoTracking()
                    .ToListAsync();

        var configurations =
            new List<AnalyzerConfiguration>();

        foreach (
            AnalyzerConfigurationEntity entity
            in entities)
        {
            configurations.Add(
                ToDomain(entity));
        }

        return configurations;
    }

    // =========================================================
    // Save
    // =========================================================

    public async Task SaveAsync(
        IReadOnlyCollection<AnalyzerConfiguration>
            configurations)
    {
        ArgumentNullException.ThrowIfNull(
            configurations);

        await using LabAnalyzerDbContext db =
            await _dbContextFactory
                .CreateDbContextAsync();

        // Replace the complete configuration set.
        // This keeps the storage layer synchronized
        // with the repository state.

        db.AnalyzerConfigurations.RemoveRange(
            db.AnalyzerConfigurations);

        foreach (
            AnalyzerConfiguration configuration
            in configurations)
        {
            AnalyzerConfigurationEntity entity =
                ToEntity(configuration);

            db.AnalyzerConfigurations.Add(
                entity);
        }

        await db.SaveChangesAsync();
    }

    // =========================================================
    // Domain -> Entity
    // =========================================================

    private AnalyzerConfigurationEntity
        ToEntity(
            AnalyzerConfiguration configuration)
    {
        return new AnalyzerConfigurationEntity
        {
            // IMPORTANT:
            // AnalyzerId is now the primary key.
            AnalyzerId =
                configuration.AnalyzerId,

            Name =
                configuration.Name,

            Manufacturer =
                configuration.Manufacturer,

            Model =
                configuration.Model,

            SerialNumber =
                configuration.SerialNumber,

            AnalyzerCode =
                configuration.AnalyzerCode,

            IsEnabled =
                configuration.IsEnabled,

            AutoConnect =
                configuration.AutoConnect,

            ConnectionType =
    (int)configuration.ConnectionType,

            Direction =
    (int)configuration.Direction,

            MappingProfileId =
                configuration.MappingProfileId,

            AutoReconnect =
                configuration.AutoReconnect,

            ReconnectDelaySeconds =
                configuration.ReconnectDelaySeconds,

            MaxReconnectAttempts =
                configuration.MaxReconnectAttempts,

            EnableRawMessageLogging =
                configuration.EnableRawMessageLogging,

            EnableParsedMessageLogging =
                configuration.EnableParsedMessageLogging,

            EnableErrorLogging =
                configuration.EnableErrorLogging,

            CreatedAtUtc =
                configuration.CreatedAtUtc,

            UpdatedAtUtc =
                configuration.UpdatedAtUtc,

            // =================================================
            // Complex Configuration
            // =================================================

            ProtocolJson =
                JsonSerializer.Serialize(
                    configuration.Protocol,
                    _jsonOptions),

            TcpJson =
                JsonSerializer.Serialize(
                    configuration.Tcp,
                    _jsonOptions),

            SerialJson =
                JsonSerializer.Serialize(
                    configuration.Serial,
                    _jsonOptions)
        };
    }

    // =========================================================
    // Entity -> Domain
    // =========================================================

    private AnalyzerConfiguration
        ToDomain(
            AnalyzerConfigurationEntity entity)
    {
        return new AnalyzerConfiguration
        {
            // IMPORTANT:
            // The database primary key is AnalyzerId.
            AnalyzerId =
                entity.AnalyzerId,

            Name =
                entity.Name,

            Manufacturer =
                entity.Manufacturer,

            Model =
                entity.Model,

            SerialNumber =
                entity.SerialNumber,

            AnalyzerCode =
                entity.AnalyzerCode,

            IsEnabled =
                entity.IsEnabled,

            AutoConnect =
                entity.AutoConnect,

            ConnectionType =
    (ConnectionType)entity.ConnectionType,

            Direction =
    (CommunicationDirection)entity.Direction,

            MappingProfileId =
                entity.MappingProfileId,

            AutoReconnect =
                entity.AutoReconnect,

            ReconnectDelaySeconds =
                entity.ReconnectDelaySeconds,

            MaxReconnectAttempts =
                entity.MaxReconnectAttempts,

            EnableRawMessageLogging =
                entity.EnableRawMessageLogging,

            EnableParsedMessageLogging =
                entity.EnableParsedMessageLogging,

            EnableErrorLogging =
                entity.EnableErrorLogging,

            CreatedAtUtc =
                entity.CreatedAtUtc,

            UpdatedAtUtc =
                entity.UpdatedAtUtc,

            Protocol =
                DeserializeOrDefault<
                    ProtocolConfiguration>(
                    entity.ProtocolJson),

            Tcp =
                DeserializeNullable<
                    TcpConfiguration>(
                    entity.TcpJson),

            Serial =
                DeserializeNullable<
                    SerialConfiguration>(
                    entity.SerialJson)
        };
    }

    // =========================================================
    // JSON Helpers
    // =========================================================

    private T DeserializeOrDefault<T>(
        string? json)
        where T : new()
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new T();
        }

        return JsonSerializer.Deserialize<T>(
                   json,
                   _jsonOptions)
               ?? new T();
    }

    private T? DeserializeNullable<T>(
        string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(
            json,
            _jsonOptions);
    }
}