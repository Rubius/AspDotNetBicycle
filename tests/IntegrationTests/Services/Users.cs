using Common.Models.Users;
using System;

namespace IntegrationTests.Services;

public class Users
{
    public static User GetUser() => new()
    {
        Id = new Guid("88c7101ec94c41abad7dc17d734675ad"),
        Name = "TestUser",
        Email = string.Empty,
        Login = "testlogin",
        Permissions = Enum.GetValues<Permission>()
    };
}