using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;

class DatabaseClient
{
    static void Main()
    {
        Console.WriteLine("Database Client");

        try
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "DatabasePipe", PipeDirection.InOut))
            {
                pipeClient.Connect();
                Console.WriteLine("Connected to the server.");

                while (true)
                {
                    Console.Write("Enter command (insert, remove, update, search, save, load, quit): ");
                    string input = Console.ReadLine()?.ToLower();

                    if (input == "quit")
                    {
                        break;
                    }

                    var request = ParseInput(input);

                    if (request != null)
                    {
                        SendRequest(pipeClient, request);   // Enviando solicitação para o servidor
                        // Aguardando um curto período antes de ler a próxima entrada do usuário
                        System.Threading.Thread.Sleep(100);
                    }
                    else
                    {
                        Console.WriteLine("Invalid command");
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Método para analisar a entrada do usuário e criar uma solicitação correspondente
    private static Request ParseInput(string input)
    {
        string[] tokens = input?.Split(' ');
        string command = tokens?[0];

        if (Enum.TryParse(command, true, out CommandType commandType))
        {
            Request request = new Request { Command = commandType };

            switch (commandType)
            {
                case CommandType.Insert:
                case CommandType.Update:
                    if (tokens.Length >= 3)
                    {
                        request.Tag = int.Parse(tokens[1]);
                        request.Value = string.Join(" ", tokens, 2, tokens.Length - 2);
                        return request;
                    }
                    break;

                case CommandType.Remove:
                case CommandType.Search:
                    if (tokens.Length == 2)
                    {
                        request.Tag = int.Parse(tokens[1]);
                        return request;
                    }
                    break;

                case CommandType.SaveToFile:
                case CommandType.LoadFromFile:
                    if (tokens.Length == 2)
                    {
                        request.FileName = tokens[1];
                        return request;
                    }
                    break;
            }

            return request; // Adicionado para tratar o caso padrão
        }

        return null;
    }


    // Método para enviar a solicitação para o servidor
    private static void SendRequest(NamedPipeClientStream pipeClient, Request request)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(pipeClient, request);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error sending request: {ex.Message}");
        }
    }
}
