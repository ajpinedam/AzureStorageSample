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
using Microsoft.WindowsAzure.Storage.File;

namespace AzureStorageSample
{
    [Activity (Label = "AzureStorageSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        public static readonly string PHOTO_NAME = "PHOTO_NAME";

        BlobFilesAdapter _blobItemsAdapter;

        FileSharedAdapter _fileItemsAdapter;

        bool isPhoto;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var getImagesButton = FindViewById<Button> (Resource.Id.imageButton);

            var getFilesButton = FindViewById<Button> (Resource.Id.fileButton);

            var listView = FindViewById<ListView> (Resource.Id.storageListView);

            listView.ItemClick += (sender, e) => {

                if (isPhoto)
                {
                    var intent = new Intent (this, typeof (PhotoActivity));

                    intent.PutExtra (PHOTO_NAME, _blobItemsAdapter [e.Position]);

                    StartActivity (intent);
                }
            };

            getImagesButton.Click += async (sender, e) => {
                isPhoto = true;
                var images = await GetBlobInContainer (Settings.ImageContainer);

                _blobItemsAdapter= new BlobFilesAdapter (this, images);
                listView.Adapter = _blobItemsAdapter;
            };

            getFilesButton.Click += async (sender, e) => { 
                isPhoto = false;
                var files = await GetResourceFiles (Settings.FileStorageName);

                listView.Adapter = _fileItemsAdapter = new FileSharedAdapter (this, files);                
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

        /// <summary>
        /// Gets files name in an Azure shared file storage
        /// </summary>
        /// <param name="fileStorageName">File storage name. Como fue llamado en Azure</param>
        /// <returns>An array with the files name</returns>
        async Task<string []> GetResourceFiles (string fileStorageName)
        {
            FileContinuationToken continuationToken = null;

            var fileName = new List<string> ();

            var storageInstance = CloudStorageAccount.Parse (Settings.AzureKey);

            var fileClient = storageInstance.CreateCloudFileClient ();

            var share = fileClient.GetShareReference (fileStorageName);


            if (!await share.ExistsAsync ())
            {
                return fileName.ToArray();
            }

            var root = share.GetRootDirectoryReference ();

            var myFiles = root.GetDirectoryReference ("MyFiles");

            if (!await myFiles.ExistsAsync ())
            {
                return fileName.ToArray ();
            }

            do
            {

                var files = await myFiles.ListFilesAndDirectoriesSegmentedAsync (continuationToken);

                var uris = files.Results.Select (a => a.Uri);

                continuationToken = files.ContinuationToken;

                foreach (var item in uris)
                {
                    var name = Path.GetFileName (item.AbsolutePath);

                    fileName.Add (name);
                }


            } while (continuationToken != null);


            return fileName.ToArray ();
        }

    }
}

