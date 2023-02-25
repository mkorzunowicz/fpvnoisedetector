using System.Windows;
using AutoUpdaterDotNET;
using FPVNoiseDetector.Foundation;
using Unosquare.FFME.Common;

namespace FPVNoiseDetector.ViewModels
{
    /// <summary>
    /// View model for the update window
    /// </summary>
    public class UpdateViewModel : ViewModelBase
    {

        private DelegateCommand updateCommand;
        private DelegateCommand cancelCommand;

        /// <summary>
        /// Start update command
        /// </summary>
        public DelegateCommand UpdateCommand => updateCommand ??
            (updateCommand = new DelegateCommand(a =>
            {
                if (a is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }));

        /// <summary>
        /// Cancel the update command
        /// </summary>
        public DelegateCommand CancelCommand => cancelCommand ??
            (cancelCommand = new DelegateCommand(a =>
            {
                if (a is Window window)
                {
                    window.DialogResult = false;
                    window.Close();
                }
            }));
        private string _updateMessage;

        /// <summary>
        /// Holds the update message
        /// </summary>
        public string UpdateMessage
        {
            get { return _updateMessage; }

            set => SetProperty(ref _updateMessage, value);
        }

        /// <summary>
        /// Update dialog VM constructor
        /// </summary>
        public UpdateViewModel(UpdateInfoEventArgs args)
        {
            // Set the update message
            UpdateMessage = $"A new version of the application ({args.CurrentVersion} to {args.InstalledVersion}) is available. Do you want to download and install it now?";
        }
    }
}