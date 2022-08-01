namespace Autodoc.Models
{
    /// <summary>
    /// ������ ������������� ������
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ������������� �������
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// �������� ID
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}