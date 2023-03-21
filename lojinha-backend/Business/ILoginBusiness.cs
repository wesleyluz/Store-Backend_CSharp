using lojinha_backend.Data.VO;
using lojinha_backend.Model;

namespace lojinha_backend.Business
{
    public interface ILoginBusiness
    {
        TokenVO ValidateCredentials(User user);
        TokenVO RefreshExpiredToken(TokenVO token);
        bool RevokeToken(string username);

    }
}
