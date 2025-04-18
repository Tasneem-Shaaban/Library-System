using System;
using System.Collections.Generic;
using System.Linq;

namespace LibrarySystem
{
    public class Book
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }

        public Book(int id, string name, int quantity)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
        }
    }

    public class User
    {
        public int Id { get; }
        public string Name { get; }
        public List<int> BorrowedBooks { get; }

        public User(int id, string name)
        {
            Id = id;
            Name = name;
            BorrowedBooks = new List<int>();
        }

        public void BorrowBook(int bookId)
        {
            if (!BorrowedBooks.Contains(bookId))
            {
                BorrowedBooks.Add(bookId);
            }
        }

        public void ReturnBook(int bookId)
        {
            BorrowedBooks.Remove(bookId);
        }
    }

    public static class LibraryOperations
    {
        public static void ListBooksByPrefix(List<Book> books, string prefix)
        {
            var filteredBooks = books.Where(book => book.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();

            if (filteredBooks.Any())
            {
                Console.WriteLine($"\nBooks with the prefix '{prefix}' :");
                foreach (var book in filteredBooks)
                {
                    Console.WriteLine($"ID: {book.Id}, Name: {book.Name}, Quantity: {book.Quantity}");
                }
            }
            else
            {
                Console.WriteLine("\nNo books found with the given prefix.");
            }
        }

        public static void ListBooksSorted(List<Book> books, bool sortById)
        {
            var sortedBooks = sortById ? books.OrderBy(b => b.Id).ToList() : books.OrderBy(b => b.Name).ToList();

            Console.WriteLine($"\nBooks sorted by {(sortById ? "ID" : "Name")}:");
            foreach (var book in sortedBooks)
            {
                Console.WriteLine($"ID: {book.Id}, Name: {book.Name}, Quantity: {book.Quantity}");
            }
        }

        public static void ListUsersByBook(List<User> users, List<Book> books, string bookName)
        {
            var book = books.FirstOrDefault(b => b.Name.Equals(bookName, StringComparison.OrdinalIgnoreCase));
            if (book == null)
            {
                Console.WriteLine($"\nNo book found with the name '{bookName}'.");
                return;
            }

            var borrowedUsers = users.Where(u => u.BorrowedBooks.Contains(book.Id)).ToList();

            if (borrowedUsers.Any())
            {
                Console.WriteLine($"\nUsers who borrowed the book '{bookName}':");
                foreach (var user in borrowedUsers)
                {
                    Console.WriteLine($"User ID: {user.Id}, Name: {user.Name}");
                }
            }
            else
            {
                Console.WriteLine($"\nNo users have borrowed the book '{bookName}'.");
            }
        }

        public static void ListAllUsers(List<User> users)
        {
            var sortedUsers = users.OrderBy(u => u.Id).ToList();
            Console.WriteLine("\nAll users with borrowed books:");
            foreach (var user in sortedUsers)
            {
                var borrowedBooks = user.BorrowedBooks.Count > 0 ? string.Join(", ", user.BorrowedBooks) : "No borrowed books";
                Console.WriteLine($"User ID: {user.Id}, Name: {user.Name}, Borrowed Books: {borrowedBooks}");
            }
        }
    }

    internal class Program
    {
        private static List<Book> books = new List<Book>();
        private static List<User> users = new List<User>();

        private static void Main()
        {
            bool running = true;
            int selectedIndex = 0;
            string[] menuItems = {"Add Book", "Add User", "Borrow Book", "Return Book","List Books Sorted", "List Books with Prefix", "List Users Who Borrowed a Book", "List All Users", "Exit"};

            while (running)
            {
                DisplayMenu(menuItems, selectedIndex);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex == 0) ? menuItems.Length - 1 : selectedIndex - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex == menuItems.Length - 1) ? 0 : selectedIndex + 1;
                        break;

                    case ConsoleKey.Enter:
                        Console.Clear();
                        HandleMenuSelection(selectedIndex);
                        if (selectedIndex != menuItems.Length - 1)
                        {
                            Console.WriteLine("\n\nPress any key to return to the menu...");
                            Console.ReadKey();
                        }
                        else
                        {
                            running = false;
                        }
                        break;
                }
            }
        }

        private static void DisplayMenu(string[] menuItems, int selectedIndex)
        {
            Console.Clear();
            string title = "Library System";
            Console.WriteLine(new string(' ', (Console.WindowWidth - title.Length) / 2) + title);
            Console.WriteLine("\n\n");

            for (int i = 0; i < menuItems.Length; i++)
            {
                string item = menuItems[i];
                int padding = (Console.WindowWidth - item.Length) / 2;

                Console.Write(new string(' ', padding));

                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.WriteLine($"  {item}  \n");
                Console.ResetColor();
            }
        }

        private static void HandleMenuSelection(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    AddBook();
                    break;
                case 1:
                    AddUser();
                    break;
                case 2:
                    BorrowBook();
                    break;
                case 3:
                    ReturnBook();
                    break;
                case 4:
                    ListBooksSorted();
                    break;
                case 5:
                    ListBooksByPrefix();
                    break;
                case 6:
                    ListUsersByBook();
                    break;
                case 7:
                    ListAllUsers();
                    break;
                case 8:
                    Console.WriteLine("Exiting the program...");
                    break;
            }
        }

        private static void AddBook()
        {
            try
            {
                Console.Write("Enter Book ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid input for Book ID. Please enter a valid number.");
                    return;
                }
                if (books.Any(b => b.Id == id))
                {
                    Console.WriteLine("A book with this ID already exists.");
                    return;
                }

                Console.Write("Enter Book Name: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Book name cannot be empty.");
                    return;
                }

                Console.Write("Enter Quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("Invalid input for Quantity. Please enter a valid number.");
                    return;
                }
                if (quantity < 0)
                {
                    Console.WriteLine("Quantity cannot be negative.");
                    return;
                }

                books.Add(new Book(id, name, quantity));
                Console.WriteLine("Book added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the book: {ex.Message}");
            }
        }


        private static void AddUser()
        {
            try
            {
                Console.Write("Enter User ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid input for User ID. Please enter a valid number.");
                    return;
                }

                if (users.Any(u => u.Id == id))
                {
                    Console.WriteLine("A user with this ID already exists.");
                    return;
                }

                Console.Write("Enter User Name: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("User name cannot be empty.");
                    return;
                }

                users.Add(new User(id, name));
                Console.WriteLine("User added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the user: {ex.Message}");
            }
        }


        private static void BorrowBook()
        {
            try
            {
                Console.Write("Enter User ID: ");
                if (!int.TryParse(Console.ReadLine(), out int userId))
                {
                    Console.WriteLine("Invalid User ID.");
                    return;
                }

                Console.Write("Enter Book ID: ");
                if (!int.TryParse(Console.ReadLine(), out int bookId))
                {
                    Console.WriteLine("Invalid Book ID.");
                    return;
                }

                var user = users.FirstOrDefault(u => u.Id == userId);
                var book = books.FirstOrDefault(b => b.Id == bookId);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }

                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                if (book.Quantity <= 0)
                {
                    Console.WriteLine("Book is not available.");
                    return;
                }

                user.BorrowBook(bookId);
                book.Quantity--;
                Console.WriteLine("Book borrowed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while borrowing the book: {ex.Message}");
            }
        }

        private static void ReturnBook()
        {
            try
            {
                Console.Write("Enter User ID: ");
                if (!int.TryParse(Console.ReadLine(), out int userId))
                {
                    Console.WriteLine("Invalid User ID.");
                    return;
                }

                Console.Write("Enter Book ID: ");
                if (!int.TryParse(Console.ReadLine(), out int bookId))
                {
                    Console.WriteLine("Invalid Book ID.");
                    return;
                }

                var user = users.FirstOrDefault(u => u.Id == userId);
                var book = books.FirstOrDefault(b => b.Id == bookId);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }

                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                if (!user.BorrowedBooks.Contains(bookId))
                {
                    Console.WriteLine("This user hasn't borrowed this book.");
                    return;
                }

                user.ReturnBook(bookId);
                book.Quantity++;
                Console.WriteLine("Book returned successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while returning the book: {ex.Message}");
            }
        }

        private static void ListBooksSorted()
        {
            try
            {
                Console.Write("Sort by (1 = ID, 2 = Name): ");
                string choice = Console.ReadLine();

                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                    return;
                }

                bool sortById = choice == "1";
                LibraryOperations.ListBooksSorted(books, sortById);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while listing books: {ex.Message}");
            }
        }

        private static void ListBooksByPrefix()
        {
            try
            {
                Console.Write("Enter book prefix: ");
                string prefix = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(prefix))
                {
                    Console.WriteLine("Prefix cannot be empty.");
                    return;
                }

                LibraryOperations.ListBooksByPrefix(books, prefix);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while listing books by prefix: {ex.Message}");
            }
        }

        private static void ListUsersByBook()
        {
            try
            {
                Console.Write("Enter book name: ");
                string bookName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(bookName))
                {
                    Console.WriteLine("Book name cannot be empty.");
                    return;
                }

                LibraryOperations.ListUsersByBook(users, books, bookName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while listing users: {ex.Message}");
            }
        }

    }
}