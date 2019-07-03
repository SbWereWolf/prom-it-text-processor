using System.Data.SQLite;

namespace text_processor
{
    class DataSource
    {
        public SQLiteConnection InitializeConnection()
        {
            var databaseFilePath = Properties.Settings.Default?.DataPath;
            var connection = new SQLiteConnection($"Data Source={databaseFilePath};Version=3;");
            return connection;
        }
    }
}
