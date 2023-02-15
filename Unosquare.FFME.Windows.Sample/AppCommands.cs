namespace Unosquare.FFME.Windows.Sample
{
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Unosquare.FFME.Windows.Sample.Controls;

    /// <summary>
    /// Represents the Application-Wide Commands.
    /// </summary>
    public class AppCommands
    {
        #region Private State

        private readonly WindowStatus PreviousWindowStatus = new WindowStatus();

        private DelegateCommand m_OpenCommand;
        private DelegateCommand m_PauseCommand;
        private DelegateCommand m_PlayCommand;
        private DelegateCommand m_StopCommand;
        private DelegateCommand m_PredictNoiseInWholeVideoCommand;
        private DelegateCommand m_CloseCommand;
        private DelegateCommand m_ToggleFullscreenCommand;
        private DelegateCommand m_RemovePlaylistItemCommand;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommands"/> class.
        /// </summary>
        internal AppCommands()
        {
            // placeholder
        }

        #endregion

        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public DelegateCommand OpenCommand => m_OpenCommand ??
            (m_OpenCommand = new DelegateCommand(async a =>
            {
                try
                {
                    var uriString = a as string;
                    if (string.IsNullOrWhiteSpace(uriString))
                        return;

                    var m = App.ViewModel.MediaElement;
                    var target = new Uri(uriString);
                    if (target.ToString().StartsWith(FileInputStream.Scheme, StringComparison.OrdinalIgnoreCase))
                        await m.Open(new FileInputStream(target.LocalPath));
                    else
                        await m.Open(target);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        Application.Current.MainWindow,
                        $"Media Failed: {ex.GetType()}\r\n{ex.Message}",
                        $"{nameof(MediaElement)} Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.OK);
                }
            }));

        /// <summary>
        /// Gets the close command.
        /// </summary>
        /// <value>
        /// The close command.
        /// </value>
        public DelegateCommand CloseCommand => m_CloseCommand ??
            (m_CloseCommand = new DelegateCommand(async o =>
            {
                await App.ViewModel.MediaElement.Close();

                // or, you can totally dispose manually:
                // App.ViewModel.MediaElement.Dispose();
            }));

        /// <summary>
        /// Gets the pause command.
        /// </summary>
        /// <value>
        /// The pause command.
        /// </value>
        public DelegateCommand PauseCommand => m_PauseCommand ??
            (m_PauseCommand = new DelegateCommand(async o =>
            {
                await App.ViewModel.MediaElement.Pause();
            }));

        /// <summary>
        /// Gets the play command.
        /// </summary>
        /// <value>
        /// The play command.
        /// </value>
        public DelegateCommand PlayCommand => m_PlayCommand ??
            (m_PlayCommand = new DelegateCommand(async o =>
            {
                // await Current.MediaElement.Seek(TimeSpan.Zero)
                await App.ViewModel.MediaElement.Play();
            }));

        /// <summary>
        /// Gets the stop command.
        /// </summary>
        /// <value>
        /// The stop command.
        /// </value>
        public DelegateCommand StopCommand => m_StopCommand ??
            (m_StopCommand = new DelegateCommand(async o =>
            {
                await App.ViewModel.MediaElement.Stop();
            }));

        /// <summary>
        /// Gets the predict noise command.
        /// </summary>
        /// <value>
        /// The stop command.
        /// </value>
        public DelegateCommand PredictNoiseInWholeVideoCommand => m_PredictNoiseInWholeVideoCommand ??
            (m_PredictNoiseInWholeVideoCommand = new DelegateCommand(async o =>
            {
                var dict = new Dictionary<TimeSpan, float>();
                var position = TimeSpan.Zero;
                var normalStep = 2000;
                var minStep = 50;
                var step = normalStep;
                var start = DateTime.Now;
                var hitTheEnd = false;

                TimeLine good = new()
                {
                };
                TimeLineEvent lastEvent = null;
                do
                {
                    await App.ViewModel.MediaElement.Seek(position);
                    var bitmap = await App.ViewModel.MediaElement.CaptureBitmapAsync();
                    var score = NoisePredictorModel.Predict(bitmap).Score.First();
                    if (dict.Count == 0 || Math.Abs(dict.Last().Value - score) < 0.5)
                    {
                        if (dict.Count == 0)
                        {
                            if (score > 0.5)
                                lastEvent = new TimeLineEvent { Start = TimeSpan.Zero };
                        }

                        dict[App.ViewModel.MediaElement.FramePosition] = score;
                    }
                    else if (step < minStep)
                    {
                        if (lastEvent == null)
                        {
                            lastEvent = new TimeLineEvent { Start = position };
                        }
                        else
                        {
                            lastEvent.Duration = position - lastEvent.Start;
                            good.Events.Add(lastEvent);
                            lastEvent = null;
                        }

                        dict[App.ViewModel.MediaElement.FramePosition] = score;
                        step = normalStep;
                    }
                    else
                    {
                        position -= TimeSpan.FromMilliseconds(step);
                        step /= 2;
                    }

                    position += TimeSpan.FromMilliseconds(step);
                    if (App.ViewModel.MediaElement.NaturalDuration < position && !hitTheEnd)
                    {
                        position = App.ViewModel.MediaElement.NaturalDuration.Value;
                        hitTheEnd = true;
                    }

                    if (App.ViewModel.MediaElement.NaturalDuration.Value < position && hitTheEnd)
                    {
                        if (lastEvent != null)
                        {
                            lastEvent.Duration = App.ViewModel.MediaElement.NaturalDuration.Value - lastEvent.Start;
                            good.Events.Add(lastEvent);
                        }
                    }
                }
                while (App.ViewModel.MediaElement.NaturalDuration.Value >= position);

                var done = DateTime.Now - start;
                good.Duration = App.ViewModel.MediaElement.NaturalDuration.Value;
                App.ViewModel.NoiseTimeLines = new ObservableCollection<TimeLine>();
                App.ViewModel.NoiseTimeLines.Add(good);
                Debug.WriteLine($"Done the whole movie ({App.ViewModel.MediaElement.NaturalDuration.Value.TotalSeconds}) in: {done.TotalSeconds}");
            }));

        /// <summary>
        /// Gets the toggle fullscreen command.
        /// </summary>
        /// <value>
        /// The toggle fullscreen command.
        /// </value>
        public DelegateCommand ToggleFullscreenCommand => m_ToggleFullscreenCommand ??
            (m_ToggleFullscreenCommand = new DelegateCommand(o =>
            {
                var mainWindow = Application.Current.MainWindow;

                // If we are already in fullscreen, go back to normal
                if (mainWindow.WindowStyle == WindowStyle.None)
                {
                    PreviousWindowStatus.Apply(mainWindow);
                    WindowStatus.EnableDisplayTimeout();
                }
                else
                {
                    PreviousWindowStatus.Capture(mainWindow);
                    mainWindow.WindowStyle = WindowStyle.None;
                    mainWindow.ResizeMode = ResizeMode.NoResize;
                    mainWindow.Topmost = true;
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.WindowState = WindowState.Maximized;
                    WindowStatus.DisableDisplayTimeout();
                }
            }));

        /// <summary>
        /// Gets the remove playlist item command.
        /// </summary>
        /// <value>
        /// The remove playlist item command.
        /// </value>
        public DelegateCommand RemovePlaylistItemCommand => m_RemovePlaylistItemCommand ??
            (m_RemovePlaylistItemCommand = new DelegateCommand(arg =>
            {
                if (arg is CustomPlaylistEntry entry)
                {
                    if (Uri.TryCreate(entry.MediaSource, UriKind.RelativeOrAbsolute, out var mediaSource))
                    {
                        App.ViewModel.Playlist.Entries.RemoveEntryByMediaSource(mediaSource);
                        App.ViewModel.Playlist.Entries.SaveEntries();
                    }
                }
            }));
    }
}
