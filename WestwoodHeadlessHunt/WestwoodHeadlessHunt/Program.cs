using log4net;
using log4net.Config;
using SaneWeb.Resources.Attributes;
using SaneWeb.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WestwoodHeadlessHunt.Controllers;

namespace WestwoodHeadlessHunt
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            Logger.Info("Starting webserver...");
            SaneServer server = SaneServer.CreateServer(new SaneServerConfiguration(SaneServerConfiguration.SaneServerPreset.DEFAULT, Assembly.GetExecutingAssembly(), "WestwoodHeadlessHunt"));
            server.SetErrorHandler(ErrorHandler);

            server.AddController(typeof(HomeController));

            server.Run();
            Logger.Info("Webserver live!");

            Console.ReadKey();
            server.Stop();
        }

        public static void ErrorHandler(Object sender, SaneErrorEventArgs e)
        {
            Logger.Error("Unexpected error - " + e.Exception.Message + e.Exception.StackTrace);
            e.Propogate = false;
        }
    }
}
