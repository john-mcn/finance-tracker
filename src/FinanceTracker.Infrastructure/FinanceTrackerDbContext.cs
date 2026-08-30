using FinanceTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure;

public class FinanceTrackerDbContext : DbContext
{
    public FinanceTrackerDbContext(
        DbContextOptions<FinanceTrackerDbContext> options)
        : base(options)
    {}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecurringTransactionSrc>()
            .OwnsOne(x => x.Recurrence);
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();
}