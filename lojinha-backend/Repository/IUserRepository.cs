using lojinha_backend.Model;

namespace lojinha_backend.Repository
{
    public interface IUserRepository
    {
        User ValidateCredicial(User user);
        User GetUserByName(string username);
        bool RevokeToken(string username);
        User RefreshUserInfo(User user);

    }
}
