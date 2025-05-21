using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using matala5_server.BL;

public class DBservices
{
    public DBservices()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the appsettings.json 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {
        // read the connection string from the configuration file
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString("myProjDB");
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }

    //--------------------------------------------------------------------------------------------------
    // Movies 
    //--------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method inserts a movie into the Movies table 
    //--------------------------------------------------------------------------------------------------
    public int Insert(Movies movie)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@url", movie.Url);
        paramDic.Add("@primaryTitle", movie.PrimaryTitle);
        paramDic.Add("@description", movie.Description);
        paramDic.Add("@primaryImage", movie.PrimaryImage);
        paramDic.Add("@year", movie.Year);
        paramDic.Add("@releaseDate", movie.ReleaseDate);
        paramDic.Add("@language", movie.Language);
        paramDic.Add("@budget", movie.Budget);
        paramDic.Add("@grossWorldwide", movie.GrossWorldwide);
        paramDic.Add("@genres", movie.Genres);
        paramDic.Add("@isAdult", movie.IsAdult);
        paramDic.Add("@runtimeMinutes", movie.RuntimeMinutes);
        paramDic.Add("@averageRating", movie.AverageRating);
        paramDic.Add("@numVotes", movie.NumVotes);
        paramDic.Add("@priceToRent", movie.PriceToRent);
        paramDic.Add("@rentalCount", movie.Rentalcount);

        cmd = CreateCommandWithStoredProcedureGeneral("NLM_InsertMovie", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method Delete a movie from the Movies table by ID
    //--------------------------------------------------------------------------------------------------
    public int DeleteMovieById(int id)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");// create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);

        cmd = CreateCommandWithStoredProcedureGeneral("NLM_DeleteMovie", con, paramDic);   // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method Update a Movie
    //--------------------------------------------------------------------------------------------------
    public int UpdateMovie(Movies movie)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");// create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        
        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", movie.Id);
        paramDic.Add("@url", movie.Url);
        paramDic.Add("@primaryTitle", movie.PrimaryTitle);
        paramDic.Add("@description", movie.Description);
        paramDic.Add("@primaryImage", movie.PrimaryImage);
        paramDic.Add("@year", movie.Year);
        paramDic.Add("@releaseDate", movie.ReleaseDate);
        paramDic.Add("@language", movie.Language);
        paramDic.Add("@budget", movie.Budget);
        paramDic.Add("@grossWorldwide", movie.GrossWorldwide);
        paramDic.Add("@genres", movie.Genres);
        paramDic.Add("@isAdult", movie.IsAdult);
        paramDic.Add("@runtimeMinutes", movie.RuntimeMinutes);
        paramDic.Add("@averageRating", movie.AverageRating);
        paramDic.Add("@numVotes", movie.NumVotes);
        paramDic.Add("@priceToRent", movie.PriceToRent);
        paramDic.Add("@rentalCount", movie.Rentalcount);

        cmd = CreateCommandWithStoredProcedureGeneral("NLM_UpdateMovie", con, paramDic);          // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method save a rented movie using user id into the rented Movies table 
    //--------------------------------------------------------------------------------------------------
    public int RentMovie(RentedMovie rentedMovie)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;

        try
        {
            con = connect("myProjDB");// create the connection
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@userId", rentedMovie.userId);
            paramDic.Add("@movieId", rentedMovie.movieId);
            paramDic.Add("@rentStart", rentedMovie.rentStart);
            paramDic.Add("@rentEnd", rentedMovie.rentEnd);
            paramDic.Add("@totalPrice", rentedMovie.totalPrice);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_RentMovie", con, paramDic);

            return cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            if (ex.Message.Contains("already have this movie"))
            {
                throw new Exception("You already have this movie in your collection");
            }
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();// close the db connection
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets all movies with pagination
    //--------------------------------------------------------------------------------------------------
    public MoviesPagination GetAllMovies(int pageNumber, int pageSize)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;
        List<Movies> moviesList = new List<Movies>();
        int totalCount = 0;

        try
        {
            con = connect("myProjDB");
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@pageNumber", pageNumber);
            paramDic.Add("@pageSize", pageSize);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_GetAllMovies", con, paramDic);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Movies movie = ReadMovieFromDataReader(dr);
                moviesList.Add(movie);
            }

            // Move to next result set to get total count
            dr.NextResult();
            if (dr.Read())
            {
                totalCount = Convert.ToInt32(dr["TotalCount"]);
            }

            dr.Close();

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new MoviesPagination
            {
                Movies = moviesList,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets movies by title with pagination
    //--------------------------------------------------------------------------------------------------
    public MoviesPagination GetMoviesByTitle(string title, int pageNumber, int pageSize)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;
        List<Movies> moviesList = new List<Movies>();
        int totalCount = 0;

        try
        {
            con = connect("myProjDB");
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@title", title);
            paramDic.Add("@pageNumber", pageNumber);
            paramDic.Add("@pageSize", pageSize);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_GetMoviesByTitle", con, paramDic);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Movies movie = ReadMovieFromDataReader(dr);
                moviesList.Add(movie);
            }

            // Move to next result set to get total count
            dr.NextResult();
            if (dr.Read())
            {
                totalCount = Convert.ToInt32(dr["TotalCount"]);
            }

            dr.Close();

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new MoviesPagination
            {
                Movies = moviesList,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets movies by date range with pagination
    //--------------------------------------------------------------------------------------------------
    public MoviesPagination GetMoviesByDateRange(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;
        List<Movies> moviesList = new List<Movies>();
        int totalCount = 0;

        try
        {
            con = connect("myProjDB");
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@startDate", startDate);
            paramDic.Add("@endDate", endDate);
            paramDic.Add("@pageNumber", pageNumber);
            paramDic.Add("@pageSize", pageSize);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_GetMoviesByDateRange", con, paramDic);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Movies movie = ReadMovieFromDataReader(dr);
                moviesList.Add(movie);
            }

            // Move to next result set to get total count
            dr.NextResult();
            if (dr.Read())
            {
                totalCount = Convert.ToInt32(dr["TotalCount"]);
            }

            dr.Close();

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new MoviesPagination
            {
                Movies = moviesList,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets rented movies for a user
    //--------------------------------------------------------------------------------------------------
    public List<Movies> GetRentedMovies(int userId)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;
        List<Movies> rentedMovies = new List<Movies>();

        try
        {
            con = connect("myProjDB");
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@userId", userId);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_GetRentedMovies", con, paramDic);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Movies movie = new Movies();
                movie.Id = Convert.ToInt32(dr["id"]);
                movie.Url = dr["url"].ToString();
                movie.PrimaryTitle = dr["primaryTitle"].ToString();
                movie.Description = dr["description"].ToString();
                movie.PrimaryImage = dr["primaryImage"].ToString();
                movie.Year = Convert.ToInt32(dr["year"]);
                movie.ReleaseDate = Convert.ToDateTime(dr["releaseDate"]);
                movie.Language = dr["language"].ToString();
                movie.Budget = Convert.ToDouble(dr["budget"]);
                movie.GrossWorldwide = Convert.ToDouble(dr["grossWorldwide"]);
                movie.Genres = dr["genres"].ToString();
                movie.IsAdult = Convert.ToBoolean(dr["isAdult"]);
                movie.RuntimeMinutes = Convert.ToInt32(dr["runtimeMinutes"]);
                movie.AverageRating = (float)Convert.ToDouble(dr["averageRating"]);
                movie.NumVotes = Convert.ToInt32(dr["numVotes"]);
                movie.PriceToRent = Convert.ToInt32(dr["priceToRent"]);
                movie.Rentalcount = Convert.ToInt32(dr["rentalCount"]);
            
                // Add rental information
                if (dr["rentStart"] != DBNull.Value)
                    movie.RentStart = Convert.ToDateTime(dr["rentStart"]);
                if (dr["rentEnd"] != DBNull.Value)
                    movie.RentEnd = Convert.ToDateTime(dr["rentEnd"]);
                if (dr["totalPrice"] != DBNull.Value)
                    movie.TotalPrice = Convert.ToDouble(dr["totalPrice"]);
            
                rentedMovies.Add(movie);
            }

            return rentedMovies;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method transfers a rental to another user
    //--------------------------------------------------------------------------------------------------
    public bool TransferRentalToUser(int movieId, int currentUserId, int newUserId)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;

        try
        {
            con = connect("myProjDB");
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@movieId", movieId);
            paramDic.Add("@currentUserId", currentUserId);
            paramDic.Add("@newUserId", newUserId);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_TransferRentalToUser", con, paramDic);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (SqlException ex)
        {
            // Pass up the SQL error message
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // Users 
    //--------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method inserts a user into the Users table 
    //--------------------------------------------------------------------------------------------------
    public int Register(Users user)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@name", user.Name);
        paramDic.Add("@email", user.Email);
        paramDic.Add("@passwordHash", user.PasswordHash);

        cmd = CreateCommandWithStoredProcedureGeneral("NLM_InsertUser", con, paramDic);

        try
        {
            // Read the value returned by SELECT SCOPE_IDENTITY()
            object result = cmd.ExecuteScalar();

            if (result != null && int.TryParse(result.ToString(), out int newUserId))
            {
                return newUserId;
            }
            else
            {
                // Something went wrong — no ID returned
                return -1;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }


    //--------------------------------------------------------------------------------------------------
    // This method Delete a User from the Users table by ID
    //--------------------------------------------------------------------------------------------------
    public int DeleteUser(int id)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");// create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", id);

        cmd = CreateCommandWithStoredProcedureGeneral("NLM_DeleteUser", con, paramDic);   // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method Update a User
    //--------------------------------------------------------------------------------------------------
    public int UpdateUser(int id, Users user)
    {
        using (SqlConnection con = connect("myProjDB"))
        {
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                {"@Id", id},
                {"@Email", user.Email},
                {"@PasswordHash", user.PasswordHash},
                {"@Name", user.Name},
                {"@Active", user.Active}
            };

            using (SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("NLM_UpdateUser", con, paramDic))
            {
                return cmd.ExecuteNonQuery();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method Get User By Email
    //--------------------------------------------------------------------------------------------------
    public Users GetUserByEmail(string email)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;

        try
        {
            con = connect("myProjDB");

            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@Email", email);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_GetUserByEmail", con, paramDic);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Users
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString(),
                    Email = reader["email"].ToString(),
                    PasswordHash = reader["passwordHash"].ToString(),
                    Active = (bool)reader["active"]
                };
            }

            return null; // משתמש לא נמצא
        }
        catch (Exception ex)
        {
            throw new Exception("Error while fetching user from database", ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method gets all users
    //--------------------------------------------------------------------------------------------------
    public List<Users> GetAllUsers()
    {
        SqlConnection con = null;
        SqlCommand cmd = null;
        List<Users> usersList = new List<Users>();

        try
        {
            con = connect("myProjDB");
            cmd = CreateCommandWithStoredProcedureGeneral("NLM_GetAllUsers", con, null);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Users user = new Users();
                user.Id = Convert.ToInt32(dr["id"]);
                user.Name = dr["name"].ToString();
                user.Email = dr["email"].ToString();
                user.PasswordHash = dr["passwordHash"].ToString();
                user.Active = (bool)dr["active"];
                
                usersList.Add(user);
            }

            dr.Close();
            return usersList;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    // This method toggles user active status
    //--------------------------------------------------------------------------------------------------
    public bool ToggleUserActive(int userId)
    {
        SqlConnection con = null;
        SqlCommand cmd = null;

        try
        {
            con = connect("myProjDB");
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("@userId", userId);

            cmd = CreateCommandWithStoredProcedureGeneral("NLM_ToggleUserActive", con, paramDic);
            SqlDataReader dr = cmd.ExecuteReader();

            bool isActive = false;
            if (dr.Read())
            {
                isActive = (bool)dr["active"];
            }

            dr.Close();
            return isActive;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }
    }

    //---------------------------------------------------------------------------------
    // Helper method to read movie from SqlDataReader
    //---------------------------------------------------------------------------------
    private Movies ReadMovieFromDataReader(SqlDataReader dr)
    {
        Movies movie = new Movies();
        movie.Id = Convert.ToInt32(dr["id"]);
        movie.Url = dr["url"].ToString();
        movie.PrimaryTitle = dr["primaryTitle"].ToString();
        movie.Description = dr["description"].ToString();
        movie.PrimaryImage = dr["primaryImage"].ToString();
        movie.Year = Convert.ToInt32(dr["year"]);
        movie.ReleaseDate = Convert.ToDateTime(dr["releaseDate"]);
        movie.Language = dr["language"].ToString();
        movie.Budget = Convert.ToDouble(dr["budget"]);
        movie.GrossWorldwide = Convert.ToDouble(dr["grossWorldwide"]);
        movie.Genres = dr["genres"].ToString();
        movie.IsAdult = Convert.ToBoolean(dr["isAdult"]);
        movie.RuntimeMinutes = Convert.ToInt32(dr["runtimeMinutes"]);
        movie.AverageRating = (float)Convert.ToDouble(dr["averageRating"]);
        movie.NumVotes = Convert.ToInt32(dr["numVotes"]);
        movie.PriceToRent = Convert.ToInt32(dr["priceToRent"]);
        movie.Rentalcount = Convert.ToInt32(dr["rentalCount"]);

        return movie;
    }

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

        return cmd;
    }
}