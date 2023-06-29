using Microsoft.Xrm.Sdk;

namespace DynamicsSampleApp.Helpers
{
    public class CreateRentEntities
    {
        public static List<Entity> GenerateRentEntities(int count)
        {
            List<Entity> entities = new();

            for (int i = 0; i < count; i++)
            {
                entities.Add(new Entity("cr908_rent"));
            }

            return entities;
        }
    }
}
