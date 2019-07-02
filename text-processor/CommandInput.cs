﻿using System.Linq;

namespace text_processor
{
    class CommandInput
    {
        private const int OperatorIndex = 0;
        private const int ArgumentIndex = 1;
        private const string Update = "update-from";
        private const string Mount = "mount-from";
        private const string Unmount = "unmount-all";

        private static readonly string[] WithoutArgument = { Update };
        private static readonly string[] WithArgument = { Mount, Unmount };
        private readonly string[] _command;
        private string Operation { get; set; }
        private string Argument { get; set; }

        public bool IsUpdate { get; private set; } = false;
        public bool IsMount { get; private set; } = false;
        public bool IsUnmount { get; private set; } = false;

        public CommandInput(string[] command)
        {
            this._command = command;
        }

        public bool Parse()
        {
            var length = this._command?.Length;
            var withArgument = length == 2 && WithArgument != null;
            var withoutArgument = length == 1 && WithoutArgument != null;
            var isSuccess = withoutArgument || withArgument;
            var operation = "";
            if (isSuccess)
            {
                operation = this._command?[OperatorIndex];
            }
            if (isSuccess && withoutArgument)
            {
                isSuccess = WithoutArgument.Contains(operation) ;
            }
            if (isSuccess && withArgument)
            {
                isSuccess = WithArgument.Contains(operation);
            }
            if (isSuccess && withArgument)
            {
                var argument = this._command?[ArgumentIndex];
                isSuccess = argument != null;
                this.Argument = argument;
            }
            if (isSuccess)
            {
                this.Operation = operation;

                this.IsMount = operation == Mount;
                this.IsUnmount = operation == Unmount;
                this.IsUpdate = operation == Update;

            }

            return isSuccess;
        }

        public bool Execute()
        {
            return false;
        }

    }
}