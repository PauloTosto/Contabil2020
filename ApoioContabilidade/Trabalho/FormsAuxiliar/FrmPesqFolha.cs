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

namespace ApoioContabilidade.Trabalho.FormsAuxiliar
{
    public partial class FrmPesqFolha : Form
    {
        AtualizaFolha_Cli oAtualFolha = new AtualizaFolha_Cli();
        Eventos_Cad evt_cad;
        DataRowView currenteRow;
        DataTable tabSalariosSoma;
        DataTable tabPontos;
        DataTable tabFerias;

        MonteGrid oSalarios;
        MonteGrid oFaltas;


        string tcod = "";
        public FrmPesqFolha(Eventos_Cad oevento)
        {
            InitializeComponent();
            evt_cad = oevento;
            fim.Value = evt_cad.dataRelatorio;
            inicio.Value = new DateTime(fim.Value.Year, 01, 01);
            currenteRow = (evt_cad.bmMestre.Current as DataRowView);
            tsLabel.Text = currenteRow["CODCAD"].ToString() + " " + currenteRow["NOMECAD"].ToString();
            btnConsulta_Click(tsConsulta, null);
        }
        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            bool ok = await DadosServidor(currenteRow["CODCAD"].ToString(), inicio.Value, fim.Value);
            if (!ok)
            {
                MessageBox.Show("Erro acesso Servidor");
                return;
            }
            tcod = currenteRow["CODCAD"].ToString();
            DataTable adoFaltas = Calcule_NPeriodosDecimo(tcod, inicio.Value, fim.Value);
            

            DataTable adoFerias = Calcule_VlrFeriasnoPeriodoGozo(tcod, inicio.Value, fim.Value);

            DataTable adoPesqFolha = tabSalariosSoma.Clone();
            adoPesqFolha.Columns.Add("VLRFERIAS", Type.GetType("System.Double"));
            adoPesqFolha.Columns.Add("Total_Remun", Type.GetType("System.Double"));

            foreach (DataRow orowSoma in tabSalariosSoma.Rows)
            {
                DataRow orowPesq = adoPesqFolha.NewRow();
                foreach (DataColumn ocol in tabSalariosSoma.Columns)
                {
                    orowPesq[ocol.ColumnName] = orowSoma[ocol.ColumnName];
                }
                double feriasMes = 0;
                try
                {
                    feriasMes = tabFerias.AsEnumerable().Where(row =>
                    (row.Field<DateTime>("DATA").CompareTo(TabelasIniciais_Trab.PrimeiroDiaMes(Convert.ToDateTime(orowSoma["DATA"]))) >= 0)
                  && (row.Field<DateTime>("DATA").CompareTo(TabelasIniciais_Trab.UltimoDiaMes(Convert.ToDateTime(orowSoma["DATA"]))) <= 0)
                     ).Sum(row => row.Field<double>("VlrFerias"));
                }
                catch (Exception)
                {
                    feriasMes = 0;
                }
                orowPesq["VLRFERIAS"] = feriasMes;
                orowPesq["TOTAL_REMUN"] = feriasMes + Convert.ToDouble(orowPesq["BRUTO"]);
                adoPesqFolha.Rows.Add(orowPesq);
                orowPesq.AcceptChanges();
            }

            var soma = (from gr in adoPesqFolha.AsEnumerable()
                        group gr by new
                        {

                            cod = gr.Field<string>("TRAB").Trim()
                        } into g
                        select new
                        {
                           // cod = g.Key.cod,
                            DIAS = g.Sum(row => row.Field<Double>("DIAS")),
                            SALARIO = g.Sum(row => row.Field<Double>("SALARIO")),
                            HEXT50 = g.Sum(row => row.Field<Double>("HEXT50")),
                            VLR_HEXT50 = g.Sum(row => row.Field<Double>("VLR_HEXT50")),
                            HDOM_FER = g.Sum(row => row.Field<Double>("HDOM_FER")),
                            VLRDOM_FER = g.Sum(row => row.Field<Double>("VLRDOM_FER")),
                            ADNOTURNO = g.Sum(row => row.Field<Double>("ADNOTURNO")),

                            GRATIF = g.Sum(row => row.Field<Double>("GRATIF")),
                            BRUTO = g.Sum(row => row.Field<Double>("BRUTO")),

                            VLRFERIAS = g.Sum(row => row.Field<Double>("VLRFERIAS")),

                            TOTAL_REMUN = g.Sum(row => row.Field<Double>("TOTAL_REMUN")),

                        });

            if (soma != null)
            {
                var somaUnica = soma.FirstOrDefault();
                DataRow rowPesq = adoPesqFolha.NewRow();
                rowPesq["ANOMES"] = "Totais ";
                rowPesq["ADNOTURNO"] = somaUnica.ADNOTURNO;

                rowPesq["BRUTO"] = somaUnica.BRUTO;
                rowPesq["DIAS"] = somaUnica.DIAS;
                rowPesq["GRATIF"] = somaUnica.GRATIF;
                rowPesq["HDOM_FER"] = somaUnica.HDOM_FER;
                rowPesq["HEXT50"] = somaUnica.HEXT50;

                rowPesq["SALARIO"] = somaUnica.SALARIO;

                rowPesq["TOTAL_REMUN"] = somaUnica.TOTAL_REMUN;
                rowPesq["VLRDOM_FER"] = somaUnica.VLRDOM_FER;
                rowPesq["VLRFERIAS"] = somaUnica.VLRFERIAS;
                rowPesq["VLR_HEXT50"] = somaUnica.VLR_HEXT50;


                adoPesqFolha.Rows.Add(rowPesq);
                rowPesq.AcceptChanges();
            }
            MonteGrids();
            BindingSource bmFolhaDados = new BindingSource();
            bmFolhaDados.DataSource = adoPesqFolha.AsDataView();
            oSalarios.oDataGridView = dgvFolhas;
            oSalarios.oDataGridView.DataSource = bmFolhaDados;
            oSalarios.ConfigureDBGridView();


            BindingSource bmFaltasDados = new BindingSource();
            bmFaltasDados.DataSource = adoFaltas.AsDataView();
            oFaltas.oDataGridView = dgvFaltas;
            oFaltas.oDataGridView.DataSource = bmFaltasDados;
            oFaltas.ConfigureDBGridView();




        }

        private void MonteGrids()
        {

            oSalarios = new MonteGrid();
            oSalarios.Clear();
            oSalarios.AddValores("ANOMES", "MesAno", 10, "", false, 0, "");
            oSalarios.AddValores("DIAS", "Dias", 6, "##0.00", false, 0, "");
            oSalarios.AddValores("SALARIO", "Salario", 12, "#,###,##0.00", false, 0, "");
            oSalarios.AddValores("HEXT50", "H.Ext.50%", 12, "###,##0.00", false, 0, "");
            oSalarios.AddValores("VLR_HEXT50", "Vlr.H.Ext", 12, "###,##0.00", false, 0, "");

            oSalarios.AddValores("HDOM_FER", "Hs.DomFer", 12, "###,##0.00", false, 0, "");
            oSalarios.AddValores("VLRDOM_FER", "Vlr.H.Dom", 12, "###,##0.00", false, 0, "");
            oSalarios.AddValores("ADNOTURNO", "Adc.Noturno", 12, "###,##0.00", false, 0, "");
            oSalarios.AddValores("GRATIF", "Gratif.", 12, "###,##0.00", false, 0, "");
            oSalarios.AddValores("BRUTO", "Salario Bruto", 12, "####,##0.00", false, 0, "");
            oSalarios.AddValores("VLRFERIAS", "Vlr.Ferias", 12, "####,##0.00", false, 0, "");
            oSalarios.AddValores("TOTAL_REMUN", "Remuneração", 12, "####,##0.00", false, 0, "");


            oFaltas = new MonteGrid();

            oFaltas.AddValores("INICIO", "Inicio", 10, "", false, 0, "");
            oFaltas.AddValores("FIM", "Fim", 10, "", false, 0, "");
            oFaltas.AddValores("DIASMES", "Dias Mes", 10, "##0.0", false, 0, "");
            oFaltas.AddValores("DIASFALTA", "Dias Falta", 10, "##0.0", false, 0, "");
            oFaltas.AddValores("DIASINSS", "Dias INSS", 10, "##0.0", false, 0, "");
            oFaltas.AddValores("DIASTRAB", "Dias Trab.", 10, "##0.00", false, 0, "");
          //  oFaltas.AddValores("DESCONTADECIMO", "Desc.Décimo", 10, "", false, 0, "");
            oFaltas.AddValores("DATAAFASTADO", "Dta Afastado", 13, "", false, 0, "");
            oFaltas.AddValores("DATARETORNO", "Dta Retorno", 13, "", false, 0, "");
            oFaltas.AddValores("ENT_TPSEFIP", "Ent.SEFIP", 10, "", false, 0, "");
            oFaltas.AddValores("SAI_TPSEFIP", "Sai.SEFIP", 10, "", false, 0, "");
        }



        private async Task<bool> DadosServidor(string tcod, DateTime inicio, DateTime fim)
        {
            List<string> lstString = new List<string>();
            string str = "SELECT TRAB,DATA, SUBSTR(DTOS(data),5,2)+'/'+SUBSTR(DTOS(data),1,4) as AnoMes," +
                 " SUM(hx) / 8 AS Dias," +
                 " SUM(salario) AS salario, SUM(hxa) AS HExt50,"
               + " SUM(vlr_hxa) AS Vlr_HExt50, SUM(hxs) AS HDom_Fer, SUM(vlr_hxs) "
               + "  AS vlrDom_Fer, SUM(vlr_hxn) AS adNoturno, SUM(gratif) AS gratif, SUM(salario) + SUM(vlr_hxa) + SUM(vlr_hxs) + SUM(vlr_hxn) + SUM(gratif) AS Bruto "
               + " FROM NOVACLTFOLHA AS CLTFOLHA  WHERE        (trab = '" + tcod + "' ) AND " +
               " (substr(dtos(data), 1, 6) >= '" + inicio.ToString("yyyyMM") +
                        "') AND " + " (substr(dtos(data), 1, 6) <= '" +
                fim.ToString("yyyyMM") + "') " + " GROUP BY  TRAB, DATA";
            lstString.Add(str);
            string strPonto = "SELECT DATA ,PONTO  FROM NOVACLTFOLHA AS CLTFOLHA " + " WHERE        (trab = '" 
                         + tcod + "') AND " +
               " (substr(dtos(data), 1, 6) >= '" + inicio.ToString("yyyyMM") +
                        "') AND " + " (substr(dtos(data), 1, 6) <= '" +
                fim.ToString("yyyyMM") + "') " + " ORDER BY  DATA";
            lstString.Add(strPonto);

            string strFerias = "SELECT COD,DATA,AQUIS_FIM,AQUIS_INI ,GOZO_FIM,GOZO_INI,VLR,TERCVLR FROM FERIAS " +
                " WHERE(COD = '" 
                         + tcod + "') " +
                " ORDER BY AQUIS_INI";
            lstString.Add(strFerias);

            DataSet dsSalarios = null;
            try
            {
                dsSalarios = await ApiServices.Api_QueryMulti(lstString);

            }
            catch (Exception E)
            {
                MessageBox.Show("Erro Acesso Banco de Dados " + E.Message);
                return false;
            }
            if ((dsSalarios == null) || (dsSalarios.Tables.Count == 0))
            {
                MessageBox.Show("Erro Tabelas  não encontradas");
                return false;
            }
            tabSalariosSoma = dsSalarios.Tables[0].Copy();
            tabSalariosSoma.TableName = "SALARIOSSOMA";
            tabSalariosSoma.AcceptChanges();
            tabPontos = dsSalarios.Tables[1].Copy();
            tabPontos.TableName = "PONTOS";
            tabPontos.AcceptChanges();
            tabFerias = dsSalarios.Tables[2].Copy();
            tabFerias.TableName = "FERIAS";
            tabFerias.AcceptChanges();

         
            return true;
        }

        // atualFOlha (função desta classe no DELPHI)
        private DataTable Calcule_NPeriodosDecimo(string tcod, DateTime inicio, DateTime fim)
        {

            DataTable periodosAfastados = TabelaPeriodosAfastados(inicio, fim, tcod);
            DataTable adoPeriodo = CreateAdoPeriodo();
            DateTime tdata = new DateTime(inicio.Year, inicio.Month, inicio.Day);
            while (tdata.CompareTo(fim) <= 0)
            {
                DataRow orow = adoPeriodo.NewRow();
                orow["DiasFalta"] = 0;
                orow["DIASINSS"] = 0;
                orow["DescontaDecimo"] = false;
                orow["DiasTrab"] = 0;
                orow["TRAB"] = tcod;
                orow["Inicio"] = tdata;
                if (TabelasIniciais_Trab.UltimoDiaMes(tdata).CompareTo(fim) > 0)
                {
                    orow["FIM"] = fim;
                }
                else
                {
                    orow["FIM"] = TabelasIniciais_Trab.UltimoDiaMes(tdata);
                }
                tdata = Convert.ToDateTime(orow["FIM"]).AddDays(1);
                orow["DiasMes"] = Convert.ToDateTime(orow["FIM"]).Subtract(Convert.ToDateTime(orow["INICIO"])).Days + 1;
                orow["DATAAFASTADO"] = DBNull.Value;
                orow["DATARETORNO"] = DBNull.Value;
                adoPeriodo.Rows.Add(orow);
                orow.AcceptChanges();
            }
            adoPeriodo.AcceptChanges();
       
            if ((periodosAfastados != null) && (periodosAfastados.Rows.Count > 0))
            {

                foreach (DataRow orowAfast in periodosAfastados.AsEnumerable().OrderBy(r => r.Field<DateTime>("INICIO")))
                {
                    DateTime ini_afast = Convert.ToDateTime(orowAfast["INICIO"]);
                    DateTime fim_afast = Convert.ToDateTime(orowAfast["FIM"]);
                    foreach (DataRow orow in adoPeriodo.AsEnumerable().Where(row =>
                        (
                          // inicia afastam..
                          row.Field<DateTime>("INICIO").CompareTo(ini_afast) >= 0
                          && row.Field<DateTime>("INICIO").CompareTo(fim_afast) <= 0)
                          // todo dentro
                          || (row.Field<DateTime>("INICIO").CompareTo(ini_afast) >= 0
                          && row.Field<DateTime>("FIM").CompareTo(fim_afast) <= 0)
                          // encerra afastam..
                          || (row.Field<DateTime>("FIM").CompareTo(ini_afast) >= 0
                          && row.Field<DateTime>("FIM").CompareTo(fim_afast) <= 0)
                        ))
                    {
                        orow.BeginEdit();
                        orow["DATARETORNO"] = fim_afast;
                        orow["DATAAFASTADO"] = ini_afast;
                        orow["ENT_TPSEFIP"] = orowAfast["ENT_TPSEFIP"];
                        orow["SAI_TPSEFIP"] = orowAfast["SAI_TPSEFIP"];
                        DateTime fim_afastPeriodo = Convert.ToDateTime(orow["FIM"]);
                        DateTime inicio_afastPeriodo = Convert.ToDateTime(orow["INICIO"]);
                        if (inicio_afastPeriodo.CompareTo(ini_afast) < 0)
                        {
                            inicio_afastPeriodo = ini_afast;
                        }
                        if (fim_afastPeriodo.CompareTo(fim_afast) > 0)
                        {
                            fim_afastPeriodo = fim_afast;
                        }
                        orow["DIASINSS"] = Convert.ToInt32(orow["DIASINSS"]) + fim_afastPeriodo.Subtract(inicio_afastPeriodo).Days + 1;
                        orow.EndEdit();
                        orow.AcceptChanges();
                    }
                }
                int faltasper = 0;
                foreach (DataRow orow in adoPeriodo.Rows)
                {
                    if (Convert.ToInt32(orow["DIASINSS"]) == Convert.ToInt32(orow["DIASMES"]))
                    {
                        continue;
                    }
                    int num_faltas = 0;
                    DateTime fim_Periodo = Convert.ToDateTime(orow["FIM"]);
                    DateTime inicio_Periodo = Convert.ToDateTime(orow["INICIO"]);

                    foreach (DataRow oRowPonto in tabPontos.AsEnumerable().Where(row =>
                                 (row.Field<DateTime>("DATA").CompareTo(inicio_Periodo) >= 0)
                                 && (row.Field<DateTime>("DATA").CompareTo(fim_Periodo) <= 0)))
                    {

                        faltasper = DevolveFaltas(oRowPonto["PONTO"].ToString());
                        num_faltas = num_faltas + faltasper;

                        /* MISTERIO (inicio e fim são datas vazias no original do DELPHI??????????
                         * tret := false;
                            if formatDateTime('yyyymm', FieldbyName('DATA').AsDateTime)
                              = formatDateTime('yyyymm', inicio) then
                            begin
                              decodedate(inicio, aa, mm, dd);
                              faltasper := DevolveFaltas(copy(FieldbyName('PONTO').asString,
                                (dd * 2) - 1));
                              num_faltas := num_faltas + faltasper;

                              tret := true;
                            end;
                            if formatDateTime('yyyymm', FieldbyName('DATA').AsDateTime)
                              = formatDateTime('yyyymm', fim) then
                            begin
                              decodedate(inicio, aa, mm, dd);
                              faltasper := DevolveFaltas(copy(FieldbyName('PONTO').asString, 1,
                                (dd * 2)));
                              num_faltas := num_faltas + faltasper;

                              tret := true;
                            end; */
                    }

                    orow.BeginEdit();
                    orow["DiasFalta"] = num_faltas;
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
            }
            return adoPeriodo;
        }

        private DataTable CreateAdoPeriodo()
        {
            DataTable adoPerido = new DataTable();
            adoPerido.Columns.Add("TRAB", Type.GetType("System.String"));
            adoPerido.Columns[adoPerido.Columns.Count - 1].MaxLength = 4;
            adoPerido.Columns.Add("INICIO", Type.GetType("System.DateTime"));
            adoPerido.Columns.Add("FIM", Type.GetType("System.DateTime"));
            adoPerido.Columns.Add("DIASMES", Type.GetType("System.Int32"));
            adoPerido.Columns.Add("DIASFALTA", Type.GetType("System.Int32"));
            adoPerido.Columns.Add("DIASINSS", Type.GetType("System.Int32"));
            adoPerido.Columns.Add("DIASTRAB", Type.GetType("System.Int32"));
            adoPerido.Columns.Add("DESCONTADECIMO", Type.GetType("System.Boolean"));
            adoPerido.Columns.Add("DataAfastado", Type.GetType("System.DateTime"));
            adoPerido.Columns.Add("DataRetorno", Type.GetType("System.DateTime"));
            adoPerido.Columns.Add("ENT_TPSEFIP", Type.GetType("System.String"));
            adoPerido.Columns[adoPerido.Columns.Count - 1].MaxLength = 2;
            adoPerido.Columns.Add("SAI_TPSEFIP", Type.GetType("System.String"));
            adoPerido.Columns[adoPerido.Columns.Count - 1].MaxLength = 2;
            adoPerido.TableName = "PERIODOAFAST";


            return adoPerido;
        }

        private int DevolveFaltas(string tponto)
        {
            int result = 0;
            tponto = tponto.Trim();
            int cont = tponto.Length;
            int i = 0;
            while (i < cont - 1)
            {
                if ((tponto.Substring(i, 2) == "FI") || (tponto.Substring(i, 2) == "  "))
                {
                    result++;
                }
                i = i + 2;
            }
            return result;
        }
        private int DevolveFaltasINSS(string tponto)
        {
            int result = 0;
            tponto = tponto.Trim();
            int cont = tponto.Length;
            int i = 0;
            while (i < cont - 1)
            {
                if ((tponto.Substring(i, 2) == "AP") || (tponto.Substring(i, 2) == "AD")
                    || (tponto.Substring(i, 2) == "AT") || (tponto.Substring(i, 2) == "AI")
                    )
                {
                    result++;
                }
                i = i + 2;
            }
            return result;
        }

        private DataTable TabelaPeriodosAfastados(DateTime data1, DateTime data2, string cod)
        {
            //OBSSEFIP
            DataTable dadosSefip = new DataTable();
            
            try
            {
                var  dados = TabelasIniciais_Trab.dsTabelasTrab.Tables["OBSSEFIP"].AsEnumerable().Where(row => row.Field<string>("COD").Trim() == cod.Trim());
                if (dados != null)
                    dadosSefip = dados.AsDataView().ToTable().Copy();
                else
                    dadosSefip = null;
            }
            catch (Exception E)
            {
                dadosSefip = null;
            }

            if ((dadosSefip == null) || (dadosSefip.Rows.Count == 0)) return dadosSefip;
            DataTable adoPeriodoAfastado = new DataTable();
            adoPeriodoAfastado.Columns.Add("COD", Type.GetType("System.String"));
            adoPeriodoAfastado.Columns[adoPeriodoAfastado.Columns.Count - 1].MaxLength = 4;
            adoPeriodoAfastado.Columns.Add("INICIO", Type.GetType("System.DateTime"));
            adoPeriodoAfastado.Columns.Add("FIM", Type.GetType("System.DateTime"));
            adoPeriodoAfastado.Columns.Add("ENT_TPSEFIP", Type.GetType("System.String"));
            adoPeriodoAfastado.Columns[adoPeriodoAfastado.Columns.Count - 1].MaxLength = 2;
            adoPeriodoAfastado.Columns.Add("SAI_TPSEFIP", Type.GetType("System.String"));
            adoPeriodoAfastado.Columns[adoPeriodoAfastado.Columns.Count - 1].MaxLength = 2;

            adoPeriodoAfastado.Columns.Add("DIAS_ATESTADO", Type.GetType("System.Int32"));
            adoPeriodoAfastado.Columns.Add("DIAS_AFASTADOS", Type.GetType("System.Int32"));
            try
            {
                for (int i = 0; i < dadosSefip.Rows.Count; i++)
                {
                    DataRow orow = dadosSefip.Rows[i];

                    if ((orow["TPSEFIP"].ToString() == "Q1")
                        || (orow["TPSEFIP"].ToString() == "Z1")
                        ) continue;

                    // só interessam os registros cuja datas indiquem uma situação de afastamento
                    // ou retorno dentro do periodo
                    // os afastamentos anteriores ao inicio do periodo devem constar da lista
                    // se houver um retorno antes do periodo desejado, todos os eventos anteriores são
                    // irrelevante para esta análise
                    if ((orow["TIPO"].ToString().Trim() == "E")
                         && (Convert.ToDateTime(orow["DATA"]).CompareTo(data1) < 0)
                         ) break;
                    // inicia a inserção , necessáriamente deve ser uma saida (seja dentro do periodo ou não

                    if ((orow["TIPO"].ToString().Trim() == "S"))
                    {
                        DataRow orowNovo = adoPeriodoAfastado.NewRow();

                        orowNovo["COD"] = orow["COD"];
                        orowNovo["SAI_TPSEFIP"] = orow["TPSEFIP"];
                        orowNovo["INICIO"] = orow["DATA_SAI"];
                        orowNovo["DIAS_ATESTADO"] = 0;
                        if ((!orow.IsNull("DATA_SAI")) &&
                            (Convert.ToDateTime(orow["DATA_SAI"]).CompareTo(Convert.ToDateTime(orow["DATA"])) > 0)
                            )
                        {
                            int dias_atestado = Convert.ToDateTime(orow["DATA_SAI"]).Subtract(Convert.ToDateTime(orow["DATA"])).Days;
                            orowNovo["DIAS_ATESTADO"] = dias_atestado;
                        }
                        adoPeriodoAfastado.Rows.Add(orowNovo);
                        orowNovo.AcceptChanges();
                    }
                    else if ((orow["TIPO"].ToString().Trim() == "E"))
                    {
                        DataRow orowNovo = adoPeriodoAfastado.NewRow();

                        orowNovo["COD"] = orow["COD"];
                        orowNovo["ENT_TPSEFIP"] = orow["TPSEFIP"];
                        orowNovo["FIM"] = orow["DATA"];
                        adoPeriodoAfastado.Rows.Add(orowNovo);
                        orowNovo.AcceptChanges();
                        if (i == dadosSefip.Rows.Count - 1)
                        {
                            MessageBox.Show("Entrada sem Saida??? :" + orowNovo["COD"].ToString());
                            continue;
                        }
                        i++;
                        orow = dadosSefip.Rows[i];
                        orowNovo.BeginEdit();
                        orowNovo["SAI_TPSEFIP"] = orow["TPSEFIP"];
                        orowNovo["INICIO"] = orow["DATA_SAI"];
                        orowNovo["DIAS_ATESTADO"] = 0;
                        if ((!orow.IsNull("DATA_SAI")) &&
                            (Convert.ToDateTime(orow["DATA_SAI"]).CompareTo(Convert.ToDateTime(orow["DATA"])) > 0)
                            )
                        {
                            int dias_atestado = Convert.ToDateTime(orow["DATA_SAI"]).Subtract(Convert.ToDateTime(orow["DATA"])).Days;
                            orowNovo["DIAS_ATESTADO"] = dias_atestado;
                        }
                        orowNovo.EndEdit();
                    }

                }


            }
            catch (Exception E)
            {
                MessageBox.Show("Erro Analise OBSSEFIP" + E.Message);
                return adoPeriodoAfastado = null;


            }
            if (adoPeriodoAfastado.Rows.Count == 0)
            { return adoPeriodoAfastado = null; }

            foreach (DataRow orow in adoPeriodoAfastado.Rows)
            {
                if (orow.IsNull("FIM"))
                {
                    orow.BeginEdit();
                    orow["FIM"] = DateTime.MaxValue;
                    orow.EndEdit();
                }
                else if (Convert.ToDateTime(orow["FIM"]).CompareTo(inicio.Value) <= 0)
                {
                    orow.Delete();
                }
            }
            adoPeriodoAfastado.AcceptChanges();

            return adoPeriodoAfastado;
        }

        private DataTable Calcule_VlrFeriasnoPeriodoGozo(string tcod, DateTime inicio, DateTime fim)
        {
            DataTable result = new DataTable();
            result.Columns.Add("COD", Type.GetType("System.String"));
            result.Columns[result.Columns.Count - 1].MaxLength = 4;
            result.Columns.Add("DATA", Type.GetType("System.DateTime"));
            result.Columns.Add("GOZO_INI", Type.GetType("System.DateTime"));
            result.Columns.Add("GOZO_FIM", Type.GetType("System.DateTime"));
            result.Columns.Add("AQUIS_INI", Type.GetType("System.DateTime"));
            result.Columns.Add("AQUIS_FIM", Type.GetType("System.DateTime"));
            result.Columns.Add("VLRFERIAS", Type.GetType("System.Double"));
            result.Columns.Add("VLR", Type.GetType("System.Double"));
            result.Columns.Add("TERCVLR", Type.GetType("System.Double"));
            result.Columns.Add("DIASGOZO", Type.GetType("System.Int32"));

            DateTime umMesAntes = new DateTime(inicio.Year, inicio.Month, 01).AddDays(-1);
            DateTime PrimeiroumMesAntes = new DateTime(umMesAntes.Year, umMesAntes.Month, 01);
            DateTime ultimoDiaFim = TabelasIniciais_Trab.UltimoDiaMes(fim);

            foreach(DataRow orowFe in tabFerias.AsEnumerable().Where(row =>
                          // inicia afastam..
                         ( row.Field<DateTime>("GOZO_INI").CompareTo(PrimeiroumMesAntes) >= 0)
                          && (row.Field<DateTime>("GOZO_INI").CompareTo(ultimoDiaFim) <= 0 ) )  )
            {
                DateTime per_Ini = Convert.ToDateTime(orowFe["GOZO_INI"]);
                DateTime per_Fim = Convert.ToDateTime(orowFe["GOZO_FIM"]);
                if (per_Ini.CompareTo(umMesAntes) < 0)
                {
                    per_Ini = inicio;
                    if (per_Fim.CompareTo(umMesAntes) < 0)
                    { continue; }
                }
                if (per_Fim.CompareTo(fim) > 0)
                {
                    per_Fim = fim;
                    if (per_Ini.CompareTo(fim) > 0)
                    { continue; }
                }
                // se o gozo for no mesmo mes
                if (Convert.ToDateTime(orowFe["GOZO_INI"]).ToString("yyyyMM") == Convert.ToDateTime(orowFe["GOZO_FIM"]).ToString("yyyyMM"))
                {
                    DataRow rowResult = result.NewRow();
                    rowResult["DATA"] = TabelasIniciais_Trab.UltimoDiaMes(Convert.ToDateTime(orowFe["GOZO_INI"]));
                    rowResult["GOZO_INI"] = orowFe["GOZO_INI"];
                    rowResult["GOZO_FIM"] = orowFe["GOZO_FIM"];
                    rowResult["AQUIS_INI"] = orowFe["AQUIS_INI"];
                    rowResult["AQUIS_FIM"] = orowFe["AQUIS_FIM"];

                    rowResult["COD"] = orowFe["COD"];
                    rowResult["VLR"] = orowFe["VLR"];
                    rowResult["TERCVLR"] = orowFe["TERCVLR"];
                    rowResult["VLRFERIAS"] = Convert.ToDouble(orowFe["VLR"]) + Convert.ToDouble(orowFe["TERCVLR"]);
                    rowResult["DIASGOZO"] = per_Fim.Subtract(per_Ini).Days + 1;
                    result.Rows.Add(rowResult);
                    rowResult.AcceptChanges();
                    continue;
                }
                // data do inicio do gozo_ está num mes e data do fim do gozo em outro mes
                // verifica se a data do inicio do gozo está dentro do periodo pesquisado
                int dias_gozoini = TabelasIniciais_Trab.UltimoDiaMes(Convert.ToDateTime(orowFe["GOZO_INI"])).Subtract(
                          Convert.ToDateTime(orowFe["GOZO_INI"])).Days + 1;
                int dias_gozofim = Convert.ToDateTime(orowFe["GOZO_FIM"]).Subtract(
                          TabelasIniciais_Trab.PrimeiroDiaMes(Convert.ToDateTime(orowFe["GOZO_FIM"]))).Days + 1;
                int diasTotal = Convert.ToDateTime(orowFe["GOZO_FIM"]).Subtract(
                          Convert.ToDateTime(orowFe["GOZO_INI"])).Days + 1;
                double fator = dias_gozoini / diasTotal;
                double fator2 = dias_gozofim / diasTotal;
                double totalFe = Convert.ToDouble(orowFe["VLR"]) + Convert.ToDouble(orowFe["TERCVLR"]);
                double vlrFerias = Math.Round(fator* totalFe);
                double vlrFerias2 = Math.Round(fator2 * totalFe);
                double sobra= totalFe - (vlrFerias + vlrFerias2);
                if (sobra != 0)
                {
                    if (fator >= fator2)
                    {
                        vlrFerias = vlrFerias + sobra;
                        sobra = 0;
                    }
                    else
                    {
                        vlrFerias2 = vlrFerias2 + sobra;
                        sobra = 0;
                    }
                }
                if (Convert.ToDateTime(orowFe["GOZO_INI"]).CompareTo(inicio) >= 0 )
                {
                    // se está dentro do periodo pesquisado
                    DataRow rowResult = result.NewRow();
                    rowResult["DATA"] = TabelasIniciais_Trab.UltimoDiaMes(Convert.ToDateTime(orowFe["GOZO_INI"]));
                    rowResult["GOZO_INI"] = orowFe["GOZO_INI"];
                    rowResult["GOZO_FIM"] = orowFe["GOZO_FIM"];
                    rowResult["AQUIS_INI"] = orowFe["AQUIS_INI"];
                    rowResult["AQUIS_FIM"] = orowFe["AQUIS_FIM"];

                    rowResult["COD"] = orowFe["COD"];
                    rowResult["VLR"] = orowFe["VLR"];
                    rowResult["TERCVLR"] = orowFe["TERCVLR"];
                    rowResult["VLRFERIAS"] = vlrFerias;
                    rowResult["DIASGOZO"] = dias_gozoini;
                    result.Rows.Add(rowResult);
                    rowResult.AcceptChanges();
                }
                if (Convert.ToDateTime(orowFe["GOZO_FIM"]).CompareTo(fim) <= 0)
                {
                    // se está dentro do periodo pesquisado
                    DataRow rowResult = result.NewRow();
                    rowResult["DATA"] = TabelasIniciais_Trab.UltimoDiaMes(Convert.ToDateTime(orowFe["GOZO_FIM"]));
                    rowResult["GOZO_INI"] = orowFe["GOZO_INI"];
                    rowResult["GOZO_FIM"] = orowFe["GOZO_FIM"];
                    rowResult["AQUIS_INI"] = orowFe["AQUIS_INI"];
                    rowResult["AQUIS_FIM"] = orowFe["AQUIS_FIM"];

                    rowResult["COD"] = orowFe["COD"];
                    rowResult["VLR"] = orowFe["VLR"];
                    rowResult["TERCVLR"] = orowFe["TERCVLR"];
                    rowResult["VLRFERIAS"] = vlrFerias2;
                    rowResult["DIASGOZO"] = dias_gozofim;
                    result.Rows.Add(rowResult);
                    rowResult.AcceptChanges();
                }
            }
            return result;
        }

        /*private DateTime UltimoDiaMes(DateTime data)
        {
            DateTime result =
                new DateTime(data.Year, data.Month, 01).AddMonths(1).AddDays(-1);
           
            return result;
        }
        private DateTime PrimeiroDiaMes(DateTime data)
        {
            DateTime result =
                new DateTime(data.Year, data.Month, 01);

            return result;
        }
        */

        /*

        */




        /*
         * 
         */








    }
}
