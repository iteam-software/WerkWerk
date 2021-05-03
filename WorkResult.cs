namespace WerkWerk
{
    public class WorkResult
    {
        private bool _succeeded;
        private string _error;

        public bool Succeeded => _succeeded;
        public string Error => _error;

        public static WorkResult Success()
        {
            return new WorkResult
            {
                _succeeded = true,
            };
        }

        public static WorkResult Fail(string error)
        {
            return new WorkResult
            {
                _error = error,
            };
        }

        private WorkResult() { }
    }
}