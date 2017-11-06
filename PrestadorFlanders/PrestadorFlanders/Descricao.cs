using System;
using System.ComponentModel;

namespace PrestadorFlanders
{
    public static class Descricao
    {
        const string valorDefault = "Sem Descrição";
        /// <summary>
        /// Devolve a descrição do enum selecionado
        /// </summary>
        /// <param name="enumerador"></param>
        /// <returns>Descrição</returns>
        public static string Desc(this Enum enumerador)
        {

            if (enumerador == null)
                return valorDefault;

            var fieldInfo = enumerador.GetType().GetField(enumerador.ToString());
            var atributos = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return atributos.Length > 0 ? atributos[0].Description ?? valorDefault : enumerador.ToString();
        }
    }
}
