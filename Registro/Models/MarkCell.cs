using System;
using Xamarin.Forms;

namespace Registro.Models
{
    public class MarkCell : ViewCell
    {
        public MarkCell()
        {


            //-----------------Bindings-----------------

            this.SetBinding(ItemIdProperty, nameof(MenuOption.Id));

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ItemId == 1)
            {
                DoFirstApper();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ItemId == 1)
            {
                DoFirstDisapp();
            }
        }

        public event EventHandler FirstDisapp;
        public void DoFirstDisapp()
        {
            EventHandler eh = FirstDisapp;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }


        public event EventHandler FirstApper;
        public void DoFirstApper()
        {
            EventHandler eh = FirstApper;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }


        public static readonly BindableProperty ItemIdProperty =
            BindableProperty.Create("ItemId", typeof(Int32),
                                    typeof(MarkCell), 0, BindingMode.TwoWay, null, null);

        public int ItemId
        {
            get { return (Int32)GetValue(ItemIdProperty); }
            set { SetValue(ItemIdProperty, value); }
        }
    }
}
