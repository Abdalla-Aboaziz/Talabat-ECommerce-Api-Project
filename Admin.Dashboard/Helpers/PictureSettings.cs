namespace Admin.Dashboard.Helpers
{
    public class PictureSettings
    {
        public static string UploadFile(IFormFile file, string folderName, string apiWwwRoot)
        {
            var folderPath = Path.Combine(apiWwwRoot, "images", folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + file.FileName;
            var filePath = Path.Combine(folderPath, fileName);

            using var fs = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fs);

            return $"images/{folderName}/{fileName}";
        }

        public static void DeleteFile(string folderName, string fileName, string apiWwwRoot)
        {
            var filePath = Path.Combine(apiWwwRoot, "images", folderName, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
