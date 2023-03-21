using lojinha_backend.Model;
using lojinha_backend.Model.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
namespace lojinha_backend.Repository.Implementation
{
    public class RepositoryImp : IRepository
    {
        MSSQLContext _context;
        public RepositoryImp(MSSQLContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Cria um produto na base de dados
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public Produto Create(Produto produto)
        {
            try
            {
                _context.produtos.Add(produto);
                _context.SaveChanges(); 
                
            }
            catch (Exception)
            {
                throw;
            }
            return produto;
        }

        /// <summary>
        /// Deleta um produto da base de dados
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var result = FindById(id);
            if (result != null)
            {
                try
                {
                    _context.produtos.Remove(result);
                    _context.SaveChanges();

                }
                catch (Exception)
                {

                    throw;
                }
            }
            
        }

        /// <summary>
        /// Verifica se o produto já existe na base
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exist(int id)
        {
            return _context.produtos.Any(p=> p.Id.Equals(id));
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>Retorna tudo que está na base</returns>
        public List<Produto> FindAll()
        {
           return _context.produtos.ToList();
        }

        /// <summary>
        /// Busca um produto pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Produto FindById(int id)
        {
            return _context.produtos.SingleOrDefault(p=> p.Id.Equals(id));  
        }

        /// <summary>
        /// Procura o produto pelo nome através de uma query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Produto> FindByName(string query)
        {
            return _context.produtos.FromSqlRaw(query).ToList();
        }

        /// <summary>
        ///   Retorna a quantidade de itens no banco especificado pela query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int GetCount(string query)
        {
            var result = "";
            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    result = command.ExecuteScalar().ToString();
                }
            }
            return int.Parse(result);
        }

        /// <summary>
        /// Atualiza os dados no banco
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public Produto Update(Produto produto)
        {
            var result = FindById(produto.Id);
            if(result != null) 
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(produto);
                    _context.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null;

        }
    }
}
