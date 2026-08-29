using FinanceTracker.Domain;

namespace FinanceTracker.Application;

public interface ITransactionRepository
{
    bool HasAny();

    IReadOnlyList<Transaction> GetAll();
    Transaction? GetById(long id);

    void Add(Transaction transaction);
    void AddAll(IEnumerable<Transaction> transactions);
}