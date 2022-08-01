namespace Autodoc.FormatsData.AppDataBaseData;

/// <summary>
/// Объект данных о конфигурации подключения к базе данных
/// </summary>
internal class ConfigDataBase
{
    /// <summary>
    /// Хост базы данных
    /// </summary>
    internal string HostDb { get; init; }

    /// <summary>
    /// Порт базы данных
    /// </summary>
    internal string PortDb { get; init; }

    /// <summary>
    /// Имя базы данных
    /// </summary>
    internal string NameDb { get; init; }

    /// <summary>
    /// Пользователь базы данных
    /// </summary>
    internal string UserDb { get; init; }

    /// <summary>
    /// Пароль базы данных
    /// </summary>
    internal string PasswordDb { get; init; }
}