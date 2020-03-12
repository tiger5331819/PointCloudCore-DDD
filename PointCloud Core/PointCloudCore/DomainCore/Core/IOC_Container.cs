using System;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// 框架：IOC容器
    /// </summary>
    public class IOC_Container
    {
        public Type Type;
        public Life Life;
        public object Singleton;
    }
    /// <summary>
    /// 框架：生命周期枚举
    /// Transient:瞬时
    /// Singleton:单例
    /// Scope:域单例
    /// </summary>
    public enum Life 
    {
        Transient,//瞬时
        Singleton,//单例
        Scope//域单例
    }

}
