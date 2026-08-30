namespace FinanceTracker.Domain;

public class RecurrenceRule
{
    // i.e. every 2 days, every 4 weeks
    //NOTE simple recurrence for now (i.e. no '2 times per week')
    private int _occurences;
    public int Occurences {
        get => _occurences;
        set {
            if (value <= 0) { throw new ArgumentException("Recurrency frequency must be at least 1"); }
            _occurences = value;
        }
    }
    public RecurrenceUnit Frequency { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public RecurrenceRule() {}

    public RecurrenceRule(int occurences, RecurrenceUnit frequency,
            DateTime? startDate = null, DateTime? endDate = null)
    {
        Occurences = occurences;
        Frequency = frequency;
        StartDate = startDate ?? DateTime.Now;
        EndDate = endDate;
    }
    public RecurrenceRule(RecurrenceUnit frequency, DateTime? startDate = null, DateTime? endDate = null)
        :this(1, frequency, startDate, endDate)
    {}

    public bool IsActive()
    {
        if (EndDate == null) { return true; }
        return DateTime.Now < EndDate;
    }
}

public enum RecurrenceUnit
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}