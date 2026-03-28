using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using System.Data.OleDb;
//using System.Data.Odbc;
using ClassConexao;
using ClassFiltroEdite;

namespace ClassEstoque
{
    public partial class FrmRelatorioAlmox : Form
    {
        DataTable reldep;
        DataTable detalhe_movest;
        DataRelation mestredetalheRelacao;
        DataSet DataSet1;
        ClassEstoque.AtualizaEstoque oclasse_estoque;

        public BindingSource bmSourceMestre;
        DataTable armazem;

     //   System.Windows.Forms.CurrencyManager FCSetor;

        public FrmRelatorioAlmox()
        {
            InitializeComponent();
            oclasse_estoque = new AtualizaEstoque();
            reldep = null;
            DataSet1 = new DataSet();
            bmSourceMestre = new BindingSource();

            this.dgvMestre.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalhe.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            armazem = Classarmazem.armazemselect();

            cbArmazem.Items.Clear();
            cbArmazem.Items.Add("          ");
            foreach (DataRow linha in armazem.AsEnumerable())
            {
                cbArmazem.Items.Add(linha["COD"] + " " + linha["NOME_DEP"]);
            }
            cbArmazem.Sorted = true;


        }


        private void button1_Click(object sender, EventArgs e)
        {
            Cursor ocursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            //  sbdetalhes.AutoScrollOffset(
            reldep = oclasse_estoque.RelatorioDeposito(dtinicio.Value, dtfim.Value);
            // juntar os depositos em um só geral..
            var tabelajunta = from linha in reldep.AsEnumerable()
                              group linha by new
                              {
                                  cod = linha.Field<string>("COD"),
                                  descr = linha.Field<string>("DESCR"),
                                  unid = linha.Field<string>("UNID")
                              } into g
                              orderby g.Key.cod
                              select new
                              {
                                  cod = g.Key.cod,
                                  descricao = g.Key.descr,
                                  unid = g.Key.unid,
                                  entrada = g.Sum((linha => linha.Field<Decimal?>("ENTRADA"))),
                                  saida = g.Sum((linha => linha.Field<Decimal?>("SAIDA"))),
                                  sdoatual = g.Sum((linha => linha.Field<Decimal?>("SDOATUAL"))),
                                  sdoant = g.Sum((linha => linha.Field<Decimal?>("SDOANT")))
                              };

            DataTable mestre_movest = reldep.Clone();
            mestre_movest.TableName = "Mestre";
            mestre_movest.Columns.Remove(mestre_movest.Columns["DEP"]);

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = mestre_movest.Columns["COD"];
            mestre_movest.PrimaryKey = PrimaryKeyColumns;



            foreach (var campo in tabelajunta.AsEnumerable())
            {
                DataRow novo = mestre_movest.NewRow();
                novo["COD"] = campo.cod;
                novo["DESCR"] = campo.descricao;
                novo["UNID"] = campo.unid;
                novo["SDOANT"] = campo.sdoant;
                novo["SDOATUAL"] = campo.sdoatual;
                novo["ENTRADA"] = campo.entrada;
                novo["SAIDA"] = campo.saida;
                mestre_movest.Rows.Add(novo);
            }





            //  DataTable detalhe_movest = dmovest.
            detalhe_movest = oclasse_estoque.Copia_Movest();// oview.ToTable();

            DataView oview = detalhe_movest.DefaultView;
            oview.RowFilter = " (trim(COD) <> '') ";
            var lista = from linha in detalhe_movest.AsEnumerable()
                        group linha by new
                        {
                            cod = linha.Field<string>("COD")
                        } into g
                        orderby g.Key.cod
                        select new
                        {
                            cod = g.Key.cod
                        };


            foreach (var campo in lista)
            {
                DataRow orow = mestre_movest.Rows.Find(campo.cod);
                if (orow == null)
                {
                    oview.RowFilter = oview.RowFilter + " AND (COD <> '" + campo.cod + "')";
                }

            }
            oview.Sort = "COD,DATA,TIPO";
            detalhe_movest = oview.ToTable();


            // espelho do movest
            detalhe_movest.TableName = "Detalhe";

            DataSet1 = new DataSet();
            DataSet1.Tables.Add(mestre_movest);
            DataSet1.Tables.Add(detalhe_movest);

            mestredetalheRelacao = new DataRelation("MestreDetalhe",
               mestre_movest.Columns["COD"],
               detalhe_movest.Columns["COD"]);


            DataSet1.Relations.Add(mestredetalheRelacao);




            bmSourceMestre.DataSource = DataSet1;
            bmSourceMestre.DataMember = "Mestre";
            dgvMestre.DataSource = bmSourceMestre;

            MonteGrid omestre = new MonteGrid();
            omestre.oDataGridView = dgvMestre;
            
            omestre.AddValores("COD", "Cod", mestre_movest.Columns[0].MaxLength, "", false, 0, "");
            omestre.AddValores("DESCR", "Descrição", mestre_movest.Columns[1].MaxLength, "", false, 0, "");
            omestre.AddValores("UNID", "Unid", mestre_movest.Columns[2].MaxLength, "", false, 0, "");
            omestre.AddValores("SDOANT", "Sdo Ant.", 0, "#,###,##0.00", false, 0, "");
            omestre.AddValores("ENTRADA", "Entradas", 0, "#,###,##0.00", false, 0, "");
            omestre.AddValores("SAIDA", "Saidas", 0, "#,###,##0.00", false, 0, ""); 
            omestre.AddValores("SDOATUAL", "Sdo Atual", 0, "#,###,##0.00",false, 0, "");
            omestre.NovoConfigureDBGridView();

            Cursor.Current = ocursor;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (reldep == null)
            {
                reldep = oclasse_estoque.RelatorioDeposito(dtinicio.Value, dtfim.Value);
            }
            string tipoord = "D";
            if (rbcodigo.Checked)
                tipoord = "A";
            oclasse_estoque.RelatorioExcel_Deposito(reldep, dtinicio.Value, dtfim.Value, tipoord);

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if ((sender as TabControl).SelectedIndex == 1)
            {
               // dgvMestre.CurrentRow.C
                DataRowView drv = (bmSourceMestre.Current as DataRowView);
                // verifica os depósitos validos...
                DataRow[] rowsfilha = drv.Row.GetChildRows(mestredetalheRelacao);

                var lista = from linha in rowsfilha.AsEnumerable()
                            join linhaarm in armazem.AsEnumerable()
                            on linha.Field<string>("DEPOSITO") equals
                                         linhaarm.Field<string>("COD")

                            group linha by new
                            {
                                dep = linha.Field<string>("DEPOSITO")
                            } into g
                            orderby g.Key.dep
                            select new
                            {
                                cod = g.Key.dep
                            };


                DataTable detalhe_browse = new DataTable("Detalhe_Browse");
                detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "Data", 0, false));
                detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "Tipo", 1, false));
                detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "Quant", 0, false));
                detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "Valor", 0, false));
                detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "Saldo", 0, false));
                detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "Saldo_R$", 0, false));
                detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "Cme_R$", 0, false));
                //acrescenta campos para mov depositos...
                foreach (var campo in lista.AsEnumerable())
                {
                    detalhe_browse.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "DEP_" + campo.cod, 0, false));
                }



                // prepare uma tabela detalhe_browse pronta para o Browse...
                Decimal tvalor, tquant, tcust_me;
                tvalor = 0M;
                tquant = 0M;
                tcust_me = 0M;
                DateTime tdata_cus;
                foreach (DataRow oRow in rowsfilha.AsEnumerable())
                {
                    DataRow rowbrowse = detalhe_browse.NewRow();
                    rowbrowse["DATA"] = oRow["DATA"];
                    rowbrowse["TIPO"] = oRow["TIPO"];
                    // decimal tvalor = 0;
                    if (oRow["TIPO2"].ToString() != "T")
                    {
                        if (oRow["TIPO"].ToString() == "E")
                        {

                            if (Convert.ToDecimal(oRow["VALOR"]) != 0)

                                tvalor = tvalor + Convert.ToDecimal(oRow["VALOR"]);
                            else
                                tvalor = tvalor + Decimal.Round(Convert.ToDecimal(oRow["QUANT"]) * tcust_me, 2);
                            tquant = tquant + Convert.ToDecimal(oRow["QUANT"]);
                            if ((tvalor != 0) && (tquant != 0))
                            {
                                tcust_me = Math.Round(tvalor / tquant, 4);
                                tdata_cus = Convert.ToDateTime(oRow["DATA"]);

                            }
                        }
                        else if (oRow["TIPO"].ToString() == "S")
                        {
                            tvalor = tvalor - Decimal.Round(Convert.ToDecimal(oRow["QUANT"]) * tcust_me, 2);
                            tquant = tquant - Convert.ToDecimal(oRow["QUANT"]);
                        }
                        rowbrowse["QUANT"] = Convert.ToDecimal(oRow["QUANT"]);
                        if (oRow["TIPO"].ToString() == "E")
                        {
                            if (Convert.ToDecimal(oRow["VALOR"]) != 0)

                                rowbrowse["VALOR"] = Convert.ToDecimal(oRow["VALOR"]);
                            else
                                rowbrowse["VALOR"] = Convert.ToDecimal(oRow["VALOR"]);       //Decimal.Round(Convert.ToDecimal(oRow["QUANT"]) * tcust_me, 2);
                        }
                        else
                            rowbrowse["VALOR"] = Convert.ToDecimal(oRow["VALOR"]);//Decimal.Round(Convert.ToDecimal(oRow["QUANT"]) * tcust_me, 2);
                        rowbrowse["Saldo"] = tquant;
                        rowbrowse["Saldo_R$"] = tvalor;
                        rowbrowse["Cme_R$"] = tcust_me;
                    }
                    else
                    {
                        rowbrowse["DATA"] = oRow["DATA"];
                        rowbrowse["TIPO"] = oRow["TIPO2"];
                        rowbrowse["QUANT"] = oRow["QUANT"];
                        rowbrowse["VALOR"] = Convert.ToDecimal(oRow["VALOR"]);
                        if (oRow["TIPO"].ToString() == "S")
                           rowbrowse["DEP_" + oRow["DEPOSITO"].ToString()] = Convert.ToDecimal(oRow["QUANT"]) * -1;
                        else
                            rowbrowse["DEP_" + oRow["DEPOSITO"].ToString()] = Convert.ToDecimal(oRow["QUANT"]);
                    }

                    detalhe_browse.Rows.Add(rowbrowse);
                }
                dgvDetalhe.DataSource = detalhe_browse;
            
               
                MonteGrid odetalhe = new MonteGrid();
                odetalhe.oDataGridView = dgvDetalhe;
                odetalhe.AddValores("DATA", "Data", 0, "", false, 0, "");
                odetalhe.AddValores("TIPO", "Tipo", 1, "", false, 0, "");
                odetalhe.AddValores("Quant", "Quant", 0,"###,##0.000",false, 0, "");
                odetalhe.AddValores("VALOR", "Valor", 0, "#,###,##0.00", false, 0, "");
                odetalhe.AddValores("SALDO", "Saldo(R$)", 0, "#,###,##0.000", false, 0, "");
                odetalhe.AddValores("Saldo_R$", "Saldo(R$)", 0, "#,###,##0.00", false, 0, "");
                odetalhe.AddValores("CME_R$", "C.Medio(R$)", 0, "#,###,##0.00", false, 0, "");
                for (int i = 7; i < detalhe_browse.Columns.Count; i++)
                {
                    DataColumn ocol = detalhe_browse.Columns[i];
                    odetalhe.AddValores(ocol.ColumnName, ocol.ColumnName, 0, "###,##0.000", true, 0, "");
                
                }
                odetalhe.sbTotal = sbdetalhes; 
                
                odetalhe.NovoConfigureDBGridView();
           
                //ArrayList meusformat = new ArrayList();
                
                //Prepare_DbGrid(dgvDetalhe, detalhe_browse.DefaultView,meusforma);

                odetalhe.EncontraTotaisView();
                odetalhe.NovoColocaTotais();
                
                
               // dgvDetalhe.Refresh();
                

            }






        }
        private void Prepare_DbGrid(DataGridView oDataGridView, DataView tabela, ArrayList formats)
        {

           // oDataGridView.Columns.Clear();

            oDataGridView.AutoGenerateColumns = true;
            //DataGridViewTextBoxColumn oTextCol;


            // Set the column header style.
           // DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

           // columnHeaderStyle.BackColor = Color.Beige;
           // columnHeaderStyle.Font = new Font("Microsoft Sans Serif", 7, FontStyle.Italic);
           // columnHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //            columnHeaderStyle.ApplyStyle();
           // oDataGridView.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
           // oDataGridView.Columns[0].DefaultHeaderCellType 
          //  oDataGridView.ColumnHeadersDefaultCellStyle.
            for (int i = 0; i < tabela.Table.Columns.Count; i++)
            {
                if (oDataGridView.Columns.Contains(tabela.Table.Columns[i].ColumnName))
                {
                    DataGridViewColumn ocoluna = oDataGridView.Columns[tabela.Table.Columns[i].ColumnName];
                    DataGridViewCellStyle cellStyle;
                   // DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
                    if (ocoluna.HasDefaultCellStyle)
                        cellStyle = ocoluna.DefaultCellStyle;
                    else
                    {
                        cellStyle = new DataGridViewCellStyle();
                        cellStyle.Font = oDataGridView.Font; 
                    }
                    if (tabela.Table.Columns[i].DataType == Type.GetType("System.String"))
                    {
                        Int32 maxlen = tabela.Table.Columns[i].MaxLength;
                        if (maxlen > 40)
                            maxlen = Convert.ToInt32(40 * 0.8);
                        else
                            if (maxlen < 5)
                                maxlen = maxlen + 1;

                        ocoluna.Width = Convert.ToInt32(Math.Abs(cellStyle.Font.SizeInPoints * maxlen));
                    }
                    else
                        if ((tabela.Table.Columns[i].DataType == Type.GetType("System.Decimal")) ||(tabela.Table.Columns[i].DataType == Type.GetType("System.Single"))
                            || (tabela.Table.Columns[i].DataType == Type.GetType("System.Int32")) || (tabela.Table.Columns[i].DataType == Type.GetType("System.Int16")))
                        {
                            cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            ocoluna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            cellStyle.Format = "#,###,##0.00";
                        }
                    ocoluna.HeaderCell.Value = tabela.Table.Columns[i].ColumnName.Substring(0, 1).ToUpper() + tabela.Table.Columns[i].ColumnName.Substring(1).ToLower();
                    

                    // ocoluna.HeaderCell.
                    ocoluna.DefaultCellStyle = cellStyle;
                    //ocoluna.DefaultHeaderCellType. =  columnHeaderStyle;
                    /*ocoluna 
                    //oTextCol = new DataGridViewTextBoxColumn();
                    oTextCol.DataPropertyName = LinhasCampo[i].titulo;
                    DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

                    oTextCol.HeaderText = LinhasCampo[i].cabecalho;
                    cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    if (LinhasCampo[i].tamanho > 0)
                        oTextCol.Width = LinhasCampo[i].tamanho * Convert.ToInt16(oDataGridView.Font.Size);
                    if (LinhasCampo[i].format != "")
                    {
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        cellStyle.Format = LinhasCampo[i].format;
                    }

                    cellStyle.NullValue = LinhasCampo[i].nulltext;

                    oTextCol.DefaultCellStyle = cellStyle;
                    int numcol = oDataGridView.Columns.Add(oTextCol);
                    oDataGridView.Columns[numcol].Name = LinhasCampo[i].titulo;*/
                }

            }
           // oDataGridView.CellFormatting += new DataGridViewCellFormattingEventHandler(oDataGridView_CellFormatting);

        }



    }
}
/*
 How to: Ensure the Selected Row in a Child Table Remains at the Correct Position 
Example  See Also  Send Feedback 
 

Oftentimes when you work with data binding in Windows Forms, you will display data in what is called a parent/child or master/details view. This refers to a data-binding scenario where data from the same source is displayed in two controls. Changing the selection in one control causes the data displayed in the second control to change. For example, the first control might contain a list of customers and the second a list of orders related to the selected customer in the first control. 

Starting with the .NET Framework version 2.0, when you display data in a parent/child view you might have to take extra steps to make sure that the currently selected row in the child table is not reset to the first row of the table. In order to do this, you will have to cache the child table position and reset it after the parent table changes. Typically the child reset occurs the first time a field in a row of the parent table changes.

To Cache the Current Child Position
Declare an integer variable to store the child list position and a Boolean variable to store whether to cache the child position.

Visual Basic  Copy Code 
Private cachedPosition As Integer = - 1
Private cacheChildPosition As Boolean = True



 
C#  Copy Code 
private int cachedPosition = -1;
private bool cacheChildPosition = true;


 

 
C#  Copy Code 
void relatedCM_ListChanged(object sender, ListChangedEventArgs e)
{
    // Check to see if this is a caching situation.
    if (cacheChildPosition && cachePositionCheckBox.Checked) 
    {
        // If so, check to see if it is a reset situation, and the current
        // position is greater than zero.
        CurrencyManager relatedCM = sender as CurrencyManager;
        if (e.ListChangedType == ListChangedType.Reset && relatedCM.Position > 0)

            // If so, cache the position of the child table.
            cachedPosition = relatedCM.Position;
    }
}


 

Handle the parent list's CurrentChanged event for the parent currency manager. In the handler, set the Boolean value to indicate it is not a caching scenario. If the CurrentChanged occurs, the change to the parent is a list position change and not an item value change.

Visual Basic  Copy Code 
' Handle the current changed event. This event occurs when
' the current item is changed, but not when a field of the current
' item is changed.
Private Sub bindingSource1_CurrentChanged(ByVal sender As Object, _
    ByVal e As EventArgs) Handles bindingSource1.CurrentChanged
    ' If the CurrentChanged event occurs, this is not a caching 
    ' situation.
    cacheChildPosition = False

End Sub


 
C#  Copy Code 
void bindingSource1_CurrentChanged(object sender, EventArgs e)
{
    // If the CurrentChanged event occurs, this is not a caching 
    // situation.
    cacheChildPosition = false;
}


 

To Reset the Child Position
Handle the PositionChanged event for the child binding's CurrencyManager.

Reset the child table position to the cached position saved in the previous procedure.

Visual Basic  Copy Code 
Private Sub relatedCM_PositionChanged(ByVal sender As Object, ByVal e As EventArgs) 
    ' Check to see if this is a caching situation.
    If cacheChildPosition AndAlso cachePositionCheckBox.Checked Then
        Dim relatedCM As CurrencyManager = sender

        ' If so, check to see if the current position is 
        ' not equal to the cached position and the cached 
        ' position is not out of bounds.
        If relatedCM.Position <> cachedPosition AndAlso _
            cachedPosition > 0 AndAlso cachedPosition < _
            relatedCM.Count Then
            relatedCM.Position = cachedPosition
            cachedPosition = -1
        End If
    End If
End Sub


 
C#  Copy Code 
void relatedCM_PositionChanged(object sender, EventArgs e)
{
    // Check to see if this is a caching situation.
    if (cacheChildPosition && cachePositionCheckBox.Checked)
    {
        CurrencyManager relatedCM = sender as CurrencyManager;

        // If so, check to see if the current position is 
        // not equal to the cached position and the cached 
        // position is not out of bounds.
        if (relatedCM.Position != cachedPosition && cachedPosition
            > 0 && cachedPosition < relatedCM.Count)
        {
            relatedCM.Position = cachedPosition;
            cachedPosition = -1;
        }
    }
}


 

Example
The following example demonstrates how to save the current position on the CurrencyManager.for a child table and reset the position after an edit is completed on the parent table. This example contains two DataGridView controls bound to two tables in a DataSet using a BindingSource component. A relation is established between the two tables and the relation is added to the DataSet. The position in the child table is initially set to the third row for demonstration purposes.

 
C#  Copy Code 
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BT2
{
    public class Form1 : Form
    {
        public Form1()
        {
            InitializeControlsAndDataSource();
        }

        // Declare the controls to be used.
        private BindingSource bindingSource1;
        private DataGridView dataGridView1;
        private Button button1;
        private DataGridView dataGridView2;
        private CheckBox cachePositionCheckBox;
        public DataSet set1;

        private void InitializeControlsAndDataSource()
        {
            // Initialize the controls and set location, size and 
            // other basic properties.
            this.dataGridView1 = new DataGridView();
            this.bindingSource1 = new BindingSource();
            this.button1 = new Button();
            this.dataGridView2 = new DataGridView();
            this.cachePositionCheckBox = new System.Windows.Forms.CheckBox();
            this.dataGridView1.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = DockStyle.Top;
            this.dataGridView1.Location = new Point(0, 20);
            this.dataGridView1.Size = new Size(292, 170);
            this.button1.Location = new System.Drawing.Point(18, 175);
            this.button1.Size = new System.Drawing.Size(125, 23);

            button1.Text = "Clear Parent Field";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.dataGridView2.ColumnHeadersHeightSizeMode = 
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(0, 225);
            this.dataGridView2.Size = new System.Drawing.Size(309, 130);
            this.cachePositionCheckBox.AutoSize = true;
            this.cachePositionCheckBox.Checked = true;
            this.cachePositionCheckBox.Location = new System.Drawing.Point(150, 175);
            this.cachePositionCheckBox.Name = "radioButton1";
            this.cachePositionCheckBox.Size = new System.Drawing.Size(151, 17);
            this.cachePositionCheckBox.Text = "Cache and restore position";
            this.ClientSize = new System.Drawing.Size(325, 420);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cachePositionCheckBox);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.button1);

            // Initialize the data.
            set1 = InitializeDataSet();

            // Set the data source to the DataSet.
            bindingSource1.DataSource = set1;

            //Set the DataMember to the Menu table.
            bindingSource1.DataMember = "Customers";

            // Add the control data bindings.
            dataGridView1.DataSource = bindingSource1;

            // Set the data source and member for the second DataGridView.
            dataGridView2.DataSource = bindingSource1;
            dataGridView2.DataMember = "custOrders";

            // Get the currency manager for the customer orders binding.
            CurrencyManager relatedCM = 
                bindingSource1.GetRelatedCurrencyManager("custOrders");

            // Set the position in the child table for demonstration purposes.
            relatedCM.Position = 3;

            // Handle the current changed event. This event occurs when
            // the current item is changed, but not when a field of the current
            // item is changed.
            bindingSource1.CurrentChanged += 
                new EventHandler(bindingSource1_CurrentChanged);

            // Handle the two events for caching and resetting the position.
            relatedCM.ListChanged += new ListChangedEventHandler(relatedCM_ListChanged);
            relatedCM.PositionChanged
                += new EventHandler(relatedCM_PositionChanged);

            // Set cacheing to true in case current changed event
            // occured on set up.
            cacheChildPosition = true;
        }


        // Establish the data set with two tables and a relationship
        // between them.
        private DataSet InitializeDataSet()
        {
            set1 = new DataSet();
            // Declare the DataSet and add a table and column.
            set1.Tables.Add("Customers");
            set1.Tables[0].Columns.Add("CustomerID");
            set1.Tables[0].Columns.Add("Customer Name");
            set1.Tables[0].Columns.Add("Contact Name");

            // Add some rows to the table.
            set1.Tables["Customers"].Rows.Add("c1", "Fabrikam, Inc.", "Ellen Adams");
            set1.Tables[0].Rows.Add("c2", "Lucerne Publishing", "Don Hall");
            set1.Tables[0].Rows.Add("c3", "Northwind Traders", "Lori Penor");
            set1.Tables[0].Rows.Add("c4", "Tailspin Toys", "Michael Patten");
            set1.Tables[0].Rows.Add("c5", "Woodgrove Bank", "Jyothi Pai");

            // Declare the DataSet and add a table and column.
            set1.Tables.Add("Orders");
            set1.Tables[1].Columns.Add("CustomerID");
            set1.Tables[1].Columns.Add("OrderNo");
            set1.Tables[1].Columns.Add("OrderDate");

            // Add some rows to the table.
            set1.Tables[1].Rows.Add("c1", "119", "10/04/2006");
            set1.Tables[1].Rows.Add("c1", "149", "10/10/2006");
            set1.Tables[1].Rows.Add("c1", "159", "10/12/2006");
            set1.Tables[1].Rows.Add("c2", "169", "10/10/2006");
            set1.Tables[1].Rows.Add("c2", "179", "10/10/2006");
            set1.Tables[1].Rows.Add("c2", "189", "10/12/2006");
            set1.Tables[1].Rows.Add("c3", "122", "10/04/2006");
            set1.Tables[1].Rows.Add("c4", "130", "10/10/2006");
            set1.Tables[1].Rows.Add("c5", "1.29", "10/14/2006");

            DataRelation dr = new DataRelation("custOrders",
                set1.Tables["Customers"].Columns["CustomerID"],
                set1.Tables["Orders"].Columns["CustomerID"]);
            set1.Relations.Add(dr);
            return set1;
        }
        private int cachedPosition = -1;
        private bool cacheChildPosition = true;

        void relatedCM_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Check to see if this is a caching situation.
            if (cacheChildPosition && cachePositionCheckBox.Checked) 
            {
                // If so, check to see if it is a reset situation, and the current
                // position is greater than zero.
                CurrencyManager relatedCM = sender as CurrencyManager;
                if (e.ListChangedType == ListChangedType.Reset && relatedCM.Position > 0)

                    // If so, cache the position of the child table.
                    cachedPosition = relatedCM.Position;
            }
        }
        void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            // If the CurrentChanged event occurs, this is not a caching 
            // situation.
            cacheChildPosition = false;
        }
        void relatedCM_PositionChanged(object sender, EventArgs e)
        {
            // Check to see if this is a caching situation.
            if (cacheChildPosition && cachePositionCheckBox.Checked)
            {
                CurrencyManager relatedCM = sender as CurrencyManager;

                // If so, check to see if the current position is 
                // not equal to the cached position and the cached 
                // position is not out of bounds.
                if (relatedCM.Position != cachedPosition && cachedPosition
                    > 0 && cachedPosition < relatedCM.Count)
                {
                    relatedCM.Position = cachedPosition;
                    cachedPosition = -1;
                }
            }
        }
        int count = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            // For demo purposes--modifies a value in the first row of the
            // parent table.
            DataRow row1 = set1.Tables[0].Rows[0];
            row1[1] = DBNull.Value;

        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }


}


 

To test the code example, perform the following steps:

Run the example.

Make sure the Cache and reset position check box is selected.

Click the Clear parent field button to cause a change in a field of the parent table. Notice that the selected row in the child table does not change.

Close and run the example again. You need to do this because the reset behavior occurs only on the first change in the parent row.

Clear the Cache and reset position check box.

Click the Clear parent field button. Notice that the selected row in the child table changes to the first row.

Compiling the Code
This example requires:

References to the System, System.Data, System.Drawing, System.Windows.Forms, and System.XML assemblies.

For information about how to build this example from the command line for Visual Basic or Visual C#, see Building from the Command Line (Visual Basic) or Command-line Building. You can also build this example in Visual Studio by pasting the code into a new project. For more information, see How to: Compile and Run a Complete Windows Forms Code Example Using Visual Studio.

See Also
Concepts
How to: Ensure Multiple Controls Bound to the Same Data Source Remain Synchronized
BindingSource Component
Data Binding and Windows Forms
Send feedback on this topic to Microsoft.
*/