using System;
using System.IO;
using System.Collections.Generic;

class EWallet
{
    private static string usersFile = "users.txt";
    private static Dictionary<string, User> users = new Dictionary<string, User>();
    private static User currentUser = null!; // Suppress null warning

    // User Class
    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public string BankAccount { get; set; }

        public User(string username, string password, decimal balance, string bankAccount)
        {
            Username = username;
            Password = password;
            Balance = balance;
            BankAccount = bankAccount;
        }
    }

    // Load Users from File (Fixed Parsing Errors)
    static void LoadUsers()
    {
        if (File.Exists(usersFile))
        {
            foreach (string line in File.ReadAllLines(usersFile))
            {
                var parts = line.Split(' ');

                if (parts.Length == 4 && decimal.TryParse(parts[2], out decimal balance))
                {
                    users[parts[0]] = new User(parts[0], parts[1], balance, parts[3]);
                }
                else
                {
                    Console.WriteLine($"Skipping invalid entry in users file: {line}");
                }
            }
        }
    }

    // Save Users to File text
    static void SaveUsers()
    {
        List<string> lines = new List<string>();
        foreach (var user in users.Values)
        {
            lines.Add($"{user.Username} {user.Password} {user.Balance} {user.BankAccount}");
        }
        File.WriteAllLines(usersFile, lines);
    }

    // User Registration
    static void Register()
    {
        Console.Write("Enter a new username: ");
        string username = Console.ReadLine()!;
        Console.Write("Enter a password: ");
        string password = Console.ReadLine()!;
        Console.Write("Enter your bank account number: ");
        string bankAccount = Console.ReadLine()!;

        if (users.ContainsKey(username))
        {
            Console.WriteLine("Username already exists!");
            return;
        }

        users[username] = new User(username, password, 0, bankAccount);
        SaveUsers();
        Console.WriteLine("Registration successful!");
    }

    // User Login
    static void Login()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine()!;
        Console.Write("Enter your password: ");
        string password = Console.ReadLine()!;

        if (users.ContainsKey(username) && users[username].Password == password)
        {
            currentUser = users[username];
            Console.WriteLine($"Login successful! Welcome, {username}.");
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }

    // Check Balance
    static void CheckBalance()
    {
        Console.WriteLine($"Your current balance: ${currentUser.Balance}");
    }

    // Deposit Money
    static void Deposit()
    {
        Console.Write("Enter amount to deposit: $");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
        {
            currentUser.Balance += amount;
            SaveUsers();
            Console.WriteLine($"Successfully deposited ${amount}. New Balance: ${currentUser.Balance}");
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }

    // Withdraw Money
    static void Withdraw()
    {
        Console.Write("Enter amount to withdraw: $");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0 && amount <= currentUser.Balance)
        {
            currentUser.Balance -= amount;
            SaveUsers();
            Console.WriteLine($"Successfully withdrew ${amount}. Remaining Balance: ${currentUser.Balance}");
        }
        else
        {
            Console.WriteLine("Invalid amount or insufficient funds.");
        }
    }

    // Internal Transfer (Wallet to Wallet)
    static void InternalTransfer()
    {
        Console.Write("Enter recipient's username: ");
        string recipient = Console.ReadLine()!;
        Console.Write("Enter amount to transfer: $");

        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && users.ContainsKey(recipient) && amount > 0 && amount <= currentUser.Balance)
        {
            currentUser.Balance -= amount;
            users[recipient].Balance += amount;
            SaveUsers();
            Console.WriteLine($"Successfully transferred ${amount} to {recipient}. New Balance: ${currentUser.Balance}");
        }
        else
        {
            Console.WriteLine("Invalid recipient or insufficient funds.");
        }
    }

    // External Transfer (To Bank)
    static void ExternalTransfer()
    {
        Console.Write("Enter bank account number: ");
        string bankAccount = Console.ReadLine()!;
        Console.Write("Enter amount to transfer: $");

        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0 && amount <= currentUser.Balance)
        {
            currentUser.Balance -= amount;
            SaveUsers();
            Console.WriteLine($"Successfully transferred ${amount} to bank account {bankAccount}. Remaining Balance: ${currentUser.Balance}");
        }
        else
        {
            Console.WriteLine("Invalid amount or insufficient funds.");
        }
    }

    // Main Menu
    static void Main()
    {
        LoadUsers();
        while (true)
        {
            Console.WriteLine("\n1. Register\n2. Login\n3. Exit");
            Console.Write("Choose an option: ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input! Please enter a number.");
                continue;
            }

            if (choice == 1)
            {
                Register();
            }
            else if (choice == 2)
            {
                Login();
                if (currentUser != null)
                {
                    while (true)
                    {
                        Console.WriteLine("\n1. Check Balance\n2. Deposit\n3. Withdraw\n4. Internal Transfer\n5. External Transfer\n6. Logout");
                        Console.Write("Choose an option: ");
                        if (!int.TryParse(Console.ReadLine(), out int userChoice))
                        {
                            Console.WriteLine("Invalid input! Please enter a number.");
                            continue;
                        }

                        if (userChoice == 1) CheckBalance();
                        else if (userChoice == 2) Deposit();
                        else if (userChoice == 3) Withdraw();
                        else if (userChoice == 4) InternalTransfer();
                        else if (userChoice == 5) ExternalTransfer();
                        else if (userChoice == 6) { currentUser = null!; break; }
                        else Console.WriteLine("Invalid option!");
                    }
                }
            }
            else if (choice == 3)
            {
                Console.WriteLine("Exiting program...");
                break;
            }
            else
            {
                Console.WriteLine("Invalid option! Try again.");
            }
        }
    }
}
