using FinanceTracker.Domain;

namespace FinanceTracker.Application;

public class TransactionService
{
    private readonly List<Transaction> _transactions = [];

    public void AddTransaction(Transaction transaction) { _transactions.Add(transaction); }
    
    public void AddAllTransactions(IEnumerable<Transaction> transactions)
    {
        _transactions.AddRange(transactions);    
    }

    // ToList to create snapshot instead of reference to same list
    public IReadOnlyList<Transaction> GetTransactions()
    {
        return _transactions.ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetAllIncomes()
    {
        return _transactions
            .Where(t => t.Type == TransactionType.Income)
            .ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetAllExpenses()
    {
        return _transactions
            .Where(t => t.Type == TransactionType.Expense)
            .ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetAllByCategory(string category)
    {
        return _transactions
            .Where(t => t.Category.Trim().ToLower() == category.Trim().ToLower())
            .ToList().AsReadOnly();
    }

    public decimal GetTotalIncome() { return GetAllIncomes().Sum(t => t.Amount); }
    public decimal GetTotalExpenses() { return GetAllExpenses().Sum(t => t.Amount); }
    public decimal GetBalance() { return GetTotalIncome() - GetTotalExpenses(); }
}