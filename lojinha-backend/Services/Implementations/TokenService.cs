using lojinha_backend.Config;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace lojinha_backend.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private TokenConfiguration _configuration;

        public TokenService(TokenConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// É gerado uma chave simetrica (secretkey) baseada em uma chave configurada no appsettings.
        /// Após isso é passado essa chave para gerar um signinCredentials usando HmacSha256
        /// logo em seguia é gerado as options do token.
        /// </summary>
        /// <param name="claims"></param>
        /// Claims são o payload objetos json que contem informações de segurança
        /// <returns> As opções configuradas do token </returns>
        public string GenerateAcessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret));
            var signinCredentials = new SigningCredentials(secretKey,SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_configuration.Minutes),
                signingCredentials:signinCredentials
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }

        /// <summary>
        ///  Gera um valor aleatório que será atribuido as informações do token, servindo como uma credencial 
        ///  para atualizar o access token
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            var radomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                 rng.GetBytes(radomNumber);
                return Convert.ToBase64String(radomNumber);
            }
        }

        /// <summary>
        /// Recebe um token e recupera as informações de segurança importantes do token expirado
        /// reseta os parametros de validação 
        /// cria um tokenhandler para uma nova validação
        /// remove as infromações de segurança do token
        /// Se o token for nulo ou não for igual a validação de segurança retorna um token invalido
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Principais informações de segurança</returns>
        /// <exception cref="SecurityTokenException"></exception>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = false, 
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token,tokenValidationParameters,out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCulture)) throw new SecurityTokenException("Invalid Token");
            return principal;
        }
    }
}
