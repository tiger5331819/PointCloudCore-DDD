using PointCloudCore.DomainCore.EventCore;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PointCloudCore.DomainCore.Entity
{
    /// <summary>
    /// 框架：实体模型
    /// </summary>
    public abstract class EntityModule
    {
        /// <summary>
        /// 实体唯一标识
        /// </summary>
        public IEntityID ID;

        protected EntityModule(IEntityID ID)
        {
            this.ID = ID;
        }
        /// <summary>
        /// 领域事件添加队列
        /// </summary>
        public readonly ConcurrentQueue<Event> queue=new ConcurrentQueue<Event>();
        /// <summary>
        /// 领域事件列表
        /// </summary>
        public IList<Event> Changes = new List<Event>();
        /// <summary>
        /// 事件执行函数
        /// </summary>
        /// <param name="e">事件</param>
        protected virtual void Mutate(Event e)
        {
            ((dynamic)this).When((dynamic)e);//动态执行
        }
        /// <summary>
        /// 添加事件流
        /// </summary>
        /// <param name="Events">事件列表</param>
        public void AddEventStream(IEnumerable<Event> Events)
        {
            
            foreach (var e in Events)
            {
                Mutate(e);
                Changes.Add(e);
            }
            while(queue.TryDequeue(out Event e))
            {
                Mutate(e);
                Changes.Add(e);
            }
        }
        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="e">领域事件</param>
        public void Apply(Event e)
        {
            queue.Enqueue(e);
        }
    }
}
