using PointCloudCore.DomainCore.Entity;
using System.Collections.Generic;

namespace PointCloudCore.DomainCore.EventCore
{
    /// <summary>
    /// 事件源接口标准
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="e">事件</param>
        public void AddEvent(Event e);
        /// <summary>
        /// 加载事件至事件源中
        /// </summary>
        /// <param name="ID">实体唯一标识</param>
        public void Load(IEntityID ID);
        /// <summary>
        /// 加载事件流
        /// </summary>
        /// <param name="module">实体模型</param>
        /// <returns></returns>
        public EventStream LoadEventStream(EntityModule module);
        /// <summary>
        /// 加载事件流
        /// </summary>
        /// <param name="module">实体模型</param>
        /// <param name="skipEvents">跳过事件数（左阈值）</param>
        /// <param name="maxCount">最大事件数（右阈值）</param>
        /// <returns></returns>
        public EventStream LoadEventStream(EntityModule module, long skipEvents, int maxCount);
        /// <summary>
        /// 保存事件流
        /// </summary>
        /// <param name="module">实体模型</param>
        /// <param name="expectedVersion">预期版本</param>
        /// <param name="events">事件列表</param>
        public void AppendToStream(EntityModule module, long expectedVersion, ICollection<Event> events);
        /// <summary>
        /// 加载事件流
        /// </summary>
        /// <param name="skipEvents">跳过事件数（左阈值）</param>
        /// <param name="maxCount">最大事件数（右阈值）</param>
        /// <returns></returns>
        IList<Event> LoadEvents(long skipEvents, int maxCount);
    }
}
