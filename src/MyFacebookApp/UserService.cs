using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFacebookApp
{
    public class UserService
    {
        private static ConcurrentDictionary<long, User> users = new ConcurrentDictionary<long, User>();

        public void AddOrUpdateUser(User user)
        {
            // This is for DEMO purposes only, in a real application
            // you should use persistent storage such as SQL 
            // or Windows Azure Table Storage.
            users.AddOrUpdate(user.Id, user, (key, existingVal) =>
            {
                existingVal.AccessToken = user.AccessToken;
                existingVal.Expires = user.Expires;
                existingVal.FirstName = user.FirstName;
                existingVal.LastName = user.LastName;
                existingVal.Email = user.Email;
                return existingVal;
            });
        }
    }

    public class User
    {

        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }

        public DateTime Expires { get; set; }
    }
}