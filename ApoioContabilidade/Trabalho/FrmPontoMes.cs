using ApoioContabilidade.Models;
using ClassFiltroEdite;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.DefaultExt = "xlsx";
            SaveFileDialog1.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            SaveFileDialog1.AddExtension = true;
            SaveFileDialog1.CheckFileExists = false;

            if (SaveFileDialog1.ShowDialog() != DialogResult.OK) return;

            Cursor.Current = Cursors.WaitCursor;
            string filename = SaveFileDialog1.FileName;
            if (!filename.ToLower().EndsWith(".xlsx")) filename += ".xlsx";

            try
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists) fi.Delete();

                using (var pkg = new ExcelPackage(fi))
                {
                    var ws = pkg.Workbook.Worksheets.Add("Registros");

                    // Titulo na linha 1, coluna D (numcol=3 + 1 = 4 em EPPlus)
                    ws.Cells[1, 4].Value =
                        "Relatório Pontos Trabalhadores " + inicio.ToString("d") + " a " + fim.ToString("d");
                    ws.Cells[1, 4].Style.Font.Bold = true;

                    int linhasinicio = 3;
                    foreach (DataRow orow in dtTrabalhadores.Rows)
                    {
                        if (orow == null) continue;
                        string codigo = orow["CODIGO"].ToString();
                        string nome = orow["NOME"].ToString();

                        System.Data.DataTable pontoAcrescido = dtPonto.AsEnumerable()
                            .Where(a => a.Field<string>("CODIGO") == codigo).CopyToDataTable();
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

                        oPonto = new MonteGrid();
                        MonteGrids();
                        oPonto.oDataGridView = dgvPonto;

                        bmSource = new BindingSource();
                        bmSource.DataSource = dvPontoTrabalhador;
                        dgvPonto.DataSource = bmSource;
                        oPonto.ConfigureDBGridView();

                        // Extrai DataView do binding (preserva logica original)
                        DataView odataview = null;
                        if (oPonto.oDataGridView == null) continue;
                        if (oPonto.oDataGridView.DataSource is BindingSource)
                        {
                            var bs = (BindingSource)oPonto.oDataGridView.DataSource;
                            if (bs.DataSource is DataView)
                                odataview = (DataView)bs.DataSource;
                            else if (bs.DataSource is System.Data.DataTable)
                                odataview = ((System.Data.DataTable)bs.DataSource).DefaultView;
                        }
                        if (odataview == null) continue;

                        try
                        {
                            lbTexto.Text = nome;
                            linhasinicio = CorpoExcel(ws, odataview, orow, linhasinicio);
                            linhasinicio += 3;
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show(E.Message);
                        }
                    }

                    // Sheet adicional com simbolos do ponto
                    try
                    {
                        var wsSimbolos = pkg.Workbook.Worksheets.Add("Simbolos Ponto");
                        DataView dvSimbolos = dtCLTCodigo.AsDataView();
                        CorpoExcelSimbolos(wsSimbolos, dvSimbolos);
                    }
                    catch (Exception) { }

                    pkg.Save();
                }

                MessageBox.Show("Excel " + filename + " gravado OK.");
            }
            catch (Exception E)
            {
                MessageBox.Show("Erro ao gerar planilha Excel: " + E.Message, "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        



        private int CorpoExcel(ExcelWorksheet ws, DataView odataview, DataRow orowMestre, int linhasInicioExcel)
        {
            int linhasinicio = linhasInicioExcel;
            int linhasmax = linhasinicio + odataview.Count;

            int linha = linhasinicio;
            int col = 1; // EPPlus 1-indexed
            string codigo = orowMestre["CODIGO"].ToString();
            string nome = orowMestre["NOME"].ToString();
            int maxnumcol = oPonto.LinhasCampo.Count;

            // Linha de cabecalho do trabalhador
            ws.Cells[linha, col].Value = codigo + " " + nome;
            ws.Cells[linha, col].Style.Font.Bold = true;
            ws.Cells[linha, col, linha, maxnumcol].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[linha, col, linha, maxnumcol].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            linha += 1;

            // Cabecalho dos campos
            Type tipo = null;
            col = 1;
            foreach (Campo campo in oPonto.LinhasCampo)
            {
                ws.Cells[linha, col].Value = campo.cabecalho;
                if (EhCampoCompacto(campo.titulo))
                    ws.Cells[linha, col].Style.Font.Size = 6.5f;

                ws.Column(col).Width = campo.tamanho;
                if (odataview.Table.Columns.Contains(campo.titulo))
                    tipo = odataview.Table.Columns[campo.titulo].DataType;
                else
                    tipo = null;

                if (tipo != null)
                {
                    if (tipo != typeof(string) && tipo != typeof(bool) && tipo != typeof(DateTime))
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > ws.Column(col).Width) ws.Column(col).Width = tam;
                        ws.Cells[linha, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    if (tipo == typeof(DateTime) && campo.format != "")
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > ws.Column(col).Width) ws.Column(col).Width = tam;
                        ws.Cells[linha, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }
                else if (campo.format != "")
                {
                    int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                    if (tam > ws.Column(col).Width) ws.Column(col).Width = tam;
                }
                col += 1;
            }

            // Dados
            linha += 1;
            foreach (DataRowView orow in odataview)
            {
                col = 1;
                foreach (Campo campo in oPonto.LinhasCampo)
                {
                    object dado = "";
                    try
                    {
                        tipo = orow[campo.titulo].GetType();
                        if (tipo == typeof(DateTime))
                        {
                            dado = orow[campo.titulo];
                            ws.Cells[linha, col].Style.Numberformat.Format =
                                campo.format != "" ? campo.format : "DD/MM/YYY";
                        }
                        else if (tipo != typeof(string) && tipo != typeof(bool)) // numerico
                        {
                            dado = orow[campo.titulo];
                            ws.Cells[linha, col].Style.Numberformat.Format = campo.format;
                            ws.Cells[linha, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            dado = orow[campo.titulo].ToString();
                        }
                    }
                    catch { }

                    ws.Cells[linha, col].Value = dado;
                    if (EhCampoCompacto(campo.titulo))
                        ws.Cells[linha, col].Style.Font.Size = 6.5f;
                    if (campo.titulo.ToUpper() == "OBS" && dado.ToString().Trim() != "")
                        ws.Cells[linha, col].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    col += 1;
                }
                linha += 1;
            }

            // Ultima linha (TOTAIS) em bold + fundo cinza claro
            int linhaTotal = linha - 1;
            ws.Cells[linhaTotal, 1, linhaTotal, col - 1].Style.Font.Bold = true;
            ws.Cells[linhaTotal, 1, linhaTotal, maxnumcol].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[linhaTotal, 1, linhaTotal, maxnumcol].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            int linhasretorno = linha;

            // Grade completa (Top+Bottom+Left+Right em todas as celulas)
            // Replica Range.Borders.LineStyle = xlContinuous do Excel Interop.
            var rangeGrade = ws.Cells[linhasinicio + 1, 1, linhasmax + 1, oPonto.LinhasCampo.Count];
            rangeGrade.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            return linhasretorno;
        }

        private static bool EhCampoCompacto(string titulo)
        {
            string t = titulo.ToUpper();
            return t == "SERVIÇO" || t == "MODELO" || t == "GLEBA" || t == "QUADRA";
        }

        private void CorpoExcelSimbolos(ExcelWorksheet ws, DataView odataview)
        {
            int linhasinicio = 1;
            int linhasmax = linhasinicio + odataview.Count;
            int linha = linhasinicio;
            int maxnumcol = oPonto.LinhasCampo.Count;

            ws.Cells[linha, 1].Value = "Simbolos Utilizados no Controle de Pontos";
            ws.Cells[linha, 1].Style.Font.Bold = true;
            ws.Cells[linha, 1, linha, maxnumcol].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[linha, 1, linha, maxnumcol].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            linha += 1;
            ws.Cells[linha, 1].Value = "CÓDIGO";
            ws.Column(1).Width = 7;
            ws.Cells[linha, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[linha, 2].Value = "DESCRIÇÃO";
            ws.Column(2).Width = 25;
            ws.Cells[linha, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            linha += 1;
            foreach (DataRowView orow in odataview)
            {
                ws.Cells[linha, 1].Value = orow["INDCOD"].ToString();
                ws.Cells[linha, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[linha, 2].Value = orow["DESCCOD"].ToString();
                ws.Cells[linha, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                linha += 1;
            }

            // Grade completa (Top+Bottom+Left+Right em todas as celulas)
            var rangeGrade = ws.Cells[linhasinicio + 1, 1, linhasmax + 1, 2];
            rangeGrade.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }



    }

}
