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
    public int Age { get; set; } // Para Aging
    public int AccessCount { get; set; } // Para LRU
}

// Interface para as estratégias
interface IStrategy
{
    void ProcessRequest(Request request, NamedPipeServerStream pipeServer);
}

// Estratégia FIFO 
class FIFO : IStrategy
{
    private const int MaxRecords = 10;
    private Queue<Item> fifoBuffer = new Queue<Item>();

    public void ProcessRequest(Request request, NamedPipeServerStream pipeServer)
    {
        // Implementação FIFO
        if (fifoBuffer.Count >= MaxRecords)
        {
            Item removedItem = fifoBuffer.Dequeue();
            Console.WriteLine($"FIFO: Removed oldest item: {removedItem.Tag}, {removedItem.Value}");
        }
        fifoBuffer.Enqueue(new Item { Tag = request.Tag, Value = request.Value });
    }
}

// Estratégia Aging
class Aging : IStrategy
{
    private const int AgingPeriod = 5; // Período para decrementar valores de envelhecimento
    private int agingCounter = 0;

    public void ProcessRequest(Request request, NamedPipeServerStream pipeServer)
    {
        // Implementação Aging
        agingCounter++;
        if (agingCounter % AgingPeriod == 0)
        {
            lock (DatabaseServer.lockObject)
            {
                foreach (var item in DatabaseServer.items)
                {
                    item.Age--; // Decrementa o valor de envelhecimento
                }
                Console.WriteLine("Aging: Decremented aging values");
            }
        }
    }
}

//Estratégia LRU 
class LRU : IStrategy
{
    private int accessCounter = 0;

    public void ProcessRequest(Request request, NamedPipeServerStream pipeServer)
    {
        // Implementação LRU
        lock (DatabaseServer.lockObject)
        {
            accessCounter++;
            var accessedItem = DatabaseServer.items.Find(item => item.Tag == request.Tag);
            if (accessedItem != null)
            {
                accessedItem.AccessCount = accessCounter;
                Console.WriteLine($"LRU: Updated access count for item: {accessedItem.Tag}, {accessedItem.Value}");
            }
        }
    }
}

// Fábrica de estratégias
static class StrategyFactory
{
    public static IStrategy CreateStrategy(StrategyType strategyType)
    {
        switch (strategyType)
        {
            case StrategyType.FIFO:
                return new FIFO();
            case StrategyType.Aging:
                return new Aging();
            case StrategyType.LRU:
                return new LRU();
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

class DatabaseServer
{
    public static List<Item> items = new List<Item>();
    public static object lockObject = new object();
    private IStrategy strategy;

    public void Start()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("DatabasePipe", PipeDirection.InOut))
        {
            Console.WriteLine("Waiting for connection from the client...");

            ThreadPool.QueueUserWorkItem(_ => ProcessClientRequests(pipeServer));

            Console.ReadLine(); // Aguardando a tecla Enter para encerrar o servidor
        }
    }

    private void ProcessClientRequests(NamedPipeServerStream pipeServer)
    {
        try
        {
            while (true)
            {
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected.");

                BinaryFormatter formatter = new BinaryFormatter();
                object requestData = formatter.Deserialize(pipeServer);

                if (requestData is Request)
                {
                    var request = (Request)requestData;

                    lock (lockObject)
                    {
                        strategy = StrategyFactory.CreateStrategy(request.StrategyType);

                        strategy.ProcessRequest(request, pipeServer);
                    }
                }

                pipeServer.Disconnect();
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (SerializationException ex)
        {
            Console.WriteLine($"Error deserializing request: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }

    // Método para processar diferentes tipos de solicitações
     private void ProcessRequest(Request request, NamedPipeServerStream pipeServer)
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
                SendResponse(pipeServer, "Invalid command");
                break;
        }
    }


    // Métodos para realizar operações no banco de dados
    // Métodos Insert, Remove, Update, Search, SaveToFile, LoadFromFile

    private  void Insert(int tag, string value)
    {
        var newItem = new Item { Tag = tag, Value = value };
        items.Add(newItem);
        Console.WriteLine($"Inserted: {newItem.Tag}, {newItem.Value}");
    }

    private void Remove(int tag)
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

    private void Update(int tag, string newValue)
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

    private void Search(int tag)
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

    private  void SaveToFile(string fileName)
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

    private void LoadFromFile(string fileName)
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
     // Método para enviar uma resposta para o cliente
    private void SendResponse(NamedPipeServerStream pipeServer, string response)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(pipeServer, response);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error sending response: {ex.Message}");
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
