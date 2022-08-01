using Aspose.Imaging;
using Autodoc.Accessories.LoggingAccessories;
using Microsoft.AspNetCore.Mvc;

namespace Autodoc.Controllers;

/// <inheritdoc />
/// <summary>
/// Контроллер Ajax запросов с сайта
/// </summary>

[Route("[controller]/[action]")]
public class FilesController : ControllerBase
{
    /// <summary>
    /// Метод - формируем набор bytea данных из списка файлов
    /// </summary>
    /// <remarks>Формирует типизированный список файлов на сервере Autodoc в ByteA</remarks>
    /// <response code="200">Успешный запрос - сформировано</response>
    /// <response code="404">Данных не существует на сервере</response>
    /// <response code="500">Сервер не доступен</response>
    /// <param name="ImageFiles">Список файлов</param>
    /// <param name="saveServer">Сохранять на сервере в папке wwwroot/files?</param>
    /// <returns>Список ByteA файлов для сохранения на сервер</returns>
    [HttpPost]
    [ProducesResponseType(typeof(List<byte[]>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<byte[]> ConvertFilesToByteA(IFormFile ImageFiles, bool saveServer = false)
    {
        if (ImageFiles == null) return null;

        //формируем путь временного сохранения
        string[] paths = { LoggingExtensions.WWWDir,
                            LoggingExtensions.TotalSecondsTimeString() + "_" + ImageFiles.FileName};

        //конструируем путь к папке files
        string path = Path.Combine(paths);

        //сохраняем файл в папку files в каталоге wwwroot
        using (var fileStream = new FileStream(path, FileMode.Create))
        {
            await ImageFiles.CopyToAsync(fileStream);
        }

        //конвертируем 
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);

        byte[] photo = br.ReadBytes((int)fs.Length);

        br.Close();
        fs.Close();

        if (saveServer == false)
            await DeleteAsync(path);

        //отдаём файл (для последующего сохранения в БД)
        return photo;
    }

    /// <summary>
    /// Метод - Производит конвертацию файла с базы данных в её оригинальный формат
    /// </summary>
    /// <param name="file">byte[] данные файла</param>
    /// <param name="saveServer">Сохранять на сервере в папке wwwroot/img?</param>
    /// <remarks>Конвертирует файл с базы данных для получения</remarks>
    /// <response code="200">Успешный запрос - конвертировано</response>
    /// <response code="404">Данных не существует на сервере</response>
    /// <response code="500">Сервер не доступен</response>
    [HttpPost]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<Image> ConvertByteToImageAsync(byte[] file, bool saveServer = true)
    {
        if (file == null || file.Length == 0) return null;

        using (var productImageStream = new MemoryStream(file))
        {
            Image img;
            try
            {
                img = Image.Load(productImageStream);
            }
            catch(Exception exception)
            {
                exception.LogException();
                return null;
            }
            
            if(img == null) return null;
            return img;
        }
    }

    /// <summary>
    /// Метод - Производит асинхронное удаление файла с сервера
    /// </summary>
    /// <remarks>Удаляет файл с сервера Autodoc</remarks>
    /// <response code="200">Успешный запрос - удалено</response>
    /// <response code="404">Данных не существует на сервере</response>
    /// <response code="500">Сервер не доступен</response>
    /// <param name="path">Путь до файла на сервере</param>
    [HttpDelete]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public Task DeleteAsync(string path)
    {
        return Task.Factory.StartNew(() => new FileInfo(path).Delete());
    }
}