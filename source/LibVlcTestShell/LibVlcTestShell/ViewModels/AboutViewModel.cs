using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LibVlcTestShell.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {

        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            RestartWebCamCommand = new Command(async () => await RestartWebCamAction());
        }

        private bool _isStartup = true;

        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public bool IsStartup
        {
            get => _isStartup;
            private set => SetProperty(ref _isStartup, value, nameof(IsStartup));
        }
        private LibVLC _libVLC;

        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => _libVLC;
            private set => SetProperty(ref _libVLC, value, nameof(LibVLC));
        }

        private MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => SetProperty(ref _mediaPlayer, value, nameof(MediaPlayer));
        }


        private bool _doneLoading = false;
        public bool DoneLoading
        {
            get => _doneLoading;
            private set => SetProperty(ref _doneLoading, value, nameof(DoneLoading));
        }

        async Task RefreshWebCamViewAction()
        {
            string uri = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";

            List<string> options = new()
            {
                string.Format("--file-caching={0}", 150)
            };

            using LibVLC libvlc = new(options.ToArray());
            using Media Media = new(libvlc, uri, FromType.FromLocation);

            MediaParsedStatus result = await Media.Parse(MediaParseOptions.ParseNetwork);

            MediaPlayer = new MediaPlayer(Media)
            {
                EnableHardwareDecoding = true,
                NetworkCaching = Convert.ToUInt32(150),
                FileCaching = Convert.ToUInt32(150),
            };
        }

        void StopWebCamViewAction()
        {
            MediaPlayer?.Stop();
        }

        void StartWebCamViewAction()
        {
            _ = MediaPlayer?.Play();
        }

        public ICommand OpenWebCommand { get; }

        public ICommand RestartWebCamCommand { get; set; }
        async Task RestartWebCamAction()
        {
            StopWebCamViewAction();
            await Task.Delay(10);
            await RefreshWebCamViewAction();
            await Task.Delay(100);
            StartWebCamViewAction();
        }

        public async Task OnAppearing()
        {
            try
            {
                if (IsStartup)
                {
                    DoneLoading = false;
                    //Core.Initialize();
                    await RefreshWebCamViewAction();
                    await Task.Delay(100);
                    IsStartup = false;
                }
                StartWebCamViewAction();

                DoneLoading = true;
            }
            catch (Exception exc)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Exception",
                    $"Message: {exc.Message}\nStackTrace: {exc.StackTrace}",
                    "OK"
                    );
            }
        }

        internal void OnDisappearing()
        {
            try
            {
                StopWebCamViewAction();
                // If enabled, the app crashes on the next appearing at line 56 above
                //MediaPlayer.Dispose();
                //LibVLC.Dispose();
            }
            catch (Exception exc)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Exception",
                    $"Message: {exc.Message}\nStackTrace: {exc.StackTrace}",
                    "OK"
                    );
            }
        }
    }
}