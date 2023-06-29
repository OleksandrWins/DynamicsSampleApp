using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.PowerPlatform.Dataverse.Client;
using DynamicsSampleApp.Interfaces;

namespace DynamicsSampleApp.Services
{
    public class Dynamics365Service : IDynamics365Service
    {
        private readonly string connectionString;
        private readonly Random random;

        public Dynamics365Service(string connectionString)
        {
            this.connectionString = connectionString;
            random = new Random();
        }

        public void CreateEntities(string entityName, List<Entity> entities)
        {
            var service = new ServiceClient(connectionString);

            EntityCollection carCollection = service.RetrieveMultiple(new FetchExpression(Constants.carFetch));

            EntityCollection classCollection = service.RetrieveMultiple(new FetchExpression(Constants.classFetch));

            var contacts = GetAllContacts(service);

            foreach (Entity entity in entities)
            {
                var cars = carCollection.Entities.ToList();
                var classes = classCollection.Entities.ToList();

                SetReservedPickupDateTime(entity);
                SetReservedHandoverDateTime(entity);
                SetCar(cars, classes, entity);
                SetCustomer(entity, contacts);
                SetLocation(entity);
                SetStatus(entity);
                ProcessStatus(entity, service);
                SetPaid(entity);

                Guid id = service.Create(entity);
            }
        }

        public void SetReservedPickupDateTime(Entity entity)
        {
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2020, 12, 31);
            TimeSpan randomizeTime = TimeSpan.FromDays(random.NextDouble() * (endDate - startDate).Days);
            DateTime reservedPickupDateTime = startDate + randomizeTime;

            entity.Attributes.Add("cr908_reservedpickup", reservedPickupDateTime);
            entity.Attributes.Add("cr908_actualpickup", reservedPickupDateTime);
        }

        public void SetReservedHandoverDateTime(Entity entity)
        {
            DateTime reservedPickupDateTime = entity.GetAttributeValue<DateTime>("cr908_reservedpickup");
            int randomizeDays = random.Next(1, 31);
            DateTime reservedHandoverDateTime = reservedPickupDateTime.AddDays(randomizeDays);

            entity.Attributes.Add("cr908_reservedhandover", reservedHandoverDateTime);
            entity.Attributes.Add("cr908_actualreturn", reservedPickupDateTime);
        }

        public void SetCar(List<Entity> cars, List<Entity> classes, Entity entity)
        {
            var carChoice = random.Next(cars.Count);
            var car = cars[carChoice];

            //through LinkedEntity get carClass - through Join

            var classresult = classes.Where(c => c.GetAttributeValue<string>("cr908_cr908_carclasscode_cr908_car_Carclass") == car.GetAttributeValue<string>("cr908_carclasscode")).FirstOrDefault();
            EntityReference carlookup = new()
            {
                Id = car.Id,
                LogicalName = car.LogicalName
            };

            EntityReference classlookup = new()
            {
                Id = classresult.Id,
                LogicalName = classresult.LogicalName
            };

            entity.Attributes.Add("cr908_rentcar", carlookup);
            entity.Attributes.Add("cr908_rentclass", classlookup);
            entity.Attributes.Add("cr908_price", classresult.GetAttributeValue<Money>("cr908_classprice"));
        }

        public void SetCustomer(Entity entity, List<Entity> customers)
        {
            var randomCustomer = customers[random.Next(customers.Count)];

            EntityReference lookup = new()
            {
                Id = randomCustomer.Id,
                LogicalName = "contact"
            };

            entity.Attributes.Add("cr908_customer", lookup);
        }

        public void SetLocation(Entity entity)
        {
            var random = new Random();
            List<OptionSetValue> locations = new() { new OptionSetValue(575430000), new OptionSetValue(575430001), new OptionSetValue(575430002) };

            var pickupLocation = random.Next(locations.Count);
            var returnLocation = random.Next(locations.Count);

            entity.Attributes.Add("cr908_pickuplocation", new OptionSetValue(575430000));
            entity.Attributes.Add("cr908_returnlocation", new OptionSetValue(575430002));
        }

        private void SetStatus(Entity entity)
        {
            double randomValue = random.NextDouble();

            //OptionSets through enum

            if (randomValue < 0.05)
                entity.Attributes.Add("cr908_status", new OptionSetValue(575430000));
            else if (randomValue < 0.1)
                entity.Attributes.Add("cr908_status", new OptionSetValue(575430001));
            else if (randomValue < 0.15)
                entity.Attributes.Add("cr908_status", new OptionSetValue(575430002));
            else if (randomValue < 0.9)
                entity.Attributes.Add("cr908_status", new OptionSetValue(575430003));
            else
                entity.Attributes.Add("cr908_status", new OptionSetValue(575430004));
        }

        public void ProcessStatus(Entity entity, ServiceClient service)
        {
            var status = entity.GetAttributeValue<OptionSetValue>("cr908_status");

            if (status.Value.Equals(575430002))
            {
                CreatePickupReport(entity, service);
            }
            else if (status.Value.Equals(575430004))
            {
                CreatePickupReport(entity, service);
                CreateReturnReport(entity, service);
            }
        }

        public void CreatePickupReport(Entity entity, ServiceClient service)
        {
            DateTime pickupDate = entity.GetAttributeValue<DateTime>("cr908_reservedpickup");
            EntityReference carid = (EntityReference)entity.Attributes["cr908_rentcar"];

            EntityReference carLookup = new()
            {
                Id = carid.Id,
                LogicalName = "cr908_car"
            };

            bool hasDamage = random.NextDouble() < 0.05;

            Entity pickupReport = new("cr908_cartransferreport");
            pickupReport.Attributes.Add("cr908_reportdate", pickupDate);
            pickupReport.Attributes.Add("cr908_cartoreport", carLookup);
            pickupReport.Attributes.Add("cr908_type", false);
            pickupReport.Attributes.Add("cr908_damages", hasDamage);
            pickupReport.Attributes.Add("cr908_damagesdescription", hasDamage ? "damage" : string.Empty);

            Guid reportId = service.Create(pickupReport);

            EntityReference pickupLookup = new()
            {
                Id = reportId,
                LogicalName = "cr908_cartransferreport"
            };

            entity.Attributes.Add("cr908_pickupreport", pickupLookup);

        }

        public void CreateReturnReport(Entity entity, ServiceClient service)
        {
            DateTime returnDate = entity.GetAttributeValue<DateTime>("cr908_reservedhandover");
            EntityReference carid = (EntityReference)entity.Attributes["cr908_rentcar"];

            EntityReference carLookup = new()
            {
                Id = carid.Id,
                LogicalName = "cr908_car"
            };

            bool hasDamage = random.NextDouble() < 0.05;

            Entity returnReport = new("cr908_cartransferreport");
            returnReport.Attributes.Add("cr908_reportdate", returnDate);
            returnReport.Attributes.Add("cr908_cartoreport", carLookup);
            returnReport.Attributes.Add("cr908_type", true);
            returnReport.Attributes.Add("cr908_damages", hasDamage);
            returnReport.Attributes.Add("cr908_damagesdescription", hasDamage ? "damage" : string.Empty);

            Guid reportId = service.Create(returnReport);

            EntityReference returnLookup = new()
            {
                Id = reportId,
                LogicalName = "cr908_cartransferreport"
            };

            entity.Attributes.Add("cr908_returnreport", returnLookup);
        }

        public void SetPaid(Entity entity)
        {
            var status = entity.GetAttributeValue<OptionSetValue>("cr908_status");

            if (status.Value.Equals(575430001) && random.NextDouble() < 0.9)
                entity.Attributes.Add("cr908_paid", true);
            else if (status.Value.Equals(575430002) && random.NextDouble() < 0.999)
                entity.Attributes.Add("cr908_paid", true);
            else if (status.Value.Equals(575430003) && random.NextDouble() < 0.9998)
                entity.Attributes.Add("cr908_paid", true);
            else
                entity.Attributes.Add("cr908_paid", false);
        }

        private static List<Entity> GetAllContacts(IOrganizationService service)
        {
            var pageNumber = 1;
            var pagingCookie = string.Empty;
            var result = new List<Entity>();
            EntityCollection resp;
            do
            {
                var query = new QueryExpression("contact") { ColumnSet = new ColumnSet("emailaddress1", "lastname") };
                //better ColumnSet true
                
                query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
                query.Criteria.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.NotNull));
                query.Criteria.FilterOperator = LogicalOperator.And;

                query.PageInfo = new PagingInfo
                {
                    PageNumber = 1,
                    Count = 5000
                };
                if (pageNumber != 1)
                {
                    query.PageInfo.PageNumber = pageNumber;
                    query.PageInfo.PagingCookie = pagingCookie;
                }
                resp = service.RetrieveMultiple(query);
                if (resp.MoreRecords)
                {
                    pageNumber++;
                    pagingCookie = resp.PagingCookie;
                }
                result.AddRange(resp.Entities.ToList());
            }

            while (resp.MoreRecords);

            return result.ToList();

            //better return entityCollection
        }
    }
}

