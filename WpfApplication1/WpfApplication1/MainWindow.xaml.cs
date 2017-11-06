/*using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace ProgramaModificaArquivoZip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int valorColuna;
        private string caminhoFinal;

        private StringBuilder texto = new StringBuilder();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vAbreArq = new OpenFileDialog
            {
                Title = "Selecione o Arquivo XLS Flor",
                Filter = "Arquivos xls (*.xls)|*.xls",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            
            var selecionouArquivo = vAbreArq.ShowDialog();

            if (selecionouArquivo ?? false)
            {
                var coluna = new Coluna();
                coluna.ShowDialog();

                valorColuna = coluna.RetornoColuna;

                var conexao =
                    new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + vAbreArq.FileName + ";" +
                                        "Extended Properties=Excel 8.0;");
                try
                {
                    var ds = new DataSet();

                    var da = new OleDbDataAdapter("Select * From [Plan1$]", conexao);
                    da.Fill(ds);

                    var listaTabela = ds.Tables[0].Rows;

                    string caminhoInicio = Convert.ToString(ConfigurationManager.AppSettings["caminhoInicio"]);
                    caminhoFinal = Convert.ToString(ConfigurationManager.AppSettings["caminhoFinal"]);

                    MoverArquivos(caminhoInicio, listaTabela);

                    ds.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "IHULL DEU ERRO!!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        private void MoverArquivos(string caminhoInicio, DataRowCollection listaTabela)
        {
            var dirOriginal = new DirectoryInfo(caminhoInicio);
            FileInfo[] arquivosCaminho = dirOriginal.GetFiles();

            if (caminhoInicio == caminhoFinal)
                return;

            foreach (DataRow itens in listaTabela)
            {
                DataRow itens1 = itens;

                if (!string.IsNullOrEmpty((string)itens1[valorColuna]))
                {
                    var listaArquivos = arquivosCaminho.Where(x => x.Name.Contains((string)itens1[valorColuna]));

                    if (!listaArquivos.Any())
                    {
                        texto.AppendLine("Nenhum arquivo com o nome " + (string) itens1[valorColuna] + "na pasta " +
                                         caminhoInicio);
                        textoGeracao.Text = texto.ToString();
                    }

                    foreach (var arquivo in listaArquivos)
                    {
                        if (!File.Exists(Path.Combine(@"" + caminhoFinal, arquivo.Name)))
                        {
                            texto.AppendLine("Movendo arquivo " + arquivo.Name);
                            File.Move(Path.Combine(@"" + caminhoInicio, arquivo.Name),
                                Path.Combine(@"" + caminhoFinal, arquivo.Name));
                        }
                        else
                        {
                            texto.AppendLine("Já existe arquivo " + arquivo.Name + " na pasta destino");
                        }

                        textoGeracao.Text = texto.ToString();
                    }
                }
            }

            if (tudoDaOrigem.IsChecked == true)
            {
                DirectoryInfo[] diretorios = dirOriginal.GetDirectories();

                foreach (var dir in diretorios)
                {
                    MoverArquivos(dir.FullName, listaTabela);
                }
            }
        }
    }
}

*/

using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace ProgramaModificaArquivoZip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int valorColuna;
        private string caminhoFinal;

        private StringBuilder texto = new StringBuilder();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ds = new DataSet();

                string caminhoInicio = Convert.ToString(ConfigurationManager.AppSettings["caminhoInicio"]);
                caminhoFinal = Convert.ToString(ConfigurationManager.AppSettings["caminhoFinal"]);


                MoverArquivos(caminhoInicio);

                ds.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IHULL DEU ERRO!!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MoverArquivos(string caminhoInicio)
        {
            var dirOriginal = new DirectoryInfo(caminhoInicio);
            FileInfo[] arquivosCaminho = dirOriginal.GetFiles();
    

            foreach (var arquivo in arquivosCaminho)
            {
                if (arquivo.Name.EndsWith("Log"))
                {
                    var a = arquivo.OpenText();
                    var readTexto = a.ReadToEnd();
                    a.Close();

                    if (!readTexto.Contains("Criado PEG:"))
                    {
                        if (readTexto.Contains("Cod2000") || readTexto.Contains("Cod2230"))
                        {
                            if (
                                !File.Exists(Path.Combine(@"" + caminhoFinal, arquivo.Name.Replace("Log", "ope"))))
                            {
                                texto.AppendLine("Movendo arquivo " + arquivo.Name);
                                File.Move(Path.Combine(@"" + caminhoInicio, arquivo.Name),
                                    Path.Combine(@"" + caminhoFinal, arquivo.Name.Replace("Log", "Ope")));
                            }
                        }
                        else if (readTexto.Contains("Cod2400"))
                        {
                            if (
                                !File.Exists(Path.Combine(@"" + caminhoFinal, arquivo.Name.Replace("Log", "Ava"))))
                            {
                                texto.AppendLine("Movendo arquivo " + arquivo.Name);
                                File.Move(Path.Combine(@"" + caminhoInicio, arquivo.Name),
                                    Path.Combine(@"" + caminhoFinal, arquivo.Name.Replace("Log", "Ava")));
                            }
                        }
                        else
                        {
                            if (!File.Exists(Path.Combine(@"" + caminhoFinal, arquivo.Name.Replace("Log", "Rej"))))
                            {
                                texto.AppendLine("Movendo arquivo " + arquivo.Name);
                                File.Move(Path.Combine(@"" + caminhoInicio, arquivo.Name),
                                    Path.Combine(@"" + caminhoFinal, arquivo.Name.Replace("Log", "Rej")));
                            }
                        }

                        textoGeracao.Text = texto.ToString();
                    }
                }
            }
        }
    }
}
