using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChefsNDishes.Models;
using Microsoft.EntityFrameworkCore;

namespace ChefsNDishes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.AllChefs = _context.Chefs.Include(d => d.CreatedDishes).ToList();
            return View();
        }

        [HttpGet("dishes")]
        public IActionResult AllDishes()
        {
            ViewBag.AllDishes = _context.Dishes.Include(c => c.Creator).ToList();
            return View();
        }

        [HttpGet("chefs/new")]
        public IActionResult NewChef()
        {
            return View();
        }

        [HttpGet("dishes/new")]
        public IActionResult NewDish()
        {
            ViewBag.AllChefs = _context.Chefs.ToList();
            return View();
        }

        [HttpPost("chefs/create")]
        public IActionResult CreateChef(Chef newChef)
        {
            if (ModelState.IsValid)
            {
                // Date of Birth check start
                if (DateTime.Now.Year - newChef.DOB.Year > 18)
                {
                    _context.Add(newChef);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if (DateTime.Now.Year - newChef.DOB.Year == 18)
                {
                    if (newChef.DOB.Month < DateTime.Now.Month)
                    {
                        _context.Add(newChef);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else if (newChef.DOB.Month == DateTime.Now.Month)
                    {
                        if (newChef.DOB.Day <= DateTime.Now.Day)
                        {
                            _context.Add(newChef);
                            _context.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("DOB", "Chef must be at least 18 years old.");
                            return View("NewChef");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("DOB", "Chef must be at least 18 years old.");
                        return View("NewChef");
                    }
                }
                else
                {
                    ModelState.AddModelError("DOB", "Chef must be at least 18 years old.");
                    return View("NewChef");
                }
                // Date of Birth check end
            }
            else
            {
                return View("NewChef");
            }
        }

        [HttpPost("dishes/create")]
        public IActionResult CreateDish(Dish newDish)
        {
            if (ModelState.IsValid)
            {
                _context.Dishes.Add(newDish);
                _context.SaveChanges();
                return RedirectToAction("AllDishes");
            }
            else
            {
                ViewBag.AllChefs = _context.Chefs.ToList();

                return View("NewDish");
            }
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
