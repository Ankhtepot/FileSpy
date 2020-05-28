using FileSpy.Model;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileSpy.Services
{
    public class StringServices
    {
        internal static string TransformInfoToCsvLineString(FileVersionInfo info, string rootPath, LoggedProperties loggedProps, char delimiter)
        {
            var result = new StringBuilder($"{GetRelativePathFromRootPath(info.FileName, rootPath)}{delimiter}");

            if (loggedProps.LogFileVersion) result.Append(info.FileVersion + delimiter);
            if (loggedProps.LogProductVersion) result.Append(info.ProductVersion + delimiter);

            return RemoveRedundantDelimiter(result.ToString(), delimiter);
        }

        public static string GetRelativePathFromRootPath(string absPath, string rootPath)
        {
            var relativePath = Regex.Replace(absPath, Regex.Escape(rootPath), ".");
            return relativePath == "." ? "Root Directory" : relativePath;
        }

        public static string RemoveRedundantDelimiter(string text, char delimiter)
        {
            if (text.ToString().LastOrDefault() == delimiter)
            {
                text = text.Remove(text.Length - 1, 1);
            }

            return text;
        }

        public static bool IsTextIncludingPatterns(string text, string[] searchPatterns)
        {
            foreach (var pattern in searchPatterns)
            {
                if (Regex.Match(text, pattern).Success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
