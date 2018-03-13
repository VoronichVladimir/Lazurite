﻿using Lazurite.ActionsDomain.Attributes;
using Lazurite.MainDomain;
using System.Collections.Generic;
using System.Linq;

namespace Lazurite.Security.Permissions
{
    [HumanFriendlyName("Запретить для пользователей...")]
    public class DenyForUsersPermission : IPermission
    {
        public List<string> UsersIds { get; set; } = new List<string>();

        public ScenarioAction DenyAction { get; set; } = ScenarioAction.Execute;

        public bool IsAvailableForUser(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            if (action > DenyAction)
                return true;
            return !UsersIds.Any(x => x.Equals(user.Id));
        }
    }
}