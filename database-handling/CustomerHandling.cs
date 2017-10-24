using System;
using System.Linq;
using System.Collections.Generic;
using database_handling.Models;
using Microsoft.EntityFrameworkCore;

namespace database_handling
{
    public class CustomerHandling
    {
        //List all customers and their information in a single bank
        public static void ViewCustomersInBank(int bankId)
        {
            BankdbContext context = new BankdbContext();
            var bank = context.Bank.Where(b => b.Id == bankId).ToListAsync().Result;
            try
            {
                if (bank.Capacity > 0)
                {
                    var customers = bank.Single().Customer;
                    foreach (Customer c in customers)
                    {
                        Console.WriteLine($"Customer ID: {c.Id}  Customer name: {c.FirstName} {c.LastName}  Customer account number: {c.BankAccount}");
                    }
                    if (customers.Count == 0)
                        Console.WriteLine("No customers in bank {0}.", bankId);
                    Console.WriteLine();
                }
                else
                    Console.WriteLine("Bank with id {0} does not exist.", bankId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Returns all customers in a single bank as a List
        public static List<Customer> GetCustomersInBank(int bankId)
        {
            BankdbContext context = new BankdbContext();
            
            try
            {
                return context.Customer.Where(c => c.BankId == bankId).ToListAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        /// <summary>
        /// Adds a new customer and returns the ID of the customer
        /// </summary>
        public static int AddCustomer(string firstName, string lastName, int bankId)
        {
            BankdbContext context = new BankdbContext();

            Customer newCustomer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                BankId = bankId
            };
            
            context.Customer.Add(newCustomer);
            context.SaveChanges();

            //returns id of the just added customer
            return context.Customer.Where(c => c.FirstName == firstName && c.LastName == lastName && c.BankId == bankId)
                .ToListAsync().Result.LastOrDefault().Id;
        }



        /// <summary>
        /// Changes customer's information in the database with new values.
        /// 'customerId' declares which customer is going to be changed.
        /// </summary>
        public static void ChangeCustomerInformation(int customerId, string firstName, string lastName, int bankId)
        {
            BankdbContext context = new BankdbContext();
            Customer changeCustomer = context.Customer.Where(c => c.Id == customerId).FirstOrDefault();

            if (changeCustomer != null)
            {
                changeCustomer.FirstName = firstName;
                changeCustomer.LastName = lastName;
                changeCustomer.BankId = bankId;
                context.Customer.Update(changeCustomer);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes customer entity information
        /// </summary>
        public static void RemoveCustomer(int customerId)
        {
            BankdbContext context = new BankdbContext();
            Customer removeCustomer = context.Customer.Where(c => c.Id == customerId).FirstOrDefault();
            if (removeCustomer != null)
            {

                // First remove all accounts that are linked to the customer
                List<BankAccount> accounts = context.BankAccount.Where(a => a.CustomerId == customerId).ToListAsync().Result;
                foreach (BankAccount a in accounts)
                {
                    AccountHandling.RemoveAccount(a.Iban);
                }
                // Then remove customer
                context.Customer.Remove(removeCustomer);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes all customers
        /// </summary>
        public static void RemoveAllCustomers(int bankId)
        {
            BankdbContext context = new BankdbContext();

            // Then remove all customers of that bank
            List<Customer> customers = CustomerHandling.GetCustomersInBank(bankId);
            foreach (Customer c in customers)
            {
                CustomerHandling.RemoveCustomer(c.Id);
            }
            context.SaveChanges();
        }
    }
}
