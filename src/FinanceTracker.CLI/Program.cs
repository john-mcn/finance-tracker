// Author: John McNally
// Entry file (top-level statements)

using System;
using FinanceTracker.Application;
using FinanceTracker.Domain;

var service = new TransactionService();
var running = true;

// Set up example transaction service
service.AddTransaction(new Transaction(2.50, TransactionType.Expense, "Groceries", ""));
service.AddTransaction(new Transaction(220M, TransactionType.Income, "", "freelance"));
service.AddTransaction(new Transaction(11.30, TransactionType.Expense, "Groceries", "weekly shop"));
service.AddTransaction(new Transaction(6M, TransactionType.Income, "", ""));

while (running)
{
    Console.WriteLine();
    Console.WriteLine(@"========== Finance Tracker ==========
0. Exit
1. View transactions
2. Add transaction
3. View balance"
    );
    // Console.WriteLine();
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "0":
        case "":
            // running = false;
            return;
        case "1":
            CLIMethods.HandleViewTransactions(service);
            break;
        case "2":
            CLIMethods.CreateTransaction(service);
            break;
        case "3":
            CLIMethods.ViewBalance(service.GetBalance());
            break;
        default:
            CLIMethods.PrintWarning($"Invalid option \"{choice}\"");
            break;
    }
}

static class CLIMethods
{
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

    public static void ViewBalance (decimal balance)
    {
        var sign = (balance < 0) ? "-" : "";
        Console.WriteLine($"Balance: {sign}£{balance:F2}");
    }

    public static void HandleViewTransactions(TransactionService service)
    {
        // Handle which transactions to view
        Console.WriteLine(@"Select which transactions to view:
0. Back
1. All
2. All Incomes
3. All Expenses
4. By Category"
        );
        var choice = Console.ReadLine() ?? "";
        IEnumerable<Transaction> transactions = [];
        switch (choice)
        {
            case "0":
            case "":
                return;
            case "1":
                transactions = service.GetTransactions();
                break;
            case "2":
                transactions = service.GetAllIncomes();
                break;
            case "3":
                transactions = service.GetAllExpenses();
                break;
            case "4":
                Console.Write("Enter category: ");
                var category = Console.ReadLine() ?? "";
                transactions = service.GetAllByCategory(category);
                break;
            default:
                PrintError($"Invalid choice: {choice}");
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
                CLIMethods.PrintError($"[ERROR] {e.Message}\n");
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
}