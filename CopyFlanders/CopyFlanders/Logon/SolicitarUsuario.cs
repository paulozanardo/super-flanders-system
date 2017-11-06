using System;
using System.Configuration;
using CopyFlanders.Password;

namespace CopyFlanders.Logon
{
    internal class SolicitarUsuario
    {
        public Classes.Logon RetornarDadosLogon()
        {
            string usuario;
            string senha;
            string dominio;

            string logon = Convert.ToString(ConfigurationManager.AppSettings["Logon"]);
            if (string.IsNullOrEmpty(logon))
            {
                Console.WriteLine("Usuário:");
                usuario = Console.ReadLine();

                Console.WriteLine("Password: ");
                senha = TratamentoPassword.LeituraComMascara();

                Console.WriteLine("Domínio:");
                dominio = Console.ReadLine();
            }
            else
            {
                var logon1 = logon.Split(';');
                if (logon1.Length == 3)
                {
                    usuario = logon1[0];
                    senha = logon1[2];
                    dominio = logon1[1];
                }
                else
                {throw new Exception("Usuário não informado");
                }
            }

            return new Classes.Logon()
            {
                Dominio = dominio,
                Senha = senha,
                Usuario = usuario
            };
        }
    }
}
