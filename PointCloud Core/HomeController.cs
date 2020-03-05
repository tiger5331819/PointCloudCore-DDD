using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointCloudCore
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return $"欢迎使用！\n"
                    + "这是由苏琥元设计轻量级跨语言领域驱动框架PointCloudCore\n"
                    + "此端是基于.NET Core 3.1 c#8.0编写，Java端在Github上：https://github.com/tiger5331819 \n"
                    + "由此感谢张维忠老师的栽培和我的好搭档仉鹏\n"
                    + "感谢Point Cloud 点云实验室\n"
                    + "感谢我的项目组：PointCloud Niphon 以此纪念~\n"
                    + "\n"
                    + "样例目录如下：\n"
                    + "DefaultDemo DemoURL: \n"
                    + "RepositoryDemo: /Demo/RepositoryDemo \n"
                    + "RabbitMQDemo: /Demo/RabbitMQDemo \n"
                    + "NetSocketCoreDemo: /Demo/NetSocketCoreDemo \n"
                    + "RPCDemo: /Demo/RPCDemo \n"
                    + "\n"
                    + "DemoWithCore DemoURL: \n"
                    + "RabbitMQDemo: /DemoWithCore/RabbitMQDemo \n"
                    + "RepositoryDemo: /DemoWithCore/RepositoryDemo \n"
                    + "RPCDemo: /DemoWithCore/RPCDemo \n"
                    + "ESDemo1: /DemoWithCore/ESDemo1 \n"
                    + "ESDemo2: /DemoWithCore/ESDemo2 \n";
        }
        public string test(string name)
        {
            return $"my name is {name}";
        }      
    } 
}
