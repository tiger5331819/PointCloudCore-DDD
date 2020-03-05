using System;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// 框架：领域核心
    /// 支持IOC模块和AMQP模块
    /// </summary>
    public class Core
    {
        private static Core DomainCore = null;
        private readonly static Object objLock=new Object();

        /// <summary>
        /// IOC模块
        /// </summary>
        private readonly static IOC CoreIOC=IOC.ioc;

        /// <summary>
        /// AMQP模块（目前只支持RabbitMQ）
        /// </summary>
        public CoreAMQPModule AMQPmodule=new CoreAMQPModule(); 

        public static string CoreName { get; set; }
        public IOC ioc { get { return CoreIOC; } }

        /// <summary>
        /// 单例
        /// </summary>
        public static Core Instance
        {
            get
            {
                if (DomainCore == null)
                {
                    lock (objLock)
                    {
                        if (DomainCore == null) DomainCore = new Core();
                    }
                }
                return DomainCore;
            }

        }

        static Core()
        {

        }

        /// <summary>
        /// 实例化
        /// 继承关系（is-a）
        /// </summary>
        /// <typeparam name="TService">父类</typeparam>
        /// <param name="Scope">域名称</param>
        /// <param name="Name">名称</param>
        /// <returns>实例化对象</returns>
        public static TService Resolve<TService>(string Scope = "Default", string Name = null)
        {
            return CoreIOC.Resolve<TService>(Scope, Name);
        }        
    }
}
