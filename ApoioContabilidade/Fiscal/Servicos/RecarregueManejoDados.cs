using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassFiltroEdite;
//using PrjApiParceiro_C.AcessosServidor;
//using PrjApiParceiro_C.Config_Componentes;

namespace ApoioContabilidade.Fiscal.Servicos
{
    public class RecarregueManejoDados
    {
        public BindingSource dispLoteBSource; // detalhe
        public BindingSource fazendasBSource; // mestre
        private DataSet odataset;
        private bool inclueZerados = false;
        public bool InclueZerados
        {
            get => inclueZerados;
            set
            {
                inclueZerados = value;
                if ((fazendasBSource != null) && (fazendasBSource.Current != null))
                {
                    try
                    {
                        fazendasBSource.ResetCurrentItem();   }
                    catch (Exception)
                    {
                    }
                    
                }
            }
        }
        private MonteGrid Mestre;
        private MonteGrid Detalhe;
        public RecarregueManejoDados()
        {
            dispLoteBSource = new BindingSource();
            fazendasBSource = new BindingSource();
        }
        
        public async Task<bool> inicieRecarregue(MonteGrid oMestre,
            MonteGrid oDetalhe)
        {
            Mestre = oMestre;
            Detalhe = oDetalhe;
            bool ok = await ClassEventosNF.Recarregue_FazendaDisponivel();
            if (!ok) return ok;
            odataset = ClassEventosNF.odata_disp_Fazendas.Copy();
            dispLoteBSource.DataSource = odataset.Tables["LoteFazenda"];
            fazendasBSource.DataSource = odataset.Tables["SomaSetor"];
            return ok;
        }
        public void fazendas_PositionChanged(object sender, EventArgs e)
        {
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);
            if (registro != null)
            {
                string tsetor = registro["setor"].ToString();
                if (tsetor == " 1") { tsetor = " 2"; }
                if (!inclueZerados)
                {
                    var dado = odataset.Tables["LoteFazenda"].AsEnumerable().Where(row =>
                     ((row.Field<double>("SDOKG_ENT") != 0) && (row.Field<string>("SETOR") == tsetor))).OrderBy(row => row.Field<DateTime>("DTA_ENT"));
                    if ((dado != null) && (dado.Count() > 0))
                    { dispLoteBSource.DataSource = dado.CopyToDataTable(); }
                    else { dispLoteBSource.DataSource = odataset.Tables["LoteFazenda"].Clone(); }

                }
                else
                {
                    var dado = odataset.Tables["LoteFazenda"].AsEnumerable().Where(row =>
                     ((row.Field<string>("SETOR") == tsetor))).OrderBy(row => row.Field<DateTime>("DTA_ENT"));
                    if ((dado != null) && (dado.Count() > 0))
                    { dispLoteBSource.DataSource = dado.CopyToDataTable(); }
                    else { dispLoteBSource.DataSource = odataset.Tables["LoteFazenda"].Clone(); }
                }
            }
            else { dispLoteBSource.DataSource = odataset.Tables["LoteFazenda"].Clone(); }
            Detalhe.FuncaoSoma();
            Detalhe.ssColocaTotais();

        }

        public void SelecaoIndex(int index, bool InclueZerados)

        {
            try
            {
                inclueZerados = InclueZerados;
                switch (index)
                {
                    case 0: // GERAL
                        fazendasBSource.CurrentItemChanged -= fazendas_PositionChanged;
                        fazendasBSource.DataSource = odataset.Tables["SomaSetor"];
                        fazendasBSource.CurrentItemChanged += fazendas_PositionChanged;
                        if ((fazendasBSource.DataSource as DataTable).Rows.Count == 0)
                        {
                            fazendas_PositionChanged(fazendasBSource, null);
                        }
                        fazendasBSource.ResetCurrentItem();
                        Mestre.FuncaoSoma();
                        Mestre.ssColocaTotais();
                      //  Detalhe.FuncaoSoma();
                      //  Detalhe.ColocaTotais();
                        break;
                    // case 1:
                     
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
