using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Globalization;
using thestory.DataModel;
using System.Linq.Expressions;

namespace thestory.Data
{
    class DataService
    {
        private static DataService instance;

        // Story items (both local and remote)
        private IMobileServiceSyncTable<StoryItem> storyTable;
        private IMobileServiceSyncTable<Choice> choiceTable;

        public bool IsInitialized { get; set; }
        public string LocalCulture { get; set; }

        public static DataService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataService();
                }
                return instance;
            }
        }

        private DataService()
        {
            storyTable = App.MobileService.GetSyncTable<StoryItem>();
            choiceTable = App.MobileService.GetSyncTable<Choice>();
        }

        public async Task Initialize()
        {
            if (!IsInitialized)
            {
                await InitLocalStoreAsync();
                LocalCulture = CultureInfo.CurrentUICulture.Name.Substring(0, 2);
                IsInitialized = true;
            }
        }

        public async Task<StoryItem> GetStory(string id, bool isRoot = false)
        {
            StoryItem result = null;
            Expression<Func<StoryItem, bool>> predicate = (StoryItem s) => s.Id == id;

            if (isRoot)
            {
                predicate = (StoryItem s) => s.IsRoot == isRoot && s.Language == LocalCulture;
            }

            var stories = await storyTable
                .Where(predicate)
                .ToListAsync();
            if (stories.Count > 0)
            {
                result = stories[0];
                var choices = await choiceTable.Where(c => c.AncestorId == result.Id).ToListAsync();
                result.Choices = choices;
            }

            return result;
        }

        private async void Vote(string id, bool up)
        {
            var param = new Dictionary<string, string>();
            param.Add("id", id);

            if (up)
            {
                param.Add("vote", "up");
            }
            else
            {
                param.Add("vote", "down");
            }

            try
            {
                // Asynchronously call the custom API using the POST method. 
                await App.MobileService
                    .InvokeApiAsync("vote",
                    System.Net.Http.HttpMethod.Post, param);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.ToString());
            }
        }

        #region Offline Sync

        private async Task InitLocalStoreAsync()
        {
            if (!App.MobileService.SyncContext.IsInitialized)
            {
                var store = new MobileServiceSQLiteStore("localstore.db");
                store.DefineTable<StoryItem>();
                store.DefineTable<Choice>();
                await App.MobileService.SyncContext.InitializeAsync(store);
            }

            await SyncAsync();
        }

        private async Task SyncAsync()
        {
            String errorString = null;

            try
            {
                await App.MobileService.SyncContext.PushAsync();
                await storyTable.PullAsync("storyItem", storyTable.CreateQuery());
                await storyTable.PullAsync("choices", choiceTable.CreateQuery());
            }

            catch (MobileServicePushFailedException ex)
            {
                errorString = "Push failed because of sync errors: " +
                  ex.PushResult.Errors.Count + " errors, message: " + ex.Message;
            }
            catch (Exception ex)
            {
                errorString = "Pull failed: " + ex.Message +
                  "\n\nIf you are still in an offline scenario, " +
                  "you can try your Pull again when connected with your Mobile Serice.";
            }

            if (errorString != null)
            {
                MessageDialog d = new MessageDialog(errorString);
                await d.ShowAsync();
            }
        }

        #endregion
    }
}
