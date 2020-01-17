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
            //GET
            public const string GetUsers = BaseSitePanel + "/admin/users";
        }
        public static class AdminRoles
        {
            //GET
            public const string GetUserRoles = BaseSitePanel + "/admin/users/{userId}/roles";
            //PUT
            public const string ChangeRoles = BaseSitePanel + "/admin/users/{userId}/changerole";
        }
        public static class AdminGates
        {
            //GET
            public const string GetGates = BaseSitePanel + "/admin/users/{userId}/gates";
            //PATCH
            public const string ChangeActiveGate = BaseSitePanel + "/admin/gates/{gateId}/activegate";
            //PATCH
            public const string ChangeDirectGate = BaseSitePanel + "/admin/gates/{gateId}/directgate";
            //PATCH
            public const string ChangeIpGate = BaseSitePanel + "/admin/gates/{gateId}/ipgate";
        }
        public static class AdminDocument
        {
            //GET
            public const string GetDocuments = BaseSitePanel + "/admin/documents";
            //GET
            public const string GetUserDocuments = BaseSitePanel + "/admin/users/{userId}/documents";
            //GET
            public const string GetDocument = BaseSitePanel + "/admin/documents/{documentId}";
            //PUT
            public const string UpdateDocument = BaseSitePanel + "/admin/documents/{documentId}";
        }
        public static class AdminEasyPay
        {
            //GET
            public const string GetEasyPays = BaseSitePanel + "/admin/easyPays";
            //GET
            public const string GetUserEasyPays = BaseSitePanel + "/admin/users/{userId}/easyPays";
            //GET
            public const string GetEasyPay = BaseSitePanel + "/admin/easyPays/{easypayId}";
            //DELETE
            public const string DeleteEasyPay = BaseSitePanel + "/admin/easyPays/{easypayId}";
        }
        public static class AdminTicket
        {
            //POST
            public const string AddTicket = BaseSitePanel + "/admin/tickets/add";
            //GET
            public const string GetTickets = BaseSitePanel + "/admin/tickets";
            //GET
            public const string GetUserTickets = BaseSitePanel + "/admin/users/{userId}/tickets";
            //GET
            public const string GetTicket = BaseSitePanel + "/admin/tickets/{ticketId}";
            //PATCH
            public const string SetTicketClosed = BaseSitePanel + "/admin/tickets/{ticketId}";
            //---------------------------------------------------------------------------------------------------------
            //POST
            public const string AddTicketContent = BaseSitePanel + "/admin/tickets/{ticketId}/ticketcontents";
            //GET
            public const string GetTicketContent = BaseSitePanel + "/admin/tickets/{ticketId}/ticketContents/{ticketContentId}";
            //GET
            public const string GetTicketContents = BaseSitePanel + "/admin/tickets/{ticketId}/ticketcontents";
        }

        public static class AdminFactors
        {
            //GET
            public const string GetUserFactors = BaseSitePanel + "/admin/users/{userId}/factors";

        }

        #endregion
        #region AccountantRoutes

        public static class Accountant
        {
            //GET
            public const string GetInventories = BaseSitePanel + "/inventories";
            //GET
            public const string GetInventoryWallets = BaseSitePanel + "/inventories/wallets/{userId}";
            //GET
            public const string GetInventoryBankCard = BaseSitePanel + "/inventories/bankcards/{userId}";
            //PATCH
            public const string BlockInventoryWallet = BaseSitePanel + "/inventories/blockwallet/{walletId}";
            //PATCH
            public const string ApproveInventoryWallet = BaseSitePanel + "/inventories/approvebankcard/{bankcardId}";
            //PATCH
            public const string ChangeActiveGate = BaseSitePanel + "/financial/gates/{gateId}/activegate";
            //PATCH
            public const string ChangeDirectGate = BaseSitePanel + "/financial/gates/{gateId}/directgate";
            //PATCH
            public const string ChangeIpGate = BaseSitePanel + "/financial/gates/{gateId}/ipgate";
            //GET
            public const string GetWallets = BaseSitePanel + "/inventories/allwallets";
            //GET
            public const string GetGates = BaseSitePanel + "/financial/allgates";

            //GET
            public const string GetWalletGates = BaseSitePanel + "/financial/wallets/{walletId}/gates";
            //GET
            public const string GetBankCards = BaseSitePanel + "/inventories/allbankcards";
        }

        #endregion

# region EntryRoutes
        public static class Entry
        {
            //GET
            public const string GetApproveEntries = BaseSitePanel + "/entries/approve";
            //GET
            public const string GetPardakhtEntries = BaseSitePanel + "/entries/pardakht";
            //GET
            public const string GetDoneEntries = BaseSitePanel + "/entries/archive";
            //GET
            public const string GetEntry = BaseSitePanel + "/entries/{entryId}";
            //GET
            public const string GetBankCardEntries = BaseSitePanel + "/bankcards/{bankcardId}/entries";
            //GET
            public const string GetWalletEntries = BaseSitePanel + "/wallets/{walletId}/entries";
            //PUT
            public const string UpdateEntry = BaseSitePanel + "/entries/{entryId}/update";
            //PATCH
            public const string ApproveEntry = BaseSitePanel + "/entries/{entryId}/approve";
            //PATCH
            public const string PardakhtEntry = BaseSitePanel + "/entries/{entryId}/pardakht";
            //PATCH
            public const string RejectEntry = BaseSitePanel + "/entries/{entryId}/reject";
            //DELETE
            public const string DeleteEntry = BaseSitePanel + "/entries/{entryId}/delete";

        }

        #endregion

        #region FactorRoutes
        public static class Factors
        {
            //GET
            public const string GetFactors = BaseSitePanel + "/factors";
            
            //GET
            public const string GetWalletFactors = BaseSitePanel + "/wallets/{walletId}/factors";
            //GET
            public const string GetGateFactors = BaseSitePanel + "/gates/{gateId}/factors";
            //GET
            public const string GetFactor = BaseSitePanel + "/factors/{factorId}";
            //PATCH
            public const string StatusFactor = BaseSitePanel + "/factors/{factorId}/status";
            //PATCH
            public const string EditFactor = BaseSitePanel + "/factors/{factorId}/edit";
            //DELETE
            public const string DeleteFactor = BaseSitePanel + "/factors/{factorId}/delete";

        }
        #endregion
        #region UsersRoutes

        public static class Users
        {
            //api​/v1​/site​/panel​/Users​
            //GET
            public const string GetUsers = BaseSitePanel + "/users";
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

        #region BlogGroupRoutes
        public static class BlogGroup
        {
            //api/v1/site/panel/users/{userId}/blogGroups
            //POST
            public const string AddBlogGroup = BaseSitePanel + "/users/{userId}/blogGroups";
            //api/v1/site/panel/users/{userId}/blogGroups
            //GET
            public const string GetBlogGroups = BaseSitePanel + "/users/{userId}/blogGroups";
            //api/v1/site/panel/users/{userId}/blogGroups/{id}
            //GET
            public const string GetBlogGroup = BaseSitePanel + "/users/{userId}/blogGroups/{id}";
            //api/v1/site/panel/blogGroups/{id}
            //PUT
            public const string UpdateBlogGroup = BaseSitePanel + "/users/{userId}/blogGroups/{id}";
            //api/v1/site/panel/users/{userId}/blogGroups/{id}
            //DELETE
            public const string DeleteBlogGroup = BaseSitePanel + "/users/{userId}/blogGroups/{id}";

        }
        #endregion
        #region FactorRoutes
        public static class UsersFactors
        {

            //GET
            public const string GetGateFactors = BaseSitePanel + "/users/{userId}/gates/{gateId}/factors";
            //GET
            public const string GetFactor = BaseSitePanel + "/users/{userId}/factors/{factorId}";

        }
        #endregion

        #region BlogRoutes
        public static class Blog
        {
            //api/v1/site/panel/users/{userId}/blogs/upload
            //POST
            public const string UploadBlogImage = BaseSitePanel + "/users/{userId}/blogs/upload";
            //api/v1/site/panel/users/{userId}/blogs/deleteupload
            //DELETE
            public const string DeleteBlogImage = BaseSitePanel + "/users/{userId}/blogs/deleteupload";
            //api/v1/site/panel/users/{userId}/blogs
            //POST
            public const string AddBlog = BaseSitePanel + "/users/{userId}/blogs";
            //api/v1/site/panel/users/{userId}/blogs
            //GET
            public const string GetBlogs = BaseSitePanel + "/users/{userId}/blogs";
            //api/v1/site/panel/users/{userId}/blogs/{id}
            //GET
            public const string GetBlog = BaseSitePanel + "/users/{userId}/blogs/{id}";
            //api/v1/site/panel/blogs/{id}
            //PUT
            public const string UpdateBlog = BaseSitePanel + "/users/{userId}/blogs/{id}";
            //api/v1/site/panel/users/{userId}/blogs/{id}
            //DELETE
            public const string DeleteBlog = BaseSitePanel + "/users/{userId}/blogs/{id}";
            //api/v1/site/panel/users/{userId}/blogs/{id}
            //PUT
            public const string SelectBlog = BaseSitePanel + "/users/{userId}/blogs/{id}/selectBlog";
            //api/v1/site/panel/users/{userId}/blogs/{id}
            //PUT
            public const string ApproveBlog = BaseSitePanel + "/users/{userId}/blogs/{id}/approveBlog";
            //api/v1/site/panel/blogs/{id}
            //Get
            public const string GetUnverifiedBlogCount = BaseSitePanel + "/users/{userId}/blogs/unverifiedcount";


        }
        #endregion

        #region CommonRoutes
        public static class Common
        {
            //Get
            public const string GetNotifications = BaseSitePanel + "/users/{id}/common/getnotifications";

        }
        #endregion
        #region DashboardRoutes
        public static class Dashboard
        {
            //Get
            public const string GetAdminDashboard = BaseSitePanel + "/admin/common/dashboard";
            //Get
            public const string GetAccountantDashboard = BaseSitePanel + "/accountant/common/dashboard";
            //Get
            public const string GetBlogDashboard = BaseSitePanel + "/blog/{userId}/common/dashboard";
            //Get
            public const string GetUserDashboard = BaseSitePanel + "/user/{userId}/common/dashboard";

        }
        #endregion
    }
}
