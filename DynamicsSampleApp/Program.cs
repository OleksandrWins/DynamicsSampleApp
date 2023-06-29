using Microsoft.Xrm.Sdk;
using DynamicsSampleApp.Interfaces;
using DynamicsSampleApp.Helpers;
using DynamicsSampleApp.Services;

namespace DynamicsSampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            IDynamics365Service dynamics365Service = new Dynamics365Service(Constants.connectionString);

            List<Entity> entities = CreateRentEntities.GenerateRentEntities(200); 

            dynamics365Service.CreateEntities("cr908_rent", entities);

            Console.ReadLine();
        }

    }
}




