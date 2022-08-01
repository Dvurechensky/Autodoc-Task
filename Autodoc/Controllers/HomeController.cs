using Autodoc.Accessories.LoggingAccessories;
using Autodoc.Models;
using Autodoc.Services.DataBaseService;
using Autodoc.Services.DataBaseService.Implements;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Autodoc.Controllers;

/// <summary>
/// Базовый функционал
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Главная страница
    /// </summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("/")]
    public async Task<IActionResult> Index()
    {
        try
        {
            //используем базу приложения
            using IDataBaseService appDb = new DataBaseService();

            var tasks = await appDb.GetAllTaskAsync();

            return View(new AutodocModel
            {
                Tasks = tasks
            });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }

    /// <summary>
    /// Cтраница документации
    /// </summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public IActionResult Documnets()
    {
        //редиректим на страницу документации
        return RedirectPermanent("~/swagger");
    }

    /// <summary>
    /// Страница ошибки
    /// </summary>
    /// <returns>Результат действия</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public IActionResult Error()
    {
        try
        {
            //отдаем страницу ошибки
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем 404 ошибку
            return StatusCode(404);
        }
    }
}