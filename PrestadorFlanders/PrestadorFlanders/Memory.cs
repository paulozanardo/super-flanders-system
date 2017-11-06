using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace PrestadorFlanders
{
    internal class Memory
    {
        // Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        public void CreateSample(string outPathname, string folderName, string caminhoCompletoArquivo)
        {

            FileStream fsOut = File.Create(outPathname);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(5); //0-9, 9 being the highest level of compression

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            CompressFolder(folderName, zipStream, folderOffset,caminhoCompletoArquivo);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        // Recurses down the folder structure
        //
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset, string caminhoCompletoArquivo)
        {

            FileInfo fi = new FileInfo(caminhoCompletoArquivo);

            string entryName = caminhoCompletoArquivo.Substring(folderOffset); // Makes the name in zip based on the folder
            entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

            newEntry.Size = fi.Length;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(caminhoCompletoArquivo))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset, caminhoCompletoArquivo);
            }
        }
    }
}
