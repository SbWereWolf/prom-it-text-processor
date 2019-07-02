using System;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace text_processor
{
    class Mount : IHandle
    {
        private readonly CommandInput _command;

        public Mount(CommandInput command)
        {
            _command = command;
        }
        public bool Execute()
        {
            var filename = this._command?.Argument;
            var wholeText = string.Empty;
            try
            {
                using (var sr = new StreamReader(filename, Encoding.UTF8))
                {
                    wholeText = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            var lines = wholeText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

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

            CREATE TABLE file_line
            (
                line TEXT
                );
            CREATE INDEX file_line_line_index ON file_line(line);
            CREATE TABLE autocompletion
            (
                word TEXT NOT NULL,
                count INTEGER NOT NULL
                );
            CREATE UNIQUE INDEX autocompletion_word_uindex ON autocompletion(word);
            ";
            command.ExecuteNonQuery();


            command.CommandText = "BEGIN TRANSACTION";
            command.ExecuteNonQuery();

            command.CommandText = "insert into file_line(line) VALUES(@line)";

            foreach (var line in lines)
            {
                command.Parameters.AddWithValue("@line", line);
                command.ExecuteNonQuery();
            }

            command.CommandText = "insert into autocompletion select line, count(line) AS C from file_line GROUP BY line HAVING C >3 ORDER BY line";
            command.ExecuteNonQuery();


            command.CommandText = "COMMIT TRANSACTION";
            command.ExecuteNonQuery();

            connection.Close();

            return true;
        }
    }
}
