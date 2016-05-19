using ExoMail.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ExoMail.Smtp.Authentication
{
    /// <summary>
    /// The UserManager store a list of UserStores that the application can use
    /// to look up and authenticate users.  It is designed as a singleton instance
    /// that should seperate the access to the underlying UserStores.
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// A lazy singleton initializer.
        /// </summary>
        private static readonly Lazy<UserManager> _instance =
            new Lazy<UserManager>(() => new UserManager());

        /// <summary>
        /// Gets this UserManager instance.
        /// </summary>
        public static UserManager GetUserManager { get { return _instance.Value; } }

        /// <summary>
        /// Helper property to flatten the list of IUserIdentities in the UserStores.
        /// </summary>
        private List<IUserIdentity> _identities
        {
            get
            {
                {
                    return this.UserStores
                        .SelectMany(x => x.GetIdentities())
                        .ToList();
                }
            }
        }

        /// <summary>
        /// A List of UserStores the UserManager has access to.
        /// </summary>
        private List<IUserStore> UserStores { get; set; }

        /// <summary>
        /// Private constructor for this singleton.
        /// </summary>
        private UserManager()
        {
            this.UserStores = new List<IUserStore>();
        }

        /// <summary>
        /// Adds a UserStore to the UserManager.
        /// </summary>
        /// <param name="userStore">I UserStore object containing a list of IUserIdentities.</param>
        /// <returns>This UserManager.</returns>
        public UserManager AddUserStore(IUserStore userStore)
        {
            this.UserStores.Add(userStore);
            return this;
        }

        /// <summary>
        /// Finds an IUserIdentity.
        /// </summary>
        /// <param name="emailAddress">The email address to find the identity with.</param>
        /// <returns>IUserIdentity</returns>
        public IUserIdentity FindByEmailAddress(string emailAddress)
        {
            return this._identities
                .FirstOrDefault(y => y.EmailAddress.ToUpper() == emailAddress.ToUpper() || y.AliasAddresses.Any(a => a.ToUpper() == emailAddress.ToUpper()));
        }

        /// <summary>
        /// Finds an IUserIdentity.
        /// </summary>
        /// <param name="userId">The UserId to find the identity with.</param>
        /// <returns>IUserIdentity</returns>
        public IUserIdentity FindById(string userId)
        {
            return this._identities
                .FirstOrDefault(y => y.UserId.ToUpper() == userId.ToUpper());
        }

        /// <summary>
        /// Finds and IUserIdentity
        /// </summary>
        /// <param name="userName">The UserName to find the identity with.</param>
        /// <returns>IUserIdentity</returns>
        public IUserIdentity FindByUserName(string userName)
        {
            return this._identities
              .FirstOrDefault(y => y.UserName.ToUpper() == userName.ToUpper());
        }

        /// <summary>
        /// Checks to see if the credentials are authenticated.
        /// </summary>
        /// <param name="userName">The UserName of the user to authenticate.</param>
        /// <param name="password">The Password of the user to authenticate</param>
        /// <returns>True if authenticated.</returns>
        public bool IsUserAuthenticated(string userName, string password)
        {
            return this.UserStores.Any(x => x.IsUserAuthenticated(userName, password));
        }

        /// <summary>
        /// Checks to see if the credentials are authenticated.
        /// </summary>
        /// <param name="networkCredential">A NetworkCredential to use for authentication.</param>
        /// <returns>True if authenticated.</returns>
        public bool IsUserAuthenticate(NetworkCredential networkCredential)
        {
            if (networkCredential.SecurePassword != null)
                throw new NotSupportedException("SecurePassword Property is not supported.");

            return this.UserStores.Any(x => x.IsUserAuthenticated(networkCredential.UserName, networkCredential.Password));
        }

        /// <summary>
        /// Checks to see if the specified email address exists in the UserStore.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public bool IsValidRecipient(string emailAddress)
        {
            return this.UserStores.Any(x => x.IsValidRecipient(emailAddress));
        }
    }
}