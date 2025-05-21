namespace matala5_server.BL
{
    public class Movies
    {
        // הוספתי משתנה סטטי לשמירת המזהה הבא
        private static int _nextId = 1;

        int id;
        string url;
        string primaryTitle;
        string description;
        string primaryImage;
        int year;
        DateTime releaseDate;
        string language;
        double budget;
        double grossWorldwide;
        string genres;
        bool isAdult;
        int runtimeMinutes;
        float averageRating;
        int numVotes;
        int priceToRent ;
        int rentalcount ;



        public Movies()
        {
        }

        public Movies(int id, string url, string primaryTitle, string description, string primaryImage, int year, DateTime releaseDate, string language, double budget, double grossWorldwide, string genres, bool isAdult, int runtimeMinutes, float averageRating, int numVotes, int priceToRent, int rentalcount)
        {
            Id = id;
            Url = url;
            PrimaryTitle = primaryTitle;
            Description = description;
            PrimaryImage = primaryImage;
            Year = year;
            ReleaseDate = releaseDate;
            Language = language;
            Budget = budget;
            GrossWorldwide = grossWorldwide;
            Genres = genres;
            IsAdult = isAdult;
            RuntimeMinutes = runtimeMinutes;
            AverageRating = averageRating;
            NumVotes = numVotes;
            PriceToRent = priceToRent;
            Rentalcount = rentalcount;
        }

        public int Id { get => id; set => id = value; }
        public string Url { get => url; set => url = value; }
        public string PrimaryTitle { get => primaryTitle; set => primaryTitle = value; }
        public string Description { get => description; set => description = value; }
        public string PrimaryImage { get => primaryImage; set => primaryImage = value; }
        public int Year { get => year; set => year = value; }
        public DateTime ReleaseDate { get => releaseDate; set => releaseDate = value; }
        public string Language { get => language; set => language = value; }
        public double Budget { get => budget; set => budget = value; }
        public double GrossWorldwide { get => grossWorldwide; set => grossWorldwide = value; }
        public string Genres { get => genres; set => genres = value; }
        public bool IsAdult { get => isAdult; set => isAdult = value; }
        public int RuntimeMinutes { get => runtimeMinutes; set => runtimeMinutes = value; }
        public float AverageRating { get => averageRating; set => averageRating = value; }
        public int NumVotes { get => numVotes; set => numVotes = value; }
        public int PriceToRent { get => priceToRent; set => priceToRent = value; }
        public int Rentalcount { get => rentalcount; set => rentalcount = value; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }
        public double TotalPrice { get; set; }

        public int Insert()
        {
            this.PriceToRent = new Random().Next(10, 31); 
            this.Rentalcount = 0;

            DBservices dbs = new DBservices();
            return dbs.Insert(this);
        }

        //static public List<Movies> Read()
        //{
        //    return movieslist;
        //}


        //static public List<Movies> GetByTitle(string title)
        //{
        //    List<Movies> MoviesByTitle = new List<Movies>();
        //    foreach (var movie in movieslist)
        //    {
        //        if (movie.primaryTitle.ToLower().Contains(title.ToLower()))
        //        {
        //            MoviesByTitle.Add(movie);
        //        }
        //    }
        //    return MoviesByTitle;
        //}
        //static public List<Movies> GetByReleaseDate(DateTime startDate, DateTime endDate)
        //{
        //    List<Movies> MoviesByDate = new List<Movies>();
        //    foreach (var movie in movieslist)
        //    {
        //        if (movie.ReleaseDate >= startDate && movie.releaseDate <= endDate)
        //        {
        //            MoviesByDate.Add(movie);
        //        }
        //    }
        //    return MoviesByDate;
        //}
        static public int DeleteMovieById(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.DeleteMovieById(id);
        }
        public int Update()
        {
            DBservices dbs = new DBservices();
            return dbs.UpdateMovie(this);
        }

    }
}
