using System.Data.SQLite;

namespace text_processor
{
    class Unmount : IHandle
    {
        public Unmount(CommandInput command)
        {

        }
        public bool Execute()
        {
            var databaseFilePath = Properties.Settings.Default?.DataPath;
            var connection = new SQLiteConnection($"Data Source={databaseFilePath};Version=3;");

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
            DROP TABLE IF EXISTS file_line;
            DROP TABLE IF EXISTS autocompletion;
            DROP INDEX IF EXISTS file_line_line_index;
            DROP INDEX IF EXISTS autocompletion_word_uindex;

            VACUUM;
            ";
            command.ExecuteNonQuery();

            connection.Close();

            return true;
        }
    }
}
