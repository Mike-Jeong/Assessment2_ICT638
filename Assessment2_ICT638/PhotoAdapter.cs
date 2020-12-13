using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.Widget;

namespace Assessment2_Ict638
{
    public class Data
    {
        public string heading { get; set; }
        public string description { get; set; }
        public string numberofroom { get; set; }
        public string numberoftoilet { get; set; }
        public string rentfee { get; set; }
        public string location { get; set; }
        public string agencyname { get; set; }

    }

    class PhotoAdapter : RecyclerView.Adapter
    {
        List<Data> dataList = new List<Data>();
        public event EventHandler<int> ItemClick;

        public PhotoAlbum mPhotoAlbum;
        public PhotoAdapter(PhotoAlbum photoAlbum, List<Data> list)
        {
            mPhotoAlbum = photoAlbum;
            dataList = list;
        }

        public override int ItemCount
        {
            get { return dataList.Count(); }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PhotoViewHolder vh = holder as PhotoViewHolder;
            vh.image.SetImageResource(mPhotoAlbum[position]);
            vh.heading.Text = dataList[position].heading;
            vh.description.Text = string.Format(dataList[position].numberofroom + ", " + dataList[position].numberoftoilet);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.content_recycler, parent, false);
            PhotoViewHolder vh = new PhotoViewHolder(itemView, OnClick);
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}