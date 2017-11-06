using System;
using System.Configuration;
using System.Threading;
using CopyFlanders.ImpersonalHelper;
using CopyFlanders.Logon;
using CopyFlanders.Password;

namespace CopyFlanders
{
    internal class Program
    {
        [STAThreadAttribute]
        public static void Main(string[] args)
        {
            try
            {
                var logon = new SolicitarUsuario().RetornarDadosLogon();

                using (new ImpersonateHelper(logon.Usuario, logon.Dominio, logon.Senha))
                {
                    Console.Clear();
                    var tipoAtt = "";

                    if (args.Length > 0)
                    {
                        foreach (var s in args)
                        {
                            tipoAtt = s;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Digite o que pretende atualizar:");
                        Console.WriteLine(" 1 - TODOS ");
                        Console.WriteLine(" 2 - MASTER ");
                        Console.WriteLine(" 3 - RELEASE");
                        Console.WriteLine(" 4 - DESENV ");
                        Console.WriteLine(" 5 - PACKAGES ");
                        Console.WriteLine(" 6 - SOMENTE VERSÕES");

                        tipoAtt = Console.ReadLine();
                    }

                    if (tipoAtt.Equals(1))
                    {
                        ChamarAtualizacao("Release");
                        ChamarAtualizacao("Master");
                        ChamarAtualizacao("Desenv");
                        ChamarAtualizacao("Pacotes");
                    }

                    if (tipoAtt.Equals(2))
                    {
                        ChamarAtualizacao("Master");
                    }

                    if (tipoAtt.Equals(3))
                    {
                        ChamarAtualizacao("Release");
                    }

                    if (tipoAtt.Equals(4))
                    {
                        ChamarAtualizacao("Desenv");
                    }

                    if (tipoAtt.Equals(5))
                    {
                        ChamarAtualizacao("Pacotes");
                    }

                    if (tipoAtt.Equals(6))
                    {
                        ChamarAtualizacao("Release");
                        ChamarAtualizacao("Master");
                        ChamarAtualizacao("Desenv");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void ChamarAtualizacao(string nomeAppSeetings)
        {
            string caminhos = Convert.ToString(ConfigurationManager.AppSettings[nomeAppSeetings]);
            var copia = new CopiaBin();
            copia.Bin = caminhos;
            var tCopia = new Thread(copia.Copiar);
            tCopia.Start();
            tCopia.Join();
        }
    }
}