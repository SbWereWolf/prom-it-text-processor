using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleServer
{
    class Server
    {
        private readonly string _interfacese;
        private readonly int _port;
        private TcpListener _listener;
        private readonly string _dataPath = string.Empty;

        public Server(IReadOnlyList<string> args, string interfacese)
        {
            _interfacese = interfacese;
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
                _dataPath = args[1];
            }
        }
        public void Run()
        {
            try
            {
                if (_port == 0 )
                {
                    throw new ArgumentOutOfRangeException(paramName: nameof(_port), message: "Не задано значение порта для приёма соединений");
                }
                if (_dataPath == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(paramName: nameof(_dataPath), message: "Не задан путь к базе данных автодополнений");
                }
                if (string.IsNullOrWhiteSpace(_interfacese))
                {
                    throw new ArgumentOutOfRangeException(paramName: nameof(_interfacese), message: "Не задан адрес для приёма соединений");
                }
                _listener = new TcpListener(IPAddress.Parse(_interfacese), _port);
                _listener.Start();

                while (true)
                {
                    var client = _listener.AcceptTcpClient();
                    var handler = new RequestHandler(client, _dataPath);
                    
                    new Thread(handler.Process).Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _listener?.Stop();
            }
        }
    }
}
