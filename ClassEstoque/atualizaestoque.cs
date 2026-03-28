using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;

using ClassConexao;
using ClassApropriaçoes;
using ClassLibTrabalho;


namespace ClassEstoque
{
    public class AtualizaEstoque
    {
        DataTable movest;
        DataTable cadest;
        OleDbDataAdapter oadapter_movest;
        OleDbDataAdapter oadapter_cadest;
        public DateTime inicio, fim;
        ClassModelo omodelo;
        ClassParceiro oparceria;
        List<DataRow> linhacadest;
        
     

        class TObjParcIns
        {
            public Decimal quant_pa;
            public Decimal quant_fr;
            public Decimal vlr_pa;
            public Decimal vlr_fr;
            public string tprod;

        }

        public void AtiveRelatorio()
        {

            FrmRelatorioAlmox oform = new FrmRelatorioAlmox();
            Cursor ocursor = Cursor.Current;

            Cursor.Current = Cursors.WaitCursor;
            
            oform.Show();
            Cursor.Current = ocursor;

            
        }

        public AtualizaEstoque()
        {
            oadapter_cadest = Classcadest.cadestConstruaAdaptador("ESTOQUE");
            oadapter_movest = Classmovest.movestConstruaAdaptador("ESTOQUE");
            omodelo = new ClassModelo();
            oparceria = new ClassParceiro();
            linhacadest = new List<DataRow>();
         
        }

        public DataTable Copia_Movest()
        {
            if (movest == null)
            {
                oadapter_movest.SelectCommand.Parameters[0].Value = inicio;//TDataControlReduzido.FormatDataGravar(inicio);
                oadapter_movest.SelectCommand.Parameters[1].Value = fim;// TDataControlReduzido.FormatDataGravar(fim);
                movest = new DataTable("movest");
                oadapter_movest.Fill(movest);

                return movest;
            }
            return movest.Copy();
        
        }

        public Boolean Atualize(DateTime inicio, DateTime fim)
        {

            oadapter_cadest.SelectCommand.Parameters[0].Value = inicio;//TDataControlReduzido.FormatDataGravar(inicio);
            oadapter_cadest.SelectCommand.Parameters[1].Value = fim;// TDataControlReduzido.FormatDataGravar(fim);
            cadest = new DataTable("cadest");
            oadapter_cadest.Fill(cadest);

            oadapter_movest.SelectCommand.Parameters[0].Value = inicio;//TDataControlReduzido.FormatDataGravar(inicio);
            oadapter_movest.SelectCommand.Parameters[1].Value = fim;// TDataControlReduzido.FormatDataGravar(fim);
            movest = new DataTable("movest");
            oadapter_movest.Fill(movest);

            oadapter_movest.RowUpdating += new OleDbRowUpdatingEventHandler(oadapter_movest_RowUpdating);
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = movest.Columns["NUMERO"];
            movest.PrimaryKey = PrimaryKeyColumns;
            movest.AcceptChanges();


            foreach (DataRow orow in cadest.AsEnumerable())
            {

                orow["VALOR"] = 0.0M;
                orow["QUANT"] = 0.0M;
                orow["DATA"] = orow["DATA_CAD"];
            }
            int numeroconta = 0;

            foreach (DataRow orow in cadest.AsEnumerable())
            {
                if (orow["COD"].ToString().Trim() == "") continue;
                /* Boolean sim = false;
                 foreach (string cod in procure.AsEnumerable())
                 {
                     if (cod == orow["COD"].ToString())
                     {
                         sim = true;
                         break;
                     }
                
                 }
                 if (!sim) continue;*/
                DateTime tdata_ini = Convert.ToDateTime(orow["DATA_CAD"]);
                Decimal tvalor = 0.0M;// Convert.ToDecimal(orow["VALOR"]);
                Decimal tquant = 0.0M;// Convert.ToDecimal(orow["QUANT"]);
                string tcod = orow["COD"].ToString();
                Decimal tcust_me = 0.0M;
                Decimal vlr_sai = 0.0M;
                Decimal quant_sai = 0.0M;
                Boolean naoprocesse = false;
                DateTime tdata_cus = tdata_ini;
                //if ((tvalor != 0) && (tquant != 0))
                //  tcust_me = Decimal.Round(tvalor / tquant, 4);
                DateTime tdatapesq = inicio;
                if (tdata_ini < tdatapesq)
                    tdatapesq = tdata_ini;
                /*  DataRow[] movestfiltrada = movest.Select("(COD ='" + tcod + "') " +
                                                           " AND (TIPO2 <> 'T' )"+ 
                                                               "AND (DATA >= '" + tdatapesq.ToString("d") + "') "
                                                                 , "DATA,TIPO");
                  */

                IEnumerable<DataRow> movestfiltrada =
                from linha in movest.AsEnumerable()
                where (linha.Field<string>("COD") == tcod)
                && (linha.Field<string>("TIPO2") != "T")
                && (linha.Field<DateTime>("DATA") >= tdatapesq)
                orderby linha.Field<DateTime>("DATA"), linha.Field<string>("TIPO")
                select linha;




                if (movestfiltrada.Count() == 0) continue;
                naoprocesse = false;
                foreach (DataRow orowmov in movestfiltrada.AsEnumerable())
                {
                    // if (orowmov["TIPO2"].ToString() == "T") continue;

                    if (orowmov["TIPO"].ToString() == "E")
                    {

                        if (Convert.ToDecimal(orowmov["VALOR"]) > 0)
                        {
                            tvalor = tvalor + Convert.ToDecimal(orowmov["VALOR"]);
                        }
                        else
                        {
                            tvalor = tvalor + Decimal.Round((tcust_me * Convert.ToDecimal(orowmov["QUANT"])), 2);
                        }
                        tquant = tquant + Convert.ToDecimal(orowmov["QUANT"]);

                        if ((tquant > 0) && (tvalor > 0))
                        {
                            tcust_me = ptRound((tvalor / tquant), 4);
                            tdata_cus = Convert.ToDateTime(orowmov["DATA"]);
                        }
                    }
                    else
                        if (orowmov["TIPO"].ToString() == "S")
                        {
                            quant_sai = Convert.ToDecimal(orowmov["QUANT"]);
                            vlr_sai = ptRound((tcust_me * quant_sai), 2);

                            tvalor = tvalor - vlr_sai;
                            tquant = tquant - quant_sai;
                            if ((tquant < 0) && (!naoprocesse))
                            {
                                naoprocesse = true;
                            };

                            TObjParcIns objins;
                            try
                            {
                                objins = ValoresParc(orowmov);
                                if (objins == null) MessageBox.Show("erro 44 Nos Valores dos parceiros " + tcod);

                                orowmov.BeginEdit();
                                orowmov["PUnit"] = tcust_me;
                                orowmov["VALOR"] = vlr_sai;
                                orowmov["DATAC"] = tdata_cus;
                                orowmov["QUANT_PA"] = objins.quant_pa;
                                orowmov["QUANT_FR"] = objins.quant_fr;
                                orowmov["VLR_FR"] = objins.vlr_fr;
                                orowmov["VLR_PA"] = objins.vlr_pa;
                                if (objins.tprod != "") orowmov["PROD"] = objins.tprod;
                                orowmov.EndEdit();
                            }
                            catch
                            {
                                MessageBox.Show("erro Nos Valores dos parceiros " + tcod);

                            }
                        }
                }

                if (naoprocesse)
                {
                    numeroconta += 1;
                    GuardeRelatorioExcel(orow);

                }
                orow.BeginEdit();
                orow["QUANT"] = tquant;
                orow["VALOR"] = tvalor;
                orow["DATA"] = tdata_cus;
                orow.EndEdit();
            }
            if (numeroconta > 1)
            {
                if (MessageBox.Show("Existem erros.Saldos de Estoque Negativo.Publica relatorio Excel?", "Relatorio Excel", MessageBoxButtons.YesNo)
                    == DialogResult.Yes)
                    RelatorioExcel();
            }
            try
            {

                oadapter_movest.Update(movest);

                oadapter_cadest.RowUpdating += new OleDbRowUpdatingEventHandler(oadapter_cadest_RowUpdating);
                oadapter_cadest.Update(cadest);
            }
            catch
            {

                MessageBox.Show("Erro atualização");
                throw;
            }
            return true;
        }

        void oadapter_movest_RowUpdating(object sender, OleDbRowUpdatingEventArgs e)
        {
            string com = Convert.ToDateTime(e.Row["Data"]).ToString("d") + " " + e.Row["COD"].ToString() + " " + e.Row["TIPO"].ToString();
        }

        void oadapter_cadest_RowUpdating(object sender, OleDbRowUpdatingEventArgs e)
        {
            string com = e.Row["COD"].ToString();
            //throw new NotImplementedException();
        }

        TObjParcIns ValoresParc(DataRow orowmov)
        {
            TObjParcIns result = new TObjParcIns();

            result.quant_pa = 0.0M;
            result.quant_fr = 0.0M;
            if (Convert.ToDecimal(orowmov["QUANT"]) != 0)
                result.quant_fr = Convert.ToDecimal(orowmov["QUANT"]);
            result.vlr_pa = 0;
            result.vlr_fr = 0;
            if (Convert.ToDecimal(orowmov["VALOR"]) != 0)
                result.vlr_fr = Convert.ToDecimal(orowmov["VALOR"]);
            result.tprod = "";



            if (orowmov["NUM_MOD"].ToString().Trim() == "") return result;

            string modelonatureza = omodelo.ModeloNatureza(orowmov["NUM_MOD"].ToString()).Trim();
            if ((modelonatureza != "CUSTEIO") && (modelonatureza != "CUST.PARCEIRO") &&
               (modelonatureza != "CUST.OUTORGAN"))
            {
                return result;
            }
            result.tprod = omodelo.ModeloProduto(orowmov["NUM_MOD"].ToString());

            if (result.tprod == "") return result;

            if (oparceria.VerParceria(orowmov["QUADRA"].ToString(), orowmov["GLEBA"].ToString(),
                    result.tprod, Convert.ToDateTime(orowmov["DATA"])) == true)
            {
                //with Result do {
                if (modelonatureza == "CUSTEIO")
                {
                    result.quant_fr = Decimal.Round((Convert.ToDecimal(orowmov["QUANT"]) / 2), 2);
                    result.quant_pa = Convert.ToDecimal(orowmov["QUANT"]) - result.quant_fr;
                    result.vlr_fr = Decimal.Round(Convert.ToDecimal(orowmov["VALOR"]) / 2, 2);
                    result.vlr_pa = Convert.ToDecimal(orowmov["VALOR"]) - result.vlr_fr;
                }
                else
                    if (modelonatureza == "CUST.OUTORGAN")
                    {
                        result.quant_fr = Convert.ToDecimal(orowmov["QUANT"]);
                        result.quant_pa = 0M;
                        result.vlr_fr = Convert.ToDecimal(orowmov["VALOR"]);
                        result.vlr_pa = 0M;
                    }
                    else
                    {
                        result.quant_fr = 0M;
                        result.quant_pa = Convert.ToDecimal(orowmov["QUANT"]);
                        result.vlr_fr = 0M;
                        result.vlr_pa = Convert.ToDecimal(orowmov["VALOR"]);
                    }
            }
            return result;
        }
        void GuardeRelatorioExcel(DataRow rowcadest)
        {
            //linhasmovest.Add(rowsmovest);
            linhacadest.Add(rowcadest);

        }
        void RelatorioExcel()
        {

            Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J',
                                'K',  'L','M','N','O','P','Q','R','S','T','U'};
            Int16[] Colunas = new Int16[21] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };

            FolderBrowserDialog fbd = new FolderBrowserDialog();//New FolderBrowserDialog
            fbd.SelectedPath = System.Windows.Forms.Application.StartupPath;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                Microsoft.Office.Interop.Excel.ApplicationClass ExcelObj = new Microsoft.Office.Interop.Excel.ApplicationClass();

                try
                {
                    if (ExcelObj == null)
                    {
                        MessageBox.Show("ERROR: EXCEL não pôde ser iniciado!");
                        return;
                    }
                    ExcelObj.Visible = false;                                          // read only
                    object template = "";
                    DateTime currentDateTime = DateTime.Now;
                    string nomerelatorio = "Estoque_Erros";

                    Microsoft.Office.Interop.Excel.Workbook oWorkbook = ExcelObj.Workbooks.Add(template);

                    try
                    {
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;
                        while (sheets.Count > 1)
                        {
                            Microsoft.Office.Interop.Excel._Worksheet oworksheet2 = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(sheets.Count);
                            oworksheet2.Delete();
                        }

                        try
                        {
                            RelatorioErros(sheets);
                        }
                        catch
                        {
                            MessageBox.Show("Problemas no Relatorio Erros");
                            throw;
                        }



                        string format = "MMMMd";
                        try
                        {
                            //  oWorkbook.Close(true, oworksheet.Name + currentDateTime.ToString(format) + ".xlsx", fbd.SelectedPath);

                            string diretorio = fbd.SelectedPath + "\\" + nomerelatorio + currentDateTime.ToString(format);
                            oWorkbook.SaveAs(diretorio,
                               Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
                               Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true, null, null, true);
                        }
                        catch
                        {
                            try
                            {
                                oWorkbook.Close(false, nomerelatorio + currentDateTime.ToString(format), fbd.SelectedPath);
                            }
                            catch { throw; }
                        }
                        ExcelObj.Quit();

                    }
                    catch (Exception)
                    {
                        ExcelObj.Quit();
                        MessageBox.Show("Erro Excel ", "",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
                finally
                {
                    if (ExcelObj != null)
                        ExcelObj.Quit();

                    Cursor.Current = Cursors.Default;

                }
            }


        }

        private void RelatorioErros(Microsoft.Office.Interop.Excel.Sheets sheets)
        {
            int i = 0;
            foreach (DataRow orow in linhacadest.AsEnumerable())
            {

                Microsoft.Office.Interop.Excel._Worksheet oworksheet;
                if (i == 0)
                {
                    oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);
                    oworksheet.Name = "Codigo_" + orow["COD"].ToString() + "";

                }
                else
                {
                    oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.Add(Type.Missing, sheets.get_Item(sheets.Count), Type.Missing, Type.Missing);
                    oworksheet.Name = "Codigo_" + orow["COD"].ToString() + "";
                }
                i = i + 1;
                if (orow["COD"].ToString().Trim() == "")
                {
                    continue;
                }
                DateTime tdata_ini = Convert.ToDateTime(orow["DATA_CAD"]);
                Decimal tvalor = 0;// Convert.ToDecimal(orow["VALOR"]);
                Decimal tquant = 0;// Convert.ToDecimal(orow["QUANT"]);
                string tcod = orow["COD"].ToString();
                Decimal tcust_me = 0.0M;
                Decimal vlr_sai = 0.0M;
                Decimal quant_sai = 0.0M;
                DateTime tdata_cus;
                //if ((tvalor != 0) && (tquant != 0))
                //   tcust_me = Decimal.Round(tvalor / tquant, 4);
                // cabecalho

                DateTime tdatapesq = inicio;
                if (tdata_ini < tdatapesq)
                    tdatapesq = tdata_ini;


                Microsoft.Office.Interop.Excel.Range orange = oworksheet.get_Range("A1", "A1");

                orange.Value2 = "Codigo:" + orow["COD"].ToString();
                orange = orange.get_Offset(0, 1);
                orange.Value2 = orow["DESC"].ToString().Trim();
                orange = orange.get_Offset(0, 2);
                orange.Value2 = "Unid:" + orow["UNID"].ToString();

                orange = orange.get_Offset(0, 4);
                orange.Value2 = "Dta Cadastro:" + Convert.ToDateTime(orow["DATA_CAD"]).ToString("d");

                orange = oworksheet.get_Range("A3", "A3");
                orange.ColumnWidth = 10;
                orange.Value2 = "Data";
                //orange.EntireColumn.NumberFormat = "dd/mm/yyyy";


                // fornecedor
                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 10;
                orange.Value2 = "Fornecedor";


                orange = orange.get_Offset(0, 1);
                //Entrada
                orange.ColumnWidth = 12;
                orange.Value2 = "Entrada(Quant)";
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "####.##0,000";


                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 12;
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,00";
                orange.Value2 = "Entrada(R$)";

                // setor
                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 5;
                orange.Value2 = "Setor";

                // saida
                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 12;
                orange.Value2 = "Saida(Quant)";
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "####.##0,000";


                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 12;
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,00";
                orange.Value2 = "Saida(R$)";
                // saida
                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 12;
                orange.Value2 = "Saldo(quant)";
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "####.##0,000;[Vermelho]####.##0,000";
                // #,##0.000;[Red]#,##0.000

                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 12;
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,00;[Vermelho]#.###.##0,00";
                orange.Value2 = "Saldo(R$)";

                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 12;
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,00;[Vermelho]#.###.##0,00";
                orange.Value2 = "Custo Medio(R$)";

                Microsoft.Office.Interop.Excel.Range orangeini;// = oworksheet.get_Range("A4", "A4");

                /* orangeini.Value2 = tdata_ini.ToString("d");

                 orange = orangeini.get_Offset(0, 2);
                 orange.Value2 = tquant;

                 orange = orangeini.get_Offset(0, 3);
                 orange.Value2 = tvalor;

                 orange = orangeini.get_Offset(0, 7);
                 orange.Value2 = tquant;

                 orange = orangeini.get_Offset(0, 8);
                 orange.Value2 = tvalor;

                 orange = orangeini.get_Offset(0, 9);
                 orange.Value2 = tcust_me;
                 */
                int linha = 4;

                /*DataRow[] movestfiltrada = movest.Select("(COD ='" + tcod + "') " +
                                                             "AND (DATA >= '" + tdatapesq.ToString("d") + "') "
                                                               , "DATA,TIPO");

                if (movestfiltrada.Length == 0) continue;*/

                IEnumerable<DataRow> movestfiltrada =
                from linha_ in movest.AsEnumerable()
                where (linha_.Field<string>("COD") == tcod)
                && (linha_.Field<string>("TIPO2") != "T")
                && (linha_.Field<DateTime>("DATA") >= tdatapesq)
                orderby linha_.Field<DateTime>("DATA"), linha_.Field<string>("TIPO")
                select linha_;




                if (movestfiltrada.Count() == 0) continue;


                foreach (DataRow orowmov in movestfiltrada.AsEnumerable())
                {
                    if (orowmov["TIPO2"].ToString() == "T") continue;
                    orangeini = oworksheet.get_Range("A" + linha.ToString().Trim(), "A" + linha.ToString().Trim());
                    linha = linha + 1;
                    string devolucao = "";
                    if (orowmov["TIPO"].ToString() == "E")
                    {
                        if (Convert.ToDecimal(orowmov["VALOR"]) != 0)
                        {
                            tvalor = tvalor + Convert.ToDecimal(orowmov["VALOR"]);
                        }
                        else
                        { // "e uma devolucao de v}a ( não deverá estar acontec}o)
                            //showmessage("Devolução de V}a? Compra sem Valor? Codigo:"+tcod);
                            devolucao = "Devolução de Compra??";
                            tvalor = tvalor + Decimal.Round((tcust_me * Convert.ToDecimal(orowmov["QUANT"])), 2);
                        }
                        tquant = tquant + Convert.ToDecimal(orowmov["QUANT"]);
                        if ((tquant != 0) && (tvalor != 0))
                        {
                            tcust_me = Decimal.Round((tvalor / tquant), 4);
                            tdata_cus = Convert.ToDateTime(orowmov["DATA"]);
                        }
                        orangeini.Value2 = Convert.ToDateTime(orowmov["DATA"]).ToString("d");

                        orange = orangeini.get_Offset(0, 1);
                        orange.Value2 = orowmov["FORN"].ToString().Trim();

                        orange = orangeini.get_Offset(0, 2);
                        orange.Value2 = Convert.ToDecimal(orowmov["QUANT"]);
                        orange = orangeini.get_Offset(0, 3);
                        orange.Value2 = Convert.ToDecimal(orowmov["VALOR"]);
                        orange = orangeini.get_Offset(0, 7);
                        orange.Value2 = tquant;
                        orange = orangeini.get_Offset(0, 8);
                        orange.Value2 = tvalor;
                        orange = orangeini.get_Offset(0, 9);
                        orange.Value2 = tcust_me;
                        if (devolucao != "")
                        {
                            orange = orangeini.get_Offset(0, 10);
                            orange.Value2 = devolucao;
                        }
                    }
                    else
                        if (orowmov["TIPO"].ToString() == "S")
                        {
                            quant_sai = Convert.ToDecimal(orowmov["QUANT"]);
                            vlr_sai = Decimal.Round((tcust_me * quant_sai), 2);

                            tvalor = tvalor - vlr_sai;
                            tquant = tquant - quant_sai;

                            orangeini.Value2 = Convert.ToDateTime(orowmov["DATA"]).ToString("d");
                            orange = orangeini.get_Offset(0, 4);
                            orange.Value2 = orowmov["SETOR"].ToString();

                            orange = orangeini.get_Offset(0, 5);
                            orange.Value2 = Convert.ToDecimal(orowmov["QUANT"]);
                            orange = orangeini.get_Offset(0, 6);
                            orange.Value2 = vlr_sai;
                            orange = orangeini.get_Offset(0, 7);
                            orange.Value2 = tquant;
                            orange = orangeini.get_Offset(0, 8);
                            orange.Value2 = tvalor;

                        }
                }

            }

        }
        public Boolean ConserteTabelas()
        {

            oadapter_cadest.SelectCommand.Parameters[0].Value = inicio;//TDataControlReduzido.FormatDataGravar(inicio);
            oadapter_cadest.SelectCommand.Parameters[1].Value = fim;// TDataControlReduzido.FormatDataGravar(fim);
            cadest = new DataTable("cadest");
            oadapter_cadest.Fill(cadest);

            oadapter_movest.SelectCommand.Parameters[0].Value = inicio;//TDataControlReduzido.FormatDataGravar(inicio);
            oadapter_movest.SelectCommand.Parameters[1].Value = fim;// TDataControlReduzido.FormatDataGravar(fim);
            movest = new DataTable("movest");
            oadapter_movest.Fill(movest);
            //DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            //PrimaryKeyColumns[0] = cadest.Columns["COD"];
            //cadest.PrimaryKey = PrimaryKeyColumns;

            //int numeroconta = 0;
            var maxrow =
                  from linha in cadest.AsEnumerable()
                  where (linha.Field<string>("COD").Substring(3, 1) != " ")
                  orderby linha.Field<string>("COD") descending
                  select linha.Field<string>("COD");
            //         MessageBox.Show(maxrow.First());

            IEnumerable<DataRow> codigos_com_espaco =
                 from linha in cadest.AsEnumerable()
                 where (linha.Field<string>("COD").Substring(0, 1) != " ")
                && (linha.Field<string>("COD").Substring(3, 1) == " ")
                 orderby linha.Field<string>("COD")
                 select linha;


            foreach (DataRow orow in codigos_com_espaco.AsEnumerable())
            {
                // IEnumerable<DataRow> codigos_sem_espaco =
                //   from linha in cadest.AsEnumerable()
                //   where (linha.Field<string>("COD").Substring(0, 1) == " ")
                // && (linha.Field<string>("COD").Substring(1, 3) == orow["COD"].ToString().Substring(0,3))
                //  select linha;
                // if (codigos_sem_espaco.Count()> 0)
                // {
                DataRow[] movestfiltrada = movest.Select("(COD ='" + orow["COD"].ToString() + "') "
                                                       , "DATA,TIPO");

                orow.BeginEdit();
                orow["COD"] = "3" + orow["COD"].ToString().Substring(0, 3);
                orow.EndEdit();

                foreach (DataRow orowmov in movestfiltrada.AsEnumerable())
                {
                    orowmov.BeginEdit();
                    orowmov["COD"] = orow["COD"].ToString();
                    orowmov.EndEdit();

                }


                //MessageBox.Show(orow["COD"].ToString() + " = " + orow2["COD"].ToString());
                //}

            }

            try
            {
                oadapter_movest.Update(movest);
                oadapter_cadest.Update(cadest);
            }
            catch
            {

                MessageBox.Show("Erro atualização");
                throw;
            }
            return true;
        }
        // esse funcao está no  
        static public decimal ptRound(Decimal valor, Int16 numero)
        {
            //return Math.Round(valor, numero);
            return TDataControlTrabalho.ptRound(valor, numero);

            /*        Decimal[] divisor = new Decimal[6] { 10.0M, 100.0M, 1000.0M, 10000.0M, 100000.0M, 1000000.0M };
                    Int32 numero2 = numero - 1;
                    if (numero2 > 5)
                    {
                        MessageBox.Show("Limite de PtTruncRound = 5");
                        return valor;
                    }
                    if (numero2 < 0)
                    {
                        MessageBox.Show("Limite de PtTruncRound >= 1");
                        return valor;
                    }
                    try
                    {
                        Decimal vlrint = Math.Round(Decimal.Multiply(valor, divisor[numero2]), 5);
                        vlrint = Roundparabaixo(vlrint, numero);
                        decimal result = Math.Round(Decimal.Divide(vlrint, divisor[numero2]), numero);
                        return (result);
                    }
                    catch
                    {
                        return valor;
                    }*/
        }


        



        public DataTable RelatorioDeposito(DateTime inicioper, DateTime fimper)
        {
            this.inicio = Convert.ToDateTime("01/01/1990");
            this.fim = fimper;
            
            oadapter_cadest.SelectCommand.Parameters[0].Value = this.inicio;//TDataControlReduzido.FormatDataGravar(inicio);
            oadapter_cadest.SelectCommand.Parameters[1].Value = fim;// TDataControlReduzido.FormatDataGravar(fim);
            cadest = new DataTable("cadest");
            oadapter_cadest.Fill(cadest);

            oadapter_movest.SelectCommand.Parameters[0].Value = this.inicio;//TDataControlReduzido.FormatDataGravar(inicio);
            oadapter_movest.SelectCommand.Parameters[1].Value = fim;// TDataControlReduzido.FormatDataGravar(fim);
            movest = new DataTable("movest");
            oadapter_movest.Fill(movest);

            DataTable reldep = new DataTable("CrieRelat");
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "COD", 4, false));
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "DESCR", 55, false));
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SDOANT", 0, false));
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ENTRADA", 0, false));
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SAIDA", 0, false));
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SDOATUAL", 0, false));
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "DEP", 2, false));
            reldep.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "UNID", 3, false));

            var qritensentrada = from linhacad in cadest.AsEnumerable()
                                 join linhamov in movest.AsEnumerable()
                                 on linhacad.Field<string>("COD") equals
                                     linhamov.Field<string>("COD")

                                 where (linhamov.Field<DateTime>("DATA") >= inicioper
                                 && linhamov.Field<DateTime>("DATA") <= fimper)
                                 && (linhamov.Field<string>("TIPO") == "E")
                                 group linhamov by new
                                 {
                                     cod = linhamov.Field<string>("COD"),
                                     descr = linhacad.Field<string>("DESC"),
                                     unid = linhacad.Field<string>("UNID"),
                                     deposito = linhamov.Field<string>("DEPOSITO")
                                     //safra = linhamov.Field<string>("Safra")
                                 } into g
                                 orderby g.Key.cod, g.Key.deposito
                                 select new
                                        {
                                            cod = g.Key.cod,
                                            descricao = g.Key.descr,
                                            deposito = g.Key.deposito,
                                            unid = g.Key.unid,
                                            total = g.Sum((linhamov => linhamov.Field<Decimal?>("QUANT")))
                                        };

            var qritenssaida = from linhacad in cadest.AsEnumerable()
                               join linhamov in movest.AsEnumerable()
                               on linhacad.Field<string>("COD") equals
                                   linhamov.Field<string>("COD")

                               where (linhamov.Field<DateTime>("DATA") >= inicioper
                               && linhamov.Field<DateTime>("DATA") <= fimper)
                               && (linhamov.Field<string>("TIPO") == "S")
                               group linhamov by new
                               {
                                   cod = linhamov.Field<string>("COD"),
                                   descr = linhacad.Field<string>("DESC"),
                                   unid = linhacad.Field<string>("UNID"),
                                   deposito = linhamov.Field<string>("DEPOSITO")
                                   //safra = linhamov.Field<string>("Safra")
                               } into g
                               orderby g.Key.cod, g.Key.deposito
                               select new
                               {
                                   cod = g.Key.cod,
                                   descricao = g.Key.descr,
                                   unid = g.Key.unid,
                                   deposito = g.Key.deposito,
                                   total = g.Sum((linhamov => linhamov.Field<Decimal?>("QUANT")))
                               };


            var qrsdoentrada = from linhacad in cadest.AsEnumerable()
                               join linhamov in movest.AsEnumerable()
                               on linhacad.Field<string>("COD") equals
                                   linhamov.Field<string>("COD")

                               where (linhamov.Field<DateTime>("DATA") < inicioper
                               )
                               && (linhamov.Field<string>("TIPO") == "E")
                               group linhamov by new
                               {
                                   cod = linhamov.Field<string>("COD"),
                                   descr = linhacad.Field<string>("DESC"),
                                   unid = linhacad.Field<string>("UNID"),
                                   deposito = linhamov.Field<string>("DEPOSITO")
                                   //safra = linhamov.Field<string>("Safra")
                               } into g
                               orderby g.Key.cod, g.Key.deposito
                               select new
                               {
                                   cod = g.Key.cod,
                                   descricao = g.Key.descr,
                                   unid = g.Key.unid,
                                   deposito = g.Key.deposito,
                                   total = g.Sum((linhamov => linhamov.Field<Decimal?>("QUANT")))
                               };

            var qrsdosaida = from linhacad in cadest.AsEnumerable()
                             join linhamov in movest.AsEnumerable()
                             on linhacad.Field<string>("COD") equals
                                 linhamov.Field<string>("COD")

                             where (linhamov.Field<DateTime>("DATA") < inicioper
                             )
                             && (linhamov.Field<string>("TIPO") == "S")
                             group linhamov by new
                             {
                                 cod = linhamov.Field<string>("COD"),
                                 descr = linhacad.Field<string>("DESC"),
                                 unid = linhacad.Field<string>("UNID"),
                                 deposito = linhamov.Field<string>("DEPOSITO")
                                 //safra = linhamov.Field<string>("Safra")
                             } into g
                             orderby g.Key.cod, g.Key.deposito
                             select new
                             {
                                 cod = g.Key.cod,
                                 descricao = g.Key.descr,
                                 unid = g.Key.unid,
                                 deposito = g.Key.deposito,
                                 total = g.Sum((linhamov => linhamov.Field<Decimal?>("QUANT")))
                             };
            DataRow orow = null;

            foreach (var campo in qritensentrada)
            {
                if (campo.total == 0) continue;

                DataRow[] orows = reldep.Select("(COD = '" + campo.cod + "')" +
                                               " AND (DEP = '" + campo.deposito + "')");

                if (orows.Count() == 0)
                {
                    orow = reldep.NewRow();
                    orow.BeginEdit();
                    orow["COD"] = campo.cod;
                    orow["DEP"] = campo.deposito;
                    orow["DESCR"] = campo.descricao;
                    orow["UNID"] = campo.unid;
                    orow["SDOANT"] = 0.00M;
                    orow["SDOATUAL"] = 0.00M;
                    orow["ENTRADA"] = 0.00M;
                    orow["SAIDA"] = 0.00M;
                   
                    reldep.Rows.Add(orow);
                }
                else
                {
                    foreach (DataRow orowp in orows.AsEnumerable())
                    {
                        if (orowp["DEP"].ToString() == campo.deposito)
                        {
                            orow = orowp;
                            break;
                        }
                    }
                    orow.BeginEdit();
                }



                orow["ENTRADA"] = campo.total;
                orow.EndEdit();
            }
            foreach (var campo in qritenssaida)
            {
                if (campo.total == 0) continue;
                DataRow[] orows = reldep.Select("(COD = '" + campo.cod + "')" +
                                               " AND (DEP = '" + campo.deposito + "')");
                if (orows.Count() == 0)
                {
                    orow = reldep.NewRow();
                    orow.BeginEdit();
                    orow["COD"] = campo.cod;
                    orow["DEP"] = campo.deposito;
                    orow["DESCR"] = campo.descricao;
                    orow["UNID"] = campo.unid;
                    orow["SDOANT"] = 0.00M;
                    orow["SDOATUAL"] = 0.00M;
                    orow["ENTRADA"] = 0.00M;
                    orow["SAIDA"] = 0.00M;
                   
                    reldep.Rows.Add(orow);
                }
                else
                {
                    foreach (DataRow orowp in orows.AsEnumerable())
                    {
                        if (orowp["DEP"].ToString() == campo.deposito)
                        {
                            orow = orowp;
                            break;
                        }
                    }
                    orow.BeginEdit();
                }

                orow["SAIDA"] = campo.total;
                orow.EndEdit();
            }
            foreach (var campo in qrsdoentrada)
            {
                if (campo.total == 0) continue;
                DataRow[] orows = reldep.Select("(COD = '" + campo.cod + "')" +
                                               " AND (DEP = '" + campo.deposito + "')");
                if (orows.Count() == 0)
                {
                    orow = reldep.NewRow();
                    orow.BeginEdit();
                    orow["COD"] = campo.cod;
                    orow["DEP"] = campo.deposito;
                    orow["DESCR"] = campo.descricao;
                    orow["UNID"] = campo.unid;
                    orow["SDOANT"] = 0.00M;
                    orow["SDOATUAL"] = 0.00M;
                    orow["ENTRADA"] = 0.00M;
                    orow["SAIDA"] = 0.00M;
                   
                    reldep.Rows.Add(orow);
                }
                else
                {
                    foreach (DataRow orowp in orows.AsEnumerable())
                    {
                        if (orowp["DEP"].ToString() == campo.deposito)
                        {
                            orow = orowp;
                            break;
                        }
                    }
                    orow.BeginEdit();
                }


                orow["SDOANT"] = campo.total;
                orow.EndEdit();
            }
            foreach (var campo in qrsdosaida)
            {
                if (campo.total == 0) continue;
                DataRow[] orows = reldep.Select("(COD = '" + campo.cod + "')" +
                                               " AND (DEP = '" + campo.deposito + "')");
                if (orows.Count() == 0)
                {
                    orow = reldep.NewRow();
                    orow.BeginEdit();
                    orow["COD"] = campo.cod;
                    orow["DEP"] = campo.deposito;
                    orow["DESCR"] = campo.descricao;
                    orow["UNID"] = campo.unid;
                    orow["SDOANT"] = 0.00M;
                    orow["SDOATUAL"] = 0.00M;
                    orow["ENTRADA"] = 0.00M;
                    orow["SAIDA"] = 0.00M;
                   
                    reldep.Rows.Add(orow);
                }
                else
                {
                    foreach (DataRow orowp in orows.AsEnumerable())
                    {
                        if (orowp["DEP"].ToString() == campo.deposito)
                        {
                            orow = orowp;
                            break;
                        }
                    }
                    orow.BeginEdit();
                }
                orow["SDOANT"] = Convert.ToDecimal(orow["SDOANT"]) - campo.total;
                orow.EndEdit();
            }
            foreach (DataRow orowmov in reldep.AsEnumerable())
            {
                orowmov["SDOATUAL"] = Convert.ToDecimal(orowmov["SDOANT"]) + Convert.ToDecimal(orowmov["ENTRADA"])
                        - Convert.ToDecimal(orowmov["SAIDA"]);
            }
            return reldep;
        }

        public void RelatorioExcel_Deposito(DataTable tabprov,DateTime inicioper,DateTime fimper,string tipoord)
        {

            Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J',
                                'K',  'L','M','N','O','P','Q','R','S','T','U'};
            Int16[] Colunas = new Int16[21] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };

            FolderBrowserDialog fbd = new FolderBrowserDialog();//New FolderBrowserDialog
            fbd.SelectedPath = System.Windows.Forms.Application.StartupPath;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                Microsoft.Office.Interop.Excel.ApplicationClass ExcelObj = new Microsoft.Office.Interop.Excel.ApplicationClass();

                try
                {
                    if (ExcelObj == null)
                    {
                        MessageBox.Show("ERROR: EXCEL não pôde ser iniciado!");
                        return;
                    }
                    ExcelObj.Visible = false;                                          // read only
                    object template = "";
                    DateTime currentDateTime = DateTime.Now;
                    string nomerelatorio = "Estoque_Depositos";

                    Microsoft.Office.Interop.Excel.Workbook oWorkbook = ExcelObj.Workbooks.Add(template);

                    try
                    {
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;
                        while (sheets.Count > 1)
                        {
                            Microsoft.Office.Interop.Excel._Worksheet oworksheet2 = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(sheets.Count);
                            oworksheet2.Delete();
                        }

                        try
                        {
                            RelatorioDepositos(sheets,tabprov, inicioper,fimper,tipoord);
                        }
                        catch
                        {
                            MessageBox.Show("Problemas no Relatorio Erros");
                            throw;
                        }



                        string format = "MMMMd";
                        try
                        {
                            //  oWorkbook.Close(true, oworksheet.Name + currentDateTime.ToString(format) + ".xlsx", fbd.SelectedPath);

                            string diretorio = fbd.SelectedPath + "\\" + nomerelatorio + currentDateTime.ToString(format);
                            oWorkbook.SaveAs(diretorio,
                               Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
                               Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true, null, null, true);
                        }
                        catch
                        {
                            try
                            {
                                oWorkbook.Close(false, nomerelatorio + currentDateTime.ToString(format), fbd.SelectedPath);
                            }
                            catch { throw; }
                        }
                        ExcelObj.Quit();

                    }
                    catch (Exception)
                    {
                        ExcelObj.Quit();
                        MessageBox.Show("Erro Excel ", "",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
                finally
                {
                    if (ExcelObj != null)
                        ExcelObj.Quit();

                    Cursor.Current = Cursors.Default;

                }
            }


        }

        private void RelatorioDepositos(Microsoft.Office.Interop.Excel.Sheets sheets, DataTable tabprov,DateTime inicioper,DateTime fimper,string tipoord)
        {

            var depositos = 
                         from linha in tabprov.AsEnumerable()
                         group linha by new
                         {
                             deposito = linha.Field<string>("DEP")

                         } into g
                         orderby g.Key.deposito
                         select new
                         {
                             deposito = g.Key.deposito
                         };


            int i = 0;
            foreach (var campo  in depositos.AsEnumerable())
            {

                Microsoft.Office.Interop.Excel._Worksheet oworksheet;
                if (i == 0)
                {
                    oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);
                    oworksheet.Name = "Deposito_" + campo.deposito + "";

                }
                else
                {
                    oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.Add(Type.Missing, sheets.get_Item(sheets.Count), Type.Missing, Type.Missing);
                    oworksheet.Name = "Deposito_" + campo.deposito + "";
                }
                i = i + 1;
                IEnumerable<DataRow> deposito_filtrado;
                if (tipoord == "D")
                {
                   deposito_filtrado =
                   from linha_ in tabprov.AsEnumerable()
                   where (linha_.Field<string>("DEP") == campo.deposito)
                   orderby linha_.Field<string>("DESCR")
                   select linha_;
                }
                else
                {
                    deposito_filtrado =
                     from linha_ in tabprov.AsEnumerable()
                     where (linha_.Field<string>("DEP") == campo.deposito)
                     orderby linha_.Field<string>("COD")
                     select linha_;
                
                }

                Microsoft.Office.Interop.Excel.Range orange = oworksheet.get_Range("A1", "A1");

                orange.Value2 = "Relatorio Almoxarifado Periodo:" + inicioper.ToString("d") +" a "+ fimper.ToString("d");
               
                orange = oworksheet.get_Range("A3", "A3");

               
                // fornecedor
               // orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 5;
                orange.Value2 = "Codigo";

                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 40;
                orange.Value2 = "Descriçâo";

                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 5;
                orange.Value2 = "Unid.";


                orange = orange.get_Offset(0, 1);
                //Entrada
                orange.ColumnWidth = 14;
                orange.Value2 = "  Saldo Anterior";
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,000";



                orange = orange.get_Offset(0, 1);
                //Entrada
                orange.ColumnWidth = 14;
                orange.Value2 = "Entrada";
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,000";


                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 14;
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,00";
                orange.Value2 = "Saida";

                // saldo atual
                orange = orange.get_Offset(0, 1);
                orange.ColumnWidth = 14;
                orange.Value2 = "Saldo Atual";
                orange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                orange.EntireColumn.NumberFormat = "#.###.##0,000";


                Microsoft.Office.Interop.Excel.Range orangeini;// = oworksheet.get_Range("A4", "A4");

                int linha = 4;

              
            

                foreach (DataRow orowmov in deposito_filtrado.AsEnumerable())
                {
                    orangeini = oworksheet.get_Range("A" + linha.ToString().Trim(), "A" + linha.ToString().Trim());
                    linha = linha + 1;


                    orange = orangeini.get_Offset(0, 0);
                    orange.Value2 = orowmov["COD"];

                    orange = orangeini.get_Offset(0, 1);
                    orange.Value2 = orowmov["DESCR"];

                    orange = orangeini.get_Offset(0, 2);
                    orange.Value2 = orowmov["UNID"]; 


                    orange = orangeini.get_Offset(0, 3);
                    orange.Value2 = Convert.ToDecimal(orowmov["SDOANT"]); 

                    orange = orangeini.get_Offset(0, 4);
                    orange.Value2 = Convert.ToDecimal(orowmov["ENTRADA"]);
                    orange = orangeini.get_Offset(0, 5);
                    orange.Value2 = Convert.ToDecimal(orowmov["SAIDA"]);
                    orange = orangeini.get_Offset(0, 6);
                    orange.Value2 = Convert.ToDecimal(orowmov["SDOATUAL"]);
                }

            }

        }
        
    }
    /*
     * 

     * procedure TFrmRelDeposito.ApronteDetalhe(linhaGeral:integer);
var
ponto : Tpoint;
tcod: string;
j,i,ind1,colunas,linhas, ind_dep: integer;
tvalor, tquant,tcust_me : double;
tdata_ini ,inicio, fim, tdata_cus: TDateTime;
//Ano,MEs,Dia : Word;
ListaDep : TStrings;
oDeposito : TDeposito;
begin

ListaDep := TStringList.Create;

if linhaGeral = -1 then
   LinhaGeral := sgCompara.Row;

if LinhaGeral = -1  then exit;

tcod   := sgCompara.Cells[1,LinhaGeral];

with dmtrab.tblCadEst do
begin
  if active = false then active := true;
  indexname := 'ICES1';
  indexdefs.Update;
  setkey;
  fieldbyname('COD').asString := tcod;
  if not gotokey then
     exit;
  tdata_ini := fieldbyName('DATA_CAD').asDateTime;
end;


tquant:= 0;
tvalor:= 0 ;

tcust_me := 0; //ptROUND(tVALOR/tQUANT,4);

 with SgMeses do
 begin
   enabled := false;
   visible := false;
   Width  := 20;
   sbTotMeses.Top := ( TabSheet1.Height - sbTotMeses.Height - 12);
   sbTotMeses.visible := false;
   ponto := GetAveCharSize(Canvas);
   for ind1 := 0 to RowCount - 1 do
       Rows[ind1].Clear;
   for ind1 := 0 to ColCount - 1 do
       Cols[ind1].Clear;

   ColCount :=  1 + 4 + 3;  // data,tipo,quant,valor ,...(depositos)
   FixedRows := 1;
   FixedCols := 1;
  // ponto.x := 6;
   RowCount := 2;
      // tam da coluna fixa
   ColWidths[0] := ponto.x;
      // Montagem do cabecário de 2 linhas
      // ind2
   Colunas := 1;
   linhas  := 0;

   Cells[colunas,linhas] :=  ' Data ';
   ColWidths[colunas] := 10 * ponto.x;
   INC(Colunas,1);

    Cells[colunas,linhas] :=  'Tipo';
    ColWidths[colunas] := 4* ponto.x;
    INC(Colunas,1);

    Cells[colunas,linhas] :=  'Quant';
    ColWidths[colunas] := 12* ponto.x;
    INC(Colunas,1);

    Cells[colunas,linhas] :=  'Valor(R$)';
    ColWidths[colunas] := 12* ponto.x;
    INC(Colunas,1);

    Cells[colunas,linhas] :=  'Saldo';
    ColWidths[colunas] := 12* ponto.x;
    INC(Colunas,1);

    Cells[colunas,linhas] :=  'Saldo(R$)';
    ColWidths[colunas] := 12* ponto.x;
    INC(Colunas,1);

    Cells[colunas,linhas] :=  'C.Médio(R$)';
    ColWidths[colunas] := 12* ponto.x;
    INC(Colunas,1);



    colunas := FixedCols;

    linhas  := FixedRows;



   with dmtrab.tblMovEst do
   begin
     if active = false then active := true;
     indexname := 'IMES2';
     indexdefs.Update;
     setkey;
     fieldbyname('COD').asString := tcod;
     fieldbyname('DATA').asDateTime := tdata_ini;
     gotonearest;
     while (not eof) and (tcod = fieldbyname('COD').asString) do
     begin
      if (RowCount-1) < linhas then
          RowCount := Linhas + 1 ;

       if fieldbyName('TIPO2').asString <> 'T' then
       begin
          if FieldbyName('TIPO').asString  = 'E' then
          begin
            if (FieldbyName('VALOR').asFloat <> 0) then
              tvalor := tvalor + FieldbyName('VALOR').asFloat
            else
              tvalor := ptROUND(tcust_me*FieldbyName('QUANT').asFloat,2);  // caso de uma devolucao de entrada que seja nao uma compra e sim uma devolucao de mercadoria

            tquant := tquant + FieldbyName('QUANT').asFloat;
            if (tquant <> 0) and (tvalor <> 0) then
            begin
               tcust_me  := ptROUND(tVALOR/tQUANT,4);
               tdata_cus := FieldbyName('DATA').asDateTime;
            end;

          end
          else if fieldbyName('TIPO').asString = 'S' then
          begin
            tvalor := tvalor- ptROUND(tcust_me*FieldbyName('QUANT').asFloat,2);
            tquant := tquant - FieldbyName('QUANT').asFloat;
          end;
          Cells[1,linhas] := datetostr(FieldbyName('DATA').asDatetime);
          Cells[2,linhas] := FieldbyName('TIPO').asString;
          Cells[3,linhas] := CompleteString(Canvas,floattostrf(FieldbyName('QUANT').asFloat,ffnumber,12,2),ColWidths[3],false);
          if Fieldbyname('TIPO').asString = 'E' then
          begin
             if fieldbyName('VALOR').asFloat <> 0 then
                Cells[4,linhas] := CompleteString(Canvas,floattostrf(fieldbyName('VALOR').asFloat,ffnumber,12,2),ColWidths[4],false)
             else
                Cells[4,linhas] := CompleteString(Canvas,floattostrf(ptRound(tcust_me*fieldbyName('QUANT').asFloat,2),ffnumber,12,2),ColWidths[4],false);
          end
          else
             Cells[4,linhas] := CompleteString(Canvas,floattostrf(ptRound(tcust_me*fieldbyName('QUANT').asFloat,2),ffnumber,12,2),ColWidths[4],false);
       end
       else
       begin
          Cells[1,linhas] := datetostr(FieldbyName('DATA').asDatetime);
          Cells[2,linhas] := FieldbyName('TIPO2').asString;
          Cells[3,linhas] := CompleteString(Canvas,floattostrf(FieldbyName('QUANT').asFloat,ffnumber,12,2),ColWidths[3],false);
          Cells[4,linhas] := CompleteString(Canvas,floattostrf(fieldbyName('VALOR').asFloat,ffnumber,12,2),ColWidths[4],false)
       end;
       Cells[5,linhas] := CompleteString(Canvas,floattostrf(tquant,ffnumber,12,2),ColWidths[5],false);
       Cells[6,linhas] := CompleteString(Canvas,floattostrf(tvalor,ffnumber,12,2),ColWidths[6],false);
       Cells[7,linhas] := CompleteString(Canvas,floattostrf(tcust_me,ffnumber,12,2),ColWidths[7],false);
       ind_dep := ListaDep.IndexOf(FieldbyName('Deposito').asString);
       if ind_dep = -1 then
       begin
          ColCount := ColCount + 1 ;
          oDeposito := TDeposito.Create;
          oDeposito.Coluna := ColCount-1;
          oDeposito.Total := 0;
          Cells[oDeposito.Coluna,0] :=  'Dep.'+FieldbyName('Deposito').asString;
          ColWidths[oDeposito.Coluna] := 12* ponto.x;
          ListaDep.AddObject(FieldbyName('Deposito').asString,oDeposito);
          ind_dep := ListaDep.Count -1;
       end
       else
          oDeposito := TDeposito(ListaDep.Objects[ind_dep]);

       if FieldbyName('TIPO').asString = 'E' then
       begin
          odeposito.Total := odeposito.Total + FieldbyName('QUANT').asFloat;
          Cells[oDeposito.Coluna,linhas] := CompleteString(Canvas,floattostrf(FieldbyName('QUANT').asFloat,ffnumber,12,2),ColWidths[oDeposito.Coluna],false);
       end
       else
       begin
          odeposito.Total := odeposito.Total - FieldbyName('QUANT').asFloat;
          Cells[oDeposito.Coluna,linhas] := CompleteString(Canvas,floattostrf(FieldbyName('QUANT').asFloat * -1,ffnumber,12,2),ColWidths[oDeposito.Coluna],false);
       end;
       ListaDep.Objects[ind_dep] :=    oDeposito;
       next;
       Inc(linhas,1);
     end;
  end;
  if (RowCount-1) < linhas then
          RowCount := Linhas + 1 ;
  Cells[1,linhas] := 'Totais =>';
  for i := 0 to ListaDep.Count - 1 do
  begin
     oDeposito := TDeposito(ListaDep.Objects[i]);
     Cells[oDeposito.Coluna,linhas] := CompleteString(Canvas,floattostrf(oDeposito.Total,ffnumber,12,2),ColWidths[oDeposito.Coluna],false);
  end;

  Width  := 20;
  Height := DefaultRowHeight * (RowCount+1);

  i := 0;
  while (Top + Height ) > sbTotMeses.Top  do
  begin
     INC(i,1);
     Height := DefaultRowHeight * (RowCount-i);
  end;
  for i := 0 to (ColCount - 1) do
  begin
      Width := Width + ColWidths[i]+1;
  end;
  if width > (TabSheet2.Width - 30) then
     width := (TabSheet2.Width - 30);
  enabled := true;
  visible := true;
end;


     * 
     * 


 TabProv := TTable.Create(self);
 with TabProv do
 begin
     Active := false;
     DataBaseName := 'DBESTOQUE';
     TableName := 'TPESQ1';
     TableType := ttFoxPro;
     if exists then
     begin
        num := 1;
        while (TenteExcluir(TabProv) = false) and (num < 10) do
        begin
           INC(num,1);
           tabProv.TableName := 'TPESQ'+ trim(inttostr(num));
        end;
     end;

     with FieldDefs do begin
       Clear;
       Add( 'COD',ftString,4,false);
       Add( 'DESC',ftString,35,false);
       Add( 'SDOANT',ftFloat,0,false);
       Add( 'ENTRADA',ftFloat,0,false);
       Add( 'SAIDA',ftFloat,0,false);
     end;
     with IndexDefs do begin
       Clear;
       Add('ITEMP1','COD',[ixExpression]);
       Add('ITEMP2','DESC',[ixExpression]);
     end;

     try

       CreateTable;
     except
       ShowMessage('Erro ao criar Tabela Temporaria');
        Destroy;
        exit;
     end;
  end;







//try
tdeposito := '';
if (cbDeposito.ItemIndex > - 1)  then
  tdeposito := NovoObj(cbDeposito.Items.Objects[cbDeposito.ItemIndex]).cod;

tcod := '';
if (CbItens.ItemIndex > - 1)  then
  tcod := NovoObj(CbItens.Items.Objects[CbItens.ItemIndex]).cod;

with QrItensEntrada do
begin
 Close;
 Params.Clear;
 Params.CreateParam(ftDate,'PDATA1',ptunKnown);
// Params.CreateParam(ftString,'PCOD',ptunKnown);
 Params.CreateParam(ftDate,'PDATA2',ptunKnown);
 if tcod <> '' then
    Params.CreateParam(ftString,'PCOD',ptunKnown);
 if tdeposito <> '' then
    Params.CreateParam(ftString,'PDEPOSITO',ptunKnown);
 With SQL do
 begin
   Clear;
   Add('Select Cod,cadest."Desc" as Descr ,sum(movest.Quant) as tot  FROM Cadest as Cadest');
   Add('LEFT OUTER JOIN  ":DBESTOQUE:MOVEST" as Movest ON (cadest.cod = movest.cod)');
   Add(' where ');
   Add(' ( MOVEST.DATA BETWEEN :PDATA1 and :PDATA2)');
   Add(' and (TIPO = ''E'')');
   if tdeposito <> '' then
      Add(' and (DEPOSITO = :PDEPOSITO)');
   if tcod <> '' then
      Add(' and (COD = :PCOD)');
   Add(' group by cod,cadest."Desc" ');
      Add(' order by Cod ');
 end;
  Prepare;
end;

with QrItensSaida do
begin
 Close;
 Params.Clear;
 Params.CreateParam(ftDate,'PDATA1',ptunKnown);
  Params.CreateParam(ftDate,'PDATA2',ptunKnown);
 if tcod <> '' then
    Params.CreateParam(ftString,'PCOD',ptunKnown);
 if tdeposito <> '' then
    Params.CreateParam(ftString,'PDEPOSITO',ptunKnown);
 With SQL do
 begin
   Clear;
   Add('Select Cod,cadest."Desc" as Descr ,sum(movest.Quant) as tot  FROM Cadest as Cadest');
   Add('LEFT OUTER JOIN  ":DBESTOQUE:MOVEST" as Movest ON (cadest.cod = movest.cod)');
   Add(' where ');
   Add(' ( MOVEST.DATA BETWEEN :PDATA1 and :PDATA2)');
   Add(' and (TIPO = ''S'')');
   if tdeposito <> '' then
      Add(' and (DEPOSITO = :PDEPOSITO)');
   if tcod <> '' then
      Add(' and (COD = :PCOD)');
   Add(' group by cod,cadest."Desc" ');
      Add(' order by Cod ');
 end;
  Prepare;
end;

with QrSdoEnt do
begin
 Close;
 Params.Clear;
 Params.CreateParam(ftDate,'PDATA1',ptunKnown);
 if tcod <> '' then
    Params.CreateParam(ftString,'PCOD',ptunKnown);
 if tdeposito <> '' then
    Params.CreateParam(ftString,'PDEPOSITO',ptunKnown);
 With SQL do
 begin
   Clear;
   Add('Select Cod,cadest."Desc" as Descr ,sum(movest.Quant) as tot  FROM Cadest as Cadest');
   Add('LEFT OUTER JOIN  ":DBESTOQUE:MOVEST" as Movest ON (cadest.cod = movest.cod)');
   Add(' where ');
   Add(' ( MOVEST.DATA < :PDATA1 )');
   Add(' and (TIPO = ''E'')');
   if tdeposito <> '' then
      Add(' and (DEPOSITO = :PDEPOSITO)');
   if tcod <> '' then
      Add(' and (COD = :PCOD)');
   Add(' group by cod,cadest."Desc" ');
   Add(' order by Cod ');
 end;
  Prepare;
end;



with QrSdoSai do
begin
 Close;
 Params.Clear;
 Params.CreateParam(ftDate,'PDATA1',ptunKnown);
 if tcod <> '' then
    Params.CreateParam(ftString,'PCOD',ptunKnown);
 if tdeposito <> '' then
    Params.CreateParam(ftString,'PDEPOSITO',ptunKnown);
 With SQL do
 begin
   Clear;
   Add('Select Cod,cadest."Desc" as Descr ,sum(movest.Quant) as tot  FROM Cadest as Cadest');
   Add('LEFT OUTER JOIN  ":DBESTOQUE:MOVEST" as Movest ON (cadest.cod = movest.cod)');
   Add(' where ');
   Add(' ( MOVEST.DATA < :PDATA1 )');
   Add(' and (TIPO = ''S'')');
   if tdeposito <> '' then
      Add(' and (DEPOSITO = :PDEPOSITO)');
   if tcod <> '' then
      Add(' and (COD = :PCOD)');
   Add(' group by cod,cadest."Desc" ');
   Add(' order by Cod ');
 end;
 Prepare;
end;

if tdeposito <> '' then
begin
  qritensEntrada.ParamByName('PDEPOSITO').asString := tdeposito;
  qritensSaida.ParamByName('PDEPOSITO').asString := tdeposito;
  qrSdoEnt.ParamByName('PDEPOSITO').asString := tdeposito;
  qrSdoSai.ParamByName('PDEPOSITO').asString := tdeposito;
end;

if (tcod <> '') then
begin
  qritensEntrada.ParamByName('PCOD').asString := tcod;
  qritensSaida.ParamByName('PCOD').asString := tcod;
  qrSdoEnt.ParamByName('PCOD').asString := tcod;
  qrSdosai.ParamByName('PCOD').asString := tcod;
end;

qritensEntrada.ParamByName('PDATA1').asDateTime := dtdata1.Date;
qritensEntrada.ParamByName('PDATA2').asDateTime := dtdata2.Date;

qritensSaida.ParamByName('PDATA1').asDateTime := dtdata1.Date;
qritensSaida.ParamByName('PDATA2').asDateTime := dtdata2.Date;

qrSdoEnt.ParamByName('PDATA1').asDateTime := dtdata1.Date;
qrSdoSai.ParamByName('PDATA1').asDateTime := dtdata1.Date;

qrItensEntrada.Active := true;
qrItenssaida.Active := true;
qrSdoEnt.Active := true;
qrSdoSai.Active := true;
with tabprov do
begin
  tabprov.IndexName := 'ITEMP1';
  active := true;
  tabprov.FieldDefs.Update;
end;

if not qrItensEntrada.Eof then qrItensEntrada.First;
if not qrItenssaida.Eof then qrItensSaida.First;
if not qrSdoEnt.Eof then qrSdoEnt.First;
if not qrSdoSai.Eof then qrSdoSai.First;


while (not qrItensEntrada.Eof) and
       (not qrItenssaida.Eof) do
begin
    if qrItensEntrada.Fieldbyname('COD').asString =
       qrItensSaida.Fieldbyname('COD').asString  then
    begin
      if (qrItensEntrada.Fieldbyname('TOT').asFloat = 0) and
      ( qrItensSaida.Fieldbyname('TOT').asFloat = 0)  then
       begin
        qrItensSaida.Next;
        qrItensEntrada.Next;
        continue;
       end;
       with tabprov do
       begin
         append;
         FieldbyName('COD').asString := qrItensEntrada.Fieldbyname('COD').asString;
         FieldbyName('DESC').asString := qrItensEntrada.Fieldbyname('DESCR').asString;
         fieldbyName('ENTRADA').asFloat :=  qrItensEntrada.Fieldbyname('TOT').asFloat;
         fieldbyName('SAIDA').asFloat :=  qrItensSaida.Fieldbyname('TOT').asFloat;
         POST;
       end;
       qrItensSaida.Next;
       qrItensEntrada.Next;
    end
    else
      if (qrItensEntrada.Fieldbyname('COD').asString <
       qrItensSaida.Fieldbyname('COD').asString)  then
      begin
         with tabprov do
         begin
           append;
           FieldbyName('COD').asString := qrItensEntrada.Fieldbyname('COD').asString;
           FieldbyName('DESC').asString := qrItensEntrada.Fieldbyname('DESCR').asString;
           fieldbyName('ENTRADA').asFloat :=  qrItensEntrada.Fieldbyname('TOT').asFloat;
           POST;
         end;
         qrItensEntrada.Next;
      end
      else
      begin
         with tabprov do
         begin
           append;
           FieldbyName('COD').asString := qrItensSaida.Fieldbyname('COD').asString;
           FieldbyName('DESC').asString := qrItensSaida.Fieldbyname('DESCR').asString;
           fieldbyName('Saida').asFloat :=  qrItensSaida.Fieldbyname('TOT').asFloat;
           post;
         end;
         qrItensSaida.Next;
      end;
end;

while (not qrItensEntrada.Eof) do
begin
  with tabprov do
  begin
      append;
      FieldbyName('COD').asString := qrItensEntrada.Fieldbyname('COD').asString;
      FieldbyName('DESC').asString := qrItensEntrada.Fieldbyname('DESCR').asString;
      fieldbyName('ENTRADA').asFloat :=  qrItensEntrada.Fieldbyname('TOT').asFloat;
      POST;
   end;
  qrItensEntrada.Next;
end;
while (not qrItensSaida.Eof) do
begin
  with tabprov do
  begin
     append;
     FieldbyName('COD').asString := qrItensSaida.Fieldbyname('COD').asString;
     FieldbyName('DESC').asString := qrItensSaida.Fieldbyname('DESCR').asString;
     fieldbyName('Saida').asFloat :=  qrItensSaida.Fieldbyname('TOT').asFloat;
     POST;
  end;
  qrItensSaida.Next;
end;


while (not qrSdoEnt.Eof) and
     (not qrSdoSai.Eof)
do
begin
 if qrSdoEnt.Fieldbyname('COD').asString =
    QrSdoSai.Fieldbyname('COD').asString  then
 begin
     if (qrSdoEnt.Fieldbyname('TOT').isnull) and
      (qrSdoSai.Fieldbyname('TOT').isnull) then
     begin
        QrSdoEnt.Next;
        QrSdoSai.Next;
        continue;
     end;
     with TabProv do
     begin
       Setkey;
       FieldbyName('COD').asString := QrSDoEnt.FieldbyName('COD').asString;
       if not gotoKey then
       begin
         append;
         FieldbyName('COD').asString := qrSdoEnt.Fieldbyname('COD').asString;
         FieldbyName('DESC').asString := qrSdoEnt.Fieldbyname('DESCR').asString;
         Fieldbyname('SDOANT').asFloat := QrSdoEnt.Fieldbyname('TOT').asFloat - QrSdoSai.Fieldbyname('TOT').asFloat;
         post;
       end
       ELSE
       begin
         EDIT;
         Fieldbyname('SDOANT').asFloat := QrSdoEnt.Fieldbyname('TOT').asFloat - QrSdoSai.Fieldbyname('TOT').asFloat;
         POST;
       end;
     end;
     QrSdoEnt.Next;
     QrSdoSai.Next;
 end
 else
    if qrSdoEnt.Fieldbyname('COD').asString < qrSdoSai.Fieldbyname('COD').asString then
    begin
      if (qrSdoEnt.Fieldbyname('TOT').asFloat = 0) then
      begin
        QrSdoEnt.Next;
        continue;
      end;
      with TabProv do
      begin
        Setkey;
        FieldbyName('COD').asString := QrSDoEnt.FieldbyName('COD').asString;
        if not gotoKey then
        begin
          append;
          FieldbyName('COD').asString := QrSdoEnt.Fieldbyname('COD').asString;
          FieldbyName('DESC').asString := QrSdoEnt.Fieldbyname('DESCR').asString;
          Fieldbyname('SDOANT').asFloat := Fieldbyname('SDOANT').asFloat + QrSdoEnt.Fieldbyname('TOT').asFloat;
          post;
         end
         ELSE
         begin
           EDIT;
           Fieldbyname('SDOANT').asFloat := Fieldbyname('SDOANT').asFloat + QrSdoEnt.Fieldbyname('TOT').asFloat;
           POST;
         end;
      end;
      QrSdoEnt.Next;
    end
    else
    begin
      if (qrSdoSai.Fieldbyname('TOT').asFloat = 0) then
      begin
        QrSdoSai.Next;
        continue;
      end;
      with TabProv do
      begin
        Setkey;
        FieldbyName('COD').asString := QrSdoSai.FieldbyName('COD').asString;
        if not gotoKey then
        begin
          append;
          FieldbyName('COD').asString := QrSdoSai.Fieldbyname('COD').asString;
          FieldbyName('DESC').asString := QrSdoSai.Fieldbyname('DESCR').asString;
          Fieldbyname('SDOANT').asFloat := Fieldbyname('SDOANT').asFloat - QrSdoSai.Fieldbyname('TOT').asFloat;
          post;
         end
         ELSE
         begin
          EDIT;
          Fieldbyname('SDOANT').asFloat := Fieldbyname('SDOANT').asFloat - QrSdoSai.Fieldbyname('TOT').asFloat;
          POST;
         end;
      end;
      QrSdoSai.Next;
    end;
end;

while (not qrSdoEnt.Eof)
do
begin
 if (qrSdoEnt.Fieldbyname('TOT').asFloat = 0) then
 begin
    QrSdoEnt.Next;
    continue;
 end;
 with TabProv do
 begin
    Setkey;
    FieldbyName('COD').asString := QrSdoEnt.FieldbyName('COD').asString;
    if not gotoKey then
    begin
      append;
      FieldbyName('COD').asString := QrSdoEnt.Fieldbyname('COD').asString;
      FieldbyName('DESC').asString := QrSdoEnt.Fieldbyname('DESCR').asString;
    end
    ELSE
      EDIT;
    Fieldbyname('SDOANT').asFloat := Fieldbyname('SDOANT').asFloat + QrSdoEnt.Fieldbyname('TOT').asFloat;
    POST;
  end;
  QrSdoEnt.Next;
end;

while (not qrSdoSai.Eof)
do
begin
 if (qrSdoSai.Fieldbyname('TOT').asFloat = 0) then
 begin
    QrSdoSai.Next;
    continue;
 end;
 with TabProv do
 begin
    Setkey;
    FieldbyName('COD').asString := QrSdoSai.FieldbyName('COD').asString;
    if not gotoKey then
    begin
      append;
      FieldbyName('COD').asString := QrSdoSai.Fieldbyname('COD').asString;
      FieldbyName('DESC').asString := QrSdoSai.Fieldbyname('DESCR').asString;
    end
    ELSE
      EDIT;
    Fieldbyname('SDOANT').asFloat := Fieldbyname('SDOANT').asFloat - QrSdoSai.Fieldbyname('TOT').asFloat;
    POST;
  end;
  QrSdoSai.Next;
end;

if rbAlfa.Checked = true then
begin
  tabprov.IndexName := 'ITEMP2';
  tabprov.FieldDefs.Update;
end;
tabProv.First;


with SgCompara do
begin
   totarea := 0;
   enabled := false;
   visible := false;
   Width  := 20;
   sbTotais.Top := ( TabSheet1.Height - sbTotais.Height - 12);
   sbTotais.visible := false;
   ponto := GetAveCharSize(Canvas);
   for ind1 := 0 to RowCount - 1 do
       Rows[ind1].Clear;
   for ind1 := 0 to ColCount - 1 do
       Cols[ind1].Clear;


   ColCount := 6+1;

   FixedRows := 1;
   FixedCols := 1;

   RowCount := 2;
      // tam da coluna fixa
   ColWidths[0] := ponto.x;
      // Montagem do cabecário de 2 linhas
      // ind2
   Colunas := 1;
   linhas  := 0;



   Cells[colunas,linhas] :=  'Codigo';
   ColWidths[colunas] := 6* ponto.x;
   INC(Colunas,1);

  Cells[colunas,linhas] :=  'Descricao';
  ColWidths[colunas] := 30* ponto.x;
  INC(Colunas,1);

  ColWidths[colunas] := 10* ponto.x;
  Cells[colunas,linhas] := completestring(Canvas,'Sdo Ant',ColWidths[colunas],false);
  INC(Colunas,1);


  ColWidths[colunas] := 10* ponto.x;
  Cells[colunas,linhas] := completestring(Canvas,'Entrada',ColWidths[colunas],false);
  INC(Colunas,1);

  ColWidths[colunas] := 10* ponto.x;
  Cells[colunas,linhas] := completestring(Canvas,'  Saida',ColWidths[colunas],false);
  INC(Colunas,1);

  ColWidths[colunas] := 10* ponto.x;
  Cells[colunas,linhas] := completestring(Canvas,'  Saldo',ColWidths[colunas],false);
  INC(Colunas,1);

  linhas  := FixedRows;
  while not TabProv.Eof do
  begin
    tsdo := TabProv.Fieldbyname('SDOANT').asFloat + TabProv.Fieldbyname('ENTRADA').asFloat -
              TabProv.Fieldbyname('SAIDA').asFloat;
     if tsdo = 0 then
     begin
       tabprov.Next;
       continue;
     end;
     if (RowCount-1) < linhas then   RowCount := Linhas + 1 ;
     Cells[1,linhas] := TabProv.Fieldbyname('COD').asString;
     Cells[2,linhas] := TabProv.Fieldbyname('DESC').asString;
     Cells[3,linhas] :=  CompleteString(Canvas,floattostrf(TabProv.Fieldbyname('SDOANT').asFloat,ffnumber,10,2),ColWidths[3],false);
     Cells[4,linhas] :=  CompleteString(Canvas,floattostrf(TabProv.Fieldbyname('ENTRADA').asFloat,ffnumber,10,2),ColWidths[3],false);
     Cells[5,linhas] :=  CompleteString(Canvas,floattostrf(TabProv.Fieldbyname('SAIDA').asFloat,ffnumber,10,2),ColWidths[3],false);
     tsdo := TabProv.Fieldbyname('SDOANT').asFloat + TabProv.Fieldbyname('ENTRADA').asFloat -
              TabProv.Fieldbyname('SAIDA').asFloat;
     Cells[6,linhas] :=  CompleteString(Canvas,floattostrf(tsdo,ffnumber,10,2),ColWidths[3],false);
     TabProv.Next;

     INC(linhas,1);
   end;
  Width  := 20;
   for i := 0 to (ColCount - 1) do
   begin
      Width := Width + ColWidths[i]+1;

   end;


   enabled := true;
   visible := true;
  // sbTotais.Visible := true;



end;
qrItensEntrada.Active := false;
qrItensSaida.Active := false;
qrSdoEnt.Active := false;
qrSdoSai.Active := false;

TabProv.Active := false;

//Guia.Free;
Screen.Cursor := velhoCursor;



     */




    /*  function ptRound(tvalor : double; numero : cardinal):double;
  const
    divisor : array[1..5] of Integer = (10,100,1000,10000,100000);
  begin
  result := tvalor;
  if numero > 5 then
  begin
    ShowMessage('Limite de PtRound = 5');
    exit;
  end;

  try
   result := (round(tvalor * divisor[numero])/divisor[numero]);
  except
   result := tvalor;
   raise;
  end;
  end;
      */


}
