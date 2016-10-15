using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Deploygate.Sdk;

namespace Amesh.Droid
{
    [Application]
    public class AmeshApplication : Application
    {
        protected AmeshApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //DeployGate.Install(this);
            //DeployGate.LogDebug("log message");
        }
    }
}