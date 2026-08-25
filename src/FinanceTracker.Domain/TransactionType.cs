namespace FinanceTracker.Domain;

public enum TransactionType
{
    Income,
    Expense
}

public static class TransactionTypeMethods
{
    public static TransactionType FromString(string str)
    {
        var sanitised = str.Trim();
        return Enum.Parse<TransactionType>(sanitised, ignoreCase: true);
    }
}