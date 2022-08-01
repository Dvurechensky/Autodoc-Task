namespace Autodoc.Models
{
    /// <summary>
    /// Модель представления ошибки
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Показать ID
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}