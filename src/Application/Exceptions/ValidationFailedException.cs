namespace Application.Exceptions
{
    public class ValidationFailedException(string propertyName, string message) : Exception(message)
    {
        public string PropertyName { get; } = propertyName;
    }
}
