using Autodoc.Accessories.LoggingAccessories;

namespace Autodoc.Accessories.ConfigurationAccessories;

/// <summary>
/// Класс вспомогательных методов общей конфигурации
/// </summary>
public static class CommonConfigurationExtensions
{
    /// <summary>
    /// Данные общей конфигурации
    /// </summary>
    private static IConfigurationRoot _commonConfiguration;

    /// <summary>
    /// Обертка общих данных конфигурации
    /// </summary>
    public static IConfigurationRoot CommonConfiguration => _commonConfiguration ??= GenerateConfiguration();

    /// <summary>
    /// Главный домен приложения
    /// </summary>
    private static string _mainDomain;

    /// <summary>
    /// Обертка главного домена приложения
    /// </summary>
    public static string MainDomain => _mainDomain ??= CommonConfiguration["appSettings:mainDomain"];

    /// <summary>
    /// Метод генерирует данные общей конфигурации
    /// </summary>
    /// <returns>Данные общей конфигурации</returns>
    private static IConfigurationRoot GenerateConfiguration()
    {
        try
        {
            //инициализируем строителя конфигурации
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{LoggingExtensions.AppDir}/common_configuration.json");

            //строим и отдаем параметры конфигурации
            return builder.Build();
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
    }
}