using System.Data.SQLite;

namespace text_processor
{
    class DatabaseHandler
    {
        protected readonly CommandInput _command;

        public DatabaseHandler(CommandInput command)
        {
            _command = command;
        }

        protected SQLiteConnection InitializeConnection()
        {
            var databaseFilePath = Properties.Settings.Default?.DataPath;
            var connection = new SQLiteConnection($"Data Source={databaseFilePath};Version=3;");
            return connection;
        }
    }
}
