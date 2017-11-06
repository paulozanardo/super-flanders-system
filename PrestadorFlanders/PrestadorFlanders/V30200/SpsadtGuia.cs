using Schema.V30200;

namespace PrestadorFlanders.V30200
{
    internal class SpsadtGuia
    {
        public bool AlterarSpsadtGuia(ctm_spsadtGuia item, bool retorno)
        {
            decimal valorTotalProcedimento = 0;
            decimal valorDiarias = 0;
            decimal valorTaxasAlugueis = 0;
            decimal valorMateriais = 0;
            decimal valorMedicamentos = 0;
            decimal valorOPME = 0;
            decimal valorGasesMedicinais = 0;

            ctm_spsadtGuia itemConvertido = item;


            if (itemConvertido.procedimentosExecutados != null)
                foreach (var procedimentoExecutado in itemConvertido.procedimentosExecutados)
                {
                    valorTotalProcedimento += procedimentoExecutado.valorTotal;
                }

            if (itemConvertido.outrasDespesas != null)
            {
                foreach (var outraDespesa in itemConvertido.outrasDespesas)
                {
                    if (outraDespesa.codigoDespesa == dm_outrasDespesas.Item05)
                    {
                        valorDiarias += outraDespesa.servicosExecutados.valorTotal;
                    }
                    else if (outraDespesa.codigoDespesa == dm_outrasDespesas.Item07)
                    {
                        valorTaxasAlugueis += outraDespesa.servicosExecutados.valorTotal;
                    }
                    else if (outraDespesa.codigoDespesa == dm_outrasDespesas.Item01)
                    {
                        valorGasesMedicinais += outraDespesa.servicosExecutados.valorTotal;
                    }
                    else if (outraDespesa.codigoDespesa == dm_outrasDespesas.Item03)
                    {
                        valorMateriais += outraDespesa.servicosExecutados.valorTotal;
                    }
                    else if (outraDespesa.codigoDespesa == dm_outrasDespesas.Item02)
                    {
                        valorMedicamentos += outraDespesa.servicosExecutados.valorTotal;
                    }
                    else if (outraDespesa.codigoDespesa == dm_outrasDespesas.Item08)
                    {
                        valorOPME += outraDespesa.servicosExecutados.valorTotal;
                    }

                }
            }

            var valorTotalDeTudo = valorTotalProcedimento + valorDiarias + valorTaxasAlugueis +
                                   valorGasesMedicinais + valorMateriais + valorMedicamentos + valorOPME;

            if (itemConvertido.valorTotal.valorProcedimentos != valorTotalProcedimento)
            {
                itemConvertido.valorTotal.valorProcedimentos = valorTotalProcedimento;
                retorno = true;
            }
            
            if (itemConvertido.valorTotal.valorDiarias != valorDiarias)
            {
                itemConvertido.valorTotal.valorDiarias = valorDiarias;
                retorno = true;
            }

            if (itemConvertido.valorTotal.valorTaxasAlugueis != valorTaxasAlugueis)
            {
                itemConvertido.valorTotal.valorTaxasAlugueis = valorTaxasAlugueis;
                retorno = true;
            }

            if (itemConvertido.valorTotal.valorGasesMedicinais != valorGasesMedicinais)
            {
                itemConvertido.valorTotal.valorGasesMedicinais = valorGasesMedicinais;
                retorno = true;
            }

            if (itemConvertido.valorTotal.valorMateriais != valorMateriais)
            {
                itemConvertido.valorTotal.valorMateriais = valorMateriais;
                retorno = true;
            }

            if (itemConvertido.valorTotal.valorOPME != valorOPME)
            {
                itemConvertido.valorTotal.valorOPME = valorOPME;
                retorno = true;
            }

            if (itemConvertido.valorTotal.valorMedicamentos != valorMedicamentos)
            {
                itemConvertido.valorTotal.valorMedicamentos = valorMedicamentos;
                retorno = true;
            }

            if (itemConvertido.valorTotal.valorTotalGeral != valorTotalDeTudo)
            {
                itemConvertido.valorTotal.valorTotalGeral = valorTotalDeTudo;
                retorno = true;
            }

            return retorno;
        }
    }
}
