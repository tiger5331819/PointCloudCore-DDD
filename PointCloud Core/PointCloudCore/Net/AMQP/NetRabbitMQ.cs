using PointCloudCore.DomainCore;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace PointCloudCore.Net.AMQP.RabbitMQ
{
    /// <summary>
    /// 框架：RabbitMQ模块
    /// 封装.NET Core的RabbitMQ.Client
    /// 通过继承此模块来自定义RabbitMQ客户端
    /// 自定义RabbitMQ客户端需要实现IAMQP接口
    /// 使用方式请看RabbitMQ.Client的官方文档
    /// </summary>
    public abstract class NetRabbitMQ:IAMQP
    {
        private readonly ConnectionFactory Factory;
        private readonly IConnection Connection;
        private readonly Dictionary<string, IModel> Model;
        protected NetRabbitMQ(string UserName,string Password,string HostName,string VirtualHost="/")
        {
            Factory = new ConnectionFactory();
            Factory.UserName = UserName;
            Factory.Password = Password;
            Factory.VirtualHost = VirtualHost;
            Factory.HostName = HostName;


            Model = new Dictionary<string, IModel>();
            try
            {
                Connection = Factory.CreateConnection();
            }
            catch(ErrorException ex)
            {
                ErrorMessage.GetError(ex);
            }
            
        }
        ~NetRabbitMQ()
        {
            foreach(KeyValuePair<string,IModel> channel in Model)
            {
                channel.Value.Close();
            }
            Connection.Close();
        }

        public bool Bind(string exchange,string queueName,string routingKey, string exchangeType = ExchangeType.Direct)
        {
            try
            {
                IModel channel = Connection.CreateModel();
                channel.ExchangeDeclare(exchange, exchangeType);
                channel.QueueDeclare(exchange, true, false, false, null);
                channel.QueueBind(queueName, exchange, routingKey, null);
                Model.Add(queueName, channel);
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return false;
            }
            return true;
        }

        public abstract void Send(string exchange, string queueName, string routingKey, byte[] data);

        public IModel this[string queueName]
        {
            get
            {
                if (Model.ContainsKey(queueName))
                    return Model[queueName];
                else return null;
            }
        }


    }
}
