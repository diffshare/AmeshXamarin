using System;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;
using Com.Deploygate.Sdk;
using Java.Net;
using ModernHttpClient;

namespace Amesh.Droid
{
    [Activity(Label = "アメッシュ", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var image1 = FindViewById<ImageView>(Resource.Id.image1);
            Glide.With(this).Load("http://tokyo-ame.jwa.or.jp/map/map000.jpg").Into(image1);

            var image3 = FindViewById<ImageView>(Resource.Id.image3);
            Glide.With(this).Load("http://tokyo-ame.jwa.or.jp/map/msk000.png").Into(image3);

            caches = new LruCache(100);
            Caching();

            Update(Now());
        }

        private async void Caching()
        {
            var now = Now();
            try
            {
                for (int i = 0; i <= 120; i += 5)
                {
                    var url = Url(now.AddMinutes(-i));
                    var httpClient = new HttpClient(new NativeMessageHandler());
                    var byteArray = await httpClient.GetByteArrayAsync(url);
                    var bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
                    caches.Put(url, bitmap);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e);
            }
            Console.WriteLine("Cache End");
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var point = new Point();
            WindowManager.DefaultDisplay.GetSize(point);
            var x = e.GetX() / point.X;
            //Console.WriteLine(x);
            var now = Now().AddMinutes((x - 1) * 120);
            var remainder = now.Minute % 5;
            now = now.AddMinutes(-remainder);
            Update(now);
            return base.OnTouchEvent(e);
        }

        private async void Update(DateTime current)
        {
            try
            {
                var url = Url(current);
                Bitmap bitmap = (Bitmap)caches.Get(url);

                if (bitmap == null)
                {
                    var httpClient = new HttpClient(new NativeMessageHandler());
                    var byteArray = await httpClient.GetByteArrayAsync(url);
                    bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
                    caches.Put(url, bitmap);
                }

                var image2 = FindViewById<ImageView>(Resource.Id.image2);
                image2.SetImageBitmap(bitmap);
                Title = current.ToString("東京アメッシュ yyyy年MM月dd日HH時mm分");
            }
            catch (UnknownHostException e)
            {
                new AlertDialog.Builder(this).SetTitle("エラー").SetMessage(e.Message).SetPositiveButton("OK",
                    (sender, args) =>
                    {
                    }).Show();
            }
        }

        private string Url(DateTime current)
        {
            return string.Format("http://tokyo-ame.jwa.or.jp/mesh/000/{0:yyyyMMddHHmm}.gif", current);
        }

        private DateTime Now()
        {
            var now = DateTime.Now;
            now = now.AddMinutes(-1);
            var remainder = now.Minute % 5;
            now = now.AddMinutes(-remainder);
            return now;
        }

        private LruCache caches;
    }
}


