using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csvLineBreakRemover.Utils
{
    public static class Constants
    {
        public const int BufferSize = 0x2000;
    }

    public static class FileAsync
    {
        /// <summary>Asynchronously creates a new file, writes a collection
        /// of strings to the file, and then closes the file.</summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <returns>A Task that represents completion of the method.</returns>
        public static async Task WriteAllLinesAsync(string path, IEnumerable<string> contents)
        {
            await WriteAllLinesAsync(path, contents, Encoding.UTF8);
        }

        /// <summary>Asynchronously creates a new file by using the specified encoding,
        /// writes a collection of strings to the file, and then closes the file.</summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>A Task that represents completion of the method.</returns>
        public static async Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding)
        {
            using (var memoryStream = new MemoryStream(contents.SelectMany(s => encoding.GetBytes(s.EndsWith("\r\n") ? s : s + "\r\n")).ToArray()))
            {
                using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, Constants.BufferSize, true))
                {
                    await memoryStream.CopyToAsync(stream, Constants.BufferSize);
                }
            }
        }
    }
}
