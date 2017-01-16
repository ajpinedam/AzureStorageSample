using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

namespace AzureStorageSample
{
    public class BlobFilesAdapter : BaseAdapter<string>
    {
        readonly IList<string> _items;
        readonly Activity _context;

        public BlobFilesAdapter (Activity context) : this(context, new List<string>())
        {
        }

        public BlobFilesAdapter (Activity context, IList<string> items)
        {
            _context = context;

            _items = items;
        }

        public override string this [int position]
        {
            get
            {
                return _items [position];
            }
        }

        public override int Count
        {
            get
            {
                return _items.Count ();
            }
        }

        public void AddItems (IList<string> items)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                _items.Add (item);
            }
        }

        public void ReplaceItems (IList<string> items)
        {
            _items.Clear ();

            if (items == null)
                return;

            foreach (var item in items)
            {
                _items.Add (item);
            }
        }

        public override long GetItemId (int position)
        {
            return position;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = _context.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem1, null);
            }

            view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = _items [position];

            return view;
        }
    }
}
