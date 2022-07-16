using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Multi_Claims.Data;
using Multi_Claims.Models;

namespace Multi_Claims.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext context;

        public MovieController(ApplicationDbContext context)
        {
            this.context = context;
        }

        void CONFIG()
        {
            var subTitles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Khmer", Value = "Khmer" },
                new SelectListItem { Text = "English", Value = "English" },
                new SelectListItem { Text = "Japanese", Value = "Japanese" },
                new SelectListItem { Text = "Chinese", Value = "Chinese" },
            };

            var languages = new List<SelectListItem>
            {
                new SelectListItem { Text = "Khmer", Value = "Khmer" },
                new SelectListItem { Text = "English", Value = "English" },
                new SelectListItem { Text = "Japanese", Value = "Japanese" },
                new SelectListItem { Text = "Chinese", Value = "Chinese" },
            };

            ViewBag.SubTitles = subTitles;
            ViewBag.Languages = languages;
        }

        public IActionResult Index()
        {
            var movies = context.Movies.ToList();

            return View(movies);
        }

        public IActionResult Create()
        {
            CONFIG();

            return View();
        }

        [Authorize(Policy = "CreatePolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie model)
        {
            CONFIG();

            if (ModelState.IsValid)
            {
                context.Movies.Add(model);
                context.SaveChanges();

                TempData["SuccessMsg"] = "Create movie successfully.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Edit(int? id)
        {
            CONFIG();

            var movie = context.Movies.Find(id);

            if (movie == null || movie.MovieId == 0)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie model)
        {
            CONFIG();

            if (ModelState.IsValid)
            {
                context.Movies.Update(model);
                context.SaveChanges();

                TempData["SuccessMsg"] = "Create movie successfully.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            var movie = context.Movies.Find(id);

            if (movie == null || movie.MovieId == 0)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int? id)
        {
            var data = context.Movies.Find(id);

            if (data == null)
            {
                NotFound();
            }

            context.Movies.Remove(data);
            context.SaveChanges();

            TempData["SuccessMsg"] = "Remove movie successfully.";
            return RedirectToAction("Index");
        }
    }
}
