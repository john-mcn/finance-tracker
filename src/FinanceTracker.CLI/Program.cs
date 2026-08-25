// Author: John McNally
// Entry file (top-level statements)

using System;
using FinanceTracker.Application;
using FinanceTracker.Domain;

var service = new TransactionService();
var running = true;

// Set up example transaction service
service.AddTransaction(new Transaction((decimal) 2.50, TransactionType.Expense, "Groceries", ""));
service.AddTransaction(new Transaction(22.0, TransactionType.Income, "Comission", ""));

while (running)
{
    Console.WriteLine();
    Console.WriteLine(@"========== Finance Tracker ==========
1. View transactions
2. Add transaction
3. View balance
4. Exit"
    );
    // Console.WriteLine();
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine(string.Join(", ", service.GetTransactions()));
            break;
        case "2":
            Console.WriteLine("=== New Transaction ===");
            Console.Write("Amount (e.g. '2.35'): ");
            decimal amount = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Type (e.g. 'income' or 'expense'): ");
            TransactionType type = TransactionTypeMethods.FromString(Console.ReadLine() ?? "");
            Console.Write("Category: ");
            string category = Console.ReadLine() ?? "";
            Console.Write("Description: ");
            string description = Console.ReadLine() ?? "";
            service.AddTransaction(new Transaction(amount, type, category, description));
            break;
        case "3":
            Console.WriteLine($"Balance: £{service.GetBalance():F2}");
            break;
        case "4":
        case "":
            // running = false;
            return;
        default:
            Console.WriteLine($"Invalid option \"{choice}\"");
            break;
    }
}