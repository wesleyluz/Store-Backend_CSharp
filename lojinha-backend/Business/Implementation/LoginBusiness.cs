using lojinha_backend.Config;
using lojinha_backend.Data.VO;
using lojinha_backend.Model;
using lojinha_backend.Repository;
using lojinha_backend.Services;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace lojinha_backend.Business.Implementation
{
    public class LoginBusiness : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfiguration _configuration;

        private IUserRepository _repository;
        private readonly ITokenService _tokenService;

        public LoginBusiness(TokenConfiguration configuration, IUserRepository repository, ITokenService tokenService)
        {
            _configuration = configuration;
            _repository = repository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// recebe as credenciais (login e senha) e valida no banco,
        /// se tiver correto retorna o usuario. Caso não esteja nulo
        /// as Claims serão geradas junto com o token e o refresh token,
        /// setando os dois valores (token e o refresh) no usuario recuperado do banco e
        /// então atualiza na base as informações do usuario.
        /// Define as datas de criação e de expiração, logo em seguida 
        /// seta as informações do token(se ele está autenticado, a data de criação,
        /// o token de acesso e o token atualizado
        /// </summary>
        /// <param name="userCredentials"></param>
        /// <returns>retorna o token configurado</returns>
        public TokenVO ValidateCredentials(User userCredentials)
        {
            var user = _repository.ValidateCredicial(userCredentials);
            if (user == null) return null;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName)
            }; 
            var acessToken = _tokenService.GenerateAcessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpiry);

            TokenVO Newtoken = GenerateNewToken(user, acessToken, refreshToken);

            return Newtoken;
        }

        /// <summary>
        /// Recebe um objeto token que ja expirou,
        /// a partir do acess token é estraido os Claims(payload com as principais informações de segurança do token).
        /// O nome do usuario é extraido das informações e é validado, 
        /// caso o usuario seja nulo, ou o refresh token não seja o dele, ou a validade do refreshtoke estiver expirado
        /// o token não é atualizado.
        /// caso esteja tudo ok, é gerado um novo token a partir do token de acesso e atualizado o refresh token
        /// o user recebe um novo refresh token e um novo token é gerado
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Novo token</returns>
        public TokenVO RefreshExpiredToken(TokenVO token)
        {
            var acessToken = token.AcessToken;
            var refreshToken = token.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(acessToken);

            var username = principal.Identity.Name;
            var user = _repository.GetUserByName(username);
            if (user == null || 
                user.RefreshToken != refreshToken || 
                user.RefreshTokenExpiryTime <= DateTime.Now) return null;

            acessToken = _tokenService.GenerateAcessToken(principal.Claims);
            refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            TokenVO Newtoken = GenerateNewToken(user, acessToken, refreshToken);

            return Newtoken;
        }

        /// <summary>
        /// Recebe o usuário, o toke de acesso e o novo refresh token
        /// atualiza as informações do usuario no banco
        /// atualiza as datas de criação e expiração do token
        /// cria um novo token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="acessToken"></param>
        /// <param name="refreshToken"></param>
        /// <returns>Novo token</returns>
        public TokenVO GenerateNewToken(User user, string acessToken, string refreshToken) 
        {
            _repository.RefreshUserInfo(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                acessToken,
                refreshToken
            );
        }

        /// <summary>
        /// Retira o refresh token do usuário
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool RevokeToken(string username)
        {
            return _repository.RevokeToken(username);
        }
    }
}
