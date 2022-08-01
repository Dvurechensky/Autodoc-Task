using Autodoc.Services.AppLoggingService;
using Autodoc.Services.AppLoggingService.Implements;
using System.Reflection;

namespace Autodoc.Accessories.LoggingAccessories;

/// <summary>
/// Вспомогательные методы логирования
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Полный путь к рабочей директории приложения
    /// </summary>
    private static string _appDir;

    /// <summary>
    /// Обертка полного пути к рабочей директории приложения
    /// </summary>
    public static string AppDir => _appDir ??= Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

    /// <summary>
    /// Полный путь к директории wwwroot/files
    /// </summary>
    private static string _WWWDir;

    /// <summary>
    /// Обертка полного пути к директории wwwroot/files
    /// </summary>
    public static string WWWDir
    {
        get
        {
            //формируем путь временного сохранения
            string[] paths = { Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "files"};

            //конструируем путь к папке files
            _WWWDir = Path.Combine(paths);

            //отдаём путь
            return _WWWDir;
        }
    }

    /// <summary>
    /// Обертка полного пути к директории wwwroot/img
    /// </summary>
    public static string WWWDirImg
    {
        get
        {
            //формируем путь временного сохранения
            string[] paths = { Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "img"};

            //конструируем путь к папке img
            _WWWDir = Path.Combine(paths);

            //отдаём путь
            return _WWWDir;
        }
    }

    /// <summary>
    /// Интерфейс логирования
    /// </summary>
    private static ILoggingService _logging;

    /// <summary>
    /// Обертка интерфейса логирования
    /// </summary>
    public static ILoggingService Logging => _logging ??= new LoggingService();

    /// <summary>
    /// Метод - формирует дату в секунды
    /// </summary>
    /// <returns></returns>
    public static long TotalSecondsTimeString() => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

    /// <summary>
    /// Метод - расширение логирует исключение
    /// </summary>
    /// <param name="exception">Исключение</param>
    /// <param name="notice">Дополнительная метка для исключения</param>
    public static void LogException(this Exception exception, string notice = null)
    {
        try
        {
            //логируем исключение
            Logging?.LogExceptionAsync(exception, notice);
        }
        catch
        {
            //Ignore
        }
    }

    /// <summary>
    /// Метод - расширение логирует сообщение
    /// </summary>
    /// <param name="textMessage">Текст сообщения</param>
    public static void LogMessage(this string textMessage)
    {
        try
        {
            //логируем сообщение
            Logging?.LogMessageAsync(textMessage);
        }
        catch
        {
            //Ignore
        }
    }
}