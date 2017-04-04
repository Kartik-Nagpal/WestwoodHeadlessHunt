using SaneWeb.Resources.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WestwoodHeadlessHunt.Controllers
{
    public static class HomeController
    {
        [DataBoundView("/index.html")]
        public static Object Index(HttpListenerContext context)
        {
            return new Object();
        }

        [Controller("/headGallery", APIType.GET, "application/json")]
        public static byte[] GetHeadGallery(HttpListenerContext context, String body)
        {
            //TODO:
            return new byte[] { };
        }

        [Controller("/uploadHead", APIType.POST, "application/json")]
        public static byte[] UploadHeadImage(HttpListenerContext context, String body)
        {
            //TODO:
            return new byte[] { };
        }
    }
}
