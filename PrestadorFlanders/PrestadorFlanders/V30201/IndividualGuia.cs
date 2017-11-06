using System;
using Schema.V30201;

namespace PrestadorFlanders.V30201
{
    internal class IndividualGuia
    {
        public bool AlterarIndividualGuia(ctm_honorarioIndividualGuia item, bool retorno)
        {
            decimal valorTotal = 0;

            ctm_honorarioIndividualGuia itemConvertido = item;

            decimal valorTotalCalculado = 0;

            if (itemConvertido.procedimentosRealizados != null)
            {
                foreach (var procedimentoRealizado in itemConvertido.procedimentosRealizados)
                {
                    valorTotalCalculado += procedimentoRealizado.valorTotal;
                }

                if (itemConvertido.valorTotalHonorarios != valorTotalCalculado)
                {
                    itemConvertido.valorTotalHonorarios = valorTotalCalculado;
                    retorno = true;
                }
            }
            
            return retorno;
        }
    }
}
