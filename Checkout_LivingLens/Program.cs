using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_LivingLens
{
    class Program
{
        static void Main(string[] args)
        {

            string userInput = "";
            Console.WriteLine("Type 'new' to start new scan or 'exit' to stop");

            while (userInput != "exit")
            {
                Console.Write("> ");
                userInput = Console.ReadLine().ToLower();

                switch (userInput)
                {
                    case "exit":
                        break;
                    case "new":
                        {
                            AddNewItem();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("\"{0}\" is not a recognized command.", userInput);
                            break;
                        }
                }

            }

        }
        static void AddNewItem()
        {
            var fakeDatabase = new FakeDatabase();
            var listItems = new List<ItemModel>();
            var checkout = new Checkout(fakeDatabase, listItems);
            Console.WriteLine("Type just the return button to stop");
            var scanner = "start";
            while (scanner != "")
            {
                Console.WriteLine("Add item>");
                scanner = Console.ReadLine();
                if (scanner != "")
                {
                    checkout.Scan(scanner);
                }
            }
            var totalPrice = checkout.Total();
            Console.WriteLine("The total is {0}", totalPrice);
        }
    }
}
