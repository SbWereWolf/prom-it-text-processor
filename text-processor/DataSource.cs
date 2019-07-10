using System.Data.SQLite;

namespace text_processor
{
    class DataSource
    {
        public SQLiteConnection InitializeConnection(string databaseFilePath)
        {
            var connection = new SQLiteConnection($"Data Source={databaseFilePath};Version=3;");
            return connection;
        }
    }
}
