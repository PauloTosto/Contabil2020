using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
namespace ApoioContabilidade
{
   
    public class ArmeEdicao : Control
    {
        public List<Linha> Linhas;
        private List<TGuardaAnt> TodosCampos;
        private Panel EPanel;
        private BindingSource BmSource;
        private FormDlgForm oFormEdite;
        private string camposelecionado;
        private ToolStripButton cancele,salve;
        //int Top_Pad, Left_Pad;
        //Boolean SempreVisivel;
        Boolean mudou;
       // evento de gravação do Edite
        public event EventHandler<AlteraRegistroEventArgs> AlteraRegistrosOk;
       
        protected virtual void OnAlteraLinhas(AlteraRegistroEventArgs e)
        {
            EventHandler<AlteraRegistroEventArgs> handler = AlteraRegistrosOk;
            if (handler != null)
            {
                handler(this, e);
            }
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
            ToolStripButton novodelete = new ToolStripButton();
            novodelete.Font = odelete.Font;
            novodelete.Image = odelete.Image;
            novodelete.Size = odelete.Size;
            novodelete.Click += new EventHandler(DeleteItem_Click);
          

            // alteraçao do novo
            ToolStripButton onovo = (ToolStripButton)oNavegador.AddNewItem;
            oNavegador.Items.Remove(onovo);
            ToolStripButton novo = new ToolStripButton();
            novo.Font = onovo.Font;
            novo.Image = onovo.Image;
            novo.Size = onovo.Size;
            novo.Click += new EventHandler(AddNewItem_Click);
            

            oNavegador.Items.Insert(0, first);
            oNavegador.Items.Insert(1, prev);

            oNavegador.Items.Insert(3, next);

            oNavegador.Items.Insert(4, last);
            
            oNavegador.Items.Add(novodelete);
            oNavegador.Items.Add(novo);

            cancele = new ToolStripButton();
            cancele.Name = "Cancele";
            cancele.Font = onovo.Font;
            cancele.Text = "&CANCELE";
            
            cancele.Size = onovo.Size;
            cancele.Enabled = false;
            cancele.Click += new EventHandler(cancele_Click);
            oNavegador.Items.Add(cancele);
           // oNavegador.Items[0].O

            salve = new ToolStripButton();
            salve.Name = "Salve";
            salve.Font = onovo.Font;
            salve.Text = "&SALVE";
            salve.Size = onovo.Size;
            salve.Enabled = false;
            salve.Click += new EventHandler(salve_Click);
            oNavegador.Items.Add(salve);
           // oNavegador.ItemClicked += new ToolStripItemClickedEventHandler(oNavegador_ItemClicked);
            oNavegador.RefreshItems += new EventHandler(oNavegador_RefreshItems); 
            return oNavegador;
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
                    oNavegador.Items["Primeiro"].Enabled =false;
                else 
                    if (oNavegador.Items["Primeiro"].Enabled ==false)
                        oNavegador.Items["Primeiro"].Enabled =true;
            if (oNavegador.Items.ContainsKey("Anterior"))
                if (posicao < 1)
                    oNavegador.Items["Anterior"].Enabled = false;
                else
                    if (oNavegador.Items["Anterior"].Enabled == false)
                        oNavegador.Items["Anterior"].Enabled = true;
            if (oNavegador.Items.ContainsKey("Ultimo"))
                if (posicao == (count-1))
                    oNavegador.Items["Ultimo"].Enabled = false;
                else
                    if (oNavegador.Items["Ultimo"].Enabled == false)
                        oNavegador.Items["Ultimo"].Enabled = true;
            if (oNavegador.Items.ContainsKey("Proximo"))
                if (posicao == (count-1))
                    oNavegador.Items["Proximo"].Enabled = false;
                else
                    if (oNavegador.Items["Proximo"].Enabled == false)
                        oNavegador.Items["Proximo"].Enabled = true;
            
        }

        void cancele_Click(object sender, EventArgs e)
        {
            BmSource.CancelEdit();
            ((ToolStripButton)sender).Enabled = false;
            salve.Enabled = false;
            mudou = false;
        }

        void salve_Click(object sender, EventArgs e)
        {
            BindingSource_Mudou(BmSource);

            ((ToolStripButton)sender).Enabled = false;
            cancele.Enabled = false;
        }

        void DeleteItem_Click(object sender, EventArgs e)
        {
            DataRow orow = ((DataRowView)BmSource.Current).Row;
            if ((orow.RowState == DataRowState.Added) && (salve.Enabled == false))
            {
                BmSource.CancelEdit();
                BmSource.RemoveCurrent();
                BmSource.MovePrevious();
            }
            else
                if (MessageBox.Show("Confirma Exclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ((DataRowView)BmSource.Current).Delete();
                    try
                    {
                        OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));
                    }
                    catch (Exception)
                    {
                        throw; 
                    }
                }

        }
     
        
        void AddNewItem_Click(object sender, EventArgs e)
        {
            BmSource.AddNew();
        }

        
        void NextItem_Click(object sender, EventArgs e)
        {
          BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;
            BindingSource_Mudou(BmSource);
            if (oNavegador.BindingSource.Position != (oNavegador.BindingSource.Count - 1))
               oNavegador.BindingSource.Position += 1;
        }

        
        void PrevItem_Click(object sender, EventArgs e)
        {
            BindingNavigator oNavegador = (BindingNavigator)((ToolStripItem)sender).Owner;
            DialogResult retorno = BindingSource_Mudou(BmSource);
            if (retorno == DialogResult.Ignore) return; 
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
            BmSource.Position = 0;
        }
        void LastItem_Click(object sender, EventArgs e)
        {
            BindingSource_Mudou(BmSource);
            BmSource.Position = BmSource.Count - 1;
        }


        private DialogResult BindingSource_Mudou(BindingSource oBind)
        {

            DialogResult retorno = DialogResult.None;
            DataRow orow = ((DataRowView)oBind.Current).Row;
            oBind.EndEdit();
            if (orow.RowState == DataRowState.Unchanged) return retorno;
            
           /* if (orow.RowState == DataRowState.Detached)
            {
                if (MessageBox.Show("Confirma Inclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    retorno = DialogResult.Yes;
                    oBind.EndEdit();
                    try
                    {
                        OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));
                        orow.AcceptChanges();
                    }
                    catch
                    {
                        oBind.CancelEdit();
                        //oBind.RaiseListChangedEvents = false;
                        //oBind.RemoveCurrent();
                        //oBind.MovePrevious();
                        //oBind.RaiseListChangedEvents = true;
                        throw;
                    }
                }
                else
                {
                    retorno = DialogResult.No;
                    oBind.CancelEdit();
                    //oBind.RaiseListChangedEvents = false;
                    //oBind.RemoveCurrent();
                    //oBind.MovePrevious();
                    //oBind.RaiseListChangedEvents = true;
                }

                cancele.Enabled = false;
                salve.Enabled = false;
                // return retorno; 
            }
            else
                oBind.EndEdit();
            */
            if (orow.RowState == DataRowState.Added) // absoleto?
            {

                /*Boolean ok = false;
                for (int i = 0; i < oBind.CurrencyManager.Bindings.Count; i++)
                {
                    Binding bindcontrol = oBind.CurrencyManager.Bindings[i];
                    string campo = bindcontrol.BindingMemberInfo.BindingField;

                    if (Convert.ToString(orow[campo, DataRowVersion.Current]) != Convert.ToString(orow[campo, DataRowVersion.Default]))
                    {
                        ok = true;
                        break;
                    }
                }*/
                if (salve.Enabled)
                {
                    if (MessageBox.Show("Confirma Inclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        retorno = DialogResult.Yes;
                        oBind.EndEdit();
                        try
                        {
                            OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));
                            orow.AcceptChanges();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        retorno = DialogResult.No;
                        ((DataRowView)oBind.CurrencyManager.Current).Delete();
                        int pos = oBind.CurrencyManager.Position;
                        oBind.CurrencyManager.Refresh();
                        // ((DataRowView)oBind.Current).Delete();
                        oBind.CurrencyManager.Position = pos - 1;
                  
                       /* oBind.CancelEdit();
                        oBind.RaiseListChangedEvents = false;
                        oBind.RemoveCurrent();
                        oBind.MovePrevious();
                        oBind.RaiseListChangedEvents = true;*/
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
                   // ((DataRowView)oBind.Current).Delete();
                    oBind.CurrencyManager.Position = pos - 1;
                    //oBind.MovePrevious();
                   // oBind.ResetCurrentItem();
                    oBind.RaiseListChangedEvents = true;
                }


                cancele.Enabled = false;
                salve.Enabled = false;

            }
            else
                if (orow.RowState == DataRowState.Modified)//(e.ListChangedType == System.ComponentModel.ListChangedType.ItemChanged)
                {
                    //DataRow orow = ((DataRowView)oBind.Current).Row;
                    string texto = "Confirma Alteracão?";
                    // if (orow.RowState == DataRowState.Added)
                    // {
                    //    texto = "Confirma Inclusão?";
                    // }
                    if (MessageBox.Show(texto, "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        oBind.EndEdit();
                        try
                        {
                            OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));
                            orow.AcceptChanges();
                            retorno = DialogResult.Yes;
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    else
                    {
                        oBind.CancelEdit();
                        retorno = DialogResult.No;
                    }
                    cancele.Enabled = false;
                    salve.Enabled = false;

                }
                else
                    if (orow.RowState == DataRowState.Deleted)//(e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
                    {
                        if (MessageBox.Show("Confirma Exclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                //DataRow orow = ((DataRowView)oBind.Current).Row;

                                if (orow.RowState == DataRowState.Deleted)
                                {
                                    try
                                    {
                                        OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState));
                                        orow.AcceptChanges();
                                        retorno = DialogResult.Yes;
                                    }
                                    catch
                                    {
                                        throw;
                                    }
                                }
                                return DialogResult.Yes;
                            }
                            catch (Exception)
                            {
                                throw;
                            }

                        }
                        else
                        {
                            if (orow.RowState == DataRowState.Deleted)
                            {
                                oBind.RaiseListChangedEvents = false;
                                orow.RejectChanges();
                                oBind.RaiseListChangedEvents = true;
                            }
                            return DialogResult.No;
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

        public ArmeEdicao()
        {
            Linhas = new List<Linha>();
        }
        public ArmeEdicao(BindingSource oBmSource)
        {
            oFormEdite = new FormDlgForm();
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
            //Top_Pad = -1;
            //Left_Pad = -1;
            //SempreVisivel = false;
        }


        public Boolean Edite(Form oformpai)
        {
            return Edite(oformpai, "");
        }

        public Boolean Edite(Form oformpai, string field_selecionado)
        {
            Point pontoini = new Point(250, 250);
            oFormEdite.tamanho = Width;
            oFormEdite.altura = Height;

            EPanel.Parent = oFormEdite;
            oFormEdite.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            oFormEdite.AutoScroll = false;
            oFormEdite.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            oFormEdite.MaximizeBox = false;
            camposelecionado = field_selecionado;
            //Receber Notificação quando um control acabar de ser validado
            mudou = false; //acuso no binding a alteração do controle. É completado no validated
            BmSource.BindingComplete += new BindingCompleteEventHandler(BmSource_BindingComplete);
            try
            {
                for (int i = 0; i < Linhas.Count-1; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count - 1; j++)
                    {
                        if (oEdites[j] == null) break;
                        oEdites[j].Validated += new EventHandler(ArmeEdicao_Validated);
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

            oFormEdite.Closing += new System.ComponentModel.CancelEventHandler(oFormEdite_Closing);
            oFormEdite.Activated += new EventHandler(oFormEdite_Activated);
            
            if (oFormEdite.ShowDialog() == DialogResult.OK)
                result = true;
            else
                result = false;

            oFormEdite.oNavegador.Dispose();
            oFormEdite.oNavegador = null;
            BmSource.BindingComplete -= new BindingCompleteEventHandler(BmSource_BindingComplete);

            try
            {
                for (int i = 0; i < Linhas.Count - 1; i++)
                {
                    List<Control> oEdites = Linhas[i].oedite;

                    for (int j = 0; j < oEdites.Count - 1; j++)
                    {
                        if (oEdites[j] == null) break;
                        oEdites[j].Validated -= new EventHandler(ArmeEdicao_Validated);
                    }
                }
            }
            catch (Exception)
            { }
            
            return result;
        }

        // validador geral de todos os controles que estão sendo editados
        void ArmeEdicao_Validated(object sender, EventArgs e)
        {
            Control ocontrol = (Control)sender;
            if (ocontrol.DataBindings == null) return;
            Binding bindcontrol;
            if (mudou == true)
            {
                salve.Enabled = true;
                cancele.Enabled = true;
                mudou = false;
            }
            return;
            /*try
            {
                bindcontrol = ocontrol.DataBindings[0];
            }
            catch (Exception) { return; }
            
            string valor = null;
            if (ocontrol is DateTimePicker)
                valor = Convert.ToString(((DateTimePicker)ocontrol).Value);
            else
                valor = ocontrol.Text;
            
            if (valor == null) return;
            string campo = bindcontrol.BindingMemberInfo.BindingField;
            BindingSource oBind = (BindingSource)bindcontrol.DataSource;
            DataRow orow = ((DataRowView)oBind.Current).Row;
            
            if (orow.RowState == DataRowState.Detached)
            {
                string valorformatado = "";
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


                else
                    if (valor.Trim() != Convert.ToString(orow[campo, DataRowVersion.Original]).Trim())
                    {
                        salve.Enabled = true;
                        cancele.Enabled = true;
                    }
            */
        }

       
      
        void oFormEdite_Activated(object sender, EventArgs e)
        {
            if (BmSource.CurrencyManager.Count < 1)
                BmSource.AddNew();
            if (camposelecionado == "") return;
            Form oform = (Form) sender;
            if ((oform.ActiveControl.DataBindings.Count >0) &&
                 (oform.ActiveControl.DataBindings[0].BindingMemberInfo.BindingField ==  camposelecionado) )
                 return;

            Panel oPanel = EPanel;  
            {
                for (int i = 0; i < oPanel.Controls.Count; i++)
                {
                    if ((((Control)oPanel.Controls[i]).DataBindings.Count >0) &&
                         (((Control)oPanel.Controls[i]).DataBindings[0].BindingMemberInfo.BindingField == camposelecionado))
                        if (((Control)oPanel.Controls[i]).CanFocus)
                            ((Control)oPanel.Controls[i]).Focus();
                }
            }
        }



        void oFormEdite_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((DataRowView)BmSource.Current) == null) return;
            DataRowView orow = (DataRowView)BmSource.Current;
            if (orow.Row == null) return;
            if (salve.Enabled)
            {
                e.Cancel = true;
                salve.PerformClick();
               
            }
        }

       

       
        void BmSource_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            BindingSource oBind = (BindingSource)sender;

            // dados do datasource sendo atualizados nos controles
            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                if ((oBind.Position == (oBind.Count - 1)) &&
                   ((e.Binding.Control is ComboBox)))//&& !(((DataRowView)oBind.Current).IsEdit))
                {
                    string campo = e.Binding.BindingMemberInfo.BindingField;

                    DataRow orow = ((DataRowView)oBind.Current).Row;
                    string valorcampo = Convert.ToString(orow[campo, DataRowVersion.Current]);
                    ComboBox ocombo = (ComboBox)e.Binding.BindableComponent;
                    ocombo.SelectAll();
                    ocombo.SelectedText = valorcampo;
                   // ocombo.Text = valorcampo;
                    e.Binding.BindingManagerBase.EndCurrentEdit();
                  
                }
                else
                    e.Binding.BindingManagerBase.EndCurrentEdit();
            }
            // dados do controle sendo atualizados no datasource
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                && e.Exception == null)
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    mudou = true;
                    //salve.Enabled = true;
                    //cancele.Enabled = true;
                }
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
        class TPosicaoLin { public int cabtop, cableft, cabtam, cabalt, objtop, objleft, objtam, objalt, adc_tam, adc_alt;}
        struct TLinEdicao
        {
            public List<TPosicaoLin> oListLin;
            public int maxtam, maxalt;
        }
        struct TGuardaAnt
        {
            public Control curcontrol;
            public List<string> Anterior;
        }

        public void MonteEdicao() // antes ArmeEdicao
        {
            if (Linhas.Count == 0) return;
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
                        TodosCampos.Add(objGuarde);
                        TPosicaoLin oposicao = new TPosicaoLin();
                        oposicao.cabtam = (Linhas[i].cabecalho[j].Length * ponto.X) + inter_min;
                        oposicao.cabalt = ponto.Y + 4; // padrao
                        oposicao.objalt = Linhas[i].oedite[j].Height + 2;
                        oposicao.objtam = Linhas[i].oedite[j].Width + inter_min;

                        if (((Control)Linhas[i].oedite[j]) is TextBoxBase)
                            oposicao.objalt = oposicao.objalt + 6;

                        if ((i == Linhas.Count - 1) && (((Control)Linhas[i].oedite[j]) is CheckedListBox)) // TDBEditCheckList o checklist do Historico
                            //((Linhas.OEdite[i,j] as TwinControl) is TDBEditCheckList) 
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
    }
    //Construir a classe de eneto que o edite vai disparar
    public class AlteraRegistroEventArgs : EventArgs
    {
        public AlteraRegistroEventArgs(DataRow[] orows,DataRowState otipo)
        {
            rows = orows;
            tipomuda = otipo;
        }
        private DataRow[] rows;
        private DataRowState tipomuda;

        public DataRowState TipoMuda
        {
            get { return tipomuda; }
            set { tipomuda = value; }

        }
        public DataRow [] Rows
        {
            get { return rows; }
            set { rows = value; }
        }
    }

}

/*
       void BmSource_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
       {
           BindingSource oBind = (BindingSource)sender;
            
           if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
           {

                   DataRow orow = ((DataRowView)oBind.Current).Row;
                   if (orow.RowState == DataRowState.Unchanged) return;
                    
                   Boolean ok = false;
                   for (int i = 0; i < oBind.CurrencyManager.Bindings.Count; i++)
                   {
                       Binding bindcontrol = oBind.CurrencyManager.Bindings[i];
                           string campo = bindcontrol.BindingMemberInfo.BindingField;
                            
                           if (Convert.ToString(orow[campo, DataRowVersion.Current]) != Convert.ToString(orow[campo, DataRowVersion.Default]))
                           {
                               ok = true;
                               break;
                           }
                       }
                   if (ok)
                   {
                       if (MessageBox.Show("Confirma Inclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                       {
                           //   DataRow orow = ((DataRowView)oBind.Current).Row;
                           oBind.EndEdit();
                           OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, e.ListChangedType));
                       }
                       else
                       {

                           oBind.CancelEdit();
                           oBind.RaiseListChangedEvents = false;
                           oBind.RemoveCurrent();
                           oBind.MovePrevious();
                           oBind.RaiseListChangedEvents = true;
                       }
                   }
                   cancele.Enabled = false;
                   salve.Enabled = false;
               }
           //}
           if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemChanged)
           {
               DataRow orow = ((DataRowView)oBind.Current).Row;
               string texto = "Confirma Alteracão?";
               if (orow.RowState == DataRowState.Added)
               {
                   texto = "Confirma Inclusão?";
               }
               if (MessageBox.Show(texto, "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                   {
                       //DataRow orow = ((DataRowView)oBind.Current).Row;
                       for (int i = 0; i <oBind.CurrencyManager.Bindings.Count;i++)
                       {
                           Binding bindcontrol = oBind.CurrencyManager.Bindings[i];
                           if (bindcontrol.BindableComponent is TextBox)
                           {
                               string campo = bindcontrol.BindingMemberInfo.BindingField;
                               if (orow[campo, DataRowVersion.Current] != orow[campo, DataRowVersion.Original])
                               {
                                   TextBox otext = (TextBox)bindcontrol.BindableComponent;
                                   if (otext.AutoCompleteSource == AutoCompleteSource.CustomSource)
                                   {
                                           if (!otext.AutoCompleteCustomSource.Contains(otext.Text))
                                               otext.AutoCompleteCustomSource.Add(otext.Text);
                                   }
                               }
                           }
                       }

                       oBind.EndEdit();
                        
                       OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[]{orow},e.ListChangedType));
                   }
                   else
                   {
                     oBind.CancelEdit();
                     if (orow.RowState != DataRowState.Added)
                     {
                         oBind.RaiseListChangedEvents = false;
                         orow.RejectChanges();
                         oBind.RaiseListChangedEvents = true;
                     }
                     else
                     {
                         BmSource.CancelEdit();
                         BmSource.RaiseListChangedEvents = false;
                         BmSource.RemoveCurrent();
                         BmSource.MovePrevious();
                         BmSource.RaiseListChangedEvents = true;
                     }
                   }
               cancele.Enabled = false;
               salve.Enabled = false;
           }
            
           if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
           {
               if (MessageBox.Show("Confirma Exclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                   {
                      DataRow orow = ((DataRowView)oBind.Current).Row;
                       if (orow.RowState == DataRowState.Deleted)
                          OnAlteraLinhas(new AlteraRegistroEventArgs(new DataRow[]{orow},e.ListChangedType));
                   }
                   else
                   {
                      DataRow orow = ((DataRowView)oBind.Current).Row;
                     if (orow.RowState == DataRowState.Deleted)   
                     {
                         oBind.RaiseListChangedEvents = false;
                         orow.RejectChanges();
                         oBind.RaiseListChangedEvents = true;
                     }
                   }
                 
           }
                      
       }*/


      
/*
 * namespace DotNetEvents
{
    using System;
    using System.Collections.Generic;

    // Define a class to hold custom event info
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string s)
        {
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    // Class that publishes an event
    class Publisher
    {

        // Declare the event using EventHandler<T>
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public void DoSomething()
        {
            // Write some code that does something useful here
            // then raise the event. You can also raise an event
            // before you execute a block of code.
            OnRaiseCustomEvent(new CustomEventArgs("Did something"));

        }

        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        protected virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<CustomEventArgs> handler = RaiseCustomEvent;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Format the string to send inside the CustomEventArgs parameter
                e.Message += String.Format(" at {0}", DateTime.Now.ToString());

                // Use the () operator to raise the event.
                handler(this, e);
            }
        }
    }

    //Class that subscribes to an event
    class Subscriber
    {
        private string id;
        public Subscriber(string ID, Publisher pub)
        {
            id = ID;
            // Subscribe to the event using C# 2.0 syntax
            pub.RaiseCustomEvent += HandleCustomEvent;
        }

        // Define what actions to take when the event is raised.
        void HandleCustomEvent(object sender, CustomEventArgs e)
        {
            Console.WriteLine(id + " received this message: {0}", e.Message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Publisher pub = new Publisher();
            Subscriber sub1 = new Subscriber("sub1", pub);
            Subscriber sub2 = new Subscriber("sub2", pub);

            // Call the method that raises the event.
            pub.DoSomething();

            // Keep the console window open
            Console.WriteLine("Press Enter to close this window.");
            Console.ReadLine();

        }
    }
}
*/