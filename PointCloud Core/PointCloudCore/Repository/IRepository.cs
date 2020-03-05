namespace PointCloudCore.Repository
{
    /// <summary>
    /// 存储库模式接口
    /// Key-Valuv模式
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface IRepository<K,V>
    {
        public V GetEntity(K key);
        public bool UpdateEntity(V value);
        public bool DeleteEntity(V value);
        public bool PutEntity(V value);
        public bool RemoveEntity(V value);
    }
}
