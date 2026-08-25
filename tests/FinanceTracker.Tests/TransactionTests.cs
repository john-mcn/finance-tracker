using Xunit;
using FinanceTracker.Domain;

namespace TransactionTests;

public class TransactionGetterSetterTests
{
    [Fact]
    public void SetAmount_InputLessThan0_Set0()
    {
        Transaction transaction = new((decimal) -10, TransactionType.Income, "", "");
        Assert.Equal(0, transaction.Amount);
    }

    [Fact]
    public void SetCategory_InputOverLimit_IsClipped()
    {
        var category = new string('a', 50);
        //NOTE Change char limit to match Transaction.Category
        var categoryStrLimit = 10;
        Transaction transaction = new();
        transaction.Category = category;
        Assert.Equal(categoryStrLimit, transaction.Category.Length);
    }
    
    [Fact]
    public void SetDescription_InputOverLimit_IsClipped()
    {
        var description = new string('a', 50);
        //NOTE Change char limit to match Transaction.Description
        var descriptionStrLimit = 30;
        Transaction transaction = new();
        transaction.Description = description;
        Assert.Equal(descriptionStrLimit, transaction.Description.Length);
    }
}

// Constructor uses setters tested above
public class TransactionConstructorTests
{
    [Fact]
    public void CreateTransaction_ValidInputs_Set0()
    {
        Transaction expected = new Transaction();
        expected.Amount = (decimal) 2.50;
        expected.Type = TransactionType.Income;
        expected.Category = "Category";
        expected.Description = "Description";
        Transaction actual = new((decimal) 2.50, TransactionType.Income, "Category", "Description");
        Assert.Equal(expected, actual);
    }
}