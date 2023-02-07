using Serilog;
using System.Net.NetworkInformation;

namespace 网络测试工具
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.Console()
              .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();
            int count;
            Log.Information("请输入需要Ping的IP地址数量：");
            try
            {
                count = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                Log.Error("输入的数量有误");
                return;
            }

            List<string> ips = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Log.Information($"请输入Ping的ip地址{i + 1}:");
                string? ip = Console.ReadLine()!.Trim();
                ips.Add(ip);
            }
            Parallel.ForEach(ips, ip =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        Ping p = new Ping();
                        PingReply r = p.Send(ip);
                        if (r.Status == IPStatus.Success)
                        {
                            if (r.RoundtripTime >= 100)
                            {
                                Log.Warning($"ping {ip}成功，延迟{r.RoundtripTime}ms");
                            }
                            else
                            {
                                Log.Information($"ping {ip}成功，延迟{r.RoundtripTime}ms");
                            }

                        }
                        else
                        {
                            Log.Error($"ping {ip}:{r.Status}");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"ping {ip}异常+{e.Message}");
                        continue;
                    }
                }
            });
            Console.ReadKey();
        }
    }
}