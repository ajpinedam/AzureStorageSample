using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Microsoft.WindowsAzure.Storage;
using Android.Graphics;

namespace AzureStorageSample
{
    [Activity (Label = "PhotoActivity")]
    public class PhotoActivity : Activity
    {
        ImageView _azureImage;

        string _photoName;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.Photo);

            //Pude haber pasado el URI directamente pero con esta manera 
            //muestro como hacerlo teniendo solo el mombre
            _photoName = Intent.GetStringExtra (MainActivity.PHOTO_NAME);

            _azureImage = FindViewById<ImageView> (Resource.Id.azureImageView);
        }

        protected override async void OnResume ()
        {
            base.OnResume ();

            await LoadPhotoFromAzureContainer (Settings.ImageContainer);
        }


        async Task LoadPhotoFromAzureContainer (string containerName)
        {

            if (string.IsNullOrEmpty (_photoName))
            {
                //Notificar el usuario ??
                return;
            }

            var storageInstance = CloudStorageAccount.Parse (Settings.AzureKey);

            var blogClient = storageInstance.CreateCloudBlobClient ();

            var container = blogClient.GetContainerReference (containerName);

            var blob = container.GetBlobReference (_photoName);

            if (await blob.ExistsAsync ())
            {
                await blob.FetchAttributesAsync ();

                var size = blob.Properties.Length;

                var fileBytes = new byte [size];

                var bytesDownloaded = await blob.DownloadToByteArrayAsync (fileBytes, 0);

                if (bytesDownloaded == size)
                {
                    System.Console.WriteLine ("Blob se descargo satisfactoriamente");

                    var bitmap = BitmapFactory.DecodeByteArray (fileBytes, 0, fileBytes.Length);

                    _azureImage.SetImageBitmap (bitmap);
                }
                else
                {
                    var dialog = new AlertDialog.Builder (this)
                                                .SetMessage (Resource.String.image_download_error_dialog_msg)
                                                .SetPositiveButton (Resource.String.image_download_error_dialog_ok, (sender, e) => {})
                                                .SetTitle (Resource.String.image_download_error_dialog_title);

                    dialog.Show ();
                }

            }

        }

    }
}
