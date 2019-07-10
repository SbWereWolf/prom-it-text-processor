namespace text_processor
{
    class Unmount : CommandHandler, IHandle
    {
        public bool Execute(string dataPath)
        {
            this.setupConnection(dataPath);

            var request = this.CreateRequest();

            var result = false;
            if (request != null)
            {

                request.CommandText = @"
DROP TABLE IF EXISTS file_line;
DROP INDEX IF EXISTS file_line_line_index;
DROP TABLE IF EXISTS new_data;
DROP INDEX IF EXISTS new_data_line_index;
DROP TABLE IF EXISTS autocompletion;
DROP INDEX IF EXISTS autocompletion_word_uindex;

VACUUM ;
            ";

                this.Connect();
                request.ExecuteNonQuery();

                this.Disconnect();
                result = true;
            }

            return result;
        }

        public Unmount(CommandInput command) : base(command)
        {
        }
    }
}
