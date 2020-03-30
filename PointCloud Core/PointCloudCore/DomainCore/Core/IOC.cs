using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// 框架：IOC模式
    /// </summary>
    public class IOC
    {
        private static IOC _IOC=null;
        /// <summary>
        /// 容器字典
        /// </summary>
        private IDictionary<string, IOC_Container> Container;
        /// <summary>
        /// 容器初始化所用参数字典
        /// </summary>
        private IDictionary<string, object[]> ContainerValue;
        /// <summary>
        /// 域容器字典
        /// </summary>
        private IDictionary<string, IDictionary<string ,object>> ContainerScope;
        private static readonly object LockObject=new Object();

        private string GetKey(string FullName, string Name,string Scope) => $"{Scope}_{FullName}_{Name}";

        /// <summary>
        /// 单例
        /// </summary>
        public static IOC ioc 
        {
            get
            {
                if (_IOC == null)
                {
                    lock (LockObject)
                    {
                        if (_IOC == null) _IOC = new IOC();
                    }
                }
                return _IOC;
            }
        }

        private IOC()
        {
            Container = new ConcurrentDictionary<string,IOC_Container>();
            ContainerValue = new ConcurrentDictionary<string, object[]>();
            ContainerScope = new ConcurrentDictionary<string, IDictionary<string, object>>();
        }

        /// <summary>
        /// 注册
        /// 继承关系（is-a）
        /// </summary>
        /// <typeparam name="TService">父类</typeparam>
        /// <typeparam name="TImplementation">子类</typeparam>
        /// <param name="ParaList">初始化列表</param>
        /// <param name="LifeType">生命周期</param>
        /// <param name="Scope">域名称</param>
        /// <param name="Name">名称</param>
        /// <returns></returns>
        public bool Register<TService, TImplementation>(object[] ParaList=null,Life LifeType=Life.Transient,string Scope="Default",string Name=null)where TImplementation:TService
        {
            try
            {
                Container.Add(GetKey(typeof(TService).FullName, Name, Scope), new IOC_Container()
                {
                    Life = LifeType,
                    Type = typeof(TImplementation)
                });
                if (ParaList != null && ParaList.Length > 0) ContainerValue.Add(GetKey(typeof(TService).FullName, Name, Scope), ParaList);
                return true;
            }
            catch(ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return false;
            }
        }

        /// <summary>
        /// 实例化
        /// 继承关系（is-a）
        /// </summary>
        /// <typeparam name="TService">父类</typeparam>
        /// <param name="Scope">域名称</param>
        /// <param name="Name">名称</param>
        /// <returns>实例化对象</returns>
        public TService Resolve<TService>(string Scope="Default",string Name = null)
        {
            return (TService)ResolveObject(typeof(TService),Scope, Name);
        }

        private object ResolveObject(Type abstractType,string Scope=null ,string Name = null)
        {
            string key = GetKey(abstractType.FullName, Name,Scope);      
            var model= Container.ContainsKey(key) ? Container[key] : null;
            if (model == null) return default;

            #region 生命周期检查
            switch (model.Life)
            {
                case Life.Transient:
                    break;
                case Life.Singleton:
                    if (model.Singleton == null)
                    {
                        break;
                    }
                    else
                    {
                        return model.Singleton;
                    }
                case Life.Scope:
                    if (ContainerScope.ContainsKey(Scope))
                    {
                        IDictionary<string,object> dictionary =ContainerScope[Scope];
                        if (dictionary.ContainsKey(key))
                        {
                            return dictionary[key];
                        }
                        else break;

                    }
                    else
                    {
                        break;
                    }
            }
            #endregion

            #region 构造函数注入
            Type type = model.Type;
            ConstructorInfo constructor = null;

            constructor = type.GetConstructors().FirstOrDefault(c => c.IsDefined(typeof(ConstructorAttribute), true));
            if (constructor == null)
            {
                constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
            }

            List<object> paraList = new List<object>();

            object[] paraConstant = ContainerValue.ContainsKey(key) ? this.ContainerValue[key] : null;
            int iIndex = 0;
            foreach (var para in constructor.GetParameters())
            {
                if (para.IsDefined(typeof(ParameterAttribute), true))
                {
                    paraList.Add(paraConstant[iIndex]);
                    iIndex++;
                }
                else
                {
                    Type paraType = para.ParameterType;

                    string paraName = GetName(para);
                    string ScopeName = GetScope(para);
                    object paraInstance = ResolveObject(paraType, ScopeName, paraName);
                    paraList.Add(paraInstance);
                }
            }
            
            object Instance = constructor.Invoke(paraList.ToArray());
            #endregion

            #region 属性注入
            foreach (var prop in type.GetProperties().Where(p => p.IsDefined(typeof(PropertyAttribute), true)))
            {
                Type propType = prop.PropertyType;
                string paraName = GetName(prop);
                string ScopeName = GetScope(prop);
                object propInstance = ResolveObject(propType, ScopeName, paraName);
                prop.SetValue(Instance, propInstance);
            }
            #endregion

            #region 方法注入
            foreach (var method in type.GetMethods().Where(m => m.IsDefined(typeof(MethodInjectionAttribute), true)))
            {
                List<object> paraInjectionList = new List<object>();
                foreach (var para in method.GetParameters())
                {
                    Type paraType = para.ParameterType;
                    string paraName = GetName(para);
                    string ScopeName = GetScope(para);
                    object paraInstance = ResolveObject(paraType, ScopeName, paraName);
                    paraInjectionList.Add(paraInstance);
                }
                method.Invoke(Instance, paraInjectionList.ToArray());
            }
            #endregion

            switch (model.Life)
            {
                case Life.Transient:
                    break;
                case Life.Singleton:
                    model.Singleton = Instance;
                    break;
                case Life.Scope:
                    if (ContainerScope.ContainsKey(Scope))
                    {
                        IDictionary<string, object> dictionary = ContainerScope[Scope];
                        dictionary.Add(key, Instance);
                    }
                    else
                    {
                        IDictionary<string, object> dictionary = new ConcurrentDictionary<string,object>();
                        dictionary.Add(key, Instance);
                        ContainerScope.Add(Scope, dictionary);
                    }
                    break;
            }

            return Instance;
        }
        private string GetName(ICustomAttributeProvider provider)
        {
            if (provider.IsDefined(typeof(NameAttribute), true))
            {
                var attribute = (NameAttribute)(provider.GetCustomAttributes(typeof(NameAttribute), true)[0]);
                return attribute.Name;
            }
            else return null;
        }
        private string GetScope(ICustomAttributeProvider provider)
        {
            if (provider.IsDefined(typeof(ScopeAttribute), true))
            {
                var attribute = (ScopeAttribute)(provider.GetCustomAttributes(typeof(ScopeAttribute), true)[0]);
                return attribute.Name;
            }
            else return "Default";
        }
    }
}
