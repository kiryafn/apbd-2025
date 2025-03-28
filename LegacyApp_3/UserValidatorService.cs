using System;

namespace LegacyApp
{
    public class UserValidator : IUserValidator
    {
        public bool ValidateBasicUserInfo(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                return false;
            }

            if (!IsAgeValid(dateOfBirth))
            {
                return false;
            }

            return true;
        }

        private bool IsAgeValid(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;

            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
            {
                age--;
            }

            return age >= 21;
        }
    }
}