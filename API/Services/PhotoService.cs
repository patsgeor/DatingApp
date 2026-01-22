using API.Entities;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var account= new Account (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary= new Cloudinary(account);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult= new ImageUploadResult();
        if (file.Length > 0)
        {
            await using var stream= file.OpenReadStream();//opens file stream
            var uploadParams= new ImageUploadParams //sets upload parameters
            {
                File= new FileDescription(file.FileName, stream),//δημιουργεί ένα FileDescription αντικείμενο με το όνομα του αρχείου και το stream
                Transformation= new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"), // επεξεργασία εικόνας με κέντρο το πρόσωπο,
                Folder= "datingapp-angular-20" //ορίζει τον φάκελο στον οποίο θα αποθηκευτεί η εικόνα
            };

            //uploading the image
            uploadResult= await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;

    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deletephoto = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deletephoto);
    }

   

}
