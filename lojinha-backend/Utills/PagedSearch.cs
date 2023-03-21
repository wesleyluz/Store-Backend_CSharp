using System;
using System.Collections.Generic;

namespace lojinha_backend.Utills
{   // Paged Search cria a estrutura para paginar as buscas e facilitar a navegação na hora de consumir a api
    public class PagedSearch <T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public string SortFields { get; set; }
        public string SortDirections { get; set; }
        public Dictionary<string, Object> Filters { get; set; }
        public List<T> List { get; set; }

        public PagedSearch() { }
        
        public PagedSearch(int currentPage, int pageSize, string sortFields, string sortDirections)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            SortFields = sortFields;
            SortDirections = sortDirections;
        }

        public PagedSearch(int currentPage, int pageSize, string sortFields, string sortDirections, Dictionary<string, object> filters)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            SortFields = sortFields;
            SortDirections = sortDirections;
            Filters = filters;
        }

        public PagedSearch(int currentPage, string sortFields, string sortDirections)
                : this(currentPage,10,sortFields,sortDirections){}

        public int GetCurrentPage()
        {
            return CurrentPage == 0 ? 2: CurrentPage;
        }

        public int GetPageSize()
        {
            return PageSize == 0 ? 10 : PageSize;
        }
    }
}
