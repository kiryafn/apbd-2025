namespace LegacyApp.Interfaces;

public interface IUserService
{
    bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId);

}