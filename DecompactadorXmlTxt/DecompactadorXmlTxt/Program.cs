using System;
using System.Configuration;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace DecompactadorXmlTxt
{
    internal class Program
    {
        private static void Main()
        {
            string caminhoZip = Convert.ToString(ConfigurationManager.AppSettings["caminhoZip"]);
            string caminhoXml = Convert.ToString(ConfigurationManager.AppSettings["caminhoXml"]);
            string caminhoOutrosArquivos = Convert.ToString(ConfigurationManager.AppSettings["caminhoOutrosArquivos"]);

            var dirOriginal = new DirectoryInfo(caminhoZip);
            FileInfo[] arquivosZip = dirOriginal.GetFiles("*.zip");

            foreach (FileInfo arquivo in arquivosZip)
            {
                var alteraArquivo = false;

                using (FileStream fs = File.OpenRead(arquivo.FullName))
                {
                    ZipFile arquivoZip = null;
                    try
                    {
                        arquivoZip = new ZipFile(fs);

                        foreach (ZipEntry zipEntry in arquivoZip)
                        {
                            if (!zipEntry.IsFile)
                                continue;

                            if (zipEntry.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                            {
                                alteraArquivo = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Erro ao ler arquivo {0} : {1}", arquivo.Name, e.Message);
                        
                    }
                    finally
                    {
                        if (arquivoZip != null)
                        {
                            arquivoZip.IsStreamOwner = true;
                            arquivoZip.Close();
                        }
                    }

                    fs.Close();
                }

                try
                {
                    if (alteraArquivo && !File.Exists(caminhoXml + "\\" + arquivo.Name))
                        arquivo.MoveTo(caminhoXml + "\\" + arquivo.Name);
                    else
                    {
                        arquivo.MoveTo(caminhoOutrosArquivos + "\\" + arquivo.Name);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro ao modificar caminho do arquivo {0} : {1}", arquivo.Name, e.Message);
                }
            }

            FileInfo[] arquivoTxt = dirOriginal.GetFiles("*.txt");

            foreach (var txt in arquivoTxt)
            {
                txt.MoveTo(caminhoOutrosArquivos + "\\" + txt.Name);
            }

        }


        /*try
            {
                string caminhoZip = Convert.ToString(ConfigurationManager.AppSettings["caminhoZip"]);
                string caminhoXml = Convert.ToString(ConfigurationManager.AppSettings["caminhoXml"]);

                var dirOriginal = new DirectoryInfo(caminhoZip);
                FileInfo[] arquivosZip = dirOriginal.GetFiles("*.zip");

                foreach (FileInfo arquivoZip in arquivosZip)
                {
                    var alteraArquivo = false;

                    try
                    {
                        using (var arquivoZipAberto = ZipFile.OpenRead(arquivoZip.FullName))
                        {
                            foreach (ZipArchiveEntry arquivo in arquivoZipAberto)
                            {
                                if (arquivo.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                                {
                                    alteraArquivo = true;

                                    //arquivo.ExtractToFile(Path.Combine(caminhoXml, arquivo.FullName));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Erro ao ler arquivo: " + e.Message);
                    }
                    try
                    {
                        if (alteraArquivo && !File.Exists(caminhoXml + "\\" + arquivoZip.Name))
                            arquivoZip.MoveTo(caminhoXml + "\\" + arquivoZip.Name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Erro ao modificar caminho do arquivo {0} : {1}", arquivoZip.Name, e.Message);
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }*/
    }
}
