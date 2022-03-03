using System.Collections.Generic;
using System.Configuration;

namespace ais_erp_proxy.Models
{
    public class UserBL
    {
        public List<User> GetUsers()
        {
            List<User> userList = new List<User>();
            userList.Add(new User()
            {
                ID = 101,
                UserName = ConfigurationManager.AppSettings["ProxyUsername"],
                Password = ConfigurationManager.AppSettings["ProxyPassword"]
            });
            return userList;
        }
    }
}