namespace LegacyApp;

public interface IUserValidator
{
    bool ValidateBasicUserInfo(string firstName, string lastName, string email, DateTime dateOfBirth);

}