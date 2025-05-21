using matala5_server.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace matala5_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        //// GET: api/<MoviesController>
        //[HttpGet]
        //public IEnumerable<Movies> Get()
        //{
        //    return Movies.Read();
        //}


        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        //[HttpGet("SearchMoviesByTitle")]
        //public IEnumerable<Movies> SearchMoviesByTitle(string title)
        //{
        //    return Movies.GetByTitle(title);
        //}
        //[HttpGet("searchByRouting/from/{startDate}/until/{endDate}")] // this uses resource routing
        //public IEnumerable<Movies> GetByReleaseDate(DateTime startDate, DateTime endDate)
        //{
        //    return Movies.GetByReleaseDate(startDate, endDate);

        //}

        // POST api/<MoviesController>
        [HttpPost]
        public int Post([FromBody] Movies movie)

        {
            return movie.Insert();
        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        public int Put(int id, [FromBody] Movies movie)
        {
            movie.Id = id;
            return movie.Update();
        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            return Movies.DeleteMovieById(id);
        }

        [HttpGet("GetRentedMovies/{userId}")]
        public IActionResult GetRentedMovies(int userId)
        {
            try
            {
                DBservices dbs = new DBservices();
                var rentedMovies = dbs.GetRentedMovies(userId);
                return Ok(rentedMovies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("TransferRental")]
        public IActionResult TransferRental([FromBody] TransferRentalRequest request)
        {
            try
            {
                DBservices dbs = new DBservices();
                bool success = dbs.TransferRentalToUser(request.MovieId, request.CurrentUserId, request.NewUserId);
                return Ok(new { success = success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllMovies(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                DBservices dbs = new DBservices();
                var result = dbs.GetAllMovies(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("GetByTitle")]
        public IActionResult GetMoviesByTitle(string title, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                DBservices dbs = new DBservices();
                var result = dbs.GetMoviesByTitle(title, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("GetByDateRange")]
        public IActionResult GetMoviesByDateRange(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                DBservices dbs = new DBservices();
                var result = dbs.GetMoviesByDateRange(startDate, endDate, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("RentMovie")]
        public IActionResult RentMovie([FromBody] RentedMovie rentedMovie)
        {
            try
            {
                DBservices dbs = new DBservices();
                int result = dbs.RentMovie(rentedMovie);

                if (result > 0)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest("Failed to rent movie");
                }
            }
            catch (Exception ex)
            {
                // Check for specific error message
                if (ex.Message.Contains("already have this movie"))
                {
                    return BadRequest("You already have this movie in your collection");
                }
                return StatusCode(500, ex.Message);
            }
        }
    }
}
