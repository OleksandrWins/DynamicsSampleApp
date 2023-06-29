namespace DynamicsSampleApp
{
    public class Constants
    {
        public static readonly string url = "https://org4be75c22.crm4.dynamics.com/";

        public static readonly string username = "Gavryliuk@Educational821.onmicrosoft.com";

        public static readonly string password = "k6w3JX@fL48KSrT";

        public static readonly string connectionString = $@"
        AuthType = OAuth;
        Url = {url};
        UserName = {username};
        Password = {password};
        AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
        RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
        LoginPrompt=Auto;
        RequireNewInstance = True";

        public static readonly string carFetch = @"
                                  <fetch distinct= 'false' mapping= 'logical' output-format= 'xml-platform' version= '1.0'>
                                  <entity name='cr908_car'>
                                  <attribute name='cr908_vin'/>
                                  <attribute name='cr908_purchasedate'/>
                                  <attribute name='cr908_productiondate'/>
                                  <attribute name='cr908_carmodel'/>
                                  <attribute name='cr908_carmanufacturer'/>
                                  <attribute name='cr908_carclass'/>
                                  <attribute name='cr908_carid'/>
                                  <order descending='false' attribute='cr908_vin'/>
                                  <link-entity name='cr908_carclasscode' alias='ab' link-type='inner' to='cr908_carclass' from='cr908_carclasscodeid'>
                                  </link-entity>
                                  </entity>
                                  </fetch>";

        public static readonly string classFetch = @"<fetch distinct = 'true' no-lock='false' mapping ='logical' version = '1.0'>
                                    <entity name='cr908_carclasscode'>
                                    <attribute name ='cr908_code'/>
                                    <order descending ='false' attribute = 'cr908_code'/>
                                    <attribute name ='cr908_classprice'/>
                                    <attribute name= 'cr908_classdescription'/>
                                    <attribute name='cr908_carclasscodeid'/>
                                    </entity>
                                    </fetch>";
    }
}
