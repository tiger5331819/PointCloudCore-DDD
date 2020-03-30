using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PointCloud_Core.demo;
using PointCloudCore.demo;
using PointCloudCore.DomainCore;
using PointCloudCore.DomainCore.EventCore;
using PointCloudCore.Net.RPC;
using PointCloudCore.Repository;

namespace PointCloudCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Core.CoreName = "PointCloud Core";
            //1.初始化核心
            Core core = Core.Instance;
            //2.使用注册
            core.Use((IOC) =>
            {
                IOC.Register<IRepository<EntityDemoIdentity, EntityDemo>, RepositoryDemo>(new object[] { 99999 }, Life.Singleton);
                IOC.Register<IEventStore, EventStoreDemo>(null, Life.Singleton, "Default", "EventStoreTest");
                IOC.Register<ApplicationService, ApplicationServiceDemo>();
                IOC.Register<IRPCInvoke, RPCInvoke>(new object[] { "127.0.0.1", 9001 }, Life.Scope, "RPC");
                IOC.Register<IRPCProxy, RPCProxy>(new object[] { "127.0.0.1", 9001 }, Life.Scope, "RPC");
            });
            //3.使用AMQP
            core.UseAMQP(() =>
            {
                RabbitModule module = new RabbitModule("MSI-SU", "suhuyuan", "localhost");
                module.Bind(".NETTest", ".NETTest", "1");
                return module;
            });
            

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
