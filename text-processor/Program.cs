using System;
using System.Text;

namespace text_processor
{
    public class Program
    {
        static void Main(string[] args)
        {
            ProcessCommand(args);

            ProcessKeyPressed();
        }

        private static void ProcessKeyPressed()
        {
            ConsoleKeyInfo button;
            var buffer = new StringBuilder();
            do
            {
                button = Console.ReadKey();
                if (button.Key != ConsoleKey.Escape && button.Key != ConsoleKey.Enter)
                {
                    buffer.Append(button.KeyChar);
                }
                if (buffer.Length == 0)
                {
                    button = new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false);
                }

                var amount = 0;
                if (button.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    amount = buffer.Length;
                }
                if (button.Key == ConsoleKey.Enter && amount > 2)
                {
                    var input = buffer.ToString();
                    buffer.Clear();

                    var databaseFilePath = Properties.Settings.Default?.DataPath;
                    var processor = new InputProcessor(input, databaseFilePath);
                    var suggestion = processor.Process();
                    if (suggestion != null)
                    {
                        foreach (var word in suggestion)
                        {
                            Console.WriteLine(word);
                        }
                    }
                    Console.WriteLine();
                }
            } while (button.Key != ConsoleKey.Escape);
        }

        public static void ProcessCommand(string[] args)
        {
            var command = new string[0];
            var tryParse = false;
            if (args?.Length != 0)
            {
                command = args;
                tryParse = true;
            }
            var initialInput = new CommandInput(command);
            var isValid = false;
            if (tryParse)
            {
                isValid = initialInput.Parse();
            }
            if (tryParse && !isValid && args != null)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine($"Ошибка распознания аргументов: `{argument}`");
            }
            var isSuccess = false;
            if (isValid)
            {
                var handler = new Manager(initialInput).getHandler();
                var databaseFilePath = Properties.Settings.Default?.DataPath;
                isSuccess = handler != null && handler.Execute(databaseFilePath);
            }
            if (isValid && !isSuccess && args != null)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine($"Ошибка выполнения команды: `{argument}`");
            }
            if (isValid && isSuccess && args != null)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine($"Команда `{argument}` выполнена успешно");
            }
        }
    }
}
