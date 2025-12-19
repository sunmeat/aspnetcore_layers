namespace Soccer.BLL.Infrastructure
{
    public class ValidationException : Exception
    {
        public string Property { get; protected set; } // назва властивості, що викликала помилку

        public ValidationException(string message, string prop) : base(message)
        {
            Property = prop; // зберігаємо назву властивості
        }
    }
}