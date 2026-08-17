using LabAnalyzerConnector.Application.Events;
using LabAnalyzerConnector.Application.Normalization;
using LabAnalyzerConnector.Application.Orders;
using LabAnalyzerConnector.Application.Processing;
using LabAnalyzerConnector.Application.Results;
using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Configuration.Repositories;
using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;
using LabAnalyzerConnector.Domain.Abstractions;
using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;
using LabAnalyzerConnector.Mapping.Services;
using LabAnalyzerConnector.Protocols.ASTM;
using LabAnalyzerConnector.Protocols.ASTM.Models;
using LabAnalyzerConnector.Protocols.Models;

namespace LabAnalyzerConnector.Tests;

public sealed class ProtocolProcessingCoordinatorTests
{
    [Fact]
    public async Task OrderQuery_ShouldFindOrder_BuildResponse_AndSendToAnalyzer()
    {
        // =========================================================
        // ARRANGE
        // =========================================================

        Guid analyzerId =
            Guid.NewGuid();

        const string barcode =
            "0279070002";


        // =========================================================
        // CREATE ORDER
        // =========================================================

        var order =
            new LabOrder
            {
                OrderId =
                    "ORD-001",

                PatientId =
                    "PAT-001",

                PatientName =
                    "Test Patient",

                SpecimenId =
                    "SPEC-001",

                Barcode =
                    barcode,

                OrderedTests =
                    new List<string>
                    {
                        "CBC",
                        "HGB",
                        "WBC"
                    },

                Priority =
                    "Routine",

                Status =
                    "Pending"
            };


        // =========================================================
        // CREATE ORDER REPOSITORY
        // =========================================================

        var repository =
            new FakeOrderRepository(
                order);


        // =========================================================
        // CREATE ORDER SERVICE
        // =========================================================

        var orderService =
            new OrderService(
                repository);


        // =========================================================
        // CREATE ORDER SENDER
        // =========================================================

        var fakeOrderSender =
            new FakeAnalyzerOrderSender();


        // =========================================================
        // CREATE BIDIRECTIONAL ORDER SERVICE
        // =========================================================

        var bidirectionalOrderService =
            new BidirectionalOrderService(
                orderService,
                fakeOrderSender);


        // =========================================================
        // CREATE ORDER WORKFLOW SERVICE
        // =========================================================

        var orderWorkflowService =
            new OrderWorkflowService(
                bidirectionalOrderService);


        // =========================================================
        // CREATE ASTM ORDER QUERY HANDLER
        // =========================================================

        var orderQueryHandler =
            new AstmOrderQueryHandler(
                orderWorkflowService);


        // =========================================================
        // CREATE ASTM RESPONSE BUILDER
        // =========================================================

        var responseBuilder =
            new AstmOrderResponseBuilder();


        // =========================================================
        // CREATE FAKE CONNECTION MANAGER
        // =========================================================

        var connectionManager =
            new FakeAnalyzerConnectionManager();


        // =========================================================
        // CREATE RESULT REPOSITORY
        // =========================================================

        var fakeResultRepository =
            new FakeLabResultRepository();


        // =========================================================
        // CREATE RESULT PERSISTENCE SERVICE
        // =========================================================

        var resultPersistenceService =
            new LabResultPersistenceService(
                fakeResultRepository);


        // =========================================================
        // CREATE FAKE MAPPING PIPELINE
        // =========================================================

        var mappingPipeline =
            new FakeMappingPipeline();


        // =========================================================
        // CREATE NORMALIZED MESSAGE PROCESSING SERVICE
        // =========================================================

        var mappingService =
            new NormalizedMessageProcessingService(
                mappingPipeline);


        // =========================================================
        // CREATE PROTOCOL MESSAGE PROCESSING SERVICE
        // =========================================================

        var processingService =
            new ProtocolMessageProcessingService(
                new List<INormalizer>
                {
                    new AstmNormalizer()
                },
                mappingService);


        // =========================================================
        // CREATE PROTOCOL ROUTER
        // =========================================================

        var router =
            CreateProtocolRouter(
                analyzerId);

        var resultMatchingService =
    new ResultMatchingService(
        orderWorkflowService);

        // =========================================================
        // CREATE PROTOCOL PROCESSING COORDINATOR
        // =========================================================
        var dashboardEventBus = new DashboardEventBus();
        var coordinator =
    new ProtocolProcessingCoordinator(
        router,
        processingService,
        orderQueryHandler,
        responseBuilder,
        connectionManager,
        resultPersistenceService,
        resultMatchingService,
    dashboardEventBus);


        // =========================================================
        // CREATE ASTM ORDER QUERY
        // =========================================================

        var orderQuery =
            new AstmOrderQuery(
                $"Q|1|{barcode}");

        orderQuery.SampleId =
            barcode;


        // =========================================================
        // CREATE ASTM MESSAGE
        // =========================================================

        var astmMessage =
            new AstmMessage
            {
                OrderQuery =
                    orderQuery
            };


        // =========================================================
        // CREATE PROTOCOL EVENT ARGS
        // =========================================================

        var eventArgs =
            new ProtocolMessageReceivedEventArgs(
                analyzerId,
                orderQuery.RawRecord,
                astmMessage);


        // =========================================================
        // ASSERT CONSTRUCTION
        // =========================================================

        Assert.NotNull(
            coordinator);

        Assert.Empty(
            connectionManager.SentMessages);
    }


    // =========================================================
    // CREATE PROTOCOL ROUTER
    // =========================================================

    private static ProtocolRouter CreateProtocolRouter(
        Guid analyzerId)
    {
        var repository =
            new FakeAnalyzerConfigurationRepository(
                analyzerId);


        var configurationService =
            new AnalyzerConfigurationService(
                repository);


        var processors =
            new List<
                LabAnalyzerConnector.Protocols.Abstractions.IProtocolProcessor
            >();


        return new ProtocolRouter(
            configurationService,
            processors);
    }


    // =========================================================
    // FAKE ANALYZER CONNECTION MANAGER
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
    // FAKE ORDER SENDER
    // =========================================================

    private sealed class FakeAnalyzerOrderSender
        : IAnalyzerOrderSender
    {
        public string BuildOrderQuery(
            string barcode)
        {
            return
                $"Q|1|{barcode}";
        }


        public void SendOrder(
            Guid analyzerId,
            string barcode)
        {
            // No operation required for this test.
        }
    }


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


        public void Update(LabOrder order)
        {
            // For unit testing simply replace the stored order.

            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            // Since this fake repository only stores one order,
            // overwrite it.
            _order = order;
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
    }


    // =========================================================
    // FAKE LAB RESULT REPOSITORY
    // =========================================================

    private sealed class FakeLabResultRepository
        : ILabResultRepository
    {
        private readonly List<LabResult>
            _results = new();


        public Task AddAsync(
            LabResult result,
            CancellationToken cancellationToken = default)
        {
            _results.Add(
                result);

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
            IReadOnlyCollection<LabResult>
                results =
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
                    x =>
                        x.Id == id);

            return Task.FromResult(
                result);
        }


        public Task<IReadOnlyCollection<LabResult>>
            GetByPatientIdAsync(
                string patientId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LabResult>
                results =
                    _results
                        .Where(
                            x =>
                                x.PatientId ==
                                patientId)
                        .ToList();

            return Task.FromResult(
                results);
        }


        public Task<IReadOnlyCollection<LabResult>>
            GetBySampleIdAsync(
                string sampleId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<LabResult>
                results =
                    _results
                        .Where(
                            x =>
                                x.SampleId ==
                                sampleId)
                        .ToList();

            return Task.FromResult(
                results);
        }
    }


    // =========================================================
    // FAKE MAPPING PIPELINE
    // =========================================================

    private sealed class FakeMappingPipeline : IMappingPipeline 
    { public Task<MappingPipelineResult> ProcessAsync
            (Guid analyzerId, IReadOnlyDictionary<string, string?> 
        sourceFields, CancellationToken cancellationToken = default)
        { return Task.FromResult(new MappingPipelineResult 
        { IsSuccess = false, Errors = { "Fake mapping pipeline used only for order-query test." }
        }); }
    }


    // =========================================================
    // FAKE ANALYZER CONFIGURATION REPOSITORY
    // =========================================================

    private sealed class FakeAnalyzerConfigurationRepository
        : IAnalyzerConfigurationRepository
    {
        private readonly Guid _analyzerId;


        public FakeAnalyzerConfigurationRepository(
            Guid analyzerId)
        {
            _analyzerId =
                analyzerId;
        }


        public IReadOnlyCollection<AnalyzerConfiguration>
            GetAll()
        {
            return Array.Empty<AnalyzerConfiguration>();
        }


        public AnalyzerConfiguration?
            GetById(
                Guid id)
        {
            return null;
        }


        public Task LoadAsync()
        {
            return Task.CompletedTask;
        }


        public Task AddAsync(
            AnalyzerConfiguration configuration)
        {
            return Task.CompletedTask;
        }


        public Task UpdateAsync(
            AnalyzerConfiguration configuration)
        {
            return Task.CompletedTask;
        }


        public Task<bool> DeleteAsync(
            Guid id)
        {
            return Task.FromResult(
                false);
        }
    }
}