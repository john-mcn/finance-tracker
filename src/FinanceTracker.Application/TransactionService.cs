using FinanceTracker.Domain;

namespace FinanceTracker.Application;

public class TransactionService
{
    private readonly List<Transaction> _transactions = [];

    public void AddTransaction(Transaction transaction) { _transactions.Add(transaction); }

    public IReadOnlyList<Transaction> GetTransactions() { return _transactions; }

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

    public decimal GetTotalIncome() { return GetAllIncomes().Sum(t => t.Amount); }

    public decimal GetTotalExpenses() { return GetAllExpenses().Sum(t => t.Amount); }

    public decimal GetBalance() { return GetTotalIncome() - GetTotalExpenses(); }
}