using Common.Models.Users;

namespace Application.Requests.Users.Commands.Login;

public class LoginCommandResult
{
    public User User { get; set; } = new();
}