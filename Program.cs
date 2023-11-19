using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

// A classe representa um item no banco de dados.
[Serializable]
class Item
{
    public int Tag { get; set; }
    public string Value { get; set; }
}

// A classe representa um banco de dados simples.
class SimpleDatabase
{
    private List<Item> items;

    // Construtor que inicializa a lista de itens.
    public SimpleDatabase()
    {
        items = new List<Item>();
    }

    // Método para inserir um novo item no banco de dados.
    public void Insert(int tag, string value)
    {
        var newItem = new Item { Tag = tag, Value = value };
        items.Add(newItem);
        Console.WriteLine($"Inserted: {newItem.Tag}, {newItem.Value}");
    }

    // Método para remover um item do banco de dados com base na tag.
    public void Remove(int tag)
    {
        var itemToRemove = items.Find(item => item.Tag == tag);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            Console.WriteLine($"Removed: {itemToRemove.Tag}, {itemToRemove.Value}");
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }

    // Método para atualizar o valor de um item no banco de dados com base na tag.
    public void Update(int tag, string newValue)
    {
        var itemToUpdate = items.Find(item => item.Tag == tag);
        if (itemToUpdate != null)
        {
            itemToUpdate.Value = newValue;
            Console.WriteLine($"Updated: {itemToUpdate.Tag}, {itemToUpdate.Value}");
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }

    // Método para buscar um item no banco de dados com base na tag e imprimir o valor.
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

    // Método para salvar a lista de itens em um arquivo usando serialização.
    public void SaveToFile(string fileName)
    {
        try
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, items);
            }
            Console.WriteLine($"Database saved to file: {fileName}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error saving database to file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    // Método para carregar a lista de itens de um arquivo usando desserialização.
    public void LoadFromFile(string fileName)
    {
        try
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                items = (List<Item>)formatter.Deserialize(fileStream);
            }
            Console.WriteLine($"Database loaded from file: {fileName}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"File not found: {fileName}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error loading database from file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}

// Classe principal que contém o método Main.
class Program
{
    static void Main()
    {
        SimpleDatabase database = new SimpleDatabase();

        while (true)
        {
            Console.Write("Enter command (insert, remove, update, search, save, load, quit): ");
            string input = Console.ReadLine()?.ToLower();

            if (input == "quit")
            {
                break;
            }

            string[] tokens = input?.Split(' ');
            string command = tokens?[0];

            try
            {
                switch (command)
                {
                    case "insert":
                        if (tokens.Length >= 3)
                        {
                            int tag = int.Parse(tokens[1]);
                            string value = string.Join(" ", tokens, 2, tokens.Length - 2);
                            database.Insert(tag, value);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Format: insert <tag> <value>");
                        }
                        break;

                    case "remove":
                        if (tokens.Length == 2)
                        {
                            int tag = int.Parse(tokens[1]);
                            database.Remove(tag);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Format: remove <tag>");
                        }
                        break;

                    case "update":
                        if (tokens.Length >= 3)
                        {
                            int tag = int.Parse(tokens[1]);
                            string value = string.Join(" ", tokens, 2, tokens.Length - 2);
                            database.Update(tag, value);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Format: update <tag> <value>");
                        }
                        break;

                    case "search":
                        if (tokens.Length == 2)
                        {
                            int tag = int.Parse(tokens[1]);
                            database.Search(tag);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Format: search <tag>");
                        }
                        break;

                    case "save":
                        if (tokens.Length == 2)
                        {
                            string fileName = tokens[1];
                            database.SaveToFile(fileName);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Format: save <filename>");
                        }
                        break;

                    case "load":
                        if (tokens.Length == 2)
                        {
                            string fileName = tokens[1];
                            database.LoadFromFile(fileName);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Format: load <filename>");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid format. Please enter a valid number.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
