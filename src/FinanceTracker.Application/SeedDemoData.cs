using FinanceTracker.Domain;

namespace FinanceTracker.Application;

public static class SeedDemoData
{
    public static void Seed(TransactionService service)
    {
        // Only seed if no existing data on startup
        if (service.HasAny()) { return; }

        var now = DateTime.Now;

        List<Transaction> transactionsToAdd = [
            new Transaction(2.50, TransactionType.Expense, "Groceries", ""),
            new Transaction(220M, TransactionType.Income, "", "freelance"),
            new Transaction(11.30, TransactionType.Expense, "Groceries", "weekly shop"),
            new Transaction(12M, TransactionType.Income, "Friends", "money payed back"),
            new Transaction(50.25, TransactionType.Income, "Other", "shopping for appliances")
        ];
        transactionsToAdd[0].TransactionDate = now.AddDays(-2);
        transactionsToAdd[1].TransactionDate = now.AddDays(-1);
        transactionsToAdd[4].TransactionDate = now.AddDays(2);

        service.AddAllTransactions(transactionsToAdd);
    }
}