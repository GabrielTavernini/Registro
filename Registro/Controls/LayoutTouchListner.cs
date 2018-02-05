using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Registro.Controls
{
    public class LayoutTouchListner : StackLayout
    {
        public bool IsEnebleScroll
        {
            get { return (bool)base.GetValue(IsEnebleScrollProperty); }
            set { base.SetValue(IsEnebleScrollProperty, value); }
        }

        public static readonly BindableProperty IsEnebleScrollProperty =
            BindableProperty.Create("IsEnebleScroll", typeof(bool),
                typeof(LayoutTouchListner), false);
        
        public float? YTouch { get; set; }

        public event EventHandler TouchEvent;
        public void DoTouchEvent(float? ytouch)
        {
            System.Diagnostics.Debug.WriteLine("DoTouchEvent");
            YTouch = ytouch;
            EventHandler eh = TouchEvent;
            if (eh != null)
                eh(this, new EvArg(ytouch));
        }
    }

    public class EvArg : EventArgs
    {
        public EvArg(float? par)
        {
            this.Val = par;
        }
        public float? Val { get; set; }
    }
}
