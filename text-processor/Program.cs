using System;
using System.Text;

namespace text_processor
{
    class Program
    {
        static void Main(string[] args)
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
            if (tryParse && !isValid)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine($"Ошибка распознания аргументов: `{argument}`");
            }
            var isSuccess = false;
            if (isValid)
            {
                var handler = (new Manager(initialInput)).getHandler();
                isSuccess = handler!= null && handler.Execute();
            }
            if (isValid && !isSuccess)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine($"Ошибка выполнения команды: `{argument}`");
            }
            if (isValid && isSuccess)
            {
                var argument = string.Join(" ", args);
                Console.WriteLine($"Команда `{argument}` выполнена успешно");
            }

            ConsoleKeyInfo button;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;
            
            var buffer = new StringBuilder();
            do
            {
                var input = string.Empty;
                button = Console.ReadKey();
                if (button.Key != ConsoleKey.Escape && button.Key != ConsoleKey.Enter)
                {
                    buffer.Append(button.KeyChar);
                }
                if (button.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    input = buffer.ToString();
                    buffer.Clear();
                }
                if (string.IsNullOrWhiteSpace(input) )
                {
                    button = new ConsoleKeyInfo('\r', ConsoleKey.Escape,false,false,false);
                }
            } while (button.Key != ConsoleKey.Escape);
            
            
        }
    }
}
