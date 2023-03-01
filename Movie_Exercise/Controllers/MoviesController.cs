using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_Exercise.Data;
using Movie_Exercise.Models;
using Movie_Exercise.Services;

namespace Movie_Exercise.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieservice;

        public MoviesController(IMovieService movieService)
        {
            _movieservice = movieService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
              return View(await Task.Run(()=> _movieservice.GetAllMovies()));
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _movieservice.GetAllMovies() == null)
            {
                return NotFound();
            }

            var movie = await Task.Run(()=>_movieservice.GetMovieById(id));
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => _movieservice.AddMovie(movie));
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _movieservice.GetAllMovies() == null)
            {
                return NotFound();
            }

            //var movie = await _context.Movies.FindAsync(id);
            var movie = await Task.Run(()=> _movieservice.GetMovieById(id));

            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(() => _movieservice.UpdateMovie(movie));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _movieservice.GetAllMovies() == null)
            {
                return NotFound();
            }

            var movie = await Task.Run(() => _movieservice.GetMovieById(id));
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_movieservice.GetAllMovies() == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movies'  is null.");
            }
            var movie = await Task.Run(() => _movieservice.GetMovieById(id));
            if (movie != null)
            {
                await Task.Run(() => _movieservice.RemoveMove(movie));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return _movieservice.IsExists(id);
        }
    }
}
