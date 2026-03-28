using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using ClassFiltroEdite.UserControls;

namespace ClassFiltroEdite
{

    public class ArmeEdicao : Control
    {
        public List<Linha> Linhas;
        private List<TGuardaAnt> TodosCampos;
        private Panel EPanel;
        private BindingSource BmSource;
        public BindingNavigator Navegador;
        public FormDlgForm oFormEdite;
        private string camposelecionado; // se refere ao cmpo selecionado no brawse quando é acionada a EDIÇÃO
        public ToolStripButton cancele, salve, novodelete, novo, sair;
        private bool IncluaPrimeiro;
        private Control firstControl;
        public bool IncluiAutomatico = true;
        private DialogResult retorno;
      //  public System.Windows.Forms.ErrorProvider errorProvider1;

        //   private bool novoNaoSalvo;
        // private bool mouseDown;
        // private Point lastLocation;


        private Boolean novomodeloedicao;
        //int Top_Pad, Left_Pad;
        //Boolean SempreVisivel;
        Boolean mudou;
        // evento de gravação do Edite
        public ArmeEdicao()
        {
            Linhas = new List<Linha>();
            novomodeloedicao = false;
        }
        public ArmeEdicao(BindingSource oBmSource)
        {
            oFormEdite = new FormDlgForm();
            //  oFormEdite.Force = true;
            oFormEdite.oArmeEdicao = this;

            BmSource = oBmSource;
            mudou = false;
            Linhas = new List<Linha>();
            TodosCampos = new List<TGuardaAnt>();
            EPanel = new Panel();
            EPanel.Parent = oFormEdite;
            EPanel.Visible = true;

            Top = -1;
            Left = -1;
            Width = 0;
            Height = 0;
            novomodeloedicao = false;

        }
        public Boolean NovoModeloEdicao
        {
            get { return novomodeloedicao; }
            set { novomodeloedicao = value; }
        }

        public int Add(string campo)  // para ficar compativel com TArmeEdicao
        {
            Linhas.Add(new Linha(campo));
            return Linhas.Count;
        }
        public int Add(Linha olinha)  // para ficar compativel com TArmeEdicao
        {
            Linhas.Add(olinha);
            return Linhas.Count;
        }
        public void Clear() //para ficar +- compativel com TarmeEdicao (delphi 7)
        {
            for (int i = 0; i < Linhas.Count - 1; i++)
            {
                Linha oLinha = Linhas[i];
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if (oLinha.oedite[j] != null)
                    {
                        oLinha.oedite[j].Enabled = false;
                        oLinha.oedite[j].Visible = false;
                    }
                }
            }
            Linhas.Clear();
        }


        //classes auxiliares da MOnteEdicao
        class TPosicaoLin { public int cabtop, cableft, cabtam, cabalt, objtop, objleft, objtam, objalt, adc_tam, adc_alt; }
        struct TLinEdicao
        {
            public List<TPosicaoLin> oListLin;
            public int maxtam, maxalt;
        }
        struct TGuardaAnt
        {
            public Control curcontrol;
            public List<string> Anterior;
            public string cabecalho;
            // public TPosicaoLin posicaoLin;
        }

        public void MonteEdicao() // antes ArmeEdicao
        {
            if (Linhas.Count == 0) return;
            if (NovoModeloEdicao)
            {
                MessageBox.Show("Nao Implementado Novo Modelo");
                //MonteEdicaoNovo();
                return;
            }
            Font fontForm = oFormEdite.Font;
            Point ponto = new Point(Convert.ToInt16(fontForm.Size), Convert.ToInt16(fontForm.Height));
            List<TLinEdicao> olistpos = new List<TLinEdicao>();
            int maxtam = 0;

            int margem_left = 4 * 4;
            int inter_min = (2 * ponto.X);
            for (int i = 0; i < Linhas.Count; i++)
            {
                TLinEdicao olinha = new TLinEdicao();
                olinha.oListLin = new List<TPosicaoLin>();
                olinha.maxtam = 0;//margem_left;
                olinha.maxalt = 0;
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if ((Linhas[i].oedite[j] != null) && (Linhas[i].oedite[j] is Control))
                    {
                        TGuardaAnt objGuarde = new TGuardaAnt();
                        objGuarde.curcontrol = (Control)Linhas[i].oedite[j];
                        objGuarde.Anterior = new List<string>();
                        objGuarde.cabecalho = Linhas[i].cabecalho[j];
                        TodosCampos.Add(objGuarde);
                        TPosicaoLin oposicao = new TPosicaoLin();
                        oposicao.cabtam = (Linhas[i].cabecalho[j].Length * ponto.X) + inter_min;
                        oposicao.cabalt = ponto.Y + 4; // padrao
                        oposicao.objalt = Linhas[i].oedite[j].Height + 2;
                        oposicao.objtam = Linhas[i].oedite[j].Width + inter_min;

                        if (((Control)Linhas[i].oedite[j]) is TextBoxBase)
                            oposicao.objalt = oposicao.objalt + 6;

                        if ((i == Linhas.Count - 1) && (((Control)Linhas[i].oedite[j]) is CheckedListBox)) // TDBEditCheckList o checklist do Historico
                            oposicao.adc_tam = Linhas[i].oedite[j].Width + inter_min;

                        if ((oposicao.objtam + oposicao.adc_tam) > oposicao.cabtam)
                            olinha.maxtam = olinha.maxtam + oposicao.objtam + oposicao.adc_tam;
                        else
                            olinha.maxtam = olinha.maxtam + oposicao.cabtam;

                        if ((oposicao.cabalt + oposicao.objalt) > olinha.maxalt)
                            olinha.maxalt = (oposicao.cabalt + oposicao.objalt);
                        olinha.maxtam = olinha.maxtam + inter_min;//dist. horiz entre objetos
                        olinha.oListLin.Add(oposicao);
                    }
                    else
                        olinha.oListLin.Add(null);
                };
                if (olinha.maxtam > maxtam)
                {
                    maxtam = olinha.maxtam;

                };

                olistpos.Add(olinha);
            }

            int altura = 8;

            int ttop = altura;
            for (int i = 0; i < olistpos.Count; i++)
            {
                TLinEdicao olinha = olistpos[i];
                int intervalo = 0;
                if (maxtam > olinha.maxtam)
                    intervalo = (maxtam - olinha.maxtam);
                int num_objs = 0;
                for (int j = 0; j < olinha.oListLin.Count; j++)
                    if (olinha.oListLin[j] != null) num_objs += 1;//INC(Num_objs,1);
                if ((intervalo != 0) && (num_objs > 2))
                    intervalo = intervalo / num_objs;
                if (intervalo < inter_min) intervalo = inter_min;
                int tleft = margem_left;
                for (int j = 0; j < olinha.oListLin.Count; j++)
                {
                    if (olinha.oListLin[j] != null)
                    {
                        TPosicaoLin oposicao = olinha.oListLin[j];
                        oposicao.cableft = tleft;
                        oposicao.objleft = tleft;
                        oposicao.cabtop = ttop;
                        oposicao.objtop = (ttop + oposicao.cabalt);
                        if (oposicao.objtam > oposicao.cabtam)
                            tleft = tleft + oposicao.objtam + intervalo;
                        else
                            tleft = tleft + oposicao.cabtam + intervalo;
                        olinha.oListLin[j] = oposicao;
                    }
                }
                ttop = ttop + olinha.maxalt + 8;
                altura = altura + olinha.maxalt + 8;
                olistpos[i] = olinha;
            };
            altura = altura + 16;
            int tamanho = maxtam + margem_left + margem_left;

            EPanel.Dock = DockStyle.Fill;
            EPanel.BorderStyle = BorderStyle.Fixed3D;
            EPanel.TabStop = false;
            EPanel.Visible = true;
            EPanel.Enabled = true;

            EPanel.Width = tamanho;
            EPanel.Height = altura;
            this.Width = tamanho + 100;
            this.Height = altura + 100;

              
            tamanho = 0;
            altura = 0;

            int ttaborder = 0;
            for (int i = 0; i < Linhas.Count; i++)
            {
                TLinEdicao olinha = olistpos[i];
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if ((Linhas[i].oedite[j] != null) && (Linhas[i].oedite[j] is Control))
                    {
                        TPosicaoLin oposicao = olinha.oListLin[j];
                        GroupBox objGroup = new GroupBox(); // TGroupBox.Create(EPanel);
                        objGroup.Parent = EPanel;
                        objGroup.Visible = true;
                        objGroup.Enabled = true;
                        objGroup.TabStop = false;

                        if ((Linhas[i].oedite[j] as Control) is UserControl)
                        {

                            objGroup.Left = oposicao.objleft;
                            objGroup.Top = oposicao.objtop;
                            //if (oposicao.cabtam < oposicao.objtam)
                            objGroup.Width = oposicao.objtam - inter_min;
                            //else
                            //  objGroup.Width = oposicao.cabtam;
                            objGroup.Height = oposicao.objalt;
                            objGroup.Text = Linhas[i].cabecalho[j].Trim();
                        }
                        else
                        {
                            objGroup.Left = oposicao.cableft - 6;
                            objGroup.Top = oposicao.cabtop;
                            objGroup.Width = oposicao.objtam + 4 + 4;
                            objGroup.Height = oposicao.objalt + oposicao.cabalt + 8;
                            objGroup.Text = Linhas[i].cabecalho[j];
                        }

                        Linhas[i].oedite[j].Parent = EPanel;
                        Linhas[i].oedite[j].Enabled = true;
                        Linhas[i].oedite[j].Visible = true;
                        Linhas[i].oedite[j].TabIndex = ttaborder;
                        Linhas[i].oedite[j].TabStop = true;
                        Linhas[i].oedite[j].Left = oposicao.objleft;
                        Linhas[i].oedite[j].BackColor = Color.White;
                        Linhas[i].oedite[j].Top = oposicao.objtop;
                        Linhas[i].oedite[j].BringToFront();
                        ttaborder += 1;
                        
                    }
                }
            }

            BmSource.BindingComplete += new BindingCompleteEventHandler(BmSource_BindingComplete);
            try
            {
                for (int i = 0; i < Linhas.Count; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count; j++)
                    {
                        if (oEdites[j] == null) break;
                        // oEdites[j].Validated -= new EventHandler(ArmeEdicao_Validated);
                        oEdites[j].Validated += new EventHandler(ArmeEdicao_Validated);
                        oEdites[j].KeyDown += ArmeEdicao_KeyDown;
                    }
                }
            }
            catch (Exception)
            { }

           Navegador = new BindingNavigator(false);
           Navegador.Parent = EPanel;
           Navegador.Dock = DockStyle.Bottom;
           ConstruaNavegador(Navegador);
            Navegador.BindingSource = BmSource;
            oFormEdite.oNavegador = Navegador;



           oFormEdite.FormClosing += OFormEdite_FormClosing;
           oFormEdite.Activated += new EventHandler(oFormEdite_Activated);
          // oFormEdite.FormClosed += OFormEdite_FormClosed;

            if (oFormEdite.ClientSize.Width != Width)
                oFormEdite.ClientSize = new Size(Width, ClientSize.Height);
            if (oFormEdite.ClientSize.Height != Height)
                oFormEdite.ClientSize = new Size(ClientSize.Width, Height);
            oFormEdite.StartPosition = FormStartPosition.CenterParent;
            oFormEdite.modeloFiltro = true;
            oFormEdite.tamanho = 0; // Width;
            oFormEdite.altura = 0; // Height;
         
            oFormEdite.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            oFormEdite.AutoScroll = false;
            oFormEdite.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;  // Nones
            oFormEdite.MinimizeBox = false;
            oFormEdite.MaximizeBox = false;
            

        }

        
        // rotinas para permitir mexer no formdlg
        /*   private void FormEdite_MouseDown(object sender, MouseEventArgs e)
           {
               mouseDown = true;
               lastLocation = e.Location;
           }

           private void Formedite_MouseMove(object sender, MouseEventArgs e)
           {
               if (mouseDown)
               {
                   this.Location = new Point(
                       (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                   this.Update();
               }
           }

           private void FormEdite_MouseUp(object sender, MouseEventArgs e)
           {
               mouseDown = false;
           }
        */
      
        public Boolean Edite(Form oformpai)
        {
            return Edite(oformpai, "");
        }

        public Boolean Edite(Form oformpai, string field_selecionado, bool inclua = false)
        {
            Boolean result = false;

            IncluaPrimeiro = inclua;
            camposelecionado = field_selecionado;
            //Receber Notificação quando um control acabar de ser validado

            EventHandler<InicioEdicaoEventArgs> handler = OnPrimeiroRegistro;
            if (handler != null)
            {
                OnPrimeiroRegistro(this, new InicioEdicaoEventArgs((BmSource.Current as DataRowView)));
            }
            try
            {
                for (int i = 0; i < Linhas.Count; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count; j++)
                    {
                        if (oEdites[j] == null) break;
                        if (oEdites[j] is UserControl)
                        {
                            UserControlsValidation(oEdites[j], true);
                        }
                        else oEdites[j].CausesValidation = true;
                    }
                }
            }
            catch (Exception)
            { }
        
            cancele.Enabled = false;
            mudou = false;
            novo.Enabled = true;

            salve.Enabled = false;
            novodelete.Enabled = true;

            Int32 IN = BmSource.CurrencyManager.Count;
            if (oFormEdite.ShowDialog() == DialogResult.OK)
                result = true;
            else
                result = false;
            return result;
        }
        



        void oFormEdite_Activated(object sender, EventArgs e)
        {
            this.EPanel.Focus();
            if (IncluaPrimeiro)
            {
                IncluaPrimeiro = false;
                novo.PerformClick();
                return;
            }
            else
            if (BmSource.CurrencyManager.Count < 1)
            {
                novo.PerformClick();
                return;
            }
            //return; // desativei em 15 de setembro esta funÇão de se posicionar num campo selecionado
            // a pedito de lisnmar
            Panel oPanel = EPanel;
            if (camposelecionado == "")
            {
                for (int i = 0; i < Linhas.Count; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count; j++)
                    {
                        if (oEdites[j] == null) break;

                        if ((oEdites[j] as Control).CanFocus)
                        {
                            (oEdites[j] as Control).Focus();
                            i = Linhas.Count;
                            break;
                        }
                    }
                }

                return;
            }    
            {
                for (int i = 0; i < oPanel.Controls.Count; i++)
                {
                    if ((((Control)oPanel.Controls[i]).DataBindings.Count > 0) &&
                         (((Control)oPanel.Controls[i]).DataBindings[0].BindingMemberInfo.BindingField == camposelecionado))
                        if (((Control)oPanel.Controls[i]).CanFocus)
                        {
                            ((Control)oPanel.Controls[i]).Focus();
                            break;
                        }
                }
            }
        }

        private void OFormEdite_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (((DataRowView)BmSource.Current) == null) {
                RemeteFocusInicial();
                return;
            }
            DataRowView orow = (DataRowView)BmSource.Current;
            if (orow.Row == null) {
                RemeteFocusInicial();
                return;
            }
            if (salve.Enabled)
            {
                e.Cancel = true;
                BindingSource_Mudou(BmSource);
                if (retorno != DialogResult.Yes)
                {
                    if (orow.Row.RowState == DataRowState.Modified)
                    {
                        orow.Row.CancelEdit();
                        orow.Row.RejectChanges();
                    }
                    else if ((orow.Row.RowState == DataRowState.Added) || (orow.Row.RowState == DataRowState.Detached) )
                    {
                        orow.Row.CancelEdit();
                        orow.Delete();
                    }
                }
            }
            else
            {
                if ( (orow.Row.RowState == DataRowState.Detached) || (orow.Row.RowState == DataRowState.Added)) // não permte que algum registre que esteja editando seja retornado com um valor não checado ainda
                {
                    BmSource.CancelEdit();
                    orow.Delete();
                    int pos = BmSource.CurrencyManager.Position;
                    BmSource.CurrencyManager.Refresh();
                }
            }
            orow.Row.Table.AcceptChanges();
            RemeteFocusInicial();
        }

        private void RemeteFocusInicial()
        {
            try
            {
                for (int i = 0; i < Linhas.Count; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count; j++)
                    {
                        if (oEdites[j] == null) break;
                        // oEdites[j].Validated -= new EventHandler(ArmeEdicao_Validated);
                        if (oEdites[j] is UserControl)
                        {
                            UserControlsValidation(oEdites[j], false);
                            /*reach (Control ocontrol in oEdites[j].Controls)
                            {
                                if (ocontrol.CanFocus)
                                    ocontrol.CausesValidation = false;
                            }*/
                        }
                        else oEdites[j].CausesValidation = false;
                    }
                }
            }
            catch (Exception)
            { }

            try
            {
                oFormEdite.errorProvider1.SetError(oFormEdite.ActiveControl, "");
            }
            catch (Exception)
            {

            }

            oFormEdite.errorProvider1.Clear();

            this.EPanel.Focus();

        }

        private void UserControlsValidation(Control userControl, bool validacao)
        {
            foreach (Control ocontrol in userControl.Controls)
            {
                if (ocontrol is ComboBox)
                    ocontrol.CausesValidation = validacao;
                else
                    UserControlsValidation(ocontrol, validacao);
            }
        }




        private void OFormEdite_FormClosed(object sender, FormClosedEventArgs e)
        {
/*           try
            {
                for (int i = 0; i < Linhas.Count; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count; j++)
                    {
                        if (oEdites[j] == null) break;
                        // oEdites[j].Validated -= new EventHandler(ArmeEdicao_Validated);
                        oEdites[j].CausesValidation = false;
                    }
                }
            }
            catch (Exception)
            { }

            try
            {
                oFormEdite.errorProvider1.SetError(oFormEdite.ActiveControl, "");
            }
            catch (Exception)
            {

            }
          
            oFormEdite.errorProvider1.Clear();

             this.EPanel.Focus();
  */          
        }






        // Comportamento durante Edicao       
        public event EventHandler<AlteraRegistroEventArgs> BeforeAlteraRegistros;
        public event EventHandler<AlteraRegistroEventArgs> AlteraRegistrosOk;
        public event EventHandler<AlteraRegistroEventArgs> DeletaRegistrosOk;
        public event EventHandler<AlteraRegistroEventArgs> BeforeDeletaRegistros;
        public event EventHandler<InicioEdicaoEventArgs> OnPrimeiroRegistro;

        public event EventHandler<AlteraRegistroEventArgs> AfterDeletaRegistros;
        public event EventHandler<AlteraRegistroEventArgs> AfterAlteraRegistros;
        public event EventHandler<AlteraRegistroEventArgs> DeletaRegistrosAdapter;


        public event EventHandler<AsyncAlteraRegistroEventArgs> AsyncAlteraRegistrosOk;


        // protected virtual bool OnAlteraLinhas(AlteraRegistroEventArgs e)
        public bool OnAlteraLinhas(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = AlteraRegistrosOk;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Cancela) return false;
            else
                return true;
        }


        public async Task<bool> OnAsyncAlteraLinhas(AsyncAlteraRegistroEventArgs e)
        {
            EventHandler<AsyncAlteraRegistroEventArgs> handler = AsyncAlteraRegistrosOk;
            if (handler != null)
            {
                handler(this, e);
                handler.Invoke(this, e);
                return await(e.Cancela ?? Task.FromResult(false));
            }
            bool result =  e.Cancela.Result;
            if (result) return false;
            else
                return true;
        }


        // protected virtual 
        public bool OnBeforeAltera(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = BeforeAlteraRegistros;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Cancela) return false;
            else
                return true;
        }

        //protected virtual 
        public bool OnBeforeDeleta(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = BeforeDeletaRegistros;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Cancela) return false;
            else
                return true;
        }

        public bool OnDeletaLinhas(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = DeletaRegistrosOk;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Cancela) return false;
            else
                return true;
        }

        public bool OnAfterDeleta(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = AfterDeletaRegistros;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Cancela) return false;
            else
                return true;
        }

        public bool OnDeletaAdapter(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = DeletaRegistrosAdapter;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Cancela) return false;
            else
                return true;
        }

        public bool OnAfterAltera(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = AfterAlteraRegistros;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Cancela) return false;
            else
                return true;
        }



        // a construir 
        public class BindingNavigatorPt : BindingNavigator
        {
            BindingNavigatorPt(Boolean complete)
            {
                if (complete)
                    AddStandardItems();

            }
        }

        // navegador para o edite
        public void ConstruaNavegador(BindingNavigator oNavegador )
        {
            
            oNavegador.AddStandardItems();
            // alteraçao do first
            ToolStripButton ofirst = (ToolStripButton)oNavegador.MoveFirstItem;
            oNavegador.Items.Remove(ofirst);
            ToolStripButton first = new ToolStripButton();
            first.Name = "Primeiro";
            first.Font = ofirst.Font;
            first.Image = ofirst.Image;
            first.Size = ofirst.Size;
            first.Click += new EventHandler(FirstItem_Click);


            // alteraçao do Previus
            ToolStripButton oprev = (ToolStripButton)oNavegador.MovePreviousItem;
            oNavegador.Items.Remove(oprev);
            ToolStripButton prev = new ToolStripButton();
            prev.Name = "Anterior";
            prev.Font = oprev.Font;
            prev.Image = oprev.Image;
            prev.Size = oprev.Size;
            prev.Click += new EventHandler(PrevItem_Click);

            // alteraçao do next
            ToolStripButton onext = (ToolStripButton)oNavegador.MoveNextItem;
            oNavegador.Items.Remove(onext);
            ToolStripButton next = new ToolStripButton();
            next.Name = "Proximo";
            next.Font = onext.Font;
            next.Image = onext.Image;
            next.Size = onext.Size;
            next.Click += new EventHandler(NextItem_Click);


            // alteraçao do last
            ToolStripButton olast = (ToolStripButton)oNavegador.MoveLastItem;
            oNavegador.Items.Remove(olast);
            ToolStripButton last = new ToolStripButton();
            last.Font = olast.Font;
            last.Name = "Ultimo";
            last.Image = olast.Image;
            last.Size = olast.Size;
            last.Click += new EventHandler(LastItem_Click);

            // alteraçao do delete
            ToolStripButton odelete = (ToolStripButton)oNavegador.DeleteItem;
            oNavegador.Items.Remove(odelete);
            novodelete = new ToolStripButton();
            novodelete.Font = odelete.Font;
            novodelete.Image = odelete.Image;
            novodelete.Size = odelete.Size;
            novodelete.Text = "&Delete";
            novodelete.Enabled = true;
            novodelete.Name = "NovoDelete";
            novodelete.TextImageRelation = TextImageRelation.ImageAboveText;
            novodelete.Click += new EventHandler(DeleteItem_Click);


            // alteraçao do novo
            ToolStripButton onovo = (ToolStripButton)oNavegador.AddNewItem;
            oNavegador.Items.Remove(onovo);
            novo = new ToolStripButton();
            novo.Name = "Novo";
            novo.Font = onovo.Font;
            novo.Image = onovo.Image;
            novo.Text = "&Novo";
            novo.TextImageRelation = TextImageRelation.ImageAboveText;
            novo.Size = onovo.Size;
            novo.Click += new EventHandler(AddNewItem_Click);
            //novo.Click += Novo_Click;



            oNavegador.Items.Insert(0, first);
            oNavegador.Items.Insert(1, prev);

            oNavegador.Items.Insert(3, next);

            oNavegador.Items.Insert(4, last);

            oNavegador.Items.Add(novodelete);
            oNavegador.Items.Add(novo);

            cancele = new ToolStripButton();
            cancele.Name = "Cancele";
            cancele.Font = onovo.Font;
            cancele.Text = "&Cancele";
            //cancele.
            cancele.Image = (System.Drawing.Image)ClassFiltroEditeResource.cancelaStripButton_Image;
            cancele.TextImageRelation = TextImageRelation.ImageAboveText;
            cancele.Size = onovo.Size;
            cancele.Enabled = false;
            cancele.Click += new EventHandler(cancele_Click);
            oNavegador.Items.Add(cancele);
            // oNavegador.Items[0].O

            salve = new ToolStripButton();

            salve.Name = "Salve";
            salve.Font = onovo.Font;
            salve.Text = "&Salve";
            salve.AccessibleDescription = "Salva as alterações pendentes no registro";
            salve.Image = (System.Drawing.Image)ClassFiltroEditeResource.saveToolStripButton_Image;//(resources.GetObject("saveToolStripButton.Image")));
            salve.TextImageRelation = TextImageRelation.ImageAboveText;
            salve.Size = onovo.Size;
            salve.Enabled = false;
            salve.Click += new EventHandler(salve_Click);
            oNavegador.Items.Add(salve);

            oNavegador.RefreshItems += new EventHandler(oNavegador_RefreshItems);
        }

      
        void oNavegador_RefreshItems(object sender, EventArgs e)
        {
            BindingNavigator oNavegador = (BindingNavigator)sender;
            int posicao = -1;
            if (oNavegador.BindingSource != null)
                posicao = oNavegador.BindingSource.Position;
            DataRowView orowview = null;
            if ((DataRowView)BmSource.Current != null)
                orowview = (DataRowView)BmSource.Current;
            int count = -1;
            if (oNavegador.BindingSource != null)
                count = oNavegador.BindingSource.Count;

            if (oNavegador.Items.ContainsKey("Primeiro"))
                if (posicao < 1)
                    oNavegador.Items["Primeiro"].Enabled = false;
                else
                    if (oNavegador.Items["Primeiro"].Enabled == false)
                    oNavegador.Items["Primeiro"].Enabled = true;
            if (oNavegador.Items.ContainsKey("Anterior"))
                if (posicao < 1)
                    oNavegador.Items["Anterior"].Enabled = false;
                else
                    if (oNavegador.Items["Anterior"].Enabled == false)
                    oNavegador.Items["Anterior"].Enabled = true;
            if (oNavegador.Items.ContainsKey("Ultimo"))
                if (posicao == (count - 1))
                    oNavegador.Items["Ultimo"].Enabled = false;
                else
                    if (oNavegador.Items["Ultimo"].Enabled == false)
                    oNavegador.Items["Ultimo"].Enabled = true;
            if (oNavegador.Items.ContainsKey("Proximo"))
                if (posicao == (count - 1))
                    oNavegador.Items["Proximo"].Enabled = true;
                else
                    if (oNavegador.Items["Proximo"].Enabled == false)
                    oNavegador.Items["Proximo"].Enabled = true;
        }

        void cancele_Click(object sender, EventArgs e)
        {
            // salve.Enabled = false;
            // mudou = false;
            // ZERA EVENTUAS MENSAGENS DE ErRO
            oFormEdite.errorProvider1.Clear();

            if ((DataRowView)BmSource.Current == null)
            {
                cancele.Enabled = false;
                if (this.oFormEdite.Visible)
                   this.oFormEdite.Close();
                return;
            }
            DataRow orow = null;
            try
            {
                orow = ((DataRowView)BmSource.Current).Row;
            }
            catch (Exception)
            {
                cancele.Enabled = false;
                if (this.oFormEdite.Visible)
                    this.oFormEdite.Close();
                return;

            }
           
            if ((orow.RowState == DataRowState.Unchanged)  || (orow.RowState == DataRowState.Modified))
            {
                BmSource.CancelEdit();
                orow.RejectChanges();

            }
            else if ((orow.RowState == DataRowState.Added) || (orow.RowState == DataRowState.Detached)
               || (((DataRowView)BmSource.Current).IsEdit) || (((DataRowView)BmSource.Current).IsNew))
            {
                // BindingSource_Mudou(BmSource);
                BmSource.CancelEdit();
                orow.Delete();
                int pos = BmSource.CurrencyManager.Position;
                if (pos >= 0)
                    BmSource.CurrencyManager.Refresh();
                //BmSource.CurrencyManager.Position = pos - 1;
                cancele.Enabled = false;
                if (BmSource.Position < 0)
                {
                    if (this.oFormEdite.Visible)
                        this.oFormEdite.Close();
                    return;
                }
               
              //  novodelete.Enabled = true;
            }
            else
            {
                BmSource.CancelEdit();
                orow.RejectChanges();

            }
            cancele.Enabled = false;
            if ((DataRowView)BmSource.Current == null)
            {
                mudou = false;
                novo.Enabled = true;

                salve.Enabled = false;
                novodelete.Enabled = true;
                if (this.oFormEdite.Visible)
                    this.oFormEdite.Close();
                return;
            }
            if (BmSource.Position < 0)
            {
                mudou = false;
                novo.Enabled = true;

                salve.Enabled = false;
                novodelete.Enabled = true;
                if (this.oFormEdite.Visible)
                    this.oFormEdite.Close();
                return;
            }
            // ((ToolStripButton)sender).Enabled = false;
            // salve.Enabled = false;
            mudou = false;
            novo.Enabled = true;
            
            salve.Enabled = false;
            novodelete.Enabled = true;
            // depois de um cancele POSICIONA SEMPRE NO PRIMEIRO CONTROLE DO FORME De eDICAO
            if ((Linhas.Count > 0) && (Linhas[0].oedite[0] != null))
            {
                firstControl = Linhas[0].oedite[0];
                if (!firstControl.Focused)
                {
                    firstControl.Focus();
                }
            }
        }

        void  salve_Click(object sender, EventArgs e)
        {

            BindingSource_Mudou(BmSource);

        }

        void DeleteItem_Click(object sender, EventArgs e)
        {
            //novoNaoSalvo = false;
            DataRow orow;
            try
            {
                orow = ((DataRowView)BmSource.Current).Row;
            }
            catch (Exception)
            {
                return;
            }
            novo.Enabled = true;

            if ((orow.RowState == DataRowState.Added) && (salve.Enabled == false))
            {
                BmSource.CancelEdit();
                BmSource.RemoveCurrent();
                BmSource.MovePrevious();
                if (BmSource.Position < 0)
                {
                    if (this.oFormEdite.Visible)
                    this.oFormEdite.Close();
                    return;
                }
            }
            else if (orow.RowState == DataRowState.Detached)
            {
                return;
              
            }
            else
            {
                if (OnBeforeDeleta(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                {

                    if (MessageBox.Show("Confirma Exclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            EventHandler<AlteraRegistroEventArgs> handler = DeletaRegistrosOk;
                            if (handler != null)
                            {
                                if (OnDeletaLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                                {
                                    ((DataRowView)BmSource.Current).Delete();
                                    
                                    // Para os Casos em que se usa uma Adapter (tabelas dbf)
                                    
                                    OnDeletaAdapter(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));


                                    // era assim 
                                    orow.Table.AcceptChanges();
                                    OnAfterDeleta(new AlteraRegistroEventArgs(new DataRow[] { null }, DataRowState.Deleted));
                                   
                                   

                                    if (BmSource.CurrencyManager.Count < 1)
                                    {
                                        if (this.oFormEdite.Visible)
                                            this.oFormEdite.Close();
                                    }
                                    //BmSource.AddNew();

                                }
                            }
                            else
                            {
                                // deleta só virtualmente ago/2020
                                ((DataRowView)BmSource.Current).Delete();
                                OnDeletaAdapter(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));
                                if (BmSource.CurrencyManager.Count < 1)
                                    if (this.oFormEdite.Visible)
                                        this.oFormEdite.Close();

                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        // não foi consentido deletar

                    }
                }
            }

        }

        void AddNewItem_Click(object sender, EventArgs e)
        {
            // BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;
            //DialogResult retorno;
            if ((DataRowView)BmSource.Current != null)
            {
               // retorno = BindingSource_Mudou(BmSource);
                if (retorno == DialogResult.Ignore) return;
                if (retorno == DialogResult.Abort) return;
            }
            //  BmSource.CurrencyManager.Refresh();
            DataRowView orowNovo = (DataRowView)BmSource.AddNew();
            if ((Linhas.Count > 0) && (Linhas[0].oedite[0] != null))
            {
                firstControl = Linhas[0].oedite[0];
                if (!firstControl.Focused)
                {
                    Control oControlOutroFocused = null;
                    if (this.oFormEdite.Visible)
                    {
                        oControlOutroFocused = this.oFormEdite.ActiveControl;
                        if (oControlOutroFocused is UserControl)
                        {
                            UserControlsValidation(oControlOutroFocused, false);
                        }
                        else oControlOutroFocused.CausesValidation = false;
                    }
                        //Linhas[0].oedite[0].Validated -= new EventHandler(ArmeEdicao_Validated);
                    firstControl.Focus();

                    if ((this.oFormEdite.Visible) && (oControlOutroFocused != null))
                    {
                        if (oControlOutroFocused is UserControl)
                        {
                            UserControlsValidation(oControlOutroFocused, true);
                        }
                        else oControlOutroFocused.CausesValidation = true;

                    }
                    // Linhas[0].oedite[0].Validated += new EventHandler(ArmeEdicao_Validated);
                }
            }
            //   if (oBindSource.IsBindingSuspended) return;
            // BmSource.SuspendBinding();
            
            int pos = 0;
            foreach(DataRowView obj in  BmSource.CurrencyManager.List)
            {    
                if (obj.Row.RowState == DataRowState.Added)
                {
                    break;
                }
                pos++;
            }
            if (BmSource.Position != pos)
                BmSource.Position = pos;
             //novoNaoSalvo = true;
             // BmSource.ResetBindings(false);
            novodelete.Enabled = false;
            cancele.Enabled = true;
            novo.Enabled = false;
            //if (firstControl != null) firstControl.Focus();



        }


        void NextItem_Click(object sender, EventArgs e)
        {
            BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;

            BindingSource_Mudou(BmSource);
            if (retorno == DialogResult.Ignore) return;
            if (retorno == DialogResult.Abort) return;

            PegAnteriores();
            if (retorno == DialogResult.No) return;

            if (oNavegador.BindingSource.Position != (oNavegador.BindingSource.Count - 1))
                oNavegador.BindingSource.Position += 1;
            else
            {
                /*if ((BindingSource_Mudou(BmSource) == DialogResult.OK) || (BindingSource_Mudou(BmSource) == DialogResult.None)
                    || (BindingSource_Mudou(BmSource) == DialogResult.Yes))*/
                if ((retorno == DialogResult.OK) || (retorno == DialogResult.None)
                    || (retorno == DialogResult.Yes))
                {
                    if (novo.Enabled == true)
                        novo.PerformClick();
                }
            }
        }

        void PrevItem_Click(object sender, EventArgs e)
        {
            BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;
            //Boolean nao_mova = falso;// pos = BmSource.CurrencyManager.Position;
            //nao_mova = (((DataRowView)BmSource.Current).Row.RowState == DataRowState.Added);
            //nao_mova = (nao_mova && (!salve.Enabled));

            DataRow orow;
            try
            {
                orow = ((DataRowView)BmSource.Current).Row;
            }
            catch (Exception)
            {
                return;
            }

            // DialogResult retorno = 
            BindingSource_Mudou(BmSource);
            if ((orow.RowState == DataRowState.Added))
            {
                if (retorno == DialogResult.Ignore)
                {
                    cancele.PerformClick();
                   /* BmSource.CancelEdit();
                    BmSource.RemoveCurrent();

                    if (BmSource.Position > 0)
                    {
                        BmSource.MovePrevious();
                    }
                    
                    if (BmSource.Position < 0) this.oFormEdite.Close();
                   */
                }

            } 
            else
            {
                if (retorno == DialogResult.Ignore) return;
                if (retorno == DialogResult.Abort) return;
                PegAnteriores();
                // if (nao_mova) return;
                if (BmSource.Position > 0)
                {
                    BmSource.MovePrevious();
                }
            }
        }

        private void Sair_Click(object sender, EventArgs e)
        {

        }

        void FirstItem_Click(object sender, EventArgs e)
        {
            BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;
            // DialogResult retorno = 
            BindingSource_Mudou(BmSource);
            if (retorno == DialogResult.Ignore) return;
            if (retorno == DialogResult.Abort) return;
            PegAnteriores();
            BmSource.Position = 0;
        }
        void LastItem_Click(object sender, EventArgs e)
        {
            BindingSource_Mudou(BmSource);
            PegAnteriores();
            BmSource.Position = BmSource.Count - 1;
        }

        private bool VerifiqueMudancaReal(DataRow orow)
        {
            bool result = false;
            if ((orow.RowState == DataRowState.Modified) || (orow.RowState == DataRowState.Unchanged))
            {
                foreach (DataColumn ocol in orow.Table.Columns)
                {
                    string corrente = orow[ocol, DataRowVersion.Current].ToString().Trim();
                    string original = orow[ocol, DataRowVersion.Original].ToString().Trim();
                    if ((corrente == "") && (original == ""))
                        continue;
                    if (corrente != original)                    {
                        result = true;
                        break;
                    }
                }

            }
            return result;
        }

        private void BindingSource_Mudou(BindingSource oBind)
        {

            retorno = DialogResult.None;
            DataRow orow = ((DataRowView)oBind.Current).Row;
            bool mudouRegistro = false;
            // isolei em setembro/2020 
            oBind.EndEdit();

            //   if (orow.RowState == DataRowState.Unchanged) return retorno;
            if ((orow.RowState == DataRowState.Modified) || (orow.RowState == DataRowState.Unchanged))
            {
                mudouRegistro = VerifiqueMudancaReal(orow);
                if (!mudouRegistro) return;
            }
            if (orow.RowState == DataRowState.Added) // absoleto?
            {

                if (salve.Enabled)
                {

                    //novoNaoSalvo = false;

                    if (OnBeforeAltera(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                    {
                        if (MessageBox.Show("Confirma Inclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            retorno = DialogResult.Yes;
                            //oBind.EndEdit();
                            try
                            {
                                EventHandler<AlteraRegistroEventArgs> handler = AlteraRegistrosOk;
                                if (handler != null)
                                {

                                    if (OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                                    {
                                        orow.AcceptChanges();
                                        novo.Enabled = true;
                                        novodelete.Enabled = true;
                                        cancele.Enabled = false;
                                        salve.Enabled = false;
                                        retorno = DialogResult.Yes;
                                        orow.Table.AcceptChanges();
                                    }
                                    else
                                    {
                                        retorno = DialogResult.No;
                                        cancele.Enabled = true;
                                        salve.Enabled = false;
                                    }
                                } 
                                /*else
                                {
                                    EventHandler<AsyncAlteraRegistroEventArgs> handlerAsync = AsyncAlteraRegistrosOk;
                                    if (handlerAsync != null)
                                    {
                                        bool resposta = await OnAsyncAlteraLinhas(new AsyncAlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));
                                        if (resposta)
                                        {
                                            orow.AcceptChanges();
                                            novo.Enabled = true;
                                            novodelete.Enabled = true;
                                            cancele.Enabled = false;
                                            salve.Enabled = false;
                                            retorno = DialogResult.Yes;
                                            orow.Table.AcceptChanges();
                                        }
                                        else
                                        {
                                            retorno = DialogResult.No;
                                            cancele.Enabled = true;
                                        }
                                    }

                                }*/
                            }
                            catch (Exception)
                            {
                                retorno = DialogResult.Abort;
                                oBind.CancelEdit();
                                novo.Enabled = true;
                                novodelete.Enabled = true;
                                cancele.Enabled = false;
                                salve.Enabled = false;

                                throw;
                            }
                        }
                        else
                        {
                            //   retorno = DialogResult.No;
                            // ((DataRowView)oBind.CurrencyManager.Current).Delete();
                            // int pos = oBind.CurrencyManager.Position;
                            // oBind.CurrencyManager.Refresh();
                            // oBind.CurrencyManager.Position = pos - 1;
                            retorno = DialogResult.No;
                            cancele.Enabled = true;
                            //novodelete.Enabled = true;
                            //novo.Enabled = true;
                        }
                    }
                    else
                    {
                        cancele.Enabled = true;
                        retorno = DialogResult.No;
                    }
                }
                else
                {
                    retorno = DialogResult.Ignore;//
                }
              
            }
            else if (orow.RowState == DataRowState.Detached)
            {
                //novoNaoSalvo = false;
                if (salve.Enabled)
                {
                    if (OnBeforeAltera(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                    {

                        if (MessageBox.Show("Confirma Inclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            retorno = DialogResult.Yes;
                            // oBind.EndEdit();
                            try
                            {
                                if (OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                                {
                                    orow.AcceptChanges();
                                    retorno = DialogResult.OK;
                                    orow.Table.AcceptChanges();
                                }
                                else
                                {
                                    oBind.RaiseListChangedEvents = false;
                                    orow.RejectChanges();
                                    oBind.RaiseListChangedEvents = true;
                                    retorno = DialogResult.Abort;
                                }
                            }
                            catch (Exception)
                            {
                                retorno = DialogResult.Abort;
                                throw;
                            }
                        }
                        else
                        {
                            retorno = DialogResult.No;
                            oBind.CancelEdit();
                            //  int pos = oBind.CurrencyManager.Position;
                        }
                    }
                    else
                    {
                        retorno = DialogResult.Abort;//
                        oBind.RaiseListChangedEvents = false;
                        oBind.CurrencyManager.CancelCurrentEdit();
                        int pos = oBind.CurrencyManager.Position;
                        oBind.CurrencyManager.Refresh();
                    }
                }
                else
                {
                    retorno = DialogResult.Abort;//
                    oBind.RaiseListChangedEvents = false;
                    oBind.CurrencyManager.CancelCurrentEdit();
                    int pos = oBind.CurrencyManager.Position;
                    oBind.CurrencyManager.Refresh();

                    oBind.RaiseListChangedEvents = true;
                }
                cancele.Enabled = false;
                salve.Enabled = false;
            }
            else
                if ((orow.RowState == DataRowState.Modified)  || mudouRegistro)        //(e.ListChangedType == System.ComponentModel.ListChangedType.ItemChanged)
                {
                //novoNaoSalvo = false;
                
                if (OnBeforeAltera(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                {

                    string texto = "Confirma Alteracão?";
                    if (MessageBox.Show(texto, "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            if (OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                            {
                                orow.AcceptChanges();
                                //  orow.Table.AcceptChanges();
                                novo.Enabled = true;
                                novodelete.Enabled = true;
                                cancele.Enabled = false;
                                salve.Enabled = false;
                                orow.Table.AcceptChanges();
                                retorno = DialogResult.Yes;
                            }
                            else
                            {
                                //oBind.CancelEdit();
                                //orow.RejectChanges();
                                retorno = DialogResult.No;
                                cancele.Enabled = true;
                                return;
                            }
                        }
                        catch
                        {
                            oBind.CancelEdit();
                            orow.RejectChanges();
                            cancele.Enabled = false;
                            salve.Enabled = false;
                            retorno = DialogResult.Abort;
                            novodelete.Enabled = true;
                        }
                    }
                    else
                    {
                        cancele.Enabled = true;
                        retorno = DialogResult.No;
                        // oBind.CancelEdit();
                        // orow.RejectChanges();
                        
                    }
                }
                else
                {
                    retorno = DialogResult.No;
                    cancele.Enabled = true;

                    return;
                }
                //cancele.Enabled = false;
                //salve.Enabled = false;

            }
            else
            if (orow.RowState == DataRowState.Deleted)//(e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
            {
                //novoNaoSalvo = false;
                if (OnBeforeDeleta(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                {

                    if (MessageBox.Show("Confirma Exclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            if (orow.RowState == DataRowState.Deleted)
                            {
                                try
                                {
                                    if (OnDeletaLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                                    {
                                        orow.AcceptChanges();
                                        retorno = DialogResult.Yes;
                                        orow.Table.AcceptChanges();
                                        OnAfterDeleta(new AlteraRegistroEventArgs(new DataRow[] { null }, DataRowState.Deleted));
                                    }
                                    else
                                    {
                                        //oBind.RaiseListChangedEvents = false;
                                        //orow.RejectChanges();
                                        //oBind.RaiseListChangedEvents = true;
                                        retorno = DialogResult.No;
                                        return;
                                    }
                                }
                                catch
                                {
                                    retorno = DialogResult.Abort;
                                    throw;
                                }
                            }


                        }
                        catch (Exception)
                        {
                            retorno = DialogResult.Abort;
                            throw;
                        }

                    }
                    else
                    {
                      //  oBind.RaiseListChangedEvents = false;
                       // orow.RejectChanges();
                       // oBind.RaiseListChangedEvents = true;
                        retorno = DialogResult.Abort;
                        return;
                    }
                }
                else
                {
                    //oBind.RaiseListChangedEvents = false;
                    //orow.RejectChanges();
                    //oBind.RaiseListChangedEvents = true;

                    retorno = DialogResult.Ignore;
                    return;
                }
            }
            if (retorno == DialogResult.Yes)
            {
                for (int i = 0; i < oBind.CurrencyManager.Bindings.Count; i++)
                {
                    Binding bindcontrol = oBind.CurrencyManager.Bindings[i];
                    if (bindcontrol.BindableComponent is TextBox)
                    {
                        string campo = bindcontrol.BindingMemberInfo.BindingField;
                        if ((orow.RowState == DataRowState.Modified) && (orow[campo, DataRowVersion.Current] == orow[campo, DataRowVersion.Original])) continue;

                        TextBox otext = (TextBox)bindcontrol.BindableComponent;
                        if (otext.AutoCompleteSource == AutoCompleteSource.CustomSource)
                        {
                            if (!otext.AutoCompleteCustomSource.Contains(otext.Text))
                                otext.AutoCompleteCustomSource.Add(otext.Text);
                        }

                    }
                }
            }

            return;
        }


        private void ArmeEdicao_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                if (UltimoAtivo(sender as Control))
                {
                    firstControl = PrimeiroControlePossiveldeFoco();
                    if (firstControl != null)
                    //firstControl = Linhas[0].oedite[0];
                    {
                        if (!firstControl.Focused)
                            firstControl.Focus();
                    }
                    if (IncluiAutomatico)
                    {
                        if (oFormEdite.oNavegador.Items.ContainsKey("Proximo"))
                            oFormEdite.oNavegador.Items["Proximo"].PerformClick();
                    }
                    else
                    {
                        if (oFormEdite.oNavegador.Items.ContainsKey("Salve"))
                            oFormEdite.oNavegador.Items["Salve"].PerformClick();
                    }
                }

            }
        }
        private Control PrimeiroControlePossiveldeFoco()
        {
            Control primeiroFocavel = null;
            for (int i = 0; i < Linhas.Count; i++)
            {
                List<Control> oEdites = Linhas[i].oedite;

                for (int j = 0; j < oEdites.Count; j++)
                {
                    if (oEdites[j] == null) break;
                    if (oEdites[j].CanFocus)
                    {
                        primeiroFocavel = oEdites[j];
                        break;
                    }
                   
                }
                if (primeiroFocavel != null) break;
            }
            return primeiroFocavel;
        }


        int PesqControl(Control oControlPai, int index)
        {
            int result = index;
            foreach (Control oControl in oControlPai.Controls)
            {
                if (oControl.Controls.Count > 0)
                {
                    result = PesqControl(oControl, result);
                    continue;
                }
                if (oControl.TabStop)
                    if (oControl.TabIndex > result)
                        result = oControl.TabIndex;
            }
            return result;
        }

        Boolean UltimoAtivo(Control curControl)
        {
            if (oFormEdite.Controls.Count == 0) return false;
            int oindex = -1;
            oindex = PesqControl(oFormEdite.Controls[0], oindex);

            if (oindex == curControl.TabIndex)
                return true;
            else
                return false;
        }


        // validador geral de todos os controles que estão sendo editados
        void ArmeEdicao_Validated(object sender, EventArgs e)
        {
            if (mudou == true)
            {
                mudou = false;
            }


            Control ocontrol = (Control)sender;

            if (ocontrol is UserControl)
            {
                if ((ocontrol.Name == "ComboBoxMD2") || (ocontrol.Name == "ComboBoxMD3") || (ocontrol.Name == "ComboMDInverte"))
                {
                    if (Convert.ToInt32(ocontrol.Tag) == 1)
                    {
                        salve.Enabled = true;
                        cancele.Enabled = true;
                        return;
                    }
                }
            }



            if (ocontrol.DataBindings == null) return;
            Binding bindcontrol;
            try
            {
                bindcontrol = ocontrol.DataBindings[0];
            }
            catch (Exception) { return; }

            BindingSource oBind = (BindingSource)bindcontrol.DataSource;
            if ((DataRowView)oBind.Current == null) return;
            DataRow orow = null;
            try
            {
                orow = ((DataRowView)oBind.Current).Row;
            }
            catch (Exception)
            {

                MessageBox.Show("Erro ao Validar Geral. Lin 1269 (EditaForm)");
                return;
            }
          
            string valor = null;
            if (ocontrol is DateTimePicker)
                valor = Convert.ToString(((DateTimePicker)ocontrol).Value);
            else
                if (ocontrol is ComboBox)
            {
                if (((ComboBox)ocontrol).SelectedValue != null)
                    valor = ((ComboBox)ocontrol).SelectedValue.ToString();
                else
                    valor = ((ComboBox)ocontrol).Text;
            }
            else
                valor = ocontrol.Text;

            //  if (valor == null)  return;
            if (valor == null)
                valor = "";
            string campo = bindcontrol.BindingMemberInfo.BindingField;

                        if ((orow.RowState == DataRowState.Detached) || (orow.RowState == DataRowState.Added))
            {
                //  string valorformatado = "";

                salve.Enabled = true;
                cancele.Enabled = true;
                return;
            }


           /* if (orow.RowState == DataRowState.Detached)
            {
                //  string valorformatado = "";
                if (ocontrol is NumericTextBox)
                {
                    if (((NumericTextBox)ocontrol).DecimalValue != Convert.ToDecimal(bindcontrol.NullValue))
                    {
                        salve.Enabled = true;
                        cancele.Enabled = true;
                    }
                }
                else
                    if ((!(Convert.IsDBNull(orow.Table.Columns[campo].DefaultValue) && (valor.Trim() == "")))
                && (valor.Trim() != Convert.ToString(orow.Table.Columns[campo].DefaultValue)))
                {
                    salve.Enabled = true;
                    cancele.Enabled = true;
                }
            }
            else
            if (orow.RowState == DataRowState.Added)
            {

                if (valor.Trim() != Convert.ToString(orow[campo, DataRowVersion.Default]).Trim())
                {
                    salve.Enabled = true;
                    cancele.Enabled = true;
                }
            }


            else */
            {
                if (ocontrol is NumericTextBox)
                {
                    if (((NumericTextBox)ocontrol).DecimalValue != Convert.ToDecimal(orow[campo, DataRowVersion.Original]))
                    {
                        salve.Enabled = true;
                        cancele.Enabled = true;
                    }
                }
                else
                    
                if (valor.Trim() != Convert.ToString(orow[campo, DataRowVersion.Original]).Trim())
                {
                    salve.Enabled = true;
                    cancele.Enabled = true;
                }
            }
        }
        
        /*void ArmeEdicao_Validated(object sender, EventArgs e)
        {
            Control ocontrol = (Control)sender;

            if (ocontrol.DataBindings == null) return;
            Binding bindcontrol;

            if (mudou == true)
            {
                mudou = false;
            }
            try
            {
                bindcontrol = ocontrol.DataBindings[0];
            }
            catch (Exception) { return; }

            BindingSource oBind = (BindingSource)bindcontrol.DataSource;
            if ((DataRowView)oBind.Current == null) return;
            string campo = bindcontrol.BindingMemberInfo.BindingField;

            DataRow orow = null;
            try
            {
                orow = ((DataRowView)oBind.Current).Row;
            }
            catch (Exception)
            {

                MessageBox.Show("Erro ao Validar Geral. Lin 1658 (EditaForm)");
                return;
            }
            if ((orow.RowState == DataRowState.Detached) || (orow.RowState == DataRowState.Added))
            {
                //  string valorformatado = "";

                salve.Enabled = true;
                cancele.Enabled = true;
                return;
            }
            if (orow.Table.Columns.Contains(campo))
            {
                DataColumn dc = orow.Table.Columns[campo];
                {
                    if (!(orow[dc, DataRowVersion.Original].Equals(
                          orow[dc, DataRowVersion.Current])))
                    {
                        salve.Enabled = true;
                        cancele.Enabled = true;
                    }

                }
            }
        }
        */


        // todos as columns alteradas
        private List<DataColumn> CampoFoiAlterado(DataRowView orow)
        {
            List<DataColumn> fields = new List<DataColumn>();
            if (orow.Row.RowState == DataRowState.Modified)
            {
                foreach (DataColumn dc in orow.Row.Table.Columns)
                {
                    if (!orow.Row[dc, DataRowVersion.Original].Equals(
                          orow.Row[dc, DataRowVersion.Current])) /* skipped Proposed as indicated by a commenter */
                    {
                        fields.Add(dc);
                    }
                }
            }
            return fields;
        }
        // todos as columns alteradas
        private Dictionary<string, string> valoresAlterados(DataRowView orow, string campo)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            if (orow.IsEdit)    //         (orow.Row.RowState == DataRowState.Modified)
            {
                DataColumn dc = orow.Row.Table.Columns[campo];

                if (!orow.Row[dc, DataRowVersion.Original].Equals(
                      orow.Row[dc, DataRowVersion.Current])) /* skipped Proposed as indicated by a commenter */
                {
                    string original = orow.Row[dc, DataRowVersion.Original].ToString();
                    string current = orow.Row[dc, DataRowVersion.Current].ToString();
                    fields.Add(original, current);
                }
            }
            return fields;
        }

        void BmSource_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            BindingSource oBindSource = (BindingSource)sender;
            if (oBindSource.IsBindingSuspended) return;
            DataRowView orow = null;
            try
            {
                orow = (DataRowView)oBindSource.Current;
            }
            catch (Exception)
            {

                return;
            }
            if (orow == null) return;
            //bool eNovo = false;
            //List<DataColumn> lstDataRow = CampoFoiAlterado(orow);
            Binding bind = e.Binding;
            string campo = bind.BindingMemberInfo.BindingField;
            object texto = orow[campo];
            
            DataRow dataRow = orow.Row;
           // eNovo = (dataRow.RowState == DataRowState.Added);
            ComboBox ocombo = null;
            if (bind.Control is ComboBox)
            {
                ocombo = (bind.Control as ComboBox);
            }

            // dados do datasource sendo atualizados nos controles
            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                
                /*if (ocombo != null)
                {
                    
                    if ((bind.PropertyName == "SelectedValue") && (!string.IsNullOrEmpty(texto.ToString())))
                    {
                        //  ((ComboBox)bind.Control).SelectedValue =((DataRowView)oBind.Current)[bind.BindingMemberInfo.BindingField];
                        if (ocombo.SelectedValue == null)
                        {
                           
                           
                        }
                    }
                }*/
                if (!(bind.Control is UserControl))
                {
                        bind.BindingManagerBase.EndCurrentEdit();
                }
            }
            // dados do controle sendo atualizados no datasource
            else if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                && e.Exception == null)
            {
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    object valor = null;
                    if (ocombo != null)
                    {
                        if (bind.PropertyName == "SelectedValue")
                        {
                            if (ocombo.SelectedValue == null)
                            {
                                if (ocombo.DisplayMember == ocombo.ValueMember)
                                    orow[campo] = ocombo.Text;
                                else
                                {
                                    
                                    /*if (!eNovo)
                                    {
                                        try
                                        {
                                            string campofiltro = "";
                                            string valorfiltro = "";
                                            BindingSource combobind = (ocombo.DataSource as BindingSource);
                                            string filtro = combobind.Filter;
                                            if (filtro != null)
                                            {
                                                int igual = filtro.IndexOf("=");
                                                if (igual > 0)
                                                {
                                                    campofiltro = filtro.Substring(0, igual).Trim();
                                                    valorfiltro = filtro.Substring(igual + 1).Trim();
                                                    Dictionary<string, string> keyValuePairs = valoresAlterados(orow, campofiltro);
                                                    string filtronovo = campofiltro + " = '" + orow[campofiltro].ToString() + "'";
                                                    combobind.Filter = filtronovo;
                                                    ocombo.SelectedValue = orow.Row[campo, DataRowVersion.Original].ToString();
                                                    if (ocombo.SelectedValue != null)
                                                        orow[campo] = orow.Row[campo, DataRowVersion.Original].ToString();
                                                }
                                            }
                                        }
                                        catch (Exception E)
                                        {
                                        }
                                    }
                                    */
                                }

                            }
                            else
                            {
                                orow[campo] = ocombo.SelectedValue;
                            }
                        }
                        valor = ocombo.SelectedValue;
                    }
                    else
                    {
                        if (bind.Control is NumericTextBox)
                        {
                            valor = ((NumericTextBox)bind.Control).DecimalValue;
                        }
                        else
                        {
                            if (!(bind.Control is UserControl))
                                valor = bind.Control.Text;
                        }
                    }
                   // orow[campo] = valor; 
                }
            }
        }
        public void PegAnteriores()
        {
            try
            {
                int z = 0;
                foreach (Linha linha in Linhas)
                {
                    foreach (Control item in linha.oedite)
                    {
                        if (item == null) { break; }
                        string texto = "";
                        if (item is TextBoxBase)
                        {
                            texto = item.Text;
                        }
                        else
                        if (item is DateTimePicker)
                        {
                            texto = (item as DateTimePicker).Text;
                        }
                        else
                        if (item is ComboBox)
                        {
                            texto = item.Text;
                        }
                        if (item is UserControl)
                        {
                            try
                            {
                                if (item is ComboBoxMD2)
                                {
                                    (item as ComboBoxMD2).PegAnterior();
                                    z++; continue;
                                }
                                
                                else if (item is ComboMDInverte2)
                                {
                                    (item as ComboMDInverte2).PegAnterior();
                                    z++; continue;

                                }
                                else if (item is ComboMDInverte3)
                                {
                                    (item as ComboMDInverte3).PegAnterior();
                                    z++; continue;

                                }
                            }
                            catch (Exception)
                            {
                            }
                            // PegDadosUserControl
                        }
                        if (texto == "") { z++; continue; }
                        if (TodosCampos.Count > z)
                        {
                            TGuardaAnt guardaAnt = TodosCampos[z];
                            if (guardaAnt.Anterior.Count == 0) { guardaAnt.Anterior.Add(texto); }
                            else
                            {
                                if (!guardaAnt.Anterior.Contains(texto)) { guardaAnt.Anterior.Insert(0, texto); }
                            }
                            if (guardaAnt.Anterior.Count > Apoio.ind) //8
                                guardaAnt.Anterior.RemoveAt(guardaAnt.Anterior.Count - 1);
                        }
                        z++;
                    }
                }
            }
            catch
            {

            }

        }

        /*       procedure TArmeEdicao.PegAnteriores;
               var
                 ttext: string;
         i, j, z: Integer;
       begin
         z := 0;
         for i := 0 to LinhasEdicao.Count - 1 do
         begin
           for j := 0 to LinhasEdicao.Maxindex do
           begin
             if (LinhasEdicao.OEdite[i, j] <> nil) and
               (TControl(LinhasEdicao.OEdite[i, j]) is TwinControl) then
             begin
               if (LinhasEdicao.OEdite[i, j] as TwinControl) is TDBEdit then
                 with LinhasEdicao.OEdite[i, j] as TDBEdit do
                   ttext := Text;
               if (LinhasEdicao.OEdite[i, j] as TwinControl) is TDBComboBox then
                 with LinhasEdicao.OEdite[i, j] as TDBComboBox do
                   ttext := Text;
               if (LinhasEdicao.OEdite[i, j] as TwinControl) is TDBListBox then
                 with LinhasEdicao.OEdite[i, j] as TDBListBox do
                   ttext := Text;
               if (LinhasEdicao.OEdite[i, j] as TwinControl) is TDBLookUpComboBox then
                 with LinhasEdicao.OEdite[i, j] as TDBLookUpComboBox do
                   ttext := Text;
               with TGuardaAnt(TodosCampos.Objects[z]).Anterior do
               begin
                 if Count = 0 then
                   Add(ttext)
                 else if comparestr(ttext, Strings[Count - 1]) <> 0 then
                 begin
                   Insert(0, ttext);
                   if Count > LinhasEdicao.Maxindex then
                     Delete(Count - 1);
               end;
               end;
               INC(z, 1);
               end;
           end;
         end;
       end;
        */
        public bool AtiveString(Control ativeControl)
        {
            bool result = false;
            TGuardaAnt cur_encontrado = new TGuardaAnt();
            foreach (TGuardaAnt cur in TodosCampos)
            {
                if (ativeControl == cur.curcontrol)
                {
                    cur_encontrado = cur;
                    result = true;
                    break;
                }
            }
            if (!result)
                return result;
            // achou o controle ativo

            // remove da lista dos anteriores os campos que não estão no combobox...
            /*if ((ativeControl is ComboBox) &&
               (cur_encontrado.Anterior.Count > 0) ) {
                int j = 0; 
                while (j < cur_encontrado.Anterior.Count)
                {
                   if ((ativeControl as ComboBox).Items.IndexOf(cur_encontrado.Anterior[j]) == -1)
                    {
                        cur_encontrado.Anterior.RemoveAt(j);
                    }
                   else { j++; }
                }
            }*/

            if (cur_encontrado.Anterior.Count == 0)
            {
                result = false;
                return result;

            }
            ListBoxDialog listBoxDialog = new ListBoxDialog();
            listBoxDialog.Lines.AddRange(cur_encontrado.Anterior);
            listBoxDialog.Title = cur_encontrado.cabecalho;
            if (listBoxDialog.Execute())
            {
                DataRowView orow = (DataRowView)BmSource.Current;
                if (ativeControl is TextBoxBase)
                {
                    if (!((orow.IsEdit) || (orow.IsNew)))
                    {
                        orow.BeginEdit();
                    }
                    ativeControl.Text = listBoxDialog.SelItem();
                }
                if (ativeControl is ComboBox)
                {
                    if (!((orow.IsEdit) || (orow.IsNew)))
                    {
                        orow.BeginEdit();
                    }
                    string texto = listBoxDialog.SelItem();
                    (ativeControl as ComboBox).Text = texto;
                    // (ativeControl as ComboBox).SelectedIndex = (ativeControl as ComboBox).Items.IndexOf(texto);
                }

            }


            return result;

        }
        /*
         * function TArmeEdicao.AtiveString(CurControl: TwinControl): boolean;
var
  ListBox: TListBoxDialog;
  i, j: Integer;
  // TStringValideCombo : TStrings;
begin

  for i := 0 to TodosCampos.Count - 1 do
    if CurControl = TGuardaAnt(TodosCampos.Objects[i]).CurControl then
      break;
  if not(i < TodosCampos.Count) then
    exit;

  result := false;
  ListBox := TListBoxDialog.Create(CurControl.Parent);

  if (CurControl is TCustomComboBox) and
    (TGuardaAnt(TodosCampos.Objects[i]).Anterior.Count > 0) then
  begin
    j := 0;
    while j < TGuardaAnt(TodosCampos.Objects[i]).Anterior.Count do
    begin
      if (CurControl as TCustomComboBox)
        .Items.IndexOf(TGuardaAnt(TodosCampos.Objects[i]).Anterior[j]) = -1 then
        TGuardaAnt(TodosCampos.Objects[i]).Anterior.Delete(j)
      else
        INC(j, 1);
    end;

  end;

  ListBox.SetLines(TGuardaAnt(TodosCampos.Objects[i]).Anterior);

  if ListBox.Execute then
  begin
    if CurControl is TDBEdit then
      with CurControl as TDBEdit do
      begin
        if (DataSource.DataSet.state <> dsEdit) or
          (DataSource.DataSet.state <> dsInsert) then
          DataSource.DataSet.Edit;
        Text := ListBox.SelItem;
        Field.value := Text;

      end;

    if CurControl is TDBComboBox then
      with CurControl as TDBComboBox do
      begin
        if (DataSource.DataSet.state <> dsEdit) or
          (DataSource.DataSet.state <> dsInsert) then
          DataSource.DataSet.Edit;
        SelText := ListBox.SelItem;
        // Field.value := text;
        // ItemIndex := Items.IndexOf(Text);
        refresh;
      end
    else if CurControl is TComboBox then
      with CurControl as TComboBox do
      begin
        Text := ListBox.SelItem;
        ItemIndex := Items.IndexOf(Text);
      end;
    if CurControl is TDBListBox then
      with CurControl as TDBListBox do
      begin
        if (DataSource.DataSet.state <> dsEdit) or
          (DataSource.DataSet.state <> dsInsert) then
          DataSource.DataSet.Edit;
        Text := ListBox.SelItem;
        // ItemIndex := Items.IndexOf(Text);
        Field.value := Text;
      end;

    result := true;
  end;
  ListBox.Free;

end;


         */

    }
    //Construir a classe de evento que o edite vai disparar
    public class AlteraRegistroEventArgs : EventArgs
    {
        public AlteraRegistroEventArgs(DataRow[] orows, DataRowState otipo)
        {
            rows = orows;
            tipomuda = otipo;
            cancela = false;
        }
        private DataRow[] rows;
        private DataRowState tipomuda;
        private Boolean cancela;

        public DataRowState TipoMuda
        {
            get { return tipomuda; }
            set { tipomuda = value; }

        }
        public DataRow[] Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        public Boolean Cancela
        {
            get { return cancela; }
            set { cancela = value; }
        }
    }


    public class AsyncAlteraRegistroEventArgs : EventArgs
    {
        public AsyncAlteraRegistroEventArgs(DataRow[] orows, DataRowState otipo)
        {
            rows = orows;
            tipomuda = otipo;
            cancela = Task.FromResult(false);
        }
        private DataRow[] rows;
        private DataRowState tipomuda;
        private Task<Boolean> cancela;

        public DataRowState TipoMuda
        {
            get { return tipomuda; }
            set { tipomuda = value; }

        }
        public DataRow[] Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        public Task<Boolean> Cancela
        {
            get { return cancela; }
            set { cancela = value; }
        }
    }



    public class NewAlteraRegistroEventArgs : EventArgs
    {
        public NewAlteraRegistroEventArgs(DataRow[] orows, DataRowState otipo)
        {
            rows = orows;
            tipomuda = otipo;
            cancela = false;
        }
        private DataRow[] rows;
        private DataRowState tipomuda;
        private Boolean cancela;

        public DataRowState TipoMuda
        {
            get { return tipomuda; }
            set { tipomuda = value; }

        }
        public DataRow[] Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        public Boolean Cancela
        {
            get { return cancela; }
            set { cancela = value; }
        }
    }

    public class InicioEdicaoEventArgs : EventArgs
    {
        public InicioEdicaoEventArgs(DataRowView orow)
        {
            row = orow;
            // bmSource = obmSource;
            cancela = false;
        }
        private DataRowView row;
        // private BindingSource bmSource;
        private Boolean cancela;

        public DataRowView current
        {
            get { return row; }
            //  set { tipomuda = value; }

        }
        /* public BindingSource bmSourceFonte
         {
             get { return bmSource; }
         }*/
        public Boolean Cancela
        {
            get { return cancela; }
            set { cancela = value; }
        }
    }

    
}

