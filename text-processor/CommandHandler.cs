using System.Data.SQLite;

namespace text_processor
{
    class CommandHandler
    {
        protected readonly CommandInput Command;
        private SQLiteConnection _connection;

        protected CommandHandler(CommandInput command)
        {
            Command = command;
        }
        protected  void setupConnection(string dataPath)
        {
            _connection = DataSource.InitializeConnection(dataPath);
        }

        protected SQLiteCommand CreateRequest()
        {
            return _connection?.CreateCommand();
        }

        protected void Connect()
        {
            _connection?.Open();
        }
        protected void Disconnect()
        {
            _connection?.Close();
        }

    }
}
