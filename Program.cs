using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
class Item
{
    public int Tag { get; set; }
    public string Value { get; set; }
}

class SimpleDatabase
{
    private List<Item> items;

    public SimpleDatabase()
    {
        items = new List<Item>();
    }

    public void Insert(int tag, string value)
    {
        var newItem = new Item { Tag = tag, Value = value };
        items.Add(newItem);
        Console.WriteLine("Inserted: " + newItem.Tag + ", " + newItem.Value);
    }

    public void Remove(int tag)
    {
        var itemToRemove = items.Find(item => item.Tag == tag);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            Console.WriteLine("Removed: " + itemToRemove.Tag + ", " + itemToRemove.Value);
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }

    public void Update(int tag, string newValue)
    {
        var itemToUpdate = items.Find(item => item.Tag == tag);
        if (itemToUpdate != null)
        {
            itemToUpdate.Value = newValue;
            Console.WriteLine("Updated: " + itemToUpdate.Tag + ", " + itemToUpdate.Value);
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }

    public void Search(int tag)
    {
        var itemToSearch = items.Find(item => item.Tag == tag);
        if (itemToSearch != null)
        {
            Console.WriteLine(itemToSearch.Value);
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }
    public void SaveToFile(string fileName)
    {
        try
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, items);
            }
            Console.WriteLine("Database saved to file: " + fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving database to file: " + ex.Message);
        }
    }

    public void LoadFromFile(string fileName)
    {
        try
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                items = (List<Item>)formatter.Deserialize(fileStream);
            }
            Console.WriteLine("Database loaded from file: " + fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading database from file: " + ex.Message);
        }
    }
}
class Program
{
    static void Main(string[] args)
    {
        SimpleDatabase database = new SimpleDatabase();

        while (true)
        {
            Console.Write("Enter command (insert, remove, update, search, save, load, quit): ");
            string input = Console.ReadLine().ToLower();

            if (input == "quit")
            {
                break;
            }

            string[] tokens = input.Split(' ');
            string command = tokens[0];

            switch (command)
            {
                case "insert":
                    if (tokens.Length >= 3)
                    {
                        int tag = int.Parse(tokens[1]);
                        string value = string.Join(" ", tokens, 2, tokens.Length - 2);
                        database.Insert(tag, value);
                    }
                    break;

                case "remove":
                    if (tokens.Length == 2)
                    {
                        int tag = int.Parse(tokens[1]);
                        database.Remove(tag);
                    }
                    break;

                case "update":
                    if (tokens.Length >= 3)
                    {
                        int tag = int.Parse(tokens[1]);
                        string value = string.Join(" ", tokens, 2, tokens.Length - 2);
                        database.Update(tag, value);
                    }
                    break;

                case "search":
                    if (tokens.Length == 2)
                    {
                        int tag = int.Parse(tokens[1]);
                        database.Search(tag);
                    }
                    break;

                case "save":
                    if (tokens.Length == 2)
                    {
                        string fileName = tokens[1];
                        database.SaveToFile(fileName);
                    }
                    break;

                case "load":
                    if (tokens.Length == 2)
                    {
                        string fileName = tokens[1];
                        database.LoadFromFile(fileName);
                    }
                    break;

                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
        }
    }
}