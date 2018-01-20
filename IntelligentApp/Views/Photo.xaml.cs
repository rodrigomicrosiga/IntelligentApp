﻿using Xamarin.Forms;

namespace IntelligentApp.Views
{
    public partial class Photo : ContentPage
    {
        public Photo()
        {
            InitializeComponent();
            Extentions.IPhoneX(this);
        }

        protected override void OnAppearing()
        {
            if (this.BindingContext is ViewModels.ViewModel viewModel)
                viewModel.OnInitialize();
        }
    }
}