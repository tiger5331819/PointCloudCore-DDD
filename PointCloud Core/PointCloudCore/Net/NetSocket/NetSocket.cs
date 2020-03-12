using PointCloudCore.DomainCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;

namespace PointCloudCore.Net.NetSocket
{
    /// <summary>
    /// 自定义序列化时所使用的程序集
    /// Type BindToType函数重载与基类SerializationBinder
    /// </summary>
    class SerializableFind : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;  // 当前程序集
            return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
        }
    }

    /// <summary>
    /// 框架：NetSocketCore模块
    /// 负责网络通讯功能的实现
    /// </summary>
    public class NetSocketCore
    {
        protected readonly Socket Socket = null;
        private readonly IPAddress IP;
        private readonly IPEndPoint Point;
        private static uint Bytesize;
        public readonly IDictionary<string, Socket> SocketsMap;//Socket列表
        public static uint ByteSize { get { return Bytesize; } }

        /// <summary>
        /// Socket套接字初始化
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="Point">端口号</param>
        /// <param name="MB">二进制模块大小（MB）</param>
        public NetSocketCore([Parameter]string IP, [Parameter]int Point, uint MB = 1, AddressFamily addressFamily = AddressFamily.InterNetwork,
                            SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            this.IP = IPAddress.Parse(IP);
            this.Point = new IPEndPoint(this.IP, Point);
            /*
                AddressFamily.InterNetWork：使用 IP4地址。
                SocketType.Stream：支持可靠、双向、基于连接的字节流，而不重复数据。
                此类型的 Socket 与单个对方主机进行通信，并且在通信开始之前需要远程主机连接。
                Stream 使用传输控制协议 (Tcp) ProtocolType 和 InterNetworkAddressFamily。
                ProtocolType.Tcp：使用传输控制协议。
            */
            Socket = new Socket(addressFamily, socketType, protocolType);
            SocketsMap = new ConcurrentDictionary<string, Socket>();
            Bytesize = MB * 1024 * 1024;
        }

        /// <summary>
        /// 发送数据
        /// 不提供序列化方式
        /// 序列化方式由上层提供
        /// </summary>
        /// <param name="data">序列化函数</param>
        /// <returns></returns>
        public bool Send(Func<byte[]> data)
        {
            try
            {
                Socket.Send(data());
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 通过指定的socket发送数据
        /// 不提供序列化方式
        /// 序列化方式由上层提供
        /// </summary>
        /// <param name="data">序列化函数</param>
        /// <param name="socket">Socket句柄</param>
        /// <returns></returns>
        public bool Send(Func<byte[]> data, Socket socket)
        {
            try
            {
                socket.Send(data());
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 监听端口
        /// </summary>
        public void LinkBind()
        {
            //创建监听用的Socket

            try
            {
                Socket.Bind(Point);
                Socket.Listen(10);
                Console.WriteLine("服务器开始监听");

                //这个线程用于实例化socket，每当一个子端connect时，new一个socket对象并保存到相关数据集合
                Thread acceptInfo = new Thread(AcceptInfo);
                acceptInfo.IsBackground = true;
                acceptInfo.Start();
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
            }
        }

        /// <summary>
        ///每有一个客户端连接，就会创建一个socket对象用于保存客户端传过来的套接字信息
        ///如果有自定义方法，需要重载此方法。
        /// </summary>
        public virtual void AcceptInfo()
        {
            while (true)
            {
                try
                {
                    //没有客户端连接时，accept会处于阻塞状态
                    Socket tSocket = Socket.Accept();
                    tSocket.ReceiveTimeout = 100;
                    string point = tSocket.RemoteEndPoint.ToString();
                    Console.WriteLine(point + "连接成功！");
                    SocketsMap.Add(point, tSocket);
                }
                catch (ErrorException ex)
                {
                    ErrorMessage.GetError(ex);
                    Socket.Close();
                }
            }
        }
        /// <summary>
        /// 连接
        /// </summary>
        public Socket Connect()
        {
            try
            {
                Socket.Connect(Point);
                Console.WriteLine("Link server");
                return Socket;
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return null;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                foreach (KeyValuePair<string, Socket> keyValue in SocketsMap)
                {
                    keyValue.Value.Close();
                }
                Socket.Close();
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
            }

        }

        /// <summary>
        /// 迭代器
        /// 通过RemoteEndPoint来获取对应的Socket
        /// </summary>
        /// <param name="point">RemoteEndPoint("0.0.0.0"指自身Socket)</param>
        /// <returns></returns>
        public Socket this[string point]
        {
            get
            {
                if (point.Equals("0.0.0.0")) return Socket;
                else return SocketsMap.ContainsKey(point) == true ? SocketsMap[point] : null;
            }
        }

        ~NetSocketCore()
        {
            Close();
        }
    }

}
