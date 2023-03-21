using lojinha_backend.Model;
using lojinha_backend.Model.Context;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace lojinha_backend.Repository.Implementation
{
    public class UserRepositoryImp : IUserRepository
    {
        private readonly MSSQLContext _context;

        public UserRepositoryImp(MSSQLContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Descripitografa a senha do usuário no banco de dados e valida
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Retorna o usuário validado</returns>
        public User ValidateCredicial(User user)
        {
            var pass = ComputeHash(user.PassWord, new SHA256CryptoServiceProvider());
            return _context.users.FirstOrDefault(u => (u.UserName == user.UserName) && (u.PassWord == pass));
        }
        /// <summary>
        /// Atualiza as informações do usuário no banco
        /// </summary>
        /// <param name="user"></param>
        /// <returns>O usuário atualizado</returns>
        public User RefreshUserInfo(User user)
        {
            if (!_context.users.Any(u => u.Id.Equals(user.Id))) return null;

            var result = _context.users.SingleOrDefault(u=> u.Id.Equals(user.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }
        /// <summary>
        /// decodifica a senha 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        private string ComputeHash(string input, SHA256CryptoServiceProvider algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes);
        }

        /// <summary>
        /// Busca o usuário no banco pelo nome
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserByName(string username)
        {
            return _context.users.FirstOrDefault(u => (u.UserName == username));
        }

        /// <summary>
        ///  Invalida o refresh token do usuário na base de dados para que ele faça login novamente
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool RevokeToken(string username)
        {
            var user = _context.users.FirstOrDefault(u => (u.UserName == username));
            if (user != null) return false;
            user.RefreshToken = null;
            _context.SaveChanges();
            return true;
        }
    }
}
