using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;

namespace MyFacebookApp.Controllers
{
public class AccountController : Controller
{
    //
    // GET: /Account/Login

    public ActionResult Login()
    {
        // Build the Return URI form the Request Url
        var redirectUri = new UriBuilder(Request.Url);
        redirectUri.Path = Url.Action("FbAuth", "Account");

        var client = new FacebookClient();

        // Generate the Facebook OAuth URL
        // Example: https://www.facebook.com/dialog/oauth?
        //                client_id=YOUR_APP_ID
        //               &redirect_uri=YOUR_REDIRECT_URI
        //               &scope=COMMA_SEPARATED_LIST_OF_PERMISSION_NAMES
        //               &state=SOME_ARBITRARY_BUT_UNIQUE_STRING
        var uri = client.GetLoginUrl(new
        {
            client_id = ConfigurationManager.AppSettings["FacebookAppId"],
            redirect_uri = redirectUri.Uri.ToString(),
            scope = "email",
        });

        return Redirect(uri.ToString());
    }

public ActionResult FbAuth()
{
    var client = new FacebookClient();
    var oauthResult = client.ParseOAuthCallbackUrl(Request.Url);

    // Build the Return URI form the Request Url
    var redirectUri = new UriBuilder(Request.Url);
    redirectUri.Path = Url.Action("FbAuth", "Account");

    dynamic result = client.Get("/oauth/access_token", new
    {
        client_id = ConfigurationManager.AppSettings["FacebookAppId"],
        redirect_uri = redirectUri.Uri.ToString(),
        client_secret = ConfigurationManager.AppSettings["FacebookAppSecret"],
        code = oauthResult.Code,
    });

    // Read the auth values
    string accessToken = result.access_token;
    DateTime expires = DateTime.UtcNow.AddSeconds(Convert.ToDouble(result.expires));

    dynamic me = client.Get("/me", new { fields = "first_name,last_name,email", access_token = accessToken });

    // Read the Facebook user values
    long facebookId = Convert.ToInt64(me.id);
    string firstName = me.first_name;
    string lastName = me.last_name;
    string email = me.email;

    // Add the user to our persistent store
    var userService = new UserService();
    userService.AddOrUpdateUser(new User
    {
        Id = facebookId,
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        AccessToken = accessToken,
        Expires = expires
    });

    // Set the Auth Cookie
    FormsAuthentication.SetAuthCookie(email, false);

    // Get the Auth Redirect URL
    var redirectUrl = FormsAuthentication.GetRedirectUrl(email, false);
        
    return Redirect(redirectUrl);
}

}
}
