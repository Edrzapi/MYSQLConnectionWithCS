using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DatabaseConnectionTutorial.Utility
{
    public class GenericMenu<T> where T : class, new()
    {
        private readonly IService<T> _service;
        private readonly PropertyInfo _idProp;
        private readonly PropertyInfo[] _dataProps;

        public GenericMenu(IService<T> service)
        {
            _service = service;
            var props = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();

            // find the int Id
            _idProp = props
                .FirstOrDefault(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(int));
            if (_idProp == null)
                throw new InvalidOperationException($"{typeof(T).Name} must have an int Id property.");

            _dataProps = props.Where(p => p != _idProp).ToArray();
        }

        public void Show()
        {
            var typeName = typeof(T).Name;
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine($"==== {typeName} Management ====");
                Console.WriteLine("1) Create");
                Console.WriteLine("2) List All");
                Console.WriteLine("3) Update");
                Console.WriteLine("4) Delete");
                Console.WriteLine("0) Exit");
                Console.Write("Choice: ");

                switch (Console.ReadLine())
                {
                    case "1": Create(); break;
                    case "2": ListAll(); break;
                    case "3": Update(); break;
                    case "4": Delete(); break;
                    case "0": exit = true; break;
                    default:
                        Console.WriteLine("Invalid option.");
                        Pause();
                        break;
                }
            }
        }

        private void Create()
        {
            Console.Clear();
            Console.WriteLine($"-- Create new {typeof(T).Name} --");
            var entity = new T();
            foreach (var prop in _dataProps)
                PromptFor(prop, entity, isUpdate: false);

            _service.Create(entity);
            var newId = (int)_idProp.GetValue(entity);
            Console.WriteLine($"✔ Created with Id = {newId}");
            Pause();
        }

        private void ListAll()
        {
            Console.Clear();
            Console.WriteLine($"-- All {typeof(T).Name}s --");
            var items = _service.Read();
            if (!items.Any())
            {
                Console.WriteLine("<no records>");
            }
            else
            {
                foreach (var item in items)
                    Console.WriteLine(Format(item));
            }
            Pause();
        }

        private void Update()
        {
            Console.Clear();
            Console.WriteLine($"-- Update {typeof(T).Name} --");
            Console.Write("Enter Id: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Invalid Id.");
                Pause();
                return;
            }

            var entity = _service.Read(id);
            if (entity == null)
            {
                Console.WriteLine($"No {typeof(T).Name} with Id={id}");
                Pause();
                return;
            }

            foreach (var prop in _dataProps)
                PromptFor(prop, entity, isUpdate: true);

            _service.Update(entity);
            Console.WriteLine("✔ Updated.");
            Pause();
        }

        private void Delete()
        {
            Console.Clear();
            Console.WriteLine($"-- Delete {typeof(T).Name} --");
            Console.Write("Enter Id: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Invalid Id.");
                Pause();
                return;
            }

            Console.Write($"Are you sure you want to delete Id={id}? (y/N): ");
            if (Console.ReadLine().Trim().ToLowerInvariant() == "y")
            {
                _service.Delete(id);
                Console.WriteLine("✔ Deleted.");
            }
            else
            {
                Console.WriteLine("Canceled.");
            }
            Pause();
        }

        private void PromptFor(PropertyInfo prop, T entity, bool isUpdate)
        {
            var current = isUpdate ? prop.GetValue(entity)?.ToString() : null;
            Console.Write(prop.Name + (isUpdate ? $" (current: {current}, leave blank to keep)" : "") + ": ");
            var input = Console.ReadLine();
            if (isUpdate && string.IsNullOrWhiteSpace(input))
                return;

            object value = Convert.ChangeType(input, prop.PropertyType);
            prop.SetValue(entity, value);
        }

        private string Format(T entity)
        {
            var idVal = _idProp.GetValue(entity);
            var data = _dataProps
                .Select(p => $"{p.Name}={p.GetValue(entity)}")
                .Join(", ");
            return $"[{idVal}] {data}";
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue…");
            Console.ReadKey(intercept: true);
        }
    }

    // Extension for joining
    internal static class StringExtensions
    {
        public static string Join(this IEnumerable<string> items, string separator) =>
            string.Join(separator, items);
    }
}
