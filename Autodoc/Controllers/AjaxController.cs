using Autodoc.Accessories.JsonAccessories;
using Autodoc.Accessories.LoggingAccessories;
using Autodoc.FormatsData.AppEnumsData;
using Autodoc.FormatsData.AppRequestData.AjaxData;
using Autodoc.FormatsData.AppTaskData;
using Autodoc.Services.DataBaseService;
using Autodoc.Services.DataBaseService.Implements;
using Microsoft.AspNetCore.Mvc;

namespace Autodoc.Controllers;

/// <summary>
/// Контроллер Ajax для работы с задачами
/// </summary>

[Route("[controller]/[action]")]
public class AjaxController : ControllerBase
{
    /// <summary>
    /// Получить все задачи
    /// </summary>
    /// <remarks>Получает список задач Autodoc</remarks>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Данных не существует</response>
    /// <response code="500">Сервер не доступен</response>
    /// <returns>Метод возвращает все открытые задачи</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskData>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAllTaskAsync()
    {
        try
        {
            //используем базу приложения
            using IDataBaseService appDb = new DataBaseService();

            //получаем список задач
            var tasks = await appDb.GetAllTaskAsync();

            //проверяем данные списка задач
            if (tasks == null || tasks.Count == 0) return BadRequest("error get tasks");

            //отдаем список задач
            return tasks.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Получить конкретную задачу
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <remarks>Получает задачу Autodoc</remarks>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Данных не существует</response>
    /// <response code="500">Сервер не доступен</response>
    /// <returns>Метод возвращает задачу</returns>
    [HttpGet]
    [Route("{idTask:long}")]
    [ProducesResponseType(typeof(TaskData), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetTask(long idTask)
    {
        try
        {
            //используем базу приложения
            using IDataBaseService appDb = new DataBaseService();

            //получаем задачу
            var task = await appDb.GetTask(idTask);

            //проверяем данные задачи
            if (task == null) return BadRequest("error get tasks");

            //отдаем задачу
            return task.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Добавить задачу
    /// </summary>
    /// <param name="addTaskDataRequest">Объект данных о задаче</param>
    /// <param name="ImageFiles_1">Первое изображение</param>
    /// <param name="ImageFiles_2">Второе изображение</param>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Данных не существует</response>
    /// <response code="500">Сервер не доступен</response>
    /// <returns>Метод возвращает результат добавления задачи</returns>
    [HttpPost]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AddTask(AddTaskDataRequest addTaskDataRequest, IFormFile ImageFiles_1, IFormFile ImageFiles_2)
    {
        try
        {
            //используем базу приложения
            using IDataBaseService appDb = new DataBaseService();

            //получаем список файлов в формате byteA
            var byteA_1 = await new FilesController().ConvertFilesToByteA(ImageFiles_1);
            var byteA_2 = await new FilesController().ConvertFilesToByteA(ImageFiles_2);

            //добавляем задачу
            var task = await appDb.AddTask(addTaskDataRequest.Name, addTaskDataRequest.Status, addTaskDataRequest.Files, byteA_1, byteA_2);

            //проверяем результат добавления задачи
            if (task == false) return BadRequest("error get tasks");

            //отдаем результат добавления задачи
            return task.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Изменить статус задачи
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <param name="status">Статус выполнения задачи</param>
    /// <remarks>Изменяет статус выполнения задачи Autodoc</remarks>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Данных не существует</response>
    /// <response code="500">Сервер не доступен</response>
    /// <returns>Метод возвращает результат изменения задачи</returns>
    [HttpPatch]
    [Route("{idTask:long}/{status}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ChangeStatusTask(long idTask, bool status)
    {
        try
        {
            //используем базу приложения
            using IDataBaseService appDb = new DataBaseService();

            //меняем статус выполнения задачи
            var task = await appDb.ChangeStatusTask(idTask, status);

            //проверяем результат изменения задачи
            if (task == false) return BadRequest("error get tasks");

            //отдаем результат изменения задачи
            return task.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Изменить файл задачи
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <param name="index">Какой файл менять</param>
    /// <param name="ImageFiles">Файл</param>
    /// <remarks>Изменяет файл задачи Autodoc</remarks>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Задачи не существует</response>
    /// <response code="500">Сервер не доступен</response>
    /// <returns>Метод возвращает результат изменения задачи</returns>
    [HttpPatch]
    [Route("{idTask:long}/{index}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ChangeFileTask(long idTask, FileNumber index, IFormFile ImageFiles)
    {
        try
        {
            //используем базу приложения
            using IDataBaseService appDb = new DataBaseService();

            //получаем файл в формате byteA
            var byteA_1 = await new FilesController().ConvertFilesToByteA(ImageFiles);

            //меняем статус выполнения задачи
            var task = await appDb.ChangeFileTask(idTask, index, byteA_1);

            //проверяем результат изменения задачи
            if (task == false) return BadRequest("error get tasks");

            //отдаем результат изменения задачи
            return task.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }

    /// <summary>
    /// Удалить задачу
    /// </summary>
    /// <remarks>Удаляет задачу Autodoc</remarks>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Данных не существует</response>
    /// <response code="500">Сервер не доступен</response>
    /// <returns>Метод возвращает результат удаления задачи</returns>
    [HttpDelete]
    [Route("{idTask:long}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteTask(long idTask)
    {
        try
        {
            //используем базу приложения
            using IDataBaseService appDb = new DataBaseService();

            //удаляем задачу
            var task = await appDb.DeleteTask(idTask);

            //проверяем результат добавления задачи
            if (task == false) return BadRequest("error get tasks");

            //отдаем результат добавления задачи
            return task.SuccessResponse();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем BadRequest
            return BadRequest("failed");
        }
    }
}