using Api.Autor.Image;
using Grpc.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using Api.Autor.Image.Settings;
using Microsoft.Extensions.Options;
using Imageupload;

namespace Api.Autor.Image.Services
{
    public class ImageUploaderService : ImageUploader.ImageUploaderBase
    {
        private readonly ILogger<ImageUploaderService> _logger;
        private readonly IMongoCollection<BsonDocument> _imagesCollection;

        public ImageUploaderService(ILogger<ImageUploaderService> logger, IOptions<MongoDbSettings> mongoDbSettings)
        {
            _logger = logger;
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _imagesCollection = database.GetCollection<BsonDocument>(mongoDbSettings.Value.CollectionName);
        }

        public override async Task<ImageUploadResponse> UploadImage(ImageUploadRequest request, ServerCallContext context)
        {
            try
            {
                // Convertir la imagen a BSON
                var imageDocument = new BsonDocument
                {
                    { "guid", request.Guid },
                    { "image_data", new BsonBinaryData(request.ImageData.ToByteArray()) }
                };

                await _imagesCollection.InsertOneAsync(imageDocument);

                return new ImageUploadResponse
                {
                    Success = true,
                    Message = "Image uploaded successfully!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return new ImageUploadResponse
                {
                    Success = false,
                    Message = "Error uploading image"
                };
            }
        }

        public override async Task<GetImageByGuidResponse> GetImageByGuid(GetImageByGuidRequest request, ServerCallContext context)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("guid", request.Guid);
                var imageDocument = await _imagesCollection.Find(filter).FirstOrDefaultAsync();

                if (imageDocument == null)
                {
                    return new GetImageByGuidResponse { Success = false, Message = "Image not found" };
                }

                return new GetImageByGuidResponse
                {
                    Success = true,
                    Message = "Image retrieved successfully",
                    ImageData = Google.Protobuf.ByteString.CopyFrom(imageDocument["image_data"].AsByteArray)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image");
                return new GetImageByGuidResponse { Success = false, Message = "Error retrieving image" };
            }
        }
    }
}
