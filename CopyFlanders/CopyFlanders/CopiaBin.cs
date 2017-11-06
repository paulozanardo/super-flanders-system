using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyFlanders
{
    internal class CopiaBin
    {
        public string Bin;

        private static void CopiarDiretoriosInternos(DirectoryInfo diretorioAtual, string destDirName)
        {

            DirectoryInfo dir = new DirectoryInfo(diretorioAtual.FullName);

            var diretorioNovo = Path.Combine(destDirName, diretorioAtual.Name);

            if (!Directory.Exists(diretorioNovo))
            {
                Directory.CreateDirectory(diretorioNovo);
            }

            FileInfo[] files = dir.GetFiles();

            var dirCopia = new DirectoryInfo(diretorioNovo);

            FileInfo[] filesCopia = dirCopia.GetFiles();

            CopiarArquivos(files, filesCopia, diretorioNovo);

            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                CopiarDiretoriosInternos(subdir, diretorioNovo);
            }

        }

        public void Copiar()
        {
            try
            {
                if (!string.IsNullOrEmpty(Bin))
                {
                    try
                    {
                        var quebra = Bin.Split('>');
                        string diretorioOriginal = @"" + quebra.GetValue(0);
                        string diretorioCopia = @"" + quebra.GetValue(1);

                        var dirOriginal = new DirectoryInfo(diretorioOriginal);

                        DirectoryInfo[] dirs = dirOriginal.GetDirectories();

                        FileInfo[] files = dirOriginal.GetFiles();

                        var dirCopia = new DirectoryInfo(diretorioCopia);
                        FileInfo[] filesCopia = dirCopia.GetFiles();

                        Parallel.ForEach(dirs, dir =>
                        {
                            CopiarDiretoriosInternos(dir, diretorioCopia);
                        });

                    }
                    catch (DirectoryNotFoundException a)
                    {
                        Console.WriteLine(a.Message);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception se)
            {
                int ret = Marshal.GetLastWin32Error();
                MessageBox.Show(ret.ToString(), "Erro: " + ret.ToString());
                MessageBox.Show(se.Message);
            }
        }

        private static void CopiarArquivos(FileInfo[] files, FileInfo[] filesCopia, string diretorioCopia)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            foreach (var fInfo in files)
            {
                var receba = filesCopia.FirstOrDefault(x => x.Name == fInfo.Name);

                if (receba == null)
                {
                    File.Copy(fInfo.FullName, diretorioCopia + "\\" + fInfo.Name);
                    Console.WriteLine("Copiado arquivo :" + fInfo.Name);
                }
                else
                {
                    try
                    {
                        FileStream fileStream = fInfo.OpenRead();
                        FileStream fileStreamCopia = receba.OpenRead();

                        fileStream.Position = 0;
                        fileStreamCopia.Position = 0;

                        byte[] hashOriginal = md5.ComputeHash(fileStream);
                        byte[] hashCopia = md5.ComputeHash(fileStreamCopia);

                        Console.WriteLine("Validando arquivo :" + fInfo.FullName);

                        fileStream.Close();
                        fileStreamCopia.Close();

                        if (CompararHash(hashOriginal, hashCopia))
                        {
                            File.Delete(receba.FullName);

                            File.Copy(fInfo.FullName, receba.FullName);
                            Console.WriteLine("Alterado arquivo :" + fInfo.Name);

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" ****** ERRO NO ARQUIVO :" + fInfo.Name + " ****** " + e.Message);
                    }
                }
            }
        }

        public static bool CompararHash(byte[] array1, byte[] arra2)
        {
            int i;
            string a = "";
            string b = "";
            for (i = 0; i < array1.Length; i++)
            {
                a += String.Format("{0:X2}", array1[i]);
            }

            for (i = 0; i < arra2.Length; i++)
            {
                b += String.Format("{0:X2}", arra2[i]);
            }

            if (a == b)
                return false;

            return true;
        }
    }
}
