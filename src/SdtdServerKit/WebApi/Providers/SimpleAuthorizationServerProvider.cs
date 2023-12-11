using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;

namespace SdtdServerKit.WebApi.Providers
{
    internal class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.CompletedTask;
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (CheckCredential(context.UserName, context.Password) == false)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return Task.CompletedTask;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);

            return Task.CompletedTask;
        }

        private static bool CheckCredential(string userName, string password)
        {
            var appSettings = ModApi.AppSettings;
            return userName == appSettings.UserName && password == appSettings.Password;
        }
    }
}