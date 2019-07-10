using System.Data.SQLite;

namespace text_processor
{
    class Mount : CommandHandler, IHandle
    {
        public bool Execute(string dataPath)
        {
            var filename = this.Command?.Argument;
            var lines = new FileHandler(filename).GetLines();

            SQLiteCommand request = null;
            if (lines?.Length != 0 )
            {
                this.SetupConnection(dataPath);
                request = this.CreateRequest();
            }

            var result = false;
            if (request != null && lines != null)
            {
                request.CommandText = @"
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

                this.Connect();
                request.ExecuteNonQuery();


                request.CommandText = "BEGIN TRANSACTION";
                request.ExecuteNonQuery();

                request.CommandText = "insert into file_line(line) VALUES(@line)";

                foreach (var line in lines)
                {
                    request.Parameters?.AddWithValue("@line", line);
                    request.ExecuteNonQuery();
                }

                request.CommandText = "insert into autocompletion " +
                                      "select line, count(line) AS C from file_line " +
                                      "where length(line) <@Length GROUP BY line HAVING C> @Amount ORDER BY line";
                request.Parameters?.AddWithValue("@Length", 16);
                request.Parameters?.AddWithValue("@Amount", 3);
                request.ExecuteNonQuery();


                request.CommandText = "COMMIT TRANSACTION";
                request.ExecuteNonQuery();

                this.Disconnect();

                result = true;
            }

            return result;
        }

        public Mount(CommandInput command) : base(command)
        {
        }
    }
}
