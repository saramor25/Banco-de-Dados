using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

[Serializable]
class Item
{
    public int Tag { get; set; }
    public string Value { get; set; }
}

// Interface para as estratégias
interface IStrategy
{
    void ProcessRequest(Request request, NamedPipeServerStream pipeServer);
}

// Estratégia FIFO (PersonFIFO)
class PersonFIFO : IStrategy
{
    private const int MaxRecords = 10;
    private Queue<Item> fifoBuffer = new Queue<Item>();

    public void ProcessRequest(Request request, NamedPipeServerStream pipeServer)
    {
        // Implementação FIFO - Saulo
        if (fifoBuffer.Count >= MaxRecords)
        {
            Item removedItem = fifoBuffer.Dequeue();
            Console.WriteLine($"FIFO: Removed oldest item: {removedItem.Tag}, {removedItem.Value}");
        }
        fifoBuffer.Enqueue(new Item { Tag = request.Tag, Value = request.Value });
    }
}

// Fábrica de estratégias
static class StrategyFactory
{
    public static IStrategy CreateStrategy(StrategyType strategyType)
    {
        // Todos devem deixar esse código como está
        switch (strategyType)
        {
            case StrategyType.FIFO:
                return new PersonFIFO();
            case StrategyType.Aging:
                return new PersonAging();
            case StrategyType.LRU:
                return new PersonLRU();
            default:
                throw new ArgumentException("Invalid strategy type");
        }
    }
}

// Enumeração para os tipos de estratégias
public enum StrategyType
{
    FIFO,
    Aging,
    LRU
}

class DatabaseServer
{
    private static List<Item> items = new List<Item>();
    private static readonly object lockObject = new object();

    static void Main()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("DatabasePipe", PipeDirection.InOut))
        {
            Console.WriteLine("Waiting for connection from client...");

            // Utilizando um ThreadPool para suportar processamento concorrente
            ThreadPool.QueueUserWorkItem(_ => ProcessClientRequests(pipeServer));

            Console.ReadLine(); // Aguardando a tecla Enter para encerrar o servidor
        }
    }

    private static void ProcessClientRequests(NamedPipeServerStream pipeServer)
    {
        try
        {
            while (true)
            {
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected.");

                // Deserializando a solicitação recebida do cliente
                BinaryFormatter formatter = new BinaryFormatter();
                object requestData = formatter.Deserialize(pipeServer);

                if (requestData is Request)
                {
                    var request = (Request)requestData;

                    // Processando a solicitação dentro de um lock para garantir a consistência dos dados
                    lock (lockObject)
                    {
                        ProcessRequest(request);
                    }
                }

                pipeServer.Disconnect(); // Desconectando após processar a solicitação
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Método para processar diferentes tipos de solicitações

    private static void ProcessRequest(Request request)
    {
        switch (request.Command)
        {
            case CommandType.Insert:
                Insert(request.Tag, request.Value);
                break;
            case CommandType.Remove:
                Remove(request.Tag);
                break;
            case CommandType.Update:
                Update(request.Tag, request.Value);
                break;
            case CommandType.Search:
                Search(request.Tag);
                break;
            case CommandType.SaveToFile:
                SaveToFile(request.FileName);
                break;
            case CommandType.LoadFromFile:
                LoadFromFile(request.FileName);
                break;
            default:
                Console.WriteLine("Invalid command");
                break;
        }
    }

    // Métodos para realizar operações no banco de dados
    // ... (Métodos Insert, Remove, Update, Search, SaveToFile, LoadFromFile)

    private static void Insert(int tag, string value)
    {
        var newItem = new Item { Tag = tag, Value = value };
        items.Add(newItem);
        Console.WriteLine($"Inserted: {newItem.Tag}, {newItem.Value}");
    }

    private static void Remove(int tag)
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

    private static void Update(int tag, string newValue)
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

    private static void Search(int tag)
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

    private static void SaveToFile(string fileName)
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving database to file: {ex.Message}");
        }
    }

    private static void LoadFromFile(string fileName)
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading database from file: {ex.Message}");
        }
    }
}

// Classe que representa uma solicitação do cliente para o servidor

[Serializable]
public class Request
{
    public CommandType Command { get; set; }
    public int Tag { get; set; }
    public string Value { get; set; }
    public string FileName { get; set; }
    public StrategyType StrategyType { get; set; } // Adicionado para especificar a estratégia
}

// Enumeração que define os diferentes tipos de comandos suportados

public enum CommandType
{
    Insert,
    Remove,
    Update,
    Search,
    SaveToFile,
    LoadFromFile
}
