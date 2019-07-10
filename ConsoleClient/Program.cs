using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ConsoleClient
{
    class Program
    {
        private static int _port;
        private  static string _address = string.Empty;
        static void Main(string[] args)
        {
            var client = SetupClient(args);
            try
            {
                if (client != null)
                {
                    var stream = client.GetStream();

                    while (true)
                    {
                        var wordPart = Console.ReadLine();
                        var letExit = string.IsNullOrWhiteSpace(wordPart);
                        if (letExit)
                        {
                            break;
                        }
                        var upload = Encoding.Unicode.GetBytes(wordPart);
                        stream.Write(upload, 0, upload.Length);

                        var portion = new byte[64];
                        var builder = new StringBuilder();
                        do
                        {
                            var bytes = stream.Read(portion, 0, portion.Length);
                            builder.Append(Encoding.Unicode.GetString(portion, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        var words = builder.ToString();
                        Console.WriteLine("Возможные варианты: {0}", words);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client?.Close();
            }
        }

        private static TcpClient SetupClient(IReadOnlyList<string> args)
        {
            var isValid = args != null;
            if (isValid)
            {
                isValid = args.Count > 1;
            }
            var portExists = false;
            if (isValid)
            {
                portExists = args[0] != null;
            }
            if (portExists)
            {
                _port = int.Parse(args[0]);
            }
            var pathExists = false;
            if (isValid)
            {
                pathExists = args[1] != null;
            }
            if (pathExists)
            {
                _address = args[1];
            }

            if (_port == 0)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(_port), message: "Не задано значение порта");
            }
            if (_address == string.Empty)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(_address), message: "Не задано значение адреса сервера");
            }

            var client = new TcpClient(_address ?? throw new InvalidOperationException("Не задано значение адреса сервера"), _port);
            return client;
        }
    }
}
