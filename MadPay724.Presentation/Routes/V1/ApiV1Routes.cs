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
            //api/v1/site/panel/AdminUsers
            //Post
            public const string EditRoles = BaseSitePanel + "/adminusers/editroles/{userName}";
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

        #region NotificationRoutes
        public static class Notification
        {
            //api/v1/site/panel/{userId}/notifications
            //Put
            public const string UpdateUserNotify = BaseSitePanel + "/users/{userId}/notifications";
            //api/v1/site/panel/notifications/{userId}
            //Get
            public const string GetUserNotify = BaseSitePanel + "/notifications/{userId}";
        }
        #endregion

        #region BankCardRoutes
        public static class BankCard
        {
            //api/v1/site/panel/users/{userId}/bankcards
            //POST
            public const string AddBankCard = BaseSitePanel + "/users/{userId}/bankcards";
            //api/v1/site/panel/users/{userId}/bankcards
            //GET
            public const string GetBankCards = BaseSitePanel + "/users/{userId}/bankcards";
            //api/v1/site/panel/users/{userId}/bankcards/{id}
            //GET
            public const string GetBankCard = BaseSitePanel + "/users/{userId}/bankcards/{id}";
            //api/v1/site/panel/bankcards/{id}
            //PUT
            public const string UpdateBankCard = BaseSitePanel + "/users/{userId}/bankcards/{id}";
            //api/v1/site/panel/users/{userId}/bankcard/{id}
            //DELETE
            public const string DeleteBankCard = BaseSitePanel + "/users/{userId}/bankcards/{id}";

        }
        #endregion

        #region DocumentRoutes
        public static class Document
        {
            //api/v1/site/panel/users/{userId}/documents
            //POST
            public const string AddDocument = BaseSitePanel + "/users/{userId}/documents";
            //api/v1/site/panel/users/{userId}/documents
            //GET
            public const string GetDocuments = BaseSitePanel + "/users/{userId}/documents";
            //api/v1/site/panel/users/{userId}/documents/{id}
            //GET
            public const string GetDocument = BaseSitePanel + "/users/{userId}/documents/{id}";

        }
        #endregion

        #region WalletRoutes
        public static class Wallet
        {
            //api/v1/site/panel/users/{userId}/documents
            //POST
            public const string AddWallet = BaseSitePanel + "/users/{userId}/wallets";
            //api/v1/site/panel/users/{userId}/documents
            //GET
            public const string GetWallets = BaseSitePanel + "/users/{userId}/wallets";
            //api/v1/site/panel/users/{userId}/documents/{id}
            //GET
            public const string GetWallet = BaseSitePanel + "/users/{userId}/wallets/{id}";

        }
        #endregion

        #region TicketRoutes
        public static class Ticket
        {
            //api/v1/site/panel/users/{userId}/tickets
            //POST
            public const string AddTicket = BaseSitePanel + "/users/{userId}/tickets";
            //api/v1/site/panel/users/{userId}/tickets
            //GET
            public const string GetTickets = BaseSitePanel + "/users/{userId}/tickets/page/{page}";
            //api/v1/site/panel/users/{userId}/tickets/{id}
            //GET
            public const string GetTicket = BaseSitePanel + "/users/{userId}/tickets/{id}";
            //api/v1/site/panel/users/{userId}/tickets/{id}/ticketContents
            //Put
            public const string SetTicketClosed = BaseSitePanel + "/users/{userId}/tickets/{id}";
            //---------------------------------------------------------------------------------------------------------
            //api/v1/site/panel/users/{userId}/tickets/{id}/ticketContents
            //POST
            public const string AddTicketContent = BaseSitePanel + "/users/{userId}/tickets/{id}/ticketcontents";
            //api/v1/site/panel/users/{userId}/tickets/{ticketId}/ticketContents/{id}
            //GET
            public const string GetTicketContent = BaseSitePanel + "/users/{userId}/tickets/{ticketId}/ticketContents/{id}";
            //api/v1/site/panel/users/{userId}/tickets/{id}/ticketContents
            //GET
            public const string GetTicketContents = BaseSitePanel + "/users/{userId}/tickets/{id}/ticketcontents";


        }
        #endregion

        #region GateRoutes
        public static class Gate
        {
            //api/v1/site/panel/users/{userId}/gates
            //POST
            public const string AddGate = BaseSitePanel + "/users/{userId}/gates";
            //api/v1/site/panel/users/{userId}/gates
            //GET
            public const string GetGates = BaseSitePanel + "/users/{userId}/gates";
            //api/v1/site/panel/users/{userId}/gates/{id}
            //GET
            public const string GetGate = BaseSitePanel + "/users/{userId}/gates/{id}";
            //api/v1/site/panel/users/{userId}/gates/{id}
            //PUT
            public const string UpdateGate = BaseSitePanel + "/users/{userId}/gates/{id}";
            //api/v1/site/panel/users/{userId}/gates/{id}/active
            //PUT
            public const string ActiveDirectGate = BaseSitePanel + "/users/{userId}/gates/{id}/active";
            

        }
        #endregion

        #region EasyPayRoutes
        public static class EasyPay
        {
            //api/v1/site/panel/users/{userId}/easyPays
            //POST
            public const string AddEasyPay = BaseSitePanel + "/users/{userId}/easyPays";
            //api/v1/site/panel/users/{userId}/easyPays
            //GET
            public const string GetEasyPays = BaseSitePanel + "/users/{userId}/easyPays";
            //api/v1/site/panel/users/{userId}/easyPays/{id}
            //GET
            public const string GetEasyPay = BaseSitePanel + "/users/{userId}/easyPays/{id}";
            //api/v1/site/panel/users/{userId}/easyPays/{id}/gateswallets
            //GET
            public const string GetEasyPayGatesWallets = BaseSitePanel + "/users/{userId}/easyPays/{id}/gateswallets";
            //api/v1/site/panel/easyPays/{id}
            //PUT
            public const string UpdateEasyPay = BaseSitePanel + "/users/{userId}/easyPays/{id}";
            //api/v1/site/panel/users/{userId}/easyPays/{id}
            //DELETE
            public const string DeleteEasyPay = BaseSitePanel + "/users/{userId}/easyPays/{id}";

        }
        #endregion
    }
}
