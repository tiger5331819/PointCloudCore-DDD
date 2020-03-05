using System;
using System.Collections.Generic;

namespace PointCloudCore.DomainCore.EventCore
{
    /// <summary>
    /// 框架：事件流
    /// </summary>
    public class EventStream
    {
        public long Version;//版本号
        public List<Event> Events;//事件列表
    }

    /// <summary>
    /// 框架：事件
    /// </summary>
    public abstract class Event : EventArgs
    {
        public string EventName;
        /// <summary>
        /// Event类的构造函数
        /// </summary>
        /// <param name="typeevent">Event的名字</param>
        public Event(string EventName)
        {
            this.EventName = EventName;
        }
    }
}
