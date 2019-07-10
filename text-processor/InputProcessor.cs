using System.Collections.Generic;
using System.Data.SQLite;

namespace text_processor
{
    public class InputProcessor
    {
        private readonly string _dataPath;
        private readonly string _input;
        private static readonly DataSource Source = new DataSource();

        public InputProcessor(string input,string dataPath)
        {
            _input = input;
            _dataPath = dataPath;
        }

        public string[] Process()
        {
            var connection = Source?.InitializeConnection(_dataPath);

            SQLiteCommand command = null;
            if (connection != null)
            {
                command = connection.CreateCommand();
            }

            var result = new string[0];
            if (command != null)
            {
                command.CommandText = @"
select
       word
from autocompletion
WHERE
    word LIKE @INPUT || '%'
    ORDER BY count DESC, word
LIMIT @amount
            ";
                command.Parameters?.AddWithValue("@INPUT", _input);
                command.Parameters?.AddWithValue("@amount", 5);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        var resultSet = new List<string>();
                        while (reader.Read())
                        {
                            resultSet.Add(reader.GetString(0));
                        }

                        result = resultSet.ToArray();
                    }
                }
                connection.Close();

            }

            return result;
        }
    }
}
