using DUTC.Data;

namespace DUTC.Repository.Interface
{
    public interface IAccountRepository
    {
        public Task<User> changedRole(string id);
    }
}
