using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaApi.ControllersEski
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


        // GET: api/<MoviesController>
        [HttpGet]
        public IActionResult Get()
        {
            //return _cinemaDbContext.Movies;
            return Ok(_cinemaDbContext.Movies);
            //return NotFound(); ;
            //return StatusCode(401);
            //return StatusCode(StatusCodes.Status200OK);
        }

        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var movie = _cinemaDbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("No record found against this Id");

            }
            return Ok(movie);
        }

        [HttpGet("[action]/{id}")] // api/movies/Test/2
        public void Test(int id)
        {

        }

        // POST api/<MoviesController>
        //[HttpPost]
        //public IActionResult Post([FromBody] Movie movieObj)
        //{
        //    _cinemaDbContext.Movies.Add(movieObj);
        //    _cinemaDbContext.SaveChanges();

        //    return StatusCode(StatusCodes.Status201Created);
        //}

        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid+".jpg");

            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath,FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }
            movieObj.ImageUrl = filePath.Remove(0,7);//0-7 deleted
            _cinemaDbContext.Movies.Add(movieObj);
            _cinemaDbContext.SaveChanges();

            return Ok(StatusCodes.Status201Created);
        }

        // PUT api/<MoviesController>/5
        //[HttpPut("{id}")]
        //public IActionResult Put(int id, [FromBody] Movie movieObj)
        //{
        //    var movie = _cinemaDbContext.Movies.Find(id);

        //    if (movie == null)
        //    {
        //        return NotFound("No record found against this Id");
        //    }

        //    movie.Name = movieObj.Name;
        //    movie.Language = movieObj.Language;
        //    movie.Rating = movieObj.Rating;

        //    _cinemaDbContext.SaveChanges();

        //    return Ok("Record updated successfully");
        //}
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
            movie.Language = movieObj.Language;
            movie.Rating = movieObj.Rating;

            _cinemaDbContext.SaveChanges();

            return Ok("Record updated successfully");
        }


        // DELETE api/<MoviesController>/5
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
