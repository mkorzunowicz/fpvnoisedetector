﻿using System.Net.Http;
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

        private string _newVersion;
        /// <summary>
        /// Holds the update message
        /// </summary>
        public string NewVersion
        {
            get { return _newVersion; }

            set => SetProperty(ref _newVersion, value);
        }

        private string _changelog;
        /// <summary>
        /// Holds the changelog message
        /// </summary>
        public string Changelog
        {
            get { return _changelog; }

            set => SetProperty(ref _changelog, value);
        }
        private string _oldVersion;
        /// <summary>
        /// Holds the update message
        /// </summary>
        public string OldVersion
        {
            get { return _oldVersion; }

            set => SetProperty(ref _oldVersion, value);
        }
        /// <summary>
        /// Gets Markdown Style
        /// </summary>
        public Style MdStyle
        {
            get
            {
                return MdXaml.MarkdownStyle.Sasabune;
            }
        }
        /// <summary>
        /// Update dialog VM constructor
        /// </summary>
        public UpdateViewModel()
        {
            NewVersion = "15.10.1.0";
            OldVersion = "1.0.1.0";
            Changelog = "\r\n\t\t# sample title\r\n\t\t* document1\r\n\t\t\t* two\r\n\t\t\t* three\r\n\t\t* document2";

        }
        /// <summary>
        /// Update dialog VM constructor
        /// </summary>
        public UpdateViewModel(UpdateInfoEventArgs args, string changelog)
        {
            NewVersion = args.CurrentVersion;
            OldVersion = args.InstalledVersion.ToString();
            Changelog = changelog;
        }
    }
}