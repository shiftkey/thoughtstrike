using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ideastrike.Helpers;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;
using Ideastrike.Models.ViewModels;

namespace Ideastrike.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageRepository _images;
        private readonly ISettingsRepository _settings;
        private readonly IIdeaRepository _ideas;

        public ImageController(IImageRepository imageRepository, ISettingsRepository settingsRepository, IIdeaRepository ideaRepository)
        {
            _images = imageRepository;
            _settings = settingsRepository;
            _ideas = ideaRepository;
        }

        public FileResult Index(string id)
        {
            if (id.Contains("."))
            {
                id = id.Substring(0, id.IndexOf(".")); //string .jpg in case it was send in
            }
            var image = _images.Get(int.Parse(id));
            return File(new MemoryStream(image.ImageBits), "image/jpeg");
        }

        public FileResult Thumb(int id, int width)
        {

            var image = _images.Get(id);
            using (var memoryStream = new MemoryStream(image.ImageBits))
            {
                var drawingImage = System.Drawing.Image.FromStream(memoryStream);
                int thumbWidth = width;
                if (thumbWidth > _settings.MaxThumbnailWidth)
                {
                    thumbWidth = _settings.MaxThumbnailWidth;
                }
                var thumb = drawingImage.ToThumbnail(thumbWidth);
                using (var thumbnailStream = new MemoryStream())
                {
                    // TODO: format should be adaptive based on backing source?
                    thumb.Save(thumbnailStream, ImageFormat.Jpeg);
                    return File(new MemoryStream(thumbnailStream.GetBuffer()), "image/jpeg"); //massive WTF? If I just use thumnailStream, it doesn't work...
                }
            }
        }

        [HttpGet]
        public JsonResult ForIdea(int id)
        {
            Idea idea = _ideas.Get(id);
            //             return Response.AsJson().WithHeader("Vary", "Accept");
            return new JsonResult
                       {
                           JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                           Data =
                               idea.Images.Select(i => new ImageViewModel(i.Id, i.ImageBits.Length, i.Name)).ToArray(),
                       };
        }
    }
}
