using System;
using Microsoft.AspNetCore.Mvc;
using PointCloud_Core.demo;
using PointCloudCore.DomainCore;
using PointCloudCore.DomainCore.EventCore;
using PointCloudCore.Net.NetSocket;
using PointCloudCore.Net.RPC;
using PointCloudCore.Repository;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointCloud_Core
{
    public class DemoWithCoreController : Controller
    {
        // GET: /<controller>/
        public string Index()
        {
            return $"DemoWithCore DemoURL: \n"
                    + "RabbitMQDemo: /DemoWithCore/RabbitMQDemo \n"
                    + "RPCDemo: /DemoWithCore/RPCDemo \n"
                    + "ESDemo1: /DemoWithCore/ESDemo1 \n"
                    + "ESDemo2: /DemoWithCore/ESDemo2 \n"
                    + "RepositoryDemo: /DemoWithCore/RepositoryDemo \n";
        }
        public string RabbitMQDemo()
        {
            TTT name = null;
            //注册AMQP接受事件
            Core.Instance.AMQPReceive(".NETTest", (consumer) =>
            {
                consumer.Received += (ch, ea) =>
                {
                    byte[] body = ea.Body;
                    name = body.SystemDeserializer<TTT>();
                };
                return consumer;
            });


            Core.Instance.AMQPSend(".NETTest", ".NETTest", "1", new TTT("测试").SystemSerializer());

            while (name == null) { }
            return name.Text;
        }

        [Serializable]
        class TTT
        {
            private string text;
            public TTT(string text)
            {
                this.text = text;
            }
            public string Text { get { return text; } }
        }


        public struct Param
        {
            public string a;
            public string b;
            public Param(string a, string b)
            {
                this.a = a;
                this.b = b;
            }
        }
        private string Add(Param arg)
        {
            return arg.a + arg.b;
        }
        public string RPCDemo()
        {
            //获取RPCInvoke
            IRPCInvoke invoke = Core.Resolve<IRPCInvoke>("RPC");
            invoke.Register<Param, string>("ADD", Add);
            //获取RPCProxy
            IRPCProxy proxy = Core.Resolve<IRPCProxy>("RPC");
            //输入错误的函数名称会报错
            string result = proxy.DoFunc<Param, string>("Add", new Param("Demo:", "Test1"));
            IRPCProxy proxy2 = Core.Resolve<IRPCProxy>("RPC");
            result += "\n";
            result += proxy2.DoFunc<Param, string>("ADD", new Param("Demo:", "Test2"));
            return result;
        }

        public string ESDemo1()
        {
            //事件源添加事件
            Core.Resolve<IEventStore>("Default", "EventStoreTest").AddEvent(new EntityDemoEvent("Test999"));
            //运行领域事件
            Core.Resolve<ApplicationService>().DoService();
            EntityDemo demo = Core.Instance.ioc.Resolve<IRepository<EntityDemoIdentity, EntityDemo>>().GetEntity(new EntityDemoIdentity(100));
            return demo.ToString();
        }
        public string ESDemo2()
        {                       
            EntityDemo demo = Core.Instance.ioc.Resolve<IRepository<EntityDemoIdentity, EntityDemo>>().GetEntity(new EntityDemoIdentity(100));
            //添加领域事件
            demo.Apply(new EntityDemoEvent("Test-10"));
            //运行领域事件
            Core.Resolve<ApplicationService>().DoService();
            return demo.ToString();
        }
        public string RepositoryDemo()
        {
            //获取存储库
            IRepository<EntityDemoIdentity, EntityDemo> repository = Core.Resolve<IRepository<EntityDemoIdentity, EntityDemo>>();
            //从存储库中获取实体
            EntityDemo demo = repository.GetEntity(new EntityDemoIdentity(100));
            return demo.ToString();
        }
    }
}
