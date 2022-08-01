namespace Autodoc;

/// <summary>
/// ��������� ����� ����������
/// </summary>
public static class Program
{
    /// <summary>
    /// ��������� ����� ����������
    /// </summary>
    public static void Main(string[] args)
    {
        //�������������� ��������� ������������
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        //�������������� ���� ����������
        var host = new WebHostBuilder()

            //���������� Kestrel
            .UseKestrel(options =>
            {
                //���������� ��������� ������������� ���������� ������������� ����������
                options.Limits.MaxConcurrentConnections = null;

                //���������� ��������� ������������� ���������� ������������� ����������
                options.Limits.MaxConcurrentUpgradedConnections = null;

                //������� �� ��������� ���������� ������ 30 ������
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

                //������� �� ��������� ���� ������ ������ 2 ������
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);

                //������� ����� �� �������� ������
                options.Limits.MaxRequestBodySize = null;
            })

            //������� ���������� ����������
            .UseContentRoot(Directory.GetCurrentDirectory())

            //��������� ����� ����������
            .UseStartup<Startup>()

            //������ host
            .Build();

        //��������� host
        host.Run();
    }
}