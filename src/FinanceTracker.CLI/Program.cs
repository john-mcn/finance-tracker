// Author: John McNally
// Entry file (top-level statements)

using System;
using FinanceTracker.Application;
using FinanceTracker.Domain;
using FinanceTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<FinanceTrackerDbContext>()
    .UseSqlite("Data Source=db/finance-tracker.db")
    .Options;

using var db = new FinanceTrackerDbContext(options);
db.Database.EnsureCreated();

var repository = new TransactionRepository(db);
var service = new TransactionService(repository);

// Set up example transaction service
SeedDemoData.Seed(service);

Console.OutputEncoding = System.Text.Encoding.UTF8;
CLIMethods.RunCLI(service);

static class CLIMethods
{
    public static void RunCLI(TransactionService service)
    {
        var running = true;
        while (running)
        {
            Console.WriteLine();
            var optionsStr = @"========== Finance Tracker ==========
0. Exit
1. View transaction
2. View transactions
3. Add transaction
4. View balance
5. View categories
6. View analytics";
            try {
                var choice = ChooseOption(optionsStr);
                switch (choice)
                {
                    case "0":
                    case "":
                        // running = false;
                        return;
                    case "1":
                        HandleViewTransaction(service);
                        break;
                    case "2":
                        HandleViewTransactions(service);
                        break;
                    case "3":
                        CreateTransaction(service);
                        break;
                    case "4":
                        PrintNotice(">> Loading balance\n");
                        ViewBalance(service.GetBalance());
                        break;
                    case "5":
                        ViewCategories(service);
                        break;
                    case "6":
                        ViewAnalytics(service.GetTransactions());
                        break;
                    default:
                        PrintWarning($"Invalid option \"{choice}\"");
                        break;
                }
            } catch(Exception e) { PrintError($"[ERROR] {e.Message}\n"); }
        }
    }

    public static void PrintError(string str)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(str);
        Console.ResetColor();
    }

    public static void PrintWarning(string str)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(str);
        Console.ResetColor();
    }

    public static void PrintNotice(string str)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(str);
        Console.ResetColor();
    }

    public static string ChooseOption(string options)
    {
        Console.WriteLine(options);
        return (Console.ReadLine() ?? "").Trim();
    }

    public static void ViewBalance (decimal balance)
    {
        var sign = (balance < 0) ? "-" : "";
        Console.WriteLine($"Balance: {sign}£{balance:F2}");
    }

    public static void HandleViewTransaction(TransactionService service)
    {
        var optionsStr = @"Select transactions by:
0. Back
1. ID";
        var choice = ChooseOption(optionsStr);
        Transaction? transaction = null;
        var noticeStr = ">> No output\n";
        switch (choice)
        {
            case "0":
            case "":
                return;
            case "1":
                Console.Write("Enter numerical ID: ");
                var idInpt = Console.ReadLine();
                long id = Convert.ToInt64(idInpt);
                transaction = service.GetById(id);
                noticeStr = $">> Found transaction with ID {id}\n";
                break;
            default:
                PrintError($"[ERROR] Invalid choice: {choice}\n");
                break;
        }
        
        if (transaction == null) {
            PrintWarning("No transaction found\n");
            return;
        }
        PrintNotice(noticeStr);
        Console.Write("Display as pretty? (y/n)");
        var inpt = Console.ReadLine() ?? "";
        var pretty = inpt.Contains('y');
        ShowTransaction(transaction, pretty: pretty);
    }

    public static void HandleViewTransactions(TransactionService service)
    {
        // Handle which transactions to view
        var optionsStr = @"Select which transactions to view:
0. Back
1. All
2. All Incomes
3. All Expenses
4. By Category
5. Description contains
6. Comparing date";
        var choice = ChooseOption(optionsStr);
        IEnumerable<Transaction> transactions = [];
        switch (choice)
        {
            case "0":
            case "":
                return;
            case "1":
                transactions = service.GetTransactions();
                PrintNotice($">> Found {transactions.Count()} transactions\n");
                break;
            case "2":
                transactions = service.GetAllIncomes();
                PrintNotice($">> Found {transactions.Count()} transactions of type Income\n");
                break;
            case "3":
                PrintNotice($">> Found {transactions.Count()} transactions of type Expense\n");
                transactions = service.GetAllExpenses();
                break;
            case "4":
                Console.Write("Enter category: ");
                var category = Console.ReadLine() ?? "";
                transactions = service.GetByCategory(category);
                PrintNotice($">> Found {transactions.Count()} transactions with category '{category}'\n");
                break;
            case "5":
                Console.Write("Enter substring: ");
                var substring = Console.ReadLine() ?? "";
                transactions = service.GetByDescriptionIncludes(substring);
                PrintNotice($">> Found {transactions.Count()} transactions with description containing '{substring}'\n");
                break;
            case "6":
                transactions = TransactionsByDateOptions(service);
                break;
            default:
                PrintError($"[ERROR] Invalid choice: {choice}");
                break;
        }
        
        // Handle how to display transactions
        Console.Write("Add optional combination of modifiers ('p' = pretty, 'n' = newline): ");
        var modifiers = Console.ReadLine() ?? "";
        var pretty = modifiers.Contains('p') || modifiers.Contains("pretty");
        var newLine = modifiers.Contains('n') || modifiers.Contains("newline");

        if (!transactions.Any())
        {
            PrintWarning("No transactions found\n");
            return;
        }
        ShowTransactions(transactions, pretty: pretty, newLine: newLine);
    }

    public static void ShowTransaction(Transaction transaction, bool pretty = false)
    {
        if (pretty)
        {
            Console.WriteLine(transaction.ToStringPretty());
        } else
        {
            Console.WriteLine(transaction);
        }
    }

    public static void ShowTransactions(IEnumerable<Transaction> transactions, bool pretty = false, bool newLine = false)
    {
        IEnumerable<string> transactionsStr = transactions.Select((t) => t.ToString());
        if (pretty)
        {
            transactionsStr = transactions.Select((t) => t.ToStringPretty());
        }
        if (newLine)
        {
            Console.WriteLine(string.Join("\n", transactionsStr));
        } else
        {
            Console.WriteLine(string.Join(", ", transactionsStr));
        }
    }

    public static IEnumerable<Transaction> TransactionsByDateOptions(TransactionService service)
    {
        Console.Write("Enter date (or 'now'): ");
        var dateChoice = Console.ReadLine() ?? "";
        DateTime dateTime;
        if (dateChoice.Trim().ToLower().Equals("now")) { dateTime = DateTime.Now; }
        else { dateTime = DateTime.Parse(dateChoice); }

        var dateCompareChoice = ChooseOption("Before (1), On (2), After (3): ");
        IEnumerable<Transaction> transactions = [];
        switch (dateCompareChoice)
        {
            case "":
            case "0":
                break;
            case "1":
                transactions = service.GetByBeforeDate(dateTime);
                PrintNotice($">> Found {transactions.Count()} transactions BEFORE {dateTime.ToString("dd-MM-yyyy")}\n");
                break;
            case "2":
                transactions = service.GetByOnDate(dateTime);
                PrintNotice($">> Found {transactions.Count()} transactions ON {dateTime.ToString("dd-MM-yyyy")}\n");
                break;
            case "3":
                transactions = service.GetByAfterDate(dateTime);
                PrintNotice($">> Found {transactions.Count()} transactions AFTER {dateTime.ToString("dd-MM-yyyy")}");
                break;
            default:
                PrintError($"[ERROR] Invalid choice: {dateCompareChoice}");
                break;
        }
        return transactions;
    }

    public static void CreateTransaction(TransactionService service)
    {
        var runningBuildTransaction = true;
        while (runningBuildTransaction)
        {
            try
            {
                Transaction transaction = CLIMethods.BuildTransaction();
                service.AddTransaction(transaction);
                runningBuildTransaction = false;
            } catch (Exception e)
            {
                PrintError($"[ERROR] {e.Message}\n");
                Console.Write("Enter any character to try again, or 'exit' to return to main menu ");
                var response = Console.ReadLine() ?? "exit";
                if (response.Equals("exit") || response.Equals(""))
                {
                    // runningBuildTransaction = false;
                    break;
                }
            }
        }
    }

    public static Transaction BuildTransaction()
    {
        Console.WriteLine("=== New Transaction ===");
        Console.Write("Amount (e.g. '2.35'): ");
        decimal amount = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Type (e.g. 'income' or 'expense'): ");
        TransactionType type = TransactionTypeMethods.FromString(Console.ReadLine() ?? "");
        Console.Write("Category: ");
        string category = Console.ReadLine() ?? "";
        Console.Write("Description: ");
        string description = Console.ReadLine() ?? "";
        return new Transaction(amount, type, category, description);
    }

    public static void ViewCategories(TransactionService service)
    {
        List<string> categories = service.GetCategories();
        PrintNotice($">> Found {categories.Count} categories\n");
        if (categories.Any(c => c.Length < 1))
        {
            PrintWarning("At least one category is empty\n");
        }
        Console.WriteLine(string.Join(", ", categories
            .Select(c => $"'{c}' ({service.GetByCategory(c).Count})")));
    }

    public static void ViewAnalytics(IEnumerable<Transaction> transactions)
    {
        TransactionAnalysisData analysisData = new(transactions);
        PrintNotice($">> Analysing {analysisData.TransactionCount} transactions\n");
        var balanceSign = analysisData.TotalBalance < 0 ? "-" : "";

        Console.WriteLine($"Total Balance = {balanceSign}£{Math.Abs(analysisData.TotalBalance):N2}");

        // Income analysis
        Console.WriteLine(
            $"\nIncomes ({analysisData.Incomes.Count}, totalling £{analysisData.TotalIncome:N2})"
        );
        Console.WriteLine(string.Join(
            Environment.NewLine,
            analysisData.TotalIncomeByCategory
                .Where(x => x.Value > 0)
                .Select(x => $" • {$"'{x.Key}'",-12} = £{x.Value:N2}")
        ));

        // Expense analysis
        Console.WriteLine(
            $"\nExpenses ({analysisData.Expenses.Count}, totalling -£{analysisData.TotalExpense:N2})"
        );
        Console.WriteLine(string.Join(
            Environment.NewLine,
            analysisData.TotalExpenseByCategory
                .Where(x => x.Value > 0)
                .Select(x => $" • {$"'{x.Key}'",-12} = -£{x.Value:N2}")
        ));

        // Category analysis
        Console.WriteLine($"\nCategory analysis ({analysisData.Categories.Count} categories)");
        if (analysisData.Categories.Any(c => c.Length < 1))
        {
            PrintWarning("At least one category is empty\n");
        }
        Console.WriteLine("  Top income categories");
        Console.WriteLine(string.Join(
            Environment.NewLine,
            analysisData.Top3IncomesByCategory
                .Select(x =>
                    $"   • {$"'{x.Key}'",-15} = £{Math.Abs(x.Value):N2}"
                )
        ));
        Console.WriteLine("  Top expense categories");
        Console.WriteLine(string.Join(
            Environment.NewLine,
            analysisData.Top3ExpensesByCategory
                .Select(x =>
                    $"   • {$"'{x.Key}'",-15} = -£{Math.Abs(x.Value):N2}"
                )
        ));

        // Month analysis
        Console.WriteLine("\nMonthly analysis");
        Console.WriteLine(string.Join(
            Environment.NewLine,
            analysisData.MeanBalancePerMonth
                .Select(x =>
                    $" • {x.Key.ToString(Transaction.DATEMONTH_PATTERN),-15} = {(x.Value < 0 ? "-" : "")}£{Math.Abs(x.Value):N2}"
                )
        ));

    }
}