using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Newtonsoft.Json;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

using System.Drawing;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace Munchy_UI
{
    // Enum used for determining which language is currently being used.
    public enum Languages
    {
        English,
        Bulgarian
    }

    // Enum used for determining which recipe view mode is being used.
    public enum PreviewModes
    {
        Saved,
        Created,
        Cooked,
        Normal
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Sign in variables and API call variables        
        const string m_clientID = "240851946326-ig45flska49isspupmabn0cu9imi1eb8.apps.googleusercontent.com";
        const string m_clientSecret = "3gTmtrk1CBB-f5RVg5c4di9X";
        const string m_authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string m_tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string m_userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        const string m_headerParam = "Ocp-Apim-Subscription-Key";
        const string m_headerValue = "d47c2f203a574ffbb552395447f1c253";

        #region User variables
        private string m_userID = "noID";
        private string m_userName = "";
        #endregion

        #region Recipe variables    
        private List<string> m_SuggestedRecipeNames;
        private List<string> m_AuthorNames;
        private List<string> m_RecipeDescs;
        private List<int> m_RecipeRatings;
        private List<string> m_ImageNames;
        private List<string> m_RecipeUserUUID;
        private List<string> m_CurrentRecipeIngrs;
        private List<float> m_CurrentRecipeAmounts;
        private List<string> m_CurrentRecipeUnits;
        private List<string> m_CurrentRecipeSteps;
        private int m_CurrentIndex = 0;
        private string m_RecipeName;
        private string m_RecipeAuthor;
        private string m_RecipeDesc;
        private string m_recipeID = "";

        string m_SerIngrList;
        string m_SerIngrAmountList;
        string m_SerIngrUnits;
        string m_SerRecipeTags;
        string m_SerRecipeSteps;
        string m_RecipeImage;
        #endregion

        #region Fridge variables
        private List<string> m_foodDataNamesUS;
        private List<string> m_foodDataNamesBG;
        private Dictionary<string, string> m_FoodNames;
        private Dictionary<string, float> m_CurrentFridge;
        private Dictionary<string, float> m_SuggestedFoodAmounts;
        private Dictionary<string, string> m_FoodUnits;
        private List<string> m_CurrentFridgeItems;
        #endregion

        #region Shopping list variables     
        private List<string> m_CurrentShoppingList;
        #endregion

        #region Recipe management variables 
        private Dictionary<string, string> m_SavedRecipes;
        private Dictionary<string, string> m_CreatedRecipes;
        private Dictionary<string, string> m_CookedRecipes;
        #endregion

        #region Recipe preview 
        private string m_RecipePreviewName;
        private string m_RecipePreviewAuthor;
        private string m_RecipePreviewDesc;
        private string m_RecipePreviewImg;
        PreviewModes m_CurrentPreviewMode;
        #endregion

        #region Recipe creation variables
        private string m_NewRecipeName;
        private string m_NewRecipeDesc;
        private string m_NewRecipeTimeToCook;
        private List<string> m_NewRecipeTimeTags;
        private List<string> m_NewRecipeTags;
        private List<string> m_NewRecipeIngrs;
        private List<float> m_NewRecipeIngrAmounts;
        private List<string> m_NewRecipeUnits;
        private List<string> m_NewRecipeSteps;
        private List<string> m_CreatedRecipeDisplayTags;
        #endregion

        #region User variables
        private List<string> m_UserPrefTags;
        private Dictionary<string, string> TagTranslationsBG = new Dictionary<string, string>()
        {
            {"isvegan", "Веган съм"},
            {"isvegetarian", "Вегетарианец съм"},
            {"isdiabetic", "Диабетик съм"},
            {"eggs", "Алергичен съм към яйца"},
            {"dairy", "Алергичен съм към мляко"},
            {"fish", "Алергичен съм към риба"},
            {"nuts", "Алергичен съм към ядки"},
            {"gluten", "Алергичен съм към глутен"}
        };

        private Dictionary<string, string> TagTranslationsUS = new Dictionary<string, string>()
        {
            {"isvegan", "I'm vegan"},
            {"isvegetarian", "I'm vegetarian"},
            {"isdiabetic", "I'm diabetic"},
            {"eggs", "I'm allergic to eggs"},
            {"dairy", "I'm allergic to dairy"},
            {"fish", "I'm allergic to fish"},
            {"nuts", "I'm allergic to nuts"},
            {"gluten", "I'm allergic to gluten"}
        };

        private Dictionary<string, string> RecipeTagTranslationsBG = new Dictionary<string, string>()
        {
             {"isvegan", "За вегани"},
             {"isvegetarian", "За вегетарианци"},
             {"isdiabetic", "За диабетици"},
             {"eggs", "За хора алергични към яйца"},
             {"dairy", "За хора алергични към мляко"},
             {"fish", "За хора алергични към риба"},
             {"nuts", "За хора алергични към ядки"},
             {"gluten", "За хора алергични към глутен"}
        };

        private Dictionary<string, string> RecipeTagTranslationsUS = new Dictionary<string, string>()
        {
             {"isvegan", "For vegans"},
             {"isvegetarian", "For vegetarians"},
             {"isdiabetic", "For diabetics"},
             {"eggs", "For people allergic to eggs"},
             {"dairy", "For people allergic to dairy"},
             {"fish", "For people allergic to fish"},
             {"nuts", "For people allergic to nuts"},
             {"gluten", "For people allergic to gluten"}
        };


        #endregion

        #region Image handling variables
        private string m_imageName;
        private string m_OpenedImage = "noimage";
        private bool recipeLoaded = false;
        #endregion

        #region Localization variables
        private string m_CurrentLocale = "us";
        Languages m_ActiveLanguage = Languages.English;
        private List<string> m_BGUOM = new List<string> { "чаша(и)", "супена лъжица", "чаена лъжица", "грамове", "мл", "бр" };
        private List<string> m_USUOM = new List<string> { "cup(s)", "tbsp", "tsp", "grams", "ml", "count" };
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            m_CurrentPreviewMode = PreviewModes.Normal;
            m_SuggestedRecipeNames = new List<string>();
            m_foodDataNamesUS = new List<string>();
            m_foodDataNamesBG = new List<string>();
            m_CurrentFridgeItems = new List<string>();
            m_NewRecipeIngrs = new List<string>();
            m_NewRecipeIngrAmounts = new List<float>();
            m_NewRecipeUnits = new List<string>();
            m_NewRecipeTags = new List<string>();
            m_NewRecipeTimeTags = new List<string>();
            m_CreatedRecipeDisplayTags = new List<string>();
            m_NewRecipeSteps = new List<string>();

            m_UserPrefTags = new List<string>();

            m_CurrentShoppingList = new List<string>();
            m_CurrentFridge = new Dictionary<string, float>();

            m_ActiveLanguage = Languages.English;
        }

        #region Localization Logic
        //Handles localization of the UI as well as setting the app "locale". 
        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            string LocaleCode = (string)((Button)sender).Tag;
            Localizer.SwitchLanguage(this, LocaleCode);

            if (LocaleCode == "bg-BG")
            {
                m_ActiveLanguage = Languages.Bulgarian;
                m_CurrentLocale = "bg";
            }

            if (LocaleCode == "en-US")
            {
                m_ActiveLanguage = Languages.English;
                m_CurrentLocale = "us";
            }

            m_CurrentIndex = 0;
            InitFridgeSearch();
            InitShoppingListSearch();
            SetupShoppingList();
            SetupFridge();
            GetSuggestedRecipes();
            UpdatePrefTagList();
        }
        #endregion

        #region Sign in logic
        // The following is the standard code that google recommends for creating a sign in request.
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private async void Begin_Sign_In(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generates state and PKCE values.
                string state = randomDataBase64url(32);
                string code_verifier = randomDataBase64url(32);
                string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
                const string code_challenge_method = "S256";

                // Creates a redirect URI using an available port on the loopback address.
                string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
                //output("redirect URI: " + redirectURI);

                // Creates an HttpListener to listen for requests on that redirect URI.
                var http = new HttpListener();
                http.Prefixes.Add(redirectURI);
                //output("Listening..");
                http.Start();

                // Creates the OAuth 2.0 authorization request.
                string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                    m_authorizationEndpoint,
                    System.Uri.EscapeDataString(redirectURI),
                    m_clientID,
                    state,
                    code_challenge,
                    code_challenge_method);

                // Opens request in the browser.
                System.Diagnostics.Process.Start(authorizationRequest);

                // Waits for the OAuth authorization response.
                var context = await http.GetContextAsync();

                // Brings this app back to the foreground.
                this.Activate();

                // Sends an HTTP response to the browser.
                var response = context.Response;
                string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Sign in complete!</body></html>");
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var responseOutput = response.OutputStream;
                Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
                {
                    responseOutput.Close();
                    http.Stop();
                    Console.WriteLine("HTTP server stopped.");
                });

                // Checks for errors.
                if (context.Request.QueryString.Get("error") != null)
                {
                    //  output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                    return;
                }
                if (context.Request.QueryString.Get("code") == null
                    || context.Request.QueryString.Get("state") == null)
                {
                    //  output("Malformed authorization response. " + context.Request.QueryString);
                    return;
                }

                // extracts the code
                var code = context.Request.QueryString.Get("code");
                var incoming_state = context.Request.QueryString.Get("state");

                // Compares the receieved state to the expected value, to ensure that
                // this app made the request which resulted in authorization.
                if (incoming_state != state)
                {
                    // output(String.Format("Received request with invalid state ({0})", incoming_state));
                    return;
                }
                // output("Authorization code: " + code

                // Starts the code exchange at the Token Endpoint.
                performCodeExchange(code, code_verifier, redirectURI);
            }
            catch (Exception ex)
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please launch app as an admin.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля стартирайте приложението като админ.";
                        break;
                }
            }

        }

        async void performCodeExchange(string code, string code_verifier, string redirectURI)
        {
            // "Exchanging code for tokens..."

            // builds the  request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                m_clientID,
                code_verifier,
                m_clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    userinfoCall(access_token);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                        }
                    }
                }

                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "An error occured while logging in, try again later.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Възникна грешка при регистрацията, моля опитайте по-късно.";
                        break;
                }
            }
        }


        async void userinfoCall(string access_token)
        {
            //"Making API Call to Userinfo..."

            // builds the  request
            string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

            // sends the request
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // gets the response
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                // reads response body
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                var user = JsonConvert.DeserializeObject<Object>(userinfoResponseText);
                dynamic dynamic = user;

                m_userID = dynamic.sub.ToString();
                m_userName = dynamic.name.ToString();

                if (m_userID != "noID")
                {
                    Storyboard sb = this.FindResource("CloseLogin") as Storyboard;
                    if (sb != null) { BeginStoryboard(sb); }
                    InitHomePage();
                }
                else
                {
                    // Setup error handling.
                }
                // =========================== USER INFORMATION OUTPUT ===============================
                // =========================== USER INFORMATION OUTPUT ===============================
                // =========================== USER INFORMATION OUTPUT ===============================
                // =========================== USER INFORMATION OUTPUT ===============================
            }
        }



        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
        #endregion


        #region API requests

        //Calls API that checks if user exsits. If he doesn't a new user is created. If he does nothing happens.
        private async void CreateUser()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/CreateUser";
            uriBuilder.Query = "UserID=" + m_userID + "&UserName=" + m_userName;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);
        }

        //Gets the information of the logged in user. In this case only the user preferences.
        private async void GetUser()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetUser";
            uriBuilder.Query = "UserID=" + m_userID;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_user = user;
            if (d_user.userPrefList != null)
            {
                m_UserPrefTags = JsonConvert.DeserializeObject<List<string>>(d_user.userPrefList.ToString());
                SetupPrefList();
            }
        }

        //Saves the users preferences based on what has has selected from the menu.
        private async void SaveUserPrefs()
        {
            string serPrefList = JsonConvert.SerializeObject(m_UserPrefTags);
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/SavePrefList";
            uriBuilder.Query = "userID=" + m_userID + "&serPrefList=" + serPrefList;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
        }

        //Gets the data for the food items used in the fridge, shopping list and recipe ingredients.
        private async void GetFoodData()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetBGFoodList";
            //uriBuilder.Query = "UserID=" + userID;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var foodDataContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_foodData = foodDataContainer;
            if (d_foodData.foodList != null)
            {
                m_FoodNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(d_foodData.foodList.ToString());
                InitFoodData();
            }
        }

        //Gets a list of suggested recipes based on the users compatibility index.
        private async void GetSuggestedRecipes()
        {
            recipeLoaded = false;
            m_CurrentPreviewMode = PreviewModes.Normal;
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetSuggestedRecipes";
            uriBuilder.Query = "userID=" + m_userID + "&locale=" + m_CurrentLocale;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var recipeContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_RecipeData = recipeContainer;
            if (d_RecipeData.ids != null)
            {
                m_RecipeUserUUID = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.ids.ToString());
            }

            if (d_RecipeData.recipes != null)
            {
                m_SuggestedRecipeNames = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.recipes.ToString());
            }

            if (d_RecipeData.authors != null)
            {
                m_AuthorNames = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.authors.ToString());
            }

            if (d_RecipeData.descs != null)
            {
                m_RecipeDescs = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.descs.ToString());
            }

            if (d_RecipeData.rating != null)
            {
                m_RecipeRatings = JsonConvert.DeserializeObject<List<int>>(d_RecipeData.rating.ToString());
            }

            if (d_RecipeData.image != null)
            {
                m_ImageNames = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.image.ToString());
            }

            if (m_SuggestedRecipeNames.Count > 0)
            {
                GetFullRecipeInfo(m_SuggestedRecipeNames[m_CurrentIndex], m_RecipeUserUUID[m_CurrentIndex], m_ImageNames[m_CurrentIndex]);
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Munchy couldn't find any suitable recipes. Try later.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Munchy не успя да намери рецепти, опитайте по-късно";
                        break;
                }
            }
        }

        //Gets the full infomation about a recipes based on the recipe name and the user UUDI associated with it.
        private async void GetFullRecipeInfo(string recipeName, string userID, string currentImage)
        {
            if (m_SuggestedRecipeNames.Count > 0 || m_CurrentPreviewMode != PreviewModes.Normal)
            {
                GetImage(currentImage);

                var client = new HttpClient();

                // Request headers
                client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "https";
                uriBuilder.Host = "munchyapitestmanager.azure-api.net";
                uriBuilder.Path = "Api/GetFullRecipeInfo";
                uriBuilder.Query = "RecipeName=" + recipeName + "&recipeID=" + userID + "&locale=" + m_CurrentLocale;
                var uri = uriBuilder.ToString();

                var response = await client.GetAsync(uri);

                string responseBody = await response.Content.ReadAsStringAsync();
                var foodDataContainer = JsonConvert.DeserializeObject<Object>(responseBody);
                var recipeContainer = JsonConvert.DeserializeObject<Object>(responseBody);
                dynamic d_RecipeData = recipeContainer;
                if (d_RecipeData.recipeName != null)
                {
                    m_RecipeName = d_RecipeData.recipeName.ToString();
                }

                if (d_RecipeData.recipeAuthor != null)
                {
                    m_RecipeAuthor = d_RecipeData.recipeAuthor.ToString();
                }

                if (d_RecipeData.recipeDesc != null)
                {
                    m_RecipeDesc = d_RecipeData.recipeDesc.ToString();
                }

                if (d_RecipeData.ingrs != null)
                {
                    m_CurrentRecipeIngrs = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.ingrs.ToString());
                }

                if (d_RecipeData.amounts != null)
                {
                    m_CurrentRecipeAmounts = JsonConvert.DeserializeObject<List<float>>(d_RecipeData.amounts.ToString());
                }

                if (d_RecipeData.units != null)
                {
                    m_CurrentRecipeUnits = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.units.ToString());
                }

                if (d_RecipeData.steps != null)
                {
                    m_CurrentRecipeSteps = JsonConvert.DeserializeObject<List<string>>(d_RecipeData.steps.ToString());
                    SetupRecipe();
                }
            }
        }

        //Gets the fridge of the logged in user and populated the listbox with the information.
        private async void GetFridge()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetFridge";
            uriBuilder.Query = "userID=" + m_userID;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var foodDataContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            var fridgeContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_fridgeData = fridgeContainer;
            if (d_fridgeData.fridgeList != null)
            {
                m_CurrentFridge = JsonConvert.DeserializeObject<Dictionary<string, float>>(d_fridgeData.fridgeList.ToString());
            }

            if (d_fridgeData.foodAmount != null)
            {
                m_SuggestedFoodAmounts = JsonConvert.DeserializeObject<Dictionary<string, float>>(d_fridgeData.foodAmount.ToString());
            }

            if (d_fridgeData.foodUnit != null)
            {
                m_FoodUnits = JsonConvert.DeserializeObject<Dictionary<string, string>>(d_fridgeData.foodUnit.ToString());
                SetupFridge();
            }
        }

        //Saves the fridge when something is added or removed from the fridge by the user.
        private async void SaveFridge()
        {
            string serFridge = JsonConvert.SerializeObject(m_CurrentFridge);


            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/SaveFridge";
            uriBuilder.Query = "userID=" + m_userID + "&serFridge=" + serFridge;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            GetFridge();
        }

        //Gets the shopping list of the logged in user.
        private async void GetShoppingList()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetShoppingList";
            uriBuilder.Query = "id=" + m_userID;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var shoppingListContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            var shoppingList = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_shoppingList = shoppingList;
            if (d_shoppingList.shoppingList != null)
            {
                m_CurrentShoppingList = JsonConvert.DeserializeObject<List<string>>(d_shoppingList.shoppingList.ToString());
                SetupShoppingList();
            }
        }

        //Saves the shopping list when something is added or removed by the user.
        private async void SaveShoppingList()
        {
            string serShoppingList = JsonConvert.SerializeObject(m_CurrentShoppingList);


            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/SaveShoppingList";
            uriBuilder.Query = "userID=" + m_userID + "&userShoppingList=" + serShoppingList;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            GetShoppingList();
        }

        //Gets the list of saved recipes that the user has.
        private async void GetSavedRecipes()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetSavedRecipes";
            uriBuilder.Query = "userID=" + m_userID + "&locale=" + m_CurrentLocale;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var recipes = JsonConvert.DeserializeObject<Object>(responseBody);
            var recipeContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_recipes = recipeContainer;
            if (d_recipes.recipeList != null)
            {
                m_SavedRecipes = JsonConvert.DeserializeObject<Dictionary<string, string>>(d_recipes.recipeList.ToString());
                SetupSavedRecipes();
            }
        }

        //Gets the list of created recipes that the user has.
        private async void GetCreatedRecipes()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetCreatedRecipes";
            uriBuilder.Query = "userID=" + m_userID + "&locale=" + m_CurrentLocale;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var recipes = JsonConvert.DeserializeObject<Object>(responseBody);
            var recipeContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_createdRecipes = recipeContainer;
            if (d_createdRecipes.recipeList != null)
            {
                m_CreatedRecipes = JsonConvert.DeserializeObject<Dictionary<string, string>>(d_createdRecipes.recipeList.ToString());
                SetupCreatedRecipes();
            }
        }

        //Gets the list of cooked recipes that the user has.
        private async void GetCookedRecipes()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetCookedRecipes";
            uriBuilder.Query = "userID=" + m_userID + "&locale=" + m_CurrentLocale;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var recipes = JsonConvert.DeserializeObject<Object>(responseBody);
            var recipeContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_recipes = recipeContainer;
            if (d_recipes.recipeList != null)
            {
                m_CookedRecipes = JsonConvert.DeserializeObject<Dictionary<string, string>>(d_recipes.recipeList.ToString());
                SetupCookedRecipes();
            }
        }

        //Saved the recipes that is currently displayed on the recipe view grid to the list of saved recipes of the user.      
        private async void SaveToSavedRecipes()
        {
            if (m_SuggestedRecipeNames.Count > 0)
            {
                var client = new HttpClient();

                // Request headers
                client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "https";
                uriBuilder.Host = "munchyapitestmanager.azure-api.net";
                uriBuilder.Path = "Api/SaveToSavedRecipes";
                uriBuilder.Query = "userID=" + m_userID + "&recipeName=" + m_SuggestedRecipeNames[m_CurrentIndex] + "&recipeUUID=" + m_RecipeUserUUID[m_CurrentIndex] + "&locale=" + m_CurrentLocale;
                var uri = uriBuilder.ToString();

                var response = await client.GetAsync(uri);

                string responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        //Saved the recipes that is currently displayed on the recipe view grid to the list of cooked recipes of the user.      
        private async void SaveToCookedRecipes()
        {
            if (m_SuggestedRecipeNames.Count > 0)
            {
                var client = new HttpClient();

                // Request headers
                client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "https";
                uriBuilder.Host = "munchyapitestmanager.azure-api.net";
                uriBuilder.Path = "Api/SaveToCookedRecipes";
                uriBuilder.Query = "userID=" + m_userID + "&recipeName=" + m_SuggestedRecipeNames[m_CurrentIndex] + "&recipeUUID=" + m_RecipeUserUUID[m_CurrentIndex] + "&locale=" + m_CurrentLocale;
                var uri = uriBuilder.ToString();

                var response = await client.GetAsync(uri);

                string responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        //Gets the recipe preview of a recipe from the saved, created or cooked recipes panel after which the preview setup is called.
        private async void GetRecipePreview(string recipeID, string recipeName)
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "munchyapitestmanager.azure-api.net";
            uriBuilder.Path = "Api/GetFullRecipeInfo";
            uriBuilder.Query = "RecipeName=" + recipeName + "&recipeID=" + recipeID + "&locale=" + m_CurrentLocale;
            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            var foodDataContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            var recipeContainer = JsonConvert.DeserializeObject<Object>(responseBody);
            dynamic d_RecipeData = recipeContainer;
            if (d_RecipeData.recipeName != null)
            {
                m_RecipePreviewName = d_RecipeData.recipeName.ToString();
            }

            if (d_RecipeData.recipeAuthor != null)
            {
                m_RecipePreviewAuthor = d_RecipeData.recipeAuthor.ToString();
            }

            if (d_RecipeData.recipeDesc != null)
            {
                m_RecipePreviewDesc = d_RecipeData.recipeDesc.ToString();
            }

            if (d_RecipeData.image != null)
            {
                m_RecipePreviewImg = d_RecipeData.image.ToString();
                GetPreviewImage( d_RecipeData.image.ToString());
                SetupPreview();
            }

        }

        //Handles publishing the recipe and interpreting the response from the server and notifying the user of the state of the publishing process. 
        private async void PublishRecipe()
        {
            if (PrepareForPublishing() == true)
            {
                var client = new HttpClient();

                // Request headers
                client.DefaultRequestHeaders.Add(m_headerParam, m_headerValue);

                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "https";
                uriBuilder.Host = "munchyapitestmanager.azure-api.net";
                uriBuilder.Path = "Api/CreateRecipe";
                uriBuilder.Query =
                    "locale=" + m_CurrentLocale +
                    "&userID=" + m_userID +
                    "&author=" + m_userName +
                    "&recipeName=" + m_NewRecipeName +
                    "&recipeDesc=" + m_NewRecipeDesc +
                    "&recipeIngr=" + m_SerIngrList +
                    "&recipeAmounts=" + m_SerIngrAmountList +
                    "&recipeUnits=" + m_SerIngrUnits +
                    "&recipeSteps=" + m_SerRecipeSteps +
                    "&recipeImageName=" + m_RecipeImage +
                    "&recipeTags=" + m_SerRecipeTags +
                    "&timeToCook=" + m_NewRecipeTimeToCook;
                var uri = uriBuilder.ToString();

                var response = await client.GetAsync(uri);

                string responseBody = await response.Content.ReadAsStringAsync();
                var publishResponse = JsonConvert.DeserializeObject<Object>(responseBody);
                var responseContainer = JsonConvert.DeserializeObject<Object>(responseBody);
                dynamic d_response = responseContainer;
                string status = d_response.uploadstatus;
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                Storyboard sb2 = this.FindResource("ShowCreatedRecipesGrid") as Storyboard;
                switch (status)
                {
                    case ("success"):
                        UploadImage();
                        if (sb2 != null) { BeginStoryboard(sb2); }
                        if (sb1 != null) { BeginStoryboard(sb1); }
                        switch (m_ActiveLanguage)
                        {
                            case Languages.English:
                                tb_InfoBlock.Text = "Recipe published!";
                                break;
                            case Languages.Bulgarian:
                                tb_InfoBlock.Text = "Рецептата публикувана";
                                break;
                        }
                        GetCreatedRecipes();
                        ClearRecipeInputs();
                        break;

                    case ("recipeexists"):
                        if (sb1 != null) { BeginStoryboard(sb1); }
                        switch (m_ActiveLanguage)
                        {
                            case Languages.English:
                                tb_InfoBlock.Text = "You've already created such a recipe.";
                                break;
                            case Languages.Bulgarian:
                                tb_InfoBlock.Text = "Вече сте публикували такава рецепта.";
                                break;
                        }
                        break;

                    case ("fail"):
                        if (sb1 != null) { BeginStoryboard(sb1); }
                        switch (m_ActiveLanguage)
                        {
                            case Languages.English:
                                tb_InfoBlock.Text = "A network error occured, please try again later.";
                                break;
                            case Languages.Bulgarian:
                                tb_InfoBlock.Text = "Възникна проблем във връзката, моля опитайте по-късно.";
                                break;
                        }
                        break;
                }
            }
        }

        #endregion

        #region API data uploads
        //Handles opening an image dialouge so the user can select an image to upload.
        private void OpenImage()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                m_OpenedImage = op.FileName;
                Uri imgURI = new Uri(op.FileName);
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = imgURI;
                img.EndInit();
                ImageBrush brush = new ImageBrush(img);
                RecipeImgInput.Fill = brush;
            }
        }

        //Handles uploading an image to azure blobs.
        private async void UploadImage()
        {
            //The following code is recommended by Microsoft for connection to Azure Storage.
            if (m_OpenedImage != "noimage")
            {
                m_imageName = m_NewRecipeName.ToLower() + m_userID;
                // This is one common way of creating a CloudStorageAccount object. You can get 
                // your Storage Account Name and Key from the Azure Portal.
                StorageCredentials credentials = new StorageCredentials("munchyimages", "Jl8zCfQ/snukpGPfyu34RmnqWTwxNR4TpmaeTbeU0rpPkYM7p8pEKTsRMG1oJafIk3hmw38MHvm5Nlhn5AXhOQ==");
                CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, useHttps: true);

                // Another common way to create a CloudStorageAccount object is to use a connection string:
                // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // This call creates a local CloudBlobContainer object, but does not make a network call
                // to the Azure Storage Service. The container on the service that this object represents may
                // or may not exist at this point. If it does exist, the properties will not yet have been
                // popluated on this object.
                CloudBlobContainer blobContainer = blobClient.GetContainerReference("recipeimages");

                // This makes an actual service call to the Azure Storage service. Unless this call fails,
                // the container will have been created.

                // This also does not make a service call, it only creates a local object.
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(m_imageName);

                // This transfers data in the file to the blob on the service.
                try
                {
                    blob.UploadFromFile(m_OpenedImage);
                }
                catch (StorageException e)
                {
                    Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                    if (sb1 != null) { BeginStoryboard(sb1); }
                    switch (m_ActiveLanguage)
                    {
                        case Languages.English:
                            tb_InfoBlock.Text = "An error occured while uploading the photo, please try again later.";
                            break;
                        case Languages.Bulgarian:
                            tb_InfoBlock.Text = "Получи се грешка при качването на снимката. Опитайте по-късно.";
                            break;
                    }
                }
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please select an image.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля изберете снимка.";
                        break;
                }
            }

        }

        //Gets the image of the current recipe being displayed.
        private async void GetImage(string currentImage)
        {
            if (currentImage != null)
            {
                //The following code is recommended by Microsoft for connection to Azure Storage.

                string ImgFolder = null;

                // Create a file in your local MyDocuments folder to upload to a blob.
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string localFileName = "tempImgs";
                ImgFolder = System.IO.Path.Combine(localPath, localFileName);
                string imgDest = System.IO.Path.Combine(ImgFolder, currentImage);
                if (!File.Exists(imgDest) || m_CurrentPreviewMode != PreviewModes.Normal || m_CurrentPreviewMode == PreviewModes.Normal && m_ImageNames.Count > 0)
                {
                    if (!Directory.Exists(ImgFolder))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(ImgFolder);
                    }



                    // This is one common way of creating a CloudStorageAccount object. You can get 
                    // your Storage Account Name and Key from the Azure Portal.
                    StorageCredentials credentials = new StorageCredentials("munchyimages", "Jl8zCfQ/snukpGPfyu34RmnqWTwxNR4TpmaeTbeU0rpPkYM7p8pEKTsRMG1oJafIk3hmw38MHvm5Nlhn5AXhOQ==");
                    CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, useHttps: true);

                    // Another common way to create a CloudStorageAccount object is to use a connection string:
                    // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // This call creates a local CloudBlobContainer object, but does not make a network call
                    // to the Azure Storage Service. The container on the service that this object represents may
                    // or may not exist at this point. If it does exist, the properties will not yet have been
                    // popluated on this object.
                    CloudBlobContainer blobContainer = blobClient.GetContainerReference("recipeimages");

                    // This also does not make a service call, it only creates a local object.
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(currentImage);
                    try
                    {

                        if (blob.Exists())
                        {
                            await blob.DownloadToFileAsync(imgDest, FileMode.Create);
                            Uri imgURI = new Uri(imgDest);
                            var img = new BitmapImage();
                            img.BeginInit();
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.UriSource = imgURI;
                            img.EndInit();
                            //ImageBrush brush = new ImageBrush(new BitmapImage(imgURI));
                            ImageBrush brush = new ImageBrush(img);
                            Img_RecipeImage.Fill = brush;
                            recipeLoaded = true;
                        }
                    }
                    catch (StorageException ex)
                    {
                        recipeLoaded = true;
                        Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                        if (sb1 != null) { BeginStoryboard(sb1); }
                        switch (m_ActiveLanguage)
                        {
                            case Languages.English:
                                tb_InfoBlock.Text = "Image failed to load, try again later.";
                                break;
                            case Languages.Bulgarian:
                                tb_InfoBlock.Text = "Снимката не успя да зареди, опитайте по-късно.";
                                break;
                        }
                    }
                }
                else
                {
                    try
                    {
                        Uri imgURI = new Uri(imgDest);
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.UriSource = imgURI;
                        img.EndInit();

                        //ImageBrush brush = new ImageBrush(new BitmapImage(imgURI));
                        ImageBrush brush = new ImageBrush(img);
                        Img_RecipeImage.Fill = brush;
                        recipeLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                        if (sb1 != null) { BeginStoryboard(sb1); }
                        switch (m_ActiveLanguage)
                        {
                            case Languages.English:
                                tb_InfoBlock.Text = "Image failed to load, try again later.";
                                break;
                            case Languages.Bulgarian:
                                tb_InfoBlock.Text = "Снимката не успя да зареди, опитайте по-късно.";
                                break;
                        }
                    }
                }
            }        
        }

        //Gets the image of the recipe that the user wants to preview.
        private async void GetPreviewImage(string recipePreviewImg)
        {
            string ImgFolder = null;
           
            // Create a file in your local MyDocuments folder to upload to a blob.
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string localFileName = "previeImgs";
            ImgFolder = System.IO.Path.Combine(localPath, localFileName);
            string imgDest = System.IO.Path.Combine(ImgFolder, recipePreviewImg);

            if (!Directory.Exists(ImgFolder))
            {
                DirectoryInfo di = Directory.CreateDirectory(ImgFolder);
            }

            if (!File.Exists(imgDest) || m_CurrentPreviewMode != PreviewModes.Normal || m_CurrentPreviewMode == PreviewModes.Normal && m_ImageNames.Count > 0)
            {


                // This is one common way of creating a CloudStorageAccount object. You can get 
                // your Storage Account Name and Key from the Azure Portal.
                StorageCredentials credentials = new StorageCredentials("munchyimages", "Jl8zCfQ/snukpGPfyu34RmnqWTwxNR4TpmaeTbeU0rpPkYM7p8pEKTsRMG1oJafIk3hmw38MHvm5Nlhn5AXhOQ==");
                CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, useHttps: true);

                // Another common way to create a CloudStorageAccount object is to use a connection string:
                // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // This call creates a local CloudBlobContainer object, but does not make a network call
                // to the Azure Storage Service. The container on the service that this object represents may
                // or may not exist at this point. If it does exist, the properties will not yet have been
                // popluated on this object.
                CloudBlobContainer blobContainer = blobClient.GetContainerReference("recipeimages");

                // This also does not make a service call, it only creates a local object.
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(recipePreviewImg);
                try
                {

                    if (blob.Exists())
                    {
                        await blob.DownloadToFileAsync(imgDest, FileMode.Create);
                        Uri imgURI = new Uri(imgDest);
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.UriSource = imgURI;
                        img.EndInit();

                        ImageBrush brush = new ImageBrush(img);

                        switch (m_CurrentPreviewMode)
                        {
                            case PreviewModes.Saved:
                                Img_prev_saved.Fill = brush;

                                break;
                            case PreviewModes.Created:
                                Img_prev_created.Fill = brush;

                                break;
                            case PreviewModes.Cooked:
                                Img_prev_cooked.Fill = brush;

                                break;
                        }
                    }
                }
                catch (StorageException ex)
                {
                    Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                    if (sb1 != null) { BeginStoryboard(sb1); }
                    switch (m_ActiveLanguage)
                    {
                        case Languages.English:
                            tb_InfoBlock.Text = "Image failed to load, try again later.";
                            break;
                        case Languages.Bulgarian:
                            tb_InfoBlock.Text = "Снимката не успя да зареди, опитайте по-късно.";
                            break;
                    }
                }
            }
            else
            {
                try
                {
                    Uri imgURI = new Uri(imgDest);
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.UriSource = imgURI;
                    img.EndInit();

                    //ImageBrush brush = new ImageBrush(new BitmapImage(imgURI));
                    ImageBrush brush = new ImageBrush(img);
                    switch (m_CurrentPreviewMode)
                    {
                        case PreviewModes.Saved:
                            Img_prev_saved.Fill = brush;

                            break;
                        case PreviewModes.Created:
                            Img_prev_created.Fill = brush;

                            break;
                        case PreviewModes.Cooked:
                            Img_prev_cooked.Fill = brush;

                            break;
                    }
                }
               catch(Exception ex)
                {
                    Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                    if (sb1 != null) { BeginStoryboard(sb1); }
                    switch (m_ActiveLanguage)
                    {
                        case Languages.English:
                            tb_InfoBlock.Text = "Image failed to load, try again later.";
                            break;
                        case Languages.Bulgarian:
                            tb_InfoBlock.Text = "Снимката не успя да зареди, опитайте по-късно.";
                            break;
                    }
                }
            }
        }

        //Handles adding an ingredient to the recipe being created.
        private void AddRecipeIngredient()
        {
            string ingrNameToAdd = "";
            float ingrAmountToAdd = 0;
            string ingrUnitToAdd = "";

            if (float.TryParse(Tb_IngrAmount.Text, out float parsedValue))
            {
                ingrAmountToAdd = parsedValue;
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please provide a valid ingredient amount.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля въведете валидно количество за съставката.";
                        break;
                }
            }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    if (Lb_IngrSearchResultList.SelectedItem != null)
                    {
                        ingrNameToAdd = Lb_IngrSearchResultList.SelectedItem.ToString();
                    }

                    break;
                case Languages.Bulgarian:
                    if (Lb_IngrSearchResultList.SelectedItem != null)
                    {
                        ingrNameToAdd = m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_IngrSearchResultList.SelectedItem.ToString())];
                    }

                    break;
                default:
                    break;
            }

            foreach (RadioButton unit in Lb_UnitList.Items)
            {
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        if (unit.IsChecked == true)
                        {
                            ingrUnitToAdd = unit.Content.ToString();
                        }
                        break;
                    case Languages.Bulgarian:
                        if (unit.IsChecked == true)
                        {
                            ingrUnitToAdd = m_USUOM[m_BGUOM.IndexOf(unit.Content.ToString())];
                        }
                        break;
                }
            }

            if (!m_NewRecipeIngrs.Contains(ingrNameToAdd))
            {
                m_NewRecipeIngrs.Add(ingrNameToAdd);
                m_NewRecipeIngrAmounts.Add(ingrAmountToAdd);
                m_NewRecipeUnits.Add(ingrUnitToAdd);
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        Lb_CurrentIngrList.Items.Add(ingrAmountToAdd + "  " + ingrNameToAdd + "  " + ingrUnitToAdd);
                        Lb_IngredientList.Items.Add(ingrAmountToAdd + "  " + ingrNameToAdd + "  " + ingrUnitToAdd);
                        break;
                    case Languages.Bulgarian:
                        Lb_CurrentIngrList.Items.Add(ingrAmountToAdd + "  " + m_foodDataNamesBG[m_foodDataNamesUS.IndexOf(ingrNameToAdd)] + "  " + m_BGUOM[m_USUOM.IndexOf(ingrUnitToAdd)]);
                        Lb_IngredientList.Items.Add(ingrAmountToAdd + "  " + m_foodDataNamesBG[m_foodDataNamesUS.IndexOf(ingrNameToAdd)] + "  " + m_BGUOM[m_USUOM.IndexOf(ingrUnitToAdd)]);
                        break;
                    default:
                        break;
                }
            }
        }

        //Handles remove an ingredient from the list added to the recipe.
        private void RemoveIngredient()
        {
            m_NewRecipeIngrs.RemoveAt(Lb_CurrentIngrList.SelectedIndex);
            m_NewRecipeIngrAmounts.RemoveAt(Lb_CurrentIngrList.SelectedIndex);
            m_NewRecipeUnits.RemoveAt(Lb_CurrentIngrList.SelectedIndex);
            Lb_IngredientList.Items.RemoveAt(Lb_CurrentIngrList.SelectedIndex);
            Lb_IngredientList.Items.Refresh();
            Lb_CurrentIngrList.Items.RemoveAt(Lb_CurrentIngrList.SelectedIndex);
            Lb_CurrentIngrList.Items.Refresh();

        }

        //Cleares all ingredients from the recipe.
        private void ClearIngrs()
        {
            m_NewRecipeIngrs.Clear();
            m_NewRecipeIngrAmounts.Clear();
            m_NewRecipeUnits.Clear();
            Lb_CurrentIngrList.Items.Clear();
            Lb_CurrentIngrList.Items.Refresh();
            Lb_IngredientList.Items.Clear();
            Lb_IngredientList.Items.Refresh();
        }

        //Initializes the list that displays all the available ingredients. 
        private void InitRecipeIngrSearch()
        {
            Lb_IngrSearchResultList.Items.Clear();
            Lb_IngrSearchResultList.Items.Refresh();

            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    foreach (string item in m_foodDataNamesUS)
                    {
                        Lb_IngrSearchResultList.Items.Add(item);
                    }
                    break;
                case Languages.Bulgarian:
                    foreach (string item in m_foodDataNamesBG)
                    {
                        Lb_IngrSearchResultList.Items.Add(item);
                    }
                    break;
                default:
                    break;
            }
        }

        //Handles adding a new step to the list of recipe steps.
        private void AddNewStep()
        {
            if (!string.IsNullOrWhiteSpace(Tb_StepInput.Text))
            {

                if (!m_NewRecipeSteps.Contains(Tb_StepInput.Text))
                {
                    m_NewRecipeSteps.Add(Tb_StepInput.Text);
                    Lb_RecipeStepInputList.Items.Add(Tb_StepInput.Text);
                    Lb_StepList.Items.Add(Tb_StepInput.Text);
                    Lb_StepList.Items.Refresh();
                }
                else
                {
                    //Notify the user.
                }
            }
        }

        //Removes a step from the list of recipe steps.
        private void RemoveStep()
        {
            if (Lb_RecipeStepInputList.SelectedItem.ToString() != null)
            {
                m_NewRecipeSteps.Remove(Lb_RecipeStepInputList.SelectedItem.ToString());
                Lb_RecipeStepInputList.Items.RemoveAt(Lb_RecipeStepInputList.SelectedIndex);
                Lb_RecipeStepInputList.Items.Refresh();
                Lb_StepList.Items.Remove(Lb_RecipeStepInputList.SelectedItem.ToString());
                Lb_StepList.Items.Refresh();
            }
        }

        private string GetRecipeTag(string tag)
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    return RecipeTagTranslationsUS[tag];

                case Languages.Bulgarian:
                    return RecipeTagTranslationsBG[tag];

                default:
                    return RecipeTagTranslationsUS[tag];
            }
        }
        //Adds tags to the list of recipe tags.
        private void AddRecipeTag(string cbName)
        {
            switch (cbName)
            {
                case "cb_vegan":
                    if (!m_NewRecipeTags.Contains("isvegan"))
                    {
                        m_NewRecipeTags.Add("isvegan");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("isvegan"));
                        UpdateTagList();
                    }
                    break;

                case "cb_vegetarian":
                    if (!m_NewRecipeTags.Contains("isvegetarian"))
                    {
                        m_NewRecipeTags.Add("isvegetarian");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("isvegetarian"));
                        UpdateTagList();
                    }
                    break;

                case "cb_diabetic":
                    if (!m_NewRecipeTags.Contains("isdiabetic"))
                    {
                        m_NewRecipeTags.Add("isdiabetic");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("isdiabetic"));
                        UpdateTagList();
                    }
                    break;

                case "cb_a_dairy":
                    if (!m_NewRecipeTags.Contains("dairy"))
                    {
                        m_NewRecipeTags.Add("dairy");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("dairy"));
                        UpdateTagList();
                    }
                    break;

                case "cb_a_eggs":
                    if (!m_NewRecipeTags.Contains("eggs"))
                    {
                        m_NewRecipeTags.Add("eggs");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("eggs"));
                        UpdateTagList();
                    }
                    break;

                case "cb_a_gluten":
                    if (!m_NewRecipeTags.Contains("gluten"))
                    {
                        m_NewRecipeTags.Add("gluten");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("gluten"));
                        UpdateTagList();
                    }
                    break;

                case "cb_a_fish":
                    if (!m_NewRecipeTags.Contains("fish"))
                    {
                        m_NewRecipeTags.Add("fish");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("fish"));
                        UpdateTagList();
                    }
                    break;

                case "cb_a_nuts":
                    if (!m_NewRecipeTags.Contains("nuts"))
                    {
                        m_NewRecipeTags.Add("nuts");
                        m_CreatedRecipeDisplayTags.Add(GetRecipeTag("nuts"));
                        UpdateTagList();
                    }
                    break;
            }
        }

        //Updates the tag list for the recipe.
        private void UpdateTagList()
        {
            foreach (string item in m_CreatedRecipeDisplayTags)
            {
                if (!Lb_RecipeTagList.Items.Contains(item))
                {
                    Lb_RecipeTagList.Items.Add(item);
                }
            }
        }

        //Removes a tag from the list of recipe tags.
        private void RemoveTag(string cbName)
        {
            switch (cbName)
            {
                case "cb_vegan":
                    if (m_NewRecipeTags.Contains("isvegan"))
                    {
                        m_NewRecipeTags.Remove("isvegan");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("isvegan"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("isvegan"));
                    }
                    break;

                case "cb_vegetarian":
                    if (m_NewRecipeTags.Contains("isvegetarian"))
                    {
                        m_NewRecipeTags.Remove("isvegetarian");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("isvegetarian"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("isvegetarian"));
                    }
                    break;

                case "cb_diabetic":
                    if (m_NewRecipeTags.Contains("isdiabetic"))
                    {
                        m_NewRecipeTags.Remove("isdiabetic");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("isdiabetic"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("isdiabetic"));
                    }
                    break;

                case "cb_a_dairy":
                    if (m_NewRecipeTags.Contains("dairy"))
                    {
                        m_NewRecipeTags.Remove("dairy");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("dairy"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("dairy"));
                    }
                    break;

                case "cb_a_eggs":
                    if (m_NewRecipeTags.Contains("eggs"))
                    {
                        m_NewRecipeTags.Remove("eggs");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("eggs"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("eggs"));
                    }
                    break;

                case "cb_a_gluten":
                    if (m_NewRecipeTags.Contains("gluten"))
                    {
                        m_NewRecipeTags.Remove("gluten");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("gluten"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("gluten"));
                    }
                    break;

                case "cb_a_fish":
                    if (m_NewRecipeTags.Contains("fish"))
                    {
                        m_NewRecipeTags.Remove("fish");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("fish"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("fish"));
                    }
                    break;

                case "cb_a_nuts":
                    if (m_NewRecipeTags.Contains("nuts"))
                    {
                        m_NewRecipeTags.Remove("nuts");
                        m_CreatedRecipeDisplayTags.Remove(GetRecipeTag("nuts"));
                        Lb_RecipeTagList.Items.Remove(GetRecipeTag("nuts"));
                    }
                    break;
            }
        }

        //Adds a time tag to the recipe.
        private void AddTimeTag(string cbName)
        {
            switch (cbName)
            {
                case "cb_AddBreakfast":
                    if (!m_NewRecipeTimeTags.Contains("breakfast"))
                    {
                        m_NewRecipeTimeTags.Add("breakfast");
                    }
                    break;

                case "cb_AddLunch":
                    if (!m_NewRecipeTimeTags.Contains("breakfast"))
                    {
                        m_NewRecipeTimeTags.Add("breakfast");
                    }
                    break;

                case "cb_AddDinner":
                    if (!m_NewRecipeTimeTags.Contains("dinner"))
                    {
                        m_NewRecipeTimeTags.Add("breakfast");
                    }
                    break;
            }
        }

        //Removes a time tag from the list of recipe tags.
        private void RemoveTimeTag(string cbName)
        {
            switch (cbName)
            {
                case "cb_AddBreakfast":
                    if (m_NewRecipeTags.Contains("breakfast"))
                    {
                        m_NewRecipeTags.Remove("breakfast");
                    }
                    break;

                case "cb_AddLunch":
                    if (m_NewRecipeTags.Contains("breakfast"))
                    {
                        m_NewRecipeTags.Remove("breakfast");
                    }
                    break;

                case "cb_AddDinner":
                    if (m_NewRecipeTags.Contains("dinner"))
                    {
                        m_NewRecipeTags.Remove("breakfast");
                    }
                    break;
            }
        }

        private void ClearRecipeInputs()
        {
            Lb_StepList.Items.Clear();
            Lb_IngredientList.Items.Clear();
            Lb_RecipeTagList.Items.Clear();
            Lb_RecipeStepInputList.Items.Clear();
            Lb_CurrentIngrList.Items.Clear();


            Lb_StepList.Items.Refresh();
            Lb_IngredientList.Items.Refresh();
            Lb_RecipeTagList.Items.Refresh();
            Lb_RecipeStepInputList.Items.Refresh();
            Lb_CurrentIngrList.Items.Refresh();
        
            m_NewRecipeIngrs.Clear();
            m_NewRecipeIngrAmounts.Clear();
            m_NewRecipeIngrs.Clear();
            m_NewRecipeTags.Clear();
            m_NewRecipeSteps.Clear();
        }

        //Checks if the recipe is ready for publishing. If its not it notifies the user.
        private bool PrepareForPublishing()
        {
            if (!string.IsNullOrWhiteSpace(Tb_RecipeNameInput.Text))
            {
                m_NewRecipeName = Tb_RecipeNameInput.Text;
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please provide a name.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля дайте име на рецептата си.";
                        break;
                }
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Tb_DescInput.Text))
            {
                m_NewRecipeDesc = Tb_DescInput.Text;
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please describe your recipe.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля опишете рецептата си.";
                        break;
                }
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Tb_TimeToCookInput.Text))
            {
                m_NewRecipeTimeToCook = Tb_TimeToCookInput.Text;
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please provide a time to cook.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля поставете време за готвене.";
                        break;
                }
                return false;
            }

            if (m_OpenedImage != "noimage")
            {
                m_RecipeImage = m_NewRecipeName.ToLower() + m_userID;
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please select an image.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля изберете снимка.";
                        break;
                }
                return false;
            }


            if (m_NewRecipeIngrs.Count > 0 && m_NewRecipeIngrs.Count == m_NewRecipeIngrAmounts.Count && m_NewRecipeUnits.Count == m_NewRecipeIngrs.Count)
            {
                m_SerIngrList = JsonConvert.SerializeObject(m_NewRecipeIngrs);
                m_SerIngrAmountList = JsonConvert.SerializeObject(m_NewRecipeIngrAmounts);
                m_SerIngrUnits = JsonConvert.SerializeObject(m_NewRecipeUnits);
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "An error occured with the ingredients, please re-do them.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Получи се грешка с продуктите, моля ги преправете.";
                        break;
                }
                return false;
            }

            if (m_NewRecipeSteps.Count > 0)
            {
                m_SerRecipeSteps = JsonConvert.SerializeObject(m_NewRecipeSteps);
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "Please provide recipe steps.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Моля сложете стъпки за рецептата.";
                        break;
                }
                return false;
            }

            if (m_NewRecipeTags.Count >= 0)
            {
                m_SerRecipeTags = JsonConvert.SerializeObject(m_NewRecipeTags);
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "The recipe will be saved without tags";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Рецептата ще се запази без тагове";
                        break;
                }
                return false;
            }

            return true;
        }

        private string GetUserTags(string tag)
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    return TagTranslationsUS[tag];                    
                case Languages.Bulgarian:
                    return TagTranslationsBG[tag];                    
                default:
                    return TagTranslationsUS[tag];                    
            }
        }

        //Handles adding preference tags to the users preference list.
        private void AddPrefTag(string cbName)
        {
            switch (cbName)
            {
                case "cb_vegan_usertag":
                    if (!m_UserPrefTags.Contains("isvegan"))
                    {
                        m_UserPrefTags.Add("isvegan");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_vegetarian_usertag":
                    if (!m_UserPrefTags.Contains("isvegetarian"))
                    {
                        m_UserPrefTags.Add("isvegetarian");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_diabetic_usertag":
                    if (!m_UserPrefTags.Contains("isdiabetic"))
                    {
                        m_UserPrefTags.Add("isdiabetic");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_dairy_usertag":
                    if (!m_UserPrefTags.Contains("dairy"))
                    {
                        m_UserPrefTags.Add("dairy");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_eggs_usertag":
                    if (!m_UserPrefTags.Contains("eggs"))
                    {
                        m_UserPrefTags.Add("eggs");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_gluten_usertag":
                    if (!m_UserPrefTags.Contains("gluten"))
                    {
                        m_UserPrefTags.Add("gluten");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_fish_usertag":
                    if (!m_UserPrefTags.Contains("fish"))
                    {
                        m_UserPrefTags.Add("fish");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_nuts_usertag":
                    if (!m_UserPrefTags.Contains("nuts"))
                    {
                        m_UserPrefTags.Add("nuts");
                        UpdatePrefTagList();
                    }
                    break;
            }
        }

        //Handles removing preference tags from the list of user preferences.
        private void RemovePrefTag(string cbName)
        {
            switch (cbName)
            {
                case "cb_vegan_usertag":
                    if (m_UserPrefTags.Contains("isvegan"))
                    {
                        m_UserPrefTags.Remove("isvegan");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_vegetarian_usertag":
                    if (m_UserPrefTags.Contains("isvegetarian"))
                    {
                        m_UserPrefTags.Remove("isvegetarian");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_diabetic_usertag":
                    if (m_UserPrefTags.Contains("isdiabetic"))
                    {
                        m_UserPrefTags.Remove("isdiabetic");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_dairy_usertag":
                    if (m_UserPrefTags.Contains("dairy"))
                    {
                        m_UserPrefTags.Remove("dairy");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_eggs_usertag":
                    if (m_UserPrefTags.Contains("eggs"))
                    {
                        m_UserPrefTags.Remove("eggs");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_gluten_usertag":
                    if (m_UserPrefTags.Contains("gluten"))
                    {
                        m_UserPrefTags.Remove("gluten");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_fish_usertag":
                    if (m_UserPrefTags.Contains("fish"))
                    {
                        m_UserPrefTags.Remove("fish");
                        UpdatePrefTagList();
                    }
                    break;

                case "cb_a_nuts_usertag":
                    if (m_UserPrefTags.Contains("nuts"))
                    {
                        m_UserPrefTags.Remove("nuts");
                        UpdatePrefTagList();
                    }
                    break;
            }
        }

        //Updates the list that displays the preference tags.
        private void UpdatePrefTagList()
        {
            Lb_PreferenceList.Items.Clear();
            Lb_PreferenceList.Items.Refresh();
            foreach (string item in m_UserPrefTags)
            {
                if (!Lb_PreferenceList.Items.Contains(GetUserTags(item)))
                {
                    Lb_PreferenceList.Items.Add(GetUserTags(item));
                }
            }
        }
        #endregion

        #region UI data management logic

        //Handles initializing the main elements of the progam as well as the home page.
        private void InitHomePage()
        {
            GetFoodData();
            Tb_DrawerName.Text = m_userName;
            Tb_ProfileName.Text = m_userName;
            GetSuggestedRecipes();
            CreateUser();
            GetUser();
            GetShoppingList();
            GetFridge();
        }

        //Initializes the lists for english and bulgarian food names.
        private void InitFoodData()
        {
            foreach (KeyValuePair<string, string> item in m_FoodNames)
            {
                m_foodDataNamesUS.Add(item.Key);
                m_foodDataNamesBG.Add(item.Value);
            }
            InitFridgeSearch();
            InitShoppingListSearch();
        }

        //Sets up the preference list for user preferences.
        private void SetupPrefList()
        {
            foreach (string tag in m_UserPrefTags)
            {
                if (!Lb_PreferenceList.Items.Contains(GetUserTags(tag)))
                    Lb_PreferenceList.Items.Add(GetUserTags(tag));
            }
        }

        //Sets up the recipe that is supposed to be viewed.
        private void SetupRecipe()
        {
            Lb_RecipeIngrList.Items.Clear();
            Lb_RecipeIngrList.Items.Refresh();


            Lb_RecipeStepList.Items.Clear();
            Lb_RecipeStepList.Items.Refresh();

            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    TextBlock ingrTitle = new TextBlock
                    {
                        Text = "Ingredients",
                        FontSize = 30,
                        FontFamily = new FontFamily("Roboto Black")
                    };
                    Lb_RecipeIngrList.Items.Add(ingrTitle);

                    TextBlock stepsTitle = new TextBlock
                    {
                        Text = "Steps",
                        FontSize = 30,
                        FontFamily = new FontFamily("Roboto Black")
                    };
                    Lb_RecipeStepList.Items.Add(stepsTitle);
                    break;
                case Languages.Bulgarian:
                    TextBlock ingrTitleBG = new TextBlock
                    {
                        Text = "Съставки",
                        FontSize = 30,
                        FontFamily = new FontFamily("Roboto Black")
                    };
                    Lb_RecipeIngrList.Items.Add(ingrTitleBG);

                    TextBlock stepsTitleBG = new TextBlock
                    {
                        Text = "Стъпки",
                        FontSize = 30,
                        FontFamily = new FontFamily("Roboto Black")
                    };
                    Lb_RecipeStepList.Items.Add(stepsTitleBG);
                    break;
                default:
                    break;
            }

            Tb_RecipeName.Text = m_RecipeName;
            Tb_RecipeAuthor.Text = m_RecipeAuthor;
            Tb_RecipeDesc.Text = m_RecipeDesc;

            string IngrFormat = "";
            foreach (string ingr in m_CurrentRecipeIngrs)
            {
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        IngrFormat = m_CurrentRecipeAmounts[m_CurrentRecipeIngrs.IndexOf(ingr)].ToString() + "  " + m_CurrentRecipeUnits[m_CurrentRecipeIngrs.IndexOf(ingr)] + "  " + ingr;
                        break;
                    case Languages.Bulgarian:
                        IngrFormat = m_CurrentRecipeAmounts[m_CurrentRecipeIngrs.IndexOf(ingr)].ToString() + "  " + m_BGUOM[m_USUOM.IndexOf(m_CurrentRecipeUnits[m_CurrentRecipeIngrs.IndexOf(ingr)])] + "  " + m_foodDataNamesBG[m_foodDataNamesUS.IndexOf(ingr)];
                        break;
                }
                if (!Lb_IngredientList.Items.Contains(IngrFormat))
                {
                    Lb_RecipeIngrList.Items.Add(IngrFormat);
                }
            }

            foreach (string step in m_CurrentRecipeSteps)
            {
                if (!Lb_RecipeStepList.Items.Contains(step))
                {
                    Lb_RecipeStepList.Items.Add(((m_CurrentRecipeSteps.IndexOf(step)) + 1).ToString() + ". " + step);
                }
            }
        }

        //Handles the flow of showing the next recipe.
        private void NextRecipe()
        {
            m_CurrentPreviewMode = PreviewModes.Normal;
            if (m_CurrentIndex < m_SuggestedRecipeNames.Count - 1)
            {
                if (recipeLoaded == true)
                {
                    m_CurrentIndex += 1;
                    GetFullRecipeInfo(m_SuggestedRecipeNames[m_CurrentIndex], m_RecipeUserUUID[m_CurrentIndex], m_ImageNames[m_CurrentIndex]);
                }
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "You ran out of recipes! Try again later.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Рецептите свъшиха! Пробвайте по-късно.";
                        break;
                }
            }
        }

        //Handles the flow of showing the previous recipe.
        private void PreviousRecipe()
        {
            m_CurrentPreviewMode = PreviewModes.Normal;
            if (m_CurrentIndex > 0)
            {
                if (recipeLoaded == true)
                {
                    m_CurrentIndex -= 1;
                    GetFullRecipeInfo(m_SuggestedRecipeNames[m_CurrentIndex], m_RecipeUserUUID[m_CurrentIndex], m_ImageNames[m_CurrentIndex]);
                }
            }
            else
            {
                Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
                if (sb1 != null) { BeginStoryboard(sb1); }
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_InfoBlock.Text = "You ran out of recipes! Try again later.";
                        break;
                    case Languages.Bulgarian:
                        tb_InfoBlock.Text = "Рецептите свъшиха! Пробвайте по-късно.";
                        break;
                }
            }

        }

        //Sets up the fridge of the user.
        private void SetupFridge()
        {
            Lb_Fridge.Items.Clear();
            Lb_Fridge.Items.Refresh();
            m_CurrentFridgeItems.Clear();

            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    foreach (KeyValuePair<string, float> item in m_CurrentFridge)
                    {
                        Lb_Fridge.Items.Add(item.Key + "  " + item.Value + "  " + m_FoodUnits[item.Key]); m_CurrentFridgeItems.Add(item.Key);
                        if (!m_CurrentFridgeItems.Contains(item.Key))
                        {
                            m_CurrentFridgeItems.Add(item.Key);
                        }
                    }
                    break;
                case Languages.Bulgarian:
                    foreach (KeyValuePair<string, float> item in m_CurrentFridge)
                    {
                        Lb_Fridge.Items.Add(m_foodDataNamesBG[m_foodDataNamesUS.IndexOf(item.Key)] + "  " + item.Value + "  " + m_BGUOM[m_USUOM.IndexOf(m_FoodUnits[item.Key])]);
                        if (!m_CurrentFridgeItems.Contains(item.Key))
                        {
                            m_CurrentFridgeItems.Add(item.Key);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        //Initializes the ingredients in the list that shows what food items can be added into the fridge.
        private void InitFridgeSearch()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    Lb_FridgeSearchResults.Items.Clear();
                    Lb_FridgeSearchResults.Items.Refresh();
                    foreach (string item in m_foodDataNamesUS)
                    {
                        Lb_FridgeSearchResults.Items.Add(item);
                    }
                    break;
                case Languages.Bulgarian:
                    Lb_FridgeSearchResults.Items.Clear();
                    Lb_FridgeSearchResults.Items.Refresh();
                    foreach (string item in m_foodDataNamesBG)
                    {
                        Lb_FridgeSearchResults.Items.Add(item);
                    }
                    break;
            }

        }

        //Handles the on selection change event when an item in the fridge search list box is selected.
        private void OnFridgeSearchResultSelected()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    if (Lb_FridgeSearchResults.SelectedItem != null)
                    {
                        Tb_CustomAmountInput.Text = m_SuggestedFoodAmounts[Lb_FridgeSearchResults.SelectedItem.ToString()].ToString();
                    }
                    break;
                case Languages.Bulgarian:
                    if (Lb_FridgeSearchResults.SelectedItem != null)
                    {
                        Tb_CustomAmountInput.Text = m_SuggestedFoodAmounts[m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_FridgeSearchResults.SelectedItem.ToString())]].ToString();
                    }
                    break;
                default:
                    break;
            }

        }

        //Handles adding the selected item to the fridge.
        private void AddSelectionToFridge()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    if (Lb_FridgeSearchResults.SelectedItem != null && !string.IsNullOrWhiteSpace(Tb_CustomAmountInput.Text))
                    {
                        if (!m_CurrentFridge.ContainsKey(Lb_FridgeSearchResults.SelectedItem.ToString()) && float.TryParse(Tb_CustomAmountInput.Text, out float parsedValue))
                        {
                            m_CurrentFridge.Add(Lb_FridgeSearchResults.SelectedItem.ToString(), parsedValue);
                            SaveFridge();
                        }
                    }
                    break;
                case Languages.Bulgarian:
                    if (Lb_FridgeSearchResults.SelectedItem != null && !string.IsNullOrWhiteSpace(Tb_CustomAmountInput.Text))
                    {
                        if (!m_CurrentFridge.ContainsKey(m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_FridgeSearchResults.SelectedItem.ToString())]) && float.TryParse(Tb_CustomAmountInput.Text, out float parsedValue))
                        {
                            m_CurrentFridge.Add(m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_FridgeSearchResults.SelectedItem.ToString())], parsedValue);
                            SaveFridge();
                        }
                    }
                    break;
                default:
                    break;

            }
        }

        //Handles removing the item that is selected from the fidge.
        private void RemoveFromFridge()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    if (Lb_Fridge.SelectedItem != null)
                    {
                        int i = Lb_Fridge.SelectedIndex;
                        string stuff = m_CurrentFridgeItems.ToString();
                        if (m_CurrentFridge.ContainsKey(m_CurrentFridgeItems[Lb_Fridge.SelectedIndex]))
                        {
                            m_CurrentFridge.Remove(m_CurrentFridgeItems[Lb_Fridge.SelectedIndex]);
                            SaveFridge();
                        }
                    }
                    break;
                case Languages.Bulgarian:
                    if (Lb_Fridge.SelectedItem != null)
                    {
                        int i = Lb_Fridge.SelectedIndex;
                        if (m_CurrentFridge.ContainsKey(m_CurrentFridgeItems[Lb_Fridge.SelectedIndex]))
                        {
                            m_CurrentFridge.Remove(m_CurrentFridgeItems[Lb_Fridge.SelectedIndex]);
                            SaveFridge();
                        }
                    }
                    break;
                default:
                    break;

            }
        }

        private void SearchingForFridgeIngrs()
        {
            // TO DO
        }

        //Initializes the shopping list search list.
        private void InitShoppingListSearch()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    Lb_ShoppingListSearchResults.Items.Clear();
                    Lb_ShoppingListSearchResults.Items.Refresh();
                    foreach (string item in m_foodDataNamesUS)
                    {
                        Lb_ShoppingListSearchResults.Items.Add(item);
                    }
                    break;
                case Languages.Bulgarian:
                    Lb_ShoppingListSearchResults.Items.Clear();
                    Lb_ShoppingListSearchResults.Items.Refresh();
                    foreach (string item in m_foodDataNamesBG)
                    {
                        Lb_ShoppingListSearchResults.Items.Add(item);
                    }
                    break;
            }
        }

        //Populates the shopping list of the user.
        private void SetupShoppingList()
        {
            if (m_foodDataNamesBG.Count > 0 && m_foodDataNamesUS.Count > 0)
            {
                Lb_ShoppingList.Items.Clear();
                Lb_ShoppingList.Items.Refresh();

                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        foreach (string item in m_CurrentShoppingList)
                        {
                            Lb_ShoppingList.Items.Add(item);
                        }
                        break;
                    case Languages.Bulgarian:
                        foreach (string item in m_CurrentShoppingList)
                        {
                            Lb_ShoppingList.Items.Add(m_foodDataNamesBG[m_foodDataNamesUS.IndexOf(item)]);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void SearchingForShoppingListItems()
        {
            // TO DO
        }

        //Adds the selected item to the shopping list.
        private void AddSelectionToShoppingList()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    if (Lb_ShoppingListSearchResults.SelectedItem != null)
                    {
                        if (!m_CurrentShoppingList.Contains(Lb_ShoppingListSearchResults.SelectedItem.ToString()))
                        {
                            m_CurrentShoppingList.Add(Lb_ShoppingListSearchResults.SelectedItem.ToString());
                            SaveShoppingList();
                        }
                    }
                    break;
                case Languages.Bulgarian:
                    if (Lb_ShoppingListSearchResults.SelectedItem != null)
                    {
                        if (!m_CurrentShoppingList.Contains(m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_ShoppingListSearchResults.SelectedItem.ToString())]))
                        {
                            m_CurrentShoppingList.Add(m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_ShoppingListSearchResults.SelectedItem.ToString())]);
                            SaveShoppingList();
                        }
                    }
                    break;
            }
        }

        //Removes the selected item from the shopping list.
        private void RemoveFromShoppingList()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    if (Lb_ShoppingList.SelectedItem != null)
                    {
                        if (m_CurrentShoppingList.Contains(Lb_ShoppingList.SelectedItem.ToString()))
                        {
                            m_CurrentShoppingList.Remove(Lb_ShoppingList.SelectedItem.ToString());
                            SaveShoppingList();
                        }
                    }
                    break;
                case Languages.Bulgarian:
                    if (Lb_ShoppingList.SelectedItem != null)
                    {
                        if (m_CurrentShoppingList.Contains(m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_ShoppingList.SelectedItem.ToString())]))
                        {
                            m_CurrentShoppingList.Remove(m_foodDataNamesUS[m_foodDataNamesBG.IndexOf(Lb_ShoppingList.SelectedItem.ToString())]);
                            SaveShoppingList();
                        }
                    }
                    break;
            }
        }

        //Sets up the list of saved recipes.
        private void SetupSavedRecipes()
        {
            Lb_SavedRecipes.Items.Clear();
            Lb_SavedRecipes.Items.Refresh();
            foreach (KeyValuePair<string, string> recipe in m_SavedRecipes)
            {
                if (!Lb_SavedRecipes.Items.Contains(recipe.Key))
                {
                    Lb_SavedRecipes.Items.Add(recipe.Key);
                }
            }
        }

        //Sets up the list of created recipes.
        private void SetupCreatedRecipes()
        {
            Lb_CreatedRecipes.Items.Clear();
            Lb_CreatedRecipes.Items.Refresh();
            foreach (KeyValuePair<string, string> recipe in m_CreatedRecipes)
            {
                if (!Lb_CreatedRecipes.Items.Contains(recipe.Key))
                {
                    Lb_CreatedRecipes.Items.Add(recipe.Key);
                }
            }
        }

        //Sets up the list of cooked recipes.
        private void SetupCookedRecipes()
        {
            Lb_CookedRecipes.Items.Clear();
            Lb_CookedRecipes.Items.Refresh();
            foreach (KeyValuePair<string, string> recipe in m_CookedRecipes)
            {
                if (!Lb_CookedRecipes.Items.Contains(recipe.Key))
                {
                    Lb_CookedRecipes.Items.Add(recipe.Key);
                }
            }
        }

        //Gets the information for the recipe the user wants to preview.
        private void GetRecipePreviewInfo(string recipeName)
        {
            switch (m_CurrentPreviewMode)
            {
                case PreviewModes.Cooked:
                    if (m_CookedRecipes.TryGetValue(recipeName, out string cookedID))
                    {
                        GetRecipePreview(cookedID, recipeName);
                    }
                    break;

                case PreviewModes.Created:
                    if (m_CreatedRecipes.TryGetValue(recipeName, out string createdID))
                    {
                        GetRecipePreview(createdID, recipeName);
                    }
                    break;

                case PreviewModes.Saved:
                    if (m_SavedRecipes.TryGetValue(recipeName, out string savedID))
                    {
                        GetRecipePreview(savedID, recipeName);
                    }
                    break;
            }
        }


        //Handles setting up the preview based on the view mode.
        private void SetupPreview()
        {
            switch (m_CurrentPreviewMode)
            {
                case PreviewModes.Cooked:
                    Tb_prev_name_cooked.Text = m_RecipePreviewName;
                    Tb_prev_author_cooked.Text = m_RecipePreviewAuthor;
                    Tb_prev_desc_cooked.Text = m_RecipePreviewDesc;
                    break;

                case PreviewModes.Created:
                    Tb_prev_name_created.Text = m_RecipePreviewName;
                    Tb_prev_author_created.Text = m_RecipePreviewAuthor;
                    Tb_prev_desc_created.Text = m_RecipePreviewDesc;
                    break;

                case PreviewModes.Saved:
                    Tb_prev_name_saved.Text = m_RecipePreviewName;
                    Tb_prev_author_saved.Text = m_RecipePreviewAuthor;
                    Tb_prev_desc_saved.Text = m_RecipePreviewDesc;
                    break;
            }
        }
        #endregion

        #region Button click functions.
        //Btn click for next recipe.
        private void btn_cmd_ShowNextRecipe(object sender, RoutedEventArgs e)
        {
            NextRecipe();
        }

        //Btn click for previous recipe.
        private void btn_cmd_ShowPrevRecipe(object sender, RoutedEventArgs e)
        {
            PreviousRecipe();
        }

        //Btn click for opening the fridge.
        private void btn_cmd_fridgeopen(object sender, RoutedEventArgs e)
        {
            GetFridge();
        }

        //Selection changed on fridge search result.
        private void FridgeSearchResultSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnFridgeSearchResultSelected();
        }

        //Btn click for adding to fridge.
        private void btn_cmd_addselectiontofridge(object sender, RoutedEventArgs e)
        {
            AddSelectionToFridge();
        }

        //Btn click for opening shopping list.
        private void btn_cmd_openshoppinglist(object sender, RoutedEventArgs e)
        {
            GetShoppingList();
        }

        //Btn click for adding to shopping list.
        private void btn_cmd_addtoshoppinglist(object sender, RoutedEventArgs e)
        {
            AddSelectionToShoppingList();
        }

        //Btn open created recipes.
        private void btn_cmd_opencreatedrecipes(object sender, MouseButtonEventArgs e)
        {
            GetCreatedRecipes();
            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Loading...";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Зареждане...";
                    break;
            }
        }

        //Btn open saved recipes.
        private void btn_cmd_opensavedrecipes(object sender, MouseButtonEventArgs e)
        {
            GetSavedRecipes();
            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Loading...";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Зареждане...";
                    break;
            }
        }

        //Btn open cooked recipes.
        private void btn_cmd_opencookedrecipes(object sender, MouseButtonEventArgs e)
        {
            GetCookedRecipes();
            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Loading...";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Зареждане...";
                    break;
            }
        }

        //Btn click for saving a recipe.
        private void btn_cmd_addtosavedrecipes(object sender, RoutedEventArgs e)
        {
            SaveToSavedRecipes();

            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Recipe Saved!";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Рецептата запазена!";
                    break;
            }
        }

        //Selection changed for list of cooked recipes
        private void Lb_CookedRecipesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lb_CookedRecipes.SelectedItem != null)
            {
                m_CurrentPreviewMode = PreviewModes.Cooked;
                GetRecipePreviewInfo(Lb_CookedRecipes.SelectedItem.ToString());
            }
        }

        //Selection changed for list of saved recipes
        private void Lb_SavedRecipesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lb_SavedRecipes.SelectedItem != null)
            {
                m_CurrentPreviewMode = PreviewModes.Saved;
                GetRecipePreviewInfo(Lb_SavedRecipes.SelectedItem.ToString());
            }
        }

        //Selection changed for list of created recipes
        private void Lb_CreatedRecipesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lb_CreatedRecipes.SelectedItem != null)
            {
                m_CurrentPreviewMode = PreviewModes.Created;
                GetRecipePreviewInfo(Lb_CreatedRecipes.SelectedItem.ToString());
            }
        }

        //Btn click for adding ingredient to recipe.
        private void btn_cmd_addingr(object sender, RoutedEventArgs e)
        {
            AddRecipeIngredient();
        }

        //Btn click for removing ingredient from recipe.
        private void btn_cmd_removeingr(object sender, RoutedEventArgs e)
        {
            RemoveIngredient();
        }

        //Btn click for clearing ingredient list.
        private void btn_cmd_clearingrs(object sender, RoutedEventArgs e)
        {
            ClearIngrs();
        }


        private void Lb_IngrUnitsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_cmd_saverecipetags(object sender, RoutedEventArgs e)
        {
            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Tags saved!";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Тагове запазени!";
                    break;
            }
        }

        private void on_recipe_tag_checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            AddRecipeTag(c.Name.ToString());
        }

        private void btn_cmd_addstep(object sender, RoutedEventArgs e)
        {
            AddNewStep();
            Tb_StepInput.Text = "";
        }

        private void btn_cmd_removestep(object sender, RoutedEventArgs e)
        {
            RemoveStep();
        }

        private void on_recipe_tag_unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            RemoveTag(c.Name.ToString());
        }

        private void btn_cmd_publishrecipe(object sender, RoutedEventArgs e)
        {
            PublishRecipe();
            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Preparing for publishing.";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Приготвяне за публикуване.";
                    break;
            }
        }

        private void btn_cmd_openingrwindow(object sender, RoutedEventArgs e)
        {
            InitRecipeIngrSearch();
        }
        private void on_timetag_checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            AddTimeTag(c.Name.ToString());
        }

        private void on_timetag_unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            RemoveTimeTag(c.Name.ToString());
        }

        private void on_usertag_checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            AddPrefTag(c.Name.ToString());
        }

        private void on_usertag_unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            RemovePrefTag(c.Name.ToString());
        }

        private void btn_cmd_saveusertags(object sender, RoutedEventArgs e)
        {
            SaveUserPrefs();
            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Preferences saved!";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Предпочитания запазени!";
                    break;
            }
        }

        private void btn_cmd_openprofile(object sender, RoutedEventArgs e)
        {
            GetUser();
        }

        private void btn_cmd_addtocookedrecipes(object sender, RoutedEventArgs e)
        {
            SaveToCookedRecipes();

            Storyboard sb1 = this.FindResource("ShowInfo") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    tb_InfoBlock.Text = "Recipe saved to cooked!";
                    break;
                case Languages.Bulgarian:
                    tb_InfoBlock.Text = "Рецептата запазена като сготвена!";
                    break;
            }
        }

        private void OnAppClose(object sender, EventArgs e)
        {
            string ImgFolder;
            string ImgFolder2;
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string localFileName = "tempImgs";
            string localPrevies = "previeImgs";
            ImgFolder = System.IO.Path.Combine(localPath, localFileName);
            ImgFolder2 = System.IO.Path.Combine(localPath, localPrevies);


            if (Directory.Exists(ImgFolder))
            {
                System.IO.DirectoryInfo imgInfo = new System.IO.DirectoryInfo(ImgFolder);

                foreach (System.IO.FileInfo file in imgInfo.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch(Exception ex)
                    {

                    }
                    
                }                

                Directory.Delete(ImgFolder);
            }


            if (Directory.Exists(ImgFolder2))
            {
                System.IO.DirectoryInfo imgInfo2 = new System.IO.DirectoryInfo(ImgFolder2);

                foreach (System.IO.FileInfo file in imgInfo2.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception ex)
                    {

                    }
                }               

                Directory.Delete(ImgFolder2);
            }
        }

        private void btn_cmd_removefromshoppinglist(object sender, RoutedEventArgs e)
        {
            RemoveFromShoppingList();
        }

        private void btn_cmd_removefromfridge(object sender, RoutedEventArgs e)
        {
            RemoveFromFridge();
        }

        private void btn_cmd_logout(object sender, RoutedEventArgs e)
        {
            Storyboard sb1 = this.FindResource("CloseMenuDrawer") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
            m_userID = null;
            m_userName = null;
        }

        private void click_add_img(object sender, MouseButtonEventArgs e)
        {
            OpenImage();
        }

        private void btn_cmd_exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void doubleclick_open_from_saved(object sender, MouseButtonEventArgs e)
        {
            GetFullRecipeInfo(Lb_SavedRecipes.SelectedItem.ToString(), m_SavedRecipes[Lb_SavedRecipes.SelectedItem.ToString()], m_RecipePreviewImg);          
            Storyboard sb1 = this.FindResource("ShowRecipeViewGrid") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
        }

        private void doubleclick_open_from_created(object sender, MouseButtonEventArgs e)
        {
            GetFullRecipeInfo(Lb_CreatedRecipes.SelectedItem.ToString(), m_CreatedRecipes[Lb_CreatedRecipes.SelectedItem.ToString()], m_RecipePreviewImg);
            Storyboard sb1 = this.FindResource("ShowRecipeViewGrid") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
        }

        private void doubleclick_open_from_cooked(object sender, MouseButtonEventArgs e)
        {
            GetFullRecipeInfo(Lb_CookedRecipes.SelectedItem.ToString(), m_CookedRecipes[Lb_CookedRecipes.SelectedItem.ToString()], m_RecipePreviewImg);
            Storyboard sb1 = this.FindResource("ShowRecipeViewGrid") as Storyboard;
            if (sb1 != null) { BeginStoryboard(sb1); }
        }

        private void btn_cmd_cancelpublish(object sender, RoutedEventArgs e)
        {

        }

        private void tb_got_focus(object sender, RoutedEventArgs e)
        {
            TextBox box = ((TextBox)sender);
            box.Text = "";
        }
    }
    #endregion
}
