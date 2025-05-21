using Konscious.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace matala5_server.BL
{
    public class Users
    {
        private static int _nextId = 1;
        private int id;
        private string name;
        private string email;
        private string passwordHash;
        private bool active = true;


        public Users() { }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string PasswordHash { get => passwordHash; set => passwordHash = value; }
        public bool Active { get => active; set => active = value; }

        //public static List<Users> Read() { return usersList; }
        public static Users Register(Users user)
        {
            if (!user.IsValidEmail(user.Email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            user.PasswordHash = HashPassword(user.PasswordHash);

            DBservices dbs = new DBservices();
            int userId = dbs.Register(user); 

            if (userId > 0)
            {
                user.Id = userId;
                return user;
            }
            else
            {
                throw new Exception("Failed to register user.");
            }
        }


        public static Users Login(string email, string password)
        {
            DBservices dbs = new DBservices();
            Users user = dbs.GetUserByEmail(email);

            if (user == null) return null; 

            return VerifyPassword(password, user.PasswordHash) ? user : null;
        }



        //public static List<Users> GetAllUsers()
        //{
        //    return usersList.Where(u => u.Active).ToList();
        //}

        public static int DeleteUser(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.DeleteUser(id);
        }


        public static bool Update(int id, Users user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                // הצפנת הסיסמה החדשה
                user.PasswordHash = HashPassword(user.PasswordHash);
            }

            try
            {
                DBservices dbs = new DBservices();
                return dbs.UpdateUser(id, user) == 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // Hash password using Argon2
        public static string HashPassword(string password)
        {
            byte[] salt = GenerateSalt(); // 16 bytes
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            byte[] hash = argon2.GetBytes(32);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                string[] parts = hashedPassword.Split(':');
                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] expectedHash = Convert.FromBase64String(parts[1]);

                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
                {
                    Salt = salt,
                    DegreeOfParallelism = 8,
                    MemorySize = 65536,
                    Iterations = 4
                };

                byte[] actualHash = argon2.GetBytes(32);

                return actualHash.SequenceEqual(expectedHash);
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

    }
}
