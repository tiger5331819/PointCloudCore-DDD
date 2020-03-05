using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PointCloud_Core.demo;
using PointCloudCore.demo;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PointCloudCore.Net.NetSocket;
using System.Net.Sockets;
using System.Threading;
using PointCloudCore.Net.RPC;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointCloud_Core
{
    public class DemoController : Controller
    {
        // GET: /<controller>/
        public string Index()
        {
            return $"DefaultDemo DemoURL: \n"
                    + "RepositoryDemo: /Demo/RepositoryDemo \n"
                    + "RabbitMQDemo: /Demo/RabbitMQDemo \n"
                    + "NetSocketCoreDemo: /Demo/NetSocketCoreDemo \n"
                    + "RPCDemo: /Demo/RPCDemo \n";
        }

        public string RepositoryDemo()
        {
            //1.初始化存储库
            RepositoryDemo repository = new RepositoryDemo(100);
            //2.从存储库中获取实体
            EntityDemo demo = repository.GetEntity(new EntityDemoIdentity(100));
            return demo.ToString();
        }

        public string RabbitMQDemo()
        {
            //1.初始化RabbitMQ客户端
            RabbitMQDemo rabbit = new RabbitMQDemo();
            //2.绑定交换规则、队列名、路由码
            rabbit.Bind(".NETTest", ".NETTest", "1");
            //3.发送数据
            rabbit.Send(".NETTest", ".NETTest", "1", Encoding.UTF8.GetBytes("RabbitMQ!"));
            //4.获得队列句柄
            IModel channel = rabbit[".NETTest"];
            //5.创建接收事件
            var consumer = new EventingBasicConsumer(channel);
            string message = null;
            //6.接收事件注册
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };
            string consumerTag = channel.BasicConsume(".NETTest", true, consumer);
            while (message == null) { }
            return message + "    consumertag: " + consumerTag;
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
        public string NetSocketCoreDemo()
        {
            //1.初始化服务端网络通讯模块
            NetSocketCore server = new NetSocketCore("127.0.0.1", 9000);
            //2.监听端口
            server.LinkBind();
            //3.初始化客户端网络通讯模块
            NetSocketCore client = new NetSocketCore("127.0.0.1", 9000);
            //4.连接服务端
            Socket socket = client.Connect();
            //5.往服务端发送消息
            TTT ttt = new TTT("Hello mytest!");
            client.Send(ttt.SystemSerializer);

            NetSocketCore client2 = new NetSocketCore("127.0.0.1", 9000);
            Socket socket2 = client2.Connect();
            ttt = new TTT("Hello mytest2!");
            client2.Send(ttt.SystemSerializer);
            NetSocketCore client3 = new NetSocketCore("127.0.0.1", 9000);
            Socket socket3 = client3.Connect();
            ttt = new TTT("Hello mytest3!");
            client3.Send(ttt.SystemSerializer);
            NetSocketCore client4 = new NetSocketCore("127.0.0.1", 9000);
            Socket socket4 = client4.Connect();
            ttt = new TTT("Hello mytest4!");
            client4.Send(ttt.SystemSerializer);
            NetSocketCore client5 = new NetSocketCore("127.0.0.1", 9000);
            Socket socket5 = client5.Connect();
            ttt = new TTT("Hello mytest5!");
            client5.Send(ttt.SystemSerializer);
            NetSocketCore client6 = new NetSocketCore("127.0.0.1", 9000);
            Socket socket6 = client6.Connect();
            ttt = new TTT("Hello mytest6!");
            client6.Send(ttt.SystemSerializer);
            string result = "";

            //服务端接受客户端传送的数据
            Task.Run(async () =>
            {
                result += "\n" + await Task.Run(async () =>
                 {
                     foreach (var pair in server.SocketsMap)
                     {
                         result += "\n" + await Task.Run(() =>
                         {
                             byte[] bb = new byte[NetSocketCore.ByteSize];
                             pair.Value.Receive(bb);
                             TTT tt = bb.SystemDeserializer<TTT>();
                             return tt.Text;
                         });
                         Console.WriteLine(result);
                     }
                     return result;
                 });

            });
            Thread.Sleep(10000);
            return result;
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
            //1.初始化RPC服务端并监听
            RPCInvoke invoke = new RPCInvoke("127.0.0.1", 8888);
            //2.注册调用函数
            invoke.Register<Param, string>("Add", Add);
            //3.初始化RPC客户端并监听
            IRPCProxy proxy = new RPCProxy("127.0.0.1", 8888);
            //4.调用函数并获得结果
            string result = proxy.DoFunc<Param, string>("Add", new Param("Demo:", "Test1"));
            IRPCProxy proxy2 = new RPCProxy("127.0.0.1", 8888);
            result += "\n";
            result += proxy2.DoFunc<Param, string>("Add", new Param("Demo:", "Test2"));
            return result;
        }
    }
}
