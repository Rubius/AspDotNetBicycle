using Common.Models.Users;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationTests.Services;

public class TestUsersCrudService : IUsersCrudService
{
    public TestUsersCrudService()
    {
        User = Users.GetUser();
    }

    public User User { get; }
    public Task CheckAllGroupsExistAsync(IEnumerable<string> ldapGroups)
    {
        return Task.CompletedTask;
    }

    public Task CheckAllUsersExistAsync(IEnumerable<string> userAccountNames)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetAdGroups()
    {
        return Task.FromResult(Enumerable.Empty<string>());
    }

    public Task<User[]> GetByAdGroups(IEnumerable<string> adGroups)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid? userId)
    {
        return User.Id != userId
            ? Task.FromResult((User?)null)
            : Task.FromResult((User?)User);
    }

    public Task<Dictionary<Guid, User>> GetByIdsAsync(IList<Guid> userGuids)
    {
        return userGuids.Contains(User.Id)
            ? Task.FromResult(new Dictionary<Guid, User> { { User.Id, User } })
            : Task.FromResult(new Dictionary<Guid, User>());
    }
}