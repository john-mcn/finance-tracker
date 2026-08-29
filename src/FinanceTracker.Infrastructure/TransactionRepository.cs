using FinanceTracker.Application;
using FinanceTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure;

public class TransactionRepository : ITransactionRepository
{
    private readonly FinanceTrackerDbContext _context;

    public TransactionRepository(FinanceTrackerDbContext context) { _context = context; }

    public bool HasAny() { return _context.Transactions.Any(); }

    public Transaction? GetById(long id)
    {
        return _context.Transactions.FirstOrDefault(t => t.Id == id);
    }

    public IReadOnlyList<Transaction> GetAll()
    {
        return _context.Transactions
            .AsNoTracking()
            .ToList();
    }

    public void Add(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        _context.SaveChanges();
    }

    public void AddAll(IEnumerable<Transaction> transactions)
    {
        _context.Transactions.AddRange(transactions);
        _context.SaveChanges();
    }
}