﻿namespace text_processor
{
    class Unmount : DatabaseHandler, IHandle
    {
        public bool Execute()
        {
            var connection = this.InitializeConnection();

            var command = connection?.CreateCommand();

            var result = false;
            if (command != null)
            {

                command.CommandText = @"
DROP TABLE IF EXISTS file_line;
DROP INDEX IF EXISTS file_line_line_index;
DROP TABLE IF EXISTS new_data;
DROP INDEX IF EXISTS new_data_line_index;
DROP TABLE IF EXISTS autocompletion;
DROP INDEX IF EXISTS autocompletion_word_uindex;

VACUUM ;
            ";

                connection.Open();
                command.ExecuteNonQuery();

                connection.Close();
                result = true;
            }

            return result;
        }

        public Unmount(CommandInput command) : base(command)
        {
        }
    }
}
