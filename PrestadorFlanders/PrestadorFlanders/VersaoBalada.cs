using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace PrestadorFlanders
{
    public class VersaoBalada
    {
        public enum VersaoTiss
        {
            [Description("3.02.00")]
            V30200,
            [Description("3.02.01")]
            V30201
        }

        public static string RecuperarValorXmlNo(Stream xml, string nomeNo)
        {
            xml.Position = 0;
            var reader = XmlReader.Create(xml, new XmlReaderSettings());

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(nomeNo))
                {
                    reader.Read();
                    return reader.Value ?? String.Empty;
                }
            }

            return String.Empty;
        }


        public static VersaoTiss? ValidarEstruturaArquivoXml(string arquivoProcessando)
        {
            try
            {
                var versao = String.Empty;
                using (var stream = new FileStream(arquivoProcessando, FileMode.Open))
                {
                    versao = XmlUtils.RecuperarValorXmlNo(stream, "ans:versaoPadrao");
                }

                if (VersaoTiss.V30200.Desc().Equals(versao))
                {
                    return VersaoTiss.V30200;
                }

                if (VersaoTiss.V30201.Desc().Equals(versao))
                {
                    return VersaoTiss.V30201;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
    }
}
