namespace FinanceTracker.Domain;

public class Transaction
{
    public decimal Amount { get; set; }
    public string Category {get; set; } = string.Empty;
    public string Description {get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}