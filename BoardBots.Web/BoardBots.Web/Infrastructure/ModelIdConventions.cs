using Raven.Client;
using BoardBots.Web.Models;

namespace BoardBots.Web.Infrastructure
{
    public static class ModelIdConventions
    {
        public static void RegisterConventions(this IDocumentStore docStore)
        {
            docStore.Conventions.RegisterIdConvention<UserAccount>((dbname, commands, user) => "users/" + user.Username);
            docStore.Conventions.RegisterIdConvention<OAuthAssociation>((dbname, commands, association) => "oauthassociations/" + association.ProviderName + "/" + association.ProviderUserId);
        }

        public static UserAccount LoadUserAccount(this IDocumentSession s, string username)
        {
            return s.Load<UserAccount>("users/" + username);
        }

        public static OAuthAssociation LoadOAuthAssociation(this IDocumentSession s, string providerName, string providerUserId)
        {
            return s.Include<OAuthAssociation>(x => x.UserAccountId).Load("oauthassociations/" + providerName + "/" + providerUserId);
        }
    }
}