using System;
using System.Collections.Generic;

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

                

                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
        }
    }
}