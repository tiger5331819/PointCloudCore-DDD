using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PointCloudCore.Net.AMQP;
using PointCloudCore.Net.AMQP.RabbitMQ;

namespace PointCloudCore.demo
{
    public class RabbitMQDemo : NetRabbitMQ
    {
        public RabbitMQDemo() : base("MSI-SU","suhuyuan","localhost")
        {
            
        }

        public override void Send(string exchange,string queueName,string routingKey,byte[] data)
        {
            var channel=this[queueName];
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 1;
            channel.BasicPublish(exchange, routingKey,true, properties,data);
        }
    }
}
