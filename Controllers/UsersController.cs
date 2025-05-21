using matala5_server.BL;
using Microsoft.AspNetCore.Mvc;

namespace matala5_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        //public IEnumerable<Users> Get()
        //{
        //    return Users.Read();
        //}

        

        [HttpPost]
        public Users Post([FromBody] Users user)
        {
            return Users.Register(user);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Users user)
        {
            try
            {
                var loggedInUser = Users.Login(user.Email, user.PasswordHash);
        
                if (loggedInUser == null)
                {
                    return NotFound("Invalid email or password");
                }
        
                if (!loggedInUser.Active)
                {
                    return Unauthorized("Your account has been deactivated. Please contact support.");
                }
        
                return Ok(loggedInUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            return Users.DeleteUser(id);
        }

        [HttpPut("Update/{id}")]
        public IActionResult Put([FromRoute] int id, [FromBody] Users user)
        {
            try
            {
                if (Users.Update(id, user))
                {
                    return Ok("User updated successfully.");
                }
                else
                {
                    return NotFound("User not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the user: " + ex.Message);
            }
        }       

        [HttpGet("getByEmail")]
        public IActionResult GetByEmail(string email)
        {
            DBservices dbs = new DBservices();
            var user = dbs.GetUserByEmail(email);
            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                DBservices dbs = new DBservices();
                var users = dbs.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("ToggleActive/{userId}")]
        public IActionResult ToggleUserActive(int userId)
        {
            try
            {
                DBservices dbs = new DBservices();
                bool isActive = dbs.ToggleUserActive(userId);
                return Ok(new { isActive = isActive });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }     
    }
}
