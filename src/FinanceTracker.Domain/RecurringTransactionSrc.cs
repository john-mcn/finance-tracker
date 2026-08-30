namespace FinanceTracker.Domain;

// Transaction 'amount' can only be positive - income/expense stored in 'type'
public class RecurringTransactionSrc: TransactionDefinition
{
    // Database ID
    public long Id { get; private set; }
    public RecurrenceRule Recurrence { get; set; }

    // Constructor for DB - ignore warning, allow null temporarily
    public RecurringTransactionSrc() { Recurrence = null!; }

    public RecurringTransactionSrc(
            decimal amount, TransactionType type, string category, string description,
            RecurrenceRule recurrence
    )
    {
        Amount = amount;
        Category = category;
        Description = description;
        Type = type;
        Recurrence = recurrence;
    }
}