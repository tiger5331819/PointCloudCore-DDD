using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PointCloudCore.DomainCore;
using PointCloudCore.Net.NetSocket;

namespace PointCloudCore.Net.RPC
{
    /// <summary>
    /// 函数容器
    /// 用于装载已注册的函数
    /// </summary>
    /// <typeparam name="TParam">参数类型</typeparam>
    /// <typeparam name="TOUT">返回类型</typeparam>
    class FunctionContainer<TParam, TOUT>
    {
        public Func<TParam, TOUT> Func;
        public FunctionContainer(Func<TParam, TOUT> func)
        {
            this.Func = func;
        }
        public TOUT Do (string json)
        {           
            return Func(json.Json2Object<TParam>());
        }
    }
    /// <summary>
    /// 框架支持RPC服务端实现
    /// 可以通过继承RPCCore与IRPCInvoke来自定义RPC服务端
    /// </summary>
    public class RPCInvoke : RPCCore,IRPCInvoke
    {
        public RPCInvoke([Parameter]string IP, [Parameter]int Point) :base(IP, Point, RPCType.Server)
        {
            Link();
            Thread thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Start();
        }
        /// <summary>
        /// 函数注册
        /// </summary>
        /// <typeparam name="TParam">参数类型</typeparam>
        /// <typeparam name="TOUT">返回值类型</typeparam>
        /// <param name="FuncName">函数名称</param>
        /// <param name="func">函数本身</param>
        /// <returns></returns>
        public virtual bool Register<TParam, TOUT>(string FuncName, Func<TParam, TOUT> func)
        {
            //创建函数容器
            FunctionContainer<TParam, TOUT> container = new FunctionContainer<TParam, TOUT>(func);
            return AddFunc(FuncName,container.Do) ? true : false;//将函数容器注入函数表中
        }

        /// <summary>
        /// RPC服务端运行线程
        /// </summary>
        private async void Run()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    foreach (var pair in GetSocketMap())
                    {
                        Task.Run(() => {
                            byte[] buffer = new byte[NetSocketCore.ByteSize];
                            int flag = 0;
                            try
                            {
                                flag=pair.Value.Receive(buffer);
                            }
                            catch (SocketException)
                            {
                            }
                            if(flag!=0)
                            {
                                JObject obj =buffer.Json2ObjectDeserialize<JObject>();
                                string FuncName = obj.Value<string>("FuncName");
                                Task.Run(() =>
                                {
                                    Delegate fun = GetFunc(FuncName);
                                    if (fun != null) {
                                        pair.Value.Send(fun.DynamicInvoke(obj.Value<JObject>("Params").ToString()).ToJsonByte());                                     
                                    } 
                                    else pair.Value.Send("err:服务端无效函数".stringSerializer(Encoding.UTF8));
                                    return;
                                });
                            }
                            return;
                        });
                    }
                });
                Thread.Sleep(100);
            }
        }
        ~RPCInvoke()
        {
            End();
        }
    }
}
