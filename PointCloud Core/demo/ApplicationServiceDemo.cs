using PointCloudCore.DomainCore;
using PointCloudCore.DomainCore.Entity;
using PointCloudCore.DomainCore.EventCore;
using PointCloudCore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointCloud_Core.demo
{
    public class ApplicationServiceDemo : ApplicationService
    {

        public ApplicationServiceDemo([Name("EventStoreTest")]IEventStore eventStore):base(eventStore)
        {
            Entity= Core.Resolve<IRepository<EntityDemoIdentity, EntityDemo>>().GetEntity(new EntityDemoIdentity(100));
            eventStore.Load(Entity.ID);
        }
    }
}
