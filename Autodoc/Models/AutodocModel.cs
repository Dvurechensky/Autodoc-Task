using Autodoc.FormatsData.AppTaskData;

namespace Autodoc.Models;

/// <summary>
/// Хранилище данных Autodoc
/// </summary>
public class AutodocModel
{
    /// <summary>
    /// Список задач в базе
    /// </summary>
    public List<TaskData> Tasks { get; init; }
}
