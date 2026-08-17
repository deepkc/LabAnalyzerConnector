using LabAnalyzerSimulator.Database.Entities;
using LabAnalyzerSimulator.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerSimulator.Repositories;

public sealed class SqlitePatientRepository
    : IPatientRepository
{
    private readonly IDbContextFactory<SimulatorDbContext>
        _dbContextFactory;

    public SqlitePatientRepository(
        IDbContextFactory<SimulatorDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<PatientEntity?> GetByBarcodeAsync(
        string barcode)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        return await db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Barcode == barcode);
    }

    public async Task<IReadOnlyCollection<PatientEntity>>
        GetAllAsync()
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        return await db.Patients
            .AsNoTracking()
            .OrderBy(x => x.PatientName)
            .ToListAsync();
    }

    public async Task AddAsync(
        PatientEntity patient)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        db.Patients.Add(patient);

        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        PatientEntity patient)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        db.Patients.Update(patient);

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Guid id)
    {
        using SimulatorDbContext db =
            _dbContextFactory.CreateDbContext();

        PatientEntity? entity =
            await db.Patients.FindAsync(id);

        if (entity == null)
            return;

        db.Patients.Remove(entity);

        await db.SaveChangesAsync();
    }
}