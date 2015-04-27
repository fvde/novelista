using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using thestory;
using thestory.Data;
using thestory.DataModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace thestory
{

    sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await DataService.Instance.Initialize();
            await Start("");
        }

        private async Task Start(string id)
        {
            Exception exception = null;
            StoryItem story = null;
            try
            {
                // get next story
                if (String.IsNullOrEmpty(id))
                {
                    story = await DataService.Instance.GetStory(null, true);
                } else {
                    story = await DataService.Instance.GetStory(id);
                }
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                Text.Text = story.Text;
                Choices.ItemsSource = story.Choices;
            }
        }
    }
}
