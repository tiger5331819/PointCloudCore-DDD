using PointCloudCore.Net.AMQP;
using System.Collections.Generic;
using PointCloudCore.Net.AMQP.RabbitMQ;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// 框架：核心支持AMQP模块
    /// </summary>
    public class CoreAMQPModule
    {
        private IDictionary<string, object> AMQP;//AMQP列表

        public CoreAMQPModule()
        {
            AMQP = new ConcurrentDictionary<string, object>();
        }
        /// <summary>
        /// 添加AMQP
        /// </summary>
        /// <param name="AMQPType">AMQP名称</param>
        /// <param name="amqp">AMQP客户端</param>
        /// <returns></returns>
        public bool AddAMQP(string AMQPType,IAMQP amqp)
        {
            if (!AMQP.ContainsKey(AMQPType))
            {
                AMQP.Add(AMQPType, amqp);
                return true;
            }
            else return false;
        }
        /// <summary>
        /// 迭代器
        /// 通过AMQP名称获得对应的AMQP客户端
        /// </summary>
        /// <param name="AMQPType">AMQP名称</param>
        /// <returns></returns>
        public object this[string AMQPType] 
        {
            get 
            {
                if (AMQP.ContainsKey(AMQPType))
                {
                    return AMQP[AMQPType];
                }
                else return null;
            }
        }
    }

    /// <summary>
    /// 框架支持的RabbitMQ模块
    /// </summary>
    class RabbitModule : NetRabbitMQ, IAMQP
    {
        public RabbitModule(string UserName, string Password, string HostName, string VirtualHost = "/") : base(UserName,Password,HostName,VirtualHost)
        {
        }

        public new bool Bind(string exchange, string queueName, string routingKey, string exchangeType = ExchangeType.Direct)
        {
            return base.Bind(exchange,queueName,routingKey,exchangeType);
        }
        public override void Send(string exchange, string queueName, string routingKey, byte[] data)
        {
            var channel = this[queueName];
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 1;
            channel.BasicPublish(exchange, routingKey, true, properties, data);
        }
    }
}
