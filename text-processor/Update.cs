using System.Data.SQLite;

namespace text_processor
{
    class Update : CommandHandler, IHandle
    {
        public bool Execute(string dataPath)
        {
            var filename = this.Command?.Argument;
            var lines = new FileHandler(filename).GetLines();

            var connection = InitializeConnection(dataPath);

            SQLiteCommand command = null;
            if (lines?.Length != 0 && connection != null)
            {
                command = connection.CreateCommand();
            }

            var result = false;
            if (command != null && lines != null)
            {
                connection.Open();


                command.CommandText = "BEGIN TRANSACTION";
                command.ExecuteNonQuery();

                command.CommandText = "insert into new_data(line) VALUES(@line)";

                foreach (var line in lines)
                {
                    command.Parameters?.AddWithValue("@line", line);
                    command.ExecuteNonQuery();
                }


                command.CommandText = @"
update autocompletion
set count = (
    select count(line)
    from new_data
    WHERE autocompletion.word = new_data.line
    GROUP BY line
)
WHERE word in (select new_data.line from new_data where new_data.line = word)
                ";
                command.ExecuteNonQuery();

                command.CommandText = @"
insert into autocompletion
select line, count(line) AS C from new_data
where length(line) <16 AND NOT EXISTS (SELECT NULL from autocompletion where word = line ) GROUP BY line HAVING C> 3 ORDER BY line
                ";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM new_data WHERE TRUE";
                command.ExecuteNonQuery();

                command.CommandText = "COMMIT TRANSACTION";
                command.ExecuteNonQuery();

                connection.Close();

                result = true;
            }

            return result;
        }

        public Update(CommandInput command) : base(command)
        {
        }
    }
}
