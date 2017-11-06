using System;
using Schema.V30200;

namespace PrestadorFlanders.V30200
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
                    valorTotalCalculado = Convert.ToDecimal(procedimentoRealizado.quantidadeExecutada)*
                                          procedimentoRealizado.valorUnitario;

                    if (procedimentoRealizado.valorTotal != valorTotalCalculado)
                    {
                        procedimentoRealizado.valorTotal = valorTotalCalculado;
                        retorno = true;
                    }
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
