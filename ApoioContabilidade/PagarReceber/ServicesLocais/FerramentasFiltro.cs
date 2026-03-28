using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.PagarReceber.ServicesLocais
{
    static public class FerramentasFiltro
    {
        // originalmente no TDataControlContab
        static public Boolean fPassa_Contab2(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            bool classificado = false;
            
            if ((oLinha.dado[0] == "") || (oLinha.dado[0] == "*")) return classificado;
           
            if (opesquisa == null)
            {
                if ((Boolean)odatarow["TP_FIN"] == true)
                {
                    if ((string)odatarow["TIPO"] == "P")
                    {
                        if (((EBancoNormalizado((string)odatarow["CREDITO"], opesquisa, true)))//|| (((string)odatarow["CREDITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                            classificado = true;
                    }
                    else
                        if (((EBancoNormalizado((string)odatarow["DEBITO"], opesquisa, true)))// || (((string)odatarow["DEBITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
                else
                {
                    if (((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])) || (((string)odatarow["CREDITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                        classificado = true;
                    if ((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"]) || (((string)odatarow["DEBITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
            }
            else
            {
                if ((Boolean)odatarow["TP_FIN"] == true)
                {
                    if ((string)odatarow["TIPO"] == "P")
                    {                                          //  || (((string)odatarow["CREDITO"]).Trim() == "00"))
                        if (((EBancoNormalizado((string)odatarow["CREDITO"], opesquisa, true)))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                            classificado = true;
                    }
                    else
                        if (((EBancoNormalizado((string)odatarow["DEBITO"], opesquisa, true)))//|| (((string)odatarow["DEBITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
                else
                {
                    if (((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])) || (((string)odatarow["CREDITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                        classificado = true;
                    if ((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"]) || (((string)odatarow["DEBITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
            }
            return classificado;

        }
        static public bool EBancoNormalizado(string conta, PesquisaGenerico opesquise, Boolean contabil)
        {
            Boolean result = false;
            if (conta.Trim().Length != 2) return result;
            int intconta;
            try
            { intconta = Convert.ToInt16(conta.Trim()); }
            catch
            {
                return result;
            }
            if (intconta == 0) return result;
            conta = intconta.ToString("D2");

            DataRow[] linhas = opesquise.PesqLinhas("BANCOS", new string[1] { "NBANCO" }, new string[1] { conta });
            if (linhas.Length == 0) return result;
            if (contabil && (Convert.ToString(linhas[0]["CONTAB"]).Trim() == ""))
                return result;
            return true;
        }

    }
}
