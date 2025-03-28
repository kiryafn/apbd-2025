using System;
using LegacyApp.Interfaces;

namespace LegacyApp
{
    public class UserService : IUserService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserCreditService _userCreditService;
        private readonly IUserValidator _userValidator;

        public UserService() : this(new ClientRepository(), new UserCreditService(), new UserValidator()) {}

        public UserService(IClientRepository clientRepository, IUserCreditService userCreditService, IUserValidator userValidator)
        {
            _clientRepository = clientRepository;
            _userCreditService = userCreditService;
            _userValidator = userValidator;
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!_userValidator.ValidateBasicUserInfo(firstName, lastName, email, dateOfBirth))
            {
                return false;
            }

            var client = _clientRepository.GetById(clientId);
            if (client == null)
            {
                return false;
            }

            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);

            AssignCreditLimit(user, client);

            if (!ValidateCredit(user))
            {
                return false;
            }
            
            UserDataAccess.AddUser(user);

            return true;
        }

        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
            return new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName,
                HasCreditLimit = false,
                CreditLimit = 0
            };
        }

        private void AssignCreditLimit(User user, Client client)
        {
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else if (client.Type == "ImportantClient")
            {
                user.HasCreditLimit = true;
                user.CreditLimit = GetAdjustedCreditLimit(user, 2);
            }
            else
            {
                user.HasCreditLimit = true;
                user.CreditLimit = GetAdjustedCreditLimit(user, 1);
            }
        }

        private int GetAdjustedCreditLimit(User user, int multiplier)
        {
            var creditLimit = _userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
            return creditLimit * multiplier;
        }

        private bool ValidateCredit(User user)
        {
            return !user.HasCreditLimit || user.CreditLimit >= 500;
        }
    }
}