using Autodoc.Accessories.LoggingAccessories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Autodoc;

/// <summary>
/// Класс конфигурации приложения 
/// </summary>
public class Startup
{
    /// <summary>
    /// Конфигурация используемых сервисов в приложении
    /// </summary>
    /// <param name="services">Интерфейс коллекции используемых сервисов</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        services.AddLogging();

        //добавляем использование контроллеров
        services.AddControllers();

        //добавляем политику CORS
        services.AddCors();

        //добавляем Swagger
        services.AddSwaggerGen(options =>
        {
            //пишем опции Swagger
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "REST API Autodoc Task",
                Description = "Получение, отправка, удаление и редактирование задач Autodoc",
                Contact = new OpenApiContact
                {
                    Name = "Nikolay",
                    Email = "star26061997@gmail.com",
                }
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
    }

    /// <summary>
    /// Конфигурация роутинга запросов приложения и действий при старте и остановке
    /// </summary>
    /// <param name="app">Интерфейс конфигурации роутинга запросов приложения</param>
    /// <param name="env">Интерфейс web хостинга приложения</param>
    /// <param name="appLifetime">Интерфейс управлением запуска и отключения приложения</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
    {
        //инициализируем опции перенаправления заголовков запросов
        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All,
            RequireHeaderSymmetry = false,
            ForwardLimit = null
        };
        appLifetime.ApplicationStarted.Register(() =>//старт работы приложения
        {
            LoggingExtensions.Logging.InitializeLogging("Autodoc Task");//инициализируем сервис логирования
        });
        appLifetime.ApplicationStopping.Register(() =>//остановка работы приложения
        {
            LoggingExtensions.Logging.DeinitializeLogging();//останавливаем сервис логирования
        });

        if (env.IsDevelopment())//если среда разработки
        {
            //app.UseDeveloperExceptionPage(); //используем страницу исключений

            //app.UseSwagger();

            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Документация Autodoc API");
            //});
        }
        else
        {
            app.UseExceptionHandler("/error"); //используем страницу ошибок на случай исключений
        }

        app.UseDeveloperExceptionPage(); //используем страницу исключений

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Документация Autodoc API");
        });

        app.UseHttpsRedirection();

        //конфигурируем получение оригинальных заголовков
        app.UseForwardedHeaders(forwardedHeadersOptions);

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseCors(options =>//используем CORS с любых хостов
        {
            options.SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });

        //маршрутизация
        app.UseEndpoints(endpoints =>
        {
            //дефолтный роутер
            endpoints.MapDefaultControllerRoute();

            //для маршрутизации если используются атрибуты
            endpoints.MapControllers();
        });

        //добавляем роутинг по умолчанию
        app.Run(async context =>
        {
            //возвращаем 404 ошибку
            await Task.Run(() => context.Response.StatusCode = 404);
        });
    }
}