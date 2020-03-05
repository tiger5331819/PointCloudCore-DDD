namespace PointCloudCore.DomainCore.Entity
{
    /// <summary>
    /// 框架：实体唯一标识接口
    /// </summary>
    public interface IEntityID
    {
        /// <summary>
        /// 获取实体唯一标识
        /// </summary>
        /// <returns>实体唯一标识对象</returns>
        public object GetEntityID();
        /// <summary>
        /// 获取实体唯一标识值
        /// </summary>
        /// <returns>实体唯一标识值</returns>
        public object ID();
    }
}
