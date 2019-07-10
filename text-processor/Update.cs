using System.Data.SQLite;

namespace text_processor
{
    class Update : CommandHandler, IHandle
    {
        public bool Execute(string dataPath)
        {
            var filename = this.Command?.Argument;
            var lines = new FileHandler(filename).GetLines();

            SQLiteCommand request = null;
            if (lines?.Length != 0 )
            {
                this.setupConnection(dataPath);
                request = this.CreateRequest();
            }

            var result = false;
            if (request != null && lines != null)
            {
                this.Connect();

                request.CommandText = "BEGIN TRANSACTION";
                request.ExecuteNonQuery();

                request.CommandText = "insert into new_data(line) VALUES(@line)";

                foreach (var line in lines)
                {
                    request.Parameters?.AddWithValue("@line", line);
                    request.ExecuteNonQuery();
                }
                
                request.CommandText = @"
update autocompletion
set count = (
    select count(line)
    from new_data
    WHERE autocompletion.word = new_data.line
    GROUP BY line
)
WHERE word in (select new_data.line from new_data where new_data.line = word)
                ";
                request.ExecuteNonQuery();

                request.CommandText = @"
insert into autocompletion
select line, count(line) AS C from new_data
where length(line) <16 AND NOT EXISTS (SELECT NULL from autocompletion where word = line ) GROUP BY line HAVING C> 3 ORDER BY line
                ";
                request.ExecuteNonQuery();

                request.CommandText = "DELETE FROM new_data WHERE TRUE";
                request.ExecuteNonQuery();

                request.CommandText = "COMMIT TRANSACTION";
                request.ExecuteNonQuery();

                this.Disconnect();

                result = true;
            }

            return result;
        }

        public Update(CommandInput command) : base(command)
        {
        }
    }
}
