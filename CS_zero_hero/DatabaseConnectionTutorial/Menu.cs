using System;
using System.Collections.Generic;

namespace DatabaseConnectionTutorial
{
    public static class Menu
    {
        private static readonly UserService userService = new UserService("tutorial");

        public static void Show()
        {
            while (true)
            {
                Console.WriteLine(GetOptions());
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter first name: ");
                        string firstName = Console.ReadLine();
                        Console.Write("Enter last name: ");
                        string lastName = Console.ReadLine();
                        userService.Create(firstName, lastName);
                        Console.WriteLine("User created.");
                        break;

                    case "2":
                        var users = userService.ReadAll();
                        foreach (var user in users)
                            Console.WriteLine($"{user.userId}: {user.firstName} {user.lastName}");
                        break;

                    case "3":
                        Console.Write("Enter ID to update: ");
                        if (int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            Console.Write("New first name: ");
                            string newFirstName = Console.ReadLine();
                            Console.Write("New last name: ");
                            string newLastName = Console.ReadLine();
                            userService.Update(updateId, newFirstName, newLastName);
                            Console.WriteLine("User updated.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;

                    case "4":
                        Console.Write("Enter ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            userService.Delete(deleteId);
                            Console.WriteLine("User deleted.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private static string GetOptions()
        {
            return @"
==== User Management ====
1 - Create
2 - Read All
3 - Update
4 - Delete
0 - Exit
Choose an option:";
        }
    }
}
