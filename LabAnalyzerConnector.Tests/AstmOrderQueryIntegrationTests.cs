using LabAnalyzerConnector.Application.Normalization;
using LabAnalyzerConnector.Application.Orders;
using LabAnalyzerConnector.Application.Processing;
using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Configuration.Repositories;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;
using LabAnalyzerConnector.Mapping.Services;
using LabAnalyzerConnector.Protocols.ASTM;
using LabAnalyzerConnector.Protocols.ASTM.Models;
using LabAnalyzerConnector.Protocols.Models;
using LabAnalyzerConnector.Application.Results;
using LabAnalyzerConnector.Domain.Abstractions;
using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Application.Events;


namespace LabAnalyzerConnector.Tests;

public sealed class AstmOrderQueryIntegrationTests
{
    [Fact]
    public void OrderQuery_ShouldRouteThroughProtocolRouter_AndSendOrderResponse()
    {
        // =========================================================
        // ARRANGE
        // =========================================================

        Guid analyzerId =
            Guid.NewGuid();

        const string barcode =
            "0279070002";


        // =========================================================
        // CREATE LAB ORDER
        // =========================================================

        var order =
            new LabOrder
            {
                OrderId = "ORD-001",

                PatientId = "PAT-001",

                PatientName = "Test Patient",

                SpecimenId = "SPEC-001",

                Barcode = barcode,

                OrderedTests =
                    new List<string>
                    {
                        "CBC",
                        "HGB",
                        "WBC"
                    },

                Priority = "Routine",

                Status = "Pending"
            };


        // =========================================================
        // CREATE ORDER REPOSITORY
        // =========================================================

        var orderRepository =
            new FakeOrderRepository(
                order);


        // =========================================================
        // CREATE ORDER SERVICES
        // =========================================================

        var orderService =
            new OrderService(
                orderRepository);


        var fakeOrderSender =
            new FakeAnalyzerOrderSender();


        var bidirectionalOrderService =
            new BidirectionalOrderService(
                orderService,
                fakeOrderSender);


        var orderWorkflowService =
            new OrderWorkflowService(
                bidirectionalOrderService);


        var orderQueryHandler =
            new AstmOrderQueryHandler(
                orderWorkflowService);


        // =========================================================
        // CREATE ORDER RESPONSE BUILDER
        // =========================================================

        var responseBuilder =
            new AstmOrderResponseBuilder();


        // =========================================================
        // CREATE CONNECTION MANAGER
        // =========================================================

        var connectionManager =
            new FakeAnalyzerConnectionManager();


        // =========================================================
        // CREATE ASTM PROCESSOR
        // =========================================================

        var astmFramer =
            new AstmMessageFramer();


        var astmParser =
            new AstmRecordParser();


        var astmProcessor =
            new AstmProtocolProcessor(
                astmFramer,
                astmParser);


        // =========================================================
        // CREATE ANALYZER CONFIGURATION
        // =========================================================

        var configuration =
            new AnalyzerConfiguration
            {
                AnalyzerId =
                    analyzerId,

                Name =
                    "Test ASTM Analyzer",

                Manufacturer =
                    "Test Manufacturer",

                Model =
                    "Test Model",

                ConnectionType =
                    ConnectionType.TcpIp,

                Protocol =
                    new ProtocolConfiguration
                    {
                        ProtocolType =
                            ProtocolType.Astm
                    }
            };


        // =========================================================
        // CREATE CONFIGURATION REPOSITORY
        // =========================================================

        var configurationRepository =
            new FakeAnalyzerConfigurationRepository(
                configuration);


        var configurationService =
            new AnalyzerConfigurationService(
                configurationRepository);


        // =========================================================
        // CREATE PROTOCOL ROUTER
        // =========================================================

        var protocolRouter =
            new ProtocolRouter(
                configurationService,
                new[]
                {
                    astmProcessor
                });


        // =========================================================
        // CREATE MAPPING PIPELINE
        // =========================================================

        var mappingPipeline =
            new FakeMappingPipeline();


        var normalizedMessageProcessingService =
            new NormalizedMessageProcessingService(
                mappingPipeline);


        var processingService =
            new ProtocolMessageProcessingService(
                new INormalizer[]
                {
                    new AstmNormalizer()
                },
                normalizedMessageProcessingService);


        // =========================================================
        // CREATE COORDINATOR
        // =========================================================

        var fakeResultRepository =
    new FakeLabResultRepository();

var resultPersistenceService =
    new LabResultPersistenceService(
        fakeResultRepository);

        var resultMatchingService =
    new ResultMatchingService(
        orderWorkflowService);
        var dashboardEventBus = new DashboardEventBus();
        var coordinator =
            new ProtocolProcessingCoordinator(
                protocolRouter,
                processingService,
                orderQueryHandler,
                responseBuilder,
                connectionManager,
                resultPersistenceService,
            resultMatchingService,
    dashboardEventBus);


        // =========================================================
        // BUILD ASTM ORDER QUERY
        // =========================================================

        string astmQuery =
    $"\u00021Q|1|{barcode}\r\u0003AA\r\n";


        // =========================================================
        // ACT
        // =========================================================

        protocolRouter.ProcessData(
            analyzerId,
            astmQuery);


        // =========================================================
        // ASSERT
        // =========================================================

        Assert.NotNull(
            coordinator);

        Assert.Single(
            connectionManager.SentMessages);


        SentMessage sentMessage =
            connectionManager.SentMessages[0];


        Assert.Equal(
            analyzerId,
            sentMessage.AnalyzerId);


        Assert.Contains(
            "H|",
            sentMessage.Data);


        Assert.Contains(
            "P|",
            sentMessage.Data);


        Assert.Contains(
            barcode,
            sentMessage.Data);


        Assert.Contains(
            "CBC",
            sentMessage.Data);


        Assert.Contains(
            "HGB",
            sentMessage.Data);


        Assert.Contains(
            "WBC",
            sentMessage.Data);


        Assert.Contains(
            "O|",
            sentMessage.Data);


        Assert.Contains(
            "L|",
            sentMessage.Data);
    }


    // =========================================================
    // FAKE CONNECTION MANAGER
    // =========================================================

    private sealed class FakeAnalyzerConnectionManager
        : IAnalyzerConnectionManager
    {
        public List<SentMessage>
            SentMessages
        {
            get;
        } = new();


        public Task SendAsync(
            Guid analyzerId,
            string data,
            CancellationToken cancellationToken = default)
        {
            SentMessages.Add(
                new SentMessage(
                    analyzerId,
                    data));

            return Task.CompletedTask;
        }
    }


    // =========================================================
    // SENT MESSAGE
    // =========================================================

    private sealed record SentMessage(
        Guid AnalyzerId,
        string Data);


    // =========================================================
    // FAKE ORDER REPOSITORY
    // =========================================================

    private sealed class FakeOrderRepository
        : IOrderRepository
    {
        private  LabOrder _order;


        public FakeOrderRepository(
            LabOrder order)
        {
            _order =
                order;
        }


        public LabOrder?
            GetByBarcode(
                string barcode)
        {
            if (string.Equals(
                    _order.Barcode,
                    barcode,
                    StringComparison.OrdinalIgnoreCase))
            {
                return _order;
            }

            return null;
        }


        public IReadOnlyCollection<LabOrder>
            GetAll()
        {
            return new[]
            {
                _order
            };
        }


        public void Add(
            LabOrder order)
        {
            throw new NotSupportedException();
        }


        public bool Remove(
            Guid orderId)
        {
            throw new NotSupportedException();
        }

        public void Update(LabOrder order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _order = order;
        }
    }


    // =========================================================
    // FAKE ANALYZER CONFIGURATION REPOSITORY
    // =========================================================

    private sealed class FakeAnalyzerConfigurationRepository
        : IAnalyzerConfigurationRepository
    {
        private readonly AnalyzerConfiguration
            _configuration;


        public FakeAnalyzerConfigurationRepository(
            AnalyzerConfiguration configuration)
        {
            _configuration =
                configuration;
        }


        public IReadOnlyCollection<AnalyzerConfiguration>
            GetAll()
        {
            return new[]
            {
                _configuration
            };
        }


        public AnalyzerConfiguration?
            GetById(
                Guid id)
        {
            if (id ==
                _configuration.AnalyzerId)
            {
                return _configuration;
            }

            return null;
        }


        public Task LoadAsync()
        {
            return Task.CompletedTask;
        }


        public Task AddAsync(
            AnalyzerConfiguration configuration)
        {
            throw new NotSupportedException();
        }


        public Task UpdateAsync(
            AnalyzerConfiguration configuration)
        {
            throw new NotSupportedException();
        }


        public Task<bool> DeleteAsync(
            Guid id)
        {
            throw new NotSupportedException();
        }
    }


    // =========================================================
    // FAKE ORDER SENDER
    // =========================================================

    private sealed class FakeAnalyzerOrderSender
        : IAnalyzerOrderSender
    {
        public string
            BuildOrderQuery(
                string barcode)
        {
            return barcode;
        }


        public void SendOrder(
            Guid analyzerId,
            string barcode)
        {
        }
    }


    // =========================================================
    // FAKE MAPPING PIPELINE
    // =========================================================

  
private sealed class FakeMappingPipeline
    : LabAnalyzerConnector.Mapping.Abstractions.IMappingPipeline
    {
        public Task<
            LabAnalyzerConnector.Mapping.Models.MappingPipelineResult>
            ProcessAsync(
                Guid analyzerId,
                IReadOnlyDictionary<string, string?> sourceFields,
                CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new LabAnalyzerConnector.Mapping.Models.MappingPipelineResult
                {
                    IsSuccess = false
                });
        }
    }



    // =========================================================
    // FAKE LAB RESULT REPOSITORY
    // =========================================================

    private sealed class FakeLabResultRepository
        : ILabResultRepository
    {
        private readonly List<LabResult> _results =
            new();


        public Task AddAsync(
            LabResult result,
            CancellationToken cancellationToken = default)
        {
            _results.Add(result);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<LabResult>> GetByAnalyzerIdAsync(
    Guid analyzerId,
    CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LabResult> results =
                _results
                    .Where(x => x.AnalyzerId == analyzerId)
                    .ToList();

            return Task.FromResult(results);
        }


        public Task<IReadOnlyCollection<LabResult>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LabResult> results =
                _results.ToList();

            return Task.FromResult(
                results);
        }


        public Task<LabResult?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            LabResult? result =
                _results.FirstOrDefault(
                    x => x.Id == id);

            return Task.FromResult(
                result);
        }


        public Task<IReadOnlyCollection<LabResult>>
            GetByPatientIdAsync(
                string patientId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LabResult> results =
                _results
                    .Where(
                        x =>
                            string.Equals(
                                x.PatientId,
                                patientId,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return Task.FromResult(
                results);
        }


        public Task<IReadOnlyCollection<LabResult>>
            GetBySampleIdAsync(
                string sampleId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LabResult> results =
                _results
                    .Where(
                        x =>
                            string.Equals(
                                x.SampleId,
                                sampleId,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return Task.FromResult(
                results);
        }
    }
}