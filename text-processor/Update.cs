namespace text_processor
{
    class Update : IHandle
    {
        private readonly CommandInput _command;

        public Update(CommandInput command)
        {
            _command = command;
        }
        public bool Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
