using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.Domain;

public abstract class TransactionDefinition
{
    public const int CATEGORY_CHAR_LIMIT = 10;
    public const int DESCRIPTION_CHAR_LIMIT = 30;

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set
        {
            if (value < 0)
                throw new ArgumentException(
                    $"Amount must not be less than 0: {value}");

            _amount = value;
        }
    }

    public TransactionType Type { get; set; }

    private string _category = string.Empty;
    public string Category
    {
        get => _category;
        set
        {
            if (value.Length > CATEGORY_CHAR_LIMIT)
                throw new ArgumentException(
                    $"Category length must not exceed {CATEGORY_CHAR_LIMIT}");

            _category = value;
        }
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set
        {
            if (value.Length > DESCRIPTION_CHAR_LIMIT)
                throw new ArgumentException(
                    $"Description length must not exceed {DESCRIPTION_CHAR_LIMIT}");

            _description = value;
        }
    }
}
