using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Routes.V1
{
    public static class ApiV1Routes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Site = "site";

        public const string Admin = "admin";

        public const string App = "app";

        public const string BaseSiteAdmin = Root + "/" + Version + "/" + Site + "/" + Admin ;
        public const string BaseSiteApp =   Root + "/" + Version + "/" + Site + "/" + App;



        #region UsersRoutes

        public static class Users
        {
            //api/v1/site/admin/Users
            //GET
            public const string GetUsers = BaseSiteAdmin + "/users";
            //api​/v1​/site​/admin​/Users​/{id}
            //GET
            public const string GetUser = BaseSiteAdmin + "/users/{id}";
            //​api​/v1​/site​/admin​/Users​/{id}
            //PUT
            public const string UpdateUser = BaseSiteAdmin + "/users/{id}";
            //​api​/v1​/site​/admin​/Users​/ChangeUserPassword​/{id}
            //PUT
            public const string ChangeUserPassword = BaseSiteAdmin + "/users/changeuserpassword/{id}";
        }

        #endregion

        #region PhotosRoutes
        public static class Photos
        {
            //api/v1/site/admin/users/{userId}/photos/{id}
            //GET
            public const string GetPhoto = BaseSiteAdmin + "/users/{userId}/photos/{id}";
            //api/v1/site/admin/users/{userId}/photos
            //GET
            public const string ChangeUserPhoto = BaseSiteAdmin + "/users/{userId}/photos";
        }
        #endregion

        #region AuthRoutes
        public static class Auth
        {
            //api/v1/site/admin/auth/register
            //GET
            public const string Register = BaseSiteAdmin + "/auth/register";
            //api/v1/site/admin/auth/login
            //GET
            public const string Login = BaseSiteAdmin + "/auth/login";
        }
        #endregion
    }
}
