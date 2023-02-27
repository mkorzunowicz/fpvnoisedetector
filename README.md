# FPV Noise detector: *A video viewer with analogue video noise detector and video splitter*

[![semantic-release: angular](https://img.shields.io/badge/semantic--release-angular-e10079?logo=semantic-release)](https://github.com/semantic-release/semantic-release)

:star: *Please star this project if you like it and show your appreciation via*
**[PayPal.Me](https://www.paypal.me/mkorzunowicz/10eur)**

![fpvnoisedetector](https://github.com/mkorzunowicz/fpvnoisedetector/raw/master/Support/fpvnoisedetector.png)

## What does this even do?

* First and foremost plays videos using FFmpeg, based on an amazing project [ffmediaelement](https://github.com/unosquare/ffmediaelement) by Mario Divece.
* Detects analogue video noise seen in analogue FPV videos (and potentially old VHS casettes) and saves a timeline distinguishing noise and actual FPV footage.
* Splits the videos and merges them together without losing quality. Does that for a whole playlist.

## Why?

As an FPV drone pilot I have generated tons of analogue videos. I hoard them. I'm sure you do too :D The problem is: there's a lot of static noise recorded - each time you take a battery out, all your goggles see is noise. If the goggles won't recognize there's no signal, it gets recorded. Duh.

Usually FPV videos are split every couple of minutes. So you often end up having battery pack footage split into 2 parts. Of course I can merge them when editing the video, but why not having them archived properly? :)

And naturally: static video noise is practically a definition of randomness, meaning there's almost no possibility to compress it, or in other words - it takes quite a lot of disk space. Lose it! I tried recompressing the videos before. But by no means I would ever trim them by hand. Hence the app!

## How?

It uses an Image classification Machine Learning Model, which I trained with static noise and proper video stills to classify them. AI power ;)
FFmpeg is used to decode and play the vidoes as well as to split/merge them. It's lossless and super fast.

## Quick Usage Guide

### Installation

1. Download the FPV Noise detector app. It currently supports only Windows as it's a WPF application. (Sorry mac/linux users).
2. Download the newest 5.1.X FFmpeg **shared** binaries (64 or 32 bit, depending on your app's target architecture). Either from here [FFmpeg Windows Downloads](https://ffmpeg.org/download.html) or here [FFmpeg-Builds](https://github.com/BtbN/FFmpeg-Builds/releases). This [x64 lib](https://github.com/BtbN/FFmpeg-Builds/releases/download/autobuild-2023-02-12-12-35/ffmpeg-n5.1.2-12-g7268323193-win64-gpl-shared-5.1.zip) is guaranteed to work.
3. Your FFmpeg build should have a `bin` folder with 3 exe files and some dll files. Copy **all those** files to the folder 'c:\ffmpeg\x64' (or 'c:\ffmpeg'). The app uses both the libraries and binaries for now.
4. The FFmpeg are GPL, I don't want to be forced to use this license, therefore you need to fetch them yourself.

### Usage

1. Run the app. Open files from the playlist or drag and drop the video files or folders with your FPV footage into the app to add them to the playlist.
2. Pause the video and execute noise prediction (first time the app loads the prediction model it takes a few seconds).
3. Execute playlist splitting/merging to go through the whole playlist with the green save button. They will end up in '/split' folder under the directory of the split video.
4. That's it! Your videos are noiseless.

### Advanced usage

You can move the timeline sliders around by clicking on and moving the pegs.
Delete a timeline part, by moving the peg to the start until it turns red, then release the mouse button.
Press the button on the right of the timeline to select a video file which the currently edited file should be merged with.

## TODO's

* Automated next file to merge detection and possibly configuration for it, also to speed up the process
* Better button icons and UI upgrade in general
* Updater?

### Since it's based on the ffmeplay Sample Application it supports these shortcuts as well

| Shortcut Key | Function Description |
| --- | --- |
| G | Example of toggling subtitle color |
| Left | Seek 1 frame to the left |
| Right | Seek 1 frame to the right |
| + / Volume Up | Increase Audio Volume |
| - / Volume Down | Decrease Audio Volume |
| M / Volume Mute | Mute Audio |
| Up | Increase playback Speed |
| Down | Decrease playback speed |
| A | Cycle Through Audio Streams |
| S | Cycle Through Subtitle Streams |
| Q | Cycle Through Video Streams |
| C | Cycle Through Closed Caption Channels |
| R | Reset Changes |
| Y / H | Contrast: Increase / Decrease |
| U / J | Brightness: Increase / Decrease |
| I / K | Saturation: Increase / Decrease |
| E | Example of cycling through audio filters |
| T | Capture Screenshot to `desktop/ffplay` folder |
| W | Start/Stop recording packets (no transcoding) into a transport stream to `desktop/ffplay` folder. |
| Double-click | Enter fullscreen |
| Escape | Exit fullscreen |
| Mouse Wheel Up / Down | Zoom: In / Out |

## Thanks

* To Mario Divece and the contributors to the FFmpeg powered MediaElement for WPF, which this app is based on, meaning the Unosquare's' [ffmediaelement](https://github.com/unosquare/ffmediaelement).
* To the <a href="http://ffmpeg.org/">FFmpeg team</a>.

## License

* Please refer to the <a href="https://github.com/mkorzunowicz/fpvnoisedetector/blob/master/FPVNoiseDetector/LICENSE.txt">LICENSE</a> file for more information.
