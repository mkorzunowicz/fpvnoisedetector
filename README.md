# FPV Noise detector: *A video viewer with analogue video noise detector and video splitter*

[![semantic-release: angular](https://img.shields.io/badge/semantic--release-angular-e10079?logo=semantic-release)](https://github.com/semantic-release/semantic-release)

:star: *Please star this project if you like it and show your appreciation if you like it via* **[PayPal.Me](https://www.paypal.me/mkorzunowicz/10eur)**

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

Download the FPV Noise detector app. It currently supports only Windows as it's a WPF application. (Sorry mac/linux users). All the necessary files are included and it is self contained, so no additional .NET runtimes should be required.

### Usage

1. Run the app. Open files from the playlist or drag and drop the video files or folders with your FPV footage into the app to add them to the playlist.
2. Pause the video and execute noise prediction (first time the app loads the prediction model it takes a few seconds).
3. Execute playlist splitting/merging to go through the whole playlist with the green save button. They will end up in '/split' folder under the directory of the split video.
4. That's it! Your videos are noiseless.

### Tutorial

Check out the detailed tutorial here: [tutorial](/FPVNoiseDetector/TUTORIAL.md)

## TODO's

* Persistant config

## Thanks

* My girlfriend for helping me with maintaining the backlog ;)
* To Mario Divece and the contributors to the FFmpeg powered MediaElement for WPF, which this app is based on, meaning the Unosquare's' [ffmediaelement](https://github.com/unosquare/ffmediaelement).
* To the all the awesome developers of libraries I used. If some license credits are missing. Sorry - let me know.

## License

* Please refer to the <a href="https://github.com/mkorzunowicz/fpvnoisedetector/blob/master/FPVNoiseDetector/LICENSE.txt">LICENSE</a> file for more information.
