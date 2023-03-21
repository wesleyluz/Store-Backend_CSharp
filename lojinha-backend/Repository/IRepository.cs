using lojinha_backend.Model;
using System.Collections.Generic;

namespace lojinha_backend.Repository
{
    public interface IRepository
    {
        public Produto Create(Produto produto);
        public Produto FindById(int id);
        public List<Produto> FindByName(string query);
        public Produto Update(Produto produto); 
        public void Delete(int id);
        public bool Exist(int id);
        public int GetCount(string query);
        public List<Produto> FindAll();
    }
}
