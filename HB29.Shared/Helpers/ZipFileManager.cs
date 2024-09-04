using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;

namespace hb29.Shared.Helpers
{
    public class ZipFileManager
    {
        public static byte[] GenerateZipArchive(List<ZipFileManager.InMemoryFile> files, string password = null)
        {
            using (MemoryStream zipMemoryStream = new MemoryStream())
            {
                ZipOutputStream zipOutputStream = new ZipOutputStream(zipMemoryStream);

                zipOutputStream.SetLevel(6);

                if (!string.IsNullOrEmpty(password))
                {
                    zipOutputStream.Password = password;
                }

                foreach (var file in files)
                {
                    ZipEntry zipEntry = new ZipEntry(file.FileName);
                    zipEntry.Size = file.Content.Length;
                    zipOutputStream.PutNextEntry(zipEntry);
                    zipOutputStream.Write(file.Content, 0, file.Content.Length);
                }

                zipOutputStream.Finish();

                byte[] zipByteArray = new byte[zipMemoryStream.Length];
                zipMemoryStream.Position = 0;
                zipMemoryStream.Read(zipByteArray, 0, (int)zipMemoryStream.Length);

                return zipMemoryStream.ToArray();
            }
        }

        public class InMemoryFile
        {
            public string FileName { get; set; }
            public byte[] Content { get; set; }

            public InMemoryFile() { }
            public InMemoryFile(string filename)
            {
                this.FileName = filename;
            }

            public void StreamToContentByteArray(Stream stream)
            {
                MemoryStream ms = new();
                stream.CopyTo(ms);
                this.Content = ms.ToArray();
            }
        }
    }
}
