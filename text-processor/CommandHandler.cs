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
        protected SQLiteConnection InitializeConnection()
        {
            var connection = Source?.InitializeConnection();
            return connection;
        }

    }
}
