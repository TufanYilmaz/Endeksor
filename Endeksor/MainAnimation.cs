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
using Android.Views.Animations;

namespace App4
{
    class MainAnimation : Animation
    {
        private View vView;
        private int iOriginalHeight;
        private int iTargetHeight;
        private int iGrowBy;

        public int ITargetHeight { get => iTargetHeight; set => iTargetHeight = value; }
        public int IGrowBy { get => iGrowBy; set => iGrowBy = value; }
        public int IOriginalHeight { get => iOriginalHeight; set => iOriginalHeight = value; }

        public MainAnimation(View view,int targetHeight)
        {
            vView = view;
            IOriginalHeight = view.Height;
            ITargetHeight = targetHeight;
            IGrowBy = ITargetHeight - IOriginalHeight;
        }
        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            //base.ApplyTransformation(interpolatedTime, t);
            vView.LayoutParameters.Height = (int)(IOriginalHeight + (IGrowBy * interpolatedTime));
            vView.RequestLayout();
        }
        public override bool WillChangeBounds()
        {
            //return base.WillChangeBounds();
            return true;
        }
    }
}