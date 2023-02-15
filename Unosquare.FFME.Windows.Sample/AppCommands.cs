namespace Unosquare.FFME.Windows.Sample
{
    using FFMediaToolkit.Decoding;
    using FFMediaToolkit.Encoding;
    using Foundation;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents the Application-Wide Commands.
    /// </summary>
    public class AppCommands
    {
        #region Private State

        private readonly WindowStatus PreviousWindowStatus = new WindowStatus();
        private DelegateCommand m_OpenCommand;
        private DelegateCommand m_OpenFilesCommand;
        private DelegateCommand m_PauseCommand;
        private DelegateCommand m_PlayCommand;
        private DelegateCommand m_StopCommand;
        private DelegateCommand m_EncodeCommand;
        private DelegateCommand m_EncodePlayListCommand;
        private DelegateCommand m_MergeEndFileCommand;
        private DelegateCommand m_PredictNoiseInWholeVideoCommand;
        private DelegateCommand m_CloseCommand;
        private DelegateCommand m_ToggleFullscreenCommand;
        private DelegateCommand m_SaveEntriesCommand;
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

        private void LoadVideos(string[] filePaths)
        {
            foreach (var path in filePaths)
            {
                if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                {
                    LoadVideos(Directory.GetFiles(path));
                }
                else if (!App.ViewModel.Playlist.Entries.Any(e => Path.GetFullPath(e.MediaSource).Equals(Path.GetFullPath(path), StringComparison.InvariantCultureIgnoreCase)))
                {
                    using var fileRead = MediaFile.Open(path);
                    App.ViewModel.Playlist.Entries.Add(Path.GetFileNameWithoutExtension(path), fileRead.Info.Duration, path);
                }
            }
        }
        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public DelegateCommand OpenFilesCommand => m_OpenFilesCommand ??
            (m_OpenFilesCommand = new DelegateCommand(a =>
            {
                try
                {
                    if (a is string[] filePaths)
                        LoadVideos(filePaths);
                    else
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        if (openFileDialog.ShowDialog() == true)
                            LoadVideos(openFileDialog.FileNames);
                    }
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
                    Uri target;
                    if (a is string)
                    {
                        var uriString = a as string;
                        if (string.IsNullOrWhiteSpace(uriString))
                            return;
                        target = new Uri(uriString);
                        App.ViewModel.NoiseTimeLine = new TimeLine();
                    }
                    else
                    {
                        var playListEntry = a as Unosquare.FFME.Windows.Sample.Foundation.CustomPlaylistEntry;
                        if (string.IsNullOrWhiteSpace(playListEntry.MediaSource))
                            return;
                        target = new Uri(playListEntry.MediaSource);

                        App.ViewModel.NoiseTimeLine = playListEntry.NoiseTimeLine;
                    }

                    var m = App.ViewModel.MediaElement;

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
                if (App.ViewModel.IsPredicting)
                {
                    App.ViewModel.ShouldStopPredicting = true;
                }
                else if (App.ViewModel.IsEncoding)
                {
                    App.ViewModel.MediaEncoder.ShouldStopEncoding = true;
                }
                else
                    await App.ViewModel.MediaElement.Stop();
            }));

        /// <summary>
        /// Gets the encode timelines command.
        /// </summary>
        /// <value>
        /// The stop command.
        /// </value>
        public DelegateCommand EncodeCommand => m_EncodeCommand ??
            (m_EncodeCommand = new DelegateCommand(async o =>
            {
                App.ViewModel.MediaEncoder.ShouldStopEncoding = false;
                App.ViewModel.IsEncoding = true;
                App.ViewModel.NotificationMessage = $"Single file encoding started";

                // for each Event in the Timeline, seek to start and capture bitmaps through the duration.
                for (int i = 0; i < App.ViewModel.NoiseTimeLine.Events.Count; i++)
                {
                    if (App.ViewModel.MediaEncoder.ShouldStopEncoding) break;
                    var eve = App.ViewModel.NoiseTimeLine.Events[i];
                    var sourcePath = App.ViewModel.MediaElement.Source.AbsolutePath;

                    var dir = Directory.CreateDirectory($@"{Path.GetDirectoryName(sourcePath)}\split");
                    var destPath = Path.Combine(dir.FullName, $"{Path.GetFileNameWithoutExtension(sourcePath)}_{i}{Path.GetExtension(sourcePath)}");
                    await Task.Run(() => App.ViewModel.MediaEncoder.CopyVideo(sourcePath, destPath, eve.Start, eve.Duration));
                }
                App.ViewModel.IsEncoding = false;
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding)
                    App.ViewModel.NotificationMessage = $"Encoding cancelled";
            }));

        /// <summary>
        /// Gets the encode playlist command.
        /// </summary>
        /// <value>
        /// The encode playlist command.
        /// </value>
        public DelegateCommand EncodePlaylistCommand => m_EncodePlayListCommand ??
            (m_EncodePlayListCommand = new DelegateCommand(o =>
            {
                App.ViewModel.IsEncoding = true;
                //CopyPlaylistSortedByName(App.ViewModel.Playlist.Entries);
                SplitMergePlaylistSortedByName(App.ViewModel.Playlist.Entries);
                App.ViewModel.IsEncoding = false;

            }));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playlist"></param>
        public async void SplitMergePlaylistSortedByName(CustomPlaylistEntryCollection playlist)
        {
            var start = DateTime.Now;
            App.ViewModel.NotificationMessage = $"Splitting the playlist..";
            var sorted = playlist.ToList();
            sorted.Sort((x, y) => x.Title.CompareTo(y.Title));

            App.ViewModel.MediaEncoder.ShouldStopEncoding = false;

            var done = new List<CustomPlaylistEntry>();
            foreach (var entry in sorted)
            {
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding)
                    break;
                if (done.Contains(entry)) continue;
                string continuedVideo = null;
                CustomPlaylistEntry nextEntry = entry;
                while (nextEntry != null)
                {

                    done.Add(nextEntry);
                    continuedVideo = await SplitEntry(nextEntry, continuedVideo);
                    if (continuedVideo == null)
                    {
                        nextEntry = null;
                        break;
                    }
                    nextEntry = App.ViewModel.Playlist.Entries.First(e => Path.GetFullPath(e.MediaSource).Equals(Path.GetFullPath(entry.NoiseTimeLine.EndFile), StringComparison.InvariantCultureIgnoreCase));
                }
            }
            var finish = DateTime.Now - start;
            App.ViewModel.NotificationMessage = $"Done splitting the playlist in {finish.TotalSeconds.ToString("0.00")}s";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="continuedVideo"></param>
        public async Task<string> SplitEntry(CustomPlaylistEntry entry, string continuedVideo)
        {
            await this.OpenCommand.ExecuteAsync(entry);
            await this.PauseCommand.ExecuteAsync();
            //App.ViewModel.MediaElement.Stop();

            var timeline = entry.NoiseTimeLine;
            var sourcePath = entry.MediaSource;

            int i = 0;

            var dir = Directory.CreateDirectory($@"{Path.GetDirectoryName(sourcePath)}\split");
            // what if the continued video should go through the whole video (one TimeLineEvent) and should continue on?
            if (continuedVideo != null)
            {
                var eve = timeline.Events[i];

                var destPath = Path.Combine(dir.FullName, $"{Path.GetFileNameWithoutExtension(sourcePath)}_merged{Path.GetExtension(sourcePath)}");
                await Task.Run(() => App.ViewModel.MediaEncoder.CutVideo(sourcePath, destPath, eve.Start, eve.Duration));
                if (continuedVideo != null)
                {
                    var mergedPath = Path.Combine(dir.FullName, $"{Path.GetFileNameWithoutExtension(sourcePath)}_{i}{Path.GetExtension(sourcePath)}");

                    await Task.Run(() => App.ViewModel.MediaEncoder.MergeVideos(new[] { continuedVideo, destPath }, mergedPath));
                    File.Delete(continuedVideo);
                    File.Delete(destPath);
                }
                i++;
                if (timeline.Events.Count == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return continuedVideo;
            }
            // for each Event in the Timeline, seek to start and capture bitmaps through the duration.
            for (; i < timeline.Events.Count; i++)
            {
                //var startEncoding = DateTime.Now;
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding) break;
                var eve = timeline.Events[i];

                var destPath = Path.Combine(dir.FullName, $"{Path.GetFileNameWithoutExtension(sourcePath)}_{i}{Path.GetExtension(sourcePath)}");
                await Task.Run(() => App.ViewModel.MediaEncoder.CutVideo(sourcePath, destPath, eve.Start, eve.Duration));

                if (timeline.Events.Count - 1 == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return destPath;
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playlist"></param>
        public async void CopyPlaylistSortedByName(CustomPlaylistEntryCollection playlist)
        {
            var sorted = playlist.ToList();
            sorted.Sort((x, y) => x.Title.CompareTo(y.Title));

            App.ViewModel.MediaEncoder.ShouldStopEncoding = false;

            var done = new List<CustomPlaylistEntry>();
            foreach (var entry in sorted)
            {
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding)
                    break;
                if (done.Contains(entry)) continue;
                MediaOutput continuedVideo = null;
                CustomPlaylistEntry nextEntry = entry;
                while (nextEntry != null)
                {
                    if (App.ViewModel.MediaEncoder.ShouldStopEncoding)
                        break;
                    done.Add(nextEntry);
                    continuedVideo = await EncodeEntry(nextEntry, continuedVideo);
                    if (continuedVideo == null)
                    {
                        nextEntry = null;
                        break;
                    }
                    nextEntry = App.ViewModel.Playlist.Entries.First(e => Path.GetFullPath(e.MediaSource).Equals(Path.GetFullPath(entry.NoiseTimeLine.EndFile), StringComparison.InvariantCultureIgnoreCase));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="continuedVideo"></param>
        public async Task<MediaOutput> EncodeEntry(CustomPlaylistEntry entry, MediaOutput continuedVideo)
        {
            await this.OpenCommand.ExecuteAsync(entry);
            await this.PauseCommand.ExecuteAsync();
            //App.ViewModel.MediaElement.Stop();

            var timeline = entry.NoiseTimeLine;
            var sourcePath = entry.MediaSource;
            using var sourceVideo = MediaFile.Open(sourcePath);
            int i = 0;
            // what if the continued video should go through the whole video (one TimeLineEvent) and should continue on?
            if (continuedVideo != null)
            {
                var eve = timeline.Events[i];
                await Task.Run(() => App.ViewModel.MediaEncoder.CopyVideoPart(sourceVideo, continuedVideo, eve.Start, eve.Duration));
                i++;
                if (timeline.Events.Count == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return continuedVideo;
                else continuedVideo.Dispose();
            }
            // for each Event in the Timeline, seek to start and capture bitmaps through the duration.
            for (; i < timeline.Events.Count; i++)
            {
                //var startEncoding = DateTime.Now;
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding) break;
                var eve = timeline.Events[i];

                var dir = Directory.CreateDirectory($@"{Path.GetDirectoryName(sourcePath)}\split");
                var destPath = Path.Combine(dir.FullName, $"{Path.GetFileNameWithoutExtension(sourcePath)}_{i}{Path.GetExtension(sourcePath)}");

                var destVideo = App.ViewModel.MediaEncoder.CreateVideo(destPath, sourceVideo.Video.Info, sourceVideo.Audio.Info);

                await Task.Run(() => App.ViewModel.MediaEncoder.CopyVideoPart(sourceVideo, destVideo, eve.Start, eve.Duration));

                if (timeline.Events.Count - 1 == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return destVideo;
                else destVideo.Dispose();
            }

            return null;
        }

        /// <summary>
        /// Adds a merge link to end of file.
        /// </summary>
        /// <value>
        /// The merge at end command.
        /// </value>
        public DelegateCommand MergeEndFileCommand => m_MergeEndFileCommand ??
            (m_MergeEndFileCommand = new DelegateCommand(o =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    App.ViewModel.NoiseTimeLine.EndFile = openFileDialog.FileName;
                    OpenFilesCommand.Execute(new string[] { openFileDialog.FileName });
                    SaveEntriesCommand.Execute();
                    App.ViewModel.NotificationMessage = $"Set merge file at end to{App.ViewModel.NoiseTimeLine.EndFile}";
                }
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
                App.ViewModel.IsPredicting = true;
                App.ViewModel.ShouldStopPredicting = false;

                var dict = new Dictionary<TimeSpan, float>();
                var position = TimeSpan.Zero;
                var normalStep = 20000;
                var minStep = 50;
                var step = normalStep;
                var start = DateTime.Now;
                var hitTheEnd = false;

                TimeLine good = new()
                {
                    Duration = App.ViewModel.MediaElement.NaturalDuration.Value
                };

                App.ViewModel.NoiseTimeLine = good;

                TimeLineEvent lastEvent = null;
                App.ViewModel.NotificationMessage = "Detecting noise...";
                do
                {
                    await App.ViewModel.MediaElement.Seek(position);
                    var bitmap = await App.ViewModel.MediaElement.CaptureBitmapAsync();
                    //var score = NoisePredictorModel.Predict(bitmap).Score.First();
                    var result = await NoisePredictorModel.PredictAsync(bitmap);
                    var score = result.Score.First();
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

                            App.ViewModel.NotificationMessage = $"Found a no noise video part";
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
                while (App.ViewModel.MediaElement.NaturalDuration.Value >= position && !App.ViewModel.ShouldStopPredicting);

                var done = DateTime.Now - start;
                if (App.ViewModel.ShouldStopPredicting)
                {
                    App.ViewModel.NoiseTimeLine = new TimeLine();
                    App.ViewModel.NotificationMessage = $"Prediction cancelled after {done.TotalSeconds.ToString("0.00")}s";
                }
                else
                {
                    App.ViewModel.NotificationMessage = $"Noise detected in a {App.ViewModel.MediaElement.NaturalDuration.Value.TotalSeconds}s long video in: {done.TotalSeconds.ToString("0.00")}s";
                    Debug.WriteLine($"Noise detected in a {App.ViewModel.MediaElement.NaturalDuration.Value.TotalSeconds}s long video in: {done.TotalSeconds.ToString("0.00")}s");

                    App.ViewModel.Playlist.Entries.AddOrUpdateEntry(App.ViewModel.MediaElement.Source, App.ViewModel.MediaElement.MediaInfo, App.ViewModel.NoiseTimeLine);
                    App.ViewModel.Playlist.Entries.SaveEntries();
                }

                App.ViewModel.IsPredicting = false;
            }));


        /// <summary>
        /// Saves the playlist entries.
        /// </summary>
        /// <value>
        /// The save playlist command.
        /// </value>
        public DelegateCommand SaveEntriesCommand => m_SaveEntriesCommand ??
            (m_SaveEntriesCommand = new DelegateCommand(o =>
            {
                App.ViewModel.Playlist.Entries.AddOrUpdateEntry(App.ViewModel.MediaElement.Source, App.ViewModel.MediaElement.MediaInfo, App.ViewModel.NoiseTimeLine);
                App.ViewModel.Playlist.Entries.SaveEntries();
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
