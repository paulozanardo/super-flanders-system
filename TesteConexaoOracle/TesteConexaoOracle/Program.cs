using System;
using System.Configuration;
using Oracle.DataAccess.Client;

namespace TesteConexaoOracle
{
    class Program
    {
        static void Main()
        {
            try
            {
                using (var con = new OracleConnection(ConfigurationManager.ConnectionStrings["SOCPRO"].ToString()))
                {
                    con.Open();
                    Console.Write("Conexão com Oracle ocorreu corretamente");
                    Console.Read();
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                Console.Read();
            }
           
        }
    }
}
