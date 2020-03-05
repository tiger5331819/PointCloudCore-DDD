using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointCloudCore.DomainCore.Entity;
using PointCloudCore.DomainCore.EventCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PointCloud_Core.demo
{
    public class EventStoreDemo : IEventStore
    {
        private List<Event> Events;
        private ConcurrentQueue<Event> AddQueue = new ConcurrentQueue<Event>();
        private long Version;

        public EventStoreDemo()
        {
            Events = new List<Event>();         
        }
        public void AddEvent(Event e)
        {
            AddQueue.Enqueue(e);
        }
        public void AppendToStream(EntityModule entity, long expectedVersion, ICollection<Event> events)
        {
            if (events.Count == 0)return;
            var name = entity.ID.GetEntityID();
            var EventArgs = new JObject();
            EventArgs["Event"] = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(events));
            EventArgs["Version"] = expectedVersion;
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/事件";
           
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            } 
            path += "/" + DateTime.Now.ToString("yyyy-MM-dd") + "事件" + name.ToString() + ".txt";
            using (StreamWriter writer = File.CreateText(path))
            {
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {Formatting=Formatting.Indented });
                serializer.Serialize(writer,EventArgs);
            }
            entity.Changes=new List<Event>();
            Events = new List<Event>();
        }

        public void Load(IEntityID ID)
        {
            
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/事件";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "/" + DateTime.Now.ToString("yyyy-MM-dd") + "事件" + ID.GetEntityID().ToString() + ".txt";
            if (!File.Exists(path))
            {
                using (StreamWriter writer = File.CreateText(path)) { } ;
            }
            using (StreamReader reader = File.OpenText(path))
            {
                JsonSerializer serializer = JsonSerializer.Create();
                JObject @object = (JObject)serializer.Deserialize(reader, typeof(JObject));
                if (@object == null)
                {
                    Version = 1;
                }
                else
                {
                    Version = @object.Value<long>("Version");
                    JArray jArray = @object.Value<JArray>("Event");
                    foreach (var obj in jArray)
                    {
                        Event e = new EntityDemoEvent(obj.Value<string>("Name"), obj.Value<string>("EventName"));
                        Events.Add(e);
                    }
                }
            }
            while(AddQueue.TryDequeue(out Event @event))
            {
                Events.Add(@event);
            }
        }

        public IList<Event> LoadEvents(long skipEvents, int maxCount)
        {
            throw new NotImplementedException();
        }

        public EventStream LoadEventStream(EntityModule id)
        {
            EventStream stream = new EventStream();
            stream.Version = Version;
            stream.Events = Events;
            return stream;
        }

        public EventStream LoadEventStream(EntityModule id, long skipEvents, int maxCount)
        {
            throw new NotImplementedException();
        }
    }
}
