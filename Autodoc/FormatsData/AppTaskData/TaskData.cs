using Aspose.Imaging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Autodoc.FormatsData.AppTaskData;

/// <summary>
/// Объект данных о задаче
/// </summary>
public class TaskData
{
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    /// <example>1</example>
    [JsonPropertyName("id")]
    public long IdTask { get; set; }

    /// <summary>
    /// Имя задачи
    /// </summary>
    /// <example>Autodoc Example Name Task</example>
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Дата задачи
    /// </summary>
    /// <example>2022-07-30T15:06:15.622Z</example>
    [JsonPropertyName("timeCreate")]
    public DateTime TimeCreate { get; set; }

    /// <summary>
    /// Статус выполнения задачи
    /// </summary>
    /// <example>false</example>
    [JsonPropertyName("status")]
    public bool Status { get; set; }

    /// <summary>
    /// Набор URL адресов файлов задачи (пути до файлов)
    /// </summary>
    /// <example>
    /// ["https://dvurechensky.com/downloads/task1", "https://dvurechensky.com/downloads/task1"]
    /// </example>
    [JsonPropertyName("files")]
    public string[] Files { get; set; }

    /// <summary>
    /// Файлы №1 загруженный на сервер
    /// </summary>
    [JsonIgnore]
    public Image ImageFiles_1 { get; set; }

    /// <summary>
    /// Файлы №2 загруженный на сервер
    /// </summary>
    [JsonIgnore]
    public Image ImageFiles_2 { get; set; }
}