using lojinha_backend.Utills;
using lojinha_backend.Model;
using System.Collections.Generic;

namespace lojinha_backend.Business
{
    public interface IProdutoBusiness
    {
        public Produto Create(Produto produto);
        public Produto Update(Produto produto);
        public PagedSearch<Produto> FindByName(string name, string sortDirection, int pageSize, int currPage);
        public List<Produto> FindAll();
        public Produto FindById(int id);
        public bool Delete(int id);

    }
}
