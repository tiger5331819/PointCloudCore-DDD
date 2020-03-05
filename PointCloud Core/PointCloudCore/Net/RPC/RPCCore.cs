using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using PointCloudCore.Net.NetSocket;

namespace PointCloudCore.Net.RPC
{
    /// <summary>
    /// RPC类型
    /// </summary>
    public enum RPCType
    {
        Server=1,
        Client=2
    }
    /// <summary>
    /// RPC传输数据类型
    /// </summary>
    /// <typeparam name="T">参数类型模板</typeparam>
    public class RPCData<T>
    {
        /// <summary>
        /// 函数名
        /// </summary>
        public string FuncName;
        /// <summary>
        /// 参数
        /// </summary>
        public T Params;
        public RPCData(string FuncName,T Params)
        {
            
            this.FuncName = FuncName;
            this.Params = Params;
        }

    }

    /// <summary>
    /// 框架：RPC框架中的RPC核心
    /// </summary>
    public class RPCCore
    {
        /// <summary>
        /// 函数委托
        /// </summary>
        /// <typeparam name="TOUT">返回类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public delegate TOUT Function<TOUT>(string json);
        private IDictionary<string, Delegate> FuncMap;///函数列表   
        private Socket Socket;
        protected NetSocketCore Net;
        public RPCType Type { get; }   

        /// <summary>
        /// RPC初始化
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="Point">端口号</param>
        /// <param name="type">RPC类型</param>
        protected RPCCore(string IP,int Point,RPCType type)
        {
            Net = new NetSocketCore(IP,Point);
            this.Type = type;
            FuncMap = new ConcurrentDictionary<string, Delegate>();
        }
        /// <summary>
        /// 获得链接的客户端列表
        /// </summary>
        /// <returns>Socket字典</returns>
        protected IDictionary<string,Socket> GetSocketMap()
        {
            return Net.SocketsMap;
        }
        /// <summary>
        /// 获取已注册函数
        /// </summary>
        /// <param name="FuncName">函数名</param>
        /// <returns>函数委托</returns>
        protected Delegate GetFunc(string FuncName)
        {
            return FuncMap.ContainsKey(FuncName) ? FuncMap[FuncName] : null;
        }
        /// <summary>
        /// 根据不同的RPC类型进行不同的连接方式（使用套接字&TCP）
        /// 服务端：监听
        /// 客户端：连接
        /// </summary>
        protected void Link()
        {
            switch (Type)
            {
                case RPCType.Client:Socket=Net.Connect(); break;
                case RPCType.Server:Net.LinkBind();break;
            }
        }
        protected void End()
        {
            Net.Close();
        }

        /// <summary>
        /// 添加函数到函数列表中
        /// </summary>
        /// <typeparam name="TOUT">返回类型</typeparam>
        /// <param name="FuncName">函数名</param>
        /// <param name="function">函数</param>
        /// <returns></returns>
        public bool AddFunc<TOUT>(string FuncName,Function<TOUT> function)
        {
            if (!FuncMap.ContainsKey(FuncName))
            {
                FuncMap.Add(FuncName, function);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 接受客户端发送的信息
        /// </summary>
        /// <returns></returns>
        protected virtual string Receive()
        {
            try
            {
                byte[] bytes = new byte[NetSocketCore.ByteSize];
                Socket.Receive(bytes);                
                return bytes.stringDeserializer(Encoding.UTF8);//将结果反序列化
            }
            catch (Exception ex)
            {
                throw(ex);
            }
        }

    }
}
