namespace FinanceTracker.Domain;

// Transaction 'amount' can only be positive - income/expense stored in 'type'
public class Transaction
{
    public const int CATEGORY_CHAR_LIMIT = 10;
    public const int DESCRIPTION_CHAR_LIMIT = 30;

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set {
            if (value <= 0) { throw new ArgumentException($"Amount must not be less than 0: {value}"); }
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
            else { _description = value; }
        }
    }

    public Transaction() {}

    public Transaction(decimal amount, TransactionType type, string category, string description)
    {
        Amount = amount;
        Category = category;
        Description = description;
        Type = type;
    }

    public Transaction(double amount, TransactionType type, string category, string description)
        : this((decimal)amount, type, category, description)
    {}

    // Make into helper?
    // Return text up to and including character at limit index
    static string ClipText(string inpt, int charLimit) {
        return inpt[..Math.Min(charLimit, inpt.Length)];
    }

    public override string ToString()
    {
        return $"Transaction{{amount:{_amount}, type:{Type}, category:{_category}, description:{_description}}}";
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