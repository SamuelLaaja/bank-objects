using System;
using System.Linq;
using System.Collections.Generic;
using database_handling.Models;
using Microsoft.EntityFrameworkCore;

namespace database_handling
{
    public class AccountHandling
    {
        /// <summary>
        /// Lists all accounts and their information in all banks
        /// </summary>
        public static void ViewAllAccountsInAllBanks()
        {
            BankdbContext context = new BankdbContext();

            var banks = context.Bank.ToListAsync().Result;
            foreach (Models.Bank b in banks)
            {
                var accounts = context.BankAccount.Where(a => a.BankId == b.Id).ToListAsync().Result;
                foreach (BankAccount a in accounts)
                {
                    Console.WriteLine($"Bank ID: {a.BankId}  Customer ID: {a.CustomerId}");
                    Console.WriteLine($"Bank account IBAN number: {a.Iban.Trim()}");
                    Console.WriteLine($"Bank account name: {(a.Name != null ? a.Name.Trim() : "Unknown")} ");
                    Console.WriteLine($"Balance: {a.Balance}\n");
                }
            }

        }

        /// <summary>
        /// Lists all accounts and their information in one bank
        /// </summary>
        public static void ViewAllAccountsInOneBank(int bankId)
        {
            BankdbContext context = new BankdbContext();

            bool bankExists = context.Bank.Where(b => b.Id == bankId).Any();
            if (bankExists)
            {
                var accounts = context.BankAccount.Where(a => a.BankId == bankId).ToList();
                foreach (BankAccount a in accounts)
                {
                    Console.WriteLine($"Bank ID: {a.BankId}  Customer ID: {a.CustomerId}");
                    Console.WriteLine($"Bank account IBAN number: {a.Iban.Trim()}");
                    Console.WriteLine($"Bank account name: {(a.Name != null ? a.Name.Trim() : "Unknown")} ");
                    Console.WriteLine($"Balance: {a.Balance}\n");
                }
                if (accounts.Capacity == 0)
                    Console.WriteLine("No accounts in bank {0}.", bankId);
            }

        }

        /// <summary>
        /// Views single customer's accounts in all banks
        /// </summary>
        /// <param name="customerId"></param>
        public static void ViewAccountsOfOneCustomer(int customerId)
        {
            try
            {
                BankdbContext context = new BankdbContext();
                var customer = context.Customer.Where(c => c.Id == customerId).SingleOrDefault();
                if (customer != null)
                {
                    var accounts = customer.BankAccount.ToList();
                    foreach (BankAccount a in accounts)
                    {
                        Console.WriteLine($"Bank account ID: {a.BankId}  Bank account IBAN number: {a.Iban.Trim()}  Bank account name: {a.Name.Trim()}  Balance: {a.Balance}");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// Adds a new account for the customer
        /// </summary>
        public static void AddAccount(int customerId, int bankId, string iban, string accountName = null)
        {
            try
            {
                BankdbContext context = new BankdbContext();

                var customer = context.Customer.Where(c => c.Id == customerId).SingleOrDefault();
                var bank = context.Bank.Where(b => b.Id == bankId).SingleOrDefault();
                bool dublicateIban = context.BankAccount.Where(a => a.Iban == iban).Any();
                if (customer != null && bank != null && !dublicateIban)
                {
                    BankAccount newAccount = new BankAccount
                    {
                        Iban = iban,
                        Name = accountName,
                        BankId = bankId, //Bank id should be derived from iban number
                        CustomerId = customerId,
                        Balance = 0,
                    };

                    context.BankAccount.Add(newAccount);
                    context.SaveChanges();
                    Console.WriteLine($"Account {iban.Trim()} added for customer {customerId}.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }


        /// <summary>
        /// Removes account from customer
        /// </summary>
        public static void RemoveAccount(string iban)
        {
            BankdbContext context = new BankdbContext();
            BankAccount removeAccount = context.BankAccount.Where(a => a.Iban == iban).FirstOrDefault();
            if (removeAccount != null)
            {
                // First remove all transactions that are linked to the account
                List<BankAccountTransaction> transactions = context.BankAccountTransaction.Where(t => t.Iban == iban).ToList();
                foreach (BankAccountTransaction t in transactions)
                {
                    context.BankAccountTransaction.Remove(t);
                }
                context.SaveChanges();

                decimal bal = removeAccount.Balance;
                context.BankAccount.Remove(removeAccount);
                context.SaveChanges();
                Console.WriteLine($"Account number {iban.Trim()} removed. {bal} amount of money was lost.");
            }
        }

        /// <summary>
        /// Removes all accounts that are linked to the given bank
        /// </summary>
        public static void RemoveAllAccounts(int bankId) {
            BankdbContext context = new BankdbContext();
            List<BankAccount> accounts = context.BankAccount.Where(a => a.BankId == bankId).ToList();
            foreach (BankAccount a in accounts)
            {
                AccountHandling.RemoveAccount(a.Iban);
            }
            context.SaveChanges();
        }
    }
}
