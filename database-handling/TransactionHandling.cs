using System;
using System.Linq;
using System.Collections.Generic;
using database_handling.Models;
using Microsoft.EntityFrameworkCore;

namespace database_handling
{
    public class TransactionHandling
    {

        public static void ViewTransactionsOfOneCustomer(int customerId)
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
                        Console.WriteLine($"Transactions in account number {a.Iban}:");

                        List<BankAccountTransaction> transactions = context.BankAccountTransaction.ToListAsync().Result;
                        foreach (BankAccountTransaction t in transactions)
                        {
                            Console.WriteLine($"Transaction date: {t.TimeStamp}  Balance: {t.Amount}");
                        }
                        Console.WriteLine($"Current balance: {a.Balance}:");
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
        /// Adds a new transaction for the customer account and update account balance
        /// </summary>
        public static void MakeTransaction(string iban, float amount)
        {
            try
            {
                BankdbContext context = new BankdbContext();

                var account = context.BankAccount.Where(a => a.Iban == iban).SingleOrDefault();
                if (account != null)
                {
                    BankAccountTransaction newTransaction = new BankAccountTransaction
                    {
                        Iban = iban,
                        Amount = (decimal)amount,
                        TimeStamp = DateTime.Today
                    };
                    
                    account.Balance += (decimal)amount;

                    context.BankAccount.Update(account);
                    context.BankAccountTransaction.Add(newTransaction);
                    context.SaveChanges();
                    Console.WriteLine($"Transaction of the amount of {amount} done for account {iban}. Current balance is: {account.Balance}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        
    }
}
