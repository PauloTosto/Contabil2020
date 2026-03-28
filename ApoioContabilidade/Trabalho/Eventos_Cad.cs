using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho
{
    public class Eventos_Cad
    {
        public BindingSource bmMestre;
        public BindingSource bmSalarios;
        //public BindingSource bmDetalhe;
        // public BindingSource bmCorrente;
        // public DataSet dsFiltrado;

        public DataTable tabCltCad = null;
        public DataTable tabCltFilhos = null;
        public DataTable tabCltFilhosIRF = null;
        public DataTable tabSalario = null;
        public DataTable tabReajuste = null;
        public DataTable tabObsSefip = null;


        public DataTable ReajusteVirtual = null;

        public DataSet dsSalarios = null;
        public DateTime dataRelatorio;
        public Eventos_Cad()
        {
            // dsFiltrado = odsFiltrado;
            bmMestre = new BindingSource();
            bmSalarios = new BindingSource();

        }

        public async Task<bool> Salarios_Acesso(DateTime data)
        {
            List<string> lst = new List<string>();
            string cltCad = "SElect * from cltcad";
            DateTime limiteData = data.AddYears(-14);
            string strFilhos = "SElect *   from cltfilho";
            //where SUBString(DTOS(NASC),1,6) >='" +
            // limiteData.ToString("yyyyMM") + "'";
            string strfilhosdependIr =
                "SELECT * FROM IRDEPEND WHERE  ( (substring(dtos(DATA), 1, 6) <> '') and (substring(dtos(DATA), 1, 8) < '" +
                  data.ToString("yyyyMMdd") + "') ) ORDER BY DATA";

            string tabSal = "SELECT * FROM TABSAL ORDER BY DATA";
            string reaj = "SELECT CLTREAJ.*, "
                   + " iif(ULT_ATUAL is null, Admi, ULT_ATUAL) as ult_atual , cltcad.salbase " +
                 " FROM         REAJUSTE as CLTREAJ, cltcad " +
                 " WHERE        CLTREAJ.trab = cltcad.codcad AND (CLTREAJ.trab <> '') AND "
                 + " (demi is null AND cltcad.prazo is null) "
                 + " and " + " (CLTREAJ.data <=  CTOD('" + data.ToString("MM/dd/yyyy") + "')) " + " ORDER BY CLTREAJ.trab, CLTREAJ.data ";

            /*
            string reaj = "SELECT TRAB, PERCCONF, REAJ_CALC, MIN_CALC,"
            + "RESIDUO, iif(ULT_ATUAL is null, Admi, ULT_ATUAL) as ult_atual,cltreaj.[DATA] "
             + " FROM REAJUSTE as CLTREAJ, cltcad "
            + " WHERE   CLTREAJ.trab = cltcad.codcad AND(CLTREAJ.trab <> '') AND "
            + " ((demi is null  AND cltcad.prazo is null) OR "
            + " (Format(cltcad.demi, 'yyyyMM') >= " + data.ToString("yyyyMM") + ")) "
            + " 	and (admi <= " + data.ToString("yyyyMMdd") + ") "
            + " and(cltreaj.[data] <= " + data.ToString("yyyyMMdd") + ") "
            + " ORDER BY CLTREAJ.trab, CLTREAJ.[DATA] ";
            */



            string obssefip = "SELECT * FROM OBSSEFIP";

            lst.Add(cltCad);
            lst.Add(strFilhos);
            lst.Add(strfilhosdependIr);
            lst.Add(tabSal);
            lst.Add(reaj);
            lst.Add(obssefip);


            dsSalarios = null;
            try
            {
                dsSalarios = await ApiServices.Api_QueryMulti(lst);

            }
            catch (Exception E)
            {
                MessageBox.Show("Erro Acesso Banco de Dados " + E.Message);
                return false;
            }
            if ((dsSalarios == null) || (dsSalarios.Tables.Count == 0))
            {
                MessageBox.Show("Erro Tabelas CLTReaj ou TABSAL não encontradas");
                return false;
            }
            tabCltCad = dsSalarios.Tables[0].Copy();
            tabCltCad.TableName = "CLTCAD";
            tabCltCad.AcceptChanges();
            tabCltFilhos = dsSalarios.Tables[1].Copy();
            tabCltFilhos.TableName = "CLTFILHO";
            tabCltFilhos.AcceptChanges();
            tabCltFilhosIRF = dsSalarios.Tables[2].Copy();
            tabCltFilhosIRF.TableName = "IRDEPEND";
            tabCltFilhosIRF.AcceptChanges();
            tabSalario = dsSalarios.Tables[3].Copy();
            tabSalario.TableName = "TABSAL";
            tabSalario.AcceptChanges();
            tabReajuste = dsSalarios.Tables[4].Copy();
            tabReajuste.TableName = "REAJUSTE";
            tabReajuste.AcceptChanges();

            tabObsSefip = dsSalarios.Tables[5].Copy();
            tabObsSefip.TableName = "OBSSEFIP";
            tabObsSefip.AcceptChanges();


            /*   query depreciado...       
                          str := 'SELECT  CLTREAJ.TRAB,CLTREAJ.DATA,CLTREAJ.REAJ as REAJ_CALC,0 as MIN_CALC,0 as RESIDUO,0 AS PERCCONF, cltcad.ult_atual, cltcad.salbase '
                            + ' FROM         CLTREAJ, cltcad ' +
                            ' WHERE        CLTREAJ.trab = cltcad.codcad AND (CLTREAJ.trab <> '''') AND '
                            + ' ((alltrim(dtos(cltcad.demi)) = '''') AND (alltrim(dtos(cltcad.prazo)) = '''')) '
                            + ' and ' + ' (CLTREAJ.data <=  CTOD(''' + formatDateTime('mm/dd/yyyy',
                            dtData1.Date) + ''')) ' + ' ORDER BY CLTREAJ.trab, CLTREAJ.data ';
            */
            return true;
        }
        public void BmMestre_PositionChanged(object sender, EventArgs e)
        {
            //BindingSource bindingSource = 
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);

            bool result = (registro != null);

            if (result) { result = !registro.Row.IsNull("CODCAD"); }
            if (result)
            {

            }
            else
            {

            }
            /*try
    with DataSet do
    begin
      obeforepost := BeforePost;
      oafterpost := AfterPost;
      BeforePost := nil;
      AfterPost := nil;
      adoCLtFilhosMenores.Filter := 'CODCAD = ''' + FieldByName('CODCAD')
        .asstring + '''';
      adoCLtFilhosMenores.Filtered := true;
      Edit;
      // FieldByName('DependCalc').asInteger :=adoCLtFilhosMenores.RecordCount;
      FieldByName('Depend').asInteger := adoCLtFilhosMenores.RecordCount;
      Post;
      adoCLtFilhosMenores.Filtered := false;

      adocltDependIR.Filter := 'CODCAD = ''' + FieldByName('CODCAD')
        .asstring + '''';
      adocltDependIR.Filtered := true;
      if adocltDependIR.RecordCount > 0 then
      begin
        adocltDependIR.Last;
        Edit;
        // FieldByName('IRDEPENDCalc').asInteger :=adocltDependIR.FieldByName('NDEPEND').AsInteger;
        FieldByName('IRDEPEND').asInteger := adocltDependIR.FieldByName
          ('NDEPEND').asInteger;
        Post;
      end
      else
      begin
        Edit;
        // FieldByName('IRDEPENDCalc').asInteger :=0;
        FieldByName('IRDEPEND').asInteger := 0;
        Post;
      end;
      adocltDependIR.Filtered := false;
    end;
  finally
    with DataSet do
    begin
      BeforePost := obeforepost;
      AfterPost := oafterpost;
    end;
  end;

             */
        }
        private double ValorSalMin(DateTime data)
        {
            double valor = 0;
            DataRow dado = tabSalario.AsEnumerable().OrderBy(row => row.Field<DateTime>("DATA")).Where(row =>
                 row.Field<DateTime>("DATA").ToString("yyyyMM").CompareTo(data.ToString("yyyyMM")) <= 0
                 // row.Field<DateTime>("DATA").Year <= data.Year
                 // && row.Field<DateTime>("DATA").Month <= data.Month
                 ).LastOrDefault();
            if (dado == null)
            {
                MessageBox.Show("Informe Sal.Min em " + data.ToString("d"));
            }
            else
            {
                valor = Convert.ToDouble(dado["VALOR"]);
            }
            return valor;
        }


        /*   First;
           while not eof and(formatDateTime('yyyymm', FieldbyName('DATA').AsDateTime)
     > formatDateTime('yyyymm', tdata)) do
               next;

   if eof then
     ShowMessage('Informe Sal Minimo')
   else
           result:= FieldbyName('VALOR').asFloat;
       }
        */

        public void ConstrucaoTabelaReajustesSalariosUnico(DateTime data)
        {
            DataRowView orowMestre = (bmMestre.Current as DataRowView);
            if (orowMestre == null) return;
            DateTime ultimodiaMes = new DateTime(data.Year, data.Month, 01).AddMonths(1).AddDays(-1);
            ReajusteVirtual = tabReajuste.Clone();
            double fator = Convert.ToDouble(orowMestre["SALBASE"]);
            double salbase = fator;
            DateTime tadmi = Convert.ToDateTime(orowMestre["ADMI"]);
            DateTime tfim = data;
            if (!(orowMestre.Row.IsNull("DEMI")) && (Convert.ToDateTime(orowMestre["DEMI"]).CompareTo(tfim) < 0))
                tfim = Convert.ToDateTime(orowMestre["DEMI"]);
            var dados_salmin =
                 tabSalario.AsEnumerable().Where(row =>
                 (row.Field<DateTime>("DATA").CompareTo(new DateTime(tadmi.Year, tadmi.Month, 01)) >= 0)
                 && (row.Field<DateTime>("DATA").CompareTo(ultimodiaMes) <= 0)
                 ).OrderBy(row => row.Field<DateTime>("DATA"));

            string trab = orowMestre["CODCAD"].ToString();
            double tsal = 0;
            if (fator == 0)
                fator = 1;
            double tperc_confianca = 0;
            double salmin_calc = 0;
            double residuo = 0;
            bool basecalc_salariomin = false;
            double ultimofator = 0;
            double ultimoresiduo = 0;
            double sal_Min = 0;
            foreach (DataRow orowReaj in tabReajuste.AsEnumerable().Where(row => (!row.IsNull("TRAB")) &&
                    (row.Field<string>("TRAB").Trim() == trab)).OrderBy(row => row.Field<DateTime>("DATA")))
            {
                tperc_confianca = Convert.ToDouble(orowReaj["PERCCONF"]);
                if (tperc_confianca < 0)
                {
                    tperc_confianca = tperc_confianca * -1;
                }
                double tperc_reaj = Convert.ToDouble(orowReaj["REAJ_CALC"]);
                tperc_reaj = tperc_reaj + tperc_confianca;

                fator = Math.Round(fator * (1 + (tperc_reaj / 100)), 4);

                fator = Math.Round(fator * (1 + (Convert.ToDouble(orowReaj["REAJ_CALC"]) / 100)), 4);

                if (Convert.ToDouble(orowReaj["MIN_CALC"]) != 0)
                {
                    salmin_calc = Convert.ToDouble(orowReaj["MIN_CALC"]);
                }
                if ((Convert.ToDouble(orowReaj["MIN_CALC"]) == 0) && (Convert.ToDouble(orowReaj["REAJ_CALC"]) == 0))
                {
                    salmin_calc = 0;
                    residuo = 0;
                }
                if (Convert.ToDouble(orowReaj["RESIDUO"]) != 0)
                {
                    residuo = Convert.ToDouble(orowReaj["RESIDUO"]);
                }
                DataRow orowVirtual = ReajusteVirtual.NewRow();
                foreach (DataColumn ocol in orowReaj.Table.Columns)
                {
                    orowVirtual[ocol.ColumnName] = orowReaj[ocol.ColumnName];

                }
                if (Convert.ToDouble(orowVirtual["REAJ_CALC"]) != 0)
                {
                    orowVirtual["REAJ"] = orowVirtual["REAJ_CALC"];
                }
                if (salmin_calc != 0)
                {
                    tsal = Math.Round(fator * salmin_calc, 2);
                    basecalc_salariomin = false;
                }
                else
                {
                    sal_Min = ValorSalMin(Convert.ToDateTime(orowVirtual["DATA"]));
                    tsal = Math.Round(fator * sal_Min, 2);
                    basecalc_salariomin = true;
                }
                // para o calculo dos que são reajustados pelo salário min
                ultimofator = fator;
                ultimoresiduo = residuo;

                tsal = tsal + residuo;

                orowVirtual["VLRSALBASE"] = tsal;

                orowVirtual["VLRSALMIN"] = sal_Min;

                ReajusteVirtual.Rows.Add(orowVirtual);
                orowVirtual.AcceptChanges();
            }
            var ultSalmin = dados_salmin.LastOrDefault();
            tfim = Convert.ToDateTime(ultSalmin["DATA"]);
            bool houveAlgumReajuste = false;
            if (ReajusteVirtual.Rows.Count > 0)
            {
                houveAlgumReajuste = true;
            }
            foreach (DataRow orow in dados_salmin)
            {
                if (houveAlgumReajuste)
                {
                    if (basecalc_salariomin &&
                      (Convert.ToDateTime(orow["DATA"]).Year == tfim.Year)
                        &&
                      (Convert.ToDateTime(orow["DATA"]).Month == tfim.Month))
                    {
                        DateTime primeiroDia = new DateTime(Convert.ToDateTime(orow["DATA"]).Year, Convert.ToDateTime(orow["DATA"]).Month, 01);
                        DateTime ultimoDia = primeiroDia.AddMonths(1).AddDays(-1);
                        DataRow orowVirtual;
                        try
                        {
                            orowVirtual = ReajusteVirtual.AsEnumerable().Where(row =>
                             (row.Field<DateTime>("DATA").CompareTo(primeiroDia) >= 0)
                          && (row.Field<DateTime>("DATA").CompareTo(ultimoDia) <= 0)).FirstOrDefault();

                            if (orowVirtual == null)
                            {
                                orowVirtual = ReajusteVirtual.NewRow();
                                orowVirtual["TRAB"] = orowMestre["CODCAD"];
                                orowVirtual["DATA"] = orow["DATA"];
                                orowVirtual["VLRSALMIN"] = orow["VALOR"];
                                orowVirtual["VLRSALBASE"] = Math.Round(Convert.ToDouble(orow["VALOR"]) * ultimofator, 2);
                                ReajusteVirtual.Rows.Add(orowVirtual);
                                orowVirtual.AcceptChanges();
                            }
                            /*else
                            {
                                orowVirtual.BeginEdit();
                                orowVirtual["VLRSALMIN"] = orow["VALOR"];
                                orowVirtual.EndEdit();

                            }*/


                        }
                        catch (Exception E)
                        {
                            MessageBox.Show("Erro " + E.Message);
                        }

                    }
                }
                else
                {
                    DataRow orowVirtual = ReajusteVirtual.NewRow();
                    orowVirtual["TRAB"] = orowMestre["CODCAD"];
                    orowVirtual["DATA"] = orow["DATA"];
                    orowVirtual["VLRSALMIN"] = orow["VALOR"];
                    orowVirtual["VLRSALBASE"] = Math.Round(Convert.ToDouble(orow["VALOR"]) * fator, 2);

                    ReajusteVirtual.Rows.Add(orowVirtual);
                    orowVirtual.AcceptChanges();
                }
            }
        }
        public void DtCadastro_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["CODCAD"] = "";
            e.Row["NOMECAD"] = "";
            e.Row["SETOR"] = "";
            e.Row["GLECAD"] = "";
            e.Row["MENSALISTA"] = "";

            e.Row["ADMI"] = DateTime.Now.AddDays(-1);
            e.Row["DEMI"] = Convert.DBNull;
            e.Row["PRAZO"] = Convert.DBNull;
        }

        public async void Cadastro__AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            // abril 2021
            if ((Convert.ToDouble(orow["SALBASE"]) < 0)
               || (orow["SETOR"].ToString().Trim() == "")
               || (orow["CODCAD"].ToString().Trim() == "")
               || (orow["GLECAD"].ToString().Trim() == "")
               || (orow.IsNull("ADMI"))
               || (orow.IsNull("NASC"))
               || (Convert.ToDateTime(orow["ADMI"]).CompareTo(Convert.ToDateTime(orow["NASC"])) <= 0)
               )
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            foreach (DataColumn col in orow.Table.Columns)
            {
                if (orow[col.ColumnName] is System.DBNull)
                {
                    orow[col.ColumnName] = col.DefaultValue;
                }
            }
            bool ok = false;
            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Detached))
            {
                // LEMBRETE: LEVAR PARA CLTPONTO A inclusão do maior NREG desta maneira 
                ok = await Prepara_Sql.OpereIncluaRegistroServidorAsync_diversosMAX(orow, "CLTCAD", new List<string>() { "NUMERO" });
            }
            else {
                ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "CLTCAD"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            }
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Inserir Registro CLTCAD"); return;
            }

            try
            {

                orow.AcceptChanges();

            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                // throw;
                return;
            }
            return;
        }





        public void Cadastro_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((Convert.ToDouble(orow["SALBASE"]) < 0)
                  || (orow["SETOR"].ToString().Trim() == "")
                  || (orow["CODCAD"].ToString().Trim() == "")
                  || (orow["GLECAD"].ToString().Trim() == "")
                  || (orow.IsNull("ADMI"))
                  || (orow.IsNull("NASC"))
                  || (Convert.ToDateTime(orow["ADMI"]).CompareTo(Convert.ToDateTime(orow["NASC"])) <= 0)
                  )
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }

            // Outras Inconsistencias que se Fizerem necessárias testar
            try { }
            catch (Exception)
            {
                MessageBox.Show("ERRO");
                e.Cancela = true;
                return;
            }
        }


        public async void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];

            bool ok = false;
            string tabela = "CLTCAD"; // orow.Table.TableName.ToUpper().Trim().Substring(1);
            try
            {
                ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, tabela);

            }
            catch (Exception)
            {
            }
            // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Deletar Registro " + tabela); return;
            }
        }
        public async void Padrao_BeforeDeletaRegistros(object sender, AlteraRegistroEventArgs e)
        {
            // ATENçÂO => ANTES DE DELETAR  É NECESSÁRIO VERIFICAR SE EXISTEM REGISTROS LIGADOS A ESTE REGISTRO
            DataRow orow = e.Rows[0];
            DataView rows = null;
            try
            {
                rows = tabCltFilhos.AsEnumerable().Where(row => row.Field<string>("CODCAD").ToString().Trim() == orow["CODCAD"].ToString().Trim()).AsDataView(); ;
            }
            catch (Exception)
            {
                rows = null;
            }
            if ((rows != null) && (rows.Count > 0))
            {

                DialogResult result = MessageBox.Show("Existem Filhos Ligados a Este Trabalhador.Remove?", "Decida", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    e.Cancela = true;
                    MessageBox.Show("Operacão Abortada!! "); return;
                }
                else
                {
                    string str = "DELETE ALL FROM CLTFILHO WHERE CODCAD = '" + orow["CODCAD"].ToString() + "'";
                    int retorno = await ApiServices.PostApi(str, 1);
                    if (retorno == rows.Count)
                    {
                        MessageBox.Show("Sucesso ao Deletar arquivo CLTFILHO");
                    }
                    else
                    {
                        MessageBox.Show("Problemas ao excluir Filhos");
                        e.Cancela = true;

                    }
                }
            }

        }

        // ALGORITMO DE VALOR DO SALARIO BASE A PARTIR DA FUNCAO DO SQLSER 

        private void SalariosBaseNovo()
        {
            // tabReajuste.AsEnumerable().Where(row=> row.Field<DateTime>(""))
            /*

             string reaj = "SELECT TRAB, PERCCONF, REAJ_CALC, MIN_CALC,"
             + "RESIDUO, iif(ULT_ATUAL is null, Admi, ULT_ATUAL) as ult_atual,cltreaj.[DATA] "
              + " FROM REAJUSTE as CLTREAJ, cltcad "
             + " WHERE   CLTREAJ.trab = cltcad.codcad AND(CLTREAJ.trab <> '') AND "
             + " ((demi is null  AND cltcad.prazo is null) OR "
             + " (Format(cltcad.demi, 'yyyyMM') >= " + data.ToString("yyyyMM") + ")) "
             + " 	and (admi <= " + data.ToString("yyyyMMdd") + ") "
             + " and(cltreaj.[data] <= " + data.ToString("yyyyMMdd") + ") "
             + " ORDER BY CLTREAJ.trab, CLTREAJ.[DATA] ";

             */
        }

        /*
         * 	DECLARE @tmp table (trab nvarchar(4),percconf numeric(19,4),reaj_calc numeric(19,4),min_calc numeric(19,4),residuo numeric(19,4),ult_atual date,datareaj date)
insert into @tmp 
SELECT  TRAB,PERCCONF,REAJ_CALC,MIN_CALC,
RESIDUO,iif(ULT_ATUAL is null,Admi,ULT_ATUAL) as ult_atual,cltreaj.[DATA]
        FROM         REAJUSTE as CLTREAJ, cltcad 
       WHERE        CLTREAJ.trab = cltcad.codcad AND (CLTREAJ.trab <> '''') AND 
	   ((demi is null  AND cltcad.prazo is null) OR 
    (Format(cltcad.demi,'yyyyMM') >= Format(@inicio,'yyyyMM'))) 
	and (admi <= @fim)
	and (cltreaj.[data] <= @fim) 
	ORDER BY CLTREAJ.trab, CLTREAJ.[DATA]

-- tabela cursor mestre que irá retornar esta função
--DECLARE @tmpunique table (codcad nvarchar(4),tsal numeric(19,4))
insert into @tmpunique 
SELECT  CODCAD, 0.0 as tsal,iif(SALBASE=0,1.0,SALBASE) as salbase, 0.0 as soma_perc_conf FROM cltcad WHERE       
	   ((demi is null  AND cltcad.prazo is null) OR 
    (Format(cltcad.demi,'yyyyMM') >= Format(@inicio,'yyyyMM'))) 
	and (admi <= @fim)	ORDER BY CODCAD;


-- dados para a operação de calculo do salario
DECLARE @tperc_confianca numeric(19,4),
        @salmin_calc numeric(19,4),
        @tresiduo numeric(19,4),
		@fator numeric(19,4),
		@tperc_reaj numeric(19,4);

DECLARE @tsal numeric(19,4);
   
DECLARE @TRAB nvarchar(4),
        @PERCCONF numeric(19,4),
        @SOMAPERCCONF numeric(19,4),
        @REAJ_CALC numeric(19,4),
        @MIN_CALC numeric(19,4),
        @RESIDUO numeric(19,4),
		@ULT_ATUAL date,
		@DATAREAJ date;

 
-- aqui começa a brincadeira 
-- um cursor para a lista de trabalhadores 

DECLARE tabela_Mestrecursor CURSOR FOR   
      SELECT * from @tmpunique order by codcad;
DECLARE @mestrecodcad nvarchar(4),
         @mestretsal numeric(19,4),
		 @mestretsalbase numeric(19,4),
		 @mestrepercconf numeric(19,4);

 OPEN tabela_Mestrecursor  

   FETCH NEXT  FROM tabela_Mestrecursor
   INTO @mestrecodcad,@mestretsal, @mestretsalbase,@mestrepercconf;
WHILE @@FETCH_STATUS = 0  
BEGIN  
  
  DECLARE tabela_cursor CURSOR FOR   
      SELECT * from @tmp where trab = @mestrecodcad order by  datareaj
 
  OPEN tabela_cursor  

   FETCH NEXT  FROM tabela_cursor
   INTO
   @TRAB,
        @PERCCONF,
        @REAJ_CALC,
        @MIN_CALC,
        @RESIDUO,
	    @ULT_ATUAL,
		@DATAREAJ;
     
   SET  @tperc_confianca =0;
   SET @SOMAPERCCONF = 0;
   SET  @tperc_reaj =0;
   SET  @salmin_calc = 0;
   SET  @tresiduo =0;
   SET @fator =@mestretsalbase;
   SET @tsal = 0;


WHILE @@FETCH_STATUS = 0  
BEGIN  
  SET @tperc_confianca = @PERCCONF;
  SET @SOMAPERCCONF = @SOMAPERCCONF + @PERCCONF;
  if @tperc_confianca < 0 
    SET @tperc_confianca = @tperc_confianca * -1;
	SET @tperc_reaj = @REAJ_CALC;
            
    SET @tperc_reaj = @tperc_reaj + @tperc_confianca;

    SET @fator = Round(@fator * (1 + (@tperc_reaj / 100)), 4);
	if (@MIN_CALC <> 0) 
       SET @salmin_calc = @MIN_CALC;

          if (@MIN_CALC = 0) and
            (@REAJ_CALC = 0) and
             (@PERCCONF = 0) 
          begin
            SET @salmin_calc = 0;
            SET @tresiduo = 0;
          end;
          if (@RESIDUO <> 0) 
            SET @tresiduo = @RESIDUO;

   FETCH NEXT  FROM tabela_cursor
   INTO
   @TRAB,
        @PERCCONF,
        @REAJ_CALC,
        @MIN_CALC,
        @RESIDUO,
        @ULT_ATUAL,
		@DATAREAJ;
  
END
  if (@salmin_calc <> 0) 
      SET @tsal = Round(@fator * @salmin_calc, 2)
  else
      SET @tsal = Round(@fator * @SalMin, 2);
  SET  @tsal = @tsal + @tresiduo;


  CLOSE tabela_cursor  
  DEALLOCATE tabela_cursor  
  
  UPDATE @tmpunique SET tsal = @tsal,soma_perc_conf = @SOMAPERCCONF  where codcad = @mestrecodcad;

  FETCH NEXT  FROM tabela_Mestrecursor
   INTO @mestrecodcad,@mestretsal,@mestretsalbase,@mestrepercconf;


        */







    }
}

