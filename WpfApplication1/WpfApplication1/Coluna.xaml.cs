using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProgramaModificaArquivoZip
{
    /// <summary>
    /// Interaction logic for Coluna.xaml
    /// </summary>
    public partial class Coluna : Window
    {
        public int RetornoColuna;
        public Coluna()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            RetornoColuna = 1;

            if (radioButtonA.IsChecked == true)
            {
                RetornoColuna = 0;
            }
            else if (radioButtonB.IsChecked == true)
            {
                RetornoColuna = 1;
            }
            else if (radioButtonC.IsChecked == true)
            {
                RetornoColuna = 2;
            }
            else if (radioButtonD.IsChecked == true)
            {
                RetornoColuna = 3;
            }

            Close();
        }
    }
}
