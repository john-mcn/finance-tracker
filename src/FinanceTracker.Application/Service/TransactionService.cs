using FinanceTracker.Domain;

namespace FinanceTracker.Application;

public class TransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository) { _repository = repository; }

    public bool HasAny() { return _repository.HasAny(); }

    public Transaction? GetById(long id)
    {
        return _repository.GetById(id);
    }

    // ToList to create snapshot instead of reference to same list
    public IReadOnlyList<Transaction> GetTransactions()
    {
        return _repository.GetAll().ToList().AsReadOnly();
    }

    public void AddTransaction(Transaction transaction) { _repository.Add(transaction); }
    
    public void AddAllTransactions(IEnumerable<Transaction> transactions)
    {
        _repository.AddAll(transactions);    
    }

    public IReadOnlyList<Transaction> GetAllIncomes()
    {
        return _repository.GetAll()
            .Where(t => t.Type == TransactionType.Income)
            .ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetAllExpenses()
    {
        return _repository.GetAll()
            .Where(t => t.Type == TransactionType.Expense)
            .ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetByCategory(string category)
    {
        return _repository.GetAll()
            .Where(t => t.Category.Trim().ToLower() == category.Trim().ToLower())
            .ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetByDescriptionIncludes(string substring)
    {
        return _repository.GetAll()
            .Where((t) => t.Description.Contains(substring))
            .ToList().AsReadOnly();
    }

    //NOTE Ignores time component
    public IReadOnlyList<Transaction> GetByOnDate(DateTime date)
    {
        return _repository.GetAll()
            .Where((t) => t.TransactionDate.Date.CompareTo(date.Date) == 0)
            .ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetByBeforeDate(DateTime date)
    {
        return _repository.GetAll()
            .Where((t) => t.TransactionDate.Date < date.Date)
            .ToList().AsReadOnly();
    }

    public IReadOnlyList<Transaction> GetByAfterDate(DateTime date)
    {
        return _repository.GetAll()
            .Where((t) => t.TransactionDate.Date > date.Date)
            .ToList().AsReadOnly();
    }

    public decimal GetTotalIncome() { return GetAllIncomes().Sum(t => t.Amount); }
    public decimal GetTotalExpenses() { return GetAllExpenses().Sum(t => t.Amount); }
    public decimal GetBalance() { return GetTotalIncome() - GetTotalExpenses(); }

    // Get *distinct* categories, ignoring case
    //NOTE same logic in TransactionAnalysisData - extract into helper method?
    //NOTE issue with empty str for category - what to do w it, ignore = lose data 
    public List<string> GetCategories()
    {
        return _repository.GetAll()
            .Select(t => t.Category.Trim().ToLower())
            .Distinct()
            // .Where(c => c.Length > 0)
            .Select(c => c.Length > 0 ? char.ToUpper(c[0]) + c[1..] : c)
            .ToList();
    }
}