using Common.Models.Users;
using Common.Services;

namespace WebApp.Services.Mock;

public class MockUsersCrudService : IUsersCrudService
{
    public Task<User?> GetByIdAsync(Guid? userId)
    {
        var user = Users.UsersList.FirstOrDefault(x => x.Id == userId);
        User? result = null;
        if (user is not null)
        {
            result = new User
            {
                Id = user.Id,
                Email = user.Email,
                Login = user.DisplayName,
                Name = user.DisplayName,
                Permissions = user.Permissions
            };
        }
        
        return Task.FromResult(result);
    }

    public Task CheckAllUsersExistAsync(IEnumerable<string> userAccountNames)
    {
        return Task.FromResult(true);
    }

    public Task CheckAllGroupsExistAsync(IEnumerable<string> ldapGroups)
    {
        return Task.FromResult(true);
    }

    public Task<Dictionary<Guid, User>> GetByIdsAsync(IList<Guid> userGuids)
    {
        throw new NotImplementedException();
    }

    public Task<User[]> GetByAdGroups(IEnumerable<string> adGroups)
    {
        var users = Users.UsersList.Select(x => new User()
        {
            Id = x.Id,
        }).ToArray();
        return Task.FromResult(users);
    }

    public Task<IEnumerable<string>> GetAdGroups()
    {
        return Task.FromResult(Enumerable.Empty<string>());
    }
}
