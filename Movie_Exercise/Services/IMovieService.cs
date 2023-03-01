using Movie_Exercise.Models;

namespace Movie_Exercise.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMovies();
        Movie GetMovieById(int? id);
        void AddMovie(Movie newMovie);
        void UpdateMovie(Movie movie);
        void RemoveMove(Movie movie);
        bool IsExists(int? id);

    }
}
