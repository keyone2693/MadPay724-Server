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

        public const string Panel = "panel";

        public const string App = "app";

        public const string BaseSitePanel = Root + "/" + Version + "/" + Site + "/" + Panel;
        public const string BaseSiteApp =   Root + "/" + Version + "/" + Site + "/" + App;

        #region AdminRoutes

        public static class AdminUsers
        {
            //api/v1/site/panel/AdminUsers
            //GET
            public const string GetUsers = BaseSitePanel + "/adminusers";
        }

        #endregion

        #region UsersRoutes

        public static class Users
        {
            //api​/v1​/site​/panel​/Users​/{id}
            //GET
            public const string GetUser = BaseSitePanel + "/users/{id}";
            //​api​/v1​/site​/panel​/Users​/{id}
            //PUT
            public const string UpdateUser = BaseSitePanel + "/users/{id}";
            //​api​/v1​/site​/panel​/Users​/ChangeUserPassword​/{id}
            //PUT
            public const string ChangeUserPassword = BaseSitePanel + "/users/changeuserpassword/{id}";
        }

        #endregion

        #region PhotosRoutes
        public static class Photos
        {
            //api/v1/site/panel/users/{userId}/photos/{id}
            //GET
            public const string GetPhoto = BaseSitePanel + "/users/{userId}/photos/{id}";
            //api/v1/site/panel/users/{userId}/photos
            //GET
            public const string ChangeUserPhoto = BaseSitePanel + "/users/{userId}/photos";
        }
        #endregion

        #region AuthRoutes
        public static class Auth
        {
            //api/v1/site/panel/auth/register
            //GET
            public const string Register = BaseSitePanel + "/auth/register";
            //api/v1/site/panel/auth/login
            //GET
            public const string Login = BaseSitePanel + "/auth/login";
        }
        #endregion
    }
}
