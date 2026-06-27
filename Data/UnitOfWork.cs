using Microsoft.EntityFrameworkCore.Storage;
using ShopNext.Data;
using ShopNext.Repositories.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShopNextDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ShopNextDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();

        if (_transaction != null)
            await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}