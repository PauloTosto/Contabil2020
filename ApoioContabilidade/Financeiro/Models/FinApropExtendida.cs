using ApoioContabilidade.Models;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Financeiro.Models
{

    /* // chamadqa desta class
     * var
  // ObjAprop : TObjFin_Aprop;
  objReg: TObjReg;
  tRecno: LongInt;
  ent_sai, i: Integer;
  aa, mm, dd: word;
begin
  Data1Fin := dsDados.DataSet.fieldbyName('DATA').asDateTime;
  Data2Fin := Data1Fin;

  ListAprop.clear;

  if (dsDados.DataSet as TCustomADODataSet).name = 'RMOV_FIN' then
    ent_sai := 1
  else
    ent_sai := 2;

  ObjFinAprop := Fin_Aprop(dsDados.DataSet,
    dsDados.DataSet.fieldbyName('OUTRO_ID').asInteger,
    dsDados.DataSet.fieldbyName('DATA').asDateTime, ent_sai,
    dsDados.DataSet.fieldbyName('VALOR').asFloat, nil);

  if ObjFinAprop <> nil then
    begin
      ListAprop.AddObject(Formatdatetime('aaaammdd',
        dsDados.DataSet.fieldbyName('DATA').asDateTime) + inttostr(ent_sai),
        ObjFinAprop);
      if ExisteForm(TFrmFinAprop, FrmFinAprop) then
        begin
          TFrmFinAprop(FrmFinAprop).TipoRelatorio := 0;
          TFrmFinAprop(FrmFinAprop).ListAprop := ListAprop;
          TFrmFinAprop(FrmFinAprop).Data1Fin := Data1Fin;
          TFrmFinAprop(FrmFinAprop).Data2Fin := Data2Fin;
          TFrmFinAprop(FrmFinAprop).Reconstrua1.Click;
          FrmFinAprop.show;
        end
      else
        begin
          _OpenWindow(TFrmFinAprop, FrmFinAprop);
          TFrmFinAprop(FrmFinAprop).TipoRelatorio := 0;
          TFrmFinAprop(FrmFinAprop).ListAprop := ListAprop;
          TFrmFinAprop(FrmFinAprop).Data1Fin := Data1Fin;
          TFrmFinAprop(FrmFinAprop).Data2Fin := Data2Fin;
          TFrmFinAprop(FrmFinAprop).Reconstrua1.Click;
          TFrmFinAprop(FrmFinAprop).windowstate := wsNormal;
        end;

      { if ExisteForm(TFrmFinAprop,FrmFinAprop) then
        begin
        // FrmFinAprop.Reconstrua1.Click;
        FrmFinAprop.show;
        end
        else
        _OpenWindow(TFrmFinAprop,FrmFinAprop);
        TFrmFinAprop(FrmFinAprop).windowstate := wsNormal; }

    end;


     */
    class ObjFinAprop
    {
       public List<ObjReg> objRegs { get; set; } // listobjReg
        public Decimal valorMestre { get; set; }
        public Decimal valorFin { get; set; }
        public int ent_sai { get; set; }
        public DateTime data { get; set; }
        public RegMovFin regMovFin { get; set; }
      
        public DataSet dados { get; set; }
        
        public ObjFinAprop()
        {
            objRegs = new List<ObjReg>();
            regMovFin = new RegMovFin();
        }
    }
    class RegMovFin
    {
        public DateTime data { get; set; }
        public string conta { get; set; }
        public string hist { get; set; }
        public string tipo { get; set; }
        public decimal valor { get; set; }
        public string docFiscal { get; set; }
        public decimal valorCalc { get; set; }
        public string obs { get; set; }
        public double nreg { get; set; }
    }

    class ObjReg
    {
        List<RegMovFin> listReg { get; set; }
        public double regAprop { get; set; }
        public int aprop_est { get; set; }

        public decimal vlr_Ap { get; set; }

        public decimal vlrFiltro_Ap { get; set; }
        public ObjReg()
        {
            listReg = new List<RegMovFin>(); 
        }

    }

    static class FinApropExtendida
    {

       /* static ObjFinAprop ErroFin_Aprop(BindingSource dsDados, int ent_sai)
        {
            ObjFinAprop result = new ObjFinAprop();

            result.ent_sai = ent_sai;
            DataRowView orow = (dsDados.Current as DataRowView);
            if (ent_sai == 2)
            {
                result.regMovFin.tipo = "D";
                result.regMovFin.conta = orow["DEBITO"].ToString();
            }
            else
            {
                result.regMovFin.tipo = "C";
                result.regMovFin.conta = orow["CREDITO"].ToString();
            }
            result.regMovFin.valor = Convert.ToDecimal(orow["Valor"]);
            result.regMovFin.hist = orow["HIST"].ToString();
            result.regMovFin.data = Convert.ToDateTime(orow["Data"]);
            result.regMovFin.docFiscal = orow["DOC_FISC"].ToString();

            result.regMovFin.obs = "Não Foi Lançado no Pagar e Receber";
            result.data = result.regMovFin.data;

            return result;
        }
       */
      /*  static DataTable PegRegMovFin(double mov_id, int ent_sai)
        {
            DataTable result = new DataTable();
            string tindexName;
            bool tret;

            string strMovFin = "SELECT * FROM MOV_FIN WHERE(MOV_ID = " + mov_id.ToString() + ")";
      
            // DataTable 
            return result;
        }
      */




        /*
        static public async Task<ObjFinAprop> FinAprop(BindingSource bmSource, double outro_id, DateTime data, int ent_sai, Decimal valor, Pesquise opesq = null) // PEsquiseGenerico ou Pesquise
        {
            ObjFinAprop result = new ObjFinAprop();
            double mestre = outro_id;
            bool ret_aprop = false;
            bool ret_contab = false;
            bool ret_estoque = false;
            // verifica se o Filtro oPesquisa Contém Algum FiltroAtivo 
            // CONTAB, APROP, ESTOQUE,  
            if (opesq != null)
            {
                // será sempre null NESTA FASE DE IMPLEMENTAÇÃO 21/10/2020
                if (opesq.Pagina("Aprop") != null)
                {
                    TabPage_Apoio pesqAp = opesq.Pagina("Aprop");
                    var lista = pesqAp.Get_LinhaSolucao();
                    foreach (var l in lista)
                    {

                    }
                }
            }

            if ((ret_aprop == false) && (ret_contab = false) && (ret_estoque = false))
            {
                ret_aprop = true;
                ret_contab = true;
                ret_estoque = true;

            }
            if (mestre== 0)
            {
                if ((ret_aprop) && (ret_contab) && (ret_estoque))
                {
                    result = ErroFin_Aprop(bmSource, ent_sai);
                    return result;
                }
            }
            // Tabelas Envolvidas no processo de apropriação
            List<string> lstSelect = new List<string>();
            lstSelect.Add("SELECT * FROM MOV_FIN WHERE(MOV_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM CTACENTR WHERE(MOV_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM MOVEST WHERE(MOV_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM MOV_APRO WHERE(OUTRO_ID = " + mestre.ToString() + ")");

            DataSet dados  = await ApiServices.Api_QueryMulti(lstSelect);
            if (dados.Tables[0].Rows.Count == 0)
            {
                result = null;
                return result;
            }
            


            return result;
        }
        */
        // auxilia APROPRIAÇÔES
        static public async Task<DataSet> FinAprop( double outro_id) 
        {
           
            double mestre = outro_id;
            if (mestre == 0)
            {
                DataSet result = null;
                return result;
            }
            
            // Tabelas Envolvidas no processo de apropriação
            List<string> lstSelect = new List<string>();
            lstSelect.Add("SELECT * FROM MOV_PGRC WHERE(MOV_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM MOV_FIN WHERE(OUTRO_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM MOV_APRO WHERE(OUTRO_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM CTACENTR WHERE(MOV_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM MOVEST WHERE(MOV_ID = " + mestre.ToString() + ")");
            lstSelect.Add("SELECT * FROM VENDAS WHERE(MOV_ID = " + mestre.ToString() + ")");


            DataSet dados = await ApiServices.Api_QueryMulti(lstSelect);
            if (dados.Tables[0].Rows.Count == 0)
            {
                dados = null;
                return dados;
            }

            dados.Tables[0].TableName = "MOV_PGRC";
            dados.Tables[1].TableName = "MOV_FIN";
            dados.Tables[2].TableName = "MOV_APRO";
            dados.Tables[3].TableName = "CTACENTR";
            dados.Tables[4].TableName = "MOVEST";
            dados.Tables[5].TableName = "VENDAS";
            return dados;
        }

        static public async Task<DataSet> FinAprop_MovFIn(double outro_id)
        {

            double mestre = outro_id;
            if (mestre == 0)
            {
                DataSet result = null;
                return result;
            }

            // Tabelas Envolvidas no processo de apropriação
            List<string> lstSelect = new List<string>();
            lstSelect.Add("SELECT * FROM MOV_FIN WHERE(OUTRO_ID = " + mestre.ToString() + ")");
        

            DataSet dados = await ApiServices.Api_QueryMulti(lstSelect);
            if (dados.Tables[0].Rows.Count == 0)
            {
                dados = null;
                return dados;
            }

            dados.Tables[0].TableName = "MOV_FIN";
            return dados;
        }



        // auxilia TRANSFERENCIAS




    }


    /*

     
     
     
     * var

  tmestre: Integer;

  objReg: TOBJReg;

  RegCentro: TRegCentro;
  RegEstoque: TRegEstoque;
  RegMovFin: TRegMovFin;
  i: Integer;
  tret: boolean;
  ret_aprop, ret_contab, ret_estoque: boolean;
  BMark: TBookMark;
  tindexName: string;
  qrEstoque: TAdoDataSet;
  qrAprop: TAdoDataSet;
  qrContab: TAdoDataSet;
  datastring: string;
begin

  tmestre := tdetalhe1;

  RegMovFin := PegRegMovFin(tmestre, Ent_Sai, nil);
  // registro mestre originador
  if RegMovFin = nil then
    exit;

  result := TOBJFIN_APROP.Create;
  result.ListObjReg := TList.Create;
  result.tvalorMestre := RegMovFin.Valor; // valor original do mestre

  result.tValorFin := tvalordet; // valor do pagamento especifico
  result.Data := tdata;

  with RegMovFin do
  begin
    if Ent_Sai = 1 then
    begin
      Tipo := 'D';
      Conta := DataSet.FieldbyName('DEBITO').asString;
    end
    else
    begin
      Tipo := 'C';
      Conta := DataSet.FieldbyName('CREDITO').asString;
    end;
    Valor := DataSet.FieldbyName('Valor').asFloat;
    HIST := DataSet.FieldbyName('HIST').asString;
    Data := DataSet.FieldbyName('Data').asDateTime;
    DocFisc := DataSet.FieldbyName('DOC_FISC').asString;
  end;

  result.RegMovFin := RegMovFin;

  result.Ent_Sai := Ent_Sai;
  objReg := nil;

  if ret_aprop then
  begin

    datastring := '';
    if (oPesquisa <> nil) and (oPesquisa.Pagina['Aprop'] <> nil) then
      datastring := oPesquisa.Criterio['Aprop'].dump;
    if datastring <> '' then
      datastring := ' AND ' + datastring;
    qrAprop := TAdoDataSet.Create(dmAdoFinan);
    qrAprop := DevolveQuery('SELECT * from ctacentr WHERE (MOV_ID = ' +
      inttostr(tmestre) + ') ' + datastring, dmAdoFinan.PATH_ESCRITOR,
      'ICTAC4', qrAprop);

    with qrAprop do
    begin
      objReg := nil;
      if (active = false) then
        active := true;
      First;
      while not eof do
      begin
        if FieldbyName('VALOR').asFloat = 0 then
        begin
          next;
          continue;
        end;

        if objReg = nil then
        begin
          objReg := TOBJReg.Create;
          with objReg do
          begin
            ListReg := TList.Create;
            vlr_Ap := 0;
            VlrFiltro_Ap := 0;
            objReg.Aprop_Est := 1;
          end;
        end;
        objReg.vlr_Ap := objReg.vlr_Ap + FieldbyName('VALOR').asFloat;
        objReg.VlrFiltro_Ap := objReg.VlrFiltro_Ap +
          FieldbyName('VALOR').asFloat;
        RegCentro := TRegCentro.Create;
        RegCentro.Data := FieldbyName('DATA').asDateTime;
        RegCentro.NUM := FieldbyName('NUM_MOD').asString;
        RegCentro.CODSER := FieldbyName('CODSER').asString;
        RegCentro.SETOR := FieldbyName('SETOR').asString;
        RegCentro.Centro := FieldbyName('GLEBA').asString;
        RegCentro.Quadra := FieldbyName('QUADRA').asString;
        RegCentro.Valor := FieldbyName('VALOR').asFloat;
        RegCentro.Quant := FieldbyName('QUANT').asFloat;
        RegCentro.Tipo := FieldbyName('ICODSER').asInteger;
        RegCentro.Historico := FieldbyName('HISTORICO').asString;
        RegCentro.nreg := RECNO;
        objReg.ListReg.Add(RegCentro);
        next;
      end;
      qrAprop.Destroy;
      // end;
    end;
    if objReg <> nil then
    begin
      result.ListObjReg.Add(objReg);
      objReg := nil;
    end;

    if ret_estoque then
    begin

      datastring := '';
      if (oPesquisa <> nil) and (oPesquisa.Pagina['Aprop'] <> nil) then
        datastring := oPesquisa.Criterio['Aprop'].dump;
      if datastring <> '' then
        datastring := ' AND ' + datastring;
      if (oPesquisa <> nil) and (oPesquisa.Pagina['Estoque'] <> nil) then
      begin
        if (oPesquisa.Criterio['Estoque'].dump <> '') then
          if datastring <> '' then
            datastring := datastring + ' AND ' + oPesquisa.Criterio
              ['Estoque'].dump;
      end;
      qrEstoque := TAdoDataSet.Create(dmAdoFinan);
      qrEstoque := DevolveQuery('SELECT * from MOVEST WHERE (MOV_ID = ' +
        inttostr(tmestre) + ') ' + datastring, dmAdoFinan.PATH_ESCRITOR,
        'IMES6', qrEstoque);

      with qrEstoque do
      begin
        if (active = false) then
          active := true;
        First;
        while not eof do
        begin
          if FieldbyName('VALOR').asFloat = 0 then
          begin
            next;
            continue;
          end;

          if objReg = nil then
          begin
            objReg := TOBJReg.Create;
            with objReg do
            begin
              ListReg := TList.Create;
              vlr_Ap := 0;
              VlrFiltro_Ap := 0;
              objReg.Aprop_Est := 2;
            end;
          end;
          objReg.vlr_Ap := objReg.vlr_Ap + FieldbyName('VALOR').asFloat;

          objReg.VlrFiltro_Ap := objReg.VlrFiltro_Ap +
            FieldbyName('VALOR').asFloat;
          RegEstoque := TRegEstoque.Create;
          RegEstoque.Data := FieldbyName('DATA').asDateTime;
          RegEstoque.Valor := FieldbyName('VALOR').asFloat;
          RegEstoque.Quant := FieldbyName('QUANT').asFloat;
          RegEstoque.Item := FieldbyName('COD').asString;
          RegEstoque.DEPOSITO := FieldbyName('DEPOSITO').asString;
          RegEstoque.NUM_mod := FieldbyName('NUM_MOD').asString;
          RegEstoque.CODSER := FieldbyName('CODSER').asString;
          RegEstoque.SETOR := FieldbyName('SETOR').asString;
          RegEstoque.Centro := FieldbyName('GLEBA').asString;
          RegEstoque.nreg := RECNO;
          objReg.ListReg.Add(RegEstoque);
          next;
        end;
        qrEstoque.Destroy;
      end;
    end;

    if objReg <> nil then
    begin
      result.ListObjReg.Add(objReg);
      objReg := nil;
    end;
    if ret_contab then
    begin

      qrContab := TAdoDataSet.Create(dmAdoFinan);
      qrContab := DevolveQuery('SELECT * from MOV_APRO WHERE (OUTRO_ID = ' +
        inttostr(tmestre) + ') ', dmAdoFinan.PATH_ESCRITOR, 'IMES6', qrAprop);

      with qrContab do
      begin
        if (active = false) then
          active := true;
        First;

        while (not eof) do
        begin
          RegMovFin := TRegMovFin.Create;
          with RegMovFin do
          begin
            if FieldbyName('DEBITO').asString <> '' then
            begin
              Conta := FieldbyName('DEBITO').asString;
              Tipo := 'D';
            end
            else
            begin
              Tipo := 'C';
              Conta := FieldbyName('CREDITO').asString;
            end;
            Valor := FieldbyName('Valor').asFloat;
            HIST := FieldbyName('HIST').asString;
            Data := FieldbyName('Data').asDateTime;
            DocFisc := FieldbyName('DOC_FISC').asString;
          end;
          if objReg = nil then
          begin
            objReg := TOBJReg.Create;
            with objReg do
            begin
              ListReg := TList.Create;
              vlr_Ap := 0;
              VlrFiltro_Ap := 0;
              objReg.Aprop_Est := 3;
            end;
          end;
          objReg.ListReg.Add(RegMovFin);
          objReg.vlr_Ap := objReg.vlr_Ap + RegMovFin.Valor;
          objReg.VlrFiltro_Ap := objReg.VlrFiltro_Ap + RegMovFin.Valor;
          next;
        end;
        qrContab.active := false;
        qrContab.Destroy();
      end;
    end;
    if objReg <> nil then
    begin
      result.ListObjReg.Add(objReg);
      objReg := nil;
    end;
  end;
  // caso geral e em que nenhum registro foi apropriado
  if (ret_aprop and ret_contab and ret_estoque) then
    if (result.ListObjReg.Count = 0) then
      exit;

  result := RateieValores(result);
end;

    */












    /* 


   TOBJFIN_APROP = class(TObject)
     ListObjReg: TList;
     tvalorMestre, tValorFin: Currency;
   
     Ent_Sai: Integer;
     Data: TDateTime;
     RegMovFin: TRegMovFin;
   end;
    */
}
/*
 * function Fin_Aprop(DataSet: TDataSet; tdetalhe1: Integer; tdata: TDateTime;
  Ent_Sai: Integer; tvalordet: Currency; oPesquisa: Tpesquisa): TOBJFIN_APROP;

  function RateieValoresContabil(ObjFin_Aprop: TOBJFIN_APROP): TOBJFIN_APROP;
  var
    objReg: TOBJReg;
    RegCentro: TRegCentro;
    RegEstoque: TRegEstoque;
    RegMovFin: TRegMovFin;
    i, j: Integer;
    base, resto: Currency;
    TotalRateio: Currency;

  begin

    result := ObjFin_Aprop;
    // o valorgeral apropriado é comparado com o mestre para
    // que possamos avaliar se todo o lançamento foi apropriado;
    // Se houver diferença o menor valor servirá de base do rateio,
    // desde que seja um valor maior ou igual ao valor do lançamento no financeiro;
    // if  result.tvalorMestre >= result.GeralAprop then
    // Base := result.tvalorMestre
    // else

    base := result.tvalorMestre;

    if (result.ListObjReg.Count = 0) or (base = 0) then
    begin
      result.Free;
      result := nil;
      exit;
    end;

    // repete o processo para o contabil

    // o valor daquele pagamento em foco em relacao ao valor do lancamento que o originou

    // 1. analise =>avaliacao em relacao aos centros e ao estoque

    // compara os vlrs  apropriados com a parcela de pagamento
    TotalRateio := 0;
    for i := 0 to result.ListObjReg.Count - 1 do
    begin
      objReg := result.ListObjReg[i];
      if (objReg.Aprop_Est = 3) then
        TotalRateio := TotalRateio + objReg.vlr_Ap;
    end;

    if base = result.tValorFin then
    begin // nao 'e necessario ratear;
      for i := 0 to result.ListObjReg.Count - 1 do
      begin
        objReg := result.ListObjReg[i];
        for j := 0 to objReg.ListReg.Count - 1 do
        begin
          if objReg.Aprop_Est = 3 then
          begin // centros
            RegMovFin := objReg.ListReg[j];
            RegMovFin.ValorCalc := RegMovFin.Valor;
            objReg.ListReg[j] := RegMovFin;
          end;
        end;
        result.ListObjReg[i] := objReg;
      end;
      exit;
    end;

    // resto := totalRateio;

    resto := ptround((TotalRateio * (result.tValorFin / base)), 2);
    if TotalRateio = base then
      resto := result.tValorFin;

    for i := 0 to result.ListObjReg.Count - 1 do
    begin
      objReg := result.ListObjReg[i];
      for j := 0 to objReg.ListReg.Count - 1 do
      begin
        if objReg.Aprop_Est = 3 then
        begin // centros
          RegMovFin := objReg.ListReg[j];
          RegMovFin.ValorCalc :=
            ptround((RegMovFin.Valor * (result.tValorFin / base)), 2);
          resto := resto - RegMovFin.ValorCalc;
          objReg.ListReg[j] := RegMovFin;
        end;

      end;
      result.ListObjReg[i] := objReg;
    end;

    if resto <> 0 then
      for i := 0 to result.ListObjReg.Count - 1 do
      begin
        objReg := result.ListObjReg[i];
        for j := 0 to objReg.ListReg.Count - 1 do
        begin
          if objReg.Aprop_Est = 3 then
          begin // centros
            RegMovFin := objReg.ListReg[j];
            RegMovFin.ValorCalc := RegMovFin.ValorCalc + resto;
            resto := 0;
            objReg.ListReg[j] := RegMovFin;
          end;
          if resto = 0 then
            break;
        end;
        result.ListObjReg[i] := objReg;
        if resto = 0 then
          break;
      end;

  end;

  function RateieValores(ObjFin_Aprop: TOBJFIN_APROP): TOBJFIN_APROP;
  var
    objReg: TOBJReg;
    RegCentro: TRegCentro;
    RegEstoque: TRegEstoque;
    RegMovFin: TRegMovFin;
    i, j: Integer;
    base, resto: Currency;
    TotalRateio: Currency;

  begin

    result := ObjFin_Aprop;
    // o valorgeral apropriado é comparado com o mestre(lancamento NÀo financeiro / original)  para
    // que possamos avaliar se todo o lançamento foi apropriado ou não;
    // Se não foi totalmente Apropriado, o valor que servirá de base para lançar no mes
    // a parte da apropriação relativa ao pagamento daquele mes será o valor do mestre.
    // se a apropriacao for maior que o vlr mestre é um erro grave.
    // Como também é um erro grave se a parcela financeira for maior que
    // o valor mestre.

    base := result.tvalorMestre;

    if (result.ListObjReg.Count = 0) or (base = 0) then
    begin
      result.Free;
      result := nil;
      exit;
    end;



    // o valor daquele pagamento em foco em relacao ao valor do lancamento que o originou

    // 1. analise =>avaliacao em relacao aos centros e ao estoque

    // compara os vlrs  apropriados com a parcela de pagamento
    TotalRateio := 0;
    for i := 0 to result.ListObjReg.Count - 1 do
    begin
      objReg := result.ListObjReg[i];
      // valores para aprops gerenciais
      if (objReg.Aprop_Est = 1) then
        TotalRateio := TotalRateio + objReg.vlr_Ap;
      if (objReg.Aprop_Est = 2) then
        TotalRateio := TotalRateio + objReg.vlr_Ap;
    end;

    // if totalrateio < base then   base := totalrateio; // significa que nem todo o mestre foi lancado nos gerenciais
    // ou é conseguencia de um filtro

    if base = result.tValorFin then
    begin // nao 'e necessario ratear;
      for i := 0 to result.ListObjReg.Count - 1 do
      begin
        objReg := result.ListObjReg[i];
        for j := 0 to objReg.ListReg.Count - 1 do
        begin
          if objReg.Aprop_Est = 1 then
          begin // centros
            RegCentro := objReg.ListReg[j];
            RegCentro.ValorCalc := RegCentro.Valor;
            objReg.ListReg[j] := RegCentro;
          end;
          if objReg.Aprop_Est = 2 then
          begin // estoque
            RegEstoque := objReg.ListReg[j];
            RegEstoque.ValorCalc := RegEstoque.Valor;
            objReg.ListReg[j] := RegEstoque;
          end;
        end;
        result.ListObjReg[i] := objReg;
      end;
      result := RateieValoresContabil(result);
      exit;
    end;

    // o total rateado deve fechar com o total apropriado
    { if result.tvalorFin > totalrateio
      resto :=   totalrateio
      else
      resto := result.tvalorFin; }

    resto := ptround((TotalRateio * (result.tValorFin / base)), 2);
    if TotalRateio = base then
      resto := result.tValorFin;

    for i := 0 to result.ListObjReg.Count - 1 do
    begin
      objReg := result.ListObjReg[i];
      for j := 0 to objReg.ListReg.Count - 1 do
      begin
        if objReg.Aprop_Est = 1 then
        begin // centros
          RegCentro := objReg.ListReg[j];
          RegCentro.ValorCalc :=
            ptround((RegCentro.Valor * (result.tValorFin / base)), 2);
          resto := resto - RegCentro.ValorCalc;
          objReg.ListReg[j] := RegCentro;
        end
        else if objReg.Aprop_Est = 2 then
        begin // Estoque
          RegEstoque := objReg.ListReg[j];
          RegEstoque.ValorCalc :=
            ptround((RegEstoque.Valor * (result.tValorFin / base)), 2);
          resto := resto - RegEstoque.ValorCalc;
          objReg.ListReg[j] := RegEstoque;
        end;
      end;
      result.ListObjReg[i] := objReg;
    end;

    if resto <> 0 then
      for i := 0 to result.ListObjReg.Count - 1 do
      begin
        objReg := result.ListObjReg[i];
        for j := 0 to objReg.ListReg.Count - 1 do
        begin
          if objReg.Aprop_Est = 1 then
          begin // centros
            RegCentro := objReg.ListReg[j];
            RegCentro.ValorCalc := RegCentro.ValorCalc + resto;
            resto := 0;
            objReg.ListReg[j] := RegCentro;
          end
          else if objReg.Aprop_Est = 2 then
          begin // Estoque
            RegEstoque := objReg.ListReg[j];
            RegEstoque.ValorCalc := RegEstoque.ValorCalc + resto;
            resto := 0;
            objReg.ListReg[j] := RegEstoque;
          end;
          if resto = 0 then
            break;
        end;
        result.ListObjReg[i] := objReg;
        if resto = 0 then
          break;
      end;

    result := RateieValoresContabil(result);

  end;

  Function PegRegMovFin(tmov_id: Integer; Ent_Sai: Integer;
    oPesquisa: Tpesquisa): TRegMovFin;
  var
    tindexName: string;
    tret: boolean;
    i: Integer;
    qrPegNum: TAdoDataSet;
    datastring: string;
  begin
    datastring := '';
    if (oPesquisa <> nil) and (oPesquisa.Pagina['Contab'] <> nil) then
      datastring := oPesquisa.Criterio['Contab'].dump;
    if datastring <> '' then
      datastring := ' AND ' + datastring;

    qrPegNum := TAdoDataSet.Create(dmAdoFinan);
    qrPegNum := DevolveQuery('SELECT * FROM MOV_FIN WHERE (MOV_ID = ' +
      inttostr(tmov_id) + ') ' + datastring, dmAdoFinan.PATH_ESCRITOR, 'IPGRC7',
      qrPegNum);
    result := TRegMovFin.Create;
    try
      with qrPegNum do
      begin
        if (eof) then
          exit;
        First;
        if Ent_Sai = 1 then
        begin
          result.Tipo := 'D';
          result.Conta := FieldbyName('DEBITO').asString;
        end
        else
        begin
          result.Tipo := 'C';
          result.Conta := FieldbyName('CREDITO').asString;
        end;
        result.Valor := FieldbyName('Valor').asFloat;
        result.HIST := FieldbyName('HIST').asString;
        result.Data := FieldbyName('Data').asDateTime;
        result.DocFisc := FieldbyName('DOC_FISC').asString;
      end
    except
      result := nil;
    end;
  end;

var

  tmestre: Integer;

  objReg: TOBJReg;

  RegCentro: TRegCentro;
  RegEstoque: TRegEstoque;
  RegMovFin: TRegMovFin;
  i: Integer;
  tret: boolean;
  ret_aprop, ret_contab, ret_estoque: boolean;
  BMark: TBookMark;
  tindexName: string;
  qrEstoque: TAdoDataSet;
  qrAprop: TAdoDataSet;
  qrContab: TAdoDataSet;
  datastring: string;
begin

  ret_aprop := false;
  ret_contab := false;
  ret_estoque := false;

  if (oPesquisa <> nil) and (oPesquisa.Pagina['Aprop'] <> nil) then
    with oPesquisa.Pagina['Aprop'] do
    begin
      for i := 0 to NumLinhas - 1 do
      begin
        if checked[i] and (variavel[i, 0] <> '') then
        begin
          ret_aprop := true;
          ret_estoque := true;
          break;
        end;
      end;
    end;
  if (oPesquisa <> nil) and (oPesquisa.Pagina['Contab'] <> nil) then
    with oPesquisa.Pagina['Contab'] do
    begin
      for i := 0 to NumLinhas - 1 do
      begin
        if checked[i] and (variavel[i, 0] <> '') then
        begin
          ret_contab := true;
          break;
        end;
      end;
    end;
  if (oPesquisa <> nil) and (oPesquisa.Pagina['Estoque'] <> nil) then
    with oPesquisa.Pagina['Estoque'] do
    begin
      for i := 0 to NumLinhas - 1 do
      begin
        if checked[i] and (variavel[i, 0] <> '') then
        begin
          ret_estoque := true;
          break;
        end;
      end;
    end;

  if (ret_aprop = false) and (ret_contab = false) and (ret_estoque = false) then
  begin
    ret_aprop := true;
    ret_contab := true;
    ret_estoque := true;
  end;
  result := nil;

  if tdetalhe1 = 0 then
  begin
    if (ret_aprop) and (ret_contab) and (ret_estoque) then
    begin
      // if oPesquisa = nil then exit;
      result := ErroFin_Aprop(DataSet, Ent_Sai, oPesquisa);
      result.tValorFin := tvalordet;
      result.Data := tdata;
      result.RegMovFin.Obs := 'Não foi Lancado no Pagar e Receber';
    end;
    exit;
  end;

  { if tDetalhe1 = 0 then
    begin
    if (ret_aprop) and (ret_contab) and (ret_estoque) then
    showMessage('Registro Não Apropriado em:'+datetostr(tdata)+ ', valor:'+checkfloattostr(tvalordet));
    exit;
    end;
  }
  tmestre := tdetalhe1;

  RegMovFin := PegRegMovFin(tmestre, Ent_Sai, nil);
  // registro mestre originador
  if RegMovFin = nil then
    exit;

  result := TOBJFIN_APROP.Create;
  result.ListObjReg := TList.Create;
  result.tvalorMestre := RegMovFin.Valor; // valor original do mestre

  result.tValorFin := tvalordet; // valor do pagamento especifico
  result.Data := tdata;

  with RegMovFin do
  begin
    if Ent_Sai = 1 then
    begin
      Tipo := 'D';
      Conta := DataSet.FieldbyName('DEBITO').asString;
    end
    else
    begin
      Tipo := 'C';
      Conta := DataSet.FieldbyName('CREDITO').asString;
    end;
    Valor := DataSet.FieldbyName('Valor').asFloat;
    HIST := DataSet.FieldbyName('HIST').asString;
    Data := DataSet.FieldbyName('Data').asDateTime;
    DocFisc := DataSet.FieldbyName('DOC_FISC').asString;
  end;

  result.RegMovFin := RegMovFin;

  result.Ent_Sai := Ent_Sai;
  objReg := nil;

  if ret_aprop then
  begin

    datastring := '';
    if (oPesquisa <> nil) and (oPesquisa.Pagina['Aprop'] <> nil) then
      datastring := oPesquisa.Criterio['Aprop'].dump;
    if datastring <> '' then
      datastring := ' AND ' + datastring;
    qrAprop := TAdoDataSet.Create(dmAdoFinan);
    qrAprop := DevolveQuery('SELECT * from ctacentr WHERE (MOV_ID = ' +
      inttostr(tmestre) + ') ' + datastring, dmAdoFinan.PATH_ESCRITOR,
      'ICTAC4', qrAprop);

    with qrAprop do
    begin
      objReg := nil;
      if (active = false) then
        active := true;
      First;
      while not eof do
      begin
        if FieldbyName('VALOR').asFloat = 0 then
        begin
          next;
          continue;
        end;

        if objReg = nil then
        begin
          objReg := TOBJReg.Create;
          with objReg do
          begin
            ListReg := TList.Create;
            vlr_Ap := 0;
            VlrFiltro_Ap := 0;
            objReg.Aprop_Est := 1;
          end;
        end;
        objReg.vlr_Ap := objReg.vlr_Ap + FieldbyName('VALOR').asFloat;
        objReg.VlrFiltro_Ap := objReg.VlrFiltro_Ap +
          FieldbyName('VALOR').asFloat;
        RegCentro := TRegCentro.Create;
        RegCentro.Data := FieldbyName('DATA').asDateTime;
        RegCentro.NUM := FieldbyName('NUM_MOD').asString;
        RegCentro.CODSER := FieldbyName('CODSER').asString;
        RegCentro.SETOR := FieldbyName('SETOR').asString;
        RegCentro.Centro := FieldbyName('GLEBA').asString;
        RegCentro.Quadra := FieldbyName('QUADRA').asString;
        RegCentro.Valor := FieldbyName('VALOR').asFloat;
        RegCentro.Quant := FieldbyName('QUANT').asFloat;
        RegCentro.Tipo := FieldbyName('ICODSER').asInteger;
        RegCentro.Historico := FieldbyName('HISTORICO').asString;
        RegCentro.nreg := RECNO;
        objReg.ListReg.Add(RegCentro);
        next;
      end;
      qrAprop.Destroy;
      // end;
    end;
    if objReg <> nil then
    begin
      result.ListObjReg.Add(objReg);
      objReg := nil;
    end;

    if ret_estoque then
    begin

      datastring := '';
      if (oPesquisa <> nil) and (oPesquisa.Pagina['Aprop'] <> nil) then
        datastring := oPesquisa.Criterio['Aprop'].dump;
      if datastring <> '' then
        datastring := ' AND ' + datastring;
      if (oPesquisa <> nil) and (oPesquisa.Pagina['Estoque'] <> nil) then
      begin
        if (oPesquisa.Criterio['Estoque'].dump <> '') then
          if datastring <> '' then
            datastring := datastring + ' AND ' + oPesquisa.Criterio
              ['Estoque'].dump;
      end;
      qrEstoque := TAdoDataSet.Create(dmAdoFinan);
      qrEstoque := DevolveQuery('SELECT * from MOVEST WHERE (MOV_ID = ' +
        inttostr(tmestre) + ') ' + datastring, dmAdoFinan.PATH_ESCRITOR,
        'IMES6', qrEstoque);

      with qrEstoque do
      begin
        if (active = false) then
          active := true;
        First;
        while not eof do
        begin
          if FieldbyName('VALOR').asFloat = 0 then
          begin
            next;
            continue;
          end;

          if objReg = nil then
          begin
            objReg := TOBJReg.Create;
            with objReg do
            begin
              ListReg := TList.Create;
              vlr_Ap := 0;
              VlrFiltro_Ap := 0;
              objReg.Aprop_Est := 2;
            end;
          end;
          objReg.vlr_Ap := objReg.vlr_Ap + FieldbyName('VALOR').asFloat;

          objReg.VlrFiltro_Ap := objReg.VlrFiltro_Ap +
            FieldbyName('VALOR').asFloat;
          RegEstoque := TRegEstoque.Create;
          RegEstoque.Data := FieldbyName('DATA').asDateTime;
          RegEstoque.Valor := FieldbyName('VALOR').asFloat;
          RegEstoque.Quant := FieldbyName('QUANT').asFloat;
          RegEstoque.Item := FieldbyName('COD').asString;
          RegEstoque.DEPOSITO := FieldbyName('DEPOSITO').asString;
          RegEstoque.NUM_mod := FieldbyName('NUM_MOD').asString;
          RegEstoque.CODSER := FieldbyName('CODSER').asString;
          RegEstoque.SETOR := FieldbyName('SETOR').asString;
          RegEstoque.Centro := FieldbyName('GLEBA').asString;
          RegEstoque.nreg := RECNO;
          objReg.ListReg.Add(RegEstoque);
          next;
        end;
        qrEstoque.Destroy;
      end;
    end;

    if objReg <> nil then
    begin
      result.ListObjReg.Add(objReg);
      objReg := nil;
    end;
    if ret_contab then
    begin

      qrContab := TAdoDataSet.Create(dmAdoFinan);
      qrContab := DevolveQuery('SELECT * from MOV_APRO WHERE (OUTRO_ID = ' +
        inttostr(tmestre) + ') ', dmAdoFinan.PATH_ESCRITOR, 'IMES6', qrAprop);

      with qrContab do
      begin
        if (active = false) then
          active := true;
        First;

        while (not eof) do
        begin
          RegMovFin := TRegMovFin.Create;
          with RegMovFin do
          begin
            if FieldbyName('DEBITO').asString <> '' then
            begin
              Conta := FieldbyName('DEBITO').asString;
              Tipo := 'D';
            end
            else
            begin
              Tipo := 'C';
              Conta := FieldbyName('CREDITO').asString;
            end;
            Valor := FieldbyName('Valor').asFloat;
            HIST := FieldbyName('HIST').asString;
            Data := FieldbyName('Data').asDateTime;
            DocFisc := FieldbyName('DOC_FISC').asString;
          end;
          if objReg = nil then
          begin
            objReg := TOBJReg.Create;
            with objReg do
            begin
              ListReg := TList.Create;
              vlr_Ap := 0;
              VlrFiltro_Ap := 0;
              objReg.Aprop_Est := 3;
            end;
          end;
          objReg.ListReg.Add(RegMovFin);
          objReg.vlr_Ap := objReg.vlr_Ap + RegMovFin.Valor;
          objReg.VlrFiltro_Ap := objReg.VlrFiltro_Ap + RegMovFin.Valor;
          next;
        end;
        qrContab.active := false;
        qrContab.Destroy();
      end;
    end;
    if objReg <> nil then
    begin
      result.ListObjReg.Add(objReg);
      objReg := nil;
    end;
  end;
  // caso geral e em que nenhum registro foi apropriado
  if (ret_aprop and ret_contab and ret_estoque) then
    if (result.ListObjReg.Count = 0) then
      exit;

  result := RateieValores(result);
end;

function ErroFin_Aprop(DataSet: TDataSet; Ent_Sai: Integer;
  oPesquisa: Tpesquisa): TOBJFIN_APROP;
var

  i: Integer;
  ret_aprop, ret_contab, ret_estoque: boolean;
begin
  result := nil;
  ret_aprop := false;
  ret_contab := false;
  ret_estoque := false;
  if (oPesquisa <> nil) and (oPesquisa.Pagina['Aprop'] <> nil) then
    with oPesquisa.Pagina['Aprop'] do
    begin
      for i := 0 to NumLinhas - 1 do
      begin
        if checked[i] and (variavel[i, 0] <> '') then
        begin
          ret_aprop := true;
          break;
        end;
      end;
    end;
  if (oPesquisa <> nil) and (oPesquisa.Pagina['Contab'] <> nil) then
    with oPesquisa.Pagina['Contab'] do
    begin
      for i := 0 to NumLinhas - 1 do
      begin
        if checked[i] and (variavel[i, 0] <> '') then
        begin
          ret_contab := true;
          break;
        end;
      end;
    end;
  if (oPesquisa <> nil) and (oPesquisa.Pagina['Estoque'] <> nil) then
    with oPesquisa.Pagina['Estoque'] do
    begin
      for i := 0 to NumLinhas - 1 do
      begin
        if checked[i] and (variavel[i, 0] <> '') then
        begin
          ret_estoque := true;
          break;
        end;
      end;
    end;

  if (ret_aprop = false) and (ret_contab = false) and (ret_estoque = false) then
  begin
    ret_aprop := true;
    ret_contab := true;
    ret_estoque := true;
  end
  else
    exit;

  result := TOBJFIN_APROP.Create;
  result.ListObjReg := TList.Create;
  result.RegMovFin := TRegMovFin.Create;

  result.Ent_Sai := Ent_Sai;

  with DataSet do
  begin
    if result.Ent_Sai = 2 then
    begin
      result.RegMovFin.Tipo := 'D';
      result.RegMovFin.Conta := FieldbyName('DEBITO').asString;
    end
    else
    begin
      result.RegMovFin.Tipo := 'C';
      result.RegMovFin.Conta := FieldbyName('CREDITO').asString;
    end;
    result.RegMovFin.Valor := FieldbyName('Valor').asFloat;
    result.RegMovFin.HIST := FieldbyName('HIST').asString;
    result.RegMovFin.Data := FieldbyName('Data').asDateTime;
    result.RegMovFin.DocFisc := FieldbyName('DOC_FISC').asString;

    result.RegMovFin.Obs := 'Não Foi Lançado no Pagar e Receber';
    result.Data := result.RegMovFin.Data;

  end;

end;


 */
