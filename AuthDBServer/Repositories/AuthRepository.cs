using AuthDB.Data;
using AuthDB.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthDBServer.Repositories
{
    public interface IAuthRepository
    {
        Task<Account?> GetAccountByAccountId(string accountId);
    }

    public class AuthRepository : IAuthRepository
    {
        protected AuthDbContext context;
        public AuthRepository(AuthDbContext context)
        {
            this.context = context;
        }

        public async Task<Account?> GetAccountByAccountId(string accountId)
        {
            return await context.Accounts.FirstOrDefaultAsync(p => p.AccountId.Equals(accountId));
        }
    }
}
