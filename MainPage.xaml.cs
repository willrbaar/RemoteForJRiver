using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RemoteForJRiver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        //public string AddressBarVisibility { get; set; }


        public ChangingVisiblity MyVisibilityControl { get; set; }

        public MainPage()

        {
            this.InitializeComponent();

            //WebSocketForRemote RemoteSocket = new WebSocketForRemote();
            WebSocketForRemote RemoteWebsocket = new WebSocketForRemote();
            var Newtask = Task.Run(() => RemoteWebsocket.WebSocketServerStart());
            //RemoteWebsocket.WebSocketServerStart();
            //Window.Current.CoreWindow.KeyUp += LookingForEscapeKey_Event;
            //Window.Current.CoreWindow.KeyUp += WebViewLookingForEscapeKey;
            this.MyVisibilityControl = new ChangingVisiblity();
            //MyVisibilityControl.AppBarVisiblity("Collapsed");
            //ChangingVisiblity.AppBarVisiblity("Visible");
            Debug.WriteLine("Value of visibility at initialization is:  " + MyVisibilityControl.VisibiltyState);

            //VisualStateManager.GoToState(this, "NavigationView", false);

            //MyHeaderFrame.Navigate(typeof(Page1));
            //AddressBarVisibility = "Visible";


            // This is a static public property that allows downstream pages to get a handle to the MainPage instance
            // in order to call methods that are in this class.
            //Current = this;
            //SampleTitle.Text = FEATURE_NAME;

        }



        static string UriToString(Uri uri)
        {
            return (uri != null) ? uri.ToString() : "";
        }

        /// <summary>
        /// This is the click handler for the "Go" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!pageIsLoading)
            {
                NavigateWebview(AddressBox.Text);
            }
            else
            {
                WebViewControl.Stop();
                pageIsLoading = false;
            }
        }
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);

        //    var _parameters = (HandlerDirections)e.Parameter;
        //    if (_parameters.AddressSendMethod == true)
        //    {

        //    }
        //    if (_parameters.AddressSendMethod == false)
        //    {

        //    }
        //}


        //private void GoButton_Helper(string Address)
        //{
        //    if (!pageIsLoading)
        //    {
        //        NavigateWebview(Address);
        //    }
        //    else
        //    {
        //        WebViewControl.Stop();
        //        pageIsLoading = false;
        //    }

        //}
        /// <summary>
        /// Property to control the "Go" button text, forward/backward buttons and progress ring.
        /// </summary>
        public bool _pageIsLoading;
        bool pageIsLoading
        {
            get { return _pageIsLoading; }
            set
            {
                _pageIsLoading = value;
                GoButton.Content = (value ? "Stop" : "Go");
                //ProgressControl.IsActive = value;

                if (!value)
                {
                    //NavigateBackButton.IsEnabled = WebViewControl.CanGoBack;
                    //NavigateForwardButton.IsEnabled = WebViewControl.CanGoForward;
                }
            }
        }

        /// <summary>
        /// Handler for the NavigateBackward button
        /// </summary>
        /// <param name = "sender" ></ param >
        /// < param name="e"></param>
        public void NavigateBackward_Click(object sender, RoutedEventArgs e)
        {
            if (WebViewControl.CanGoBack) WebViewControl.GoBack();
        }

        ///// <summary>
        ///// Handler for the GoForward button
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        public void NavigateForward_Click(object sender, RoutedEventArgs e)
        {
            if (WebViewControl.CanGoForward) WebViewControl.GoForward();
        }

        ///// <summary>
        ///// This handles the enter key in the url address box
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        void Address_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {

                NavigateWebview(AddressBox.Text);
            }
        }

        /// <summary>
        /// Helper to perform the navigation in webview
        /// </summary>
        /// <param name="url"></param>
        public void NavigateWebview(string url)
        {
            //try
            //{
            Uri targetUri = new Uri(url);
            WebViewControl.Navigate(targetUri);
            //}
            //catch (UriFormatException ex)
            //{
            //    // Bad address
            //    //AppendLog($"Address is invalid, try again. Error: {ex.Message}.");
            //}
        }

        /// <summary>
        /// Handle the event that indicates that WebView is starting a navigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void WebViewControl_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string url = UriToString(args.Uri);
            AddressBox.Text = url;
            //AppendLog($"Starting navigation to: \"{url}\".");
            pageIsLoading = true;
        }

        /// <summary>
        /// Handle the event that indicates that the WebView content is not a web page.
        /// For example, it may be a file download.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void WebViewControl_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            //AppendLog($"Content for \"{UriToString(args.Uri)}\" cannot be loaded into webview.");
            // We throw away the request. See the "Unviewable content" scenario for other
            // ways of handling the event.
            pageIsLoading = false;
        }

        /// <summary>
        /// Handle the event that indicates that WebView has resolved the URI, and that it is loading HTML content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void WebViewControl_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            //AppendLog($"Loading content for \"{UriToString(args.Uri)}\".");
        }
        /// <summary>
        /// Handle the event that indicates that the WebView content is fully loaded.
        /// If you need to invoke script, it is best to wait for this event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void WebViewControl_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            //AppendLog($"Content for \"{UriToString(args.Uri)}\" has finished loading.");
        }

        /// <summary>
        /// Event to indicate webview has completed the navigation, either with success or failure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        void WebViewControl_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            pageIsLoading = false;
            if (args.IsSuccess)
            {
                //AppendLog($"Navigation to \"{UriToString(args.Uri)}\" completed successfully.");
            }
            else
            {
                //AppendLog($"Navigation to: \"{UriToString(args.Uri)}\" failed with error {args.WebErrorStatus}.");
            }
        }
        
        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            FullScreenButtonController();
            Debug.WriteLine("Full Screen Command Executed ");

        }

        private bool _fullSreenState = false;

        public bool FullScreenState
        {
            get { return _fullSreenState; }
            set { _fullSreenState = value; }
        }
        private void FullScreenButtonController()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            Debug.WriteLine("Full Screen Controller Entered ");
            if (FullScreenState == false)
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                MyVisibilityControl.VisibiltyState = Visibility.Collapsed;
                Debug.WriteLine("Full Screen Controller calls for collapse, fullscreen parm should be false and is: " + FullScreenState);
                _fullSreenState = true;

            }

            else
            {
                applicationView.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                MyVisibilityControl.VisibiltyState = Visibility.Visible;
                Debug.WriteLine("Full Screen Controller calls for visible, fullscreen parm should be true and is: " + FullScreenState);
                _fullSreenState = false;
            }

        }

        private void SecondFakeElement_Click(object sender, RoutedEventArgs e)
        {
            FullScreenButtonController();
            Debug.WriteLine("Full Screen Command Executed by Second Fake Element");
        }
    }




}
