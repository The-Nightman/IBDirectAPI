using IBDirect.API.Entities;

namespace IBDirect.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Users user);
    }
}