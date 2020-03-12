using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PointCloud_Core.demo;
using PointCloudCore.DomainCore;
using PointCloudCore.EF;
using PointCloudCore.Repository;

namespace PointCloudCore.demo
{
    public class RepositoryDemo : Repository<int, EntityDemo>,IRepository<EntityDemoIdentity, EntityDemo>
    {
        public RepositoryDemo([Parameter]int Timeout):base(new ConcurrentDictionary<int, EntityDemo>(),new ConcurrentDictionary<int, int>(),127,Timeout)
        {

        }

        /// <summary>
        /// Demo：使用EF
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //public bool DeleteEntity(EntityDemo value)
        //{
        //    if (delete(getEntityIdentity(value))) 
        //    {
        //        using (testContext context=new testContext())
        //        {
        //            try
        //            {
        //                context.Student.Remove(context.Student.Find(getEntityIdentity(value)));
        //                context.SaveChanges();
        //                return true;
        //            }
        //            catch(Exception ex)
        //            {
        //                Console.WriteLine(ex.ToString());
        //                return false;
        //            }
        //        }
               
        //    }
        //     return true;    
        //}

        public bool DeleteEntity(EntityDemo value)
        {
            return Delete(GetEntityIdentity(value));
        }

        /// <summary>
        /// Demo：使用EF
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        //public EntityDemo GetEntity(EntityDemoIdentity key)
        //{
        //    EntityDemo EntityDemo = get(Convert.ToInt32(key.ID()));
        //    if (EntityDemo == null)
        //    {
        //        using (testContext testContext = new testContext())
        //        {
        //            Student data =testContext.Student.Find(Convert.ToInt32(key.ID()));
        //            EntityDemo = data != null ? new EntityDemo(data) : null;
        //            if (EntityDemo != null)
        //            {
        //                AddEntity.Enqueue(EntityDemo);
        //            }
        //        }
        //    }
        //    return EntityDemo;
        //}

        public EntityDemo GetEntity(EntityDemoIdentity key)
        {
            EntityDemo EntityDemo = Get(Convert.ToInt32(key.ID()));
            if (EntityDemo == null)
            {
                Student student = new Student();
                student.Id = Convert.ToInt32(key.ID());
                student.Name = "TestDemo";
                student.Record = 100;
                student.Sex = "测试";
                EntityDemo = new EntityDemo(student);
                AddEntity.Enqueue(EntityDemo);
            }
            return EntityDemo;
        }

        public bool PutEntity(EntityDemo value)
        {
            try
            {
                AddEntity.Enqueue(value);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool RemoveEntity(EntityDemo value)
        {
            return Remove(GetEntityIdentity(value))? true:false;
        }

        public bool UpdateEntity(EntityDemo value)
        {
            EntityDemo EntityDemo = GetEntity((EntityDemoIdentity)value.ID);
            if (EntityDemo != null)
            {
                if (SaveEntity(value))
                {
                    return ChangeEntity(GetEntityIdentity(value), value);
                }
                else return false;
            }
            else return false;
        }

        protected override int GetEntityIdentity(EntityDemo v)
        {
            return Convert.ToInt32(v.ID.ID());
        }

        /// <summary>
        /// Demo:使用EF
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        //protected override bool saveEntity(EntityDemo v)
        //{
        //    using (testContext context=new testContext())
        //    {
        //        try
        //        {
        //            Student data=context.Student.Find(Convert.ToInt32(v.ID.ID()));
        //            if(data==null)context.Student.Add(v.Data);
        //            else context.Entry(data).CurrentValues.SetValues(v.Data);
        //            if (context.SaveChanges() != 0)
        //            {
        //                Console.WriteLine("Save success!");
        //                return true;
        //            }
        //            return false;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //            return false;
        //        }
        //    }
        //}

        protected override bool SaveEntity(EntityDemo v)
        {
            Console.WriteLine("Save success!");
            return true;
        }

    }
}
