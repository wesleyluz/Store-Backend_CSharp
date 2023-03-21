using System.Collections.Generic;
using System.Security.Claims;

namespace lojinha_backend.Services
{
    public interface ITokenService
    {
        //Gera o token através das claims
        string GenerateAcessToken(IEnumerable<Claim> claims);
        //Gera o token para atualização
        string GenerateRefreshToken();
        //Extrai os claims de um token expirado 
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    }
}
