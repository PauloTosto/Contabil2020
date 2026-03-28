using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ClassFiltroEdite
{

    public class ArmeEdicao : Control
    {
        public List<Linha> Linhas;
        private List<TGuardaAnt> TodosCampos;
        private Panel EPanel;
        private BindingSource BmSource;
        private FormDlgForm oFormEdite;
        private string camposelecionado; // se refere ao cmpo selecionado no brawse quando é acionada a EDIÇÃO
        private ToolStripButton cancele, salve, novodelete, novo;
        private bool IncluaPrimeiro;
        private Control firstControl;
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
                MonteEdicaoNovo();
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
                        objGroup.Left = oposicao.cableft - 6;
                        objGroup.Top = oposicao.cabtop;
                        objGroup.Width = oposicao.objtam + 4 + 4;
                        objGroup.Height = oposicao.objalt + oposicao.cabalt + 8;
                        objGroup.Text = Linhas[i].cabecalho[j];


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
                        //   if (oControl is DateTimePicker)
                        //      oControl.KeyDown += new KeyEventHandler(EPanel.FormFiltro_KeyDown);
                        //  else
                        //     oControl.KeyDown += new KeyEventHandler(EPanel.FormFiltro_KeyDown);
                        /*    TDBEdit,TDBComboBox, TDBListBox,TDBLookUpComboBox*/

                    }
                }
            }
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
        public void MonteEdicaoNovo() // antes ArmeEdicao
        {
            Font fontForm = oFormEdite.Font;
            Point ponto = new Point(Convert.ToInt16(fontForm.Size), Convert.ToInt16(fontForm.Height));
            List<TLinEdicao> olistpos = new List<TLinEdicao>();
            int maxtam = 0;

            int margem_left = 32;
            int margem_right = 32;
            int margem_top = 32;
            int margem_bottom = 32;
            // entre objetosedite no sentido do comprimento da linha
            int inter_min = (2 * ponto.X);
            int inter_min_alt = (2 * ponto.Y);

            for (int i = 0; i < Linhas.Count; i++)
            {
                TLinEdicao olinha = new TLinEdicao();
                olinha.oListLin = new List<TPosicaoLin>();
                olinha.maxtam = margem_left + margem_right;
                olinha.maxalt = 0;
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if (Linhas[i].oedite[j] == null) break;

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
                        oposicao.objalt = Linhas[i].oedite[j].Height + 4;// +inter_min_alt; // 2
                        oposicao.objtam = Linhas[i].oedite[j].Width + inter_min;

                        // if (((Control)Linhas[i].oedite[j]) is TextBoxBase)
                        //   oposicao.objalt = oposicao.objalt + 6;

                        if ((i == Linhas.Count - 1) && (((Control)Linhas[i].oedite[j]) is CheckedListBox)) // TDBEditCheckList o checklist do Historico
                            oposicao.adc_tam = Linhas[i].oedite[j].Width + inter_min;

                        if ((oposicao.objtam + oposicao.adc_tam) > oposicao.cabtam)
                            olinha.maxtam = olinha.maxtam + oposicao.objtam + oposicao.adc_tam;
                        else
                            olinha.maxtam = olinha.maxtam + oposicao.cabtam;

                        if ((oposicao.cabalt + oposicao.objalt + inter_min_alt) > olinha.maxalt)
                            olinha.maxalt = (oposicao.cabalt + oposicao.objalt + inter_min_alt);
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

            int altura = margem_top;

            int ttop = altura;
            int sobra = 0;
            //int novamargem_left = 
            for (int i = 0; i < olistpos.Count; i++)
            {
                TLinEdicao olinha = olistpos[i];
                //    int intervalo = 0;
                if (maxtam > olinha.maxtam)
                    sobra = (maxtam - olinha.maxtam);
                int num_objs = 0;
                for (int j = 0; j < olinha.oListLin.Count; j++)
                    if (olinha.oListLin[j] != null) num_objs += 1;//INC(Num_objs,1);

                if ((sobra != 0) && (num_objs > 2))
                    sobra = sobra / num_objs;
                if (sobra < inter_min) sobra = inter_min;

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
                            tleft = tleft + oposicao.objtam + sobra;
                        else
                            tleft = tleft + oposicao.cabtam + sobra;
                        olinha.oListLin[j] = oposicao;

                    }
                }
                ttop = ttop + olinha.maxalt;// +8;
                altura = altura + olinha.maxalt;// +8;
                olistpos[i] = olinha;
            };
            // a altura do painel
            altura = altura + inter_min_alt;
            int tamanho = maxtam + margem_left + margem_right;

            EPanel.Dock = DockStyle.Fill;
            EPanel.BorderStyle = BorderStyle.Fixed3D;
            EPanel.TabStop = false;
            EPanel.Visible = true;
            EPanel.Enabled = true;

            EPanel.Width = tamanho;
            EPanel.Height = altura;
            this.Width = tamanho + margem_right;
            this.Height = altura + margem_bottom;

            int ttaborder = 0;
            for (int i = 0; i < Linhas.Count; i++)
            {
                TLinEdicao olinha = olistpos[i];
                for (int j = 0; j < olinha.oListLin.Count; j++)
                {
                    if ((Linhas[i].oedite[j] != null) && (Linhas[i].oedite[j] is Control))
                    {
                        TPosicaoLin oposicao = olinha.oListLin[j];
                        GroupBox objGroup = new GroupBox(); // TGroupBox.Create(EPanel);
                        objGroup.Parent = EPanel;
                        objGroup.Visible = true;
                        objGroup.Enabled = true;
                        Font ofont = (Font)objGroup.Font.Clone();
                        //  objGroup.Font = new Font(ofont.FontFamily, ofont.Size - 2);
                        // objGroup.Font = ofont;
                        objGroup.TabStop = false;
                        objGroup.Left = oposicao.cableft - 8;
                        objGroup.Top = oposicao.cabtop;
                        if (oposicao.cabtam < oposicao.objtam)
                            objGroup.Width = oposicao.objtam + 4;
                        else
                            objGroup.Width = oposicao.cabtam + 4;
                        objGroup.Height = oposicao.objalt + oposicao.cabalt + 8;
                        objGroup.Text = Linhas[i].cabecalho[j].Trim();

                        //  objGroup.Padding = new Padding(1, 1, 5, 5);

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
                        //   if (oControl is DateTimePicker)
                        //      oControl.KeyDown += new KeyEventHandler(EPanel.FormFiltro_KeyDown);
                        //  else
                        //     oControl.KeyDown += new KeyEventHandler(EPanel.FormFiltro_KeyDown);
                        /*    TDBEdit,TDBComboBox, TDBListBox,TDBLookUpComboBox*/

                    }
                }
            }
        }
        // Comportamento durante Edicao       
        public event EventHandler<AlteraRegistroEventArgs> BeforeAlteraRegistros;
        public event EventHandler<AlteraRegistroEventArgs> AlteraRegistrosOk;
        public event EventHandler<AlteraRegistroEventArgs> DeletaRegistrosOk;
        public event EventHandler<InicioEdicaoEventArgs> OnPrimeiroRegistro;




        protected virtual bool OnAlteraLinhas(AlteraRegistroEventArgs e)
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
        protected virtual bool OnBeforeAltera(AlteraRegistroEventArgs e)
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
        protected virtual bool OnDeletaLinhas(AlteraRegistroEventArgs e)
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
        public BindingNavigator ConstruaNavegador()
        {
            BindingNavigator oNavegador = new BindingNavigator(false);

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
            novodelete.TextImageRelation = TextImageRelation.ImageAboveText;
            novodelete.Click += new EventHandler(DeleteItem_Click);


            // alteraçao do novo
            ToolStripButton onovo = (ToolStripButton)oNavegador.AddNewItem;
            oNavegador.Items.Remove(onovo);
            novo = new ToolStripButton();
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
            // oNavegador.ItemClicked += new ToolStripItemClickedEventHandler(oNavegador_ItemClicked);
            oNavegador.RefreshItems += new EventHandler(oNavegador_RefreshItems);
            return oNavegador;
            // Shortcut.CtrlN;
        }


        void oNavegador_RefreshItems(object sender, EventArgs e)
        {
            BindingNavigator oNavegador = (BindingNavigator)sender;
            int posicao = -1;
            if (oNavegador.BindingSource != null)
                posicao = oNavegador.BindingSource.Position;

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
            salve.Enabled = false;
            // mudou = false;
            if ((DataRowView)BmSource.Current == null) this.oFormEdite.Close();
            DataRow orow = ((DataRowView)BmSource.Current).Row;
            if ((orow.RowState == DataRowState.Added) || (orow.RowState == DataRowState.Detached)
               || (((DataRowView)BmSource.Current).IsEdit) || (((DataRowView)BmSource.Current).IsNew))
            {
                BindingSource_Mudou(BmSource);
                BmSource.CancelEdit();
                if (BmSource.Position < 0) this.oFormEdite.Close();
                novodelete.Enabled = true;
            }
            else
            {
                BmSource.CancelEdit();

            }

            // ((ToolStripButton)sender).Enabled = false;
            // salve.Enabled = false;
            mudou = false;
            novo.Enabled = true;
        }

        void salve_Click(object sender, EventArgs e)
        {
            BindingSource_Mudou(BmSource);

            ((ToolStripButton)sender).Enabled = false;
            cancele.Enabled = false;
           // if (novo.Enabled)
             //   novoNaoSalvo = false;
            // novo.Enabled = true;
        }

        void DeleteItem_Click(object sender, EventArgs e)
        {
            novo.Enabled = true;
            //novoNaoSalvo = false;
            DataRow orow = ((DataRowView)BmSource.Current).Row;
            if ((orow.RowState == DataRowState.Added) && (salve.Enabled == false))
            {
                BmSource.CancelEdit();
                BmSource.RemoveCurrent();
                BmSource.MovePrevious();
                if (BmSource.Position < 0) this.oFormEdite.Close();
            }
            else if (orow.RowState == DataRowState.Detached)
            {
                return;
                //BmSource.CancelEdit();
                // BmSource.RemoveCurrent();
                // BmSource.MovePrevious();
            }
            else
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

                            if (BmSource.CurrencyManager.Count < 1)
                                novo.PerformClick();
                            //BmSource.AddNew();

                        }
                    }
                    else
                    {
                        // deleta só virtualmente ago/2020
                        ((DataRowView)BmSource.Current).Delete();

                        if (BmSource.CurrencyManager.Count < 1)
                            novo.PerformClick();

                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /*private void Novo_Click(object sender, EventArgs e)
         {
             DialogResult retorno;
             if ((DataRowView)BmSource.Current != null)
             {
                 retorno = BindingSource_Mudou(BmSource);
                 if (retorno == DialogResult.Ignore) return;
                 if (retorno == DialogResult.Abort) return;
             }
             //  BmSource.CurrencyManager.Refresh();
             if ((Linhas.Count > 0) && (Linhas[0].oedite[0] != null))
             {
                 firstControl = Linhas[0].oedite[0];
                 if (!firstControl.Focused)
                 {
                     Linhas[0].oedite[0].Validated -= new EventHandler(ArmeEdicao_Validated);
                     firstControl.Focus();
                     Linhas[0].oedite[0].Validated += new EventHandler(ArmeEdicao_Validated);
                 }
             }
             //   if (oBindSource.IsBindingSuspended) return;
             // BmSource.SuspendBinding();
             BmSource.AddingNew += BmSource_AddingNew;
             BmSource.AddNew();

             novoNaoSalvo = true;
             // BmSource.ResetBindings(false);
             novodelete.Enabled = false;
             cancele.Enabled = true;
             novo.Enabled = false;
             //if (firstControl != null) firstControl.Focus();

         }

         private void BmSource_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e)
         {

         }
         */
        void AddNewItem_Click(object sender, EventArgs e)
        {
            // BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;
            DialogResult retorno;
            if ((DataRowView)BmSource.Current != null)
            {
                retorno = BindingSource_Mudou(BmSource);
                if (retorno == DialogResult.Ignore) return;
                if (retorno == DialogResult.Abort) return;
            }
            //  BmSource.CurrencyManager.Refresh();
            if ((Linhas.Count > 0) && (Linhas[0].oedite[0] != null))
            {
                firstControl = Linhas[0].oedite[0];
                if (!firstControl.Focused)
                {
                    Linhas[0].oedite[0].Validated -= new EventHandler(ArmeEdicao_Validated);
                    firstControl.Focus();
                    Linhas[0].oedite[0].Validated += new EventHandler(ArmeEdicao_Validated);
                }
            }
            //   if (oBindSource.IsBindingSuspended) return;
            // BmSource.SuspendBinding();
            BmSource.AddNew();

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

            DialogResult retorno = BindingSource_Mudou(BmSource);
            if (retorno == DialogResult.Ignore) return;
            if (retorno == DialogResult.Abort) return;

            PegAnteriores();

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
            DialogResult retorno = BindingSource_Mudou(BmSource);
            if (retorno == DialogResult.Ignore) return;
            if (retorno == DialogResult.Abort) return;
            PegAnteriores();

            // if (nao_mova) return;
            if (BmSource.Position > 0)
            {
                BmSource.MovePrevious();
            }
        }
        void FirstItem_Click(object sender, EventArgs e)
        {
            BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;
            DialogResult retorno = BindingSource_Mudou(BmSource);
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
            if (orow.RowState == DataRowState.Modified)
            {
                foreach (DataColumn ocol in orow.Table.Columns)
                {
                    if ((orow[ocol, DataRowVersion.Current].ToString().Trim() == "") && (orow[ocol, DataRowVersion.Original].ToString().Trim() == ""))
                        continue;
                    if (orow[ocol, DataRowVersion.Current].ToString() != orow[ocol, DataRowVersion.Original].ToString())
                    {
                        result = true;
                        break;
                    }
                }

            }
            return result;
        }

        private DialogResult BindingSource_Mudou(BindingSource oBind)
        {

            DialogResult retorno = DialogResult.None;
            DataRow orow = ((DataRowView)oBind.Current).Row;
            // isolei em setembro/2020 
            // oBind.EndEdit();

            if (orow.RowState == DataRowState.Unchanged) return retorno;
            if (orow.RowState == DataRowState.Modified)
                if (!VerifiqueMudancaReal(orow)) return retorno;
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
                                if (OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                                {
                                    orow.AcceptChanges();
                                    // orow.Table.AcceptChanges();
                                    novo.Enabled = true;
                                    novodelete.Enabled = true;
                                    //orow.Table.AcceptChanges();
                                }
                                else
                                {
                                    ((DataRowView)oBind.CurrencyManager.Current).Delete();
                                    int pos = oBind.CurrencyManager.Position;
                                    oBind.CurrencyManager.Refresh();
                                    oBind.CurrencyManager.Position = pos - 1;
                                    retorno = DialogResult.Abort;
                                    novo.Enabled = true;
                                    novodelete.Enabled = true;
                                }
                            }
                            catch (Exception)
                            {
                                retorno = DialogResult.Abort;
                                oBind.CancelEdit();
                                novo.Enabled = true;
                                novodelete.Enabled = true;
                                throw;
                            }
                        }
                        else
                        {
                            retorno = DialogResult.No;
                            ((DataRowView)oBind.CurrencyManager.Current).Delete();
                            int pos = oBind.CurrencyManager.Position;
                            oBind.CurrencyManager.Refresh();
                            oBind.CurrencyManager.Position = pos - 1;
                            retorno = DialogResult.Abort;
                            novodelete.Enabled = true;
                            novo.Enabled = true;


                        }
                    }
                    else
                    {
                        retorno = DialogResult.Ignore;//
                        oBind.RaiseListChangedEvents = false;
                        oBind.CurrencyManager.CancelCurrentEdit();
                        ((DataRowView)oBind.CurrencyManager.Current).Delete();
                        int pos = oBind.CurrencyManager.Position;
                        oBind.CurrencyManager.Refresh();
                        oBind.CurrencyManager.Position = pos - 1;
                        oBind.RaiseListChangedEvents = true;
                        oBind.CancelEdit();
                        novodelete.Enabled = true;
                        novo.Enabled = true;

                        retorno = DialogResult.Abort;
                    }
                }
                else
                {
                    retorno = DialogResult.Ignore;//
                    oBind.RaiseListChangedEvents = false;
                    oBind.CurrencyManager.CancelCurrentEdit();
                    ((DataRowView)oBind.CurrencyManager.Current).Delete();
                    int pos = oBind.CurrencyManager.Position;
                    oBind.CurrencyManager.Refresh();
                    oBind.CurrencyManager.Position = pos - 1;
                    oBind.RaiseListChangedEvents = true;
                    retorno = DialogResult.Abort;
                    novodelete.Enabled = true;
                    novo.Enabled = true;
                }
                cancele.Enabled = false;
                salve.Enabled = false;

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
                if (orow.RowState == DataRowState.Modified)//(e.ListChangedType == System.ComponentModel.ListChangedType.ItemChanged)
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
                                retorno = DialogResult.Yes;
                            }
                            else
                            {
                                oBind.CancelEdit();
                                orow.RejectChanges();
                                cancele.Enabled = false;
                                salve.Enabled = false;
                                retorno = DialogResult.No;
                                return retorno;
                            }
                        }
                        catch
                        {
                            oBind.CancelEdit();
                            orow.RejectChanges();
                            cancele.Enabled = false;
                            salve.Enabled = false;
                            retorno = DialogResult.Abort;
                            // throw;
                        }
                    }
                    else
                    {
                        oBind.CancelEdit();
                        orow.RejectChanges();
                        retorno = DialogResult.No;
                    }
                }
                else
                {
                    oBind.CancelEdit();
                    //oBind.RaiseListChangedEvents = false;
                    orow.RejectChanges();
                    //oBind.RaiseListChangedEvents = true;
                    cancele.Enabled = false;
                    salve.Enabled = false;

                    retorno = DialogResult.Abort;
                    return retorno;
                }
                cancele.Enabled = false;
                salve.Enabled = false;

            }
            else
                    if (orow.RowState == DataRowState.Deleted)//(e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
            {
                //novoNaoSalvo = false;
                if (OnBeforeAltera(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                {

                    if (MessageBox.Show("Confirma Exclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            if (orow.RowState == DataRowState.Deleted)
                            {
                                try
                                {
                                    if (OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                                    {
                                        orow.AcceptChanges();
                                        retorno = DialogResult.Yes;
                                    }
                                    else
                                    {
                                        oBind.RaiseListChangedEvents = false;
                                        orow.RejectChanges();
                                        oBind.RaiseListChangedEvents = true;
                                        return DialogResult.No;
                                    }
                                }
                                catch
                                {
                                    return DialogResult.Abort;
                                    throw;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return DialogResult.Abort;
                            throw;
                        }

                    }
                    else
                    {
                        oBind.RaiseListChangedEvents = false;
                        orow.RejectChanges();
                        oBind.RaiseListChangedEvents = true;
                        return DialogResult.Abort;
                    }
                }
                else
                {
                    oBind.RaiseListChangedEvents = false;
                    orow.RejectChanges();
                    oBind.RaiseListChangedEvents = true;

                    return DialogResult.Abort;
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

            return retorno;
        }


        public Boolean Edite(Form oformpai)
        {
            return Edite(oformpai, "");
        }

        public Boolean Edite(Form oformpai, string field_selecionado, bool inclua = false)
        {
            Point pontoini = new Point(250, 250);
            oFormEdite.tamanho = Width;
            oFormEdite.altura = Height;
            IncluaPrimeiro = inclua;
            EPanel.Parent = oFormEdite;
            oFormEdite.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            oFormEdite.AutoScroll = false;
            oFormEdite.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;  // Nones
            oFormEdite.MinimizeBox = false;
            // oFormEdite.MouseDown += new MouseEventHandler(FormEdite_MouseDown);
            // oFormEdite.MouseUp += new MouseEventHandler(FormEdite_MouseUp);
            // oFormEdite.MouseMove += new MouseEventHandler(Formedite_MouseMove);
            oFormEdite.MaximizeBox = false;
            camposelecionado = field_selecionado;
            //Receber Notificação quando um control acabar de ser validado
            mudou = false; //acuso no binding a alteração do controle. É completado no validated
                           // BmSource.BindingComplete -= new BindingCompleteEventHandler(BmSource_BindingComplete);

            EventHandler<InicioEdicaoEventArgs> handler = OnPrimeiroRegistro;
            if (handler != null)
            {
                OnPrimeiroRegistro(this, new InicioEdicaoEventArgs((BmSource.Current as DataRowView)));
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


            if (oFormEdite.oNavegador == null)
            {
                oFormEdite.oNavegador = ConstruaNavegador();
                oFormEdite.oNavegador.Dock = DockStyle.Bottom;
                oFormEdite.oNavegador.Parent = EPanel;
            }
            oFormEdite.oNavegador.BindingSource = BmSource;

            EPanel.Visible = true;
            EPanel.Enabled = true;
            EPanel.Refresh();

            oFormEdite.StartPosition = FormStartPosition.Manual;
            oFormEdite.SetDesktopLocation(pontoini.X, pontoini.Y);
            Boolean result = false;

            //oFormEdite.Closing -= new System.ComponentModel.CancelEventHandler(oFormEdite_Closing); 
            oFormEdite.Closing += new System.ComponentModel.CancelEventHandler(oFormEdite_Closing);
            oFormEdite.FormClosing += OFormEdite_FormClosing;
            //    oFormEdite.Activated -= new EventHandler(oFormEdite_Activated);
            oFormEdite.Activated += new EventHandler(oFormEdite_Activated);
            if ((Linhas.Count > 0) && (Linhas[0].oedite[0] != null))
                firstControl = Linhas[0].oedite[0];
            if (oFormEdite.ShowDialog() == DialogResult.OK)
                result = true;
            else
                result = false;

            oFormEdite.oNavegador.Dispose();
            oFormEdite.oNavegador = null;
            BmSource.BindingComplete -= new BindingCompleteEventHandler(BmSource_BindingComplete);

            oFormEdite.Closing -= new System.ComponentModel.CancelEventHandler(oFormEdite_Closing);
            oFormEdite.FormClosing -= OFormEdite_FormClosing;

            oFormEdite.Activated -= new EventHandler(oFormEdite_Activated);

            try
            {
                for (int i = 0; i < Linhas.Count; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count; j++)
                    {
                        if (oEdites[j] == null) break;
                        oEdites[j].Validated -= new EventHandler(ArmeEdicao_Validated);
                        oEdites[j].KeyDown -= ArmeEdicao_KeyDown;
                    }
                }
            }
            catch (Exception)
            { }

            return result;
        }

        private void ArmeEdicao_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (UltimoAtivo(sender as Control))
                {
                    firstControl = Linhas[0].oedite[0];
                    if (!firstControl.Focused)
                        firstControl.Focus();
                    if (oFormEdite.oNavegador.Items.ContainsKey("Proximo"))
                        oFormEdite.oNavegador.Items["Proximo"].PerformClick();

                }
            }
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


            if (orow.RowState == DataRowState.Detached)
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
                /*try
                {   // se for original 
                    string campoq = Convert.ToString(orow[campo, DataRowVersion.Original]);
                }
                catch
                {
                    salve.Enabled = true;
                    cancele.Enabled = true;
                }*/

                if (valor.Trim() != Convert.ToString(orow[campo, DataRowVersion.Default]).Trim())
                {
                    salve.Enabled = true;
                    cancele.Enabled = true;
                }
            }


            else
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



        void oFormEdite_Activated(object sender, EventArgs e)
        {

            if (IncluaPrimeiro)
            {
                IncluaPrimeiro = false;
                novo.PerformClick();

            }
            else
            if (BmSource.CurrencyManager.Count < 1)
            {
                novo.PerformClick();
            }

            return; // desativei em 15 de setembro esta funÇão de se posicionar num campo selecionado
            // a pedito de lisnmar
            if (camposelecionado == "") return;

            Form oform = (Form)sender;
            if ((oform.ActiveControl.DataBindings.Count > 0) &&
                 (oform.ActiveControl.DataBindings[0].BindingMemberInfo.BindingField == camposelecionado))
                return;

            Panel oPanel = EPanel;
            {
                for (int i = 0; i < oPanel.Controls.Count; i++)
                {
                    if ((((Control)oPanel.Controls[i]).DataBindings.Count > 0) &&
                         (((Control)oPanel.Controls[i]).DataBindings[0].BindingMemberInfo.BindingField == camposelecionado))
                        if (((Control)oPanel.Controls[i]).CanFocus)
                            ((Control)oPanel.Controls[i]).Focus();
                }
            }
        }

        private void OFormEdite_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (((DataRowView)BmSource.Current) == null) return;
            DataRowView orow = (DataRowView)BmSource.Current;
            if (orow.Row == null) return;
            if (salve.Enabled)
            {
                e.Cancel = true;
                salve.PerformClick();

            }
            else
            {
                if (orow.IsEdit || orow.IsNew || (orow.Row.RowState == DataRowState.Detached) || (orow.Row.RowState == DataRowState.Added)) // não permte que algum registre que esteja editando seja retornado com um valor não checado ainda
                {
                    BindingSource_Mudou(BmSource);
                    BmSource.CancelEdit();
                    novodelete.Enabled = true;
                    novodelete.Enabled = true;
                    novo.Enabled = true;
                    //novoNaoSalvo = false;
                }
            }
        }


        void oFormEdite_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            /*   if (((DataRowView)BmSource.Current) == null) return;
               DataRowView orow = (DataRowView)BmSource.Current;
               if (orow.Row == null) return;
               if (salve.Enabled)
               {
                   e.Cancel = true;
                   salve.PerformClick();

               }
               else
               {
                   if (orow.IsEdit || orow.IsNew) // não permte que algum registre que esteja editando seja retornado com um valor não checado ainda
                   {
                       orow.CancelEdit();
                       // e.Cancel = true;
                      // BmSource.RemoveCurrent();
                   }
                   else if ((orow.Row.RowState == DataRowState.Detached) || (orow.Row.RowState == DataRowState.Added))  
                   {
                       orow.CancelEdit();
                      // BmSource.RemoveCurrent();
                   }
                   // este if é para assegurar que o registro novo não editado será destruido na saida
                   else if ((novodelete.Enabled == false) && (novo.Enabled == false) && (orow.Row.RowState == DataRowState.Unchanged) )
                   {
                       orow.CancelEdit();
                      // BmSource.RemoveCurrent();
                   }
                  // BmSource.MovePrevious();
               }
               */
        }
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
            bool eNovo = false;
            //List<DataColumn> lstDataRow = CampoFoiAlterado(orow);
            Binding bind = e.Binding;
            string campo = bind.BindingMemberInfo.BindingField;
            object texto = orow[campo];
            DataRow dataRow = orow.Row;
            eNovo = (dataRow.RowState == DataRowState.Added);
            ComboBox ocombo = null;
            if (bind.Control is ComboBox)
            {
                ocombo = (bind.Control as ComboBox);
            }

            // dados do datasource sendo atualizados nos controles
            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                /*if ((oBind.Position == (oBind.Count - 1)) &&
                   ((e.Binding.Control is ComboBox)))
                {
                    string campo = e.Binding.BindingMemberInfo.BindingField;

                    DataRow orow = ((DataRowView)oBind.Current).Row;
                    string valorcampo = Convert.ToString(orow[campo, DataRowVersion.Current]);
                    ComboBox ocombo = (ComboBox)e.Binding.BindableComponent;
                    ocombo.SelectAll();
                    ocombo.SelectedText = valorcampo;
                    e.Binding.BindingManagerBase.EndCurrentEdit();
                  
                }
                else*/
                if (ocombo != null)
                {
                    if ((bind.PropertyName == "SelectedValue") && (!string.IsNullOrEmpty(texto.ToString())))
                    {
                        //  ((ComboBox)bind.Control).SelectedValue =((DataRowView)oBind.Current)[bind.BindingMemberInfo.BindingField];
                        if ((ocombo.SelectedValue == null) && (ocombo.DisplayMember == ocombo.ValueMember))
                        {
                            if (ocombo.Text.Trim() != texto.ToString().Trim())
                                ocombo.Text = texto.ToString();
                        }
                    }
                }
                //    if (!eNovo)
                bind.BindingManagerBase.EndCurrentEdit();
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
                                    if (!eNovo)
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

                                }

                            }

                        }
                        valor = ocombo.SelectedValue;
                    }
                    else
                    {
                        valor = bind.Control.Text;
                    }
                    //   orow[campo] = valor; 
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

