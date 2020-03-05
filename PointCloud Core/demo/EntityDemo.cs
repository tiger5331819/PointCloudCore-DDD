using PointCloudCore.DomainCore;
using PointCloudCore.DomainCore.Entity;
using PointCloudCore.DomainCore.EventCore;
using PointCloudCore.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointCloud_Core.demo
{
    public class EntityDemoIdentity : IEntityID
    {
        private readonly int id;
        public EntityDemoIdentity(int id)
        {
            this.id = id;
        }
        public object GetEntityID()
        {
            return new EntityDemoIdentity(id);
        }
        public override string ToString()
        {
            return id.ToString();
        }

        public object ID()
        {
            return id;
        }
    }
    public class EntityDemo : EntityModule
    {
        private string Name;
        private string Sex;
        public int Record { get; set; }
        public EntityDemo([Parameter]Student student):base(new EntityDemoIdentity(student.Id))
        {
            Name = student.Name;
            Sex = student.Sex;
            Record = student.Record;
        }
        
        
        public override string ToString()
        {
            return "Student's ID: " + ID.ToString() + " " +
                    "Name: " + Name + " " + "Sex: " + Sex
                    + " " + "Record: " + Record;
        }
        public Student Data
        {
            get
            {
                Student student = new Student();
                student.Id = Convert.ToInt32(ID.ID());
                student.Name = Name;
                student.Sex = Sex;
                student.Record = Record;
                return student;
            }
        }
        public void Rename(string Name)
        {
            if (this.Name == Name)return;
            Apply(new EntityDemoEvent(Name));
        }
        public void When(EntityDemoEvent e)
        {
            Name = e.Name;
        }


    }

    public class EntityDemoEvent : Event
    {
        public string Name;
        public EntityDemoEvent(string Name,string EventName="EntityDemoEvent") : base(EventName)
        {
            this.Name = Name;
        }
    }

}
