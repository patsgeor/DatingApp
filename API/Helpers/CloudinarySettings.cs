using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace API.Helpers;

public class CloudinarySettings
{
    public required string CloudName { get; set; }
    public required string ApiKey { get; set; }
    public required string ApiSecret { get; set; }

}
