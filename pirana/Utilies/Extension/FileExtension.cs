using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace pirana.Utilies.Extension
{
    public static class FileExtension
    {
        public static bool CheckType(this IFormFile file , string type) => file.ContentType.Contains(type);
        public static bool CheckMemory(this IFormFile file, int kb) => kb * 1024 > file.Length;
        public static string SaveFile(this IFormFile file,string path)
        {
            string filename = ChangeFileName(file.FileName);
            using (FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return filename;
        }
        static string ChangeFileName(string oldname)
        {
            string extension=oldname.Substring(oldname.LastIndexOf('.'));
            if (oldname.Length < 32)
            {
                oldname= oldname.Substring(0, oldname.LastIndexOf("."));
            }
            else
            {
                oldname = oldname.Substring(0, 32);
            }
            return Guid.NewGuid().ToString() +oldname+extension;   
        }
        public static string CheckValidate(this IFormFile file, string type, int kb)
        {
            string result = "";
            if (!file.CheckType(type))
            {
                result += $"{file.FileName} tipi yanlisdir ";
            }
            if (!file.CheckMemory(kb))
            {
                result += $"{file.FileName}-in hecmi {kb}-dan artiq olmamalidir";

            }
            return result;
        }
        public static void DeleteFile(this string fileName,string root,string folder)
        {
            string path= Path.Combine(root,folder,fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
