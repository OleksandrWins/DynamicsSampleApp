using Microsoft.Xrm.Sdk;

namespace DynamicsSampleApp.Interfaces
{
    public interface IDynamics365Service
    {
        public void CreateEntities(string entityName, List<Entity> entities);
    }
}
