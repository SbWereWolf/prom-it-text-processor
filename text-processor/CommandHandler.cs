using System.Data.SQLite;

namespace text_processor
{
    class CommandHandler
    {
        protected readonly CommandInput Command;
        private static readonly DataSource Source = new DataSource();

        protected CommandHandler(CommandInput command)
        {
            Command = command;
        }
        protected static SQLiteConnection InitializeConnection(string dataPath)
        {
            var connection = Source?.InitializeConnection(dataPath);
            return connection;
        }

    }
}
