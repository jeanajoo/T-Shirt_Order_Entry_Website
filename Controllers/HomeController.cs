using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Joo_Jeana_HW5.Models;

namespace Joo_Jeana_HW5.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}