using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace RenataIngrataXLS
{
    class Program
    {
        static void Main(string[] args)
        {
            var vAbreArq = new OpenFileDialog
            {
                Filter = "*.xls | Microsoft Excel",
                Title = "Selecione o Arquivo"
            };

            var retorno = vAbreArq.ShowDialog();
            if (retorno == DialogResult.OK)
            {
                var conexao =
                    new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + vAbreArq.FileName +
                                        "; Extended Properties =’Excel 12.0 Xml; HDR = YES’;");
                OleDbDataAdapter adapter = new OleDbDataAdapter("select * from[Sheet1$]", conexao);
                var ds = new DataSet();

                try
                {
                    conexao.Open();
                    adapter.Fill(ds);
                    foreach (DataRow linha in ds.Tables[0].Rows)
                    {
                        Console.WriteLine(linha);
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    conexao.Close();
                }
            }
        }
    }
}
