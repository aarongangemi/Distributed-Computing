﻿using System.Web;
using System.Web.Mvc;

namespace Tutorial_3_Web_Service
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
