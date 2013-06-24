# Getting Started With Buddy

## Introduction

The Buddy Platform makes it easy to write a great mobile application without the difficulty and expense of writing a backend for it.  Buddy Platform is a Backend-as-a-Service (BaaS) provider that is used by over 15,000 developers and is growing every day.
 
Buddy solves many problems that application developers face, and one of them is creating and managing user accounts.  Many applications offer users a choice between direct login (username/password) and “social login” through a third party authentication provider such as Facebook or Live Connect.   Buddy makes it easy to offer your users either, or both, of these login methods.
 
In this quickstart we will demonstrate how to quickly implement both direct login and social login using the Buddy platform.
 
In the following steps, you will:

1. Quickly and easily create your first Buddy application
2. Integrate your new Buddy application with either Facebook authentication
3. Allow for creation of an App-Specific User Account
4. Retrieve information about the user you’ve just created

![App Screenshot](https://s3.amazonaws.com/Build2013WikiImages/image001.png)

## Creating Your Buddy Account

First, you’ll need to create an account on Buddy. 
 
That’s easy, all you do is:
 
#### Go to the Buddy Developer Portal at http://dev.buddy.com


#### Create an account
#### Create an application via the “Create New App” button on the upper right.

![create app button](https://s3.amazonaws.com/Build2013WikiImages/image002.png)

#### Now, find your Buddy Application Name and Buddy Application Key

![app keys button image](https://s3.amazonaws.com/Build2013WikiImages/image003.png)

![app keys image] (https://s3.amazonaws.com/Build2013WikiImages/image004.png)
  
Great!  Your new Buddy application is created and ready to be used.  While you’re here, notice that we created a sample application for you called “Sample Application – [your email]”.  This application is pre-populated with sample data so that you can take a tour through the Developer Portal and see the kinds of information it gives you about your application, but when you’re ready to create your application, use the new one that you created.

## Creating Your Windows Store Application

In Visual Studio, create a new blank Windows Store application. 
 
The first thing we’ll do is add the Buddy SDK and the Facebook SDKs to your project. 
 
We can easily do this via the NuGet package manager.
 
1. Right click on “References”
2. Choose “Manage NuGet References…”
3. In the “Search Online” box, type “Buddy” and hit enter
4. Choose “BuddyPlatformSdk” and hit “Install”.  
![Buddy SDK NuGet](https://s3.amazonaws.com/Build2013WikiImages/image005.png)  
5. Now add the NuGet package for Facebook:

![Facebook SDK NuGet](https://s3.amazonaws.com/Build2013WikiImages/image006.png) 
 
## Building the Login User Interface

![app screenshot](https://s3.amazonaws.com/Build2013WikiImages/image001.png)


In your ''MainPage.xaml'', add the following as a child of the main Grid element.  It’s quite a lot of XAML, but will get your UI all set at once:

    <StackPanel VerticalAlignment="Stretch" HorizontalAlignment="Center" Margin="219,100,183,0"> 
        <!-- Header -->
        <StackPanel Orientation="Horizontal">
            <Image Source="https://s3.amazonaws.com/BuddyAppIcons/buddy.png" Height="100"></Image>
            <TextBlock Text="Buddy Platform Quickstart" VerticalAlignment="Center" Margin="20,0,0,0" FontSize="72"/>
        </StackPanel>
            
        <!-- Login UI -->
        <Grid Name="LoginContainer" HorizontalAlignment="Center" VerticalAlignment="Bottom" Height="200" Width="800" Margin="0,25,0,0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="200"/>
                <ColumnDefinition Width="200"/>
                <ColumnDefinition Width="400"/>
            </Grid.ColumnDefinitions>
            <Grid.RowDefinitions>
                <RowDefinition Height="60"/>
                <RowDefinition Height="60"/>
                <RowDefinition Height="50"/>
            </Grid.RowDefinitions>
                
                

            <!-- Direct Login -->
            <TextBlock HorizontalAlignment="Left" VerticalAlignment="Center" FontSize="24" Text="Username" Grid.Column="0" Grid.Row="0"/>
            <TextBlock HorizontalAlignment="Left"  VerticalAlignment="Center"  FontSize="24"   Text="Password" Grid.Column="0" Grid.Row="1"/>
            <TextBox Name="Username" HorizontalAlignment="Stretch" VerticalAlignment="Center" Grid.Row="0" Grid.Column="1"/>
            <PasswordBox Name="Password" HorizontalAlignment="Stretch" VerticalAlignment="Center" Grid.Row="1" Grid.Column="1"/>
            <Button x:Name="LoginButton" Content="Login" Click="LoginButton_Click" HorizontalAlignment="Left" Grid.Column="1" Grid.Row="2" />
            <Button x:Name="SignupButton" Content="Sign Up" Click="SignupButton_Click" HorizontalAlignment="Right" Grid.Column="1" Grid.Row="2"/>

            <!-- Social (Facebook) Login -->
            <Border  HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Grid.Row="0" Grid.Column="2" Grid.RowSpan="3" Margin="25,0,0,0" BorderThickness="5,0,0,0" BorderBrush="{StaticResource ApplicationForegroundThemeBrush}">
                <Button x:Name="SocialLoginButton" Click="SocialLoginButton_Click" Content="Login with Facebook" VerticalAlignment="Center"  HorizontalAlignment="Stretch" Margin="25,0,0,0"/>
            </Border>
               
        </Grid>
            
        <!-- Profile Info UI -->
        <StackPanel Name="Profile" Visibility="Collapsed"  Orientation="Horizontal" HorizontalAlignment="Center" VerticalAlignment="Bottom">
            <Image x:Name="ProfileImage" Visibility="Collapsed" Height="40" Source="https://s3.amazonaws.com/BuddyAppIcons/buddy.png" VerticalAlignment="Center"></Image>
            <TextBlock Margin="5,0,5,0" FontSize="24"  VerticalAlignment="Center">Hello</TextBlock>
            <TextBlock FontSize="24" Name="ProfileName"  VerticalAlignment="Center">Shawn</TextBlock>
        </StackPanel>

    </StackPanel>


At this point, your designer should look like the screenshot above.
    
We’ve added UI for the title, the direct signup/login options, and the Facebook login button.

Now, switch to ''MainPage.xaml.cs'' and we’ll get our app connected to Buddy:


    using Buddy;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Media.Imaging;


    // ...

    public static string buddyAppName = "[Your Buddy Application Name]";//get this from dev.buddy.com
    public static string buddyAppPass = "[Your Buddy Application Key]"; // get this from dev.buddy.com

    // create the BuddyClient and log in the user.
    public static BuddyClient client = new BuddyClient(buddyAppName, buddyAppPass);


Here, we’ve taken the credentials that we copied from the Buddy Developer Portal and created our BuddyClient instance.  The BuddyClient instance allows us to access all the Buddy APIs that we use to build our app.

## Implementing Direct Login


Ok, let’s add our login handlers.  Paste the following code into your MainPage.xaml.cs file:

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
    }

    private async void SocialLoginButton_Click(object sender, RoutedEventArgs e)
    {

    }

What this code will do is:

1. Either create or login a user on Buddy, depending on whether Signup or Login is Clicked
2. Retrieve the users information and populate some UI with the user’s profile information.
3. Hide the login UI.

Go ahead and press Run to try your application, and click “Sign Up”.  Your user will be logged in and “Hello [username]” will display.

## Implementing Facebook Login

Now that we’ve added direct login, it’s very easy to also support Facebook Login.

First, at the top of your file, add:

    using Facebook;
    using Windows.Security.Authentication.Web;
    using Windows.Devices.Geolocation;


Then copy in the following code, which will setup the call to the Facebook SDK:

    public class SocialCreds
    {
        public string ProviderType { get; set; }
        public string ID { get; set; }
        public string AccessToken { get; set; }
    }
     
    public async Task<SocialCreds> FacebookLogin()
    {
        string _facebookAppId = "189779734480590";
        string _permissions = "user_about_me,user_birthday"; // Set your permissions here

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

Find the ``SocialLoginButton_Click`` function, and update the function as below:

    private async void SocialLoginButton_Click(object sender, RoutedEventArgs e)
    {
        var creds = await FacebookLogin();
        if (creds == null)
        {
            return;
        }
        AuthenticatedUser user = await client.SocialLoginAsync(
            creds.ProviderType, 
            creds.ID, 
            creds.AccessToken);
        FinishLogin(user);
    }

When this code is run, it will do the following:

1. Invoke the Facebook authentication UI
2. Take the user’s Facebook ID and Access Token and pass them to Buddy
3. Buddy will create (if necessary) and log in a user based on the Facebook profile

Note that in is scenario, Buddy does NOT cache the access token, it simply uses it to verify the user account on Facebook and collect profile information for populating a Buddy user.

Run the application again, and press “Login with Facebook” to try the social login integration.

## Adding More Functionality and Finishing the Application

Buddy has lots of great APIs, ranging from saving and sharing pictures, to sending messages between users, to push notifications, to geographic check ins.  For a quick example, we’ll check in our newly logged-in user using the Windows 8 geolocation APIs. 

Now that the user has been created and logged in, let’s also perform a checkin of the user’s current location.

Find the ``FinishLogin`` function, and paste in following code at the end of the function:

   
        // do a check in
        var gl = new Geolocator();
        var pos = await gl.GetGeopositionAsync();
        await user.CheckInAsync(
            pos.Coordinate.Latitude,
            pos.Coordinate.Longitude,
            "I'm at //BUILD!");

   
This code will invoke the device’s location functionality to get the current position, the pass that information to Buddy as a checkin.

In order to allow the user to check in, we will need to access the location of the device.  First, enable this by double clicking on ''package.appxmanifst'' in solution explorer then choosing Capabilities -> Location.

![windows store permissions image](https://s3.amazonaws.com/Build2013WikiImages/image008.png)

Go ahead and run the application to test it out!

## Viewing App Activity in the Buddy Developer Portal

Now that the application has run, you can go back to the Buddy Developer Portal to view the user that was just created, as well as the checkin location:

![devportal users image](https://s3.amazonaws.com/Build2013WikiImages/image009.png)

Note that you will be prompted by Windows for permission to access the devices location.
## License

#### Copyright (C) 2012 Buddy Platform, Inc.


Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at

  [http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
License for the specific language governing permissions and limitations under
the License.
