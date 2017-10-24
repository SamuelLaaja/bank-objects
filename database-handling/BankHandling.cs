using System;
using System.Linq;
using database_handling.Models;
using Microsoft.EntityFrameworkCore;

namespace database_handling
{
    public class BankHandling 
    {
        
        public static void ViewBanks() {
            BankdbContext context = new BankdbContext();

            var banks = context.Bank.ToListAsync().Result;

            foreach (Models.Bank b in banks) {
                Console.WriteLine($"Bank ID: {b.Id}  \tBank name: {b.Name.Trim()}  \tBank BIC: {b.Bic}");
            }
            Console.WriteLine();
        }

        // Add new Bank entity/tietue
        public static int AddBank(string name, string bic)
        {
            BankdbContext context = new BankdbContext();

            Models.Bank newBank = new Models.Bank
            {
                Name = name,
                Bic = bic
            };

            context.Bank.Add(newBank);
            context.SaveChanges();
            
            //returns id of the just added bank
            return context.Bank.Where(b => b.Name == name && b.Bic == bic)
                .ToListAsync().Result.LastOrDefault().Id;            
        }


        // Update Bank entity information
        public static void ChangeBankInformation(int bankId, string name, string bic)
        {
            BankdbContext context = new BankdbContext();
            Models.Bank changeBank = context.Bank.Where(b => b.Id == bankId).SingleOrDefault();

            if (changeBank != null)
            {
                changeBank.Name = name;
                changeBank.Bic = bic;
                context.Bank.Update(changeBank);
                context.SaveChanges();
            }

        }


        // Remove Bank entity information
        public static void RemoveBank(int bankId)
        {
            BankdbContext context = new BankdbContext();
            Models.Bank removeBank = context.Bank.Where(b => b.Id == bankId).SingleOrDefault();
            if (removeBank != null)
            {
                AccountHandling.RemoveAllAccounts(bankId);
                AccountHandling.ViewAllAccountsInOneBank(bankId);
                CustomerHandling.RemoveAllCustomers(bankId);
                CustomerHandling.ViewCustomersInBank(bankId);

                // Then remove the bank itself
                context.Bank.Remove(removeBank);
                context.SaveChanges();
                Console.WriteLine("Removed bank number {0}!\n", bankId);
            }
        }
    }
}
