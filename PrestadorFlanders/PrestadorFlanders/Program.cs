using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Ionic.Zip;
using PrestadorFlanders.V30200;
using TissXsd30200 = Schema.V30200;
using TissXsd30201 = Schema.V30201;

namespace PrestadorFlanders
{
    internal class Program
    {
        private static void Main()
        {
            string caminhoZip = Convert.ToString(ConfigurationManager.AppSettings["caminhoZip"]);
            string caminhoXml = Convert.ToString(ConfigurationManager.AppSettings["caminhoXml"]);
            string caminhoXmlAlterado = Convert.ToString(ConfigurationManager.AppSettings["caminhoXmlAlterado"]);

            var dirOriginal = new DirectoryInfo(caminhoZip);
            FileInfo[] arquivosZip = dirOriginal.GetFiles("*.zip");
            
            foreach (FileInfo arquivo in arquivosZip)
            {
                using (ZipFile zip = ZipFile.Read(arquivo.FullName))
                {
                    zip.ExtractAll(caminhoXml, ExtractExistingFileAction.OverwriteSilently);
                }
                arquivo.Delete();
            }

            var dirXml = new DirectoryInfo(caminhoXml);
            FileInfo[] arquivosXml = dirXml.GetFiles("*.xml");

            foreach (FileInfo arquivo in arquivosXml)
            {
                var versãoDaBalada = VersaoBalada.ValidarEstruturaArquivoXml(arquivo.FullName);

                if (versãoDaBalada.Equals(VersaoBalada.VersaoTiss.V30200))
                    ImportarArquivo<TissXsd30200.mensagemTISS>(arquivo.FullName, caminhoXmlAlterado, arquivo.Name);

                if (versãoDaBalada.Equals(VersaoBalada.VersaoTiss.V30201))
                    ImportarArquivo<TissXsd30201.mensagemTISS>(arquivo.FullName, caminhoXmlAlterado, arquivo.Name);

                File.Delete(arquivo.FullName);

             /*   arquivo.MoveTo(Path.Combine(caminhoXmlAlterado,
                    arquivo.Name.Replace(
                        "_" + Convert.ToString(ConfigurationManager.AppSettings["remessaInicio"]) + ".xml",
                        "_" + Convert.ToString(ConfigurationManager.AppSettings["remessaFim"]) + ".xml")));*/
            }
            
            modificarLocalArquivosDescompactados(caminhoXmlAlterado, caminhoXml, caminhoZip);

        }

        private static void modificarLocalArquivosDescompactados(string caminhoXmlAlterado, string caminhoXml, string caminhoZip)
        {
            var diretorioAlterados = new DirectoryInfo(caminhoXmlAlterado);
            FileInfo[] arquivosXmlAlterados = diretorioAlterados.GetFiles("*.xml");
            foreach (FileInfo arquivo in arquivosXmlAlterados)
            {
                try
                {
                    new Memory().CreateSample(arquivo.FullName.Replace("xml", "zip"), caminhoXmlAlterado,
                        arquivo.FullName);

                    var arquivoPastaPrincipal
                        = Path.Combine(caminhoZip, arquivo.Name.Replace("xml", "zip"));

                    if (File.Exists(arquivoPastaPrincipal))
                    {
                        File.Delete(arquivoPastaPrincipal);
                    }

                    if (File.Exists(arquivo.FullName.Replace("xml", "zip")))
                    {
                        FileInfo arquivo1 = new FileInfo(arquivo.FullName.Replace("xml", "zip"));
                        arquivo1.MoveTo(arquivoPastaPrincipal);
                    }

                    File.Delete(arquivo.FullName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro ao tentar modificar arquivo {0} ", arquivo.FullName);
                }
            }
        }


        private static
            void ImportarArquivo<T>(string nomeArquivo, string caminhoXmlAlterado, string nomeArquivoSomente)
        {
            try
            {
                var serializer = new XmlSerializer(typeof (T));
                var fs = new FileStream(nomeArquivo, FileMode.Open);
                var reader = new StreamReader(fs, Encoding.GetEncoding("ISO-8859-1"));
                var mensagem = (T) serializer.Deserialize((reader));
                fs.Close();

                if (mensagem is TissXsd30200.mensagemTISS)
                {
                    var msg = (TissXsd30200.mensagemTISS) Convert.ChangeType(mensagem, mensagem.GetType());
                    /*
                    if ((msg.Item is TissXsd30200.prestadorOperadora) &&
                        (msg.cabecalho.origem.Item is TissXsd30200.cabecalhoTransacaoOrigemIdentificacaoPrestador))
                    {
                        if (
                            ((TissXsd30200.cabecalhoTransacaoOrigemIdentificacaoPrestador) msg.cabecalho.origem.Item)
                                .ItemElementName == TissXsd30200.ItemChoiceType.codigoPrestadorNaOperadora)
                        {
                            if (
                                ((TissXsd30200.cabecalhoTransacaoOrigemIdentificacaoPrestador) msg.cabecalho.origem.Item)
                                    .Item.Length == 11)
                            {
                                ((TissXsd30200.cabecalhoTransacaoOrigemIdentificacaoPrestador) msg.cabecalho.origem.Item)
                                    .ItemElementName = TissXsd30200.ItemChoiceType.CPF;
                                regerarXML = true;
                            }
                            if (
                                ((TissXsd30200.cabecalhoTransacaoOrigemIdentificacaoPrestador) msg.cabecalho.origem.Item)
                                    .Item.Length > 11)
                            {
                                ((TissXsd30200.cabecalhoTransacaoOrigemIdentificacaoPrestador) msg.cabecalho.origem.Item)
                                    .ItemElementName = TissXsd30200.ItemChoiceType.CNPJ;
                                regerarXML = true;
                            }
                        }
                    }

                    var itemLote = (TissXsd30200.ctm_guiaLote) (((TissXsd30200.prestadorOperadora) msg.Item).Item);

                    foreach (var item in itemLote.guiasTISS.Items)
                    {
                         if (item is TissXsd30200.ctm_consultaGuia)
                        {
                            regerarXML = true;

                        }
                        else if (item is TissXsd30200.ctm_honorarioIndividualGuia)
                        {
                            regerarXML = new IndividualGuia().AlterarIndividualGuia(
                                (TissXsd30200.ctm_honorarioIndividualGuia)item, regerarXML);
                        }
                        else
                        if (item is TissXsd30200.ctm_internacaoResumoGuia)
                        {
                            regerarXML = new InternacaoResumoGuia().AlterarInternacaoGuia(
                                (TissXsd30200.ctm_internacaoResumoGuia) item, regerarXML);
                        }
                        else if (item is TissXsd30200.ctm_spsadtGuia)
                        {
                            regerarXML = new SpsadtGuia().AlterarSpsadtGuia((TissXsd30200.ctm_spsadtGuia) item,
                                regerarXML);
                        }
                    }*/

                    msg.epilogo.hash = msg.epilogo.hash.Substring(0,28) + "1111";

                    
                        var novoLocalArquivo = Path.Combine(caminhoXmlAlterado, nomeArquivoSomente);

                        XmlSerializer serializador = new XmlSerializer(typeof (T));
                        var teste1 = new FileStream(novoLocalArquivo, FileMode.Create);
                        var stream = new StreamWriter(teste1, Encoding.GetEncoding("ISO-8859-1"));
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("ans", "http://www.ans.gov.br/padroes/tiss/schemas");
                        serializador.Serialize(stream, mensagem, ns);

                        stream.Close();
                        teste1.Close();
                    
                }
                else
                {
                    var msg = (TissXsd30201.mensagemTISS) Convert.ChangeType(mensagem, mensagem.GetType());

                    /*  if ((msg.Item is TissXsd30201.prestadorOperadora) && (msg.cabecalho.origem.Item is TissXsd30201.cabecalhoTransacaoOrigemIdentificacaoPrestador))
                    {
                        if (
                            ((TissXsd30201.cabecalhoTransacaoOrigemIdentificacaoPrestador) msg.cabecalho.origem.Item)
                                .ItemElementName == TissXsd30201.ItemChoiceType.codigoPrestadorNaOperadora)
                        {
                            if (((TissXsd30201.cabecalhoTransacaoOrigemIdentificacaoPrestador) msg.cabecalho.origem.Item).Item.Length == 11)
                            {
                                ((TissXsd30201.cabecalhoTransacaoOrigemIdentificacaoPrestador)msg.cabecalho.origem.Item).ItemElementName = TissXsd30201.ItemChoiceType.CPF;
                                regerarXML = true;
                            }
                            if (((TissXsd30201.cabecalhoTransacaoOrigemIdentificacaoPrestador)msg.cabecalho.origem.Item).Item.Length > 11)
                            {
                                ((TissXsd30201.cabecalhoTransacaoOrigemIdentificacaoPrestador)msg.cabecalho.origem.Item).ItemElementName = TissXsd30201.ItemChoiceType.CNPJ;
                                regerarXML = true;
                            }
                        }
                    }

                    var itemLote = (TissXsd30201.ctm_guiaLote)(((TissXsd30201.prestadorOperadora)msg.Item).Item);

                    foreach (var item in itemLote.guiasTISS.Items)
                    {
                        if (item is TissXsd30201.ctm_consultaGuia)
                        {
                            regerarXML = true;
                        }
                        else if (item is TissXsd30201.ctm_honorarioIndividualGuia)
                        {
                            regerarXML = new V30201.IndividualGuia().AlterarIndividualGuia(
                                (TissXsd30201.ctm_honorarioIndividualGuia)item, regerarXML);

                        }
                        else if (item is TissXsd30201.ctm_internacaoResumoGuia)
                        {
                            regerarXML = new V30201.InternacaoResumoGuia().AlterarInternacaoGuia(
                                (TissXsd30201.ctm_internacaoResumoGuia)item, regerarXML);
                        }
                        else if (item is TissXsd30201.ctm_spsadtGuia)
                        {
                            regerarXML = new V30201.SpsadtGuia().AlterarSpsadtGuia((TissXsd30201.ctm_spsadtGuia)item,
                                regerarXML);
                        }
                    }*/

                    msg.epilogo.hash = msg.epilogo.hash.Substring(0, 28) + "1111";

                    var novoLocalArquivo = Path.Combine(caminhoXmlAlterado, nomeArquivoSomente);

                    XmlSerializer serializador = new XmlSerializer(typeof (T));
                    var teste1 = new FileStream(novoLocalArquivo, FileMode.Create);
                    var stream = new StreamWriter(teste1, Encoding.GetEncoding("ISO-8859-1"));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("ans", "http://www.ans.gov.br/padroes/tiss/schemas");
                    serializador.Serialize(stream, mensagem, ns);

                    stream.Close();
                    teste1.Close();


                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
    }
}
