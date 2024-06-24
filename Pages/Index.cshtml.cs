using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PhotoGallery.Models;
using PhotoGallery.Services;

public class IndexModel : PageModel
{
    private readonly PhotoService _photoService;

    public IndexModel(PhotoService photoService)
    {
        _photoService = photoService;
    }

    public List<Photo> Photos { get; set; }

    [BindProperty]
    public IFormFile Photo { get; set; }

    [BindProperty]
    public string Description { get; set; }

    public string ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Photos = await _photoService.GetAllPhotosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Photo != null)
        {
            // Check if the file is a valid image type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
            if (!allowedTypes.Contains(Photo.ContentType))
            {
                ErrorMessage = "Please upload a valid image file (JPEG, PNG, GIF, BMP).";
                await OnGetAsync();
                return Page();
            }

            // Save the file to the server
            var filePath = Path.Combine("wwwroot/images", Photo.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Photo.CopyToAsync(stream);
            }

            var newPhoto = new Photo
            {
                Url = $"/images/{Photo.FileName}",
                Description = Description
            };
            await _photoService.AddPhotoAsync(newPhoto);
        }

        return RedirectToPage();
    }
}
