using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using Microsoft.WindowsAzure.Storage;

namespace AzureStorageSample
{
    [Activity (Label = "FileActivity")]
    public class FileActivity : Activity
    {
        string _fileName;

        string _directory;

        TextView _fileEditText;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.FIle);

            _fileEditText = FindViewById<TextView> (Resource.Id.azureFileTextView);

            _fileName = Intent.GetStringExtra (MainActivity.FILE_NAME);

            _directory = Intent.GetStringExtra (MainActivity.DIRECTORY_NAME);

        }


        protected override async void OnResume ()
        {
            base.OnResume ();

            await GetResourceFiles (Settings.FileStorageName, _directory, _fileName);
        }

        /// <summary>
        /// Gets files name in an Azure shared file storage
        /// </summary>
        /// <param name="fileStorageName">File storage name. Como fue llamado en Azure</param>
        /// <returns>An array with the files name</returns>
        async Task GetResourceFiles (string fileStorageName, string directory, string fileName)
        {
            var storageInstance = CloudStorageAccount.Parse (Settings.AzureKey);

            var fileClient = storageInstance.CreateCloudFileClient ();

            var share = fileClient.GetShareReference (fileStorageName);

            if (!await share.ExistsAsync ())
            {
                return;
            }

            var root = share.GetRootDirectoryReference ();

            var myFiles = root.GetDirectoryReference (directory);

            if (!await myFiles.ExistsAsync ())
            {
                return;
            }

            var file = myFiles.GetFileReference (fileName);


            if (!await file.ExistsAsync ())
            {
                return;
            }

            var text = await file.DownloadTextAsync ();

            _fileEditText.Text = text;
        }
    }
}
