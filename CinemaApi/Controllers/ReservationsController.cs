using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private CinemaDbContext _cinemaDbContext;
        ReservationsController(CinemaDbContext cinemaDbContext)
        {
            _cinemaDbContext = cinemaDbContext;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] Reservation reservationObj)
        {
            reservationObj.ReservationTime = DateTime.Now;
            _cinemaDbContext.Reservations.Add(reservationObj);
            _cinemaDbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReservations()
        {
            var reservartions = from reservation in _cinemaDbContext.Reservations
                               join customer in _cinemaDbContext.Users on reservation.UserId equals customer.Id
                               join movie in _cinemaDbContext.Movies on reservation.MovieId equals movie.Id
                               select new
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = customer.Id,
                                   MovieName = movie.Name
                               };

            return Ok(reservartions);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReservationDetail(int id)
        {
            var reservartionResult = (from reservation in _cinemaDbContext.Reservations
                                join customer in _cinemaDbContext.Users on reservation.UserId equals customer.Id
                                join movie in _cinemaDbContext.Movies on reservation.MovieId equals movie.Id
                                where reservation.Id == id
                                select new
                                {
                                    Id = reservation.Id,
                                    ReservationTime = reservation.ReservationTime,
                                    CustomerName = customer.Id,
                                    MovieName = movie.Name,
                                    Email = customer.Email,
                                    Qty = reservation.Qty,
                                    Price = reservation.Price,
                                    Phone = reservation.Phone,
                                    PlayingDate = movie.PlayingDate,
                                    PlayingTime = movie.PlayingTime
                                }).FirstOrDefault();

            return Ok(reservartionResult);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = _cinemaDbContext.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound("No record found against this Id");
            }
            else
            {
                _cinemaDbContext.Reservations.Remove(reservation);
                _cinemaDbContext.SaveChanges();
                return Ok("Record deleted");
            }
        }

    }
}
