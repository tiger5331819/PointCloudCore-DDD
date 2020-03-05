using System;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// 构造函数特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class ConstructorAttribute : Attribute
    {
    }

    /// <summary>
    /// 函数方法特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodInjectionAttribute : Attribute 
    {
    }

    /// <summary>
    /// 参数特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterAttribute : Attribute
    {
    }

    /// <summary>
    /// 属性特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
    }

    /// <summary>
    /// 名称特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class NameAttribute : Attribute
    {
        public readonly string Name;
        public NameAttribute(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// 域名称特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class ScopeAttribute : Attribute
    {
        public readonly string Name;
        public ScopeAttribute(string Name="Default")
        {
            this.Name = Name;
        }
    }
}
