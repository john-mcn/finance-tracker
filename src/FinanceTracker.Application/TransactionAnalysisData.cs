namespace FinanceTracker.Application;
using FinanceTracker.Domain;

public sealed class TransactionAnalysisData(IEnumerable<Transaction> transactions)
{
    public IReadOnlyList<Transaction> Transactions { get; init; } = transactions.ToList().AsReadOnly();
    public int TransactionCount => Transactions.Count;

    // Transaction type-based analytics
    public IReadOnlyList<Transaction> Incomes =>
        Transactions.Where(t => t.Type == TransactionType.Income)
            .ToList().AsReadOnly();

    public IReadOnlyList<Transaction> Expenses =>
        Transactions.Where(t => t.Type == TransactionType.Expense)
            .ToList().AsReadOnly();

    public decimal TotalIncome => Incomes.Sum(t => t.Amount);
    public decimal TotalExpense => Expenses.Sum(t => t.Amount);
    public decimal TotalBalance => TotalIncome - TotalExpense;

    // Category-based analytics
    //NOTE same logic in TransactionService - extract into helper method?
    //NOTE issue with empty str for category - what to do w it, ignore = lose data 
    public IReadOnlyList<string> Categories => 
        Transactions.Select(t => t.Category.Trim().ToLower())
            .Distinct()
            // .Where(c => c.Length > 0)
            .Select(c => c.Length > 0 ? char.ToUpper(c[0]) + c[1..] : c)
            .ToList().AsReadOnly();

    public IReadOnlyDictionary<string, IReadOnlyList<Transaction>> TransactionsByCategory =>
        GroupByCategory(Transactions);
    
    public IReadOnlyDictionary<string, IReadOnlyList<Transaction>> IncomesByCategory =>
        GroupByCategory(Incomes);

    public IReadOnlyDictionary<string, decimal> TotalIncomeByCategory =>
        IncomesByCategory.ToDictionary(
            x => x.Key,
            x => x.Value.Sum(t => t.Amount)
        );

    public IReadOnlyList<KeyValuePair<string, decimal>> Top3IncomesByCategory =>
        TopNCategoriesByTotal(TotalIncomeByCategory, 3);

    public IReadOnlyDictionary<string, IReadOnlyList<Transaction>> ExpensesByCategory =>
        GroupByCategory(Expenses);

    public IReadOnlyDictionary<string, decimal> TotalExpenseByCategory =>
        ExpensesByCategory.ToDictionary(
            x => x.Key,
            x => x.Value.Sum(t => t.Amount)
        );

    public IReadOnlyList<KeyValuePair<string, decimal>> Top3ExpensesByCategory =>
        TopNCategoriesByTotal(TotalExpenseByCategory, 3);
    
    // Month-based analytics
    public IReadOnlyDictionary<DateTime, IReadOnlyList<Transaction>> TransactionsByMonth =>
        Transactions
            .GroupBy(t => new DateTime(t.TransactionDate.Year, t.TransactionDate.Month, 1))
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<Transaction>)g.ToList().AsReadOnly()
            );

    public IReadOnlyDictionary<DateTime, decimal> TotalBalancePerMonth =>
        TransactionsByMonth.ToDictionary(
            g => g.Key,
            g => g.Value.Sum(t => t.Amount)
        );

    public IReadOnlyDictionary<DateTime, decimal> MeanBalancePerMonth =>
        TransactionsByMonth.ToDictionary(
            g => g.Key,
            g => g.Value.Sum(t => t.Amount) / g.Value.Count
        );

    // Helper methods
    //NOTE maybe extract if used in diff areas 
    public static IReadOnlyDictionary<string, IReadOnlyList<Transaction>> GroupByCategory(
        IEnumerable<Transaction> transactions
    ) {
        return transactions.GroupBy(t => t.Category.Trim().ToLower())
            .ToDictionary(
                x => (x.Key.Length > 0) ? char.ToUpper(x.Key[0]) + x.Key[1..] : x.Key,
                x => (IReadOnlyList<Transaction>) x.ToList().AsReadOnly()
            );
    }

    public static IReadOnlyList<KeyValuePair<string, decimal>> TopNCategoriesByTotal(
            IReadOnlyDictionary<string, decimal> src, int n
    ) {
        return src
            .OrderByDescending(x => x.Value)
            .Take(Math.Min(n, src.Count))
            .ToList()
            .AsReadOnly();
    }
}
