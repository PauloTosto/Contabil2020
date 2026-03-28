using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.Models;
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

namespace ApoioContabilidade.Trabalho
{
    public partial class FrmConsultaTrab : Form
    {
        const DayOfWeek _DIADASEMANA = DayOfWeek.Thursday;
        const DayOfWeek _DIAFIMDASEMANA = DayOfWeek.Wednesday;

        


        private DateTime dataAnterior;
        public Pesquise oPesqTrab;
        private AtualizaFolha_Cli ofolhaAtual;

        private Dictionary<string, CltCodigo> ListaCodigo;

       // private DataTable tabCltCad;
        // private DataTable tabListaTrab;

        private ServCltPonto servPonto;

        public FrmConsultaTrab()
        {
            InitializeComponent();
            txAdministrador.Visible = false;
            monthCalendar1.FirstDayOfWeek = Day.Thursday;
            DateTime hoje = monthCalendar1.TodayDate;
            monthCalendar1.ShowTodayCircle = false;
            monthCalendar1.ShowWeekNumbers = true;
            monthCalendar1.MaxSelectionCount = 1;
            setDatasExistentes(DateTime.Now);
            dtData1.Enabled = true;
            dataAnterior = Util.DiadoPonto(dtData1.Value, _DIADASEMANA);
           // btnFiltro.Enabled = false;
            btnConsulta.Enabled = false;
            oPesqTrab = new Pesquise();//TPesquisa.Create(self);
            oPesqTrab.Parent = this;
            oPesqTrab.TabIndex = 0;
            oPesqTrab.Left = 0;
            oPesqTrab.Top = this.Top + pnTop.Top + pnTop.Height + 2;
            oPesqTrab.Width = pnTop.Width;
            oPesqTrab.Height = (this.ClientSize.Height - oPesqTrab.Top);

            TabelasIniciaisConfigura();
            

        }


        private async void TabelasIniciaisConfigura()
        {
            if (!TabelasIniciais_Trab.TabelasIniciaisOk())
            {
                try
                {
                    btnConsulta.Enabled = await TabelasIniciais_Trab.Execute();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                btnConsulta.Enabled = true;
            }
          
            if (btnConsulta.Enabled)
            {
                ofolhaAtual = new AtualizaFolha_Cli();
                ofolhaAtual.Gera_Arquivo();
                ListaCodigo = ofolhaAtual.Enche_TabCodigo();
                servPonto = new ServCltPonto();
                servPonto.ListaCodigo = ofolhaAtual.Enche_TabCodigo();
            }
            if (!TabelasIniciais.TabelasIniciaisOk())
            {
                bool espera = await TabelasIniciais.Execute();
            }
        }


        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            ServicoFiltroTrab servicoFiltroTrab = new ServicoFiltroTrab(dtData1.Value, servPonto);

            bool result = await servicoFiltroTrab.FiltroTrabMovRapidoNovo();
            if (result == false)
            {
                return;
            }
            FrmCltPonto frmCltPonto = new FrmCltPonto(servicoFiltroTrab);
            frmCltPonto.tsLabelSemana.Text = "SEMANA:" + dtData1.Value.ToString("dd/MM/yyyy");
            frmCltPonto.frmConsultaTrab = this; // ref consulta
            frmCltPonto.StartPosition = FormStartPosition.CenterScreen;
            if (txAdministrador.Text.Trim() == "Edite")
                frmCltPonto.administrador = true;
            frmCltPonto.Show();
        }


        private async void setDatasExistentes(DateTime dateTime)
        {
            DateTime inicial = dateTime.AddMonths(-5);
            inicial = new DateTime(inicial.Year, inicial.Month, 1);
            DateTime final = dateTime.AddMonths(2);
            string query = "SELECT DATA FROM CLTPONTO WHERE DATA >= '" + inicial.ToString("yyyy-MM-dd") + "' AND DATA <= '"
                + final.ToString("yyyy-MM-dd") +
                       "' GROUP BY DATA ORDER BY DATA";
            DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[] { query }));
            monthCalendar1.RemoveAllAnnuallyBoldedDates();
            if ((odataset != null) && (odataset.Tables != null) && (odataset.Tables.Count > 0))
            {
                foreach (DataRow orow in odataset.Tables[0].Rows)
                {
                    monthCalendar1.AddAnnuallyBoldedDate(Convert.ToDateTime(orow["DATA"]));
                }

            }
            monthCalendar1.UpdateBoldedDates();


        }

        private void FrmConsultaTrab_Load(object sender, EventArgs e)
        {
            dtData1.Value = dataAnterior;
            dtData1.Refresh();



        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (monthCalendar1.Visible)
                monthCalendar1.Visible = false;
            else
                monthCalendar1.Visible = true;
        }

        private void dtData1_ValueChanged(object sender, EventArgs e)
        {
            if (dataAnterior.CompareTo(dtData1.Value) == 0) return;
            if (dataAnterior.CompareTo(dtData1.Value.AddDays(-1)) == 0)
            {
                dtData1.Value = dataAnterior.AddDays(7);
            }
            else if (dataAnterior.CompareTo(dtData1.Value.AddDays(1)) == 0)
            {
                dtData1.Value = dataAnterior.AddDays(-7);
            }
            else
            {
                dtData1.Value = Util.DiadoPonto(dtData1.Value, _DIADASEMANA);
            }
            dataAnterior = dtData1.Value;
            //monthCalendar1.TodayDate = dtData1.Value;
            monthCalendar1.SetDate(dtData1.Value);
            //monthCalendar1.SelectionRange.End = dtData1.Value;
            //if (monthCalendar1.Visible)
              //monthCalendar1.Update();
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime inicial = e.Start;
            DateTime final = e.End;
            dtData1.Value = Util.DiadoPonto(inicial, _DIADASEMANA);
            (sender as MonthCalendar).Visible = false;

        }

        private void btnSair_Click(object sender, EventArgs e)
        {
                   this.Close();

        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (!txAdministrador.Visible)
                txAdministrador.Visible = true;
            else txAdministrador.Visible = false;
        }
    }
}
