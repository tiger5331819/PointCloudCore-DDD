using PointCloudCore.Net.AMQP;
using RabbitMQ.Client.Events;
using System;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// DomianCore拓展
    /// </summary>
    public static class DomainCoreBuilderExtensions
    {
        public static void Use(this Core DomainCore, Action<IOC> action)
        {
            if (DomainCore == null)
            {
                throw new NullReferenceException(nameof(DomainCore));
            }
            action(DomainCore.ioc);
        }
        public static void UseAMQP(this Core DomainCore, Func<IAMQP> func, string AMQPType = "RabbitMQ")
        {
            if (DomainCore == null)
            {
                throw new NullReferenceException(nameof(DomainCore));
            }
            IAMQP amqp = func();
            DomainCore.AMQPmodule.AddAMQP(AMQPType, amqp);
        }
        public static void AMQPReceive(this Core DomainCore, string queueName, Func<EventingBasicConsumer, EventingBasicConsumer> func, string AMQPType = "RabbitMQ")
        {
            if (DomainCore == null)
            {
                throw new NullReferenceException(nameof(DomainCore));
            }
            DomainCore.AMQPmodule.Receive(AMQPType, queueName, func);
        }
        public static void AMQPSend(this Core DomainCore, string exchange, string queueName, string routingKey, byte[] data, string AMQPType = "RabbitMQ")
        {
            if (DomainCore == null)
            {
                throw new NullReferenceException(nameof(DomainCore));
            }
            DomainCore.AMQPmodule.Send(exchange, queueName, routingKey, data);
        }
    }
}
