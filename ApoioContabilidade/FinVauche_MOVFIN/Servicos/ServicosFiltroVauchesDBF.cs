using ApoioContabilidade.PagarReceber.ServicesLocais;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.FinVauche_MOVFIN.Servicos
{

    static public class FiltroVauchesDBF
    {
        // 

        static public DataSet dsFinanceiro;
        // static public DataSet dsFiltrado;

        static public OleDbDataAdapter adapterMovFin;


        static public string path;

        static public bool PesquisaServidor(DateTime inicio, DateTime fim, PesquisaGenerico oPesquisa, List<LinhaSolucao> oList)
        {
            bool ret = true;
            path = TDataControlReduzido.Get_Path("CONTAB");
            if ((fim == null) || (fim.CompareTo(inicio) < 0)) fim = inicio;

            dsFinanceiro = new DataSet();

            DataSet dataSet1 = TDataControlContab.MovFinanVauches(inicio, fim, oList, oPesquisa);

            if (dataSet1.Tables.Count > 0)
            {
                dsFinanceiro.Tables.Add(dataSet1.Tables[0].Copy());
                dsFinanceiro.Tables[0].TableName = "MOVFIN";
                dsFinanceiro.Tables[0].AcceptChanges();
            }

            return ret;
        }

    }
}





