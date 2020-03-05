using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// CoreAMQPModule拓展
    /// </summary>
    static class CoreAMQPModuleExtension
    {
        public static void Receive(this CoreAMQPModule module, string AMQPType, string queueName, Func<EventingBasicConsumer, EventingBasicConsumer> func)
        {
            RabbitModule rabbitMdodule = (RabbitModule)module[AMQPType];
            if (rabbitMdodule == null) throw new NullReferenceException();
            rabbitMdodule.Receive(queueName, func);
        }
        public static void Receive(this RabbitModule AMQP, string queueName, Func<EventingBasicConsumer, EventingBasicConsumer> func)
        {
            IModel channel = AMQP[queueName];
            if (channel == null) throw new NullReferenceException();
            var consumer = func(new EventingBasicConsumer(channel));
            channel.BasicConsume(queueName, true, consumer);
        }
        public static void Send(this CoreAMQPModule module, string exchange, string queueName, string routingKey, byte[] data, string AMQPType = "RabbitMQ")
        {
            RabbitModule rabbitModule = (RabbitModule)module[AMQPType];
            if (rabbitModule == null) throw new NullReferenceException();
            rabbitModule.Send(exchange, queueName, routingKey, data);
        }
    }
}
