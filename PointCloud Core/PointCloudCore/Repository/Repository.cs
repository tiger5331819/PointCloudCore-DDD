using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace PointCloudCore.Repository
{
    /// <summary>
    /// 框架：存储库模式模板
    /// </summary>
    /// <typeparam name="KIdentity">实体唯一标识</typeparam>
    /// <typeparam name="VEntity">实体</typeparam>
    public abstract class Repository<KIdentity,VEntity>
    {
        private IDictionary<KIdentity,VEntity> Entity;//实体缓存
        private IDictionary<KIdentity, int> EntityTTL;//实体生存周期映射
        protected ConcurrentQueue<VEntity> AddEntity = new ConcurrentQueue<VEntity>();//添加队列
        private int Cachesize;//缓存大小
        private int Timeout;//过期时间
        private int Timeout_Close = 99999;//关闭URL阈值

        /// <summary>
        /// 核心存储库配置
        /// </summary>
        /// <param name="Entity">实体缓存实现</param>
        /// <param name="EntityTTL">缓存生命周期实现</param>
        /// <param name="Cachesize">缓存大小</param>
        /// <param name="Timeout">过期时间（单位为次数 100ms/次）</param>        
        protected Repository(IDictionary<KIdentity, VEntity> Entity, IDictionary<KIdentity, int> EntityTTL, int Cachesize, int Timeout)
        {
            this.Entity = Entity;
            this.EntityTTL = EntityTTL;
            this.Cachesize = Cachesize;
            this.Timeout = Timeout < Timeout_Close ? Timeout : Timeout_Close;
            Thread t = new Thread(run);
            t.Start();
        }

        /// <summary>
        /// 存储库内部运行线程
        /// </summary>
        public async void run()
        {
                while (true)
                {
                //URL实现
                KIdentity Maxk = await Task.Run<KIdentity>(() =>
                {
                    int MaxTTL = 0;
                    KIdentity MaxK = default;
                    foreach (var entry in EntityTTL)
                    {
                        KIdentity k = entry.Key;
                        int ttl = entry.Value;
                        if (ttl == Timeout && ttl != Timeout_Close)
                        {
                            Entity.TryGetValue(k, out VEntity vv);
                            EntityTTL.Remove(k);
                            Entity.Remove(k);
                            SaveEntity(vv);
                        }
                        else
                        {
                            ttl++;
                            EntityTTL[k] = ttl;
                        }
                        if (ttl > MaxTTL)
                        {
                            MaxTTL = ttl;
                            MaxK = k;
                        }
                    }
                    return MaxK;
                });

                if (AddEntity.TryDequeue(out VEntity v))
                    {
                        KIdentity kk = GetEntityIdentity(v);
                        if (Cachesize > Entity.Count)
                        {
                            if (!Entity.ContainsKey(kk))
                            {
                                Entity.Add(kk, v);
                                EntityTTL.Add(kk, 0);
                                WriteLine("Entity Add success!");
                            }
                        }
                        else
                        {
                            Entity.TryGetValue(Maxk,out VEntity vv);
                            EntityTTL.Remove(Maxk);
                            Entity.Remove(Maxk);
                            SaveEntity(vv);
                            Entity.Add(kk, v);
                            EntityTTL.Add(kk, 0);
                            WriteLine("Entity Add success!");
                        }

                    }
                    Thread.Sleep(100);
                }                      
        }

        /// <summary>
        /// 保存实体
        /// </summary>
        /// <param name="v">实体</param>
        /// <returns></returns>
        protected abstract bool SaveEntity(VEntity v);

        /// <summary>
        /// 获得实体唯一标识
        /// </summary>
        /// <param name="v">实体</param>
        /// <returns></returns>
        protected abstract KIdentity GetEntityIdentity(VEntity v);

        /// <summary>
        /// 从存储库中实体变更
        /// </summary>
        /// <param name="k">实体唯一标识</param>
        /// <param name="v">需要变更的实体</param>
        /// <returns></returns>
        protected bool ChangeEntity(KIdentity k, VEntity v)
        {
            if (Entity.ContainsKey(k))
            {
                Entity[k] = v;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 从存储库中通过实体唯一标识获得实体
        /// </summary>
        /// <param name="k">实体唯一标识</param>
        /// <returns></returns>
        protected VEntity Get(KIdentity k)
        {
            return Entity.ContainsKey(k)? Entity[k] : default;
        }

        /// <summary>
        /// 从存储库中移除实体
        /// </summary>
        /// <param name="k">实体唯一标识</param>
        /// <returns></returns>
        protected bool Remove(KIdentity k)
        {
            if (!Entity.ContainsKey(k))
            {
                return false;
            }
            else
            {
                Entity.TryGetValue(k,out VEntity v);
                Entity.Remove(k);
                EntityTTL.Remove(k);
                SaveEntity(v);
            }
            return true;
        }

        /// <summary>
        /// 从存储库中删除实体
        /// </summary>
        /// <param name="k">实体唯一标识</param>
        /// <returns></returns>
        protected bool Delete(KIdentity k)
        {
            if (!Entity.ContainsKey(k))
            {
                return false;
            }
            else
            {
                Entity.Remove(k);
                EntityTTL.Remove(k);
            }
            return true;
        }
    }
}
