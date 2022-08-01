using Autodoc.FormatsData.AppEnumsData;
using Autodoc.FormatsData.AppTaskData;

namespace Autodoc.Services.DataBaseService;

/// <inheritdoc />
/// <summary>
/// Интерфейс базы данных приложения
/// </summary>
public interface IDataBaseService : IDisposable
{
    /// <summary>
    /// Метод получает все задачи
    /// </summary>
    /// <returns>Объект данных о всех задачах Autodoc</returns>
    Task<List<TaskData>> GetAllTaskAsync();

    /// <summary>
    /// Метод получает задачу
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <returns>Объект данных о задаче Autodoc</returns>
    Task<TaskData> GetTask(long idTask);

    /// <summary>
    /// Метод добавляет задачу
    /// </summary>
    /// <param name="name">Наименование</param>
    /// <param name="status">Статус выполнения</param>
    /// <param name="files">Файлы задачи на сервере</param>
    /// <param name="uploadFiles_1">Файл 1 загружаемый в БД</param>
    /// <param name="uploadFiles_2">Файл 2 загружаемый в БД</param>
    /// <returns>Объект данных о результате добавления задачи Autodoc</returns>
    Task<bool> AddTask(string name, bool status, string[] files, byte[] uploadFiles_1, byte[] uploadFiles_2);

    /// <summary>
    /// Метод удаляет задачу
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <returns>Объект данных о результате удаления задачи Autodoc</returns>
    Task<bool> DeleteTask(long idTask);

    /// <summary>
    /// Метод изменяет статус задачи
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <param name="status">Статус выполнения</param>
    /// <returns>Объект данных о результате изменения задачи Autodoc</returns>
    Task<bool> ChangeStatusTask(long idTask, bool status);

    /// <summary>
    /// Метод изменяет файл задачи
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <param name="index">Какой файл менять</param>
    /// <param name="imageFiles">Файл</param>
    /// <returns>Объект данных о результате изменения задачи Autodoc</returns>
    Task<bool> ChangeFileTask(long idTask, FileNumber index, byte[] imageFiles);
}
