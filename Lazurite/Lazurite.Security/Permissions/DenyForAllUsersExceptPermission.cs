﻿using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain;
using System.Collections.Generic;
using System.Linq;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для всех пользователей, кроме...")]
    public class DenyForAllUsersExceptPermission : IPermission
    {
        public List<string> UsersIds { get; set; } = new List<string>();
        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source)
        {
            return user is SystemUser || UsersIds.Any(x => x.Equals(user.Id));
        }
    }
}
