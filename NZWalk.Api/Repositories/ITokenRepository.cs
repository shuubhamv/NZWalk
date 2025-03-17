using Microsoft.AspNetCore.Identity;

namespace NZWalk.Api.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user,List<string> roles);
    }
}
