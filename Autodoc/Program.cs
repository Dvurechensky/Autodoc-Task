namespace Autodoc;

/// <summary>
/// Стартовый класс приложения
/// </summary>
public static class Program
{
    /// <summary>
    /// Стартовый метод приложения
    /// </summary>
    public static void Main(string[] args)
    {
        //инициализируем строителя конфигурации
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        //инициализируем хост приложения
        var host = new WebHostBuilder()

            //используем Kestrel
            .UseKestrel(options =>
            {
                //выставляем настройку максимального количества одновременных соединений
                options.Limits.MaxConcurrentConnections = null;

                //выставляем настройку максимального количества одновременных соединений
                options.Limits.MaxConcurrentUpgradedConnections = null;

                //таймаут на получение заголовков ставим 30 секунд
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

                //таймаут на получение тела ответа ставим 2 минуты
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);

                //убираем лимит на загрузку файлов
                options.Limits.MaxRequestBodySize = null;
            })

            //текущая директория приложения
            .UseContentRoot(Directory.GetCurrentDirectory())

            //стартовый класс приложения
            .UseStartup<Startup>()

            //строим host
            .Build();

        //запускаем host
        host.Run();
    }
}