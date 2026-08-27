using Xunit;
using FinanceTracker.Domain;
using FinanceTracker.Application;

namespace TransactionServiceTests;

public class TransactionServiceMethodsTests
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
        Assert.Contains(transaction, listAfter);
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

    // [Fact]
    // public void GetAllIncomes_ReturnsAllIncomeTransactions()
    // {
    //     TransactionService service = new TransactionService();
    //     IReadOnlyList<Transaction> expected = service.GetTransactions()
    //         .Where(t => t.Type == TransactionType.Expense)
    //         .ToList().AsReadOnly();
    //     IReadOnlyList<Transaction> actual = service.GetAllIncomes();
    //     // Assert exactly 1 (income) transaction contained
    //     Assert.Single(expected);
    //     Assert.Equal(expected, actual);
    // }
}
// Assert.Throws<ArgumentException>(() => TransactionTypeMethods.FromString("kitten"));