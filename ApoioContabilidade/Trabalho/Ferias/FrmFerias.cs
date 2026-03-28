using ApoioContabilidade.Models;
using ApoioContabilidade.Trabalho.ServicesTrab;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho.Ferias
{
    public partial class FrmFerias : Form
    {

        MonteGrid oFiltro;
        List<string> CamposExcluidos;
        DateTime DataMes;
        bool pesquisaGozo;
        DataTable tabFerias;
        public FrmFerias(DateTime oDataMes, bool oPesquisaGozo)
        {
            InitializeComponent();
            DataMes = oDataMes;
            pesquisaGozo = oPesquisaGozo;
            Inicializa();
        }

        private void Inicializa()
        {
             CamposExcluidos = new List<string>();
            CamposExcluidos.Add("CCENTRO");
            CamposExcluidos.Add("NOMECAD");
            CamposExcluidos.Add("CARTTRAB");
            CamposExcluidos.Add("SERIE");
            CamposExcluidos.Add("NUMERO");
            CamposExcluidos.Add("CODCAD");
            CamposExcluidos.Add("LANCADO");
            CamposExcluidos.Add("PAGO");

        }

        private async void CarregaDados()
        {
            bool retorno = await DadosServidor();
            if (!retorno) { return; }
            MonteGrids();

        }

        private async Task<bool> DadosServidor()
        {
            DateTime Data1 = TabelasIniciais_Trab.PrimeiroDiaMes(DataMes);
            string str = "";
            if (pesquisaGozo)
            {
                str = "SELECT Ferias.*,CLTCAD.GLECAD as cCentro,CLTCAD.CODCAD,CLTCAD.NOMECAD," +
                    "CLTCAD.CARTTRAB,CLTCAD.SERIE,CLTCAD.NUMERO, false as LANCADO,false as PAGO    FROM Ferias,CLTCAD where " +
                 " CLTCAD.CODCAD = Ferias.COD AND " +
                 " SUBSTRING(dbo.DTOS(GOZO_INI),1,6) = '" +
                 Data1.ToString("yyyyMM") + "' ORDER BY GOZO_INI";
            }
            else
            {
               str = "SELECT Ferias.*,CLTCAD.GLECAD as CCentro,CLTCAD.CODCAD,CLTCAD.NOMECAD,CLTCAD.CARTTRAB," +
                    "CLTCAD.SERIE,CLTCAD.NUMERO, false as LANCADO, false as PAGO FROM Ferias,CLTCAD where " +
               " CLTCAD.CODCAD = Ferias.COD AND " +
                 "SUBSTR(DTOS(DATA),1,6) = '" +
                Data1.ToString("yyyyMM") + "' ORDER BY DATA";
            }
            List<string> lstString = new List<string>();
            lstString.Add(str);
            DataSet dsFerias = null;
            try
            {
                dsFerias = await ApiServices.Api_QueryMulti(lstString);

            }
            catch (Exception E)
            {
                MessageBox.Show("Erro Acesso Banco de Dados " + E.Message);
                return false;
            }
            if ((dsFerias == null) || (dsFerias.Tables.Count == 0))
            {
                MessageBox.Show("Erro Tabela FErias  não encontradas");
                return false;
            }
            tabFerias = dsFerias.Tables[0].Copy();
            tabFerias.TableName = "FERIAS";
            tabFerias.AcceptChanges();
            return true;
        }

        private async void FrmFerias_Load(object sender, EventArgs e)
        {
            bool carrega = false;
            if (!TabelasIniciais_Trab.TabelasIniciaisOk())
            {
                try
                {
                    carrega = await TabelasIniciais_Trab.Execute();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                    throw;
                }
            }
            else
            {
                carrega = true;
            }

            if (carrega) { CarregaDados(); }
        }
        


        private void MonteGrids()
        {

            oFiltro = new MonteGrid();

            /*
             
              DBGrid1.Columns.Clear;
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('Data');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('Cod');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('cCentro');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('AQUIS_INI');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('AQUIS_FIM');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('DIASABONO');

  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('GOZO_INI');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('GOZO_FIM');

  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('SALBASE');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('MEDIA');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('TIPO');

  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('Vlr');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('TERCVlr');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('SalCont');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('VLRABONO');


  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('FERIAS_BASE');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('FERIAS_MEDIA');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('ABONO_PEC_BASE');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('ABONO_PEC_MEDIA');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('ABONO_TERC');
 

  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('SalFam');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('INss');
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('IRF');

  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('VLRLIQ'); //cLiquido
  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('FGTS');

  DBGrid1.Columns.Add.Field  := dsFerias.DataSet.FieldByName('FALTAS');


  DBGrid1.Columns.Items[0].Title.Caption := 'Pagamento';
  DBGrid1.Columns.Items[0].Width := 6*12;


  DBGrid1.Columns.Items[1].Title.Caption := 'Nome do Trabalhador';
  DBGrid1.Columns.Items[1].Width := 6*35;
   DBGrid1.Columns.Items[1].ReadOnly := true;

  DBGrid1.Columns.Items[2].Title.Caption := 'Centro';
  DBGrid1.Columns.Items[2].Width := 6*5;
   DBGrid1.Columns.Items[1].ReadOnly := true;

  DBGrid1.Columns.Items[3].Title.Caption := 'Aquis.(ini)';
  DBGrid1.Columns.Items[3].Width := 6*12;
 DBGrid1.Columns.Items[3].ReadOnly := true;

  DBGrid1.Columns.Items[4].Title.Caption := 'Aquis.(fim)';
  DBGrid1.Columns.Items[4].Width := 6*12;
 DBGrid1.Columns.Items[4].ReadOnly := true;
 //

  DBGrid1.Columns.Items[5].Title.Caption := 'Abono';
  DBGrid1.Columns.Items[5].Width := 6*5;


  DBGrid1.Columns.Items[6].Title.Caption := 'Gozo(ini)';
  DBGrid1.Columns.Items[6].Width := 6*12;
  DBGrid1.Columns.Items[6].ButtonStyle := cbsEllipsis;

  DBGrid1.Columns.Items[7].Title.Caption := 'Gozo(fim)';
  DBGrid1.Columns.Items[7].Width := 6*12;

  DBGrid1.Columns.Items[8].Title.Caption := 'Salário Base';
  DBGrid1.Columns.Items[8].Width := 6*13;
 DBGrid1.Columns.Items[8].ReadOnly := true;

  DBGrid1.Columns.Items[9].Title.Caption := 'Média Salários';
  DBGrid1.Columns.Items[9].Width := 6*13;
 DBGrid1.Columns.Items[9].ReadOnly := true;

  DBGrid1.Columns.Items[10].Title.Caption := 'Base Cálculo';
  DBGrid1.Columns.Items[10].Width := 6*13;

  DBGrid1.Columns.Items[10].PickList.Add('Salário Base');
  DBGrid1.Columns.Items[10].PickList.Add('Média Salários');




  DBGrid1.Columns.Items[11].Title.Caption := 'Remun. Ferias';
  DBGrid1.Columns.Items[11].Width := 6*13;
  DBGrid1.Columns.Items[11].ReadOnly := true;

  DBGrid1.Columns.Items[12].Title.Caption := 'Adic. Terço';
  DBGrid1.Columns.Items[12].Width := 6*13;
  DBGrid1.Columns.Items[12].ReadOnly := true;

  DBGrid1.Columns.Items[13].Title.Caption := 'Sal. Contrib.';
  DBGrid1.Columns.Items[13].Width := 6*13;
  DBGrid1.Columns.Items[13].ReadOnly := true;

  DBGrid1.Columns.Items[14].Title.Caption := 'Abono Pec.';
  DBGrid1.Columns.Items[14].Width := 6*13;
  DBGrid1.Columns.Items[14].ReadOnly := true;


  DBGrid1.Columns.Items[15].Title.Caption := 'Ferias(Base)';
  DBGrid1.Columns.Items[15].Width := 6*13;
  DBGrid1.Columns.Items[15].ReadOnly := true;

  DBGrid1.Columns.Items[16].Title.Caption := 'Ferias(Me)';
  DBGrid1.Columns.Items[16].Width := 6*13;
  DBGrid1.Columns.Items[16].ReadOnly := true;

  DBGrid1.Columns.Items[17].Title.Caption := 'Abono(Base)';
  DBGrid1.Columns.Items[17].Width := 6*13;
  DBGrid1.Columns.Items[17].ReadOnly := true;

  DBGrid1.Columns.Items[18].Title.Caption := 'Abono(Me)';
  DBGrid1.Columns.Items[18].Width := 6*13;
  DBGrid1.Columns.Items[18].ReadOnly := true;

  DBGrid1.Columns.Items[19].Title.Caption := 'Abono(1/3)';
  DBGrid1.Columns.Items[19].Width := 6*13;
  DBGrid1.Columns.Items[19].ReadOnly := true;





  DBGrid1.Columns.Items[20].Title.Caption := 'Sal. Fam.';
  DBGrid1.Columns.Items[20].Width := 6*11;
 DBGrid1.Columns.Items[20].ReadOnly := true;

  DBGrid1.Columns.Items[21].Title.Caption := ' I N S S';
  DBGrid1.Columns.Items[21].Width := 6*11;
 DBGrid1.Columns.Items[21].ReadOnly := true;

  DBGrid1.Columns.Items[22].Title.Caption := ' IRFONTE';
  DBGrid1.Columns.Items[22].Width := 6*11;
 DBGrid1.Columns.Items[22].ReadOnly := true;

  DBGrid1.Columns.Items[23].Title.Caption := 'Vlr. Liquido';
  DBGrid1.Columns.Items[23].Width := 6*13;
 DBGrid1.Columns.Items[23].ReadOnly := true;

  DBGrid1.Columns.Items[24].Title.Caption := 'Vlr.FGTS ';
  DBGrid1.Columns.Items[24].Width := 6*13;
  DBGrid1.Columns.Items[24].ReadOnly := true;

  DBGrid1.Columns.Items[25].Title.Caption := 'Faltas';
  DBGrid1.Columns.Items[25].Width := 6*8;
 DBGrid1.Columns.Items[25].ReadOnly := true;

             
             */


        }


    }
}
