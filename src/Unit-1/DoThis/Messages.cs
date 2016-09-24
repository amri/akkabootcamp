namespace WinTail
{
    public class Messages
    {
        public class ContinueProcessing { }


        #region Success messages
        public class InputSuccess
        {
            public string Reason { get; set; }

            public InputSuccess(string reason)
            {
                Reason = reason;
            }
        }
        #endregion

        #region Error messages

        public class InputError
        {
            public string Reason { get; set; }

            public InputError(string reason)
            {
                Reason = reason;
            }
        }

        public class NullInputError : InputError
        {
            public NullInputError(string reason) : base(reason)
            {
            }
        }

        public class ValidationError : InputError
        {
            public ValidationError(string reason) : base(reason)
            {
            }
        }
        #endregion
    }
}