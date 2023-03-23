using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using static FPVNoiseDetector.NoisePredictorModel;

namespace FPVNoiseDetector.Foundation
{
    /// <summary>
    /// Simplifies 
    /// </summary>
    public static class PredictionHelper
    {
        /// <summary>
        /// Predicts the score of the given bitmap. Does the byte convertion internally.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>Prediction output</returns>
        public static ModelOutput Predict(Bitmap bitmap)
        {
            using MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bytes = ms.ToArray();

            NoisePredictorModel.ModelInput sampleData = new()
            {
                ImageSource = bytes,
            };

            // Make a single prediction on the sample data and print results.
            return NoisePredictorModel.Predict(sampleData);
        }

        /// <summary>
        /// Predicts asynchronously?
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static async Task<ModelOutput> PredictAsync(Bitmap bitmap)
        {
            return await Task.Run(() => Predict(ResizeBitmap(bitmap)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static async Task<bool> IsNoiseAsync(Bitmap bitmap)
        {
            var result = await PredictAsync(bitmap);
            return result.PredictedLabel == "noise";

        }
        /// <summary>
        /// Resizes the bitmap to unify it's size. We use 512x512 because that's what the Model is trained on.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>resized bitmap</returns>
        public static Bitmap ResizeBitmap(Bitmap bitmap)
        {
            int targetHeight = 640;

            // Calculate the new dimensions while maintaining aspect ratio
            float aspectRatio = (float)bitmap.Width / (float)bitmap.Height;
            int newHeight = targetHeight;
            int newWidth = (int)(aspectRatio * targetHeight);

            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(bitmap, 0, 0, newWidth, newHeight);
            }
            return resizedBitmap;
        }
        /// <summary>
        /// Load the Model on demand
        /// </summary>
        public static async Task LoadModel()
        {
            await Task.Run(() => NoisePredictorModel.PredictEngine.Value);
        }

        /// <summary>
        /// Check if the model has been loaded already
        /// </summary>
        public static bool IsModelLoaded
        {
            get { return NoisePredictorModel.PredictEngine.IsValueCreated; }
        }
    }
}
