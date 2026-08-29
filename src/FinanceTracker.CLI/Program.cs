// Author: John McNally
// Entry file (top-level statements)

using System;
using FinanceTracker.Application;
using FinanceTracker.Domain;

var service = new TransactionService();

// Set up example transaction service
DateTime now = DateTime.Now;
service.AddTransaction(new Transaction(2.50, TransactionType.Expense, "Groceries", ""));
    service.GetTransactions()[0].CreatedAt = now.AddDays(-2);
service.AddTransaction(new Transaction(220M, TransactionType.Income, "", "freelance"));
    service.GetTransactions()[1].CreatedAt = now.AddDays(-1);
service.AddTransaction(new Transaction(11.30, TransactionType.Expense, "Groceries", "weekly shop"));
service.AddTransaction(new Transaction(12M, TransactionType.Income, "Friends", "money payed back"));
service.AddTransaction(new Transaction(50.25, TransactionType.Income, "Other", "shopping for appliances"));
    service.GetTransactions()[4].CreatedAt = now.AddDays(2);

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
1. View transactions
2. Add transaction
3. View balance
4. View categories";
            try {
                var choice = ChooseOption(optionsStr);
                switch (choice)
                {
                    case "0":
                    case "":
                        // running = false;
                        return;
                    case "1":
                        HandleViewTransactions(service);
                        break;
                    case "2":
                        CreateTransaction(service);
                        break;
                    case "3":
                        PrintNotice(">> Loading balance\n");
                        ViewBalance(service.GetBalance());
                        break;
                    case "4":
                        ViewCategories(service);
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
                PrintNotice($">> Loading all transactions\n");
                break;
            case "2":
                transactions = service.GetAllIncomes();
                PrintNotice($">> Loading transactions of type Income\n");
                break;
            case "3":
                PrintNotice($">> Loading transactions of type Expense\n");
                transactions = service.GetAllExpenses();
                break;
            case "4":
                Console.Write("Enter category: ");
                var category = Console.ReadLine() ?? "";
                PrintNotice($">> Loading transactions with category '{category}'\n");
                transactions = service.GetByCategory(category);
                break;
            case "5":
                Console.Write("Enter substring: ");
                var substring = Console.ReadLine() ?? "";
                PrintNotice($">> Loading transactions with description containing '{substring}'\n");
                transactions = service.GetByDescriptionIncludes(substring);
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
        switch (dateCompareChoice)
        {
            case "":
            case "0":
                return [];
            case "1":
                PrintNotice($">> Loading transactions BEFORE {dateTime.ToString("dd-MM-yyyy")}\n");
                return service.GetByBeforeDate(dateTime);
            case "2":
                PrintNotice($">> Loading transactions ON {dateTime.ToString("dd-MM-yyyy")}\n");
                return service.GetByOnDate(dateTime);
            case "3":
                PrintNotice($">> Loading transactions AFTER {dateTime.ToString("dd-MM-yyyy")}");
                return service.GetByAfterDate(dateTime);
            default:
                PrintError($"[ERROR] Invalid choice: {dateCompareChoice}");
                return [];
        }
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
        PrintNotice(">> Loading categories and their occurences\n");
        List<string> categories = service.GetCategories();
        Console.WriteLine(string.Join(", ", categories
            .Select(c => $"{c} ({service.GetByCategory(c).Count})")));
    }
}