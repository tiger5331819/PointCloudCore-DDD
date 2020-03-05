namespace PointCloudCore.Net.AMQP
{
    /// <summary>
    /// AMQP接口标准
    /// 目前适用于RabbitMQ
    /// </summary>
    public interface IAMQP
    {
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="exchange">交换规则</param>
        /// <param name="queueName">队列名</param>
        /// <param name="routingKey">路由码</param>
        /// <param name="data">数据</param>
        public void Send(string exchange, string queueName, string routingKey, byte[] data);
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="exchange">交换规则</param>
        /// <param name="queueName">队列名</param>
        /// <param name="routingKey">路由码param>
        /// <param name="exchangeType">交换规则类型</param>
        /// <returns></returns>
        public bool Bind(string exchange, string queueName, string routingKey, string exchangeType);
    }
}
