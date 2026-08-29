namespace FinanceTracker.Domain;

// Transaction 'amount' can only be positive - income/expense stored in 'type'
public class Transaction
{
    // Database ID
    public long Id { get; private set; }

    public const int CATEGORY_CHAR_LIMIT = 10;
    public const int DESCRIPTION_CHAR_LIMIT = 30;

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set {
            if (value < 0) { throw new ArgumentException($"Amount must not be less than 0: {value}"); }
            else { _amount = value; }
        }
    }

    public TransactionType Type { get; set; }

    private string _category = string.Empty;
    public string Category {
        get => _category;
        set {
            if (value.Length > CATEGORY_CHAR_LIMIT)
            {
                throw new ArgumentException($"Category length must not exceed {CATEGORY_CHAR_LIMIT}");
            }
            else { _category = value; }
        }
    }

    private string _description = string.Empty;
    public string Description {
        get => _description;
        set {
            if (value.Length > DESCRIPTION_CHAR_LIMIT)
            {
                throw new ArgumentException($"Description length must not exceed {DESCRIPTION_CHAR_LIMIT}");
            }
            _description = value;
        }
    }

    public DateTime TransactionDate { get; set; }
    public const string DATETIME_PATTERN = "MM-dd-yyyy H:mm";

    public Transaction() {}

    public Transaction(
            decimal amount, TransactionType type, string category, string description,
            DateTime transactionDate
    )
    {
        Amount = amount;
        Category = category;
        Description = description;
        Type = type;
        if (transactionDate.CompareTo(DateTime.Now) == 1)
        {
            throw new ArgumentException("Creation date cannot be after now");
        }
        TransactionDate = transactionDate;
    }

    public Transaction(
            decimal amount, TransactionType type, string category, string description)
        : this(amount, type, category, description, DateTime.Now)
    {}

    public Transaction(
            double amount, TransactionType type, string category, string description)
        : this((decimal)amount, type, category, description)
    {}

    public override string ToString()
    {
        return $"Transaction{Id}{{amount:{_amount}, type:{Type}, dateTime:{TransactionDate}, category:{_category}, description:{_description}}}";
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