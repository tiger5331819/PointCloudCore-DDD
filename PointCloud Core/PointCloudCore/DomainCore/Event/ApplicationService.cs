using PointCloudCore.DomainCore.Entity;

namespace PointCloudCore.DomainCore.EventCore
{
    /// <summary>
    /// 框架：A+ES模式
    /// </summary>
    public class ApplicationService
    {
        /// <summary>
        /// 事件源
        /// </summary>
        protected readonly IEventStore EventStore;
        /// <summary>
        /// 实体需要通过存储库中获取，由子类提供,也可以通过第二个构造函数来提供(不建议)
        /// </summary>
        protected EntityModule Entity;

        public ApplicationService(IEventStore eventStore) 
        {
            EventStore = eventStore;
        }

        public ApplicationService(IEventStore eventStore, EntityModule entity)
        {
            EventStore = eventStore;
            Entity = entity;
        }

        /// <summary>
        /// 执行领域事件
        /// </summary>
        public virtual void DoService()
        {
            //1.获得事件流
            var stream = EventStore.LoadEventStream(Entity);
            //2.实体运行领域事件
            Entity.AddEventStream(stream.Events);
            //3.保存事件
            EventStore.AppendToStream(Entity, stream.Version, Entity.Changes);
        }

    }
}
