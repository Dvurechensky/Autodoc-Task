using Npgsql;
using Autodoc.Accessories.ConfigurationAccessories;
using Autodoc.Accessories.LoggingAccessories;
using Autodoc.FormatsData.AppDataBaseData;
using Autodoc.FormatsData.AppTaskData;
using NpgsqlTypes;
using Autodoc.FormatsData.AppEnumsData;
using System.Data;

namespace Autodoc.Services.DataBaseService.Implements;

/// <summary>
/// Сервис управления базой данных
/// </summary>
public class DataBaseService : IDataBaseService
{
    /// <summary>
    /// Объект данных о конфигурации подключения к базе данных
    /// </summary>
    private static ConfigDataBase _configureDb;

    /// <summary>
    /// Обертка объекта данных о конфигурации подключения к базе данных
    /// </summary>
    private static ConfigDataBase ConfigureDb => _configureDb ??= new ConfigDataBase
    {
        HostDb = CommonConfigurationExtensions.CommonConfiguration["dataBase:hostDb"],
        PortDb = CommonConfigurationExtensions.CommonConfiguration["dataBase:portDb"],
        NameDb = CommonConfigurationExtensions.CommonConfiguration["dataBase:nameDb"],
        UserDb = CommonConfigurationExtensions.CommonConfiguration["dataBase:userDb"],
        PasswordDb = CommonConfigurationExtensions.CommonConfiguration["dataBase:passwordDb"]
    };

    /// <summary>
    /// Соединение Postgresql
    /// </summary>
    private NpgsqlConnection ConnectionDb { get; }

    /// <summary>
    /// Команда Postgresql
    /// </summary>
    private NpgsqlCommand CommandDb { get; set; }

    /// <summary>
    /// Транзакция Postgresql
    /// </summary>
    private NpgsqlTransaction TransactionDb { get; set; }

    /// <summary>
    /// Конструктор
    /// </summary>
    public DataBaseService()
    {
        ConnectionDb = new NpgsqlConnection($"Host={ConfigureDb.HostDb};" +
                                            $"Port={ConfigureDb.PortDb};" +
                                            $"Username={ConfigureDb.UserDb};" +
                                            $"Password={ConfigureDb.PasswordDb};" +
                                            $"Database={ConfigureDb.NameDb};");
    }

    /// <inheritdoc />
    /// <summary>
    /// Разрушитель соединения Postgresql
    /// </summary>
    public void Dispose()
    {
        try
        {
            ConnectionDb?.Dispose(); //разрушаем соединение Postgresql
        }
        catch (Exception exception)
        {
            exception.LogException(); //логируем исключение
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Метод получает все задачи Autodoc
    /// </summary>
    /// <returns>Объект данных о всех задачах Autodoc</returns>
    public async Task<List<TaskData>> GetAllTaskAsync()
    {
        try
        {
            //инициализируем объект данных спика задач
            var tasksList = new List<TaskData>();

            //открываем соединение Postgresql
            await ConnectionDb.OpenAsync();

            //создаем транзакцию Postgresql
            TransactionDb = await ConnectionDb.BeginTransactionAsync();

            //создаем команду Postgresql
            CommandDb = new NpgsqlCommand
            {
                Connection = ConnectionDb,
                Transaction = TransactionDb
            };

            //получаем список задач
            CommandDb.CommandText = @"SELECT * FROM tasks;";

            //подставляем параметры
            CommandDb.Parameters.Clear();

            //отправляем запрос
            await using (var readerTasks = await CommandDb.ExecuteReaderAsync())
            {
                //считываем данные
                while (await readerTasks.ReadAsync())
                {
                    //добавляем тариф в коллекцию
                    tasksList.Add(new TaskData
                    {
                        IdTask = await readerTasks.GetFieldValueAsync<long>(0),
                        Name = await readerTasks.GetFieldValueAsync<string>(1),
                        Status = await readerTasks.GetFieldValueAsync<bool>(2),
                        TimeCreate = await readerTasks.GetFieldValueAsync<DateTime>(3),
                        Files = await readerTasks.GetFieldValueAsync<string[]>(4),
                        ImageFiles_1 = await new Controllers.FilesController().ConvertByteToImageAsync(await readerTasks.GetFieldValueAsync<byte[]>(5)),
                        ImageFiles_2 = await new Controllers.FilesController().ConvertByteToImageAsync(await readerTasks.GetFieldValueAsync<byte[]>(6))
                    });
                }
                //закрываем reader
                await readerTasks.CloseAsync();
            }

            //закрываем транзакцию
            await TransactionDb.CommitAsync();

            //отдаем коллекцию задач
            return tasksList;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
        finally
        {
            //если транзакция Postgresql инициализирована
            if (TransactionDb != null)
            {
                //разрушаем транзакцию Postgresql
                await TransactionDb.DisposeAsync();
            }

            //разрушаем команду Postgresql
            CommandDb?.Dispose();

            //если соединение Postgresql инициализировано
            if (ConnectionDb != null)
            {
                //закрываем соединение Postgresql
                await ConnectionDb.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Метод получает задачу
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <returns>Объект данных о задаче Autodoc</returns>
    public async Task<TaskData> GetTask(long idTask)
    {
        try
        {
            if(idTask <= 0) return null;

            //инициализируем объект данных задачи
            var taskData = new TaskData();

            //открываем соединение Postgresql
            await ConnectionDb.OpenAsync();

            //создаем транзакцию Postgresql
            TransactionDb = await ConnectionDb.BeginTransactionAsync();

            //создаем команду Postgresql
            CommandDb = new NpgsqlCommand
            {
                Connection = ConnectionDb,
                Transaction = TransactionDb
            };

            //получаем список задач
            CommandDb.CommandText = @"SELECT * FROM tasks WHERE id=@id;";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@id", idTask);

            //отправляем запрос
            await using (var readerTask = await CommandDb.ExecuteReaderAsync())
            {
                //считываем данные
                while (await readerTask.ReadAsync())
                {
                    taskData.IdTask = await readerTask.GetFieldValueAsync<long>(0);
                    taskData.Name = await readerTask.GetFieldValueAsync<string>(1);
                    taskData.Status = await readerTask.GetFieldValueAsync<bool>(2);
                    taskData.TimeCreate = await readerTask.GetFieldValueAsync<DateTime>(3);
                    taskData.Files = await readerTask.GetFieldValueAsync<string[]>(4);
                    taskData.ImageFiles_1 = await readerTask.IsDBNullAsync(5) ? null : 
                                            await new Controllers.FilesController().ConvertByteToImageAsync(await readerTask.GetFieldValueAsync<byte[]>(5));
                    taskData.ImageFiles_2 = await readerTask.IsDBNullAsync(6) ? null : 
                                            await new Controllers.FilesController().ConvertByteToImageAsync(await readerTask.GetFieldValueAsync<byte[]>(6));
                }
                //закрываем reader
                await readerTask.CloseAsync();
            }

            //закрываем транзакцию
            await TransactionDb.CommitAsync();

            if (taskData.Name == null)
                return null;

            //отдаем объект задачи
            return taskData;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return null;
        }
        finally
        {
            //если транзакция Postgresql инициализирована
            if (TransactionDb != null)
            {
                //разрушаем транзакцию Postgresql
                await TransactionDb.DisposeAsync();
            }

            //разрушаем команду Postgresql
            CommandDb?.Dispose();

            //если соединение Postgresql инициализировано
            if (ConnectionDb != null)
            {
                //закрываем соединение Postgresql
                await ConnectionDb.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Метод добавляет задачу
    /// </summary>
    /// <param name="name">Наименование</param>
    /// <param name="status">Статус выполнения</param>
    /// <param name="files">Файлы задачи на сервере</param>
    /// <param name="uploadFiles_1">Файл 1 загружаемый в БД</param>
    /// <param name="uploadFiles_2">Файл 2 загружаемый в БД</param>
    /// <returns>Объект данных о результате добавления задачи Autodoc</returns>
    public async Task<bool> AddTask(string name, bool status, string[] files, byte[] uploadFiles_1, byte[] uploadFiles_2)
    {
        try
        {
            if (string.IsNullOrEmpty(name)) return false;

            if(uploadFiles_1 == null)
                uploadFiles_1 = Array.Empty<byte>();

            if (uploadFiles_2 == null)
                uploadFiles_2 = Array.Empty<byte>();

            //открываем соединение Postgresql
            await ConnectionDb.OpenAsync();

            //создаем транзакцию Postgresql
            TransactionDb = await ConnectionDb.BeginTransactionAsync();

            //создаем команду Postgresql
            CommandDb = new NpgsqlCommand
            {
                Connection = ConnectionDb,
                Transaction = TransactionDb
            };

            //добавляем задачу Autodoc
            CommandDb.CommandText = @"INSERT INTO tasks (name, status, time, files, image_files_1, image_files_2) 
                                    VALUES (@name, @status, @time,
                                            @files, @image_files_1, @image_files_2);";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@name", name);
            CommandDb.Parameters.AddWithValue("@status", status);
            CommandDb.Parameters.AddWithValue("@time", DateTime.Now);
            CommandDb.Parameters.AddWithValue("@files", NpgsqlDbType.Array | NpgsqlDbType.Text, files.ToArray());
            CommandDb.Parameters.AddWithValue("@image_files_1", NpgsqlDbType.Bytea, uploadFiles_1);
            CommandDb.Parameters.AddWithValue("@image_files_2", NpgsqlDbType.Bytea, uploadFiles_2);

            //отправляем запрос
            await CommandDb.ExecuteNonQueryAsync();

            //закрываем транзакцию
            await TransactionDb.CommitAsync();

            //отдаем результат добавления задачи
            return true;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return false;
        }
        finally
        {
            //если транзакция Postgresql инициализирована
            if (TransactionDb != null)
            {
                //разрушаем транзакцию Postgresql
                await TransactionDb.DisposeAsync();
            }

            //разрушаем команду Postgresql
            CommandDb?.Dispose();

            //если соединение Postgresql инициализировано
            if (ConnectionDb != null)
            {
                //закрываем соединение Postgresql
                await ConnectionDb.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Метод удаляет задачу
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <returns>Объект данных о результате удаления задачи Autodoc</returns>
    public async Task<bool> DeleteTask(long idTask)
    {
        try
        {
            if (idTask <= 0) return false;

            //открываем соединение Postgresql
            await ConnectionDb.OpenAsync();

            //создаем транзакцию Postgresql
            TransactionDb = await ConnectionDb.BeginTransactionAsync();

            //создаем команду Postgresql
            CommandDb = new NpgsqlCommand
            {
                Connection = ConnectionDb,
                Transaction = TransactionDb
            };

            //количество задач под удаление
            int count = 0;

            //прроверяем существование задачи
            CommandDb.CommandText = @"SELECT count(*) FROM tasks WHERE id = @id;";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@id", idTask);

            //отправляем запрос
            await using (var readerTask = await CommandDb.ExecuteReaderAsync())
            {
                //считываем данные
                while (await readerTask.ReadAsync())
                {
                    count = await readerTask.GetFieldValueAsync<int>(0);
                }
                //закрываем reader
                await readerTask.CloseAsync();
            }

            if (count == 0)
                return false;

            //удаляем задачу Autodoc
            CommandDb.CommandText = @"DELETE FROM tasks WHERE id = @id;";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@id", idTask);

            //отправляем запрос
            await CommandDb.ExecuteNonQueryAsync();

            //закрываем транзакцию
            await TransactionDb.CommitAsync();

            //отдаем результат удаления задачи
            return true;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return false;
        }
        finally
        {
            //если транзакция Postgresql инициализирована
            if (TransactionDb != null)
            {
                //разрушаем транзакцию Postgresql
                await TransactionDb.DisposeAsync();
            }

            //разрушаем команду Postgresql
            CommandDb?.Dispose();

            //если соединение Postgresql инициализировано
            if (ConnectionDb != null)
            {
                //закрываем соединение Postgresql
                await ConnectionDb.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Метод изменяет статус задачи
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <param name="status">Статус выполнения</param>
    /// <returns>Объект данных о результате изменения задачи Autodoc</returns>
    public async Task<bool> ChangeStatusTask(long idTask, bool status)
    {
        try
        {
            if (idTask <= 0) return false;

            //открываем соединение Postgresql
            await ConnectionDb.OpenAsync();

            //создаем транзакцию Postgresql
            TransactionDb = await ConnectionDb.BeginTransactionAsync();

            //создаем команду Postgresql
            CommandDb = new NpgsqlCommand
            {
                Connection = ConnectionDb,
                Transaction = TransactionDb
            };

            //количество задач под удаление
            int count = 0;

            //проверяем существование задачи
            CommandDb.CommandText = @"SELECT count(*) FROM tasks WHERE id = @id;";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@id", idTask);

            //отправляем запрос
            await using (var readerTask = await CommandDb.ExecuteReaderAsync())
            {
                //считываем данные
                while (await readerTask.ReadAsync())
                {
                    count = await readerTask.GetFieldValueAsync<int>(0);
                }
                //закрываем reader
                await readerTask.CloseAsync();
            }

            if (count == 0)
                return false;

            //меняем задачу Autodoc
            CommandDb.CommandText = @"UPDATE tasks SET status = @status WHERE id = @id;";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@status", status);
            CommandDb.Parameters.AddWithValue("@id", idTask);

            //отправляем запрос
            await CommandDb.ExecuteNonQueryAsync();

            //закрываем транзакцию
            await TransactionDb.CommitAsync();

            //отдаем результат изменения задачи
            return true;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return false;
        }
        finally
        {
            //если транзакция Postgresql инициализирована
            if (TransactionDb != null)
            {
                //разрушаем транзакцию Postgresql
                await TransactionDb.DisposeAsync();
            }

            //разрушаем команду Postgresql
            CommandDb?.Dispose();

            //если соединение Postgresql инициализировано
            if (ConnectionDb != null)
            {
                //закрываем соединение Postgresql
                await ConnectionDb.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Метод изменяет файл задачи
    /// </summary>
    /// <param name="idTask">Идентификатор задачи</param>
    /// <param name="index">Какой файл менять</param>
    /// <param name="imageFiles">Файл</param>
    /// <returns>Объект данных о результате изменения задачи Autodoc</returns>
    public async Task<bool> ChangeFileTask(long idTask, FileNumber index, byte[] imageFiles)
    {
        try
        {
            if (idTask <= 0 || imageFiles == null) return false;

            //открываем соединение Postgresql
            await ConnectionDb.OpenAsync();

            //создаем транзакцию Postgresql
            TransactionDb = await ConnectionDb.BeginTransactionAsync();

            //создаем команду Postgresql
            CommandDb = new NpgsqlCommand
            {
                Connection = ConnectionDb,
                Transaction = TransactionDb
            };

            //количество задач под удаление
            int count = 0;

            //проверяем существование задачи
            CommandDb.CommandText = @"SELECT count(*) FROM tasks WHERE id = @id;";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@id", idTask);

            //отправляем запрос
            await using (var readerTask = await CommandDb.ExecuteReaderAsync())
            {
                //считываем данные
                while (await readerTask.ReadAsync())
                {
                    count = await readerTask.GetFieldValueAsync<int>(0);
                }
                //закрываем reader
                await readerTask.CloseAsync();
            }

            if (count == 0)
                return false;

            //меняем задачу Autodoc
            if (index == FileNumber.File_One)
                CommandDb.CommandText = @"UPDATE tasks SET image_files_1 = @image WHERE id = @id;";
            else
                CommandDb.CommandText = @"UPDATE tasks SET image_files_2 = @image WHERE id = @id;";

            //подставляем параметры
            CommandDb.Parameters.Clear();
            CommandDb.Parameters.AddWithValue("@image", NpgsqlDbType.Bytea, imageFiles);
            CommandDb.Parameters.AddWithValue("@id", idTask);

            //отправляем запрос
            await CommandDb.ExecuteNonQueryAsync();

            //закрываем транзакцию
            await TransactionDb.CommitAsync();

            //отдаем результат изменения задачи
            return true;
        }
        catch (Exception exception)
        {
            //логируем исключение
            exception.LogException();

            //отдаем null
            return false;
        }
        finally
        {
            //если транзакция Postgresql инициализирована
            if (TransactionDb != null)
            {
                //разрушаем транзакцию Postgresql
                await TransactionDb.DisposeAsync();
            }

            //разрушаем команду Postgresql
            CommandDb?.Dispose();

            //если соединение Postgresql инициализировано
            if (ConnectionDb != null)
            {
                //закрываем соединение Postgresql
                await ConnectionDb.CloseAsync();
            }
        }
    }
}
