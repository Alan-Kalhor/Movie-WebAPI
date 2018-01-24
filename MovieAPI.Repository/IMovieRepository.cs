using System.Collections.Generic;

namespace MovieAPI.Repository
{
    public interface IMovieRepository
    {
        List<Movie> GetAllMovies();
        Movie GetMovie(int id);
        bool AddMovie(Movie movie, ref int newid);
        bool UpdateMovie(Movie movie);
    }
}
