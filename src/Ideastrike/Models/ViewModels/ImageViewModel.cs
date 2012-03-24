namespace Ideastrike.Models.ViewModels
{
    public class ImageViewModel
    {
        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }
        public string imageId { get; set; }

        public ImageViewModel(int imageId, int fileLength, string name)
        {
            SetValues(imageId, fileLength, name);
        }

        private void SetValues(int imageId, int fileLength, string name)
        {
            this.name = name;
            type = "image/png";
            size = fileLength;
            progress = "1.0";
            url = "/idea/image/" + imageId;
            thumbnail_url = "/idea/imagethumb/" + imageId + "/100";
            delete_url = "/idea/deleteimage/" + imageId;
            delete_type = "DELETE";
            this.imageId = imageId.ToString();
        }
    }
}