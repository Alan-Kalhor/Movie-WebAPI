using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieAPI.Repository;
using MovieAPI.Helper;
using Moq;
using System.Linq;
using MovieAPI.Controllers;
using MovieAPI.Caching;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MovieAPI.Tests.Controllers
{
    [TestClass]
    public class MoviesControllerTest
    {
        private List<Movie> sampleMovieList;
        private List<Movie> emptyMovieList;
        private Movie sampleMovie;
        private Movie nullMovie;
        private Mock<IMovieRepository> mockRepository;
        private Mock<IMovieCacheStorage> cacheStorage;
        private Mock<IUrlHelper> mockUrlHelper;

        public MoviesControllerTest()
        {
            //Init some sample data for test
            sampleMovieList = new List<Movie> {
                new Movie { MovieId = 1, Title="Olympus Has Fallen", Classification="M15+", Genre="Action Adventure", ReleaseDate="2013", Rating=2, Cast = new string[] {"Gerard Butler" ,"Dylan McDermott" ,"Aaron Eckhart" ,"Angela Bassett" } },
                new Movie { MovieId = 2, Title="Man of Steel", Classification="M", Genre="Action Adventure", ReleaseDate="2013", Rating=3, Cast = new string[] {"Christopher Meloni" ,"Diane Lane" ,"Laurence Fishburne" ,"Amy Adams" } },
                new Movie { MovieId = 3, Title="Iron Man 3	", Classification="PG", Genre="Action Adventure", ReleaseDate="2013", Rating=4, Cast = new string[] {"Robert Downey Jr" ,"Robert Downey Jr" ,"Guy Pearce" ,"Guy Pearce" } },
            };
            emptyMovieList = new List<Movie> { };
            sampleMovie = new Movie { MovieId = 1, Title = "Olympus Has Fallen", Classification = "M15+", Genre = "Action Adventure", ReleaseDate = "2013", Rating = 2, Cast = new string[] { "Gerard Butler", "Dylan McDermott", "Aaron Eckhart", "Angela Bassett" } };
            nullMovie = null;

            // Mock the Movies Repository using Moq
            mockRepository = new Mock<IMovieRepository>();

            // Mock the Movies Repository using Moq
            cacheStorage = new Mock<IMovieCacheStorage>();

            // Mock the UrlHelper using Moq
            mockUrlHelper = new Mock<IUrlHelper>();
        }

        [TestMethod]
        public void GetAllMovies_ShouldReturn_AllMovies()
        {
            //Arrange
            var expectedData = this.sampleMovieList;

            mockRepository
                .Setup(x => x.GetAllMovies())
                .Returns(expectedData);

            var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.GetAllData();

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var actual = response.Content.ReadAsAsync<List<Movie>>().Result;
            Assert.AreEqual(expectedData.Count, actual.Count);
        }

        [TestMethod]
        public void GetAllMovies_ShouldReturnNotFound_AllMovies()
        {
            //Arrange
            var expectedData = this.emptyMovieList;

            mockRepository
                .Setup(x => x.GetAllMovies())
                .Returns(expectedData);

            var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.GetAllData();

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetMoviesById_ShouldReturn_RightMovie()
        {
            //Arrange
            var expectedData = this.sampleMovie;
            var movieId = this.sampleMovie.MovieId;
            mockRepository
                .Setup(x => x.GetMovie(movieId))
                .Returns(expectedData);

            var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.GetDataById(movieId);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var actual = response.Content.ReadAsAsync<Movie>().Result;
            Assert.AreEqual(expectedData.MovieId, actual.MovieId);
        }

        [TestMethod]
        public void GetMoviesById_ShouldReturnNotFound_InvalidId()
        {
            //Arrange
            var expectedData = this.nullMovie;
            var movieId = -1;
            mockRepository
                .Setup(x => x.GetMovie(movieId))
                .Returns(expectedData);

            var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.GetDataById(movieId);

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        [TestMethod]
        public void PostMovie_ShouldReturn_CreatedStatusCode()
        {
            //Arrange
            var newMovie = this.sampleMovie;
            var newMovieId = -1;

            mockRepository
                .Setup(x => x.AddMovie(newMovie,ref newMovieId))
                .Returns(true);

            mockUrlHelper
                .Setup(x => x.GetLink(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("http://localhost/webapi/Movie/");

            var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.PostMovie(newMovie);

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(response.Headers.Location);
        }

        [TestMethod]
        public void PostMovie_ShouldReturn_BadRequestStatusCode()
        {
            //Arrange
            var newMovie = nullMovie;
            var newMovieId = -1;

            mockRepository
                .Setup(x => x.AddMovie(newMovie, ref newMovieId))
                .Returns(false);

             var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.PostMovie(newMovie);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PutMovie_ShouldReturn_OK()
        {
            //Arrange
            var newMovie = this.sampleMovie;

            mockRepository
                .Setup(x => x.UpdateMovie(newMovie))
                .Returns(true);

            mockUrlHelper
                .Setup(x => x.GetLink(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("http://localhost/webapi/Movie/");

            var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.PutMovie(newMovie.MovieId, newMovie);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(response.Headers.Location);
        }

        [TestMethod]
        public void PutMovie_ShouldReturn_BadRequestStatusCode()
        {
            //Arrange
            var newMovie = this.sampleMovie;

            mockRepository
                .Setup(x => x.UpdateMovie(newMovie))
                .Returns(false);

            var controller = new MoviesController(mockRepository.Object, cacheStorage.Object, mockUrlHelper.Object);

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //Act
            var response = controller.PutMovie(newMovie.MovieId, newMovie);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
