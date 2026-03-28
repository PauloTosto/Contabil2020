using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Windows.Forms;
using System.Data.OleDb;
using ClassConexao;


namespace ClassLibTrabalho
{
   //Class utilizada pela pegada de ponto.. (frmcltponto

    
    
    //Defini~ção dos objetos  

    public class TCLTCodigo : Object
    {
        public string indcod;
        public string desccod;
        public Decimal diarias; //currency
        public Decimal horas50;
        public Decimal horas100;
        public Decimal horasfe;
        public Decimal horas_efetivas;
        public string compati;
    }
    public class TCLTGleba : Object
    {
        public string ok;
        public Decimal diarias;
        public Decimal horas50;
        public Decimal horas100;
        public Decimal horasfe;
        public Decimal diariasN;
        public Decimal horas50N;
        public Decimal horasfeN;
        
        public Decimal horas_efetivas;
        public Decimal quant;
        public Decimal vlrdiaria;
        public TCLTGleba()
        {
            ok = "";
            diarias = 0.0M;
            horas_efetivas = 0.0M;
            horas50 = 0.0M;
            horas100 = 0.0M;
            horasfe = 0.0M;
            diariasN = 0.0M;
            horas50N = 0.0M;
            horasfeN = 0.0M;
            horas_efetivas = 0.0M;
            quant = 0.0M;
            vlrdiaria = 0.0M;
        }
    }



    public class TObjPonto : Object
    {
        public DateTime semana;
        public DateTime inicio_semana;
        public DateTime fim_semana;
        public Boolean temdomingo;
        public int inddomingo;
        public decimal horas_padrao; //currency
        public int ndias;
        public int diainicial;
        public int diafinal;
    }

    public class TDiaCodigo : Object
    {
        public ArrayList VetCod;//array of string
        public Decimal diarias;
        public Decimal horas_extra;
        public TDiaCodigo()
        {
            diarias = 0.0M;
            horas_extra = 0.0M;
            VetCod = new ArrayList();
        }
    }
    public class TObjVet3
    {
        public string ponto;
        public List<TDiaCodigo> Dados;
        public TObjVet3()
        {
            ponto = "";
            Dados = new List<TDiaCodigo>();
            for (int z = 0; z < 7; z++) //  TObjPonto(Campo_Mov[j]).Ndias do // ?? indexacao do dia sera a partir de zero?
            {
                TDiaCodigo DiaCodigo = new TDiaCodigo();
                DiaCodigo.VetCod.Add("  ");
                Dados.Add(DiaCodigo);
            }
        }
    }

    public class ClassTrab
    {
        public int FI, FE, Doenca;
        public List<TObjVet3> Vet3;
        public Dictionary<string, ClassGleba> Vet2;
        public ClassTrab(Int16 semanas)
        {
            FI = 0;
            FE = 0;
            Doenca = 0;
            Vet3 = new List<TObjVet3>();
            Vet2 = new Dictionary<string, ClassGleba>();
            for (int i = 0; i <= semanas; i++)
            {
                TObjVet3 oVet3 = new TObjVet3();
                Vet3.Add(oVet3);
            }
        }
    }
    public class ClassGleba
    {
        public string fazmov;
        public string bl;
        public string num_mod;
        public string codser;
        public List<TCLTGleba> Semana;
        public ClassGleba(Int16 semanas)
        {
            Semana = new List<TCLTGleba>();
            for (int i = 0; i <= semanas; i++)
            {
                TCLTGleba ocltgleba = new TCLTGleba();
                Semana.Add(ocltgleba);
            }
        }
    }
    //Construir a classe de evento o atualizafolha foi disparar para avisar disponibilidade ou não de gravar pesquisa 
    public class PermiteGravar : EventArgs
    {
        public PermiteGravar(Boolean ograve)
        {
            fgrave = ograve;
        }
        private bool fgrave;
       
        public Boolean Grave
        {
            get { return fgrave; }
            //set { fgrave = value; }
        }
    }


    public class TAtualizaFolha
    {
        public const DayOfWeek _DIADASEMANA = DayOfWeek.Thursday;
        public const DayOfWeek _DIAFIMDASEMANA = DayOfWeek.Wednesday;
        DataSet oTabelasAux;
        Dictionary<string, TCLTCodigo> FCLTCodigo;
        private Dictionary<string, ClassTrab> ListaTrab;
        public DateTime inicio, fim, datadigitada;
        public DataSet dsTrabalhistas;
        public DataSet dscltcad;
        public string tipo; //adiant ou ..
        public Decimal salmin, fgts;
        private decimal perc_adiant;
        Dictionary<DateTime, TObjPonto> ListPonto;
        private bool impsindical; 
 
        public DataTable dtcltfolha;
        public DataTable dtcltadian;

        public DataTable ultimacltfolhatotal;

        public DataTable dtGlebas;

        private bool fgrave;
        private bool fvirtual;
        //public int MyProperty { get; set; }

        public Boolean ImpSindical
        {
            get 
            {
                return impsindical; 
            }

            set 
            {
                impsindical = value;
            }

        }
        
        // ao criar a classe 
        // quando fgrave é alterado é gerado um evento que poderá ser escutado 
        public event EventHandler<PermiteGravar> PermiteGravarOk;

        protected virtual void OnAlteraGrave(PermiteGravar e)
        {
            EventHandler<PermiteGravar> handler = PermiteGravarOk;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public TAtualizaFolha()
        {
            oTabelasAux = TDataDivs.TabelasCLT();// Modelo e Servico
            FCLTCodigo = GetDict_CLTCodigo();//dicionario para acesso virtual do cltcodigo
            dtGlebas = TDataDivs.GlebasAtuais().Tables[0]; //glebas para saber a que setor está alocada 
            ListaTrab = new Dictionary<string, ClassTrab>();
            salmin = 0;
            fgts = 0;
            tipo = "";
            perc_adiant = 40;
            dtcltfolha = ClassCLTFOLHA.dtCLTFOLHA();//estrutura
            dtcltadian = ClassCLTADIAN.dtCLTADIAN();// estrutura
            fgrave = false;
            fvirtual = true;// indica se a
            impsindical = false;
        }
 
        public Dictionary<string, TCLTCodigo> CLTCodig
        {
            get
            {
                return FCLTCodigo;
            }
        }

        public DataTable Modelo
        {
            get
            {
                return oTabelasAux.Tables["MODELO"];
            }
        }

        public DataTable Servico
        {
            get
            {
                return oTabelasAux.Tables["SERVIC"];
            }
        }
        // folha virtual tabela com dados p/ emissão relatorio pagamento
        public DataTable CltFolhaTotal
        {
            get
                
            {
                DataSet dsfolhatotal;
                if (fvirtual)
                {
                    if (dtcltfolha == null)
                        return null;
                    
                    if ((dscltcad != null) && (dscltcad.Tables.Count > 1))
                        dsfolhatotal = ClassCLTFOLHA.Get_CltFolha(dtcltfolha, inicio, fim, null, dscltcad.Tables["CLTCAD"], null);
                    else
                        dsfolhatotal = ClassCLTFOLHA.Get_CltFolha(dtcltfolha, inicio, fim, null, null, null);
                    if (dsfolhatotal == null)
                        return null;
                    ultimacltfolhatotal = dsfolhatotal.Tables[0].Copy();
                    return ultimacltfolhatotal;
                }
                else
                {
                    dsfolhatotal = TDataControlTrabalho.Get_CltFolha(inicio, fim, null);
                    if (dsfolhatotal == null)
                        return null;
                    ultimacltfolhatotal = dsfolhatotal.Tables[0].Copy();
                    return ultimacltfolhatotal;
                }
            }
        }
        public DataTable CltAdiant
        {
            get
            {
                DataSet dsadiantotal;
                if (fvirtual)
                {
                    if (dtcltadian == null)
                        return null;
                    if ((dscltcad != null) && (dscltcad.Tables.Count > 1))
                        dsadiantotal = ClassCLTFOLHA.Get_CltAdiant(dtcltadian, inicio, fim, null, dscltcad.Tables["CLTCAD"]);
                    else
                        dsadiantotal = ClassCLTFOLHA.Get_CltAdiant(dtcltadian, inicio, fim, null, null);
                    if (dsadiantotal == null)
                        return null;
                    return dsadiantotal.Tables[0].Copy();
                }
                else
                {
                    dsadiantotal = TDataControlTrabalho.Get_CltAdiant(datadigitada,  null);
                    if (dsadiantotal == null)
                        return null;
                    return dsadiantotal.Tables[0].Copy();
                }
            }
        }
        // formato da folha de diaristas
        public DataTable Clt_RelFolhaTotal()
        {
            DataTable ocltfolhatotal;
            if (ultimacltfolhatotal == null)
                ocltfolhatotal = CltFolhaTotal;
            else
                ocltfolhatotal = ultimacltfolhatotal;
            DataTable Clt_RelFolha = ocltfolhatotal.Clone();
            for (int i = 1; i < 17; i++)
                Clt_RelFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "DIA" + i.ToString().Trim(), 2, false));
            Clt_RelFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Int32"), "NDIAS", 0, false));
            Clt_RelFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "ASSINATURA", 62, false));
            Clt_RelFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "ORDEM", 0, false));
            //  ocltfolhatotal.Select("(MENSALISTA = '')", "NOME,SETORCAD");
            int numfolha = 1;
            Clt_RelFolha.PrimaryKey = null ;
            string tsetor = "";
            Int32 ordem = 0;
            try
            {
                foreach (DataRow odatarow in ocltfolhatotal.Select("(MENSALISTA = '')", "SETORCAD,NOME").AsEnumerable())
                {
                    DataRow totrow = Clt_RelFolha.NewRow();
                    if (tsetor != odatarow["SETORCAD"].ToString())
                    {
                        numfolha = 1;
                        tsetor = odatarow["SETORCAD"].ToString();
                    }

                    totrow["TRAB"] = odatarow["TRAB"];

                    totrow["Mensalista"] = odatarow["Mensalista"];
                    totrow["Nome"] = numfolha.ToString().PadLeft(2, char.Parse(" ")) + " " + odatarow["Nome"];
                    totrow["SetorCad"] = odatarow["SetorCad"];

                    for (int i = 1; i < 16; i++)
                    {
                        int x = (i - 1) * 2;
                        totrow["DIA" + i.ToString().Trim()] = odatarow["PONTO"].ToString().Substring(x, 2);
                    }
                    totrow["DIA16"] = "  ";
                    
                    ordem += 1;
                    totrow["ORDEM"] = ordem.ToString();

                    totrow["SALLIQ"] = DBNull.Value;
                    totrow["SBRUTO"] = DBNull.Value;
                    totrow["HE50"] = DBNull.Value; //somatrio horas    
                    totrow["ADIANT"] = DBNull.Value;
                    totrow["SALARIO"] = DBNull.Value;

                    Clt_RelFolha.Rows.Add(totrow);

                    totrow = Clt_RelFolha.NewRow();
                    totrow["TRAB"] = odatarow["TRAB"];
                    //totrow["TRAB"] = string.Concat(odatarow["TRAB"].ToString(), "B");

                    for (int i = 1; i < 17; i++)
                    {
                        int x = ((15 + i) - 1) * 2;
                        totrow["DIA" + i.ToString().Trim()] = odatarow["PONTO"].ToString().Substring(x, 2);
                    }

                    totrow["Mensalista"] = odatarow["Mensalista"];
                    totrow["Nome"] = "   Cod.:" + odatarow["TRAB"];//odatarow["Nome"];
                    totrow["SetorCad"] = odatarow["SetorCad"];
                    ordem += 1;
                    totrow["ORDEM"] = ordem.ToString();

                    totrow["NDIAS"] = Convert.ToDecimal(Convert.ToDecimal(odatarow["HX"]) / 8);
                    totrow["ASSINATURA"] = "   ";
                    totrow["ADIANT"] = Convert.ToDecimal(odatarow["ADIANT"]);
                    totrow["SALARIO"] = Convert.ToDecimal(odatarow["SALARIO"]);
                    totrow["INSS"] = Convert.ToDecimal(odatarow["INSS"]);
                    totrow["IRFONTE"] = Convert.ToDecimal(odatarow["IRFONTE"]);
                    totrow["SALFAM"] = Convert.ToDecimal(odatarow["SALFAM"]);
                    totrow["EDUC"] = Convert.ToDecimal(odatarow["EDUC"]);
                    totrow["TERC"] = Convert.ToDecimal(odatarow["TERC"]);
                    totrow["FGTS"] = Convert.ToDecimal(odatarow["FGTS"]);

                    totrow["DECIMO"] = Convert.ToDecimal(odatarow["DECIMO"]);
                    totrow["FERIAS"] = Convert.ToDecimal(odatarow["FERIAS"]);
                    totrow["VLR_HXA"] = Convert.ToDecimal(odatarow["VLR_HXA"]);
                    totrow["HX"] = Convert.ToDecimal(odatarow["HX"]);
                    totrow["VLR_HXN"] = Convert.ToDecimal(odatarow["VLR_HXN"]);
                    totrow["HXA"] = Convert.ToDecimal(odatarow["HXA"]);
                    totrow["HXN"] = Convert.ToDecimal(odatarow["HXN"]);
                    totrow["VLR_HXS"] = Convert.ToDecimal(odatarow["VLR_HXS"]);

                    totrow["SALLIQ"] = Convert.ToDecimal(odatarow["SALLIQ"]);
                    totrow["SBRUTO"] = Convert.ToDecimal(odatarow["SBRUTO"]);
                    totrow["HE50"] = Convert.ToDecimal(odatarow["HE50"]); //somatrio horas    
                    Clt_RelFolha.Rows.Add(totrow);
                    
                    numfolha += 1;
                    if (numfolha > 16)
                        numfolha = 1;
                }
            }
            catch(Exception)
            { 
                throw;
            }
            return Clt_RelFolha;
        }

        private Decimal formate_retorna(Decimal valor)
        {
            // CultureInfo ci;
            // ci = new CultureInfo("en-US");
            string result = valor.ToString("F02");
            try
            {
                return Convert.ToDecimal(result.Trim());
            }
            catch
            {
                return valor;
            }
        }
        private Decimal domingo_hextra(Decimal thoras, Decimal tsal)
        {
            if (thoras == 0.0M)
                return 0.0M;
            else
            {
                if (fim < new DateTime(2011, 12, 1))
                {
                    return TDataControlTrabalho.ptRound((thoras * (tsal / 240.0M)), 2);
                }
                else
                {
                    return TDataControlTrabalho.ptRound((thoras * ((tsal / 220.0M) * 2.0M)), 2);
                }
            }
        }
        private Decimal normal_hextra(Decimal thoras, Decimal tsal)
        {
            if (thoras == 0.0M)
                return 0.0M;
            else
                return TDataControlTrabalho.ptRound((thoras * ((tsal / 220.0M) * 1.5M)), 2);
        }
        // alterar a partir de agosto o calculo da noturno 
        private Decimal noturno_hextra(Decimal thoras, Decimal tsal)
        {
            if (thoras == 0.0M)
                return 0.0M;
            else
                if (fim < new DateTime(2011, 12, 1))
                {

                    return TDataControlTrabalho.ptRound((thoras * ((tsal / 220.0M) * 2.25M)), 2);
                }
                else
                    return 0.0M;
        }

       
        public string glebasetoratual(string gleba,DateTime data)
        {
            string tsetor = "";
            DataRow[] orows = dtGlebas.Select("COD = '"+gleba+"'","INICIO DESC");
            if (orows.Length <= 0)
                return tsetor;
            foreach (DataRow linha in orows.AsEnumerable())
            {
               if ( Convert.ToDateTime(linha["INICIO"]).Date <= data.Date)
               {
                   tsetor = linha["SETOR"].ToString();
                   break;
               }
            }
            return tsetor;
        }

        public Boolean Gera_Arquivo(DateTime odata)
        {
            fgrave = false;
            OnAlteraGrave(new PermiteGravar(fgrave));
 
            inicio = TDataControlReduzido.PrimeiroDiaMes(odata);
            fim = TDataControlReduzido.UltimoDiaMes(odata);
            datadigitada = odata;
            fvirtual = true;
            // emqualquer caso essas tabelas serão disponibilizadas
            dsTrabalhistas = TDataControlTrabalho.TabelasTrabalhistas();

            salmin = TDataControlTrabalho.CalcSalarioMinimo(fim, dsTrabalhistas.Tables["TABSAL"]);
            fgts = TDataControlTrabalho.CalcFgts(fim, dsTrabalhistas.Tables["TABSAL"]);

            dscltcad = ClassCLTFOLHA.Get_CltCad_CalcFolha(inicio, fim, null,dsTrabalhistas);


            if (tipo == "A")
            {
                dtcltadian.Rows.Clear();
                DataSet dsadiant = ClassCLTFOLHA.Get_TotalCltAdiant(inicio, fim, null);
                if (dsadiant.Tables.Count > 0)
                {
                    string tlit = "";
                    foreach (DataRow orow in dsadiant.Tables[0].AsEnumerable())
                    {
                        tlit += Convert.ToDateTime(orow["DATA"]).ToShortDateString() + "  R$" +
                           TDataControlReduzido.FormatNumero(Convert.ToSingle(orow["totadiant"]), 2);
                    }
                    if (tlit != "")
                        if (MessageBox.Show("Adiant.Já Lançado.Refaz?", tlit, MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            fvirtual = false;
                            return false;
                        }
                }
            }
            else
            {
                dtcltfolha.Rows.Clear();
                DataSet dsfolha = ClassCLTFOLHA.Get_TotalCltFolha(inicio, fim, null);
                if (dsfolha.Tables.Count > 0)
                {
                    string tlit = "";
                    foreach (DataRow orow in dsfolha.Tables[0].AsEnumerable())
                    {
                        tlit += Convert.ToDateTime(orow["DATA"]).ToShortDateString() + "  R$" +
                           TDataControlReduzido.FormatNumero(Convert.ToSingle(orow["totsalario"]), 2);
                    }

                    if (tlit != "")
                        if (MessageBox.Show("Folha.Já Lançado.Refaz?", tlit, MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            fvirtual = false;
                            return false;
                        }
                }
            }
           // dsTrabalhistas = TDataControlTrabalho.TabelasTrabalhistas();

           // salmin = TDataControlTrabalho.CalcSalarioMinimo(fim, dsTrabalhistas.Tables["TABSAL"]);
           // fgts = TDataControlTrabalho.CalcFgts(fim, dsTrabalhistas.Tables["TABSAL"]);
         
            //falta
            // Gera_Tarefa;

            //Atualiza_Tarefa;

            //dscltcad = ClassCLTFOLHA.Get_CltCad_CalcFolha(inicio, fim, null);
            try
            {
                GeraValores();
                if (tipo == "")
                    Atualiza_Folha();
                else
                    if (tipo == "A")  // adiantamento
                        Atualiza_Adiant();
                    else return false;
                fgrave = true;
                OnAlteraGrave(new PermiteGravar(fgrave));
                return true;
            }
            catch
            {
                throw; 
            }
        }
        
        public static DateTime DiaDoPonto(DateTime tdata, DayOfWeek tconst)
        {
            DateTime result = tdata;
            while (result.DayOfWeek != tconst)   //Wednesday"
                result = result.AddDays(-1);
            return result;
        }

        static public Dictionary<DateTime, TObjPonto> Peg_Ponto_LongoPeriodo(DateTime tdata, string tipo)
        {
            Dictionary<DateTime, TObjPonto> result = new Dictionary<DateTime, TObjPonto>();
            DateTime inicio = TDataControlReduzido.PrimeiroDiaMes(tdata);
            DateTime fim;
            TObjPonto oponto;
            if (tipo == "")
                fim = TDataControlReduzido.UltimoDiaMes(tdata);
            else if (tipo == "W")
                fim = DiaDoPonto(tdata, _DIADASEMANA);
            else
                fim = DiaDoPonto(tdata, _DIAFIMDASEMANA);

            tdata = DiaDoPonto(inicio, _DIADASEMANA);

            while (tdata <= fim)
            {
                oponto = new TObjPonto();
                oponto.semana = tdata;
                oponto.temdomingo = false;
                oponto.inddomingo = 0;
                oponto.inicio_semana = tdata;
                oponto.horas_padrao = 44;
                oponto.ndias = 0;
                oponto.fim_semana = oponto.inicio_semana.AddDays(6);
                if (inicio > oponto.inicio_semana)
                    oponto.inicio_semana = inicio;
                if (fim < oponto.fim_semana)
                    oponto.fim_semana = fim;
                oponto.ndias = (oponto.fim_semana - oponto.inicio_semana).Days + 1;
                oponto.diainicial = (7 - oponto.ndias) + 1;
                if ((oponto.fim_semana == fim) && (oponto.ndias != 7)) oponto.diainicial = 1;
                oponto.diafinal = oponto.diainicial + oponto.ndias - 1;
                while (((oponto.inicio_semana.AddDays(oponto.inddomingo)) <= oponto.fim_semana) && (!oponto.temdomingo))
                {
                    if (oponto.inicio_semana.AddDays(oponto.inddomingo).DayOfWeek == DayOfWeek.Sunday)
                    {
                        oponto.temdomingo = true;
                        break;
                    }
                    oponto.inddomingo += 1;
                }

                if (!oponto.temdomingo) oponto.inddomingo = 0;
                else
                    oponto.inddomingo = oponto.diainicial + oponto.inddomingo;

                result.Add(tdata.Date, oponto);
                tdata = tdata.AddDays(7);
            }
            ICollection<TObjPonto> pontos = result.Values;
            oponto = pontos.First();
            if (oponto.temdomingo)
            {
                int j = 0;
                oponto.horas_padrao = 0M;
                while ((oponto.inicio_semana.AddDays(j)) <= oponto.fim_semana)
                {
                    if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Sunday)
                        if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Saturday)
                            oponto.horas_padrao = oponto.horas_padrao + 4;
                        else
                            oponto.horas_padrao = oponto.horas_padrao + 8;
                    j += 1;
                }
            }
            oponto = pontos.Last();
            if (oponto.temdomingo)
            {
                int j = 0;
                oponto.horas_padrao = 0M;
                while ((oponto.inicio_semana.AddDays(j)) <= oponto.fim_semana)
                {
                    if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Sunday)
                        if (oponto.inicio_semana.AddDays(j).DayOfWeek != DayOfWeek.Saturday)
                            oponto.horas_padrao = oponto.horas_padrao + 4;
                        else
                            oponto.horas_padrao = oponto.horas_padrao + 8;
                    j += 1;
                }
            }

            return result;
        }

        public static TObjPonto Peg_PontoDireto(DateTime tdata, TObjPonto[] Pontos)
        {
            TObjPonto result = null;
            for (int i = Pontos.Length - 1; i >= 0; i--)
            {
                if (tdata.Date == Pontos[i].semana.Date)
                {
                    result = Pontos[i];
                    break;
                }
            }
            return result;
        }

        public csQuadraVelho.csQuadras DadosProdQuadra(string tquadra, string tcodser, string tnum, DateTime dtaponto, DateTime dtafim)
        {
            string tcult = "";
            string tunid = ServicoUnid(tcodser);
            if (tunid.Substring(1, 2) == "PL")
                tunid = "PL";
            else
                if (tunid.Substring(1, 3) == "TAR")
                    tunid = "TAR";
                else
                    if (tunid.Substring(1, 2) == "CX")
                        tunid = "CX";
            if (tnum.Trim() != "") { tcult = ModeloCultura(tnum); };
            csQuadraVelho.csQuadras result = new csQuadraVelho.csQuadras(tquadra, tcodser, tnum, tunid, tcult, dtaponto, dtafim);
            return result;
        }

        public string ServicoUnid(string fcodser)
        {
            DataRow orow = Servico.Rows.Find(new object[] { fcodser });
            if (orow == null) return "";
            return orow["UNID"].ToString();

        }
        public string ServicoDesc(string fcodser)
        {
            DataRow orow = Servico.Rows.Find(new object[] { fcodser });
            if (orow == null) return "";
            return orow["DESC"].ToString();

        }

        public string ModeloCultura(string fnum)
        {
            DataRow[] orow = Modelo.Select("(NUM = '" + fnum + "') AND (COD = '')");
            if (orow == null) return "";
            if (!(orow.Length > 0)) return "";
            return orow[0]["DESC"].ToString();
        }


        public DataSet PegDadosEstatistica(DateTime inicio, DateTime fim)
        {

            DateTime linicio = TAtualizaFolha.DiaDoPonto(inicio, TAtualizaFolha._DIADASEMANA);//ENCodeDate(ano,mes,1);
            DateTime lfim = TAtualizaFolha.DiaDoPonto(fim, TAtualizaFolha._DIAFIMDASEMANA);  //UltimoDiaMes(tdata);

            //rever...
            return TDataDivs.Get_CLTPontosPeriodo(linicio, lfim, "D");
        }

        static public Dictionary<string, TCLTCodigo> GetDict_CLTCodigo()
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            strOleDb = "SELECT  cltcodig.* FROM " + path + "CLTCODIG"; //+
            Dictionary<string, TCLTCodigo> result = new Dictionary<string, TCLTCodigo>();
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbDataReader leitor = OleDbcomm.ExecuteReader();

                if (leitor.HasRows)
                {
                    TCLTCodigo oCodigo;
                    while (leitor.Read())
                    {
                        if (Convert.IsDBNull(leitor["INDCOD"])) continue;
                        oCodigo = new TCLTCodigo();
                        oCodigo.desccod = leitor["DESCCOD"].ToString();
                        oCodigo.indcod = leitor["INDCOD"].ToString();
                        oCodigo.diarias = Convert.ToDecimal(leitor["FATOR"]); // 3  horas normais
                        oCodigo.horas_efetivas = Convert.ToDecimal(leitor["HORAS"]); // 4 horas efetivas
                        oCodigo.horas50 = Convert.ToDecimal(leitor["X_HORAS"]); // 5 horas 50
                        oCodigo.horas100 = Convert.ToDecimal(leitor["NOTURNO"]); // 6  horas 100
                        oCodigo.compati = leitor["COMPATI"].ToString();  // 7
                        oCodigo.horasfe = Convert.ToDecimal(leitor["H_NORMAL"]); // 8 horas Feriado
                        result.Add(oCodigo.indcod, oCodigo);
                    }
                }
                leitor.Close();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }


        public DataSet PegDadosGeraValor(DateTime inicio, DateTime fim)
        {
            DateTime linicio = TAtualizaFolha.DiaDoPonto(inicio, TAtualizaFolha._DIADASEMANA);//ENCodeDate(ano,mes,1);
            DateTime lfim = TAtualizaFolha.DiaDoPonto(fim, TAtualizaFolha._DIAFIMDASEMANA);  //UltimoDiaMes(tdata);
            //rever...
            return TDataDivs.GeraValor_CLTPontosPeriodo(linicio, lfim, "D");
        }

        private string CompleteDireita(string campo, int quantos)
        {
            string result = campo;
            while (result.Length < quantos)
                result += " ";
            return result;
        }

        public void GeraValores()
        {
            ListaTrab.Clear();
            Dictionary<string, TCLTCodigo> ListCodigo = CLTCodig;
            ListPonto = Peg_Ponto_LongoPeriodo(datadigitada, tipo);
            TObjPonto oponto = null;
            DateTime iniciopesq, fimpesq;
            iniciopesq = ListPonto.Keys.Min<DateTime>();
            fimpesq = ListPonto.Keys.Max<DateTime>();
            DataSet dsCltPonto = TDataDivs.GeraValor_CLTPontosPeriodo(iniciopesq, fimpesq, "D");
            string chavetarefa = string.Empty;
            ClassTrab otrab;
            ClassGleba ogleba;
            ArrayList vet_sabado = new ArrayList();
            for (int i = 1; i <= 4; i++)
                vet_sabado.Add(" " + i.ToString());
            for (int i = 1; i <= 9; i++)
                vet_sabado.Add("." + i.ToString());

            DataTable dtcltponto = dsCltPonto.Tables[0];

        
            //condições para a pegada das linhas ok 
            var linhasok =
               from linha in dtcltponto.AsEnumerable()
               where ((linha.Field<string>("OK").Trim() == "X") && (linha.Field<string>("BL").Trim() != "") &&
                        (linha.Field<string>("CODSER").Trim() != "") && (linha.Field<decimal>("QUANT") == 0))
               select linha;
            
        
            QuantitativosLinhaOk okquantitativo = new QuantitativosLinhaOk();
            okquantitativo.InformeTodasLinhasOk(linhasok, iniciopesq, fimpesq);
            DateTime comparadta = DateTime.MinValue;
            Int16 indsemana = -1;
            for (int z = 0; z < dtcltponto.Rows.Count; z++)
            {
                DataRow ponto = dtcltponto.Rows[z];
                if (comparadta != Convert.ToDateTime(ponto["DATA"]))
                {
                    comparadta = Convert.ToDateTime(ponto["DATA"]);
                    indsemana += 1;
                    oponto = ListPonto[comparadta.Date];
                    if (oponto == null)
                    {
                        MessageBox.Show("Erro pegada dias ponto");
                        return;
                    }
                }
                if (!(ListaTrab.ContainsKey(Convert.ToString(ponto["TRAB"]))))
                {
                    otrab = new ClassTrab(Convert.ToInt16(ListPonto.Count));
                    ListaTrab.Add(Convert.ToString(ponto["TRAB"]), otrab);
                }
                else
                    otrab = ListaTrab[Convert.ToString(ponto["TRAB"])];
                if (!otrab.Vet2.ContainsKey(Convert.ToString(ponto["FAZMOV"]) + Convert.ToString(ponto["BL"]) +
                            Convert.ToString(ponto["NUM_MOD"]) + Convert.ToString(ponto["CODSER"])))
                {
                    ogleba = new ClassGleba(Convert.ToInt16(ListPonto.Count));
                    ogleba.fazmov = Convert.ToString(ponto["FAZMOV"]);
                    ogleba.bl = Convert.ToString(ponto["BL"]);
                    ogleba.num_mod = Convert.ToString(ponto["NUM_MOD"]);
                    ogleba.codser = Convert.ToString(ponto["CODSER"]);
                }
                else
                    ogleba = otrab.Vet2[Convert.ToString(ponto["FAZMOV"]) + Convert.ToString(ponto["BL"]) +
                            Convert.ToString(ponto["NUM_MOD"]) + Convert.ToString(ponto["CODSER"])];

                TCLTGleba codgleba = ogleba.Semana[indsemana];
                if (!(Convert.ToDecimal(ponto["QUANT"]) == 0.0M))
                    codgleba.quant = codgleba.quant + Convert.ToDecimal(ponto["QUANT"]);
                if (!(Convert.ToString(ponto["OK"]).Trim() == ""))
                {
                    if (Convert.ToDecimal(ponto["QUANT"]) == 0.0M) // no delphi essa condição era quando quant # 0 ! consertei em junho de 2011
                        codgleba.quant = codgleba.quant + okquantitativo.quant(ponto,oponto.semana); 
                    codgleba.ok = "X";
                }
                bool desconta_remun = true;
                int j;
                for (j = oponto.diainicial; j <= oponto.diafinal; j++)
                {
                    string campo = "DIA" + j.ToString().Trim();
                    if ((ponto[campo] == null) || (ponto[campo].ToString().Trim() == "")) continue;
                    TCLTCodigo ocodigo = ListCodigo[ponto[campo].ToString()];
                    // DataRow orow = dtCLTCodig.Rows.Find(ponto[campo]);
                    if (ocodigo == null)
                    {
                        MessageBox.Show("Tabela de Codigo Incoerente :" + Convert.ToString(ponto[campo]));
                        continue;
                    }
                    TDiaCodigo DiaCodigo = otrab.Vet3[indsemana].Dados[j - 1];
                    //DiaCodigo.
                    if ((ocodigo.horas50 == 0) && (ocodigo.horas100 == 0))
                    {
                        Boolean tret = false;
                        for (int y = 0;y < DiaCodigo.VetCod.Count;y++)
                        {
                            if (DiaCodigo.VetCod[y].ToString() == "  ")
                            {
                                DiaCodigo.VetCod[y] = ocodigo.indcod;
                                tret = true;
                                break;
                            }
                        }
                        if (tret == false)
                        {
                            DiaCodigo.VetCod.Add(ocodigo.indcod);
                        }
                    }
                    
                    if (ocodigo.indcod == "FI")
                    {
                        otrab.FI = otrab.FI + 1;
                        desconta_remun = true;
                    }
                    if (ocodigo.indcod == "A")
                    {
                        otrab.Doenca = otrab.Doenca + 1;
                    }
                    if (ocodigo.indcod == "FE")
                    {
                        otrab.FE = otrab.FE + 1;
                        if (j == oponto.inddomingo)
                            desconta_remun = true;
                    }

                    // horario normal

                    if (ocodigo.diarias > 0)
                    {
                        if (fim < new DateTime(2011, 12, 1))
                        {

                            if ((oponto.inddomingo == j) && (ocodigo.horas_efetivas > 0))
                            {
                                codgleba.horasfe = codgleba.horasfe + ocodigo.diarias;
                                DiaCodigo.horas_extra = DiaCodigo.horas_extra + ocodigo.diarias;
                                if (ponto["QUANT"].ToString() == "X")
                                {
                                    codgleba.horasfeN = codgleba.horasfeN + ocodigo.diarias;
                                    //DiaCodigo.horas_extraN = DiaCodigo.horas_extraN + ocodigo.diarias;
                                }

                            }
                            else       // caso de sabado
                                if (((oponto.inddomingo - 1) == j) && (vet_sabado.IndexOf(ocodigo.indcod) != -1))
                                {
                                    codgleba.diarias = codgleba.diarias + (ocodigo.diarias * 2);
                                    DiaCodigo.diarias = DiaCodigo.diarias + (ocodigo.diarias * 2);
                                    if (ponto["QUANT"].ToString() == "X")
                                    {
                                        codgleba.diariasN = codgleba.diariasN + (ocodigo.diarias * 2);
                                        //DiaCodigo.diariasN = DiaCodigo.diariasN + (ocodigo.diarias * 2);
                                    }
                                }
                                else
                                {
                                    codgleba.diarias = codgleba.diarias + ocodigo.diarias;
                                    DiaCodigo.diarias = DiaCodigo.diarias + ocodigo.diarias;
                                    if (ponto["QUANT"].ToString() == "X")
                                    {
                                        codgleba.diariasN = codgleba.diariasN + ocodigo.diarias;
                                        //DiaCodigo.diariasN = DiaCodigo.diariasN + ocodigo.diarias;
                                    }
                                }
                        }
                        else
                        {
                            if (((oponto.inddomingo - 1) == j) && (vet_sabado.IndexOf(ocodigo.indcod) != -1))
                            {
                                codgleba.diarias = codgleba.diarias + (ocodigo.diarias * 2);
                                DiaCodigo.diarias = DiaCodigo.diarias + (ocodigo.diarias * 2);
                                if (ponto["QUANT"].ToString() == "X")
                                {
                                    codgleba.diariasN = codgleba.diariasN + (ocodigo.diarias * 2);
                                    //DiaCodigo.diariasN = DiaCodigo.diariasN + (ocodigo.diarias * 2);
                                }
                            }
                            else
                            {
                                codgleba.diarias = codgleba.diarias + ocodigo.diarias;
                                DiaCodigo.diarias = DiaCodigo.diarias + ocodigo.diarias;
                                if (ponto["QUANT"].ToString() == "X")
                                {
                                    codgleba.diariasN = codgleba.diariasN + ocodigo.diarias;
                                    //DiaCodigo.diariasN = DiaCodigo.diariasN + ocodigo.diarias;
                                }
                            }
                        }
                    }
                    if (ocodigo.horasfe > 0)
                    {
                        codgleba.horasfe = codgleba.horasfe + ocodigo.horasfe;
                        if (ponto["QUANT"].ToString() == "X")
                        {
                            codgleba.horasfeN = codgleba.horasfeN + ocodigo.horasfe;
                        }
                        DiaCodigo.horas_extra = DiaCodigo.horas_extra + ocodigo.horasfe;
                    }

                    if (ocodigo.horas50 > 0)
                    {
                        if (ponto["QUANT"].ToString() == "X")
                        {
                            codgleba.horas50N = codgleba.horas50N + ocodigo.horas50;
                        }
                        codgleba.horas50 = codgleba.horas50 + ocodigo.horas50;
                    }
                    if (fim < new DateTime(2011, 12, 1))
                    {
                        if (ocodigo.horas100 > 0)
                        {
                            codgleba.horas100 = codgleba.horas100 + ocodigo.horas100;
                        }
                    }

                    // horas EFETIVAMENTE TRABALHADAS
                    if (ocodigo.horas_efetivas > 0)
                    {
                        if (((oponto.inddomingo - 1) == j) && (ocodigo.indcod == "X "))
                            codgleba.horas_efetivas = codgleba.horas_efetivas + (ocodigo.horas_efetivas / 2);
                        else
                            codgleba.horas_efetivas = codgleba.horas_efetivas + ocodigo.horas_efetivas;
                    }
                    otrab.Vet3[indsemana].Dados[j - 1] = DiaCodigo;//j-1
                }
                ogleba.Semana[indsemana] = codgleba;
                otrab.Vet2[Convert.ToString(ponto["FAZMOV"]) + Convert.ToString(ponto["BL"]) +
                            Convert.ToString(ponto["NUM_MOD"]) + Convert.ToString(ponto["CODSER"])] = ogleba;

                ListaTrab[Convert.ToString(ponto["TRAB"])] = otrab;
            }
            // preencher o campo ponto do mes
            foreach (KeyValuePair<string, ClassTrab> k_otrab in ListaTrab)
            {
                int indponto = -1;
                foreach (KeyValuePair<DateTime, TObjPonto> k_ponto in ListPonto)
                {
                    indponto += 1;
                    oponto = k_ponto.Value;
                    otrab = k_otrab.Value;
                    otrab.Vet3[indponto].ponto = "";
                    for (int j = oponto.diainicial; j <= oponto.diafinal; j++)
                    {
                        TDiaCodigo DiaCodigo = otrab.Vet3[indponto].Dados[j - 1];
                        if (DiaCodigo.VetCod.Count == 1)       //complete á direita
                            otrab.Vet3[indponto].ponto = otrab.Vet3[indponto].ponto + CompleteDireita(DiaCodigo.VetCod[0].ToString(), 2);
                        else
                        {
                            if (DiaCodigo.diarias != 0)
                            {
                                if (DiaCodigo.diarias == 8)
                                {
                                    bool tret = false;
                                    for (int z = 0; z < DiaCodigo.VetCod.Count; z++)
                                    {
                                        if (CompleteDireita(DiaCodigo.VetCod[z].ToString(),2) == "R ") //DiaCodigo.VetCod[z].ToString().PadRight(2, char.Parse(" "))
                                        {
                                            tret = true;
                                            break;
                                        }
                                    }
                                    if (!tret)
                                        otrab.Vet3[indponto].ponto = otrab.Vet3[indponto].ponto + "X ";
                                    else
                                        otrab.Vet3[indponto].ponto = otrab.Vet3[indponto].ponto + "R ";
                                }
                                else
                                {
                                    Boolean tret = false;
                                    foreach (KeyValuePair<string, TCLTCodigo> k_cltcodigo in ListCodigo)
                                    {
                                        if (k_cltcodigo.Value.diarias == DiaCodigo.diarias)
                                        {
                                            otrab.Vet3[indponto].ponto = otrab.Vet3[indponto].ponto + k_cltcodigo.Value.indcod;
                                            tret = true;
                                            break;
                                        }
                                    }
                                    if (tret == false)
                                    {
                                        if (DiaCodigo.diarias > 8) otrab.Vet3[indponto].ponto = otrab.Vet3[indponto].ponto + "X+";
                                        else
                                        {
                                            MessageBox.Show("Nao Identificado Simbolo para Horas PAgas:");
                                            otrab.Vet3[indponto].ponto = otrab.Vet3[indponto].ponto + "? ";
                                        }
                                    }
                                }

                            }
                            else
                            {
                                for (int z = 0; z < DiaCodigo.VetCod.Count; z++)
                                {
                                    if (DiaCodigo.VetCod[z].ToString() != "R ")
                                    {
                                        otrab.Vet3[indponto].ponto = otrab.Vet3[indponto].ponto + DiaCodigo.VetCod[z];
                                        break;
                                    }
                                }

                            }

                        }
                    }
                }
            }

        }
        private void Atualiza_Folha()
        {
            Decimal totalfolha;
            Int32 ndias_periodo, dias_uteis, dias_trab;
           // Boolean esta_em_ferias;
            string tponto;
            Decimal tdias, tsal, tdep, dia_real;
            DataSet dsReajustes = TDataControlTrabalho.Get_CltReajuste(inicio, fim, null, "");
            dias_uteis = 0;
            foreach (KeyValuePair<DateTime, TObjPonto> k_ponto in ListPonto.AsEnumerable())
                dias_uteis += k_ponto.Value.ndias;
            // Tende a evoluir para 15 dias
            ndias_periodo = (fim.Date - inicio.Date).Days + 1;
            totalfolha = 0.0M;
            foreach (KeyValuePair<string, ClassTrab> oTrab in ListaTrab.AsEnumerable())
            {
                //esta_em_ferias = false;
                tponto = "";
                foreach (TObjVet3 ovet3 in oTrab.Value.Vet3)
                {
                    tponto += ovet3.ponto;
                }
                TCLTGleba vet_horas = new TCLTGleba();
                foreach (KeyValuePair<string, ClassGleba> ovet2 in oTrab.Value.Vet2)
                {
                    foreach (TCLTGleba ogleba in ovet2.Value.Semana)
                    {
                        vet_horas.diarias += ogleba.diarias;
                        vet_horas.horasfe += ogleba.horasfe;
                        vet_horas.horas50 += ogleba.horas50;
                        vet_horas.diariasN += ogleba.diariasN;
                        vet_horas.horasfeN += ogleba.horasfeN;
                        vet_horas.horas50N += ogleba.horas50N;
                        vet_horas.horas100 += ogleba.horas100;
                        vet_horas.horas_efetivas += ogleba.horas_efetivas;
                    }
                }
                tdias = TDataControlTrabalho.ptRound((vet_horas.diarias / 8), 2);

                if ((tdias != 0) || (tponto != "") || (vet_horas.horasfe != 0) ||
                    (vet_horas.horas50 != 0) || (vet_horas.horas100 != 0) || (tponto != ""))
                {
                    tsal = 0;
                    tdep = 0;
                    dias_trab = 0;
                    DataRow oRow = dscltcad.Tables["CLTCAD"].Rows.Find(oTrab.Key);
                    if (oRow == null) continue;//mande mensagem... isto não deve ocorrer
                    if (Convert.ToDateTime(oRow["DEMI"]).ToString() != TDataControlReduzido.DataVazia())
                    {
                        if ((Convert.ToDateTime(oRow["DEMI"]).CompareTo(fim) <= 0) &&
                            (Convert.ToDateTime(oRow["DEMI"]).CompareTo(inicio) >= 0))
                        {
                            dias_trab = (Convert.ToDateTime(oRow["DEMI"]).Date - inicio.Date).Days + 1;
                        }
                    }
                    if ((Convert.ToDateTime(oRow["ADMI"]).CompareTo(inicio) >= 0))
                    {
                        dias_trab = (fim.Date - Convert.ToDateTime(oRow["ADMI"]).Date).Days + 1;
                    }
                
                    tsal = TDataControlTrabalho.CalcSalBase(oTrab.Key, Convert.ToDateTime(oRow["ADMI"]),
                              Convert.ToDecimal(oRow["SALBASE"]), Convert.ToDecimal(salmin), fim, dsReajustes.Tables[0]);

                    TCLTGleba vet_salario = new TCLTGleba();
                    // para os nao demitidos
                    dia_real = TDataControlTrabalho.ptRound(tsal / ndias_periodo, 4);
                
                    // calculo de horas extras simples (ou seja trabalho no domingo e feriados)

                    //if ((vet_horas.diarias / 8.0M) == ndias_periodo)
                       // vet_salario.diarias = TDataControlTrabalho.ptRound(((vet_horas.diarias / 8.0M) * dia_real), 2);
                    //else
                      //  vet_salario.diarias = TDataControlTrabalho.ptRound(((vet_horas.diarias / 8.0M) * dia_real), 2);        // horas normais
                    vet_salario.diarias = TDataControlTrabalho.ptRound(((vet_horas.diarias / 8.0M) * dia_real), 2);
                    vet_salario.horasfe = domingo_hextra(vet_horas.horasfe, tsal);        // horas em feriados
                    vet_salario.horas50 = normal_hextra(vet_horas.horas50, tsal);        // horas extra 50 %

                    if (fim < new DateTime(2011, 12, 1).Date)
                        vet_salario.horas100 = noturno_hextra(vet_horas.horas100, tsal);       // horas extra 100 %
                    else
                    {
                        if ((vet_horas.diariasN + vet_horas.horasfeN + vet_horas.horas50N) > 0)
                        {
                            vet_salario.diariasN = TDataControlTrabalho.ptRound(((vet_horas.diariasN / 8.0M) * dia_real), 2);
                            vet_salario.horasfeN = domingo_hextra(vet_horas.horasfeN, tsal);        // horas em feriados
                            vet_salario.horas50N = normal_hextra(vet_horas.horas50N, tsal);        // horas extra 50 %
                            vet_salario.horas100 = TDataControlTrabalho.ptRound(((vet_salario.diariasN + vet_salario.horasfeN + vet_salario.horas50N) * 0.25M), 2);
                        }
                    }
                    //vet_reg =
                    grave_real_corrente(oTrab, vet_salario, vet_horas, Convert.ToString(oRow["GLECAD"]), tponto, tsal, dia_real,
                    ndias_periodo, dias_trab, oRow);

                    totalfolha = (totalfolha + vet_salario.diarias + vet_salario.horasfe + vet_salario.horas50 + vet_salario.horas100);
                }
            }
            if (totalfolha != 0)
            {
                Decimal teduc = TDataControlTrabalho.ptRound(totalfolha * 0.025M, 2);
                Decimal terc = TDataControlTrabalho.ptRound(totalfolha * 0.002M, 2);
                Decimal reduc = teduc;
                Decimal rterc = terc;
                Decimal maiorrat = 0;
                DataRow pivot = null;
                foreach (DataRow linha in dtcltfolha.AsEnumerable())
                {
                    Decimal trateio = Convert.ToDecimal(linha["SALARIO"]) + Convert.ToDecimal(linha["VLR_HXA"]) +
                        Convert.ToDecimal(linha["VLR_HXS"]) +
                           Convert.ToDecimal(linha["VLR_HXN"]);
                    linha.BeginEdit();
                    if (trateio != 0)
                    {
                        trateio = trateio / totalfolha;
                        linha["EDUC"] = TDataControlTrabalho.ptRound(trateio * teduc, 2);
                        linha["TERC"] = TDataControlTrabalho.ptRound(trateio * terc, 2);
                        reduc = reduc - Convert.ToDecimal(linha["EDUC"]);
                        rterc = rterc - Convert.ToDecimal(linha["TERC"]);
                        if (trateio > maiorrat)
                        {
                            maiorrat = trateio;
                            pivot = linha;
                        }
                    }
                    linha["TIPO"] = "D";
                    linha.EndEdit();
                }
                if (pivot != null)
                {
                    DataRow linha = dtcltfolha.Rows[dtcltfolha.Rows.IndexOf(pivot)];
                    linha.BeginEdit();
                    if (reduc != 0)
                        linha["EDUC"] = Convert.ToDecimal(linha["EDUC"]) + reduc;
                    if (rterc != 0)
                        linha["TERC"] = Convert.ToDecimal(linha["TERC"]) + rterc;
                    linha.EndEdit();
                }
            }
        }


        private void grave_real_corrente(KeyValuePair<string, ClassTrab> otrab, TCLTGleba salmes, TCLTGleba horasmes, string tcentro, string tponto, Decimal tsal, Decimal dia_real,
                Int32 ndias_periodo, Int32 dias_trab, DataRow orowcltcad)
        {
            Decimal tfgts, tsalario, tsalfam, tinss, tferias, tdecimo, tsalariorateio, trateio, tirfonte, r_irfonte;
            Decimal tfgts_fe, rfgts_fe, tfgts_dec, rfgts_dec, tmulta_fgts, rmulta_fgts;
            Decimal tferias_inss, tdecimo_inss, r_fgts, r_salfam, r_inss, r_ferias, r_salario, r_decimo, maiorrateio;
            Decimal insssalcontrib;
            string trab_erro = "";
       //     string[] otrabs = new string[1] { "8616" };
       //     if (otrabs.Contains(otrab.Key))//== "1808")
        //        trab_erro = otrab.Key;
            Int16 ndep = Convert.ToInt16(orowcltcad["DEPEND"]);
            Int16 irdep = Convert.ToInt16(orowcltcad["IRDEPEND"]);
            //diasemferias = VerifiqueDiasemFerias(inicio, fim, Convert.ToDateTime(orowcltcad["GOZO_INI"]), Convert.ToDateTime(orowcltcad["GOZO_FIM"]));
            bool ferias = emferias(inicio, fim, Convert.ToDateTime(orowcltcad["GOZO_INI"]), Convert.ToDateTime(orowcltcad["GOZO_FIM"]));
            List<DataRow> regcentro = new List<DataRow>();
            List<DataRow> regsalario = new List<DataRow>();
            List<DataRow> reghxn = new List<DataRow>();
            List<DataRow> reghxa = new List<DataRow>();
            List<DataRow> reghxs = new List<DataRow>();
            if ((salmes.diarias + salmes.horasfe + salmes.horas50 + salmes.horas100) == 0.0M)
            {
                DataRow orow = dtcltfolha.NewRow();
                orow.BeginEdit();
                orow["TRAB"] = otrab.Key;
                orow["DATA"] = fim;
                orow["CENTRO"] = tcentro;
                orow["SETOR"] = glebasetoratual(tcentro,fim);//SetorAtualData(tcentro,fim);
                orow["PONTO"] = tponto;
                orow.EndEdit();
                dtcltfolha.Rows.Add(orow);
                return;
            }
            TCLTGleba rsalmes = new TCLTGleba();
            TCLTGleba vet_tot = new TCLTGleba();
            TCLTGleba vet_horas = new TCLTGleba();
            TCLTGleba vet_salario = new TCLTGleba();
            rsalmes.diarias = salmes.diarias;
            rsalmes.horasfe = salmes.horasfe;
            rsalmes.horas50 = salmes.horas50;
            rsalmes.horas100 = salmes.horas100;
            List<TCLTGleba> horasefetivas = new List<TCLTGleba>();
            foreach (KeyValuePair<string, ClassGleba> ovet2 in otrab.Value.Vet2)
            {
                TCLTGleba ocltgleba = new TCLTGleba();
                foreach (TCLTGleba ogleba in ovet2.Value.Semana)
                {
                    ocltgleba.diarias += ogleba.diarias;
                    ocltgleba.horasfe += ogleba.horasfe;
                    ocltgleba.horas50 += ogleba.horas50;
                    ocltgleba.diariasN += ogleba.diariasN;
                    ocltgleba.horasfeN += ogleba.horasfeN;
                    ocltgleba.horas50N += ogleba.horas50N;
                    ocltgleba.horas100 += ogleba.horas100;
                    ocltgleba.horas_efetivas += ogleba.horas_efetivas;
                    ocltgleba.vlrdiaria = ogleba.vlrdiaria;
                }
                if (ocltgleba.vlrdiaria == 0.0M) ocltgleba.vlrdiaria = dia_real;
                horasefetivas.Add(ocltgleba);
            }
            tsalariorateio = 0;
            foreach (TCLTGleba ogleba in horasefetivas)
            {
                if (ogleba.horas_efetivas != 0.0M)
                    tsalariorateio += ogleba.diarias;
            }
            tsalariorateio = (rsalmes.diarias - tsalariorateio);
            if (tsalariorateio < 0) tsalariorateio = 0;
            // para cada gleba
            foreach (KeyValuePair<string, ClassGleba> ovet2 in otrab.Value.Vet2)
            {
                vet_horas = new TCLTGleba();
                vet_salario = new TCLTGleba();
                foreach (TCLTGleba ogleba in ovet2.Value.Semana)
                {
                    vet_horas.diarias += ogleba.diarias;
                    vet_horas.horasfe += ogleba.horasfe;
                    vet_horas.horas50 += ogleba.horas50;
                    vet_horas.diariasN += ogleba.diariasN;
                    vet_horas.horasfeN += ogleba.horasfeN;
                    vet_horas.horas50N += ogleba.horas50N;
                    vet_horas.horas100 += ogleba.horas100;
                    vet_horas.horas_efetivas += ogleba.horas_efetivas;
                    vet_horas.quant += ogleba.quant;
                    if (ogleba.vlrdiaria == 0.00M) ogleba.vlrdiaria = dia_real;
                    vet_salario.diarias += TDataControlTrabalho.ptRound(((ogleba.diarias / 8) * ogleba.vlrdiaria), 2);        // horas normais
                    vet_salario.horasfe += domingo_hextra(ogleba.horasfe, tsal);        // horas em feriados
                    vet_salario.horas50 += normal_hextra(ogleba.horas50, tsal);        // horas extra 50 %
                    //vet_salario.horas100 += noturno_hextra(ogleba.horas100, tsal);       // horas extra 100 %
                    if (fim < new DateTime(2011, 12, 1).Date)
                        vet_salario.horas100 += noturno_hextra(ogleba.horas100, tsal);       // horas extra 100 %
                    else
                    {
                        if ((ogleba.diariasN + ogleba.horasfeN + ogleba.horas50N) > 0)
                        {
                            vet_salario.diariasN = TDataControlTrabalho.ptRound(((ogleba.diariasN / 8.0M) * dia_real), 2);
                            vet_salario.horasfeN = domingo_hextra(ogleba.horasfeN, tsal);        // horas em feriados
                            vet_salario.horas50N = normal_hextra(ogleba.horas50N, tsal);        // horas extra 50 %
                            vet_salario.horas100 += TDataControlTrabalho.ptRound(((vet_salario.diariasN + vet_salario.horasfeN + vet_salario.horas50N) * 0.25M), 2);
                        }
                    }
                }
                if ((vet_salario.diarias + vet_salario.horasfe + vet_salario.horas50 + vet_salario.horas100) != 0)
                {
                    DataRow orow = dtcltfolha.NewRow();
                    orow.BeginEdit();
                    orow["TRAB"] = otrab.Key;
                    orow["DATA"] = fim;
                    orow["HX"] = vet_horas.diarias;
                    orow["SALARIO"] = vet_salario.diarias;
                    orow["HXS"] = vet_horas.horasfe;
                    orow["VLR_HXS"] = vet_salario.horasfe;
                    orow["HXA"] = vet_horas.horas50;
                    orow["VLR_HXA"] = vet_salario.horas50;
                    orow["HXN"] = vet_horas.horas100;
                    orow["VLR_HXN"] = vet_salario.horas100;
                    orow["CENTRO"] = ovet2.Value.fazmov;
                    orow["SETOR"] = glebasetoratual(ovet2.Value.fazmov, fim);
                    orow["QUADRA"] = ovet2.Value.bl;
                    orow["NUM_MOD"] = ovet2.Value.num_mod;
                    orow["CODSER"] = ovet2.Value.codser;
                    orow["QUANT"] = vet_horas.quant;
                    orow["HEFETIVA"] = vet_horas.horas_efetivas;
                    orow["EDUC"] = 0.0;
                    orow["TERC"] = 0.0;
                    orow["FGTS"] = 0.0;

                    orow["DECIMO"] = 0.0;// Convert.ToDecimal(odatarow["DECIMO"]) + Convert.ToDecimal(odatarow["FGTS_DEC"]);
                    orow["FERIAS"] = 0.0;// Convert.ToDecimal(odatarow["FERIAS"]) + Convert.ToDecimal(odatarow["FGTS_FE"]);
                    orow["FGTS_DEC"] = 0.0;
                    orow["FGTS_FE"] = 0.0;
                    orow.EndEdit();
                    dtcltfolha.Rows.Add(orow);
                    rsalmes.diarias = rsalmes.diarias - Convert.ToDecimal(orow["SALARIO"]);
                    rsalmes.horasfe = rsalmes.horasfe - Convert.ToDecimal(orow["VLR_HXS"]);
                    rsalmes.horas50 = rsalmes.horas50 - Convert.ToDecimal(orow["VLR_HXA"]);
                    rsalmes.horas100 = rsalmes.horas100 - Convert.ToDecimal(orow["VLR_HXN"]);
                    vet_tot.diarias = vet_tot.diarias + Convert.ToDecimal(orow["SALARIO"]);
                    vet_tot.horasfe = vet_tot.horasfe + Convert.ToDecimal(orow["VLR_HXS"]);
                    vet_tot.horas50 = vet_tot.horas50 + Convert.ToDecimal(orow["VLR_HXA"]);
                    vet_tot.horas100 = vet_tot.horas100 + Convert.ToDecimal(orow["VLR_HXN"]);
                    //
                    //dtcltfolha.Rows.
                    regcentro.Add(orow);
                    if (Convert.ToDecimal(orow["SALARIO"]) != 0.0M)
                        regsalario.Add(orow);
                    if (Convert.ToDecimal(orow["VLR_HXS"]) != 0.0M)
                        reghxs.Add(orow);
                    if (Convert.ToDecimal(orow["VLR_HXA"]) != 0.0M)
                        reghxa.Add(orow);
                    if (Convert.ToDecimal(orow["VLR_HXN"]) != 0.0M)
                        reghxn.Add(orow);

                }
            }
            if ((regsalario.Count > 0) && (rsalmes.diarias != 0.0M))
            {
                Int32 irow = dtcltfolha.Rows.IndexOf(regsalario[regsalario.Count - 1]);
                DataRow orow = dtcltfolha.Rows[irow];
                orow.BeginEdit();
                orow["SALARIO"] = Convert.ToDecimal(orow["SALARIO"]) + rsalmes.diarias;
                orow.EndEdit();
                vet_tot.diarias = vet_tot.diarias + rsalmes.diarias;
            }
            if ((reghxs.Count > 0) && (rsalmes.horasfe != 0.0M))
            {
                Int32 irow = dtcltfolha.Rows.IndexOf(reghxs[reghxs.Count - 1]);
                DataRow orow = dtcltfolha.Rows[irow];
                orow.BeginEdit();
                orow["vlr_hxs"] = Convert.ToDecimal(orow["vlr_hxs"]) + rsalmes.horasfe;
                orow.EndEdit();
                vet_tot.horasfe = vet_tot.horasfe + rsalmes.horasfe;
            }
            if ((reghxa.Count > 0) && (rsalmes.horas50 != 0.0M))
            {
                Int32 irow = dtcltfolha.Rows.IndexOf(reghxa[reghxa.Count - 1]);
                DataRow orow = dtcltfolha.Rows[irow];
                orow.BeginEdit();
                orow["vlr_hxa"] = Convert.ToDecimal(orow["vlr_hxa"]) + rsalmes.horas50;
                orow.EndEdit();
                vet_tot.horas50 = vet_tot.horas50 + rsalmes.horas50;
            }
            if ((reghxn.Count > 0) && (rsalmes.horas100 != 0.0M))
            {
                Int32 irow = dtcltfolha.Rows.IndexOf(reghxn[reghxn.Count - 1]);
                DataRow orow = dtcltfolha.Rows[irow];
                orow.BeginEdit();
                orow["vlr_hxn"] = Convert.ToDecimal(orow["vlr_hxn"]) + rsalmes.horas100;
                orow.EndEdit();
                vet_tot.horas100 = vet_tot.horas100 + rsalmes.horas100;
            }
            r_salario = salmes.diarias + salmes.horasfe + salmes.horas50 + salmes.horas100;
            tsalario = r_salario;
            tferias_inss = TDataControlTrabalho.ptRound(((tsalario / 12) * 1.333M), 2);
            tferias = tferias_inss + TDataControlTrabalho.ptRound((0.027M * tferias_inss), 2);
            tdecimo_inss = TDataControlTrabalho.ptRound((tsalario / 12), 2);
            tdecimo = tdecimo_inss + TDataControlTrabalho.ptRound((0.027M * tdecimo_inss), 2);
            /// ENTRAR O CONCEITO DE SALARIO CONTRIBUIÇAO
            // O SEJA SE O EMPREGADO NESTE MES TEVE OGOZO DE FERIAS //
            // O VALOR DAS FERIAS // O INSS TEM QUE SER RECALCULADO E
            // A DIFERENCA LANCADO NO SALARIO CONTRIBUICAO
          // if ((otrab.Key == "3643") || (otrab.Key == "3643"))
           //    r_inss = 0;
            tinss = TDataControlTrabalho.CalcInss(fim, tsalario, dsTrabalhistas.Tables["TABINSS"]);
            r_inss = tinss;
            tirfonte = TDataControlTrabalho.CalcIRF(fim, tsalario, tinss, irdep, dsTrabalhistas.Tables["TABIR"], dsTrabalhistas.Tables["IRDEDU"]);
            r_irfonte = tirfonte;
            tfgts = TDataControlTrabalho.ptTruncRound(((fgts / 100) * tsalario), 2);
            r_fgts = tfgts;
            tsalfam = 0;
            if (!ferias)
            {
                if (ndep != 0)
                {
                    tsalfam = TDataControlTrabalho.CalcSalFam(fim, tsalario, ndep, dsTrabalhistas.Tables["SALFAM"]);
                    if (dias_trab != 0)
                        tsalfam = TDataControlTrabalho.ptRound(((dias_trab / Convert.ToDecimal(ndias_periodo)) * tsalfam), 2);
                }
            }
            else
            {
                if (fim.CompareTo(Convert.ToDateTime("01/04/2009")) >= 0)
                {
                    TDataControlTrabalho.informeferias ovaloresferias = TDataControlTrabalho.UltimaFeriasGozadas(otrab.Key,
                                                      fim, dscltcad.Tables["FERIAS"]);
                    // verifica se a faixa de contribuição p/ desc.INSS mudou!
                    insssalcontrib = TDataControlTrabalho.CalcInss(fim, (ovaloresferias.salcontrib + tsalario), dsTrabalhistas.Tables["TABINSS"]);
                    if (insssalcontrib > (ovaloresferias.inss + tinss))
                    {           // analise a diferença à maior
                        insssalcontrib = insssalcontrib - (ovaloresferias.inss + tinss);
                        if (insssalcontrib > 0.02M)   // se a dif.for maior q 0,02
                        {
                            tinss = tinss + insssalcontrib;
                            r_inss = tinss;
                        }
                    }
                    // calcula se o salario familia mudou de faixa em função do salario contrib. (ferias+salario)
                    if (ndep > 0)
                    {
                        tsalfam = TDataControlTrabalho.CalcSalFam(fim, (tsalario + ovaloresferias.salcontrib), ndep, dsTrabalhistas.Tables["SALFAM"]);
                        if (tsalfam > ovaloresferias.salfam)
                            // diferença a mais pago no salfam . oque fazer???
                            tsalfam = (tsalfam - ovaloresferias.salfam);
                        else
                            tsalfam = 0;
                    }
                }
            }
            r_salfam = tsalfam;
            r_ferias = tferias;
            r_decimo = tdecimo;
            // provisionamento do fgts sobre a provisao de ferias e decimo
            tfgts_fe = TDataControlTrabalho.ptTruncRound(((fgts / 100) * tferias), 2); //valor_fgts(tferias_inss, fim);
            rfgts_fe = tfgts_fe;
            tfgts_dec = TDataControlTrabalho.ptTruncRound(((fgts / 100) * tdecimo_inss), 2); //valor_fgts(tdecimo_inss, fim);
            rfgts_dec = tfgts_dec;
            tmulta_fgts = TDataControlTrabalho.ptRound(((tfgts + tfgts_fe + tfgts_dec) * 0.40M), 2);
            rmulta_fgts = tmulta_fgts;
            maiorrateio = 0;
            DataRow pivot = null;
            foreach (DataRow indorow in regcentro)
            {
                DataRow orow = dtcltfolha.Rows[dtcltfolha.Rows.IndexOf(indorow)];
                trateio = (Convert.ToDecimal(orow["SALARIO"]) + Convert.ToDecimal(orow["VLR_HXA"]) + Convert.ToDecimal(orow["VLR_HXS"]) +
                   Convert.ToDecimal(orow["VLR_HXN"])) / tsalario;
               
                orow.BeginEdit();
                orow["FGTS"] = TDataControlTrabalho.ptRound(trateio * tfgts, 2);
                orow["SALFAM"] = TDataControlTrabalho.ptRound(trateio * tsalfam, 2);
                orow["INSS"] = TDataControlTrabalho.ptRound(trateio * tinss, 2);
                orow["IRFONTE"] = TDataControlTrabalho.ptRound(trateio * tirfonte, 2);
                orow["FERIAS"] = TDataControlTrabalho.ptRound(trateio * tferias, 2);
                orow["DECIMO"] = TDataControlTrabalho.ptRound(trateio * tdecimo, 2);
                orow["FGTS_FE"] = TDataControlTrabalho.ptRound(trateio * tfgts_fe, 2);
                orow["FGTS_DEC"] = TDataControlTrabalho.ptRound(trateio * tfgts_dec, 2);
                orow["MULTA_FGTS"] = TDataControlTrabalho.ptRound(trateio * tmulta_fgts, 2);
                orow.EndEdit();
                if (maiorrateio < trateio)
                {
                    maiorrateio = trateio;
                    pivot = indorow;
                }
                r_fgts = r_fgts - Convert.ToDecimal(orow["fgts"]);
                r_salfam = r_salfam - Convert.ToDecimal(orow["salfam"]);
                r_inss = r_inss - Convert.ToDecimal(orow["inss"]);
                r_irfonte = r_irfonte - Convert.ToDecimal(orow["irfonte"]);
                r_ferias = r_ferias - Convert.ToDecimal(orow["ferias"]);
                r_decimo = r_decimo - Convert.ToDecimal(orow["decimo"]);
                rfgts_fe = rfgts_fe - Convert.ToDecimal(orow["fgts_fe"]);
                rfgts_dec = rfgts_dec - Convert.ToDecimal(orow["fgts_dec"]);
                rmulta_fgts = rmulta_fgts - Convert.ToDecimal(orow["multa_fgts"]);
            }
            if (pivot != null)
            {
                DataRow orow = dtcltfolha.Rows[dtcltfolha.Rows.IndexOf(pivot)];
                orow.BeginEdit();
                orow["FGTS"] = Convert.ToDecimal(orow["FGTS"]) + r_fgts;
                orow["INSS"] = Convert.ToDecimal(orow["INSS"]) + r_inss;
                orow["IRFONTE"] = Convert.ToDecimal(orow["IRFONTE"]) + r_irfonte;
                orow["SALFAM"] = Convert.ToDecimal(orow["SALFAM"]) + r_salfam;
                orow["FERIAS"] = Convert.ToDecimal(orow["FERIAS"]) + r_ferias;
                orow["DECIMO"] = Convert.ToDecimal(orow["DECIMO"]) + r_decimo;
                orow["FGTS_FE"] = Convert.ToDecimal(orow["FGTS_FE"]) + rfgts_fe;
                orow["FGTS_DEC"] = Convert.ToDecimal(orow["FGTS_DEC"]) + rfgts_dec;
                orow["MULTA_FGTS"] = Convert.ToDecimal(orow["MULTA_FGTS"]) + rmulta_fgts;
                orow["PONTO"] = tponto;
                orow.EndEdit();
            }
        }

        private void Atualiza_Adiant()
        {
            Int32 ndias_periodo, dias_uteis, dias_trab;
            string tponto;
            Decimal tdias, tsal, dia_real;
            DataSet dsReajustes = TDataControlTrabalho.Get_CltReajuste(inicio, fim, null, "");
            dias_uteis = 0;
            foreach (KeyValuePair<DateTime, TObjPonto> k_ponto in ListPonto.AsEnumerable())
                dias_uteis += k_ponto.Value.ndias;
            ndias_periodo = (fim.Date - inicio.Date).Days + 1;

            Decimal tperc =   TDataControlTrabalho.ptRound(( perc_adiant / ((Convert.ToDecimal(dias_uteis) / Convert.ToDecimal(ndias_periodo))*100)), 5);

            foreach (KeyValuePair<string, ClassTrab> oTrab in ListaTrab.AsEnumerable())
            {

                DataRow oRowcltcad = dscltcad.Tables["CLTCAD"].Rows.Find(oTrab.Key);
                if (oRowcltcad == null) continue;//mande mensagem... isto não deve ocorrer

                bool ferias = emferias(inicio, fim, Convert.ToDateTime(oRowcltcad["GOZO_INI"]), Convert.ToDateTime(oRowcltcad["GOZO_FIM"]));
                if (ferias)
                    continue;
                //esta_em_ferias = false;
                tponto = "";
                foreach (TObjVet3 ovet3 in oTrab.Value.Vet3)
                {
                    tponto += ovet3.ponto;
                }
                // estemeshoras é isto aqui
                TCLTGleba vet_horas = new TCLTGleba();
                foreach (KeyValuePair<string, ClassGleba> ovet2 in oTrab.Value.Vet2)
                {
                    foreach (TCLTGleba ogleba in ovet2.Value.Semana)
                    {
                        vet_horas.diarias += ogleba.diarias;
                        vet_horas.horasfe += ogleba.horasfe;
                        vet_horas.horas50 += ogleba.horas50;
                        vet_horas.diariasN += ogleba.diariasN;
                        vet_horas.horasfeN += ogleba.horasfeN;
                        vet_horas.horas50N += ogleba.horas50N;
                        vet_horas.horas100 += ogleba.horas100;
                        vet_horas.horas_efetivas += ogleba.horas_efetivas;
                    }
                }
                tdias = TDataControlTrabalho.ptRound((vet_horas.diarias / 8), 2);
                string trab_erro;
            //    string[] otrabs = new string[2] { "8598","7210" };
             //   if (otrabs.Contains(oTrab.Key))//== "1808")
              //      trab_erro = oTrab.Key;

                if ((tdias != 0) || (tponto != "") || (vet_horas.horasfe != 0) ||
                    (vet_horas.horas50 != 0) || (vet_horas.horas100 != 0) || (tponto != ""))
                {
                    tsal = 0;
                    //tdep = 0;
                    dias_trab = 0;
                    if (Convert.ToDateTime(oRowcltcad["DEMI"]).ToString() != TDataControlReduzido.DataVazia())
                    {
                        if ((Convert.ToDateTime(oRowcltcad["DEMI"]).CompareTo(fim) <= 0) &&
                            (Convert.ToDateTime(oRowcltcad["DEMI"]).CompareTo(inicio) >= 0))
                        {
                            dias_trab = (Convert.ToDateTime(oRowcltcad["DEMI"]).Date - inicio.Date).Days + 1;
                        }
                    }
                    if ((Convert.ToDateTime(oRowcltcad["ADMI"]).CompareTo(inicio) >= 0))
                    {
                        dias_trab = (fim.Date - Convert.ToDateTime(oRowcltcad["ADMI"]).Date).Days + 1;
                    }
                
                    tsal = TDataControlTrabalho.CalcSalBase(oTrab.Key, Convert.ToDateTime(oRowcltcad["ADMI"]),
                              Convert.ToDecimal(oRowcltcad["SALBASE"]), Convert.ToDecimal(salmin), fim, dsReajustes.Tables[0]);

                    TCLTGleba vet_salario = new TCLTGleba();
                    // para os nao demitidos
                    dia_real = TDataControlTrabalho.ptRound(tsal / ndias_periodo, 4);
             
                    // calculo de horas extras simples (ou seja trabalho no domingo e feriados)

                    //if ((vet_horas.diarias / 8.0M) == ndias_periodo)
                        vet_salario.diarias = TDataControlTrabalho.ptRound(((vet_horas.diarias / 8.0M) * dia_real), 2);
                   // else
                     //   vet_salario.diarias = TDataControlTrabalho.ptRound(((vet_horas.diarias / 8.0M) * dia_real), 2);        // horas normais
                    vet_salario.horasfe = domingo_hextra(vet_horas.horasfe, tsal);        // horas em feriados
                    vet_salario.horas50 = normal_hextra(vet_horas.horas50, tsal);        // horas extra 50 %
                    if (fim < new DateTime(2011, 12, 1))
                        vet_salario.horas100 = noturno_hextra(vet_horas.horas100, tsal);       // horas extra 100 %
                    else
                    {
                        if ((vet_horas.diariasN + vet_horas.horasfeN + vet_horas.horas50N) > 0)
                        {
                            vet_salario.diariasN = TDataControlTrabalho.ptRound(((vet_horas.diariasN / 8.0M) * dia_real), 2);
                            vet_salario.horasfeN = domingo_hextra(vet_horas.horasfeN, tsal);        // horas em feriados
                            vet_salario.horas50N = normal_hextra(vet_horas.horas50N, tsal);        // horas extra 50 %
                            vet_salario.horas100 = TDataControlTrabalho.ptRound(((vet_salario.diariasN + vet_salario.horasfeN + vet_salario.horas50N) * 0.25M), 2);
                        }
                    }
                                
                   // vet_salario.horas100 = noturno_hextra(vet_horas.horas100, tsal);       // horas extra 100 %
                    Decimal tvl = vet_salario.diarias + vet_salario.horasfe + vet_salario.horas50 + vet_salario.horas100;

                    DataRow orow = dtcltadian.NewRow();
                    orow.BeginEdit();
                    orow["TRAB"] = oTrab.Key;
                    orow["DATA"] = datadigitada;
                    orow["HX"] = vet_horas.diarias;

                    orow["TIPO"] = "D";
                    orow["CENTRO"] = oRowcltcad["GLECAD"];
                    orow["SETOR"] = glebasetoratual(orow["CENTRO"].ToString(), datadigitada);
                    orow["PONTO"] = tponto;
                    if ((dias_uteis > (vet_horas.diarias / 8)) &&  (tperc > 1))
                        orow["ADIANT"] = TDataControlTrabalho.ptRound((tvl * 0.92M), 2);
                    else
                        orow["ADIANT"] = TDataControlTrabalho.ptRound((tvl * tperc), 2);
                    orow.EndEdit();
                    dtcltadian.Rows.Add(orow);
                }
            }
        }

        private Int32 VerifiqueDiasemFerias(DateTime inicio, DateTime fim, DateTime inigozo, DateTime fimgozo)
        {
            if (fimgozo.CompareTo(inicio) < 0) return 0;
            if (inigozo.CompareTo(fim) > 0) return 0;
            DateTime oinicio, ofim;
            if (inigozo.CompareTo(inicio) > 0)
                oinicio = inigozo;
            else
                oinicio = inicio;
            if (fimgozo.CompareTo(fim) < 0)
                ofim = fimgozo;
            else
                ofim = fim;
            return (ofim - oinicio).Days;
        }
        private bool emferias(DateTime inicio, DateTime fim, DateTime inigozo, DateTime fimgozo)
        {
            if (fimgozo.Date.CompareTo(inicio.Date) < 0) return false;
            if (inigozo.Date.CompareTo(fim.Date) > 0) return false;

            if ((inigozo.Date.CompareTo(inicio.Date) >= 0) && (inigozo.Date.CompareTo(fim.Date) <= 0))
            {
                if (fimgozo.Date.CompareTo(fim.Date) <= 0)
                    return true;
                if ((fim.Date - inigozo.Date).Days < 10) return false;
                return true;
            }
            return false;
        }


        public static DataTable ProvQuadra()
        {
            DataTable result = new DataTable("PVQUEM1");
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "SETOR", 2, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "CENTRO", 4, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "BL", 3, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "TIPO", 1, false));   // "D"/"E"
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "NUM_MOD", 3, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(string), "CODSER", 3, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "PINICIO", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "PFIM", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "DTAINICIO", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(DateTime), "DTAFIM", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Decimal), "HORAS", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Decimal), "DIARIAS", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Decimal), "QUANT", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Decimal), "PRODUT", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Decimal), "AREA", 0, false));
            result.Columns.Add(TDataControlReduzido.Coluna(typeof(Decimal), "PRODAREA", 0, false));
            return result;
        }


        public bool GraveCLTFOLHA()
        {
            if ((fgrave == false) || (tipo == "A"))
                return false;
            OleDbDataAdapter oadapater = ClassCLTFOLHA.CLTFOLHAConstruaAdaptador("TRABALHO");
            oadapater.DeleteCommand.Parameters[0].Value = inicio;
            oadapater.DeleteCommand.Parameters[1].Value = fim;
            oadapater.DeleteCommand.Prepare();
            oadapater.DeleteCommand.ExecuteNonQuery();
            dtcltfolha.TableName = "CLTFOLHA";
            oadapater.Update(dtcltfolha);
            oadapater.Dispose();
            fgrave = false;
            OnAlteraGrave(new PermiteGravar(fgrave));
            return true;
        }

        public bool GraveCLTADIANT()
        {
            if ((fgrave == false) || (tipo != "A"))
                return false;
            OleDbDataAdapter oadapater = ClassCLTADIAN.CLTADIANConstruaAdaptador("TRABALHO");
            oadapater.DeleteCommand.Parameters[0].Value = inicio;
            oadapater.DeleteCommand.Parameters[1].Value = fim;
            oadapater.DeleteCommand.Prepare();
            oadapater.DeleteCommand.ExecuteNonQuery();
            dtcltadian.TableName = "CLTADIAN";
            oadapater.Update(dtcltadian);
            oadapater.Dispose();
            fgrave = false;
            OnAlteraGrave(new PermiteGravar(fgrave));
            return true;
        }


    }

  

    
}
