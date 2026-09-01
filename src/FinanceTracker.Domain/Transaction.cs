namespace FinanceTracker.Domain;

// Transaction 'amount' can only be positive - income/expense stored in 'type'
public class Transaction: TransactionDefinition
{
    // Database ID
    public long Id { get; private set; }

    public DateTime TransactionDate { get; set; }
    // i.e. 01/09/2026 12:00
    public const string DATETIME_PATTERN = "MM-dd-yyyy H:mm";
    // i.e. 01 September 2026
    public const string DATE_PATTERN = "dd MMMM yyyy";
    // i.e. September 2026
    public const string DATEMONTH_PATTERN = "MMMM yyyy";

    public RecurringTransactionSrc? Recurrence { get; set; }

    public Transaction() {}

    public Transaction(
            decimal amount, TransactionType type, string category, string description,
            DateTime? transactionDate = null, RecurringTransactionSrc? recurrence = null
    )
    {
        Amount = amount;
        Category = category;
        Description = description;
        Type = type;
        if (transactionDate > DateTime.Now)
        {
            throw new ArgumentException("Creation date cannot be after now");
        }
        TransactionDate = transactionDate ?? DateTime.Now;
        Recurrence = recurrence;
    }

    public Transaction(
            double amount, TransactionType type, string category, string description,
            DateTime? transactionDate = null, RecurringTransactionSrc? recurrence = null
    ) : this((decimal)amount, type, category, description, transactionDate ?? DateTime.Now, recurrence) {}

    public override string ToString()
    {
        return $"Transaction{Id}{{amount:{Amount}, type:{Type}, dateTime:{TransactionDate}, category:{Category}, description:{Description}}}";
    }
    public string ToStringPretty()
    {
        var sign = (Type == TransactionType.Expense) ? "-" : "";
        return $"[{Id}: {sign}£{Amount:F2} ({Type}) {TransactionDate.ToString(DATETIME_PATTERN)}"
            + ((Category.Length > 0 || Description.Length > 0) ? " - " : "")
            + ((Category.Length > 0) ? $"\"{Category}\"" : "")
                + ((Category.Length > 0 && Description.Length > 0) ? ", " : "") 
            + ((Description.Length > 0) ? $"\"{Description}\"" : "")
            + "]";
    }

    public override bool Equals(object? obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        } else
        {
            if (this == obj) return true;
            Transaction t = (Transaction) obj;
            return Amount == t.Amount
                && Type == t.Type
                && Category == t.Category
                && Description == t.Description;
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(Amount, Type, Category, Description);

}