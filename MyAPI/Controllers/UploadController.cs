using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Services;

namespace MyAPI.Controllers;

[ApiController]
[Route("api/upload")]
[RequestSizeLimit(50_000_000)]
public class UploadController : ControllerBase
{
    private readonly CloudinaryService _cloudinary;

    public UploadController(CloudinaryService cloudinary)
    {
        _cloudinary = cloudinary;
    }

    private const long MAX_FILE_SIZE = 50 * 1024 * 1024;

    private readonly string[] allowedExtensions =
    {
        // images
        ".jpg",".jpeg",".png",".webp",

        // video
        ".mp4",".mov",".mkv",

        // audio
        ".mp3",".wav",

        // documents
        ".doc",".docx",
        ".xls",".xlsx",
        ".ppt",".pptx",
        ".pdf",
        ".txt",

        // archives
        ".zip",".rar",".7z",

        // app / executable
        ".apk",".exe"
    };
    private readonly string[] allowedMimeTypes =
    {
        // images
        "image/jpeg",
        "image/png",
        "image/webp",

        // video
        "video/mp4",
        "video/quicktime",

        // audio
        "audio/mpeg",
        "audio/wav",

        // documents
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",

        "application/pdf",
        "text/plain",

        // archives
        "application/zip",
        "application/x-rar-compressed",
        "application/x-7z-compressed",

        // apk
        "application/vnd.android.package-archive",

        // exe
        "application/octet-stream"
    };

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File empty");

        if (file.Length > MAX_FILE_SIZE)
            return BadRequest("File too large (max 50MB)");

        var ext = Path.GetExtension(file.FileName).ToLower();

        // check extension
        if (!allowedExtensions.Contains(ext))
            return BadRequest("File extension not allowed");

        // check MIME type
        if (!allowedMimeTypes.Contains(file.ContentType))
            return BadRequest("Invalid file type");

        // sanitize filename
        var safeFileName = Path.GetFileName(file.FileName);

        string folder = "files";

        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp")
            folder = "images";

        if (ext == ".mp4" || ext == ".mov" || ext == ".mkv")
            folder = "videos";

        if (ext == ".mp3" || ext == ".wav")
            folder = "audio";

        if (ext == ".doc" || ext == ".docx" ||
            ext == ".xls" || ext == ".xlsx" ||
            ext == ".ppt" || ext == ".pptx" ||
            ext == ".pdf" || ext == ".txt")
            folder = "documents";

        var url = await _cloudinary.UploadAsync(file, folder);

        return Ok(new
        {
            success = true,
            url,
            fileName = safeFileName,
            size = file.Length
        });
    }
}