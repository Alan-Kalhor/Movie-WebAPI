using System.Collections.Generic;
using MoviesLibrary;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Repository
{
    public class MovieRepository: IMovieRepository
    {
        private MoviesLibrary.MovieDataSource movieDS = new MoviesLibrary.MovieDataSource();

        static MovieRepository()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<Movie, MovieData>();
                cfg.CreateMap<MovieData, Movie>();
                cfg.CreateMap<List<Movie>, List<MovieData>>();
            });
        }

        public List<Movie> GetAllMovies()
        {
            List<MovieData> movieDataList =  movieDS.GetAllData();
            List<Movie> movieList = new List<Movie>() ;
            movieList = Mapper.Map<List<Movie>>(movieDataList);
            return movieList;
        }

        public Movie GetMovie(int id)
        {
            MovieData movieData = movieDS.GetDataById(id);
            Movie movie = Mapper.Map<Movie>(movieData);
            return movie;
        }

        public bool AddMovie(Movie movie, ref int newid)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(movie, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(movie, context, results,true);
            if (isValid)
            {
                MovieData movieData = Mapper.Map<MovieData>(movie);
                newid = movieDS.Create(movieData);
                return true;
            }
            else
                return false;
        }

        public bool UpdateMovie(Movie movie)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(movie, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(movie, context, results, true);
            if (isValid)
            {
                MovieData movieData = Mapper.Map<MovieData>(movie);
                movieDS.Update(movieData);
                return true;
            }
            else
                return false;
        }

    }
}