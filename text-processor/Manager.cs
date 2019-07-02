namespace text_processor
{
    class Manager
    {
        private readonly CommandInput _command;

        public Manager(CommandInput command)
        {
            _command = command;
        }

        public IHandle getHandler()
        {
            IHandle result = new Mount(_command);
            var isValid = _command != null;
            if (isValid && _command.IsUnmount)
            {
                result = new Unmount(_command);
            }
            if (isValid && _command.IsUpdate)
            {
                result = new Update(_command);
            }

            return result;
        }
    }
}
