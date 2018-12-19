﻿using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Utils;
using System;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;

namespace Lazurite.Windows.Service
{
    public class LoginValidator : UserNamePasswordValidator
    {
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly WarningHandlerBase WarningHandler = Singleton.Resolve<WarningHandlerBase>();

        private static BruteforceChecker BruteforceChecker = new BruteforceChecker();

        public override void Validate(string userName, string password)
        {
            try
            {
                if (BruteforceChecker.IsWaitListContains(userName))
                    WriteAndThrowBruteforceTry(userName);
                WarningHandler.Debug("Authentication try: " + userName);
                var passwordHash = CryptoUtils.CreatePasswordHash(password);
                var user = UsersRepository.Users.SingleOrDefault(x => x.Login.Equals(userName) && x.PasswordHash.Equals(passwordHash));
                if (user == null)
                {
                    if (BruteforceChecker.CheckIsBruteforce(userName))
                        WriteAndThrowBruteforceTry(userName);
                    else
                        throw new FaultException("Login or password not valid", new FaultCode(ServiceFaultCodes.AccessDenied));
                }
                WarningHandler.Debug("Authentication success: " + userName);
            }
            catch (FaultException e)
            {
                WarningHandler.Debug("Authentication failed: " + userName);
                throw e;
            }
            catch (Exception e)
            {
                WarningHandler.InfoFormat(e, "Error while user authenticate: [{0}]", userName);
                throw new FaultException("Error while user authenticate: " + userName, new FaultCode(ServiceFaultCodes.InternalError));
            }
        }

        private void WriteAndThrowBruteforceTry(string userName)
        {
            WarningHandler.InfoFormat("MAYBE BRUTEFORCE! Error while user authenticate: [{0}]", userName);
            throw new FaultException("Access temporary blocked", new FaultCode(ServiceFaultCodes.Locked));
        }
    }
}