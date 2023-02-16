﻿// <auto-generated />
namespace NoisePredictor;

using System.Linq;
using Microsoft.ML;

/// <summary>
/// Noise Predictor Model.
/// </summary>
public partial class NoisePredictorModel
{
    /// <summary>
    /// Retrains model using the pipeline generated as part of the training process. For more information on how to load data, see aka.ms/loaddata.
    /// </summary>
    /// <param name="mlContext">ML Context.</param>
    /// <param name="trainData">Train data.</param>
    /// <returns>Transformer.</returns>
    public static ITransformer RetrainPipeline(MLContext mlContext, IDataView trainData)
    {
        var pipeline = BuildPipeline(mlContext);
        var model = pipeline.Fit(trainData);

        return model;
    }

    /// <summary>
    /// build the pipeline that is used from model builder. Use this function to retrain model.
    /// </summary>
    /// <param name="mlContext">ML Context.</param>
    /// <returns>Estimator.</returns>
    public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
    {
        // Data process configuration with pipeline data transformations
        var pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: @"Label", inputColumnName: @"Label")
                                .Append(mlContext.MulticlassClassification.Trainers.ImageClassification(labelColumnName: @"Label", scoreColumnName: @"Score", featureColumnName: @"ImageSource"))
                                .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: @"PredictedLabel", inputColumnName: @"PredictedLabel"));

        return pipeline;
    }
}
