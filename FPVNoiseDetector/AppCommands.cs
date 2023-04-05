namespace FPVNoiseDetector
{
    using FFMediaToolkit.Decoding;
    using FFMediaToolkit.Encoding;
    using Foundation;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Emgu.CV.Structure;
    using Emgu.CV;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using Unosquare.FFME;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Represents the Application-Wide Commands.
    /// </summary>
    public class AppCommands
    {
        #region Private State

        private readonly WindowStatus PreviousWindowStatus = new WindowStatus();
        private DelegateCommand m_OpenCommand;
        private DelegateCommand m_OpenFilesCommand;
        private DelegateCommand m_OpenHomePageCommand;
        private DelegateCommand m_OpenDonateCommand;
        private DelegateCommand m_PauseCommand;
        private DelegateCommand m_ReportIssueCommand;
        private DelegateCommand m_PlayCommand;
        private DelegateCommand m_StopCommand;
        private DelegateCommand m_EncodeCommand;
        private DelegateCommand m_EncodePlayListCommand;
        private DelegateCommand m_MergeEndFileCommand;
        private DelegateCommand m_PredictNoiseInWholeVideoCommand;
        private DelegateCommand m_PredictNoiseInPlaylistCommand;
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
        /// Gets the report issue command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public DelegateCommand ReportIssueCommand => m_ReportIssueCommand ??
        (m_ReportIssueCommand = new DelegateCommand(a =>
        {
            var issueUrl = $"https://github.com/mkorzunowicz/fpvnoisedetector/issues/new?title=&body=%23%20Description%0A%0A%2APlease%20enter%20a%20general%20description%20for%20the%20issue%20here.%20Delete%20sections%20that%20are%20not%20relevant%20and%20provide%20additional%20sections%20if%20necessary.%2A%0A%2AProvide%20as%20much%20information%20as%20possible%2A%0A%0A%2A%2AIf%20prediction%20doesn%27t%20recognize%20your%20noisy%20video%20parts%2C%20send%20me%20a%20sample%20video%20with%20a%20name%20of%20your%20DVR%20recorder%2Fgoggles%20to%20improve%20the%20Prediction%20model%2A%2A%0A%0A%23%23%20Issue%20Categories%0A%0A%20-%20%5Bx%5D%20Bug%0A%20-%20%5B%20%5D%20Feature%20Request%0A%20-%20%5B%20%5D%20Prediction%20not%20recognizing%20my%20noise%0A%20-%20Question%20-%20please%20ask%20questions%20in%20the%20%28%5BDiscussions%20Section%5D%28https%3A%2F%2Fgithub.com%2Fmkorzunowicz%2Ffpvnoisedetector%2Fdiscussions%29%29%0A%0A%23%23%20Steps%20to%20Reproduce%0A%0A%201.%20Step%201%0A%202.%20Step%202%0A%203.%20Step%203%0A%0A%23%23%20Expected%20Results%0A%0A%20-%20Result%201%0A%20-%20Result%202%0A%0A%23%23%20Version%20Information%20-%20don%27t%20delete%0A%0A{App.ViewModel.AppVersion}%0A%0A%23%23%20Delete%20This%20Section%0A%0A-%20I%20work%20on%20this%20project%20for%20fun%20and%20on%20my%20free%20time.%0A-%20Please%20consider%20a%20donation%20here%3A%20https%3A%2F%2Fwww.paypal.me%2Fmkorzunowicz%2F10eur";
            var psi = new ProcessStartInfo
            {
                FileName = issueUrl,
                UseShellExecute = true
            };
            Process.Start(psi);
        }));

        /// <summary>
        /// Gets the open home page command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public DelegateCommand OpenHomePageCommand => m_OpenHomePageCommand ??
        (m_OpenHomePageCommand = new DelegateCommand(a =>
        {
            var issueUrl = $"https://github.com/mkorzunowicz/fpvnoisedetector";
            var psi = new ProcessStartInfo
            {
                FileName = issueUrl,
                UseShellExecute = true
            };
            Process.Start(psi);
        }));
        /// <summary>
        /// Gets the open home page command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public DelegateCommand OpenDonateCommand => m_OpenDonateCommand ??
        (m_OpenDonateCommand = new DelegateCommand(a =>
        {
            var issueUrl = $"https://www.paypal.com/paypalme/mkorzunowicz/10eur";
            var psi = new ProcessStartInfo
            {
                FileName = issueUrl,
                UseShellExecute = true
            };
            Process.Start(psi);
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
                        var playListEntry = a as FPVNoiseDetector.Foundation.CustomPlaylistEntry;
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
                    // App.ViewModel.Commands.CloseCommand.Execute();
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

                var entry = App.ViewModel.Playlist.Entries.First(e => e.MediaSource == App.ViewModel.Playlist.OpenMediaSource);
                var continued = await SplitEntry(entry);
                // TODO: figure out how to split this with just one file
                //if (continued != null)
                //{
                //    await SplitEntry(entry, continued);
                //}

                App.ViewModel.IsEncoding = false;
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding)
                    App.ViewModel.NotificationMessage = $"Encoding cancelled";
            }));
        private async Task EncodeVideo()
        {
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
        }
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
        public async Task<string> SplitEntry(CustomPlaylistEntry entry, string continuedVideo = null)
        {
            await this.OpenCommand.ExecuteAsync(entry);
            App.ViewModel.MediaElement.MediaReady += MediaElement_MediaReady;

            var timeline = entry.NoiseTimeLine;
            var sourcePath = entry.MediaSource;
            int i = 0;
            var dir = Directory.CreateDirectory($@"{Path.GetDirectoryName(sourcePath)}\split");
            // what if the continued video should go through the whole video (one TimeLineEvent) and should continue on?
            if (continuedVideo != null)
            {
                var eve = timeline.OrderedEvents[i];

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
                if (timeline.OrderedEvents.Count == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return continuedVideo;
            }
            // for each Event in the Timeline, seek to start and capture bitmaps through the duration.
            for (; i < timeline.OrderedEvents.Count; i++)
            {
                //var startEncoding = DateTime.Now;
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding) break;
                var eve = timeline.OrderedEvents[i];

                var destPath = Path.Combine(dir.FullName, $"{Path.GetFileNameWithoutExtension(sourcePath)}_{i}{Path.GetExtension(sourcePath)}");
                await Task.Run(() => App.ViewModel.MediaEncoder.CutVideo(sourcePath, destPath, eve.Start, eve.Duration));

                if (timeline.OrderedEvents.Count - 1 == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return destPath;
            }

            return null;
        }

        private void MediaElement_MediaReady(object sender, EventArgs e)
        {
            this.PauseCommand.ExecuteAsync();
            App.ViewModel.MediaElement.MediaReady -= MediaElement_MediaReady;
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
            App.ViewModel.MediaElement.MediaReady += MediaElement_MediaReady;
            //App.ViewModel.MediaElement.Stop();

            var timeline = entry.NoiseTimeLine;
            var sourcePath = entry.MediaSource;
            using var sourceVideo = MediaFile.Open(sourcePath);
            int i = 0;
            // what if the continued video should go through the whole video (one TimeLineEvent) and should continue on?
            if (continuedVideo != null)
            {
                var eve = timeline.OrderedEvents[i];
                await Task.Run(() => App.ViewModel.MediaEncoder.CopyVideoPart(sourceVideo, continuedVideo, eve.Start, eve.Duration));
                i++;
                if (timeline.OrderedEvents.Count == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return continuedVideo;
                else continuedVideo.Dispose();
            }
            // for each Event in the Timeline, seek to start and capture bitmaps through the duration.
            for (; i < timeline.OrderedEvents.Count; i++)
            {
                //var startEncoding = DateTime.Now;
                if (App.ViewModel.MediaEncoder.ShouldStopEncoding) break;
                var eve = timeline.OrderedEvents[i];

                var dir = Directory.CreateDirectory($@"{Path.GetDirectoryName(sourcePath)}\split");
                var destPath = Path.Combine(dir.FullName, $"{Path.GetFileNameWithoutExtension(sourcePath)}_{i}{Path.GetExtension(sourcePath)}");

                var destVideo = App.ViewModel.MediaEncoder.CreateVideo(destPath, sourceVideo.Video.Info, sourceVideo.Audio.Info);

                await Task.Run(() => App.ViewModel.MediaEncoder.CopyVideoPart(sourceVideo, destVideo, eve.Start, eve.Duration));

                if (timeline.OrderedEvents.Count - 1 == i && !string.IsNullOrEmpty(timeline.EndFile))
                    return destVideo;
                else destVideo.Dispose();
            }

            return null;
        }
        private void VerifyPlaylistConsistency()
        {
            // either we check if the whole playlist has noise detected
            // or we assume that if a video has no noise timeline, we take it whole
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
                if (!string.IsNullOrWhiteSpace(App.ViewModel.NoiseTimeLine.EndFile))
                {
                    App.ViewModel.NoiseTimeLine.EndFile = null;
                    return;
                }
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    if (Path.GetFullPath(openFileDialog.FileName) == Path.GetFullPath(App.ViewModel.Playlist.OpenMediaSource))
                    {
                        App.ViewModel.NotificationMessage = $"Cannot be the same file as current";
                        return;
                    }
                    App.ViewModel.NoiseTimeLine.EndFile = openFileDialog.FileName;
                    OpenFilesCommand.Execute(new string[] { openFileDialog.FileName });
                    SaveEntriesCommand.Execute();
                    App.ViewModel.NotificationMessage = $"File continuation set to: {Path.GetFileName(App.ViewModel.NoiseTimeLine.EndFile)}";
                }
            }));

        private async Task<string> DetectVideoContinuation(string sourcePath = null)
        {
            sourcePath = App.ViewModel.MediaElement.Source.AbsolutePath;
            sourcePath = HttpUtility.UrlDecode(sourcePath);
            // read the directory where the file is.. look up the name - check if it's a next in line file
            var dir = Path.GetDirectoryName(sourcePath);
            var fileNames = Directory.GetFiles(dir).Select(f => Path.GetFileName(f)).ToList();
            fileNames.Sort();
            var thisFileName = Path.GetFileName(sourcePath);
            var i = fileNames.IndexOf(thisFileName);
            if (i == -1) return null;
            else if (fileNames.Count > i + 1)
            {
                var nextName = fileNames[i + 1];
                if (IsNumericalSuccessor(Path.GetFileName(thisFileName), Path.GetFileName(nextName)))
                {
                    var next = Path.Combine(dir, nextName);
                    using var me = new Unosquare.FFME.MediaElement();
                    await me.Open(new Uri(next));
                    await me.Seek(TimeSpan.Zero);

                    var nextBitmap = await TryLoadBitmap(me);

                    await App.ViewModel.MediaElement.Seek(App.ViewModel.MediaElement.NaturalDuration.Value);
                    var prevBitmap = await TryLoadBitmap(App.ViewModel.MediaElement);
                    var diff = CompareBitmaps(prevBitmap, nextBitmap);
                    // if less than 0.7 then it's probably not similar enough, if it's 1, one of the frames was (so far discovered) black.
                    // but if they are really the same then this won't work :/
                    // TODO: validate
                    if (diff > 0.7 && diff < 1)
                    {
                        App.ViewModel.NoiseTimeLine.EndFile = next;
                        await OpenFilesCommand.ExecuteAsync(new[] { next });
                        return nextName;
                    }
                }
            }
            return null;
        }
        private static bool IsNumericalSuccessor(string firstFileName, string secondFileName)
        {
            Regex regex = new Regex(@"\d+");

            Match match = regex.Match(firstFileName);
            Match match2 = regex.Match(secondFileName);

            if (match.Success && match2.Success)
            {
                var first = int.Parse(match.Value);
                var second = int.Parse(match2.Value);
                return second == first + 1;
            }
            else return false;
        }
        /// <summary>
        /// Compare two Bitmaps
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
                throw new ArgumentException("Bitmaps must be of the same size.");
            Image<Rgb, byte> img1 = new Image<Rgb, byte>(bmp1.Width, bmp1.Height)
            {
                Bytes = GetBitmapData(bmp1)
            }; Image<Rgb, byte> img2 = new Image<Rgb, byte>(bmp2.Width, bmp2.Height)
            {
                Bytes = GetBitmapData(bmp2)
            };

            Mat result = new Mat();
            var diff = img1.AbsDiff(img2);
            CvInvoke.MatchTemplate(img1.Mat, img2.Mat, result, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

            var similarity = (float)(result.GetData() as float[,]).GetValue(0, 0);

            return similarity;
        }

        private static byte[] GetBitmapData(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            IntPtr ptr = bmpData.Scan0;

            byte[] rgbValues = null;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);
            bmp.UnlockBits(bmpData);

            return rgbValues;
        }
        /// <summary>
        /// Gets the predict noise command.
        /// </summary>
        /// <value>
        /// The stop command.
        /// </value>
        public DelegateCommand PredictNoiseInPlaylistCommand => m_PredictNoiseInPlaylistCommand ??
            (m_PredictNoiseInPlaylistCommand = new DelegateCommand(async o =>
            {
                var sorted = App.ViewModel.Playlist.Entries.ToList();
                sorted.Sort((x, y) => x.Title.CompareTo(y.Title));

                foreach (var entry in sorted)
                {
                    if (!File.Exists(entry.MediaSource))
                    {
                        App.ViewModel.NotificationMessage = $"File not found: {entry.MediaSource}";
                        continue;
                    }
                    if (!App.ViewModel.RedoPrediction && entry.NoiseTimeLine.Events.Any())
                        continue;
                    if (App.ViewModel.ShouldStopPredicting)
                    {
                        App.ViewModel.NotificationMessage = "Aborted playlist detection...";
                        break;
                    }
                    try
                    {
                        App.ViewModel.MediaElement.MediaReady += PredictOnReady;
                        await this.OpenCommand.ExecuteAsync(entry);
                    }
                    catch (Exception ex)
                    {
                        App.ViewModel.NotificationMessage = $"Error opening file: {ex.Message}";
                        continue;
                    }
                    App.ViewModel.IsPredicting = true;
                    await Task.Run(() => { while (App.ViewModel.IsPredicting) { Thread.Sleep(300); } });
                }

                await this.StopCommand.ExecuteAsync();
                App.ViewModel.ShouldStopPredicting = false;
                App.ViewModel.IsPredicting = false;
            }));

        private void PredictOnReady(object sender, EventArgs e)
        {
            this.PauseCommand.ExecuteAsync();
            App.ViewModel.MediaElement.MediaReady -= PredictOnReady;
            PredictNoiseInWholeVideoCommand.Execute();
        }
        private async Task<System.Drawing.Bitmap> TryLoadBitmap(MediaElement me)
        {
            var seekTo = me.Position;
            var bitmap = await Task<System.Drawing.Bitmap>.Run(async () =>
            {
                System.Drawing.Bitmap bm = null;
                for (int i = 0; i < 30; i++)
                {
                    bm = await me.CaptureBitmapAsync();
                    if (bm != null) break;
                    if (i % 10 == 0 && bm == null)
                    {
                        App.ViewModel.NotificationMessage = $"File didn't load correctly. Reloading file.";
                        await me.Open(me.Source);
                        await me.Seek(seekTo);
                        //await me.Open(new Uri(App.ViewModel.Playlist.OpenMediaSource));
                    }
                    Thread.Sleep(100);
                }
                return bm;
            });
            if (bitmap == null)
                throw new Exception("couldn't read file/bitmap in 30 tries and 3 reloads");
            return bitmap;
        }

        private async Task PredictNoiseInVideo()
        {
            App.ViewModel.IsPredicting = true;
            App.ViewModel.ShouldStopPredicting = false;

            var dict = new Dictionary<TimeSpan, bool>();
            var position = TimeSpan.Zero;
            var normalStep = App.ViewModel.Controller.PredictionPrecision * 1000;
            if (App.ViewModel.MediaElement.NaturalDuration.Value.TotalSeconds < App.ViewModel.Controller.PredictionPrecision)
                normalStep = (int) (App.ViewModel.MediaElement.NaturalDuration.Value.TotalSeconds / 2 * 1000);
            var minStep = App.ViewModel.MediaElement.PositionStep.Milliseconds == 0 ? 20 : App.ViewModel.MediaElement.PositionStep.Milliseconds;
            var step = normalStep;
            var start = DateTime.Now;
            var hitTheEnd = false;

            TimeLine good = new()
            {
                Duration = App.ViewModel.MediaElement.NaturalDuration.Value
            };

            App.ViewModel.NoiseTimeLine = good;

            TimeLineEvent lastEvent = null;
            if (predictionModelLoaded) App.ViewModel.NotificationMessage = "Detecting noise...";
            else App.ViewModel.NotificationMessage = "Loading prediction model. It can take a few seconds...";
            do
            {
                await App.ViewModel.MediaElement.Seek(position);
                //App.ViewModel.MediaElement.Position = position;
                var bitmap = await TryLoadBitmap(App.ViewModel.MediaElement);
                var isNoise = false;
                if (App.ViewModel.UseSimilarityForPrediction)
                {
                    if (position == TimeSpan.Zero)
                        await App.ViewModel.MediaElement.StepForward();
                    else
                        await App.ViewModel.MediaElement.StepBackward();

                    var bitmap2 = await TryLoadBitmap(App.ViewModel.MediaElement);
                    isNoise = CompareBitmaps(bitmap, bitmap2) < 0.15;
                }
                else
                    isNoise = await PredictionHelper.IsNoiseAsync(bitmap);

                if (!predictionModelLoaded) App.ViewModel.NotificationMessage = "Detecting noise...";
                predictionModelLoaded = true;
                if (dict.Count == 0 || dict.Last().Value == isNoise)
                {
                    if (dict.Count == 0 && !isNoise)
                        lastEvent = new TimeLineEvent { Start = TimeSpan.Zero };

                    dict[App.ViewModel.MediaElement.FramePosition] = isNoise;
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

                    dict[App.ViewModel.MediaElement.FramePosition] = isNoise;
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
                    position = App.ViewModel.MediaElement.NaturalDuration.Value - App.ViewModel.MediaElement.PositionStep;
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
            else if (App.ViewModel.UseLinkingAfterPrediction)
            {
                var foundNext = await DetectVideoContinuation();
                var message = $"Noise detected in: {done.TotalSeconds.ToString("0.00")}s.";
                if (foundNext != null) message += $" Continuation detected in {foundNext}";
                App.ViewModel.NotificationMessage = message;
                Debug.WriteLine($"Video length: {App.ViewModel.MediaElement.NaturalDuration.Value.TotalSeconds}s. {message}");

                App.ViewModel.Playlist.Entries.AddOrUpdateEntry(App.ViewModel.MediaElement.Source, App.ViewModel.MediaElement.MediaInfo, App.ViewModel.NoiseTimeLine);
                App.ViewModel.Playlist.Entries.SaveEntries();
            }
            await App.ViewModel.MediaElement.Stop();
            App.ViewModel.IsPredicting = App.ViewModel.ShouldStopPredicting = false;
        }

        bool predictionModelLoaded = false;

        // For testing of similarity
        private async Task CompareNextFrames()
        {
            App.ViewModel.IsPredicting = true;
            App.ViewModel.ShouldStopPredicting = false;
            var step = App.ViewModel.MediaElement.PositionStep.Milliseconds == 0 ? 20 : App.ViewModel.MediaElement.PositionStep.Milliseconds;
            var position = App.ViewModel.MediaElement.Position;
            await App.ViewModel.MediaElement.Seek(position);
            System.Drawing.Bitmap prevBitmap = await TryLoadBitmap(App.ViewModel.MediaElement);
            while (position <= App.ViewModel.MediaElement.NaturalDuration.Value && !App.ViewModel.ShouldStopPredicting)
            //while (await App.ViewModel.MediaElement.StepForward() && !App.ViewModel.ShouldStopPredicting)
            {
                //position += TimeSpan.FromMilliseconds(step);
                await App.ViewModel.MediaElement.Seek(position += TimeSpan.FromMilliseconds(step));

                //await App.ViewModel.MediaElement.StepForward();
                var bm = await TryLoadBitmap(App.ViewModel.MediaElement);
                var similarity = CompareBitmaps(prevBitmap, bm);

                App.ViewModel.NotificationMessage = $"Similarity: {similarity}";
                prevBitmap = bm;
            }
            App.ViewModel.IsPredicting = App.ViewModel.ShouldStopPredicting = false;
        }
        /// <summary>
        /// Gets the predict noise command.
        /// </summary>
        /// <value>
        /// The predict noise command.
        /// </value>
        public DelegateCommand PredictNoiseInWholeVideoCommand => m_PredictNoiseInWholeVideoCommand ??
            (m_PredictNoiseInWholeVideoCommand = new DelegateCommand(async o =>
            {
                //await CompareNextFrames();
                await PredictNoiseInVideo();
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
