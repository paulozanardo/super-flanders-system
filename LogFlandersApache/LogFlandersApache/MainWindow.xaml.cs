using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;

namespace LogFlandersApache
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog();

            open.Multiselect = false;
            open.Title = "Selecionar Log";
            open.InitialDirectory = @"C:\";

            var log = new List<LogApache>();

            open.ShowDialog();
            
            var lines = System.IO.File.ReadAllLines(open.FileName);

            string teste = "";

            try
            {
                foreach (string line in lines)
                {
                    teste = line;
                    var linha = line.Split(' ');

                    if (linha[5] == "\"-\"")
                    {
                        log.Add(new LogApache()
                        {
                            Ip = linha[0],
                            Campo1 = linha[1],
                            Campo2 = linha[2],
                            Data = linha[3].Replace("[", ""),
                            Acao = linha[5].Replace("\"", ""),
                            Url = linha[5],
                            TipoResposta = linha[6],
                            Porta = linha[7]
                        });
                    }
                    else
                    {
                        log.Add(new LogApache()
                        {
                            Ip = linha[0],
                            Campo1 = linha[1],
                            Campo2 = linha[2],
                            Data = linha[3].Replace("[", ""),
                            Acao = linha[5].Replace("\"", ""),
                            Url = linha[6],
                            TipoResposta = linha[8],
                            Porta = linha[9],
                            Navegacao = linha[7].Replace("\"", "")
                        });
                    }
                }

                Grida.ItemsSource = log;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro com a linha : " + teste);
            }
        }
    }
}
