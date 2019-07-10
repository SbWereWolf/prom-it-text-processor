using System;
using System.Net.Sockets;
using System.Text;
using text_processor;

namespace ConsoleServer
{
    public class RequestHandler
    {
        private readonly TcpClient _client;
        private readonly string _dataPath;
        public RequestHandler(TcpClient tcpClient, string dataPath)
        {
            _client = tcpClient;
            _dataPath = dataPath;
        }

        public void Process()
        {
            try
            {
                var stream = _client?.GetStream();
                if (stream != null)
                {
                    while (true)
                    {
                        var portion = new byte[64];
                        var builder = new StringBuilder();
                        do
                        {
                            var bytes = stream.Read(portion, 0, portion.Length);
                            builder.Append(Encoding.Unicode.GetString(portion, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        var input = builder.ToString();

                        string[] suggestion = null;
                        var letProcess = input.Length > 2;
                        if (!letProcess)
                        {
                            suggestion = new[] {"для автодополнения необходимо минимум три символа"};
                        }
                        if (letProcess)
                        {
                            var processor = new InputProcessor(input, _dataPath);
                            suggestion = processor.Process();
                        }

                        builder = new StringBuilder();
                        builder.AppendLine("");
                        if (suggestion != null)
                        {
                            foreach (var word in suggestion)
                            {
                                builder.AppendLine(word);
                            }
                        }
                        var output = builder.ToString();
                        var download = Encoding.Unicode.GetBytes(output);
                        stream.Write(download, 0, download.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _client?.Close();
            }
        }
    }
}
