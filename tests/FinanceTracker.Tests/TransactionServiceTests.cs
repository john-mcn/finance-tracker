using Xunit;
using FinanceTracker.Domain;
using FinanceTracker.Application;

namespace TransactionServiceTests;

public class TransactionServiceSetupTests
{
    [Fact]
    public void AddTransaction_IsAddedToTransactionList()
    {
        TransactionService service = new TransactionService();
        IReadOnlyList<Transaction> listBefore = service.GetTransactions();
        Transaction transaction = new Transaction();
        service.AddTransaction(transaction);
        IReadOnlyList<Transaction> listAfter = service.GetTransactions();
        // Assert size is increased by 1 and new list contains new transaction
        Assert.Empty(listBefore);
        Assert.Single(listAfter);
        Assert.Equal(transaction, listAfter[0]);
    }

    [Fact]
    public void AddAllTransactions_AddsAllTransactionsCorrectly()
    {
        TransactionService service = new TransactionService();
        IReadOnlyList<Transaction> listBefore = service.GetTransactions();
        List<Transaction> transactions = [
            new Transaction(),
            new Transaction(),
            new Transaction()
        ];
        service.AddAllTransactions(transactions);
        IReadOnlyList<Transaction> listAfter = service.GetTransactions();
        // Assert size increased by n and new list contains new transactions
        Assert.Empty(listBefore);
        Assert.Equal(transactions.Count, listAfter.Count);
        Assert.False(listAfter.Except(transactions).Any()); // i.e. no elements in a that aren't in b
    }
}

public class TransactionServiceAccessTests {
    [Fact]
    public void GetAllIncomes_ReturnsAllIncomeTransactions()
    {
        TransactionService service = new TransactionService();
        service.AddAllTransactions([
            new Transaction(1.0, TransactionType.Income, "", ""),
            new Transaction(2.0, TransactionType.Income, "", ""),
            new Transaction(3.0, TransactionType.Income, "", "")
        ]);
        IReadOnlyList<Transaction> expected = service.GetTransactions()
            .Where(t => t.Type == TransactionType.Income)
            .ToList().AsReadOnly();
        IReadOnlyList<Transaction> actual = service.GetAllIncomes();
        // Assert exactly 1 (income) transaction contained
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetTotalIncome_ReturnsSumOfIncomeTransaction()
    {
        TransactionService service = new TransactionService();
        service.AddAllTransactions([
            new Transaction(1.0, TransactionType.Income, "", ""),
            new Transaction(2.0, TransactionType.Income, "", ""),
            new Transaction(3.0, TransactionType.Income, "", "")
        ]);
        Assert.Equal(6, service.GetTotalIncome());
    }

    [Fact]
    public void GetAllExpenses_ReturnsAllExpenseTransactions()
    {
        TransactionService service = new TransactionService();
        service.AddAllTransactions([
            new Transaction(1.0, TransactionType.Expense, "", ""),
            new Transaction(2.0, TransactionType.Expense, "", ""),
            new Transaction(3.0, TransactionType.Expense, "", "")
        ]);
        IReadOnlyList<Transaction> expected = service.GetTransactions()
            .Where(t => t.Type == TransactionType.Expense)
            .ToList().AsReadOnly();
        IReadOnlyList<Transaction> actual = service.GetAllExpenses();
        // Assert exactly 1 (income) transaction contained
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetTotalExpenses_ReturnsSumOfExpenseTransaction()
    {
        TransactionService service = new TransactionService();
        service.AddAllTransactions([
            new Transaction(1.0, TransactionType.Expense, "", ""),
            new Transaction(2.0, TransactionType.Expense, "", ""),
            new Transaction(3.0, TransactionType.Expense, "", "")
        ]);
        Assert.Equal(6, service.GetTotalExpenses());
    }

    [Fact]
    public void GetBalance_ReturnsBalance()
    {
        TransactionService service = new TransactionService();
        service.AddAllTransactions([
            new Transaction(10.0, TransactionType.Income, "", ""),
            new Transaction(5.0, TransactionType.Expense, "", ""),
        ]);
        Assert.Equal(5, service.GetBalance());

        service.AddTransaction(new Transaction(3.0, TransactionType.Expense, "", ""));
        Assert.Equal(2, service.GetBalance());
    }

    [Fact]
    public void GetAllByCategory_ReturnsAllMatchingTransactions()
    {
        var category = "Food";
        TransactionService service = new TransactionService();
        List<Transaction> categoryTransactions = [
            new Transaction(1M, TransactionType.Income, category, ""),
            new Transaction(3M, TransactionType.Income, category, "")
        ];
        var len = categoryTransactions.Count;
        categoryTransactions.Add(new Transaction(2M, TransactionType.Income, "", ""));
        service.AddAllTransactions(categoryTransactions);

        IReadOnlyList<Transaction> transactions = service.GetAllByCategory(category);
        Assert.Equal(len, transactions.Count);
        Assert.False(transactions.Except(categoryTransactions).Any()); // i.e. no elements in a that aren't in b
        Assert.Equal(len, transactions.Where(t => t.Category == category).ToList().Count);
    }

    [Fact]
    public void GetByDescriptionContains_ReturnsAllMatchingTransactions()
    {
        var substring = "cook";
        TransactionService service = new TransactionService();
        List<Transaction> descTransactions = [
            new Transaction(1M, TransactionType.Expense, "", "cooking supplies"),
            new Transaction(2M, TransactionType.Income, "", "kitchenware, cooking, etc.")
        ];
        var len = descTransactions.Count;
        descTransactions.Add(new Transaction(3M, TransactionType.Income, "", ""));
        service.AddAllTransactions(descTransactions);

        IReadOnlyList<Transaction> transactions = service.GetByDescriptionIncludes(substring);
        Assert.Equal(len, transactions.Count);
        Assert.False(transactions.Except(descTransactions).Any()); // i.e. no elements in a that aren't in b
        Assert.Equal(len, transactions.Where(t => t.Description.Contains(substring)).ToList().Count);
    }

    // [Fact]
    // public void GetByOnDate_ReturnsCorrectly()
    // {
    //     var dateTime = new DateTime(2026, 08, 29, 12, 0, 0);
    //     TransactionService service = new();
    //     Transaction transaction = new(10M, TransactionType.Income, "", "", dateTime);
    //     service.AddTransaction(transaction);
    //     IReadOnlyList<Transaction> transactionList = service.GetTransactions();

    //     Assert.Single(transactionList);
    //     Assert.Equal(transaction, transactionList[0]);
    // }

    // [Fact]
    // public void GetByBeforeDate_ReturnsCorrectly()
    // {
    //     var dateTime = new DateTime(2026, 08, 29, 12, 0, 0);
    //     TransactionService service = new();
    //     IReadOnlyList<Transaction> transactionsEarlier = [
    //         new(1M, TransactionType.Income, "", "", dateTime.AddDays(-2)),
    //         new(2M, TransactionType.Income, "", "", dateTime.AddDays(-1))
    //     ];
    //     service.AddAllTransactions(transactionsEarlier
    //         .Append(new(1M, TransactionType.Income, "", "", dateTime))
    //         .Append(new(2M, TransactionType.Income, "", "", dateTime)));
    //     IReadOnlyList<Transaction> transactionList = service.GetTransactions();

    //     Assert.Equal(transactionsEarlier.Count, transactionList.Count);
    //     Assert.False(transactionList.Except(transactionsEarlier).Any());
    // }
}