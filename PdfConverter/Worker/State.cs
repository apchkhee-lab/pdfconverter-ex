namespace PdfConverter.Worker
{
    public struct State
    {
        public string Message;
        public bool IsError;

        public State(string msg, bool isErr)
        {
            Message = msg;
            IsError = isErr;
        }

        public State(string msg) : this(msg, false)
        {

        }
    }
}