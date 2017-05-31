using System.ComponentModel;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RemoteForJRiver


{
    public class ChangingVisiblity : INotifyPropertyChanged

    {
        public Windows.UI.Xaml.Visibility VisibleParameter = Windows.UI.Xaml.Visibility.Visible;

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;
       

        public  void AppBarVisiblity()
        {
        }

        public void AppBarVisiblity(string value)
        {
            Debug.WriteLine("AppBarVisibility entered");
            if (value == "Visible")
            {
          
                this.VisibleParameter = Windows.UI.Xaml.Visibility.Visible;
            }
            if (value == "Collapsed")
            {
                this.VisibleParameter = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }


        public Windows.UI.Xaml.Visibility VisibiltyState
        {
            get { return this.VisibleParameter; }
            set
            {
                this.VisibleParameter=value;
                // Call OnPropertyChanged whenever the property is updated
                // OnPropertyChanged("");
               
                OnPropertyChanged(null);
                //Debug.WriteLine("Property Change called.");

            }
        } 

        protected void OnPropertyChanged( string propertyName )
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //Debug.WriteLine("Property Change was invoked.");
        }
    }


    


}
            
  
