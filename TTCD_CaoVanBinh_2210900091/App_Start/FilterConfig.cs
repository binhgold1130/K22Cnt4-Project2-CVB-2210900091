using System.Web;
using System.Web.Mvc;

namespace TTCD_CaoVanBinh_2210900091
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
