using Android.App;
using Android.Widget;
using Android.OS;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Android.Content;

namespace AzureStorageSample
{
    [Activity (Label = "AzureStorageSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        public static readonly string PHOTO_NAME = "PHOTO_NAME";

        IList<string> _items = new List<string>();

        BlobFilesAdapter _itemsAdapter;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button> (Resource.Id.myButton);

            var listView = FindViewById<ListView> (Resource.Id.storageListView);

            _itemsAdapter = new BlobFilesAdapter (this, _items);

            listView.Adapter = _itemsAdapter;

            listView.ItemClick += (sender, e) => {

                var intent = new Intent (this, typeof (PhotoActivity));

                intent.PutExtra (PHOTO_NAME, _itemsAdapter [e.Position]);

                StartActivity (intent);
            };

            button.Click += async (sender, e) => {
                _items = await GetBlobInContainer (Settings.ImageContainer);
                _itemsAdapter.ReplaceItems (_items);
                _itemsAdapter.NotifyDataSetChanged ();
            };
        }


        async Task<string[]> GetBlobInContainer (string containerName)
        {
            BlobContinuationToken continuationToken = null;

            var fileName = new List<string> ();

            var storageInstance = CloudStorageAccount.Parse (Settings.AzureKey);

            var blogClient = storageInstance.CreateCloudBlobClient ();

            var container = blogClient.GetContainerReference (containerName);

            do
            {
                var files = await container.ListBlobsSegmentedAsync (continuationToken);

                var fileUris = files.Results.Select (a => a.Uri);

                continuationToken = files.ContinuationToken;

                foreach (var item in fileUris)
                {
                    var name = Path.GetFileName (item.AbsolutePath);

                    fileName.Add (name);
                }

            } while (continuationToken != null);


            return fileName.ToArray();
        }

    }
}

