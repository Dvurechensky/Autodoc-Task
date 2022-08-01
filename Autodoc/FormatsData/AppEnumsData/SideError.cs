namespace Autodoc.FormatsData.AppEnumsData;

/// <summary>
/// Перечисление на чьей стороне возникла ошибка
/// </summary>
public enum SideError
{
    /// <summary>
    /// Ошибка на стороне пользователя
    /// </summary>
    UserSide = 1,
    /// <summary>
    /// Ошибка на стороне сервера
    /// </summary>
    ServerSide = 2
}