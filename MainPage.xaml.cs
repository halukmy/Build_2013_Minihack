using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Buddy;
using System.Threading.Tasks;

using Facebook;
using Windows.Security.Authentication.Web;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Media.Imaging;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BuildAppWalkthrough
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public static string buddyAppName = "\[Your Buddy Application Name]";//get this from dev.buddy.com
        public static string buddyAppPass = "\[Your Buddy Application Key]"; // get this from dev.buddy.com

        // create the BuddyClient and log in the user.
        public static BuddyClient client = new BuddyClient(buddyAppName, buddyAppPass);

        public class SocialCreds
        {
            public string ProviderType { get; set; }
            public string ID { get; set; }
            public string AccessToken { get; set; }
        }

        public async Task<SocialCreds> FacebookLogin()
        {
            string _facebookAppId = "189779734480590";
            string _permissions = "user_about_me, user_birthday, email"; // Set your permissions here

            FacebookClient _fb = new FacebookClient();


            var redirectUrl = "https://www.facebook.com/connect/login_success.html";
            try
            {
                var loginUrl = _fb.GetLoginUrl(new
                {
                    client_id = _facebookAppId,
                    redirect_uri = redirectUrl,
                    scope = _permissions,
                    display = "popup",
                    response_type = "token"
                });

                var endUri = new Uri(redirectUrl);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        loginUrl,
                                                        endUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var callbackUri = new Uri(WebAuthenticationResult.ResponseData.ToString());
                    var facebookOAuthResult = _fb.ParseOAuthCallbackUrl(callbackUri);
                    var accessToken = facebookOAuthResult.AccessToken;
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        // User is not logged in, they may have canceled the login
                        return null;
                    }
                    else
                    {
                        // User is logged in and token was returned
                        dynamic parameters = new System.Dynamic.ExpandoObject();
                        parameters.access_token = accessToken;
                        parameters.fields = "id";

                        dynamic result = await _fb.GetTaskAsync("me", parameters);
                        parameters.id = result.id;

                        return new SocialCreds
                        {
                            ProviderType = "Facebook",
                            AccessToken = accessToken,
                            ID = result.id
                        };
                    }

                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    throw new InvalidOperationException("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    // The user canceled the authentication
                    return null;
                }
            }
            catch (Exception ex)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                throw ex;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

      
        private async void FinishLogin(AuthenticatedUser user)
        {
            
            // hide login UI
            LoginContainer.Visibility = Visibility.Collapsed;
            
            // show and populate the info box
            Profile.Visibility = Visibility.Visible;
            ProfileName.Text = user.Name;
            
            // show profile photo if available.
            if (user.ProfilePicture != null)
            {
                ProfileImage.Visibility = Visibility.Visible;
                ProfileImage.Source = new BitmapImage(user.ProfilePicture);
            }

            // do a check in
            var gl = new Geolocator();
            var pos = await gl.GetGeopositionAsync();
            await user.CheckInAsync(
                pos.Coordinate.Latitude,
                pos.Coordinate.Longitude,
                "I'm at //BUILD!");
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var user = await client.LoginAsync(Username.Text, Password.Password);
            FinishLogin(user);
        }

        private async void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            var name = Username.Text;
            var pass = Password.Password;

            var user = await client.CreateUserAsync(name, pass);
            FinishLogin(user);
        }

        private async void SocialLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var creds = await FacebookLogin();
            if (creds == null)
            {
                return;
            }
            AuthenticatedUser user = await client.SocialLoginAsync(creds.ProviderType, creds.ID, creds.AccessToken);
            FinishLogin(user);
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
