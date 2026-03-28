using ApoioContabilidade.Models;
using ClassFiltroEdite;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho
{
    public partial class FrmPontoMes : Form
    {
        System.Data.DataTable dtPonto;
        System.Data.DataTable dtCLTCodigo;
        System.Data.DataTable dtTrabalhadores;

        private MonteGrid oPonto;

        public int LinhasMinimas = 3;
        private string[] Letras = new string[52] {"A","B","C","D","E","F","G","H","I","J","K",
                                  "L","M","N","O","P","Q","R","S","T","U"
                                   ,"V","W","X","Y","Z","AA","AB","AC","AD","AE"
                                   ,"AF","AG","AH","AI","AJ","AK","AL","AM","AN","AO"
                                   ,"AP","AQ","AR","AS","AT","AU","AV","AW","AX","AY","AZ"
                                     };



        //public string tituloExcel = "";
        public int linhasInicioExcel = 4;
        public int colunaCabecalhoExcel = 0;
        public BindingSource bmSource;

        DateTime inicio, fim;

        public FrmPontoMes()
        {
            InitializeComponent();
            
            oPonto= new MonteGrid();
            btnExcel.Enabled = false;
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            btnExcel.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            inicio = new DateTime(dtData1.Value.Year, dtData1.Value.Month, 1);

            fim = new DateTime(dtData1.Value.Year, dtData1.Value.Month, dtData1.Value.Day);
            if (fim.AddDays(1).Month == dtData1.Value.Month)
            {
                MessageBox.Show("Não é o ultimo dia do Mes");
                return;
            }
            // store procedure 224 (dbo.Ponto_mes)
            DataSet dsResult = null;
            string str = "";
            List<string> lstquery = new List<string>();
            lstquery.Add("inicio=" + inicio.ToString("yyyy-MM-dd"));
            lstquery.Add("fim=" + fim.ToString("yyyy-MM-dd"));
            lstquery.Add("sp_numero=" + "224");

            lstquery.Add(str);
            try
            {
                dsResult = await ApiServices.Api_QuerySP(lstquery);
            }
            catch (Exception)
            {
                MessageBox.Show("ErroAcesso");
                Cursor.Current = Cursors.Default;
                return;
            }
            if ((dsResult != null) && (dsResult.Tables.Count > 0))
            {

                dtPonto = dsResult.Tables[0].Copy();
                dtPonto.TableName = "PONTOMES";
                dtCLTCodigo = dsResult.Tables[1].Copy();
                dtCLTCodigo.TableName = "CODIGOS";
                btnExcel.Enabled = true;
            }
            else
            {
                MessageBox.Show("Sem Resultados");
            }

            Cursor.Current = Cursors.Default;
        }

        private System.Data.DataTable Reload()
        {
            Cursor.Current = Cursors.WaitCursor;

            dtTrabalhadores = new System.Data.DataTable();
            try
            {

                dtTrabalhadores.TableName = "TRABALHADORES";

                dtTrabalhadores.Columns.Add("CODIGO", Type.GetType("System.String"));
                dtTrabalhadores.Columns.Add("NOME", Type.GetType("System.String"));

                dtTrabalhadores.Columns.Add("Horas_Normal", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("Horas_Noturnas", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("Horas_Feriado", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("Horas_Feriado_Noturno", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("HorasExtra_Normais", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("HorasExtra_Noturno", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("HorasExtra_100", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("HorasDomingo", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("HorasDomingo_Noturno", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("Horas_gratificadas", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("Horas_Efetivas", Type.GetType("System.Decimal"));
                dtTrabalhadores.Columns.Add("faltas", Type.GetType("System.Int64"));
                dtTrabalhadores.Columns.Add("doenca", Type.GetType("System.Int64"));
                dtTrabalhadores.Columns.Add("diasferias", Type.GetType("System.Int64"));


                // var trab = dtPonto
                string setor = txSetores.Text.Trim();
                var trabalhadores = (from gr in dtPonto.AsEnumerable()
                             .Where(a=> 
                       (setor == ""? true:(a.Field<string>("SETORCADASTRO").Trim() == setor)  ))
                             
                                     group gr by new
                                     {
                                         codigo = gr.Field<string>("CODIGO"),
                                         nome = gr.Field<string>("NOME")
                                     } into g
                                     select new
                                     {
                                         codigo = g.Key.codigo,
                                         nome = g.Key.nome,
                                         Horas_Normal = g.Sum(a => a.Field<double>("HORAS_NORMAL")),
                                         Horas_Noturnas = g.Sum(a => a.Field<double>("Horas_Noturnas")),
                                         Horas_Feriado = g.Sum(a => a.Field<double>("Horas_Feriado")),
                                         Horas_Feriado_Noturno = g.Sum(a => a.Field<double>("Horas_Feriado_Noturno")),
                                         HorasExtra_Normais = g.Sum(a => a.Field<double>("HorasExtra_Normais")),
                                         HorasExtra_Noturno = g.Sum(a => a.Field<double>("HorasExtra_Noturno")),
                                         HorasExtra_100 = g.Sum(a => a.Field<double>("HorasExtra_100")),
                                         HorasDomingo = g.Sum(a => a.Field<double>("HorasDomingo")),
                                         HorasDomingo_Noturno = g.Sum(a => a.Field<double>("HorasDomingo_Noturno")),
                                         Horas_gratificadas = g.Sum(a => a.Field<double>("Horas_gratificadas")),
                                         Horas_Efetivas = g.Sum(a => a.Field<double>("Horas_Efetivas")),
                                         faltas = g.Sum(a => a.Field<Int64>("faltas")),
                                         doenca = g.Sum(a => a.Field<Int64>("doenca")),
                                         diasferias = g.Sum(a => a.Field<Int64>("diasferias")),
                                     });

               
                try
                {
                    foreach (var dados in trabalhadores
                         )
                    {

                        // if (dataRow["NUMCONTA"].ToString() == "00000000") continue;

                        DataRow orowB = dtTrabalhadores.NewRow();
                        orowB["CODIGO"] = dados.codigo;
                        orowB["NOME"] = dados.nome;
                        orowB["Horas_Normal"] = dados.Horas_Normal;
                        orowB["Horas_Noturnas"] = dados.Horas_Noturnas;
                        orowB["Horas_Feriado"] = dados.Horas_Feriado;

                        orowB["Horas_Feriado_Noturno"] = dados.Horas_Feriado_Noturno;
                        orowB["HorasExtra_Normais"] = dados.HorasExtra_Normais;
                        orowB["HorasExtra_Noturno"] = dados.HorasExtra_Noturno;

                        orowB["HorasExtra_100"] = dados.HorasExtra_100;
                        orowB["HorasDomingo"] = dados.HorasDomingo;
                        orowB["HorasDomingo_Noturno"] = dados.HorasDomingo_Noturno;

                        orowB["Horas_gratificadas"] = dados.Horas_gratificadas;
                        orowB["Horas_Efetivas"] = dados.Horas_Efetivas;
                        orowB["faltas"] = dados.faltas;
                        orowB["doenca"] = dados.doenca;
                        orowB["diasferias"] = dados.diasferias;
                        
                        dtTrabalhadores.Rows.Add(orowB);
                        orowB.AcceptChanges();
                    }

                    dtTrabalhadores.AcceptChanges();
                }
                catch (Exception E)
                {

                    MessageBox.Show(E.Message);
                }

            }
            catch (Exception)
            {

                MessageBox.Show("Erro Geração Trabalhadores Resumo");
            }
            Cursor.Current = Cursors.Default;
            return dtTrabalhadores;

        }





        private void MonteGrids()
        {
                oPonto.Clear();
             oPonto.AddValores("Gleba", "Gleba", 4, "", false, 0, "");
            oPonto.AddValores("Quadra", "Quadra", 4, "", false, 0, "");
            oPonto.AddValores("Modelo", "Modelo", 13, "", false, 0, "");
            oPonto.AddValores("Serviço", "Serviço", 21, "", false, 0, "");
            oPonto.AddValores("noturno", "Noturno", 3, "", false, 0, "");
            for (int i = inicio.Day; i <= fim.Day; i++)
            {
                oPonto.AddValores("DIA"+i.ToString()
                    , i.ToString().PadRight(2), 3, "", false, 0, "");
            }
            oPonto.AddValores("Horas_Normal", "Hs.Normais",6, "##0.00", true, 0, "");
            oPonto.AddValores("Horas_Efetivas", "Hs.Efetivas", 6, "##0.00", true, 0, "");
           
            oPonto.AddValores("Horas_Feriado", "Hs.100%Feriado", 6, "##0", true, 0, "");
            oPonto.AddValores("HorasDomingo", "Hs.100%Domingo", 6, "##0", true, 0, "");
            oPonto.AddValores("HorasExtra_Normais", "Hs.Extra", 6, "##0", true, 0, "");
            oPonto.AddValores("Horas_Noturnas", "Hs.Noturno", 6, "##0", true, 0, "");
            oPonto.AddValores("HorasExtra_Noturno", "Hs.ExtraNoturno", 6, "##0", true, 0, "");
            
            oPonto.AddValores("Horas_Feriado_Noturno", "Hs.FeriadoNoturno", 6, "##0", true, 0, "");
            oPonto.AddValores("HorasDomingo_Noturno", "Hs.DomingoNoturno", 6, "##0", true, 0, "");
                        //oPonto.AddValores("Horas_gratificadas", "Hs.Gratific.", 6, "##0.0", true, 0, "");
            oPonto.AddValores("faltas", "Faltas", 5, "##0", true, 0, "");
            oPonto.AddValores("doenca", "Doenca", 5, "##0", true, 0, "");
            oPonto.AddValores("diasferias", "DiasFerias", 5, "##0", true, 0, "");
            oPonto.AddValores("OBS", "Obs",5, "", false, 0, "");


        }

        //PonhaNoExcel(oworksheet, 2, "M", dataColeta);

        private void btnExcel_Click(object sender, EventArgs e)
        {
            dtTrabalhadores = Reload();

            System.Windows.Forms.DialogResult result;// = System.Windows.Forms.DialogResult.OK;
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.DefaultExt = "xls";
            SaveFileDialog1.AddExtension = true;

            SaveFileDialog1.CheckFileExists = false;
            result = SaveFileDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                Microsoft.Office.Interop.Excel.Application ExcelObj = new Microsoft.Office.Interop.Excel.Application();

                try
                {
                    if (ExcelObj == null)
                    {
                        MessageBox.Show("ERROR: EXCEL não pôde ser iniciado!");
                        return;
                    }
                    // ExcelObj.Visible = true;

                    Microsoft.Office.Interop.Excel.Workbook oWorkbook =
                        ExcelObj.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);


                    // Microsoft.Office.Interop.Excel.Range Oget_Range;


                    try
                    {

                        int linhasinicio = 1;
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                        Microsoft.Office.Interop.Excel._Worksheet oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);

                        oworksheet.Activate();
                        oworksheet.Name = "Registros";

                        int linhas = linhasinicio;
                        int numcol = 3;
                        string linhastr = linhas.ToString();

                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 =
                            "Relatório Pontos Trabalhadores " + inicio.ToString("d") + " a " + fim.ToString("d");
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Font.Bold = true;
                       // oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        linhasinicio = linhasinicio + 2;
                        
                        foreach (DataRow orow in dtTrabalhadores.Rows)
                        {

                            //var orowdgv = dgvBalancete.CurrentRow;
                            
                            if (orow != null)
                            {
                                string codigo = orow["CODIGO"].ToString();


                                string nome = orow["NOME"].ToString();


                                System.Data.DataTable pontoAcrescido = dtPonto.AsEnumerable().Where(a => a.Field<string>("CODIGO") == codigo).CopyToDataTable();
                                pontoAcrescido.TableName = "PontoAcrescido";
                                DataRow orowTotal = pontoAcrescido.NewRow();
                                orowTotal["Serviço"] = "TOTAIS =>";
                                orowTotal["Horas_Normal"] = orow["Horas_Normal"];
                                orowTotal["Horas_Noturnas"] = orow["Horas_Noturnas"];
                                orowTotal["Horas_Feriado"] = orow["Horas_Feriado"];

                                orowTotal["Horas_Feriado_Noturno"] = orow["Horas_Feriado_Noturno"];
                                orowTotal["HorasExtra_Normais"] = orow["HorasExtra_Normais"];
                                orowTotal["HorasExtra_Noturno"] = orow["HorasExtra_Noturno"];

                                orowTotal["HorasExtra_100"] = orow["HorasExtra_100"];

                            
                                orowTotal["HorasDomingo"] = orow["HorasDomingo"];
                                orowTotal["HorasDomingo_Noturno"] = orow["HorasDomingo_Noturno"];
                                orowTotal["Horas_Gratificadas"] = orow["Horas_Gratificadas"];
                                orowTotal["Horas_Efetivas"] = orow["Horas_Efetivas"];


                                orowTotal["faltas"] = orow["faltas"];
                                orowTotal["doenca"] = orow["doenca"];
                                orowTotal["diasferias"] = orow["diasferias"];
                                pontoAcrescido.Rows.Add(orowTotal);
                                pontoAcrescido.AcceptChanges();

                                DataView dvPontoTrabalhador = pontoAcrescido.AsDataView();
                                //dtPonto.AsEnumerable().Where(a => a.Field<string>("CODIGO") == codigo).AsDataView();



                                oPonto = new MonteGrid();
                                MonteGrids();

                                oPonto.oDataGridView = dgvPonto;
                                //oBalancete.sbTotal = sbBalancete

                                bmSource = new BindingSource();
                                bmSource.DataSource = dvPontoTrabalhador;
                                ///bmSourceSaida.DataSource = dvSaidas;
                                dgvPonto.DataSource = bmSource;
                                oPonto.ConfigureDBGridView();
                                string odatamember = "";
                                DataSet odataset = null;
                                DataView odataview = null;
                                if (oPonto.oDataGridView == null) continue;
                                if (oPonto.oDataGridView.DataSource is BindingSource)
                                {
                                    if (((BindingSource)oPonto.oDataGridView.DataSource).DataSource is DataSet)
                                    {
                                        odataset = (DataSet)((BindingSource)oPonto.oDataGridView.DataSource).DataSource;
                                        odatamember = ((BindingSource)oPonto.oDataGridView.DataSource).DataMember;
                                    }
                                    else
                                    {
                                        if (((BindingSource)oPonto.oDataGridView.DataSource).DataSource is DataView)
                                        {
                                            odataview = (DataView)((BindingSource)oPonto.oDataGridView.DataSource).DataSource;
                                            odatamember = odataview.Table.TableName;
                                        }
                                        else if (((BindingSource)oPonto.oDataGridView.DataSource).DataSource is System.Data.DataTable)
                                        {
                                            odataview = (((BindingSource)oPonto.oDataGridView.DataSource).DataSource as System.Data.DataTable).DefaultView;
                                            odatamember = odataview.Table.TableName;
                                        }
                                    }
                                }
                                else
                                {
                                    odataset = (DataSet)oPonto.oDataGridView.DataSource;
                                    odatamember = oPonto.oDataGridView.DataMember;
                                }
                                try
                                {
                                    lbTexto.Text = nome;
                                    Cursor.Current = Cursors.WaitCursor;
                                    linhasinicio = CorpoExcel(oWorkbook, oworksheet, odataview, orow, linhasinicio);
                                    linhasinicio = linhasinicio + 3;
                                    Cursor.Current = Cursors.Default;
                                }
                                catch (Exception E)
                                {
                                    MessageBox.Show(E.Message);

                                }

                            }



                        }

                        // COLOCA AS SIMBOLOGIAS
                        try
                        {
                            sheets.Add(Type.Missing, sheets.get_Item(sheets.Count), Type.Missing, Type.Missing);
                            oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(2);

                            oworksheet.Activate();
                            oworksheet.Name = "Simbolos Ponto";
                            DataView dgvSimbolos = dtCLTCodigo.AsDataView();
                            CorpoExcelSimbolos(oWorkbook, oworksheet, dgvSimbolos);
                            oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);
                            oworksheet.Activate();

                        }
                        catch (Exception)
                        {

                            
                        }
                      


                        oWorkbook.SaveAs(SaveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                       "", "", false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                        Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true, "", "", false);

                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Erro ao tentar abrir tabela Excel ", "",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
                finally
                {
                    //oWor;
                    if (ExcelObj != null)
                    {
                        ExcelObj.Quit();
                        MessageBox.Show("Excel " + SaveFileDialog1.FileName + " Gravado Ok");
                    }

                    Cursor.Current = Cursors.Default;
                }
            }



            // oRazao.ExportaExcel();
        }

        



        private Microsoft.Office.Interop.Excel.Range Colunas_exc(string colunas, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            coluna1 = colunas.Trim();
            return oworksheet.get_Range(coluna1 + "1", coluna1 + "65536");
        }

        private Microsoft.Office.Interop.Excel.Range Colunas_exc(string Coluna1, string Coluna2, int Linha1, int Linha2, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            string coluna2 = "";
            coluna1 = Coluna1.Trim();
            coluna2 = Coluna2.Trim();
            return oworksheet.get_Range(coluna1 + Linha1.ToString().Trim(), coluna2 + Linha2.ToString().Trim());
        }


        private int CorpoExcel(Microsoft.Office.Interop.Excel.Workbook oWorkbook,
            Microsoft.Office.Interop.Excel._Worksheet oworksheet,
            DataView odataview, DataRow orowMestre, int linhasInicioExcel)
        {

            Microsoft.Office.Interop.Excel.Range Oget_Range;


            int linhasinicio = linhasInicioExcel;

            // define linha max
            int linhasmax = linhasinicio + odataview.Count;

            //titulo
            int linhas = linhasinicio;
            int numcol = 0;
            string linhastr = linhas.ToString();
            string codigo = orowMestre["CODIGO"].ToString();


            string nome = orowMestre["NOME"].ToString();
            int maxnumcol = oPonto.LinhasCampo.Count - 1;
          
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = codigo + " "+nome;
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Font.Bold = true;
           
            
          /*  oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.Pattern =
                 Microsoft.Office.Interop.Excel.XlPattern.xlPatternSolid;
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.PatternColorIndex =
                 Microsoft.Office.Interop.Excel.XlPattern.xlPatternAutomatic;
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.ThemeColor = XlThemeColor.xlThemeColorDark2;
          */

            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.Color 
                = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);



            /*
             With Selection.Interior
        .Pattern = xlSolid
        .PatternColorIndex = xlAutomatic
        .ThemeColor = xlThemeColorDark2
        .TintAndShade = 0
        .PatternTintAndShade = 0
    End With
    
             */
            //oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).

            numcol = 0;
            linhas = linhas + 1;
            linhastr = linhas.ToString();

            // cabeçalho
            Type tipo = null;
            foreach (Campo campo in oPonto.LinhasCampo)
            {
                Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                          (linhas + 1), linhasmax, oworksheet);
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
                if ((campo.titulo.ToUpper() == "SERVIÇO") || (campo.titulo.ToUpper() == "MODELO")
                       || (campo.titulo.ToUpper() == "GLEBA") || (campo.titulo.ToUpper() == "QUADRA")
                       )
                {
                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Font.Size = 6.5;

                }
               
                Oget_Range.ColumnWidth = campo.tamanho;
                if (odataview.Table.Columns.Contains(campo.titulo)) { tipo = odataview.Table.Columns[campo.titulo].DataType; }
                if (tipo != null)
                {
                    if ((tipo != Type.GetType("System.String")) && (tipo != Type.GetType("System.Boolean")) && (tipo != Type.GetType("System.DateTime")))
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > Convert.ToInt32(Oget_Range.ColumnWidth))
                            Oget_Range.ColumnWidth = tam;
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    }
                    if ((tipo == Type.GetType("System.DateTime")) && (campo.format != ""))
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > Convert.ToInt32(Oget_Range.ColumnWidth))
                            Oget_Range.ColumnWidth = tam;
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    }
                }
                else
                {
                    if (campo.format != "")
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > Oget_Range.ColumnWidth)
                            Oget_Range.ColumnWidth = tam;
                    }
                }


                numcol += 1;
            }


            linhas += 1;
            foreach (DataRowView orow in odataview)
            {
                numcol = 0;
                linhastr = linhas.ToString();
                foreach (Campo campo in oPonto.LinhasCampo)
                {
                    object dado = "";
                    try
                    {
                        tipo = orow[campo.titulo].GetType();
                        if (tipo == Type.GetType("System.DateTime"))
                        {
                            if (campo.format != "")
                            {
                                dado = orow[campo.titulo];
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                            }
                            else
                            {
                                dado = Convert.ToDateTime(orow[campo.titulo]);
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "DD/MM/YYY";
                            }

                        }
                        else if ((tipo != Type.GetType("System.String")) && (tipo != Type.GetType("System.Boolean"))) // numerico
                        {
                            if (campo.format != "")
                            {
                                if (tipo == Type.GetType("System.Decimal") || tipo == Type.GetType("System.Double") || tipo == Type.GetType("System.Single"))
                                { dado = orow[campo.titulo]; }
                                else
                                { dado = orow[campo.titulo]; }
                            }
                            else { dado = orow[campo.titulo]; }
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                        }
                        else
                            dado = orow[campo.titulo].ToString();
                    }
                    catch
                    { }

                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                    if ((campo.titulo.ToUpper() == "SERVIÇO") || (campo.titulo.ToUpper() == "MODELO")
                        || (campo.titulo.ToUpper() == "GLEBA") || (campo.titulo.ToUpper() == "QUADRA")
                        )
                    {
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Font.Size = 6.5;

                    }
                    if ((campo.titulo.ToUpper() == "OBS") && (dado.ToString().Trim() != ""))
                    {
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Font.Color =
                            Color.Red;
                    }




                    numcol += 1;
                }
                               linhas += 1;
            }
            // ultima linha (total)
            oworksheet.get_Range(Letras[0] + (linhas-1).ToString(), Letras[numcol - 1] + (linhas - 1).ToString()).Font.Bold = true;
            /* oworksheet.get_Range(Letras[0] + (linhas - 1).ToString(), Letras[maxnumcol] + (linhas - 1).ToString()).Interior.Pattern =
                  Microsoft.Office.Interop.Excel.XlPattern.xlPatternSolid;
             oworksheet.get_Range(Letras[0] + (linhas - 1).ToString(), Letras[maxnumcol] + (linhas - 1).ToString()).Interior.PatternColorIndex =
                  Microsoft.Office.Interop.Excel.XlPattern.xlPatternAutomatic;
             oworksheet.get_Range(Letras[0] + (linhas - 1).ToString(), Letras[maxnumcol] + (linhas - 1).ToString()).Interior.ThemeColor = XlThemeColor.xlThemeColorDark2;
            */
            oworksheet.get_Range(Letras[0] + (linhas - 1).ToString(), Letras[maxnumcol] + (linhas - 1).ToString()).Interior.Color
                 = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);


            int linhasretorno = linhas;

            linhas = linhasinicio;
            numcol = 0;
            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + (oPonto.LinhasCampo.Count - 1)].ToString(),
                 linhas+1, (linhasmax + 1), oworksheet);
            Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            return linhasretorno;

        }

        private void CorpoExcelSimbolos(Microsoft.Office.Interop.Excel.Workbook oWorkbook,
            Microsoft.Office.Interop.Excel._Worksheet oworksheet,
            DataView odataview)
        {

            Microsoft.Office.Interop.Excel.Range Oget_Range;


            int linhasinicio = 1;

            // define linha max
                int linhasmax = linhasinicio + odataview.Count;

            //titulo
            int linhas = linhasinicio;
            int numcol = 0;
            string linhastr = linhas.ToString();
            int maxnumcol = oPonto.LinhasCampo.Count - 1;

            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "Simbolos Utilizados no Controle de Pontos";
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Font.Bold = true;


            /*  oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.Pattern =
                   Microsoft.Office.Interop.Excel.XlPattern.xlPatternSolid;
              oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.PatternColorIndex =
                   Microsoft.Office.Interop.Excel.XlPattern.xlPatternAutomatic;
              oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.ThemeColor = XlThemeColor.xlThemeColorDark2;
            */

            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[maxnumcol] + linhastr).Interior.Color
                = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);



            numcol = 0;
            linhas = linhas + 1;
            linhastr = linhas.ToString();

            // cabeçalho
            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                          (linhas + 1), linhasmax, oworksheet);
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "CÓDIGO";
            Oget_Range.ColumnWidth = 7;
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment
                = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            
            numcol += 1;
            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                          (linhas + 1), linhasmax, oworksheet);
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "DESCRIÇÃO";
            Oget_Range.ColumnWidth = 25;
            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment 
                = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;




            linhas += 1;
            foreach (DataRowView orow in odataview)
            {
                numcol = 0;
                linhastr = linhas.ToString();
                                          
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = orow["INDCOD"].ToString();
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment
                = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                numcol += 1;
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = orow["DESCCOD"].ToString();
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment
               = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

                linhas += 1;
            }
           
            int linhasretorno = linhas;

            linhas = linhasinicio;
            numcol = 0;
            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + 1].ToString(),
                 linhas + 1, (linhasmax + 1), oworksheet);
            Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            

        }



    }

}
