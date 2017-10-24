using System;
using database_handling;

namespace bank_objects
{
    class Program
    {
        static void Main(string[] args)
        {
            //BankHandling.ViewBanks();
            int newBank = BankHandling.AddBank("New Bank", "BANKFIHH");
            //BankHandling.ViewBanks();
            if (newBank > 0)
            {
                BankHandling.ChangeBankInformation(newBank, "Modified Bank", "BANKFIHX");
                BankHandling.ViewBanks();

                //AccountHandling.ViewAllAccounts();
                int newCustomer = CustomerHandling.AddCustomer("Uusi", "Käyttäjä", newBank);
                if (newCustomer > 0)
                {
                    AccountHandling.AddAccount(newCustomer, newBank, "FI4250001510000028");
                    AccountHandling.AddAccount(newCustomer, newBank, "CF4250001510002228");
                    CustomerHandling.ChangeCustomerInformation(newCustomer, "Muokattu", "Käyttäjä", newBank);
                }
                BankHandling.RemoveBank(newBank);
                BankHandling.ViewBanks();
                
                AccountHandling.ViewAllAccountsInAllBanks();
            }
            Console.ReadKey();
        }
    }
}
