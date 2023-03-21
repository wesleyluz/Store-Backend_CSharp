using lojinha_backend.Utills;
using lojinha_backend.Model;
using lojinha_backend.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace lojinha_backend.Business.Implementation
{
    public class ProdutoBusinessImp : IProdutoBusiness
    {
        private readonly IRepository _repository;

        public ProdutoBusinessImp(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///   Recebe um produto do end-point verifica se ele existe na base de dados
        ///   se não existir ele é adicionado a base.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public Produto Create(Produto produto)
        {
            if (!_repository.Exist(produto.Id)) 
            {
                _repository.Create(produto);
                return produto;
            }
            return null;

        }

        /// <summary>
        /// Deleta um produto pelo id, após isso verifica se o produto ainda existe caso não retorna que foi deletado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
           _repository.Delete(id);
            var deleted = _repository.Exist(id) ? false : true;
            return deleted;
        }

        public List<Produto> FindAll()
        {
            return _repository.FindAll();
        }

        /// <summary>
        ///  Procura um produto pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Produto FindById(int id)
        {
           return _repository.FindById(id);
        }

        /// <summary>
        ///  Através do end-point recebe os parametros solicitados,
        ///  é verificado se o tipo de ordenação está em branco ou errado então é setado como asc (acendente) ou desc (decrescente)
        ///  a quantidade de itens na pagina(pageSize) é setado por padrão como 10 caso seja menor que 1
        ///  e o tamanho dos itens que ficarão de fora da lista(offset) é setado como 0 por padrão
        ///  recebida todas as informações elas são jogadas em uma Query e buscadas no banco
        ///  o Objeto PagedSearch é criado para melhor controle e manipulação pelo front-end,
        ///  Contendo a pagina atual, a lista de produtos, quantos por pagina, o tipo de ordenação, o total de resultados e o total de paginas.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageSize"></param>
        /// <param name="currPage"></param>
        /// <returns>O objeto PagedSearch</returns>
        public PagedSearch<Produto> FindByName(string name, string sortDirection, int pageSize, int currPage)
        {
            var sort = (!string.IsNullOrEmpty(sortDirection) && !sortDirection.Equals("desc")) ? "asc" : "desc";
            var size = (pageSize < 1) ? 10 : pageSize;
            var offset = (currPage > 0) ? (pageSize * size) : 0;
            var query = $"SELECT * from dbo.Produtos p WHERE p.nome LIKE '%{name}%' order by p.Id {sort} OFFSET {offset} ROWS FETCH NEXT {size} ROWS ONLY";
            var countQuery = $"SELECT COUNT(*) FROM dbo.Produtos p where p.nome LIKE '%{name}%'";

            var produtos = _repository.FindByName(query);
            int totalResults = _repository.GetCount(countQuery);
            var result = decimal.Divide(totalResults, size);
            int totalPages = (int) Math.Ceiling(result);

            return new PagedSearch<Produto>
            {
                CurrentPage = currPage,
                List = produtos,
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Atualiza as informações do produto na tabela através das novas informações recebidas.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public Produto Update(Produto produto)
        {
            return _repository.Update(produto);

        }
    }
}
