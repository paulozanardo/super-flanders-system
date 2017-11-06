using System;
using System.IO;
using System.IO.Compression;

namespace ZipFlanders
{
    public class Program
    {
        public void Main(string[] args)
        {
            string caminhoArquivosZip = @""+args[0];
            string extractPath = @""+args[1];

            DirectoryInfo directorySelected = new DirectoryInfo(caminhoArquivosZip);

            foreach (FileInfo direc in directorySelected.GetFiles("*.zip"))
            {
                using (ZipArchive archive = ZipFile.OpenRead(direc.FullName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(extractPath, entry.FullName));
                        }
                    }
                }
            }
        }
    }
}
