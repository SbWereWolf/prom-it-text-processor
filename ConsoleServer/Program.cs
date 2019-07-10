using System;
using System.Text;
using System.Threading;

namespace ConsoleServer
{
    class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server(args, "0.0.0.0");
            new Thread(server.Run).Start();

            ProcessKeyPressed();

            Environment.Exit(-1);
        }

        private static void ProcessKeyPressed()
        {
            ConsoleKeyInfo button;
            var builder = new StringBuilder();
            do
            {
                button = Console.ReadKey();
                if (button.Key != ConsoleKey.Escape && button.Key != ConsoleKey.Enter)
                {
                    builder.Append(button.KeyChar);
                }
                if (button.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    var input = builder.ToString();
                    builder.Clear();

                    var command = input.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (command.Length == 2)
                    {
                        command[1] = command[1]?.Replace("\"", string.Empty);
                    }

                    text_processor.Program.ProcessCommand(command);
                }
            } while (button.Key != ConsoleKey.Escape);
        }
    }
}