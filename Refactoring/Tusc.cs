using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        public static void Start(List<User> users, List<Product> products)
        {
            bool validUser = false;
            bool validPassword = false;
            string userName = String.Empty;
            string password = String.Empty;
            double userBalance = 0;

            ShowInitialGreeting();

            Login:

            userName = UsernamePrompt();

            // Validate Username
            
            if (!string.IsNullOrEmpty(userName))
            {
                validUser = VerifyUserExists(users, userName, validUser);

                if (validUser)
                {
                    password = PasswordPrompt();
                    
                    validPassword = VerifyPassword(users, userName, password, validPassword);

                    if (validPassword)
                    {
                        ShowWelcomeMessage(userName);                        
                        
                        userBalance = ShowRemainingBalance(users, userName, password, userBalance);

                        PurchaseProduct(users, products, userName, password, userBalance);
                        return;
                    }
                    else
                    {
                        InvalidPasswordProvided();
                        goto Login;
                    }
                }
                else
                {
                    InvalidUserProvided();
                    goto Login;
                }
            }

            PreventConsoleFromClosing();
        }

        private static void PurchaseProduct(List<User> users, List<Product> products, string userName, string password, double userBalance)
        {
            while (true)
            {
                string answer;
                int productChosen;

                ShowProductList(products);
                PromptUserToChooseProduct(out answer, out productChosen);

                // Product count is exit option
                if (productChosen == products.Count)
                {
                    PrepareToClose(users, products, userName, password, userBalance);
                    return;
                }
                else
                {

                    int quantityToPurchase = PromptAmountToPurchase(products, userBalance, ref answer, productChosen);

                    if (isBalanceLessThanZero(products, userBalance, productChosen, quantityToPurchase))
                    {
                        InsufficientMoniesForPurchase();
                        continue;
                    }

                    if (isNotEnoughProductForOrder(products, productChosen, quantityToPurchase))
                    {
                        NotEnoughProductMessage(products, productChosen);
                        continue;
                    }

                    if (quantityToPurchase > 0)
                    {
                        FinalizeSale(products, userBalance, productChosen, quantityToPurchase);
                    }
                    else
                    {
                        UserCancelledPurchase();
                    }
                }
            }
        }

        private static void PreventConsoleFromClosing()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit");
            Console.ReadLine();
        }       

        private static void FinalizeSale(List<Product> products, double userBalance, int productChosen, int quantityRemaining)
        {
            userBalance = CalculateUserBalance(products, userBalance, productChosen, quantityRemaining);
            CalculateProductRemaining(products, productChosen, quantityRemaining);
            UserPurchaseSuccessMessage(products, userBalance, productChosen, quantityRemaining);           
        }

        private static int PromptAmountToPurchase(List<Product> products, double userBalance, ref string answer, int num)
        {
            Console.WriteLine();
            Console.WriteLine("You want to buy: " + products[num].Name);
            Console.WriteLine("Your balance is " + userBalance.ToString("C"));

            // Prompt for user input
            Console.WriteLine("Enter amount to purchase:");
            answer = Console.ReadLine();
            int qty = Convert.ToInt32(answer);
            return qty;
        }

        private static void UpdateBalance(List<User> usrs, string userName, string pwd, double userBalance)
        {
            foreach (var usr in usrs)
            {
                // Check that name and password match
                if (usr.Name == userName && usr.Pwd == pwd)
                {
                    usr.Bal = userBalance;
                }
            }
        }

        private static void PromptUserToChooseProduct(out string answer, out int productChosen)
        {
            Console.WriteLine("Enter a number:");
            answer = Console.ReadLine();
            productChosen = Convert.ToInt32(answer);
            //subtract 1 for zero based index
            productChosen = productChosen - 1; 
        }

        private static void ShowProductList(List<Product> prods)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to buy?");
            for (int i = 0; i < 7; i++)
            {
                Product prod = prods[i];
                Console.WriteLine(i + 1 + ": " + prod.Name + " (" + prod.Price.ToString("C") + ")");
            }
            Console.WriteLine(prods.Count + 1 + ": Exit");
        }

        private static double ShowRemainingBalance(List<User> usrs, string userName, string pwd, double bal)
        {
            for (int i = 0; i < 5; i++)
            {
                User usr = usrs[i];

                // Check that name and password match
                if (usr.Name == userName && usr.Pwd == pwd)
                {
                    bal = usr.Bal;

                    // Show balance 
                    Console.WriteLine();
                    Console.WriteLine("Your balance is " + usr.Bal.ToString("C"));
                }
            }
            return bal;
        }

        private static void ShowWelcomeMessage(string userName)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Login successful! Welcome " + userName + "!");
            Console.ResetColor();
        }

        private static bool VerifyPassword(List<User> usrs, string name, string pwd, bool valPwd)
        {
            for (int i = 0; i < 5; i++)
            {
                User user = usrs[i];

                // Check that name and password match
                if (user.Name == name && user.Pwd == pwd)
                {
                    valPwd = true;
                }
            }
            return valPwd;
        }

        private static string PasswordPrompt()
        {
            Console.WriteLine("Enter Password:");
            string password = Console.ReadLine();
            return password;
        }

        private static bool VerifyUserExists(List<User> users, string name, bool validuser)
        {
            for (int i = 0; i < 5; i++)
            {
                User user = users[i];
                // Check that name matches
                if (user.Name == name)
                {
                    validuser = true;
                }
            }
            return validuser;
        }

        private static string UsernamePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Enter Username:");
            string name = Console.ReadLine();
            return name;
        }

        private static void InsufficientMoniesForPurchase()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You do not have enough money to buy that.");
            Console.ResetColor();
        }

        private static void NotEnoughProductMessage(List<Product> prods, int num)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Sorry, " + prods[num].Name + " is out of stock");
            Console.ResetColor();
        }

        private static bool isNotEnoughProductForOrder(List<Product> prods, int productChosen, int quantityToPurchase)
        {
            return prods[productChosen].Qty <= quantityToPurchase;
        }

        private static bool isBalanceLessThanZero(List<Product> prods, double bal, int num, int qty)
        {
            return bal - prods[num].Price * qty < 0;
        }

        private static void UserPurchaseSuccessMessage(List<Product> prods, double bal, int num, int qty)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You bought " + qty + " " + prods[num].Name);
            Console.WriteLine("Your new balance is " + bal.ToString("C"));
            Console.ResetColor();
        }

        private static void CalculateProductRemaining(List<Product> prods, int num, int qty)
        {
             prods[num].Qty = prods[num].Qty - qty;
        }

        private static double CalculateUserBalance(List<Product> prods, double bal, int num, int qty)
        {
            bal = bal - prods[num].Price * qty;
            return bal;
        }

        private static void UserCancelledPurchase()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("Purchase cancelled");
            Console.ResetColor();
        }

        private static void InvalidUserProvided()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid user.");
            Console.ResetColor();
        }

        private static void InvalidPasswordProvided()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid password.");
            Console.ResetColor();
        }

        private static void ShowInitialGreeting()
        {
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");
        }

        private static void PrepareToClose(List<User> users, List<Product> products, string userName, string password, double userBalance)
        {
            // Update balance
            UpdateBalance(users, userName, password, userBalance);

            // Write out new balance
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(@"Data\Users.json", json);

            // Write out new quantities
            string json2 = JsonConvert.SerializeObject(products, Formatting.Indented);
            File.WriteAllText(@"Data\Products.json", json2);

            PreventConsoleFromClosing();
        }
    }
}
