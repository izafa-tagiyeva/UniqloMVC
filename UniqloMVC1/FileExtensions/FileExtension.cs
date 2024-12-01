namespace UniqloMVC1.FileExtensions
{
    public static class FileExtension
    {
        public static async Task<string> UploadAsync(this IFormFile file, string rootPath, params string[] subPaths)
        {
          
            string uploadPath = Path.Combine(new[] { rootPath }.Concat(subPaths).ToArray());

          
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            
            string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);

            string filePath = Path.Combine(uploadPath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
           
            return fileName;
        }




        public static bool IsValidType(this IFormFile file, string type)
          => file.ContentType.StartsWith(type);
        public static bool IsValidSize(this IFormFile file, int kb)
            => file.Length <= kb * 1024;




    }
}
