using FinanceTracker.Application;
using FinanceTracker.Domain;

namespace FinanceTracker.Tests;

public class MockTransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions = [];

    public bool HasAny() { return _transactions.Any(); }

    public Transaction? GetById(long id)
    {
        return _transactions.FirstOrDefault(t => t.Id == id);
    }

    public IReadOnlyList<Transaction> GetAll()
    {
        return _transactions.ToList().AsReadOnly();
    }

    public void Add(Transaction transaction)
    {
        _transactions.Add(transaction);
    }

    public void AddAll(IEnumerable<Transaction> transactions)
    {
        _transactions.AddRange(transactions);
    }
}