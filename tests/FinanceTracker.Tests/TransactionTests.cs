using Xunit;
using FinanceTracker.Domain;

namespace TransactionTests;

public class TransactionTypeMethodsTests
{
    [Fact]
    public void FromString_InputValidString_ReturnsValidEnum()
    {
        TransactionType typeIncome = TransactionTypeMethods.FromString("income");
        Assert.Equal(TransactionType.Income, typeIncome);
    }

    [Fact]
    public void FromString_HandlesCaseAndSpaces()
    {
        var strExport = "    exPEnSe  ";
        Assert.Equal(TransactionType.Expense, TransactionTypeMethods.FromString(strExport));
        Assert.Equal(TransactionType.Expense, TransactionTypeMethods.FromString(strExport.ToUpper()));
        Assert.Equal(TransactionType.Expense, TransactionTypeMethods.FromString(strExport.ToLower()));
    }

    [Fact]
    public void FromString_InputInvalidString_ThrowsError()
    {
        Assert.Throws<ArgumentException>(() => TransactionTypeMethods.FromString("kitten"));
    }
}

public class TransactionGetterSetterTests
{
    [Fact]
    public void SetAmount_InputLessThan0_ThrowsException()
    {
        Transaction transaction = new();
        Assert.Throws<ArgumentException>(() => transaction.Amount = -10);
    }

    [Fact]
    public void SetCategory_InputOverLimit_ThrowsException()
    {
        var category = new string('a', Transaction.CATEGORY_CHAR_LIMIT + 1);
        Transaction transaction = new();
        Assert.Throws<ArgumentException>(() => transaction.Category = category);
    }
    
    [Fact]
    public void SetDescription_InputOverLimit_IsClipped()
    {
        var description = new string('a', Transaction.DESCRIPTION_CHAR_LIMIT + 1);
        Transaction transaction = new();
        Assert.Throws<ArgumentException>(() => transaction.Description = description);
    }
}

// Constructor uses setters tested above
public class TransactionConstructorTests
{
    [Fact]
    public void CreateTransaction_ValidInputs_SetsCorrectly()
    {
        Transaction expected = new Transaction();
        expected.Amount = 2.50M;
        expected.Type = TransactionType.Income;
        expected.Category = "Category";
        expected.Description = "Description";
        Transaction actual = new(2.50, TransactionType.Income, "Category", "Description");
        Assert.Equal(expected, actual);
    }
}