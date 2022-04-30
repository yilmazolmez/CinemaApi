using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _cinemaDbContext;
        public MoviesController(CinemaDbContext cinemaDbContext)
        {
            _cinemaDbContext = cinemaDbContext;
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(string sort,int? pageNumber,int? pageSize)
        {
            var currentPageNumber =  pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;
            var movies = from movie in _cinemaDbContext.Movies
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             Duration = movie.Duration,
                             Language = movie.Language,
                             Rating = movie.Rating,
                             Genre = movie.Genre,
                             ImageUrl = movie.ImageUrl
                         };

            switch (sort)
            {
                case "desc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(x => x.Rating));
                case "asc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(x=>x.Rating));
                default:
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
            }
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie = _cinemaDbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("No record found against this Id");

            }
            return Ok(movie);
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult FindMovies(string movieName)
        {
            var movies = from movie in _cinemaDbContext.Movies
                         where movie.Name.StartsWith(movieName)
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             ImageUrl = movie.ImageUrl
                         };

            return Ok(movies);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");

            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }
            movieObj.ImageUrl = filePath.Remove(0, 7);//0-7 deleted
            _cinemaDbContext.Movies.Add(movieObj);
            _cinemaDbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _cinemaDbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("No record found against this Id");
            }

            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");

            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);

                movie.ImageUrl = filePath.Remove(0, 7);//0-7 deleted
            }

            movie.Name = movieObj.Name;
            movie.Description = movieObj.Description;
            movie.Language = movieObj.Language;
            movie.Duration = movieObj.Duration;
            movie.PlayingDate = movieObj.PlayingDate;
            movie.PlayingTime = movieObj.PlayingTime;
            movie.Genre = movieObj.Genre;
            movie.Language = movieObj.Language;
            movie.TrailorUrl = movieObj.TrailorUrl;
            movie.TicketPrice = movieObj.TicketPrice;
            movie.Rating = movieObj.Rating;

            _cinemaDbContext.SaveChanges();

            return Ok("Record updated successfully");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movie = _cinemaDbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("No record found against this Id");
            }

            _cinemaDbContext.Movies.Remove(movie);
            _cinemaDbContext.SaveChanges();

            return Ok("Record deleted");
        }
    }
}
