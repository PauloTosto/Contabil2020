using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Office.Interop.Excel;
using System.Security.Claims;
using System.Drawing.Text;
//using ClassConexao;


namespace ClassFiltroEdite
{
    //passos do montegrid
    public class ptDataGridView : DataGridView
    {
        private Boolean pegou_scroll;
        public System.Windows.Forms.ScrollBar ptHorizontalScrollBar()
        {
            return this.HorizontalScrollBar;
        }
        public ptDataGridView() : base()
        {

            pegou_scroll = false;
        }
        public Boolean pegou
        {
            get
            {
                return pegou_scroll;
            }
            set
            {
                pegou_scroll = value;
            }

        }
    }

    public class Campo
    {
        public string titulo;
        public string cabecalho;
        public string format;
        public string nulltext;
        public int tamanho;
        public Boolean soma;
        public double total;
        public double totalgeral;
        public Campo(string otitulo, string ocabecalho, int otamanho, string oformat, bool osoma, double ototal, String onulltext)
        {
            titulo = otitulo;
            cabecalho = ocabecalho;
            soma = osoma;
            tamanho = otamanho;
            total = ototal;
            totalgeral = 0.00;
            format = oformat;
            nulltext = onulltext;
        }
        public Campo(string otitulo)
        {
            titulo = otitulo;
            cabecalho = "";
            soma = false;
            tamanho = 0;
            total = 0.00;
            totalgeral = 0.00;
            format = "";
            nulltext = "";
        }
    }


    public class MonteGrid
    {
        public List<Campo> LinhasCampo;
        public DataGrid oDataGrid;
        public DataGridView oDataGridView;
        public System.Windows.Forms.StatusBar sbTotal;
        public System.Windows.Forms.Panel pn_sbTotal;
        public System.Windows.Forms.StatusBar sbTotalGeral;
        public System.Windows.Forms.StatusBar sbTotalExtra;
        // acrescido nesta versão o mais moderno strip
        public System.Windows.Forms.StatusStrip ssTotal;
        public System.Windows.Forms.StatusStrip ssTotalGeral;
        public System.Windows.Forms.StatusStrip ssTotalExtra;

        public bool modeloAlternativo = false;

        public int LinhasMinimas = 3;
        private Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                                  'L','M','N','O','P','Q','R','S','T','U'};



        public string tituloExcel = "";
        public int linhasInicioExcel = 4;
        public int colunaCabecalhoExcel = 0;

        public event EventHandler<TotalizaEventArgs> Totalizando;
        private Boolean adicioneEventHandle;
        private CultureInfo cultura = CultureInfo.CreateSpecificCulture("pt-BR");
        public Dictionary<string, object> dictCampoTotal;
        protected virtual void OnTotaliza(TotalizaEventArgs e)
        {
            EventHandler<TotalizaEventArgs> handler = Totalizando;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        static public string Reformate(string campo)
        {
            string resultado = "";
            for (int i = 0; i < campo.Length; i++)
            {
                //char ochar = Convert.ToChar(campo.Substring(i,1));
                if (campo.Substring(i, 1) == ".")
                {
                    resultado += ",";
                    continue;
                }
                if (campo.Substring(i, 1) == ",")
                {
                    resultado += ".";
                    continue;
                }

                resultado += campo.Substring(i, 1);
            }

            return resultado;

        }



        public MonteGrid()
        {
            LinhasCampo = new List<Campo>();
            adicioneEventHandle = false;
        }
        public void Add(string otitulo)  // para ficar compativel com TArmeEdicao
        {
            LinhasCampo.Add(new Campo(otitulo));
        }
        public void AddValores(string otitulo, string ocabecalho, int otamanho, string oformat, bool osoma, double ototal, string onulltext)
        {
            LinhasCampo.Add(new Campo(otitulo, ocabecalho, otamanho, oformat, osoma, ototal, onulltext));
        }

        public void Clear() //para ficar +- compativel com TarmeEdicao (delphi 7)
        {
            LinhasCampo.Clear();
        }
        public void ConfigureDBGridView()
        {
            string odatamember = "";
            DataSet odataset = null;
            DataView odataview = null;
            dictCampoTotal = new Dictionary<string, object>();
            if (oDataGridView == null) return;
            if (modeloAlternativo)
            {
                ConfigureDBGridViewAlternativo();
                return;
            }
            if (oDataGridView.DataSource is BindingSource)
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is DataSet)
                {
                    odataset = (DataSet)((BindingSource)oDataGridView.DataSource).DataSource;
                    odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
                }
                else
                {
                    if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                    {
                        odataview = (DataView)((BindingSource)oDataGridView.DataSource).DataSource;
                        odatamember = odataview.Table.TableName;
                    }
                    else
                        if (((BindingSource)oDataGridView.DataSource).DataSource is BindingSource)
                    {
                        //  odataset = ((BindingSource)oDataGridView.DataSource).DataSource;
                        odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
                    }
                    else
                        if (((BindingSource)oDataGridView.DataSource).DataSource is System.Data.DataTable)
                    {
                        odatamember = ((System.Data.DataTable)((BindingSource)oDataGridView.DataSource).DataSource).TableName;
                        odataset = ((System.Data.DataTable)((BindingSource)oDataGridView.DataSource).DataSource).DataSet;
                    }
                }
            }
            else
            {
                odataset = (DataSet)oDataGridView.DataSource;
                odatamember = oDataGridView.DataMember;
            }
            if (odatamember == "") return;

            DataView otable = null;
            try
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is System.Data.DataTable)
                    otable = (((BindingSource)oDataGridView.DataSource).DataSource as System.Data.DataTable).AsDataView();
                else if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                    otable = ((BindingSource)oDataGridView.DataSource).DataSource as DataView;

            }
            catch (Exception)
            {

            }



            oDataGridView.Columns.Clear();

            // alterando padrões automáticos do DataGridView
            oDataGridView.AutoGenerateColumns = false;
            oDataGridView.RowHeadersWidth = 30;
            oDataGridView.ReadOnly = true;
            oDataGridView.AllowUserToAddRows = false;
            oDataGridView.AllowUserToDeleteRows = false;
            oDataGridView.StandardTab = true;


            oDataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            DataGridViewTextBoxColumn oTextCol;
            DataGridViewCheckBoxColumn oBoolCol;

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            columnHeaderStyle.BackColor = Color.Beige;

            columnHeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7, FontStyle.Italic);
            columnHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            oDataGridView.ColumnHeadersDefaultCellStyle = columnHeaderStyle;


            foreach (Campo ocampo in LinhasCampo)
            {

                Type tipo = null;
                if (otable != null)
                {
                    if (otable.Table.Columns.Contains(ocampo.titulo))
                    { tipo = otable.Table.Columns[ocampo.titulo].DataType; }
                }
                if (tipo == Type.GetType("System.Boolean"))
                {
                    oBoolCol = new DataGridViewCheckBoxColumn();
                    oBoolCol.DataPropertyName = ocampo.titulo;
                    DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
                    oBoolCol.HeaderText = ocampo.cabecalho;
                    cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    if (ocampo.tamanho > 0)
                        oBoolCol.Width = ocampo.tamanho * Convert.ToInt16(oDataGridView.Font.Size);
                    if (ocampo.format != "")
                    {
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        cellStyle.Format = ocampo.format;
                    }

                    cellStyle.NullValue = ocampo.nulltext;

                    oBoolCol.DefaultCellStyle = cellStyle;
                    int numcol = oDataGridView.Columns.Add(oBoolCol);
                    oDataGridView.Columns[numcol].Name = ocampo.titulo;
                }
                
                else
                {
                    oTextCol = new DataGridViewTextBoxColumn();
                    oTextCol.DataPropertyName = ocampo.titulo;
                    DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

                    oTextCol.HeaderText = ocampo.cabecalho;
                    cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    if (ocampo.tamanho > 0)
                        oTextCol.Width = ocampo.tamanho * Convert.ToInt16(oDataGridView.Font.Size);
                    if (ocampo.format != "")
                    {
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        cellStyle.Format = ocampo.format;
                    }

                    cellStyle.NullValue = ocampo.nulltext;

                    oTextCol.DefaultCellStyle = cellStyle;
                    int numcol = oDataGridView.Columns.Add(oTextCol);
                    oDataGridView.Columns[numcol].Name = ocampo.titulo;
                }

            }
       //     oDataGridView.Height = oDataGridView.ColumnHeadersHeight + (oDataGridView.RowTemplate.Height * LinhasMinimas) + oDataGridView.HorizontalScrollingOffset;

            oDataGridView.CellFormatting += new DataGridViewCellFormattingEventHandler(oDataGridView_CellFormatting);
            ConfigureTotalizadores();
        }

        private void ConfigureTotalizadores()
        {
            if (sbTotal != null)
            {
                if (sbTotal.Parent is ScrollableControl)
                { // force Autoscrool
                    if (!(((ScrollableControl)sbTotal.Parent).AutoScroll))
                        ((ScrollableControl)sbTotal.Parent).AutoScroll = true;
                }
                sbTotal.Width = 0; //14;
                sbTotal.Panels.Clear();
                sbTotal.ShowPanels = true;
                int fim = oDataGridView.DisplayedColumnCount(true);
                int inicio = 0;
                int cabecaLinhasTam = oDataGridView.RowHeadersWidth;
                try
                {
                    if (oDataGridView.IsAccessible == true)
                    {
                        inicio = oDataGridView.FirstDisplayedCell.ColumnIndex;
                        // cabecaLinhasTam = 0;
                    }
                }
                catch { inicio = 0; }

                if ((sbTotal.Parent is ScrollableControl) && (((ScrollableControl)sbTotal.Parent).AutoScroll))
                {
                    fim = oDataGridView.ColumnCount;
                    for (int i = inicio; i < fim; i++)
                    {
                        sbTotal.Panels.Add("");
                        sbTotal.Panels[i].AutoSize = StatusBarPanelAutoSize.None;
                        //if oDataGridView.DisplayedColumnCount
                        sbTotal.Panels[i].Width = oDataGridView.Columns[i].Width;// oDataGridView.Columns[i].Width;

                        sbTotal.Width = sbTotal.Width + oDataGridView.Columns[i].Width;
                        if (LinhasCampo[i].soma)
                        {
                            sbTotal.Panels[sbTotal.Panels.Count - 1].BorderStyle = StatusBarPanelBorderStyle.Raised;// pbLowered;
                            sbTotal.Panels[sbTotal.Panels.Count - 1].Style = StatusBarPanelStyle.Text;
                            sbTotal.Panels[sbTotal.Panels.Count - 1].Alignment = HorizontalAlignment.Right;
                        }
                    }
                    sbTotal.Top = oDataGridView.Top + oDataGridView.Height + 2;
                    if (oDataGridView.GetColumnDisplayRectangle(0, true).Left == 0)
                    {
                        sbTotal.Left = oDataGridView.Left + cabecaLinhasTam;
                    }
                    else
                        sbTotal.Left = oDataGridView.Left + oDataGridView.GetColumnDisplayRectangle(0, true).Left;
                }
                else
                {
                    for (int i = inicio; i < fim; i++)
                    {
                        sbTotal.Panels.Add("");
                        sbTotal.Panels[i].AutoSize = StatusBarPanelAutoSize.None;
                        //if oDataGridView.DisplayedColumnCount
                        sbTotal.Panels[i].Width = oDataGridView.GetColumnDisplayRectangle(i, true).Width <
                                sbTotal.Panels[i].Width ? sbTotal.Panels[i].Width : oDataGridView.GetColumnDisplayRectangle(i, true).Width;

                        sbTotal.Width = sbTotal.Width + oDataGridView.GetColumnDisplayRectangle(i, true).Width;
                        if (LinhasCampo[i].soma)
                        {
                            sbTotal.Panels[sbTotal.Panels.Count - 1].BorderStyle = StatusBarPanelBorderStyle.Raised;// pbLowered;
                            sbTotal.Panels[sbTotal.Panels.Count - 1].Style = StatusBarPanelStyle.Text;
                            sbTotal.Panels[sbTotal.Panels.Count - 1].Alignment = HorizontalAlignment.Right;
                        }
                    }
                    sbTotal.Top = oDataGridView.Top + oDataGridView.Height + 2;

                    if (oDataGridView.GetColumnDisplayRectangle(0, true).Left == 0)
                    {
                        sbTotal.Left = oDataGridView.Left + cabecaLinhasTam;
                    }
                    else
                        sbTotal.Left = oDataGridView.Left + oDataGridView.GetColumnDisplayRectangle(0, true).Left;
                }
                //   sbTotal.Left = oDataGridView.Left;
            }
            if (sbTotalGeral != null)
            {
                sbTotalGeral.Width = 0; //14;
                sbTotalGeral.Panels.Clear();
                sbTotalGeral.ShowPanels = true;
                for (int i = 0; i < sbTotal.Panels.Count; i++)
                {
                    sbTotalGeral.Panels.Add("");
                    sbTotalGeral.Panels[i].AutoSize = StatusBarPanelAutoSize.None;
                    sbTotalGeral.Panels[i].Width = sbTotal.Panels[i].Width;
                    sbTotalGeral.Width = sbTotal.Width;// +oDataGridView.Columns[i].Width;//DBGrid.Columns[i].Width;
                    if (LinhasCampo[i].soma)
                    {
                        sbTotalGeral.Panels[sbTotalGeral.Panels.Count - 1].BorderStyle = StatusBarPanelBorderStyle.Raised;// pbLowered;
                        sbTotalGeral.Panels[sbTotalGeral.Panels.Count - 1].Style = StatusBarPanelStyle.Text;
                        sbTotalGeral.Panels[sbTotalGeral.Panels.Count - 1].Alignment = HorizontalAlignment.Right;
                    }
                }
                sbTotalGeral.Top = sbTotal.Top + sbTotal.Height + 4; //oDataGridView.Top + oDataGridView.Height + 2;
                sbTotalGeral.Left = sbTotal.Left;
            }
            // posiciona default (top e left)
            if (sbTotalExtra != null)
            {

                if (sbTotalGeral != null)
                {
                    sbTotalExtra.Top = sbTotalGeral.Top + sbTotalGeral.Height + 4;
                    sbTotalExtra.Left = sbTotalGeral.Left;
                }
                else
                    if (sbTotal != null)
                {
                    sbTotalExtra.Top = sbTotal.Top + sbTotal.Height + 4;
                    sbTotalExtra.Left = sbTotal.Left;
                }
                else
                {
                    sbTotalExtra.Top = oDataGridView.Top + oDataGridView.Height + 2;
                    sbTotalExtra.Left = oDataGridView.Left + oDataGridView.GetColumnDisplayRectangle(0, true).Left;
                }
            }

            // NOVO STRIP
            if (ssTotal != null)
            {
                ssTotal.Items.Clear();
                ssTotal.Dock = DockStyle.None;
                ssTotal.SizingGrip = false;
                ssTotal.Width = 0;    //0; //14;
                ssTotal.AutoSize = false;
                int fim = oDataGridView.DisplayedColumnCount(true);
                fim = oDataGridView.ColumnCount;
                int index = 0;
                try
                {
                    if (oDataGridView.IsAccessible == true)
                        index = oDataGridView.FirstDisplayedCell.ColumnIndex;
                }
                catch { index = 0; }
                for (int i = index; i < fim; i++)
                {

                    ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                    ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                    ssTotal.Items[ssTotal.Items.Count - 1].Text = "";

                    ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = false;

                    ssTotal.Items[ssTotal.Items.Count - 1].Width = oDataGridView.Columns[i].Width;

                    ssTotal.Width = ssTotal.Width + oDataGridView.Columns[i].Width + 2;
                    if (LinhasCampo[i].soma)
                    {
                        ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Right;
                        ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleRight;
                    }
                }
                ssTotal.Top = 0;//  oDataGridView.Top + oDataGridView.Height + 2;
                ssTotal.Left = ssTotal.Left + oDataGridView.GetColumnDisplayRectangle(0, true).Left + 6;
                ssTotal.Left = ssTotal.Left + (ssTotal.Parent != null ? ssTotal.Parent.Left : 0 ) ;
            }

            if (ssTotalExtra != null)
            {

                if (ssTotalGeral != null)
                {
                    ssTotalExtra.Top = ssTotalGeral.Top + ssTotalGeral.Height + 4;
                    ssTotalExtra.Left = ssTotalGeral.Left;
                }
                else
                    if (ssTotal != null)
                {
                    ssTotalExtra.Top = ssTotal.Top + ssTotal.Height + 4;
                    ssTotalExtra.Left = ssTotal.Left;
                }
                else
                {
                    ssTotalExtra.Top = oDataGridView.Top + oDataGridView.Height + 2;
                    ssTotalExtra.Left = oDataGridView.Left + oDataGridView.GetColumnDisplayRectangle(0, true).Left;
                }
            }
        }


        public void ConfigureDBGridViewAlternativo()
        {
            string odatamember = "";
            if (oDataGridView == null) return;
           
            if (oDataGridView.DataSource is BindingSource)
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is Enumerable)
                {

                    odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
                    if (odatamember == "")
                        odatamember = oDataGridView.DataMember;

                }
            }
            oDataGridView.Parent.Visible = true;
            oDataGridView.Columns.Clear();

            oDataGridView.Parent.BackColor = SystemColors.ActiveBorder;

            
            oDataGridView.Enter -= new System.EventHandler(this.Padrao_Enter);
            oDataGridView.Leave -= new System.EventHandler(this.Padrao_Leave);
            oDataGridView.Enter += new System.EventHandler(this.Padrao_Enter);
            oDataGridView.Leave += new System.EventHandler(this.Padrao_Leave);


            oDataGridView.BorderStyle = BorderStyle.None;
            oDataGridView.BackgroundColor = SystemColors.ControlLight;
            oDataGridView.AutoGenerateColumns = false;


            //  Estilo dos Cabeçalhos
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle = oDataGridView.ColumnHeadersDefaultCellStyle.Clone();

            columnHeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8, FontStyle.Italic);
            columnHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            columnHeaderStyle.WrapMode = DataGridViewTriState.False;
            oDataGridView.ColumnHeadersDefaultCellStyle = columnHeaderStyle;




            // do indicador de Linhas
            DataGridViewCellStyle rowHeaderStyle = new DataGridViewCellStyle();
            rowHeaderStyle = oDataGridView.RowHeadersDefaultCellStyle.Clone();
            rowHeaderStyle.BackColor = Color.Beige;
            rowHeaderStyle.SelectionForeColor = Color.AliceBlue;
            oDataGridView.RowHeadersDefaultCellStyle = rowHeaderStyle;
            oDataGridView.RowHeadersWidth = 26;


            DataGridViewTextBoxColumn oTextCol;
            int tamDatagridView;
            tamDatagridView = oDataGridView.RowHeadersWidth + oDataGridView.Margin.Horizontal;

            DataView otable = null;
            try
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is System.Data.DataTable)
                    otable = (((BindingSource)oDataGridView.DataSource).DataSource as System.Data.DataTable).AsDataView();
                else if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                    otable = ((BindingSource)oDataGridView.DataSource).DataSource as DataView;

            }
            catch (Exception) {
            
            }

            foreach (Campo ocampo in LinhasCampo)
            {
                oTextCol = new DataGridViewTextBoxColumn();
                oTextCol.DataPropertyName = ocampo.titulo;
                // estidlo desta célula
                DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
                cellStyle = oTextCol.DefaultCellStyle.Clone();
                DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
                headerCellStyle = columnHeaderStyle.Clone();


                cellStyle.WrapMode = DataGridViewTriState.False;
                cellStyle.NullValue = ocampo.nulltext;
                cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                Type tipo = null;
                if (otable != null)
                {
                    if (otable.Table.Columns.Contains(ocampo.titulo))
                    { tipo = otable.Table.Columns[ocampo.titulo].DataType; }
                }

                oTextCol.HeaderText = ocampo.cabecalho;
                if (oTextCol.HeaderText.Length > ocampo.tamanho)
                    oTextCol.Width = oTextCol.HeaderText.Length * Convert.ToInt16(oDataGridView.Font.Size);
                else
                    oTextCol.Width = ocampo.tamanho * Convert.ToInt16(oDataGridView.Font.Size);
                if (tipo != null)
                {
                    if ((tipo != Type.GetType("System.String")) && (tipo != Type.GetType("System.Boolean")) && (tipo != Type.GetType("System.DateTime")))
                    {
                        int tam = (ocampo.format.Length > 0 ? ocampo.format.Length : 5) * Convert.ToInt16(oDataGridView.Font.Size);
                        if (tam > oTextCol.Width)
                            oTextCol.Width = tam;
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        cellStyle.Format = ocampo.format;
                        headerCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    if ((tipo == Type.GetType("System.DateTime")) && (ocampo.format != ""))
                    {
                        int tam = ocampo.format.Length * Convert.ToInt16(oDataGridView.Font.Size);
                        if (tam > oTextCol.Width)
                            oTextCol.Width = tam;
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        cellStyle.Format = ocampo.format;
                    }
                }
                else
                {
                    if (ocampo.format != "")
                    {
                        int tam = ocampo.format.Length * Convert.ToInt16(oDataGridView.Font.Size);
                        if (tam > oTextCol.Width)
                            oTextCol.Width = tam;
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        cellStyle.Format = ocampo.format;
                    }
                }
                oTextCol.DefaultCellStyle = cellStyle;
                oTextCol.HeaderCell.Style = headerCellStyle;

                int numcol = oDataGridView.Columns.Add(oTextCol);
                oDataGridView.Columns[numcol].Name = ocampo.titulo;
                oDataGridView.Columns[numcol].SortMode = DataGridViewColumnSortMode.Automatic;
                tamDatagridView = tamDatagridView + oDataGridView.Columns[numcol].Width
                    + oDataGridView.Columns[numcol].DividerWidth;
            }

            if ((oDataGridView.ScrollBars == System.Windows.Forms.ScrollBars.Both) ||
              (oDataGridView.ScrollBars == System.Windows.Forms.ScrollBars.Vertical))
            {
                // espaço do scroolBar vertical
                tamDatagridView = tamDatagridView + 16;
            }

            oDataGridView.Width = tamDatagridView; //+ oDataGridView.Columns[0].DividerWidth; ;

            oDataGridView.Height = oDataGridView.ColumnHeadersHeight + (oDataGridView.RowTemplate.Height * LinhasMinimas) + oDataGridView.HorizontalScrollingOffset;

            oDataGridView.Parent.Width = oDataGridView.Width + 20;
            oDataGridView.Left = 10;

            System.Windows.Forms.Label titulo = null;
            foreach (Control control in oDataGridView.Parent.Controls)
            {
                if (control is System.Windows.Forms.Label)
                {
                    titulo = (control as System.Windows.Forms.Label);
                    break;
                }
            }
            ConfigureTotalizadores();
        }


        // FOCUS ENTER x FOCUS LEAVE
        private void Padrao_Enter(object sender, EventArgs e)
        {
            (sender as DataGridView).RowsDefaultCellStyle.BackColor = Color.AliceBlue;
            (sender as DataGridView).RowHeadersDefaultCellStyle.BackColor = Color.AliceBlue;

        }

        private void Padrao_Leave(object sender, EventArgs e)
        {
            (sender as DataGridView).RowsDefaultCellStyle.BackColor = Color.White;
            (sender as DataGridView).RowHeadersDefaultCellStyle.BackColor = Color.White;

        }











        public void FuncaoSoma() //bool geral
        {
            List<Campo> somecampos = new List<Campo>();
            foreach (Campo ocampo in LinhasCampo)
            {
                if (!ocampo.soma) continue;

                ocampo.total = 0;
                somecampos.Add(ocampo);
            }
            dictCampoTotal.Clear();
            // caso da tabela vazia
            DataView otable;
            try
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                {
                    otable = ((BindingSource)oDataGridView.DataSource).DataSource as DataView;
                }
                else if (((BindingSource)oDataGridView.DataSource).DataSource is System.Data.DataTable)
                {
                    otable = (((BindingSource)oDataGridView.DataSource).DataSource as System.Data.DataTable).AsDataView();
                }
                else return;

                if (otable.Table.Rows.Count == 0) return;


            }
            catch (Exception)
            {

                return;
            }

            //

            foreach (DataRowView orow in otable)
            {
                foreach (Campo ocampo in somecampos)
                {
                    if (!otable.Table.Columns.Contains(ocampo.titulo)) continue;
                    if (orow.Row.IsNull(ocampo.titulo)) continue;
                    { ocampo.total = ocampo.total + Convert.ToDouble(orow[ocampo.titulo]); }

                }
            }
            foreach (Campo ocampo in somecampos)
            {
                if (!otable.Table.Columns.Contains(ocampo.titulo)) continue;
                dictCampoTotal.Add(ocampo.titulo, ocampo.total);

            }




        }
        //padrao para superar display data = 30/12/1899
        void oDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (((DataGridView)sender).Columns[e.ColumnIndex].ValueType == Type.GetType("System.DateTime"))
            {
                try
                {
                    String stringValue;
                    try
                    {
                        if (e.Value == null)
                        {
                            e.Value = "";
                            stringValue = "";
                        }
                        else
                            stringValue = ((DateTime)e.Value).ToString("d", this.cultura);// as string;
                    }
                    catch
                    {
                        e.Value = "";
                        stringValue = "";
                    }
                    if (stringValue == "30/12/1899")
                    {
                        e.Value = "";
                        stringValue = "";
                    }
                    DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                    cell.ToolTipText = stringValue;
                }
                catch
                {
                    throw;
                }
            }
        }

        public void EncontraTotais()
        {
            if (oDataGrid == null) return;
            string odatamember = "";
            DataSet odataset = null;
            DataView odataview = null;
            if (oDataGrid == null) return;
            if (oDataGrid.DataSource is BindingSource)
            {
                if (((BindingSource)oDataGrid.DataSource).DataSource is DataSet)
                {
                    odataset = (DataSet)((BindingSource)oDataGrid.DataSource).DataSource;
                    odatamember = ((BindingSource)oDataGrid.DataSource).DataMember;
                }
                else
                {
                    if (((BindingSource)oDataGrid.DataSource).DataSource is DataView)
                    {
                        odataview = (DataView)((BindingSource)oDataGrid.DataSource).DataSource;
                        odatamember = odataview.Table.TableName;
                    }
                }
            }
            else
            {
                odataset = (DataSet)oDataGrid.DataSource;
                odatamember = oDataGrid.DataMember;
            }


            if (odatamember == "") return;
            for (int i = 0; i < LinhasCampo.Count; i++)
            {
                if (LinhasCampo[i].soma)
                {
                    if (odataset != null)
                    {
                        System.Nullable<decimal> total1 =
                  (from valor1 in odataset.Tables[odatamember].AsEnumerable()
                   select valor1.Field<decimal>(LinhasCampo[i].titulo)).Sum();
                        LinhasCampo[i].total = Convert.ToDouble(total1);
                    }
                    else
                    {
                        System.Nullable<decimal> total1 =
                  (from valor1 in odataview.Table.AsEnumerable()
                   select valor1.Field<decimal>(LinhasCampo[i].titulo)).Sum();
                        LinhasCampo[i].total = Convert.ToDouble(total1);
                    }
                }
            }
        }
        public void EncontraTotaisView()
        {
            if (oDataGridView == null) return;
            string odatamember = "";
            DataSet odataset = null;
            DataView odataview = null;
            if (oDataGridView == null) return;
            if (oDataGridView.DataSource is BindingSource)
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is DataSet)
                {
                    odataset = (DataSet)((BindingSource)oDataGridView.DataSource).DataSource;
                    odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
                }
                else
                {
                    if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                    {
                        odataview = (DataView)((BindingSource)oDataGridView.DataSource).DataSource;
                        odatamember = odataview.Table.TableName;
                    }
                }
            }
            else
            {
                if (oDataGridView.DataSource is DataSet)
                {

                    odataset = (DataSet)oDataGridView.DataSource;
                    odatamember = oDataGridView.DataMember;
                }
                else
                    if (oDataGridView.DataSource is System.Data.DataTable)
                {

                    odataview = (DataView)((System.Data.DataTable)oDataGridView.DataSource).DefaultView;
                    odatamember = odataview.Table.TableName;
                }

            }


            if (odatamember == "") return;
            for (int i = 0; i < LinhasCampo.Count; i++)
            {
                if (LinhasCampo[i].soma)
                {
                    if (odataset != null)
                    {
                        System.Nullable<decimal> total1 =
                  (from valor1 in odataset.Tables[odatamember].AsEnumerable()
                   select valor1.Field<decimal>(LinhasCampo[i].titulo)).Sum();
                        LinhasCampo[i].total = Convert.ToDouble(total1);

                    }
                    else
                    {
                        System.Nullable<decimal> total1 =
                  (from valor1 in odataview.Table.AsEnumerable()
                   where (valor1.Field<Nullable<Decimal>>(LinhasCampo[i].titulo) != null)
                   select valor1.Field<decimal>(LinhasCampo[i].titulo)).Sum();
                        LinhasCampo[i].total = Convert.ToDouble(total1);
                        /*try
                        {
                            OnTotaliza(new TotalizaEventArgs(odataview.Table));
                        }
                        catch (Exception)
                        {
                            throw;
                        }*/
                    }



                }
            }
            if (odataset != null)
            {
                try
                {
                    OnTotaliza(new TotalizaEventArgs(odataset.Tables[odatamember]));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    OnTotaliza(new TotalizaEventArgs(odataview.Table));
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }


        public void ColocaTotais()
        {
            for (int i = 0; i < LinhasCampo.Count; i++)
            {
                if (LinhasCampo[i].soma)
                {
                    if (sbTotal != null)
                    {
                        if (i < sbTotal.Panels.Count)
                        {
                            if (LinhasCampo[i].format == "")
                                sbTotal.Panels[i].Text = LinhasCampo[i].total.ToString("#,###,##0.00");
                            else
                                sbTotal.Panels[i].Text = LinhasCampo[i].total.ToString(LinhasCampo[i].format);
                        }
                    }
                    if ((sbTotalGeral != null) && (i < sbTotalGeral.Panels.Count))
                        sbTotalGeral.Panels[i].Text = LinhasCampo[i].totalgeral.ToString(LinhasCampo[i].format);
                }
            }

        }
        // coloca totais do statusstrip
        public void ssColocaTotais()
        {
            if (ssTotal == null) return;
            List<Campo> somecampos = new List<Campo>();
            foreach (Campo ocampo in LinhasCampo)
            {
                if (!ocampo.soma) continue;
                somecampos.Add(ocampo);
            }

            if (somecampos.Count == 0) return;
            ssTotal.Items.Clear();
            ssTotal.Dock = DockStyle.None;
            ssTotal.SizingGrip = false;
            ssTotal.Width = 0;    //0; //14;
            ssTotal.AutoSize = false;
            // ssTotal.Top = ssTotal.Parent.Top + ;    //  oDataGridView.Top + oDataGridView.Height + 2;
            ssTotal.Left = ssTotal.Parent.Left + 5;
            // ssTotal.Left = ssTotal.Left + ssTotal.Parent.Left;

            ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
            ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
            ssTotal.Items[ssTotal.Items.Count - 1].Text = "Totais => ";
            ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = false;
            ssTotal.Items[ssTotal.Items.Count - 1].Width = "Totais => ".Length * Convert.ToInt16(ssTotal.Font.Size);
            ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Left;
            ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleLeft;
            ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;

            foreach (Campo ocampo in somecampos)
            {
                // titulo
                ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                ssTotal.Items[ssTotal.Items.Count - 1].Text = ocampo.cabecalho + ":";
                ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = false;
                ssTotal.Items[ssTotal.Items.Count - 1].Width = ocampo.cabecalho.Length * Convert.ToInt16(ssTotal.Font.Size);
                ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Right;
                ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleRight;
                ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;
                /// valor
                ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                ssTotal.Items[ssTotal.Items.Count - 1].Text = ocampo.total.ToString(ocampo.format).Trim();
                ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = false;
                ssTotal.Items[ssTotal.Items.Count - 1].Width = ocampo.total.ToString(ocampo.format).Trim().Length * Convert.ToInt16(ssTotal.Font.Size);
                ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Left;
                ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleLeft;
                ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;

                ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                ssTotal.Items[ssTotal.Items.Count - 1].Text = "";
                ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = false;
                ssTotal.Items[ssTotal.Items.Count - 1].Width = 3 * Convert.ToInt16(ssTotal.Font.Size);
                ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Right;
                ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleCenter;
                ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;
            }
            ssTotal.Refresh();
        }

        void oDataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                if (sbTotal != null)
                {
                    int u = e.NewValue;
                    if ((e.NewValue + pn_sbTotal.HorizontalScroll.LargeChange) < pn_sbTotal.HorizontalScroll.Maximum)
                    {
                        if (e.NewValue == 0)
                        {
                            pn_sbTotal.AutoScroll = true;
                        }
                        pn_sbTotal.HorizontalScroll.Value = e.NewValue;
                        if (e.NewValue == 0)
                        {
                            pn_sbTotal.Refresh();
                            pn_sbTotal.AutoScroll = false;
                        }
                    }
                }
            }
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


        public void ExportaExcel()
        {

            if (oDataGridView == null) return;
            string odatamember = "";
            DataSet odataset = null;
            DataView odataview = null;
            if (oDataGridView == null) return;
            if (oDataGridView.DataSource is BindingSource)
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is DataSet)
                {
                    odataset = (DataSet)((BindingSource)oDataGridView.DataSource).DataSource;
                    odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
                }
                else
                {
                    if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                    {
                        odataview = (DataView)((BindingSource)oDataGridView.DataSource).DataSource;
                        odatamember = odataview.Table.TableName;
                    }
                    else if (((BindingSource)oDataGridView.DataSource).DataSource is System.Data.DataTable)
                    {
                        odataview = (((BindingSource)oDataGridView.DataSource).DataSource as System.Data.DataTable).DefaultView;
                        odatamember = odataview.Table.TableName;
                    }
                }
            }
            else
            {
                odataset = (DataSet)oDataGridView.DataSource;
                odatamember = oDataGridView.DataMember;
            }


            if (odatamember == "") return;
            if (odataview == null) return;

            /*Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                                  'L','M','N','O','P','Q','R','S','T','U'};*/
            //  Int16[] Colunas = new Int16[21] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 ,21};
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

                    Microsoft.Office.Interop.Excel.Range Oget_Range;
                    try
                    {
                        
                        int linhasinicio = linhasInicioExcel;
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                        Microsoft.Office.Interop.Excel._Worksheet oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);

                        oworksheet.Activate();
                        oworksheet.Name = "Planilha1";

                        // define linha max
                        int linhasmax = linhasinicio + odataview.Count;
                        if (ssTotal != null) linhasmax += 1;
                        if (ssTotalGeral != null) linhasmax += 1;

                        //titulo
                        int linhas = 1;
                        int numcol = colunaCabecalhoExcel;
                        string linhastr = linhas.ToString();

                        if (tituloExcel != "")
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = tituloExcel;


                        numcol = 1;
                        linhas = linhasinicio;
                        linhastr = linhas.ToString();

                        // cabeçalho
                        Type tipo = null;
                        foreach (Campo campo in LinhasCampo)
                        {
                            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                      (linhas + 1), linhasmax, oworksheet);
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
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
                            numcol = 1;
                            linhastr = linhas.ToString();
                            foreach (Campo campo in LinhasCampo)
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
                                        else { dado = Convert.ToDateTime(orow[campo.titulo]).ToString("d"); }

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
                                numcol += 1;
                            }
                            linhas += 1;
                        }
                        if (ssTotal != null)
                        {
                            numcol = 1;
                            linhastr = linhas.ToString();
                            foreach (Campo campo in LinhasCampo)
                            {
                                if (numcol == 1) oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOTAIS";
                                if (!campo.soma) { numcol += 1; continue; }
                                object dado = campo.total;
                                if (campo.format != "")
                                {
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                                }

                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                                numcol += 1;
                            }
                            linhas += 1;
                        }
                        if (ssTotalGeral != null)
                        {
                            numcol = 1;
                            linhastr = linhas.ToString();
                            foreach (Campo campo in LinhasCampo)
                            {
                                if (numcol == 1) oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOT.GERAL";
                                if (!campo.soma) { numcol += 1; continue; }
                                object dado = campo.totalgeral;
                                if (campo.format != "")
                                {
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                                }

                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = HorizontalAlignment.Right;
                                numcol += 1;

                            }
                            linhas += 1;
                        }


        
                        linhas = linhasinicio;
                        numcol = 1;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + (LinhasCampo.Count - 1)].ToString(),
                             linhas, linhasmax, oworksheet);
                        Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                        oWorkbook.SaveAs(SaveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                      "", "", false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
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
        }





        public void MestreDetalhe(MonteGrid oDetalhe)
        {
            int numcol_inicial = 0;
            if (oDataGridView == null) return;
            string odatamember = "";
            DataSet odataset = null;
            DataView odataview = null;
            if (oDataGridView == null) return;
            BindingSource MestreBSource = (oDataGridView.DataSource as BindingSource);

            if (oDataGridView.DataSource is BindingSource)
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is DataSet)
                {
                    odataset = (DataSet)((BindingSource)oDataGridView.DataSource).DataSource;
                    odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
                }
                else
                {
                    if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                    {
                        odataview = (DataView)((BindingSource)oDataGridView.DataSource).DataSource;
                        odatamember = odataview.Table.TableName;
                    }
                    else if (((BindingSource)oDataGridView.DataSource).DataSource is System.Data.DataTable)
                    {
                        odataview = (((BindingSource)oDataGridView.DataSource).DataSource as System.Data.DataTable).DefaultView;
                        odatamember = odataview.Table.TableName;
                    }
                }
            }
            else
            {
                MessageBox.Show("DataSource tem que ser BindingSource");
                return;
            }


            if (odatamember == "") return;
            if (odataview == null) return;

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

                    //  Microsoft.Office.Interop.Excel.Range Oget_Range;
                    try
                    {
                        int linhasinicio = 2;
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                        Microsoft.Office.Interop.Excel._Worksheet oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);

                        oworksheet.Activate();
                        oworksheet.Name = "Planilha1";

                        // define linha max
                        int linhasmax = linhasinicio + odataview.Count;
                        if (ssTotal != null) linhasmax += 1;
                        if (ssTotalGeral != null) linhasmax += 1;

                        //titulo
                        int linhas = 1;
                        int numcol = numcol_inicial;
                        string linhastr = linhas.ToString();

                        if (tituloExcel != "")
                            oworksheet.get_Range(Letras[numcol + 1] + linhastr, Letras[numcol + 1] + linhastr).Value2 = tituloExcel;



                        linhas = linhasinicio;
                        linhastr = linhas.ToString();

                        Microsoft.Office.Interop.Excel.Range Oget_Range;
                        Type tipo = null;
                        // cabeçalho Mestre
                        numcol = numcol_inicial;
                        int numcol_inicio = numcol;
                        numcol = Cabecalho(oworksheet, this, linhas, numcol);
                        Cabecalho(oworksheet, oDetalhe, linhas, numcol);
                        linhas += 1;
                        MestreBSource.MoveFirst();
                        foreach (DataRowView orow in (MestreBSource.DataSource as System.Data.DataTable).DefaultView)
                        {
                            MestreBSource.ResetCurrentItem();
                            numcol = numcol_inicial;
                            linhastr = linhas.ToString();
                            foreach (Campo campo in LinhasCampo)
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
                                        else { dado = Convert.ToDateTime(orow[campo.titulo]).ToString("d"); }

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
                                numcol += 1;
                            }
                            // Detalhe entra AQUI

                            //linhas += 1;
                            if (numcol_inicio != numcol)
                            {
                                Oget_Range = Colunas_exc(Letras[numcol_inicio].ToString(), Letras[numcol - 1].ToString(),
                                 linhas, linhas, oworksheet);
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                            }
                            linhas = LinhasDetalhe(oworksheet, oDetalhe, linhas, numcol);
                            MestreBSource.MoveNext();
                        }
                        if (ssTotal != null)
                        {
                            numcol = numcol_inicial;
                            linhastr = linhas.ToString();
                            foreach (Campo campo in LinhasCampo)
                            {
                                if (numcol == numcol_inicial) oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOTAIS";
                                if (!campo.soma) { numcol += 1; continue; }
                                object dado = campo.total;
                                if (campo.format != "")
                                {
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                                }

                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                                numcol += 1;
                            }
                            if (numcol_inicio != numcol)
                            {
                                Oget_Range = Colunas_exc(Letras[numcol_inicio].ToString(), Letras[numcol - 1].ToString(),
                                   linhas, linhas, oworksheet);
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                            }
                            linhas += 1;
                        }
                        if (ssTotalGeral != null)
                        {
                            numcol = numcol_inicial;
                            linhastr = linhas.ToString();
                            foreach (Campo campo in LinhasCampo)
                            {
                                if (numcol == numcol_inicial) oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOT.GERAL";
                                if (!campo.soma) { numcol += 1; continue; }
                                object dado = campo.totalgeral;
                                if (campo.format != "")
                                {
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                                }

                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = HorizontalAlignment.Right;
                                numcol += 1;

                            }
                            linhas += 1;
                        }

                        oWorkbook.SaveAs(SaveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                      "", "", false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
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
                    if (ExcelObj != null)
                    {
                        ExcelObj.Quit();
                        MessageBox.Show("Excel " + SaveFileDialog1.FileName + " Gravado Ok");
                    }
                    Cursor.Current = Cursors.Default;
                }
            }
        }
        private int LinhasDetalhe(Microsoft.Office.Interop.Excel._Worksheet oworksheet, MonteGrid oDetalhe, int linhas, int numcol)
        {
            int linhas_inicio = linhas;
            int numcol_inicio = numcol;
            BindingSource DetalheBSource = (oDetalhe.oDataGridView.DataSource as BindingSource);
            DataView odataview = (DetalheBSource.DataSource as System.Data.DataTable).DefaultView;
            string linhastr = linhas.ToString();
            linhastr = linhas.ToString();
            Type tipo = null;
            foreach (DataRowView orow in odataview)
            {
                numcol = numcol_inicio;
                linhastr = linhas.ToString();
                foreach (Campo campo in oDetalhe.LinhasCampo)
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
                            else { dado = Convert.ToDateTime(orow[campo.titulo]).ToString("d"); }

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
                    numcol += 1;
                }
                linhas += 1;
            }
            if (linhas_inicio == linhas) { linhas += 1; }

            if (numcol_inicio != numcol)
            {
                Microsoft.Office.Interop.Excel.Range Oget_Range;
                Oget_Range = Colunas_exc(Letras[numcol_inicio].ToString(), Letras[numcol - 1].ToString(),
                     linhas_inicio, linhas - 1, oworksheet);
                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                Oget_Range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;

            }            //  Oget_Range.Borders.Color = Color.Black;

            // linhas += 1;

            return linhas;
        }

        private int Cabecalho(Microsoft.Office.Interop.Excel._Worksheet oworksheet, MonteGrid monteGrid, int linhas, int numcol)
        {
            int linhas_inicio = linhas;
            int numcol_inicio = numcol;
            DataView odataview = ((monteGrid.oDataGridView.DataSource as BindingSource).DataSource as System.Data.DataTable).DefaultView;
            int linhasmax = linhas + odataview.Count;
            Microsoft.Office.Interop.Excel.Range Oget_Range;
            //titulo


            string linhastr = linhas.ToString();

            Type tipo = null;
            // cabeçalho Mestre
            // numcol = 1;
            foreach (Campo campo in monteGrid.LinhasCampo)
            {
                Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                          (linhas + 1), linhasmax, oworksheet);
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
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
                        if (tam > Oget_Range.ColumnWidth)
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
            if (numcol_inicio != numcol)
            {
                Oget_Range = Colunas_exc(Letras[numcol_inicio].ToString(), Letras[numcol - 1].ToString(),
                     linhas_inicio, linhas_inicio, oworksheet);
                Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            }
            return numcol;
        }

        private int CabecalhoBalancete(Microsoft.Office.Interop.Excel._Worksheet oworksheet, MonteGrid monteGrid, int linhas, int numcol)
        {
            int linhas_inicio = linhas;
            int numcol_inicio = numcol;
            DataView odataview = ((monteGrid.oDataGridView.DataSource as BindingSource).DataSource as System.Data.DataTable).DefaultView;
            int linhasmax = linhas + odataview.Count;
            Microsoft.Office.Interop.Excel.Range Oget_Range;
            //titulo


            string linhastr = linhas.ToString();

            Type tipo = null;
            // cabeçalho Mestre
            // numcol = 1;

            /*
             Type tipo = null;
                        foreach (Campo campo in LinhasCampo)
                        {
                            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                      (linhas + 1), linhasmax, oworksheet);
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
                            Oget_Range.ColumnWidth = campo.tamanho;
                            if (campo.titulo.ToUpper() == "DESCBAL")
                            {
                                Oget_Range.ColumnWidth = Oget_Range.ColumnWidth  - 18; 
                            }
                            else
                            {
                                Oget_Range.ColumnWidth = Oget_Range.ColumnWidth  - 3;
                            }
                            
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
                                        Oget_Range.ColumnWidth = (tam-5);
                                }
                            }
                            numcol += 1;
                        }
             */

            foreach (Campo campo in monteGrid.LinhasCampo)
            {
                Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                          (linhas + 1), linhasmax, oworksheet);
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
                Oget_Range.ColumnWidth = campo.tamanho;
                if (campo.titulo.ToUpper() == "DESCBAL")
                {
                    Oget_Range.ColumnWidth = Oget_Range.ColumnWidth - 15;
                }
                else
                {
                    Oget_Range.ColumnWidth = Oget_Range.ColumnWidth - 3;
                }


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
                        if (tam > Oget_Range.ColumnWidth)
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
            if (numcol_inicio != numcol)
            {
                Oget_Range = Colunas_exc(Letras[numcol_inicio].ToString(), Letras[numcol - 1].ToString(),
                     linhas_inicio, linhas_inicio, oworksheet);
                Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            }
            return numcol;
        }


        public void ExportaExcelBalancete()
        {

            if (oDataGridView == null) return;
            string odatamember = "";
            DataSet odataset = null;
            DataView odataview = null;
            if (oDataGridView == null) return;
            if (oDataGridView.DataSource is BindingSource)
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is DataSet)
                {
                    odataset = (DataSet)((BindingSource)oDataGridView.DataSource).DataSource;
                    odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
                }
                else
                {
                    if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                    {
                        odataview = (DataView)((BindingSource)oDataGridView.DataSource).DataSource;
                        odatamember = odataview.Table.TableName;
                    }
                    else if (((BindingSource)oDataGridView.DataSource).DataSource is System.Data.DataTable)
                    {
                        odataview = (((BindingSource)oDataGridView.DataSource).DataSource as System.Data.DataTable).DefaultView;
                        odatamember = odataview.Table.TableName;
                    }
                }
            }
            else
            {
                odataset = (DataSet)oDataGridView.DataSource;
                odatamember = oDataGridView.DataMember;
            }


            if (odatamember == "") return;
            if (odataview == null) return;

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

                    Microsoft.Office.Interop.Excel.Range Oget_Range;
                    try
                    {
                        int linhasinicio = 3;
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                        Microsoft.Office.Interop.Excel._Worksheet oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);

                        oworksheet.Activate();
                        oworksheet.Name = "Planilha1";


                        oworksheet.PageSetup.RightMargin = ExcelObj.CentimetersToPoints(0.3);
                        oworksheet.PageSetup.LeftMargin = ExcelObj.CentimetersToPoints(0.3); ;
                        oworksheet.PageSetup.TopMargin = ExcelObj.InchesToPoints(0.511811023622047);
                        oworksheet.PageSetup.BottomMargin = ExcelObj.InchesToPoints(0.511811023622047);
                        oworksheet.PageSetup.PaperSize = XlPaperSize.xlPaperA4;
                        oworksheet.PageSetup.LeftHeader = "&\"Calibri\"&B&9M.Libanio Agrícola S.A.";
                        
                        oworksheet.PageSetup.CenterHeader = "";

                        oworksheet.PageSetup.RightHeader = "&\"Calibri\"&7Folha:&P    ";
                        oworksheet.PageSetup.AlignMarginsHeaderFooter = true;
                        oworksheet.PageSetup.CenterHorizontally = true;
                        


                        double linhasporPagina = ExcelObj.InchesToPoints((double)oworksheet.PageSetup.PaperSize);

                        linhasporPagina = linhasporPagina + oworksheet.PageSetup.TopMargin +
                                oworksheet.PageSetup.BottomMargin + oworksheet.PageSetup.HeaderMargin;


                        // define linha max
                        int linhasmax = linhasinicio + odataview.Count;
                        if (ssTotal != null) linhasmax += 1;
                        if (ssTotalGeral != null) linhasmax += 1;


                        // numero de linhas

                        //.FontStyle = "Bold Italic"
                        string nomecol = "A:E";
                        int cortexto = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        //oworksheet.Rows.RowHeight = 42;
                        oworksheet.Range[nomecol].EntireColumn.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        oworksheet.Range[nomecol].EntireColumn.Font.Name = "Calibri";
                        oworksheet.Range[nomecol].EntireColumn.Font.Size = 9;
                        // oworksheet.Range[nomecol].EntireColumn.Font.Bold = true;
                      //  oworksheet.Range["A:A"].EntireColumn.WrapText = true;
                        oworksheet.Range[nomecol].EntireColumn.HorizontalAlignment =
                            Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

                        int colunaInicial = 0;
                        int colunaFinal = 6;

                        nomecol = Letras[colunaInicial] + ":" + Letras[colunaFinal];
                        oworksheet.Range[nomecol].EntireColumn.ColumnWidth = 8.56;
                        oworksheet.Range[nomecol].EntireColumn.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                        /*
                        /////////////////  VALORES QUADRO
                        const int linhasinicio = 2;
                        const int colunainicio = 0;


                        int linhas = linhasinicio;
                        colunaInicial = colunainicio; // zero-based

                        // largura coluna nome
                        nomecol = Letras[colunaInicial] + ":" + Letras[colunaInicial];
                        oworksheet.Range[nomecol].EntireColumn.ColumnWidth = 18.89;

                        */



                        //titulo
                        int linhas = 1;
                        int numcol = 0;
                        string linhastr = linhas.ToString();

                        if (tituloExcel != "")
                        {
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[4] + linhastr).MergeCells = true;
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[4] + linhastr).HorizontalAlignment =
                            Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[4] + linhastr).Value2 = tituloExcel;
                        }

                        numcol = 0;
                        linhas = linhasinicio;
                        linhastr = linhas.ToString();

                        // cabeçalho
                        CabecalhoBalancete(oworksheet, this, linhas, numcol);

                        Type tipo = null;
                        /* foreach (Campo campo in LinhasCampo)
                         {
                             Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                       (linhas + 1), linhasmax, oworksheet);
                             oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
                             Oget_Range.ColumnWidth = campo.tamanho;
                             if (campo.titulo.ToUpper() == "DESCBAL")
                             {
                                 Oget_Range.ColumnWidth = Oget_Range.ColumnWidth  - 18; 
                             }
                             else
                             {
                                 Oget_Range.ColumnWidth = Oget_Range.ColumnWidth  - 3;
                             }

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
                                         Oget_Range.ColumnWidth = (tam-5);
                                 }
                             }
                             numcol += 1;
                         }
                        */
                        double tamPagina =

                            (oworksheet.get_Range(Letras[numcol] + (1).ToString(), Letras[numcol] + linhastr).EntireRow.Height

                            );
                        double linhaPadrao = oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).EntireRow.Height;

                        linhas += 1;
                        string grupo = "1";
                        bool mudaPagina = false;
                       // int linhaInc = 0;
                        int pag = 1;
                        int linhaInicioGrupo = linhasinicio;

                        foreach (DataRowView orow in odataview)
                        {
                            numcol = 0;
                            linhastr = linhas.ToString();
                            if (orow["DESCBAL"].ToString().Substring(0, 1) != grupo)
                            {
                                grupo = orow["DESCBAL"].ToString().Substring(0, 1);
                                mudaPagina = true;
                            }
                            else
                                mudaPagina = false;
                            double tamLin = oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).EntireRow.Height;
                            if (tamLin != linhaPadrao)
                            {
                                tamPagina = tamPagina + tamLin;
                            }
                            else tamPagina = tamPagina + tamLin;
                            if (tamPagina >= linhasporPagina)
                            {
                                // +oworksheet.PageSetup.TopMargin +
                                //oworksheet.PageSetup.BottomMargin
                                // MessageBox.Show("Muda Pag" +pag.ToString() + " linha"+linhastr  );
                                pag++;
                                tamPagina = 0;// oworksheet.PageSetup.TopMargin +
                                //oworksheet.PageSetup.BottomMargin;
                                mudaPagina = false;
                                /*numcol = 0;
                                Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + (LinhasCampo.Count - 1)].ToString(),
                                     linhaInicioGrupo, (linhas - 1), oworksheet);
                                Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                linhaInicioGrupo = linhas;
                                numcol = 0;
                                linhas++;
                                linhaInicioGrupo = linhas;
                                CabecalhoBalancete(oworksheet, this, linhas, numcol);
                                tamPagina = oworksheet.get_Range(Letras[numcol] + linhas.ToString(),
                                    Letras[numcol] + linhas.ToString()).EntireRow.Height;
                                linhas = linhas + 1;
                                mudaPagina = false;
                                linhastr = linhas.ToString();
                                tamPagina = tamPagina + oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).EntireRow.Height;
                                */

                            }
                            if (mudaPagina)
                            {
                                double quantasLinhas = Math.Floor(tamPagina /
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).EntireRow.Height);
                                double totallnPaginha = Math.Ceiling(linhasporPagina /
                                                           oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).EntireRow.Height);
                                double faltalinha = totallnPaginha - quantasLinhas;
                                numcol = 0;
                                Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + (LinhasCampo.Count - 1)].ToString(),
                                     linhaInicioGrupo, (linhas-1), oworksheet);
                                Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                                //double faltalinha = (linhasporPagina - tamPagina)/ oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).EntireRow.Height;
                                linhas = linhas + Convert.ToInt16(faltalinha);
                                if (linhas % Convert.ToInt16(totallnPaginha) == 0)
                                {
                                    linhas++;
                                }
                                linhaInicioGrupo = linhas;
                                numcol = 0;
                                CabecalhoBalancete(oworksheet, this, linhas, numcol);
                                tamPagina = oworksheet.get_Range(Letras[numcol] + linhas.ToString(),
                                    Letras[numcol] + linhas.ToString()).EntireRow.Height;
                                linhas = linhas + 1;
                                mudaPagina = false;
                                linhastr = linhas.ToString();
                                tamPagina = tamPagina + oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).EntireRow.Height;

                            }

                            foreach (Campo campo in LinhasCampo)
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
                                        else { dado = Convert.ToDateTime(orow[campo.titulo]).ToString("d"); }

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
                                numcol += 1;
                            }
                            linhas += 1;
                           // linhaInc += 1;

                        /*    if (ssTotal != null)
                            {
                                numcol = 0;
                                linhastr = linhas.ToString();
                                foreach (Campo campo in LinhasCampo)
                                {
                                    if (numcol == 1) oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOTAIS";
                                    if (!campo.soma) { numcol += 1; continue; }
                                    object dado = campo.total;
                                    if (campo.format != "")
                                    {
                                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                                    }

                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                                    numcol += 1;
                                }
                                linhas += 1;
                            }
                            if (ssTotalGeral != null)
                            {
                                numcol = 0;
                                linhastr = linhas.ToString();
                                foreach (Campo campo in LinhasCampo)
                                {
                                    if (numcol == 1) oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOT.GERAL";
                                    if (!campo.soma) { numcol += 1; continue; }
                                    object dado = campo.totalgeral;
                                    if (campo.format != "")
                                    {
                                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                                    }

                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = HorizontalAlignment.Right;
                                    numcol += 1;

                                }
                                linhas += 1;
                            }*/
                        }
                        //linhas = linhasinicio;
                        numcol = 0;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + (LinhasCampo.Count - 1)].ToString(),
                             linhaInicioGrupo, (linhas-1), oworksheet);
                        Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                        oWorkbook.SaveAs(SaveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                      "", "", false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
                       Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true, "", "", false);

                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Erro ao tentar abrir tabela Excel " + E.Message, "",
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


        }



        /*
        public bool Mudanca_Pagina(Microsoft.Office.Interop.Excel._Worksheet oworksheet)
{
bool result = false;
if (LocalLinha >= incremento)
{
incremento = fatorincremento + 1; // 38;
Formate(oworksheet);
LocalLinha = 0;
InicioPagina = linhas + 1;

Cabecalho(oworksheet);
result = true;

}
return result;

}

public void Formate(Microsoft.Office.Interop.Excel._Worksheet oworksheet
        )
{
string linhastr = InicioPagina.ToString();
string linhastr2 = (InicioPagina + LocalLinha).ToString();

for (int i = 1; i < totalColunas + 1; i++)
{
Microsoft.Office.Interop.Excel.Range rangeLinha = oworksheet.get_Range(Letras[i] + linhastr, Letras[i] + linhastr2.ToString());
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).LineStyle =
         Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).Weight =
        Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).ColorIndex = XlColorIndex.xlColorIndexAutomatic;

rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle =
        Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).Weight =
        Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).ColorIndex = XlColorIndex.xlColorIndexAutomatic;

rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle =
                 Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).Weight =
        Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).ColorIndex = XlColorIndex.xlColorIndexAutomatic;

rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).LineStyle =
             Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).Weight =
        Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
rangeLinha.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).ColorIndex = XlColorIndex.xlColorIndexAutomatic;

}

// return colunaLinha;

        public void Cabecalho(Microsoft.Office.Interop.Excel._Worksheet oworksheet
)
{
        Microsoft.Office.Interop.Excel.Range range;
        int inicio = ColInicio;
        int fim = ColFim;
        string linhastr;

        int NumCol;
        if (linhas > 10)
        {
            NumPagina = +1;
            linhastr = linhas.ToString();
            range = oworksheet.get_Range(Letras[ColInicio] + linhastr, Letras[ColFim] + linhastr.ToString());
            range.Font.Name = "Arial";
            range.Font.Size = 6;
            range.Font.Italic = true;

            range = oworksheet.get_Range(Letras[inicio] + linhastr, Letras[fim - 1] + linhastr.ToString());
            range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).LineStyle =
                      Microsoft.Office.Interop.Excel.XlLineStyle.xlDot;
            range.Interior.ColorIndex = XlColorIndex.xlColorIndexAutomatic;
            NumCol = 1;
            range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
            range.Value2 = SubTitulo;
            NumCol = ColFim;
            range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
            range.Value2 = "pagina:" + NumPagina.ToString();
        }
        linhas++;
        linhastr = linhas.ToString();
        if ((linhas % 2) == 0)
        {
            range = oworksheet.get_Range(Letras[inicio] + linhastr, Letras[fim] + linhastr.ToString());
            range.Interior.ColorIndex = 15;
            range.Interior.Pattern = XlPattern.xlPatternSolid;
        }
        if (Setor != "")
        {
            NumCol = 1;
            range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
            range.Font.Name = "Arial";
            range.Font.Size = 8;
            range.Font.Italic = true;
            range.Value2 = Setor.Trim() + "     (Valores em R$)";
        }
        NumCol = 2;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
        range.Value2 = " ENTRADAS  ";
        NumCol = 4;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
        range.Value2 = "  FOLHAS   ";

        NumCol = 6;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
        range.Value2 = "   INSUMOS ";

        NumCol = 7;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
        range.Value2 = "DIVERSOS";

        NumCol = 8;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
        range.Value2 = "MAQ.EQUIP. ";

        NumCol = 9;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
        range.Value2 = "    RATEIOS";

        NumCol = 10;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[NumCol] + linhastr.ToString());
        range.Value2 = "      SALDO";

        NumCol = 2;
        range = oworksheet.get_Range(Letras[NumCol] + linhastr, Letras[fim] + linhastr.ToString());
        range.Font.Name = "Arial";
        range.Font.Size = 7;
        range.Font.Italic = true;
        range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle =
                      Microsoft.Office.Interop.Excel.XlLineStyle.xlDash;
        range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).Weight =
                      Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).ColorIndex = XlColorIndex.xlColorIndexAutomatic;
        linhas++;
        PinteLinha(oworksheet);

        //   return colunaLinha;
    }


}




        */



    }

    /*private Microsoft.Office.Interop.Excel.Range Colunas_exc(string colunas, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
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


    public void ExportaExcel()
    {

        if (oDataGridView == null) return;
        string odatamember = "";
        DataSet odataset = null;
        DataView odataview = null;
        if (oDataGridView == null) return;
        if (oDataGridView.DataSource is BindingSource)
        {
            if (((BindingSource)oDataGridView.DataSource).DataSource is DataSet)
            {
                odataset = (DataSet)((BindingSource)oDataGridView.DataSource).DataSource;
                odatamember = ((BindingSource)oDataGridView.DataSource).DataMember;
            }
            else
            {
                if (((BindingSource)oDataGridView.DataSource).DataSource is DataView)
                {
                    odataview = (DataView)((BindingSource)oDataGridView.DataSource).DataSource;
                    odatamember = odataview.Table.TableName;
                }
            }
        }
        else
        {
            odataset = (DataSet)oDataGridView.DataSource;
            odatamember = oDataGridView.DataMember;
        }


        if (odatamember == "") return;
        if (odataview == null) return;

        Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                              'L','M','N','O','P','Q','R','S','T','U'};
      //  Int16[] Colunas = new Int16[21] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 ,21};
        System.Windows.Forms.DialogResult result;// = System.Windows.Forms.DialogResult.OK;
          SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
          SaveFileDialog1.DefaultExt = "xls";
          SaveFileDialog1.AddExtension = true;

          SaveFileDialog1.CheckFileExists = false;
        result = SaveFileDialog1.ShowDialog();

       if (result == System.Windows.Forms.DialogResult.OK)
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
                ExcelObj.Visible = true;

                Microsoft.Office.Interop.Excel.Workbook oWorkbook =
                    ExcelObj.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                Microsoft.Office.Interop.Excel.Range Oget_Range;
                try
                {
                    int linhasinicio = 4;
                    Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                    Microsoft.Office.Interop.Excel._Worksheet oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);

                    oworksheet.Activate();
                    oworksheet.Name = "Planilha1";







                    // define linha max
                    int linhasmax = linhasinicio + odataview.Count;
                    if (sbTotal != null) linhasmax += 1;
                    if (sbTotalGeral != null) linhasmax += 1;

                   //titulo
                    int linhas = 1;
                    int numcol = 0;
                    string linhastr = linhas.ToString();

                    if (TituloExcel != "")
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = TituloExcel;


                    numcol = 1;
                    linhas = linhasinicio;
                    linhastr = linhas.ToString();

                    // cabeçalho
                    for (int i = 0; i < LinhasCampo.Count; i++)
                    {
                      Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(), 
                           (linhas + 1),linhasmax, oworksheet);
                        Oget_Range.ColumnWidth = (oDataGridView.Columns[i].Width / oDataGridView.Font.Size);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).ColumnWidth = LinhasCampo[i].tamanho;//(oDataGridView.Columns[i].Width / oDataGridView.Font.Size);// oDataGridView.Columns[i].Width;
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = LinhasCampo[i].cabecalho;
                        if (LinhasCampo[i].format != "")
                            Oget_Range.NumberFormat = Reformate(LinhasCampo[i].format);
                        numcol += 1;
                    }

                    linhas += 1;
                    for (int i = 0; i < odataview.Count; i++)
                   {
                        DataRowView orow =  odataview[i];

                        numcol = 1;
                        linhastr = linhas.ToString();
                        for (int j = 0; j <  LinhasCampo.Count; j++)
                        {
                            string campo = "";
                            try
                            {
                                if (orow[LinhasCampo[j].titulo].GetType() == Type.GetType("System.DateTime"))
                                    campo = ((DateTime)orow[LinhasCampo[j].titulo]).ToString("d");
                                else
                                    campo = Convert.ToString(orow[LinhasCampo[j].titulo]);
                            }
                            catch
                            { }

                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo;   //gleba;//odatarow["SETOR"].ToString;
                            numcol += 1;
                        }
                        linhas += 1;
                    }
                    if ((sbTotal != null) || (ssTotal != null))
                    {
                        numcol = 1;
                        linhastr = linhas.ToString();
                        for (int i = 0; i < LinhasCampo.Count; i++)
                        {
                           if (LinhasCampo[i].soma)
                           {  
                                object campo = "";
                                try
                                {
                                    campo = (object)sbTotal.Panels[i].Text;
                                }
                                catch
                                { }
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo;   
                            }
                           else
                           if (i == 0)  oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOTAIS";

                           numcol += 1;

                        }
                        linhas += 1;                
                    }
                    if ((sbTotalGeral != null) || (ssTotalGeral != null))
                    {
                        numcol = 1;
                        linhastr = linhas.ToString();
                        for (int i = 0; i < LinhasCampo.Count; i++)
                        {
                            if (LinhasCampo[i].soma)
                            {

                                object campo = "";
                                try
                                {
                                    campo = (object)sbTotalGeral.Panels[i].Text;// LinhasCampo[i].total;//ou sbTotal.Panels[i].Text
                                }
                                catch
                                { }
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo;   //gleba;//odatarow["SETOR"].ToString;
                            }
                            else
                                if (i == 0) oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "TOTAIS GERAIS";

                            numcol += 1;

                        }
                        linhas += 1;
                    }
                    if ((sbTotalExtra != null) || (ssTotalExtra != null))
                    {
                        numcol = 1;
                        linhastr = linhas.ToString();
                        foreach (StatusBarPanel panel in sbTotalExtra.Panels)
                        {
                            object campo = "";
                            try
                            {
                                campo = (object)panel.Text; 
                            }
                            catch
                            { }
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo;
                            numcol += 1;
                        }

                    }

                    linhas = linhasinicio;
                    numcol = 1;
                    Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + (LinhasCampo.Count-1)].ToString(),
                         linhas , linhasmax, oworksheet);
                    Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                   oWorkbook.SaveAs(SaveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, 
                 "", "", false,false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
                  Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution,true,"","",false);

                }
                catch (Exception)
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
                    ExcelObj.Quit();

                Cursor.Current = Cursors.Default;

            }

        }
    }

}

    */

    //Construir a classe de evento a ser disparado
    public class TotalizaEventArgs : EventArgs
    {
        public TotalizaEventArgs(System.Data.DataTable tablepesq)
        {
            tabpesq = tablepesq;
        }
        private System.Data.DataTable tabpesq;
       // private DataRowState tipomuda;

       // public DataRowState TipoMuda
        //{
          //  get { return tipomuda; }
          //  set { tipomuda = value; }

        //}
        public System.Data.DataTable Tabpesq
        {
            get { return tabpesq; }
            set { tabpesq = value; }
        }
    }

}