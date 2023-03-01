using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Movie_Exercise.Data;
using Movie_Exercise.Models;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Movie_Exercise.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _db;
        public MovieService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Get all Movies in the data base
        public async Task<List<Movie>> GetAllMovies()
        {
            return await Task.Run(() => _db.Movies.ToList());
        }

        //Get Movie By Id
        public Movie GetMovieById(int? id)
        {
            Movie specificMovieById = _db.Movies
                .FirstOrDefault(x => x.Id == id);
            return specificMovieById;
        }

        public void AddMovie(Movie newMovie)
        {
            _db.Add(newMovie);
            _db.SaveChanges();
        }

        public void UpdateMovie(Movie movie)
        {
            _db.Update(movie);
            _db.SaveChanges();
        }


        public void RemoveMove(Movie movie)
        {
            _db.Remove(movie);
            _db.SaveChanges();
        }

        public bool IsExists(int? id)
        {
            return _db.Movies.Any(e => e.Id == id);
        }

    }
}
