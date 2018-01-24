using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MovieAPI.Caching;
using MovieAPI.Helper;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{
    public class MoviesController : ApiController
    {
        private IMovieRepository _repository;
        private IMovieCacheStorage _cacheStorage;
        private readonly IUrlHelper _urlHelper;

        private const string DEFAULT_SORT_COL = "MovieId";

        public MoviesController(IMovieRepository repository, IMovieCacheStorage cacheStorage, IUrlHelper urlHelper)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (cacheStorage == null) throw new ArgumentNullException("cacheStorage");
            if (urlHelper == null) throw new ArgumentNullException("urlHelper");

            _repository = repository;
            _cacheStorage = cacheStorage;
            _urlHelper = urlHelper;
        }

        //Relative URI= /api/movies
        public HttpResponseMessage GetAllData(string sort = DEFAULT_SORT_COL, int desc = 0, string filter = null)
        {
            List<Movie> movieList;
            movieList = _cacheStorage.Retrieve<List<Movie>>(AppSettings.MovieList_CacheKey);
            if (movieList != null)
            {
                return ControllerContext.Request.CreateResponse<IEnumerable<Movie>>
                    (HttpStatusCode.OK, movieList.AsQueryable().ApplySort<Movie>(sort, desc).ApplyFilter<Movie>(filter).ToList());
            }
            else
            {
                movieList = _repository.GetAllMovies();
                if ((movieList != null) && (movieList.Count > 0))
                {
                    _cacheStorage.Store(AppSettings.MovieList_CacheKey, movieList, DateTime.Now.AddHours(AppSettings.CacheExpirationHours), TimeSpan.Zero);
                    return ControllerContext.Request.CreateResponse<IEnumerable<Movie>>
                        (HttpStatusCode.OK, movieList.AsQueryable().ApplySort<Movie>(sort, desc).ApplyFilter<Movie>(filter));
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No record found!");
            }
        }

        //Relative URI /api/books/id
        public HttpResponseMessage GetDataById(int id)
        {
            Movie movie;
            movie = _cacheStorage.Retrieve<Movie>(AppSettings.Movie_CacheKey + id);
            if (movie == null)
            {
                movie = _repository.GetMovie(id);
                if (movie != null)
                {
                    _cacheStorage.Store(AppSettings.Movie_CacheKey + id, movie, DateTime.Now.AddHours(AppSettings.CacheExpirationHours), TimeSpan.Zero);
                    return ControllerContext.Request.CreateResponse<Movie>(HttpStatusCode.OK, movie);
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No record found!");
            }
            else
                return ControllerContext.Request.CreateResponse<Movie>(HttpStatusCode.OK, movie);
        }

        public HttpResponseMessage PostMovie(Movie movie)//Create
        {
            int newMovieId = -1; 
            if (_repository.AddMovie(movie, ref newMovieId))
            {
                var response = Request.CreateResponse<Movie>(HttpStatusCode.Created, movie);
                _urlHelper.Url = Url;
                response.Headers.Location = new Uri(_urlHelper.GetLink("DefaultApi", new { id = movie.MovieId }));

                _cacheStorage.Remove(AppSettings.MovieList_CacheKey); //Refresh cache
                return response;
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, movie);
        }

        public HttpResponseMessage PutMovie(int id, Movie movie) //Update
        {
            movie.MovieId = id;

            if (_repository.UpdateMovie(movie))
            {
                var response = Request.CreateResponse<Movie>(HttpStatusCode.OK, movie);
                _urlHelper.Url = Url;
                response.Headers.Location = new Uri(_urlHelper.GetLink("DefaultApi", new { id = movie.MovieId }));

                _cacheStorage.Remove(AppSettings.MovieList_CacheKey); //Refresh cache
                return response;
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, movie);
        }

    }
}
