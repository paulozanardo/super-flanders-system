using System;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PrestadorFlanders
{
    /// <summary>
    /// Esta classe é responsavel por implementar metodos 
    /// para tratamento de XML
    /// </summary>
    public class XmlUtils
    {
        private static String mensagensErros = String.Empty;
        private static String elementoErro = String.Empty;

        /// <summary>
        /// Serializa um objeto para XML
        /// </summary>
        /// <typeparam name="T">Classe</typeparam>
        /// <param name="obj">Objeto a ser convertido para XML</param>
        /// <returns>String contendo o XML gerado em UTF-8</returns>
        public static string SerializarClasseParaXmlUtf8<T>(T obj) where T : class
        {
            try
            {
                var stream = new MemoryStream();
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string SerializarClasseParaXmlIso88591<T>(T obj) where T : class
        {
            try
            {
                var stream = new MemoryStream();
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, obj);
                return Encoding.GetEncoding("ISO-8859-1").GetString(stream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserializa um XML em UTF-8 para um objeto
        /// </summary>
        /// <typeparam name="T">Classe</typeparam>
        /// <param name="xml">XML a ser convertido para uma instancia da classe</param>
        /// <returns>Instancia da classe</returns>
        public static T DeserializarXmlParaClasseUtf8<T>(string xml) where T : class
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)) { Position = 0 };
                return (T)xmlSerializer.Deserialize(stream);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gera um XML baseado em uma classe
        /// </summary>
        /// <param name="mensagem">Classe contendo as informacoes desejadas</param>
        /// <param name="formato">Formato de codificação da string (Opcional)</param>
        /// <returns>XML da classe</returns> 
        public static string ConverterClasseParaXml(object mensagem, Encoding formato = null)
        {
            StringWriterWithEncoding writer = new StringWriterWithEncoding((new StringBuilder()),
                formato == null ? Encoding.GetEncoding("ISO-8859-1") : formato);
            XmlSerializer serializer = new XmlSerializer(mensagem.GetType());

            serializer.Serialize(writer, mensagem);

            return writer.ToString();
        }

        /// <summary>
        /// Converte o XML em uma classe estipulada
        /// </summary>
        /// <param name="mensagem">XML com os registros</param>
        /// <param name="type">Tipo da classe</param>
        /// <returns>Objeto montado</returns>
        public static object ConverterXmlParaClasse(Stream mensagem, Type type)
        {
            mensagem.Position = 0;
            XmlSerializer serializer = new XmlSerializer(type);
            try
            {
                return serializer.Deserialize(new XmlTextReader(mensagem));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Converte um XML em um DataSet
        /// </summary>
        /// <param name="xml">String com o XML desejado</param>
        /// <returns>DataSet preenchido com o XML informado</returns>
        public static DataSet ConverterXmlParaDataSet(string xml)
        {
            DataSet ds = new DataSet();
            byte[] buf = CriarMemoryStream(xml).ToArray();
            MemoryStream ms = new MemoryStream(buf);
            ds.ReadXml(ms);
            ms.Close();
            return ds;
        }

        /// <summary>
        /// Cria uma MemoryStream com o XML desejado
        /// </summary>
        /// <param name="pString">String com o XML</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream CriarMemoryStream(String pString)
        {
            MemoryStream stream = new MemoryStream();
            byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(pString);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Faz a validação da estrutura XML com o XSD, retornando todos os erros encontrados no arquivo
        /// </summary>
        /// <param name="xmlEntrada">Stream para o XML</param>
        /// <param name="targetNamespace">Namespace do arquivo xsd</param>
        /// <param name="schemaUri">Uri de onde encontrar o arquivo xsd para validação</param>
        /// <returns>Todos os erros encontrados</returns>
        public static string ValidarXml(Stream xmlEntrada, string targetNamespace, string schemaUri)
        {
            XmlReader reader;
            XmlReaderSettings settings;

            try
            {
                XmlUrlResolver resolver = new XmlUrlResolver();
                resolver.Credentials = CredentialCache.DefaultCredentials;

                var xmlDoc = new XmlDocument();
                xmlDoc.XmlResolver = resolver;

                xmlDoc.Load(xmlEntrada);

                if (xmlDoc.DocumentElement == null)
                    return String.Empty;

                xmlEntrada.Position = 0;

                settings = new XmlReaderSettings();
                settings.Schemas.Add(targetNamespace, schemaUri);

                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += new ValidationEventHandler(ErrosValidacaoXml);

                reader = XmlReader.Create(xmlEntrada, settings);
                mensagensErros = String.Empty;
                elementoErro = String.Empty;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                        elementoErro += reader.Value;
                }

                return mensagensErros;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + e.InnerException);
            }
            finally
            {
                xmlEntrada.Position = 0;
            }
        }

        /// <summary>
        /// Faz a validação da estrutura XML com o XSD, retornando todos os erros encontrados no arquivo
        /// </summary>
        /// <param name="xmlEntrada">Stream com o XML</param>
        /// <param name="schemas">Stream dos arquivos XSD</param>
        /// <returns>Todos os erros encontrados</returns>
        public static string ValidarXml(Stream xmlEntrada, Stream[] schemas)
        {
            try
            {
                var resolver = new XmlUrlResolver { Credentials = CredentialCache.DefaultCredentials };
                var xmlDoc = new XmlDocument { XmlResolver = resolver };

                xmlDoc.Load(xmlEntrada);

                if (xmlDoc.DocumentElement == null)
                    return String.Empty;

                xmlEntrada.Position = 0;

                var settings = CriarXmlReaderSettings(schemas);

                var reader = XmlReader.Create(xmlEntrada, settings);
                mensagensErros = String.Empty;
                elementoErro = String.Empty;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                        elementoErro += reader.Value;
                }

                return mensagensErros;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + e.InnerException);
            }
            finally
            {
                xmlEntrada.Position = 0;
            }

        }

        /// <summary>
        /// Criar a configuração de validação do xml, adicionando o stream de todos os schemas envolvidos para validação
        /// </summary>
        /// <param name="schemas">Lista de stream de schemas usados para validar</param>
        /// <returns>retorna objeto contendo os settings necessários para utilizar na validação do reader do XML</returns>
        private static XmlReaderSettings CriarXmlReaderSettings(Stream[] schemas)
        {
            var settings = new XmlReaderSettings();

            foreach (var schema in schemas)
            {
                XmlSchema schemaXml = XmlSchema.Read(schema, null);
                settings.Schemas.Add(schemaXml);
            }

            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(ErrosValidacaoXml);

            return settings;
        }

        /// <summary>
        /// Acumulador de erros encontrados durante a validação do arquivo XML
        /// </summary>
        private static void ErrosValidacaoXml(object sender, ValidationEventArgs e)
        {
            mensagensErros += "Erro: " + e.Message;

            if (elementoErro != String.Empty)
                mensagensErros += "\nConteúdo inválido: " + elementoErro;

            mensagensErros += "\nLinha: " + e.Exception.LineNumber +
                              "\nColuna: " + e.Exception.LinePosition + "\n\n";

        }

        /// <summary>
        /// retorna o Enum de um tipo, passando o valor especificado em seu XmlEnumAttribute
        /// </summary>
        public static string GetXmlAttrNameFromEnumValue<T>(T pEnumVal)
        {
            Type type = pEnumVal.GetType();
            FieldInfo info = type.GetField(Enum.GetName(typeof(T), pEnumVal));
            XmlEnumAttribute att = (XmlEnumAttribute)info.GetCustomAttributes(typeof(XmlEnumAttribute), false)[0];
            //If there is an xmlattribute defined, return the name

            return att.Name;
        }

        public static T GetEnumValueFromXmlAttrName<T>(string value)
        {
            foreach (object o in System.Enum.GetValues(typeof(T)))
            {
                T enumValue = (T)o;
                if (GetXmlAttrNameFromEnumValue<T>(enumValue).Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)o;
                }
            }

            throw new ArgumentException("Nenhum código XmlEnumAttribute existe para o tipo " + typeof(T).ToString() + " correspondendo ao valor de " + value);
        }

        /// <summary>
        /// Recupera o valor de um determinado no em um xml
        /// </summary>
        /// <param name="xml">stream conténdo o xml</param>
        /// <param name="nomeNo">nome da tag dentro do xml</param>
        /// <returns>valor obtido dentro da tag</returns>
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

        #region [Calculo de Hash]

        /// <summary>
        /// Calcular Hash padrão ANS de um arquivo XML transformado em string
        /// </summary>
        /// <param name="input">XML em formato string</param>
        /// <returns>Hash calculado</returns>
        public static string CalcularMD5Hash(string input)
        {
            // Primeiro passo, calcular o MD5 hash a partir da string
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(input); //doc.InnerText); - SMS 128821 - Danilo Raisi - 01/02/2010
            byte[] hash = md5.ComputeHash(inputBytes);

            // Segundo passo, converter o array de bytes em uma string hexadecimal
            StringBuilder sb = new StringBuilder();

            foreach (byte i in hash)
                sb.Append(i.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// Calcular Hash padrão ANS de um arquivo XML transformado em stream
        /// </summary>
        /// <param name="input">XML em formato stream</param>
        /// <returns>Hash calculado</returns>
        public static string CalcularMD5Hash(Stream input)
        {
            input.Position = 0;
            XDocument doc = XDocument.Load(XmlReader.Create(input));
            String prefix = (String.IsNullOrEmpty(doc.Root.GetPrefixOfNamespace("http://www.ans.gov.br/padroes/tiss/schemas")) ?
                String.Empty : doc.Root.GetPrefixOfNamespace("http://www.ans.gov.br/padroes/tiss/schemas"));
            XNamespace ns = (!prefix.Equals(String.Empty) ? doc.Root.GetNamespaceOfPrefix(prefix)
                : doc.Root.GetDefaultNamespace());

            doc = XDocument.Parse(doc.ToString().Replace("\n", ""));
            doc.Root.Descendants(ns + "epilogo").Remove();

            return XmlUtils.CalcularMD5Hash(doc.Root.Value.Replace("\n", ""));
        }


        /// <summary>
        /// Calcular o hash dos dados de XML no padrão Tiss
        /// Para o cálculo só será considerado os valores dos campos informados e não as suas tags.
        /// O epilogo e o seu codigo Hash não é considerado no cálculo
        /// Obs: Tiss 3.02
        /// </summary>
        /// <param name="xml">String XML</param>
        /// <returns>Hash calculado com base nos dados do XML</returns>
        public static string CalcularMD5HashTiss(string xml)
        {
            XmlDocument doc = new XmlDocument();
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            doc.Load(stream);
            XmlNode root = doc.DocumentElement;

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if ("epilogo".Equals(node.LocalName))
                {
                    root.RemoveChild(node);
                    break;
                }
            }

            return CalcularMD5Hash(root.InnerText);
        }

        #endregion
    }

    /// <summary>
    /// Classe para codificar o XML.
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        public StringWriterWithEncoding(StringBuilder builder, Encoding encoding)
            : base(builder)
        {
            this.encoding = encoding;
        }

        private Encoding encoding;

        public override Encoding Encoding
        {
            get
            {
                return encoding;
            }
        }
    }

}
