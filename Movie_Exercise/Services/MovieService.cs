using Movie_Exercise.Data;

namespace Movie_Exercise.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _db;
        public MovieService(ApplicationDbContext db)
        {
            _db = db;
        }

        
    }
}
