using System.Data.SQLite;

namespace text_processor
{
    class Mount : DatabaseHandler, IHandle
    {
        public bool Execute()
        {
            var filename = this._command?.Argument;
            var lines = (new FileHandler(filename)).GetLines();

            var connection = this.InitializeConnection();

            SQLiteCommand command = null;
            if (lines?.Length != 0 && connection != null)
            {
                command = connection.CreateCommand();
            }

            var result = false;
            if (command != null && lines != null)
            {
                command.CommandText = @"
DROP TABLE IF EXISTS file_line;
DROP INDEX IF EXISTS file_line_line_index;
DROP TABLE IF EXISTS new_data;
DROP INDEX IF EXISTS new_data_line_index;
DROP TABLE IF EXISTS autocompletion;
DROP INDEX IF EXISTS autocompletion_word_uindex;

VACUUM ;

CREATE TABLE file_line
(
    line TEXT
);
CREATE INDEX file_line_line_index ON file_line (line);

CREATE TABLE new_data
(
    line TEXT NOT NULL
);
CREATE INDEX new_data_line_index ON new_data (line);

CREATE TABLE autocompletion
(
    word TEXT NOT NULL,
    count INTEGER NOT NULL
);
CREATE UNIQUE INDEX autocompletion_word_uindex ON autocompletion (word);
            ";

                connection.Open();
                command.ExecuteNonQuery();


                command.CommandText = "BEGIN TRANSACTION";
                command.ExecuteNonQuery();

                command.CommandText = "insert into file_line(line) VALUES(@line)";

                foreach (var line in lines)
                {
                    command.Parameters?.AddWithValue("@line", line);
                    command.ExecuteNonQuery();
                }

                command.CommandText = "insert into autocompletion " +
                                      "select line, count(line) AS C from file_line " +
                                      "where length(line) <16 GROUP BY line HAVING C> 3 ORDER BY line";
                command.ExecuteNonQuery();


                command.CommandText = "COMMIT TRANSACTION";
                command.ExecuteNonQuery();

                connection.Close();

                result = true;
            }

            return result;
        }

        public Mount(CommandInput command) : base(command)
        {
        }
    }
}
