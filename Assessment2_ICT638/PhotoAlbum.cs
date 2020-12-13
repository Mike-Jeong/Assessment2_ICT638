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

namespace Assessment2_Ict638
{
    class PhotoAlbum
    {
        static int[] listPhoto =
            {
            Resource.Drawable.P1,
            Resource.Drawable.P2,
            Resource.Drawable.P3,
            Resource.Drawable.P4,
            Resource.Drawable.P5,
            Resource.Drawable.P6,
            Resource.Drawable.P7,
            Resource.Drawable.P8,
            Resource.Drawable.P9,
            Resource.Drawable.P10,
        };

        private int[] photos;
        public PhotoAlbum()
        {
            this.photos = listPhoto;
        }

        public int numPhoto
        {
            get
            {
                return photos.Length;
            }
        }
        public int this[int i]
        {
            get { return photos[i]; }
        }
    }
}