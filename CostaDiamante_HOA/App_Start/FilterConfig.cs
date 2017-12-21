using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //Add this line
            filters.Add(new AuthorizeAttribute());
        }
    }
}
