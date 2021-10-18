using System;

namespace App4
{
    public static class Services
    {
        public static string ServiceEndPoint = CheckEnding(Helpers.ServiceConfig.GetServiceURL());

        public static Authentication.Authentication authenticationClient = new Authentication.Authentication()
        {
            Url = ServiceEndPoint + "Authentication.asmx", Timeout = 100000
        };
        public static Subscribers.Subscribers subscribersClient = new Subscribers.Subscribers()
        {
            Url = ServiceEndPoint + "Subscribers.asmx",Timeout=10000
        };
        public static Indexes.Indexes indexesClient = new Indexes.Indexes()
        {
            Url = ServiceEndPoint + "Indexes.asmx",Timeout=60000
        };
        public static Invoices.Invoices invoicesClient = new Invoices.Invoices()
        {
            Url = ServiceEndPoint + "Invoices.asmx",Timeout=15000
        };

        private static string CheckEnding(string ServiceEndPoint)
        {
            if (!ServiceEndPoint.EndsWith("/"))
                ServiceEndPoint += "/";
            if (!ServiceEndPoint.EndsWith("Services/"))
                ServiceEndPoint += "Services/";
            return ServiceEndPoint;
        }

        static Services()
        {
            var test = authenticationClient.Url;
        }
    }
}