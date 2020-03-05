using System;
using PointCloudCore.DomainCore;
using PointCloudCore.Net.NetSocket;

namespace PointCloudCore.Net.RPC
{
    /// <summary>
    /// 框架支持RPC代理实现
    /// 可以通过继承RPCCore与IRPCProxy来自定义RPC客户端
    /// 也可以通过直接继承RPCProxy来实现自定义RPC客户端
    /// </summary>
    public class RPCProxy : RPCCore,IRPCProxy
    {
        public RPCProxy([Parameter]string IP, [Parameter]int Point) :base(IP,Point,RPCType.Client)
        {
            Link();
        }
        
        /// <summary>
        /// RPC代理调用服务端已注册函数
        /// </summary>
        /// <typeparam name="TParam">参数类型（需要和被调用函数参数类型一致）</typeparam>
        /// <typeparam name="TOUT">返回类型（需要和被调用函数返回类型一致）</typeparam>
        /// <param name="FuncName">被调用函数名</param>
        /// <param name="Params">被调用函数参数</param>
        /// <returns></returns>
        public virtual TOUT DoFunc<TParam, TOUT>(string FuncName, TParam Params)
        {           
            RPCData<TParam> data = new RPCData<TParam>(FuncName, Params);//创建RPC数据类型
            Net.Send(data.ToJsonByte);//将数据序列化成JSON格式并发送至服务端
            string result = null;
            try
            {
                result= Receive();//获得结果                
                return result.Json2Object<TOUT>();
            }
            catch(Exception ex)
            {
                Console.WriteLine(result);
                Console.WriteLine(ex);
                return default;
            }

        }
        ~RPCProxy()
        {
            End();
        }
    }
}
