using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using netcoreapp2._1.Models;
using OpenTelemetry.Trace;

namespace netcoreapp2._1.Controllers
{
    public class HomeController : Controller
    {
        private readonly Tracer _tracer;

        public HomeController(Tracer tracer)
        {
            _tracer = tracer;
        }

        public IActionResult Index()
        {
            using (var span = _tracer.StartActiveSpan("index"))
            {
                span.SetAttribute("foo", "bar");
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
