namespace FinanceTracker.Domain;

public class Transaction
{
    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => _amount = Math.Max(0, value);
    }

    public TransactionType Type { get; set; }

    private string _category = string.Empty;
    public string Category {
        get => _category;
        set => _category = ClipText(value, 10);
    }

    private string _description = string.Empty;
    public string Description {
        get => _description;
        set => _description = ClipText(value, 30);
    }

    public Transaction(decimal amount, TransactionType type, string category, string description)
    {
        Amount = amount;
        Category = category;
        Description = description;
        Type = type;
    }

    // Make into helper?
    // Return text up to and including character at limit index
    static string ClipText(string inpt, int charLimit) {
        if (inpt.Length > charLimit)
            Console.WriteLine("WARNING - Input text exceeds given character limit, text has been clipped.");
        return inpt[..(Math.Min(charLimit, inpt.Length))];
    }

    public override string ToString()
    {
        return $"Transaction{{amount:{_amount}, type:{Type}, category:{_category}, description:{_description}}}";
    }
}