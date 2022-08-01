using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Autodoc.FormatsData.AppRequestData.AjaxData;

/// <summary>
/// Объект данных о задаче
/// </summary>
public class AddTaskDataRequest
{
    /// <summary>
    /// Имя задачи
    /// </summary>
    /// <example>Autodoc Example Name Task</example>
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Статус выполнения задачи
    /// </summary>
    /// <example>false</example>
    [Required]
    [JsonPropertyName("status")]
    public bool Status { get; set; }

    /// <summary>
    /// Набор URL адресов файлов задачи
    /// </summary>
    /// <example>
    /// ["https://dvurechensky.com/downloads/task1", "https://dvurechensky.com/downloads/task1"]
    /// </example>
    [Required]
    [JsonPropertyName("files")]
    public string[] Files { get; set; }
}