using System;

namespace PointCloudCore.Net.RPC
{
    /// <summary>
    /// RPC代理（客户端）模式接口
    /// </summary>
    public interface IRPCProxy
    {
        public TOUT DoFunc<TParam, TOUT>(string name,TParam Params );
    }
    /// <summary>
    /// RPC实现（服务端）模式接口
    /// </summary>
    public interface IRPCInvoke
    {
        public bool Register<TParam, TOUT>(string FuncName, Func<TParam, TOUT> func);
    }
}
