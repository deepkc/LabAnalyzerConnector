using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class SqliteOrderRepository
    : IOrderRepository
{
    private readonly IDbContextFactory<LabAnalyzerDbContext>
        _dbContextFactory;


    public SqliteOrderRepository(
        IDbContextFactory<LabAnalyzerDbContext> dbContextFactory)
    {
        _dbContextFactory =
            dbContextFactory
            ?? throw new ArgumentNullException(
                nameof(dbContextFactory));
    }


    // =========================================================
    // ADD ORDER
    // =========================================================

    public void Add(
        LabOrder order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(
                nameof(order));
        }

        if (string.IsNullOrWhiteSpace(
                order.Barcode))
        {
            throw new ArgumentException(
                "Order barcode cannot be empty.",
                nameof(order));
        }


        using LabAnalyzerDbContext db =
            _dbContextFactory.CreateDbContext();


        LabOrderEntity entity =
     new LabOrderEntity
     {
         Id =
             order.Id,

         AnalyzerId =
             order.AnalyzerId,

         OrderId =
             order.OrderId,

         PatientId =
             order.PatientId,

         PatientName =
             order.PatientName,

         SpecimenId =
             order.SpecimenId,

         Barcode =
             order.Barcode,

         OrderedTests =
             string.Join(
                 "|",
                 order.OrderedTests),

         Priority =
             order.Priority,

         CreatedAt =
             order.CreatedAt,

         Status =
             order.Status
     };


        db.LabOrders.Add(
            entity);

        db.SaveChanges();
    }


    // =========================================================
    // FIND ORDER BY BARCODE
    // =========================================================

    public LabOrder? GetByBarcode(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(
                barcode))
        {
            return null;
        }


        using LabAnalyzerDbContext db =
            _dbContextFactory.CreateDbContext();


        string normalizedBarcode =
            barcode.Trim();


        LabOrderEntity? entity =
            db.LabOrders
                .AsNoTracking()
                .FirstOrDefault(
                    order =>
                        order.Barcode ==
                        normalizedBarcode);


        if (entity is null)
        {
            return null;
        }


        return MapToLabOrder(
            entity);
    }


    // =========================================================
    // GET ALL ORDERS
    // =========================================================

    public IReadOnlyCollection<LabOrder>
        GetAll()
    {
        using LabAnalyzerDbContext db =
            _dbContextFactory.CreateDbContext();


        return db.LabOrders
            .AsNoTracking()
            .OrderByDescending(
                order =>
                    order.CreatedAt)
            .ToList()
            .Select(
                MapToLabOrder)
            .ToList();
    }


    // =========================================================
    // REMOVE ORDER
    // =========================================================

    public bool Remove(
        Guid orderId)
    {
        using LabAnalyzerDbContext db =
            _dbContextFactory.CreateDbContext();


        LabOrderEntity? entity =
            db.LabOrders
                .FirstOrDefault(
                    order =>
                        order.Id ==
                        orderId);


        if (entity is null)
        {
            return false;
        }


        db.LabOrders.Remove(
            entity);

        db.SaveChanges();

        return true;
    }


    // =========================================================
    // MAP ENTITY TO DOMAIN MODEL
    // =========================================================

    private static LabOrder MapToLabOrder(
        LabOrderEntity entity)
    {
        List<string> tests =
            string.IsNullOrWhiteSpace(
                entity.OrderedTests)

                ? new List<string>()

                : entity.OrderedTests
                    .Split(
                        '|',
                        StringSplitOptions.RemoveEmptyEntries)
                    .Select(
                        test =>
                            test.Trim())
                    .ToList();


        return new LabOrder
        {
            Id =
        entity.Id,

            AnalyzerId =
        entity.AnalyzerId,

            OrderId =
        entity.OrderId,

            PatientId =
        entity.PatientId,

            PatientName =
        entity.PatientName,

            SpecimenId =
        entity.SpecimenId,

            Barcode =
        entity.Barcode,

            OrderedTests =
        tests,

            Priority =
        entity.Priority,

            Status =
        entity.Status,

            // CreatedAt is init-only and cannot currently
            // be restored by this object initializer.
        };
    }

    public void Update(LabOrder order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        using LabAnalyzerDbContext db =
            _dbContextFactory.CreateDbContext();

        LabOrderEntity? entity =
            db.LabOrders.FirstOrDefault(
                x => x.Id == order.Id);

        if (entity is null)
        {
            throw new InvalidOperationException(
                $"Order '{order.Id}' was not found.");
        }

        entity.AnalyzerId = order.AnalyzerId;
        entity.OrderId = order.OrderId;
        entity.PatientId = order.PatientId;
        entity.PatientName = order.PatientName;
        entity.SpecimenId = order.SpecimenId;
        entity.Barcode = order.Barcode;
        entity.OrderedTests = string.Join("|", order.OrderedTests);
        entity.Priority = order.Priority;
        entity.Status = order.Status;

        db.SaveChanges();
    }
}