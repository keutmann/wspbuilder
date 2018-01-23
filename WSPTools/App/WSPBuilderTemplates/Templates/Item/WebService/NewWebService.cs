using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;


namespace $DefaultNamespace$
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class $rootname$ : System.Web.Services.WebService
    {

        public $rootname$() 
        {
            
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

    }

}