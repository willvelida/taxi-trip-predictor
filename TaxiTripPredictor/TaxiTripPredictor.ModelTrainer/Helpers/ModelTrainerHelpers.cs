using Azure.Storage.Blobs;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaxiTripPredictor.ModelTrainer.Models;

namespace TaxiTripPredictor.ModelTrainer.Helpers
{
    public class ModelTrainerHelpers : IModelTrainerHelpers
    {
        private readonly IBlobStorageHelpers _blobStorageHelpers;

        public ModelTrainerHelpers(
            IBlobStorageHelpers blobStorageHelpers)
        {
            _blobStorageHelpers = blobStorageHelpers;
        }

        public async Task TrainAndSaveModel(MLContext mlContext, string trainFilePath, string testFilePath, string modelPath, BlobClient blobClient)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(
                trainFilePath,
                hasHeader: true,
                separatorChar: ',');

            var pipeline = mlContext.Transforms
                .CopyColumns(outputColumnName: "Label", inputColumnName: "FareAmount")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
                .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
                .Append(mlContext.Regression.Trainers.FastTree());

            var model = pipeline.Fit(dataView);

            var modelRSquaredValue = Evalutate(mlContext, model, testFilePath);

            if (modelRSquaredValue >= 0.8)
            {
                mlContext.Model.Save(model, dataView.Schema, modelPath);

                await _blobStorageHelpers.UploadBlob(blobClient, modelPath);
            }           
        }

        private double Evalutate(MLContext mLContext, ITransformer model, string testFilePath)
        {
            IDataView dataView = mLContext.Data.LoadFromTextFile<TaxiTrip>(
                testFilePath,
                hasHeader: true,
                separatorChar: ',');

            var predictions = model.Transform(dataView);

            var metrics = mLContext.Regression.Evaluate(predictions, "Label", "Score");

            double rSquaredValue = metrics.RSquared;

            return rSquaredValue;
        }
    }
}
