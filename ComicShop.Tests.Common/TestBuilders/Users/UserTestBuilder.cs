using System;
using ComicShop.Application.Features.Users;
using ComicShop.Domain.Features.Users;

namespace ComicShop.Tests.Common.TestBuilders.Users
{
    public static class UserTestBuilder
    {
        #region User

        public static User CreateUser()
        {
            return new User();
        }

        public static User WithType(this User user, int type)
        {
            user.Type = type;
            return user;
        }

        public static User WithName(this User user, string name)
        {
            user.Name = name;
            return user;
        }

        public static User WithEmail(this User user, string email)
        {
            user.Email = email;
            return user;
        }

        public static User WithPassword(this User user, string password)
        {
            user.Password = password;
            return user;
        }

        public static User WithId(this User user, Guid id)
        {
            user.Id = id;
            return user;
        }

        #endregion

        #region UserCreateCommand

        public static UserCreate.Command CreateUserCommand()
        {
            return new UserCreate.Command();
        }

        public static UserCreate.Command WithType(this UserCreate.Command userCreateCommand, int type)
        {
            userCreateCommand.Type = type;
            return userCreateCommand;
        }

        public static UserCreate.Command WithName(this UserCreate.Command userCreateCommand, string name)
        {
            userCreateCommand.Name = name;
            return userCreateCommand;
        }

        public static UserCreate.Command WithEmail(this UserCreate.Command userCreateCommand, string email)
        {
            userCreateCommand.Email = email;
            return userCreateCommand;
        }

        public static UserCreate.Command WithPassword(this UserCreate.Command userCreateCommand, string password)
        {
            userCreateCommand.Password = password;
            return userCreateCommand;
        }

        #endregion
    }
}
