﻿using IntelligentApp.CognitiveServices;
using IntelligentApp.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IntelligentApp.ViewModels
{
    public class Photo : ViewModel
    {
        private MediaFile _mediaFile;

        private string _image;
        public string Image
        {
            get => _image;
            set => this.SetValue(value, ref _image);
        }

        private bool _canAnalyze = false;
        public bool CanAnalyze
        {
            get => _canAnalyze;
            set => this.SetValue(value, ref _canAnalyze);
        }

        public Command TakePhotoCommand { get; set; }
        public Command PickPhotoCommand { get; set; }
        public Command AnalyzeCommand { get; set; }

        private IService _service;
        private CognitiveService _cognitiveService;

        public Photo()
        {
            this.TakePhotoCommand = new Command(async () => await this.TakePhoto());
            this.PickPhotoCommand = new Command(async () => await this.PickPhoto());
            this.AnalyzeCommand = new Command(async () => await this.Analyze());
        }

        public override void OnInitialize()
        {
            this._cognitiveService = this.Parameters["CognitiveService"] as CognitiveService;
            this._service = Activator.CreateInstance(this._cognitiveService.Type) as IService;
            this.Title = this._cognitiveService.Name;
        }

        private async Task TakePhoto()
            => await Execute(() => CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = this.Title,
                Name = Guid.NewGuid().ToString()
            }));

        private async Task PickPhoto()
            => await Execute(() => CrossMedia.Current.PickPhotoAsync());

        private async Task Execute(Func<Task<MediaFile>> func)
        {
            try
            {
                this.CanAnalyze = false;
                this._mediaFile = await func();
                this.Image = this._mediaFile.Path;
                this.CanAnalyze = true;
            }
            catch
            {
                await this.Message.DisplayAlert(this.Title, "Não foi possível obter a foto", "Ok");
                this.IsBusy = false;
            }
        }

        private async Task Analyze()
        {
            var args = new object[]
            {
                "Media", this._mediaFile,
                "Service", this._service,
                "ServiceName", this._cognitiveService.Name
            };
            await this.Navigation.To<Analyze>(new Parameters(args));
        }
    }
}