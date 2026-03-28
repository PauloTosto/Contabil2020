using ApoioContabilidade.Financeiro.Models;
using ApoioContabilidade.PagarReceber;
using ApoioContabilidade.Services;
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

namespace ApoioContabilidade.Financeiro
{
    public partial class FrmFinAprop : Form
    {
        DataTable finAprop;
        MonteGrid oFinAprop;
        DataSet dsTabelasApropriadas;
        BindingSource bmSource;
        int ent_sai;
        BindingSource bmFinanceiro;
        double mestre;
        public bool mudou; // indica ao chamador que os registros ligados a este mestre, de alguma forma mudou no SERVIDOR
        // sendo necessario carregar
        public DataSet dadosAlteradosMovFin; 
        public  FrmFinAprop( int oent_sai, BindingSource obmFinanceiro)
        {
            InitializeComponent();

            mudou = false;
            dadosAlteradosMovFin = null;
            if (obmFinanceiro.Current == null) return;
            try
            {
                DataRowView orowView = (obmFinanceiro.Current as DataRowView);
                mestre = Convert.ToDouble(orowView["OUTRO_ID"]);

            }
            catch (Exception)
            {

                this.Close();
                return;
            }
            

          
             
            ent_sai = oent_sai;
            bmFinanceiro = obmFinanceiro;
            bmSource = new BindingSource(); 
            finAprop = new DataTable("FINAPROP");
          

            finAprop.Columns.Add("TPLANCAMENTO", Type.GetType("System.String")); // Doc.Fisc
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 20;

            finAprop.Columns.Add("DATA", Type.GetType("System.DateTime")); // 'Dta Pag';
            finAprop.Columns.Add("DOC_FISC", Type.GetType("System.String")); // Doc.Fisc
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 15;
           
           // finAprop.Columns.Add("DATA_APR", Type.GetType("System.DateTime")); // 'Dta Apr.';

            finAprop.Columns.Add("CTADEBITO", Type.GetType("System.String")); // Conta Apropriada 
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 25;

            finAprop.Columns.Add("CTACREDITO", Type.GetType("System.String")); // Conta Apropriada 
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 25;

            finAprop.Columns.Add("CTA_APROPRIADA", Type.GetType("System.String")); // Conta Apropriada 
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 33;


            finAprop.Columns.Add("Descritivo", Type.GetType("System.String")); // Descritivo 
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 70;

            finAprop.Columns.Add("Entradas", Type.GetType("System.Decimal"));
            finAprop.Columns.Add("Saidas", Type.GetType("System.Decimal"));

            finAprop.Columns.Add("Historico", Type.GetType("System.String")); // Descritivo 
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 70;



        

          
            finAprop.Columns.Add("Obs", Type.GetType("System.String")); // Descritivo 
            finAprop.Columns[finAprop.Columns.Count - 1].MaxLength = 80;

            finAprop.Columns.Add("VlrContabil", Type.GetType("System.Decimal"));

          




        }





        private void FrmFinAprop_Load(object sender, EventArgs e)
        {

            bmSource.DataSource = finAprop;

            MonteGrid();

            oFinAprop.oDataGridView = dgvFinAprop;
            oFinAprop.oDataGridView.DataSource = bmSource;
            oFinAprop.ConfigureDBGridView();
            Prepare_Relatorio();

          

        }
        private void MonteGrid()
        {
            oFinAprop = new MonteGrid();
            oFinAprop.Clear();

            oFinAprop.AddValores("TPLANCAMENTO", "Tipo Lançamento", 15, "", false, 0, "");
            oFinAprop.AddValores("DOC_FISC", "Documento", 12, "", false, 0, "");
            oFinAprop.AddValores("DATA", "Data", 11, "", false, 0, "");
            oFinAprop.AddValores("ENTRADAS", "Financeiro", 13, "##,###,##0.00", true, 0, "");
            oFinAprop.AddValores("SAIDAS", "Apropriação", 13, "##,###,##0.00", true, 0, "");
            oFinAprop.AddValores("CTA_APROPRIADA", "Conta Apropriada", 33, "", false, 0, "");
            oFinAprop.AddValores("DESCRITIVO", "Descritivo", 60, "", false, 0, "");
            oFinAprop.AddValores("HISTORICO", "Historico", 45, "", false, 0, "");
        }

        private async void Prepare_Relatorio()

        {
            bool result = await AcesseDados();
            if (!result)
            {
                this.Close();
                return;
            }
            DataTable movPgRc = dsTabelasApropriadas.Tables[0];
            DataTable movFin = dsTabelasApropriadas.Tables[1];
            DataTable movApro = dsTabelasApropriadas.Tables[2];

            DataTable ctaCentro = dsTabelasApropriadas.Tables[3];
            DataTable movEst = dsTabelasApropriadas.Tables[4];
            DataTable vendas = dsTabelasApropriadas.Tables[5];

            dsTabelasApropriadas.Tables[0].TableName = "MOV_PGRC";
            dsTabelasApropriadas.Tables[1].TableName = "MOV_FIN";
            dsTabelasApropriadas.Tables[2].TableName = "MOV_APRO";
            dsTabelasApropriadas.Tables[3].TableName = "CTACENTR";
            dsTabelasApropriadas.Tables[4].TableName = "MOVEST";
            dsTabelasApropriadas.Tables[5].TableName = "VENDAS";

            
            finAprop.Rows.Clear();
            finAprop.AcceptChanges();

            DataRow orowfinApr = finAprop.NewRow();

            DataRow orowMestre = movPgRc.Rows[0];

            Decimal valorMestre = 0;
            
            foreach (DataRow orow in movPgRc.Rows)
            {
                DataRow orowfinAp = finAprop.NewRow();
                orowfinAp["TPLANCAMENTO"] = "" + (ent_sai == 2 ? "FORNECEDOR " : "CLIENTE");
                orowfinAp["DATA"] = orow["DATA"];
                orowfinAp["DOC_FISC"] = orow["DOC_FISC"];
                if (ent_sai == 1) //receber 2 no pgrc
                   orowfinAp["CTA_APROPRIADA"] = orow["DEBITO"].ToString(); // temos a receber 
                                                                    // }
                                                                    // else     // pagar 1 no pgrc
                else                                                     // {
                   orowfinAp["CTA_APROPRIADA"] = orow["CREDITO"].ToString();
                //}
           //     orowfinAp["CONTA_APROP"] = conta;
              //  orowfinAp["VALORMST"] = orow["VALOR"];

                if (ent_sai == 1) 
                {
                    orowfinAp["ENTRADAS"]  = orow["VALOR"] ;
                }
                else
                {
                    orowfinAp["ENTRADAS"] = Convert.ToDecimal(orow["VALOR"]) * -1;
                    
                }
                orowfinAp["HISTORICO"] = orow["HIST"];

                valorMestre = Convert.ToDecimal(orow["VALOR"]);

                finAprop.Rows.Add(orowfinAp);
                orowfinAp.AcceptChanges();
            }

            foreach (DataRow orow in movFin.Rows)
            {
                DataRow orowfinAp = finAprop.NewRow();
                orowfinAp["TPLANCAMENTO"] = "FINANCEIRO ";
                orowfinAp["DATA"] = orow["DATA"];
                orowfinAp["DOC_FISC"] = orow["DOC"];
                string banco = "";
                string descritivo = "";
                if (ent_sai == 2)
                {
                    if (orow["CREDITO"].ToString().Trim() == "00")
                    {
                        banco = "CONTA FINANCEIRA NÃO LANÇADA";
                    }
                    else
                    {
                        banco = orow["CREDITO"].ToString().Trim() + " " + TabelasIniciais.NomeBanco(orow["CREDITO"].ToString().Trim()).Trim();
                        if (banco.Length > 25)
                            banco = banco.Substring(0, 25);
                    }
                    orowfinAp["CTA_APROPRIADA"] = banco;
                    descritivo = orow["DEBITO"].ToString().Trim();
                }
                else
                {
                    if (orow["DEBITO"].ToString().Trim() == "00")
                    {
                        banco = "CONTA FINANCEIRA NÃO LANÇADA";
                    }
                    else
                    {
                        banco = orow["DEBITO"].ToString().Trim() + " " + TabelasIniciais.NomeBanco(orow["CREDITO"].ToString().Trim()).Trim();
                        if (banco.Length > 33)
                            banco = banco.Substring(0, 33);
                    }
                    orowfinAp["CTA_APROPRIADA"] = banco;
                    descritivo = orow["CREDITO"].ToString().Trim();
                }
                //  orowfinAp["VALORMST"] = orow["VALOR"];

                if (ent_sai == 1)
                {
                    orowfinAp["ENTRADAS"] = Convert.ToDecimal(orow["VALOR"]) * -1 ;
                }
                else
                {
                    orowfinAp["ENTRADAS"] = orow["VALOR"];
                }
                descritivo = descritivo + "["+ orow["FORN"].ToString().Trim();
                if (descritivo.Length > 60)
                    descritivo = descritivo.Substring(0, 59) + "]";
                orowfinAp["DESCRITIVO"] = descritivo;
                orowfinAp["HISTORICO"] = orow["HIST"];
                
                finAprop.Rows.Add(orowfinAp);
                orowfinAp.AcceptChanges();
            }
          

          
            foreach (DataRow orow in ctaCentro.Rows)
            {
                string setor = TabelasIniciais.SetorDesc(orow["SETOR"].ToString()).Trim();
                if (!orow.IsNull("GLEBA") && (orow["GLEBA"].ToString().Trim() != ""))
                    setor = setor + " Centro:" + orow["GLEBA"].ToString();
                if (!orow.IsNull("QUADRA") && (orow["QUADRA"].ToString().Trim() != ""))
                    setor = setor + " Quadra:" + orow["QUADRA"].ToString();

                string modelo = TabelasIniciais.ModeloDesc(orow["NUM_MOD"].ToString()).Trim() + " - " +
                       TabelasIniciais.ServicoDesc(orow["CODSER"].ToString()).Trim();
                if (!orow.IsNull("ICODSER")) modelo = modelo + "/" + TabelasIniciais.TipoMovDesc(Convert.ToDouble(orow["CODSER"]));

                Decimal valor = Convert.ToDecimal(orow["VALOR"]);

                DataRow orowfinAp = finAprop.NewRow();
                orowfinAp["TPLANCAMENTO"] = "APROP.GERENCIAL";
                orowfinAp["DATA"] = orow["DATA"];

                if (orow["DBCR"].ToString() == "C")
                {
                    orowfinAp["SAIDAS"] = valor * -1;
                }
                else
                {
                    orowfinAp["SAIDAS"] = valor;
                }
                if (setor.Length > 33)
                    setor = setor.Substring(0, 33);
                orowfinAp["CTA_APROPRIADA"] = setor;
                orowfinAp["DESCRITIVO"] = modelo;
                orowfinAp["HISTORICO"] = orow["HISTORICO"].ToString();

                finAprop.Rows.Add(orowfinAp);
                orowfinAp.AcceptChanges();
               
            }


            foreach (DataRow orow in movEst.Rows)
            {
                string setor = "Almox:" + orow["DEPOSITO"].ToString() + "  " +TabelasIniciais.SetorDesc(orow["SETOR"].ToString()).Trim();
              //  if (!orow.IsNull("GLEBA") && (orow["GLEBA"].ToString().Trim() != ""))
                //    setor = setor + " Centro:" + orow["GLEBA"].ToString();
                //if (!orow.IsNull("QUADRA") && (orow["QUADRA"].ToString().Trim() != ""))
                  //  setor = setor + " Quadra:" + orow["QUADRA"].ToString();

               /* string modelo = TabelasIniciais.ModeloDesc(orow["NUM_MOD"].ToString()).Trim() + " - " +
                       TabelasIniciais.ServicoDesc(orow["CODSER"].ToString()).Trim();
                if (!orow.IsNull("ICODSER")) modelo = modelo + "/" + TabelasIniciais.TipoMovDesc(Convert.ToDouble(orow["CODSER"]));
               */
                Decimal valor = Convert.ToDecimal(orow["VALOR"]);
                
                string descritivo  = TabelasIniciais.EstoqueDesc(orow["COD"].ToString()).Trim() + ":" +
                    orow["QUANT"].ToString().Trim() + " "+ TabelasIniciais.EstoqueUnid(orow["COD"].ToString()).Trim();

                

                DataRow orowfinAp = finAprop.NewRow();
                orowfinAp["TPLANCAMENTO"] = "Compras Deposito";
                orowfinAp["DATA"] = orow["DATAC"];

                orowfinAp["SAIDAS"] = valor;

                if (setor.Length > 33)
                    setor = setor.Substring(0, 33);

                orowfinAp["CTA_APROPRIADA"] = setor;
                if (descritivo.Length > 60)
                    descritivo = descritivo.Substring(0, 60);

                orowfinAp["DESCRITIVO"] = descritivo;
               // orowfinAp["HISTORICO"] = orow["HISTORICO"];

                finAprop.Rows.Add(orowfinAp);
                orowfinAp.AcceptChanges();

                
            }


            // Decimal vlr_Apro = movApro.AsEnumerable().Sum(row => row.Field<Decimal>("VALOR"));
            foreach (DataRow orow in movApro.Rows)
            {
                DataRow orowfinAp = finAprop.NewRow();
                orowfinAp["TPLANCAMENTO"] = "APROP CONTABIL";
                orowfinAp["DATA"] = orow["DATA"];
                orowfinAp["DOC_FISC"] = orow["DOC"];
                
                if (orow["CREDITO"].ToString().Trim() != "")
                {
                    orowfinAp["CTA_APROPRIADA"] = orow["CREDITO"].ToString();
                }
                else
                {
                    orowfinAp["CTA_APROPRIADA"] = orow["DEBITO"].ToString();
                }

                if (orow["CREDITO"].ToString().Trim() == "")
                {
                
                    orowfinAp["SAIDAS"] = Convert.ToDecimal(orow["VALOR"]) * -1;
                }
                else
                {
                    orowfinAp["SAIDAS"] = orow["VALOR"];
                }
               
                orowfinAp["HISTORICO"] = orow["HIST"].ToString();

                finAprop.Rows.Add(orowfinAp);
                orowfinAp.AcceptChanges();
            }

            bmSource.DataSource = finAprop;
            oFinAprop.oDataGridView.DataSource = bmSource;
            oFinAprop.oDataGridView.Refresh();
            oFinAprop.FuncaoSoma();
            colocaSaldo();
            Decimal total_rateio = 0;
            Decimal vlr_ger = 0;
            try
            {
                vlr_ger = Convert.ToDecimal(ctaCentro.AsEnumerable().Sum(row => row.Field<Double>("VALOR")));
            }
            catch (Exception E)
            {


            }
            Decimal vlr_Est = 0;
            try
            {
                vlr_Est = Convert.ToDecimal(movEst.AsEnumerable().Sum(row => row.Field<Double>("VALOR")));
            }
            catch (Exception)
            {

            }

            Decimal vlr_Aprop = 0;
            try
            {
                vlr_Aprop = Convert.ToDecimal(movApro.AsEnumerable().Sum(row => row.Field<Double>("VALOR")));
            }
            catch (Exception E)
            {


            }


            string mensagem_erro = "";

            total_rateio = vlr_ger + vlr_Est;


            if (total_rateio != valorMestre)
            {
                mensagem_erro = String.Format("[(Gerencial + Estoque) # Mestre ={0:#,###,##0.00}] ", (valorMestre - total_rateio));

            }

            if (vlr_Aprop != valorMestre)
            {
                if (mensagem_erro != "") mensagem_erro = mensagem_erro + " e ";
                mensagem_erro = mensagem_erro + String.Format(" [Apropriaçao # Mestre = {0:#,###,##0.00}] ", (valorMestre - vlr_Aprop));

            }

            toolStripEquivalentes.Text = "";
            if (mensagem_erro != "")
            {
                mensagem_erro = "Erro ->  " + mensagem_erro;
                toolStripEquivalentes.Text = mensagem_erro;
            }
        }
        private void colocaSaldo()
        {
            decimal entradas = 0;
            if (oFinAprop.dictCampoTotal.ContainsKey("ENTRADAS"))
                entradas = Convert.ToDecimal(oFinAprop.dictCampoTotal["ENTRADAS"]);
            toolStripFinanceiro.Text = String.Format(": {0:#,###,##0.00}", entradas);

            //toolEntradas.Visible = true;
        
            decimal saidas = 0;
            if (oFinAprop.dictCampoTotal.ContainsKey("SAIDAS"))
                saidas = Convert.ToDecimal(oFinAprop.dictCampoTotal["SAIDAS"]);
            toolStripAprop.Text = String.Format(": {0:#,###,##0.00}", saidas);


          
        }


        #region AcessosDados

        private async void button1_Click(object sender, EventArgs e)
        {
            int tipoConta = (ent_sai == 1 ? 2 : 1);

            foreach(DataTable otable in dsTabelasApropriadas.Tables)
            {
                dsTabelasApropriadas.AcceptChanges();
            }
            

             FrmPagarReceber oform = new FrmPagarReceber(tipoConta, dsTabelasApropriadas.Copy());
            oform.ShowDialog();

            mudou = await Sao_Diferentes();

            if (mudou)
            {

                Prepare_Relatorio();

                //oFinAprop.FuncaoSoma();
                //colocaSaldo();


            }

            // verifique se mudou o financeiro;


        }

        private async Task<bool> Sao_Diferentes()
        {
            bool result = false;
            DataSet odataSetMudouMovFin = await FinApropExtendida.FinAprop_MovFIn(mestre);
            if (odataSetMudouMovFin == null)
            {
                result = true;
                return result;
            }
            DataTable MovFinAlterado = new DataTable();
            if (odataSetMudouMovFin.Tables.Count > 0)
                MovFinAlterado = odataSetMudouMovFin.Tables[0];
            DataTable MovFinOrig = dsTabelasApropriadas.Tables[1];
            if (MovFinAlterado.Rows.Count != MovFinOrig.Rows.Count)
            {
                result = true;
            }
            else
            {
                foreach (DataRow oroworg in MovFinOrig.Rows)
                {
                    bool igual = false;

                    foreach (DataRow orowalt in MovFinAlterado.Rows)
                    {
                        bool diferente = false;
                        foreach (DataColumn ocol in orowalt.Table.Columns)
                        {
                            if (orowalt[ocol.ColumnName] != oroworg[ocol.ColumnName])
                            {
                                diferente = true;
                                break;
                            }
                        }
                        if (!diferente)
                        {
                            igual = true;
                            break;
                        }

                    }
                    if (!igual)
                    {
                        result = true;
                        break;
                    }
                }
            }

            if (result)
                this.dadosAlteradosMovFin = odataSetMudouMovFin.Copy();

            return result;
        }

        private async Task<bool> AcesseDados()
        {
            bool result = true;

            try
            {

                dsTabelasApropriadas = await FinApropExtendida.FinAprop(mestre);
                if (dsTabelasApropriadas == null)
                    result = false;
            }
            catch (Exception)
            {

                result = false;
            }

            return result;
        }

        #endregion

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
