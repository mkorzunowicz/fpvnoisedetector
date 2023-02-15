namespace Unosquare.FFME.Windows.Sample.Foundation;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using System;
using System.Diagnostics;

internal class MediaEncoder
{
    public static void CopyVideo(string source, string destination, TimeSpan start, TimeSpan duration)
    {
        var startEncoding = DateTime.Now;

        // Opens a multimedia file.
        // You can use the MediaOptions properties to set decoder options.
        using var fileRead = MediaFile.Open(source);

        // Print informations about the video stream.
        Console.WriteLine($"Bitrate: {fileRead.Info.Bitrate / 1000.0} kb/s");
        var info = fileRead.Video.Info;
        Console.WriteLine($"Duration: {info.Duration}");

        // Console.WriteLine($"Frames count: {info.NumberOfFrames ?? "N/A"}");
        var frameRateInfo = info.IsVariableFrameRate ? "average" : "constant";
        Console.WriteLine($"Frame rate: {info.AvgFrameRate} fps ({frameRateInfo})");
        Console.WriteLine($"Frame size: {info.FrameSize}");
        Console.WriteLine($"Pixel format: {info.PixelFormat}");
        Console.WriteLine($"Codec: {info.CodecName}");
        Console.WriteLine($"Is interlaced: {info.IsInterlaced}");

        // var settings = new VideoEncoderSettings(width: 1920, height: 1080, framerate: 30, codec: VideoCodec.H264);
        // var settings = new VideoEncoderSettings(width: info.FrameSize.Width, height: info.FrameSize.Height, framerate: Convert.ToInt32(info.AvgFrameRate), codec: ffmpeg.avcodec_get_name(info.CodecId));
        var settings = new VideoEncoderSettings(width: info.FrameSize.Width, height: info.FrameSize.Height, framerate: Convert.ToInt32(info.AvgFrameRate), codec: VideoCodec.H264);
        settings.EncoderPreset = EncoderPreset.SuperFast;
        settings.CRF = 17;

        // using System.Windows.Media.Imaging;
        // using System.Windows.Media;
        var audioInfo = fileRead.Audio.Info;
        var audioSettings = new AudioEncoderSettings(audioInfo.SampleRate, audioInfo.NumChannels, AudioCodec.AAC);

        using var fileSaved = MediaBuilder.CreateContainer(destination).WithVideo(settings).WithAudio(audioSettings).Create();

        fileSaved.Video.AddFrame(fileRead.Video.GetFrame(start));

        // fileSaved.Audio.AddFrame(fileRead.Audio.GetFrame(start));
        Debug.WriteLine($"Started encoding of a {duration.TotalSeconds}s video");
        do
        {
            try
            {
                // fileSaved.Audio.AddFrame(fileRead.Audio.GetNextFrame());
                fileSaved.Video.AddFrame(fileRead.Video.GetNextFrame());
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Failed to read frames: {ex.Message}");
                break;
            }
        }
        while (fileRead.Video.Position <= start + duration);

        var done = DateTime.Now - startEncoding;
        Debug.WriteLine($"Done encoding in: {done.TotalSeconds}s");
    }
}
