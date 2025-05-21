using System.Collections.Generic;

namespace matala5_server.BL
{
    public class MoviesPagination
    {
        public List<Movies> Movies { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}