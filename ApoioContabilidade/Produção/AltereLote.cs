using ApoioContabilidade.Produção.Servicos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Produção
{
    
    public class AltereLote
    {
        public Eventos_Produto evtProduto;
        private DataRow orowTriag;
        private BindingSource bmLote;
        private BindingSource bmProduto;
        private BindingSource bmSegue;
        double conversao = 30;
        public AltereLote()
        {
            bmLote = new BindingSource();
            bmProduto = new BindingSource();
            bmSegue = new BindingSource();
        }
        public void NovoAltereLote(DataTable adoCloneTriagMov,    bool reconstrua = false )
        {
            orowTriag = adoCloneTriagMov.Rows[0];
           // adoLote = evtProduto.tabProdLote.Clone();
           bmLote.DataSource =  evtProduto.tabProdLote.AsEnumerable().Where(row=>
                   (row.Field<string>("SAFRA") == orowTriag["SAFRA"].ToString()) 
                   && (row.Field<string>("LOTE") == orowTriag["LOTE"].ToString())
                   && (row.Field<string>("SETOR") == orowTriag["SETOR"].ToString())
                   && (row.Field<string>("PROD") == orowTriag["PROD"].ToString())
                  ).AsDataView();
            if (bmLote.Count == 0) return;
            bmProduto.DataSource = evtProduto.tabProdMov.AsEnumerable().Where(row =>
                   (row.Field<string>("SAFRA") == orowTriag["SAFRA"].ToString())
                   && (row.Field<string>("LOTE") == orowTriag["LOTE"].ToString())
                   && (row.Field<string>("SETOR") == orowTriag["SETOR"].ToString())
                   && (row.Field<string>("PROD") == orowTriag["PROD"].ToString())
                  ).AsDataView();

            bmSegue.DataSource = 
            evtProduto.tabRec.AsEnumerable().Where(row =>
                   (row.Field<string>("SAFRA") == orowTriag["SAFRA"].ToString())
                   && (row.Field<string>("LOTE") == orowTriag["LOTE"].ToString())
                   && (row.Field<string>("SETOR") == orowTriag["SETOR"].ToString())
                   && (row.Field<string>("PROD") == orowTriag["PROD"].ToString())
                  ).AsDataView();

            NovoAltereLoteOperacional(adoCloneTriagMov, reconstrua);
        }
        private void NovoAltereLoteOperacional(DataTable adoCloneTriagMov, bool reconstrua = false)
        {
            orowTriag = adoCloneTriagMov.Rows[0];
            double kg_faz = 0;
            double kg_ent = 0;
            double aprove = 0;
            double desaprove = 0;
            double bom = 0;
            double peq = 0;
            double po = 0;
            double mofo = 0;

            DateTime dta_faz;
            DateTime dta_ent;
            DateTime dta_aprove;
            DateTime dta_bulk;
            DateTime dta_catag;
            DateTime dta_res;
            DateTime dta_mofo;

            double tbenef = 0;
            DateTime dta_apronte;
            DateTime dta_campo = (bmProduto.DataSource as DataView).Table.AsEnumerable().Max(row => row.Field<DateTime>("DATAE"));
            double tnatura = (bmProduto.DataSource as DataView).Table.AsEnumerable().Sum(row => row.Field<double>("NQUANT"));


            if (bmProduto.Count > 0)
            {
                foreach (DataRowView rowProd in (bmProduto.DataSource as DataView))
                {

                }
            }

            // soma a quantidade do produto

            // vem da tabela triag_mov
            dta_apronte = Convert.ToDateTime(orowTriag["APRONTE"]);
            dta_faz = dta_apronte;
            // tbenef será o total beneficiado que será o kg_faz ou se diferente o kg_ent
            kg_faz = Convert.ToDouble(orowTriag["KG_FAZ"]);
            tbenef = kg_faz;
            kg_ent = Convert.ToDouble(orowTriag["KG_ENT"]);
            dta_ent = Convert.ToDateTime(orowTriag["DTA_ENT"]);
            if (kg_ent != 0)
                tbenef = kg_ent; // valor que será rateado
            DataRowView rowLote = (bmLote.Current as DataRowView);
            if ( (Convert.ToDateTime(rowLote["TRANSP"]).CompareTo(dta_campo) != 0)
                || (Convert.ToDateTime(rowLote["APRONTE"]).CompareTo(dta_apronte) != 0) )
            {
                rowLote.Row.BeginEdit();
                rowLote.Row["TRANSP"] = dta_campo;
                rowLote.Row["APRONTE"] = dta_apronte;
                rowLote.Row.EndEdit();

            }
            rowLote.Row.BeginEdit();
            rowLote.Row["TIPOPROD"] = orowTriag["TIPOPROD"];
            rowLote.Row["BENEF"] = tbenef;
            rowLote.Row["NATURA"] = tnatura;
            if ((tbenef != 0) && (tnatura != 0))
            {
                rowLote.Row["CONVERSA"] = Math.Round(tbenef / tnatura, 3);
                double dif = Convert.ToDouble(rowLote.Row["CONVERSA"]) - conversao;
            }
        }

           /* 

            with adolote do
                begin
            Edit;
            FieldByName('TIPOPROD').AsInteger := DataSet.FieldByName('TIPOPROD')
              .AsInteger;
            FieldByName('BENEF').asFloat := tbenef;
            FieldByName('NATURA').asFloat := tnatura;
            if (FieldByName('BENEF').asFloat <> 0) and
              (FieldByName('NATURA').asFloat <> 0) then
            begin
      FieldByName('CONVERSA').asFloat :=
        ptRound(FieldByName('BENEF').asFloat / FieldByName('NATURA')
        .asFloat, 3);
        tdif:= FieldByName('CONVERSA').asFloat - conversao;
        tdif:= ptRound(((tdif / conversao) * 100), 2);
            if (tdif > PAD_DIF) or(tdif < (-1 * PAD_DIF)) then
           begin
        ListaErros.Add('Setor:' + FieldByName('SETOR').AsString + '  Safra:' +
          FieldByName('SAFRA').AsString +
          ' (Padrao <> Conversao) Fora do Padrao. Lote N.:' +
          FieldByName('LOTE').AsString);
            end;

            end;

            FieldByName('APRONTE').asDateTime := dta_faz;
            FieldByName('DTA_ENT').asDateTime := DataSet.FieldByName('DTA_ENT')
              .asDateTime;
            FieldByName('DTA_APROVE').asDateTime := DataSet.FieldByName('DTA_APROVE')
              .asDateTime;
            FieldByName('DTA_BULK').asDateTime := DataSet.FieldByName('DTA_BULK')
              .asDateTime;
            FieldByName('DTA_CATAG').asDateTime := DataSet.FieldByName('DTA_CATAG')
              .asDateTime;
            FieldByName('DTA_RES').asDateTime := DataSet.FieldByName('DTA_RES')
              .asDateTime;
            FieldByName('DTA_MOFO').asDateTime := DataSet.FieldByName('DTA_MOFO')
              .asDateTime;

            FieldByName('KG_FAZ').asFloat := DataSet.FieldByName('KG_FAZ').asFloat;
            FieldByName('KG_ENT').asFloat := DataSet.FieldByName('KG_ENT').asFloat;
            FieldByName('APROVE').asFloat := DataSet.FieldByName('APROVE').asFloat;
            FieldByName('DESAPROVE').asFloat :=
              DataSet.FieldByName('DESAPROVE').asFloat;
            FieldByName('BOM').asFloat := DataSet.FieldByName('BOM').asFloat;
            FieldByName('PEQ').asFloat := DataSet.FieldByName('PEQ').asFloat;
            FieldByName('PO').asFloat := DataSet.FieldByName('PO').asFloat;
            FieldByName('MOFO').asFloat := DataSet.FieldByName('MOFO').asFloat;

            // tblAssocCalcFields(DataSet);
            RateieLote(adolote, adoProduto);

            // oexclua := TStringList.create;
            // oexclua.Add('CONVERSAO');
            // GraveRegistro(adolote, dmadofinan.PATH_TRABALHO, 'LOTE', oexclua);
            GraveRegistro(adolote, dmadofinan.PATH_TRABALHO, 'LOTE');
            Post;

            try
      if Reconstrua then
                ReconstruaSegue(adolote, adosegue);

            except
              ShowMessage('Erro ao reconstruir SEGUE');
            end;
            end;

            }

        */

    }
}
/*
 * procedure TAltereLote.NovoAltereLote(DataSet, adolote, adoProduto,
  adosegue: TAdoDataSet; Reconstrua: boolean = false);

//
// recebe o triag_mov com informações para a alteração do lote, produto e segue
// primeiro
//
// estrutura da PRODUTO
// id, setor, gleba, qua, safra, lote, prod, quant, quant_fr, quant_pa, quant_ri,
// datae, nquant (=>datacampo e caixas mole)
// datap (data informado de apronte pela fazenda)
// tipoprod (= tipo do lote pai)
// quant (== soma de todos os tipos de "cacaus"   e (quant_fr => o que pertence a empresa)
// datap e kg_faz  (data apronte e kg informado pelo fazendeiro)
// dta_aprove, aprove, desaprove e dta_bulk
// dta_catag , bom, peq, po, dta_res
// dta_mofo,mofo
// kg_ent  // kilos que o deposito pesou


// estrutura do LOTE
// id, setor, prod, safra, lote, u_benef,   '
// ' fecha, quant_fr, quant_pa, quant_ri,  b_rateio, b_venda, tipoprod,  mofo,
// dta_mofo
// transp(data campo) , natura( total caixas)
// apronte(data faz) //  benef (total kgs fazenda inicialmente e se o kg_ent for igual) o (benef deve virar kg_faz) quando o kg_ent divergir do kg_faz
// dta_ent e kg_ent // quilos do deposito que receber o cacau da fazenda...
// dta_aprove,aprove, desaprove, data_bulk(data_bulk se este campo estiver vazio, o cacau bulk está em nanci)
// dta_catag , bom, peq, po, dta_res  ( se dta_res está vazio > está em nanci)
// dta_mofo,mofo


// ESTRUTURA SEGUE
// SELECT        id, safra, lote, prod, setor, data, quant, scs, tipoprod

var
  ind: integer;

  bMark: TBytes;
  strprodsafra: string;

  // adoLote, adoProduto, adosegue: TAdoDataSet;
begin

  // adoLote := TAdoDataSet.Create(nil);
  // adoProduto := TAdoDataSet.Create(nil);
  // adosegue := TAdoDataSet.Create(nil);

  adolote.Filtered := false;
  adolote.Filter := '(LOTE = ' + QuotedStr(DataSet.FieldByName('LOTE').AsString)
    + ') ' + ' AND (SETOR = ' + QuotedStr(DataSet.FieldByName('SETOR').AsString)
    + ') ' + ' AND (SAFRA = ' + QuotedStr(DataSet.FieldByName('SAFRA').AsString)
    + ') ' + ' AND (PROD = ' + QuotedStr(DataSet.FieldByName('PROD')
    .AsString) + ') ';

  // adolote.Filter := 'ID = ' + DataSet.FieldByName('ID_LOTE').AsString; em abril de 2016
  // alterei

  adolote.Filtered := true;

  if adolote.RecordCount = 0 then
    exit;

  adoProduto.Filtered := false;
  adoProduto.Filter := '(LOTE = ' + QuotedStr(adolote.FieldByName('LOTE')
    .AsString) + ') ' + ' AND (SETOR = ' +
    QuotedStr(adolote.FieldByName('SETOR').AsString) + ') ' + ' AND (SAFRA = ' +
    QuotedStr(adolote.FieldByName('SAFRA').AsString) + ') ' + ' AND (PROD = ' +
    QuotedStr(adolote.FieldByName('PROD').AsString) + ') ';
  adoProduto.Filtered := true;

  adosegue.Filtered := false;
  adosegue.Filter := '(LOTE = ' + QuotedStr(adolote.FieldByName('LOTE')
    .AsString) + ')' + ' AND (SETOR = ' +
    QuotedStr(adolote.FieldByName('SETOR').AsString) + ') ' + ' AND (SAFRA = ' +
    QuotedStr(adolote.FieldByName('SAFRA').AsString) + ') ' + ' AND (PROD = ' +
    QuotedStr(adolote.FieldByName('PROD').AsString) + ') ';
  adosegue.Filtered := true;

  NovoAltereLoteOperacional(DataSet, adolote, adoProduto, adosegue, Reconstrua);

end;

procedure TAltereLote.NovoAltereLoteOperacional(DataSet, adolote, adoProduto,
  adosegue: TAdoDataSet; Reconstrua: boolean = false);
var
  dta_apronte, dta_campo: TDateTime;
  tbenef, tnatura: double;
  bMark: TBytes;
  // tbeforepost: TDataSetNotifyEvent;
  nreg: LongInt;
  ind, i: integer;
  oExclua: TStrings;

  kg_faz, kg_ent, aprove, desaprove, bom, peq, po, mofo: double;
  dta_faz, dta_ent, dta_aprove, dta_bulk, dta_catag, dta_res, dta_mofo: TDate;
  tdif: double;
begin

  kg_faz := 0;
  kg_ent := 0;
  aprove := 0;
  desaprove := 0;
  bom := 0;
  peq := 0;
  po := 0;
  mofo := 0;

  dta_faz := 0;
  dta_ent := 0;
  dta_aprove := 0;
  dta_bulk := 0;
  dta_catag := 0;
  dta_res := 0;
  dta_mofo := 0;

  tbenef := 0;
  tnatura := 0;
  dta_apronte := 0;
  dta_campo := 0;

  // soma a quantidade do produto
  with adoProduto do
  begin
    if adoProduto.RecordCount > 0 then
    begin
      first;
      while not eof do
      begin

        if (dta_campo < FieldByName('DATAE').asDateTime) then
          dta_campo := FieldByName('DATAE').asDateTime;

        tnatura := tnatura + FieldByName('NQUANT').asFloat;
        next;
      end;
    end;
  end;

  // vem da tabela triag_mov
  dta_apronte := DataSet.FieldByName('APRONTE').asDateTime;
  dta_faz := dta_apronte;
  // tbenef será o total beneficiado que será o kg_faz ou se diferente o kg_ent
  kg_faz := DataSet.FieldByName('KG_FAZ').asFloat;
  tbenef := kg_faz;
  kg_ent := DataSet.FieldByName('KG_ENT').asFloat;
  dta_ent := DataSet.FieldByName('DTA_ENT').asDateTime;
  if kg_ent <> 0 then
    tbenef := kg_ent; // valor que será rateado



  // verifica das quantidade

  // soma a quantidade de cacau do segue (cacau pronto) e pesquisa
  // a menor data de apronte

  { with adoSegue do
    begin
    if not adosegue.eof then
    begin
    first;
    while not eof do
    begin
    if (dta_apronte < FieldByName('DATA').asDateTime) then
    dta_apronte := FieldByName('DATA').asDateTime;
    tbenef := tbenef + FieldByName('QUANT').asFloat;
    next;
    end;
    end;
    end;
  }

  with adolote do
  begin
    if (dta_campo <> FieldByName('TRANSP').asDateTime) or
      (dta_apronte <> FieldByName('APRONTE').asDateTime) then
    begin
      Edit;
      FieldByName('TRANSP').asDateTime := dta_campo;
      FieldByName('APRONTE').asDateTime := dta_apronte;
      Post;
    end;

    Edit;
    FieldByName('TIPOPROD').AsInteger := DataSet.FieldByName('TIPOPROD')
      .AsInteger;
    FieldByName('BENEF').asFloat := tbenef;
    FieldByName('NATURA').asFloat := tnatura;
    if (FieldByName('BENEF').asFloat <> 0) and
      (FieldByName('NATURA').asFloat <> 0) then
    begin
      FieldByName('CONVERSA').asFloat :=
        ptRound(FieldByName('BENEF').asFloat / FieldByName('NATURA')
        .asFloat, 3);
      tdif := FieldByName('CONVERSA').asFloat - conversao;
      tdif := ptRound(((tdif / conversao) * 100), 2);
      if (tdif > PAD_DIF) or (tdif < (-1 * PAD_DIF)) then
      begin
        ListaErros.Add('Setor:' + FieldByName('SETOR').AsString + '  Safra:' +
          FieldByName('SAFRA').AsString +
          ' (Padrao <> Conversao) Fora do Padrao. Lote N.:' +
          FieldByName('LOTE').AsString);
      end;

    end;

    FieldByName('APRONTE').asDateTime := dta_faz;
    FieldByName('DTA_ENT').asDateTime := DataSet.FieldByName('DTA_ENT')
      .asDateTime;
    FieldByName('DTA_APROVE').asDateTime := DataSet.FieldByName('DTA_APROVE')
      .asDateTime;
    FieldByName('DTA_BULK').asDateTime := DataSet.FieldByName('DTA_BULK')
      .asDateTime;
    FieldByName('DTA_CATAG').asDateTime := DataSet.FieldByName('DTA_CATAG')
      .asDateTime;
    FieldByName('DTA_RES').asDateTime := DataSet.FieldByName('DTA_RES')
      .asDateTime;
    FieldByName('DTA_MOFO').asDateTime := DataSet.FieldByName('DTA_MOFO')
      .asDateTime;

    FieldByName('KG_FAZ').asFloat := DataSet.FieldByName('KG_FAZ').asFloat;
    FieldByName('KG_ENT').asFloat := DataSet.FieldByName('KG_ENT').asFloat;
    FieldByName('APROVE').asFloat := DataSet.FieldByName('APROVE').asFloat;
    FieldByName('DESAPROVE').asFloat :=
      DataSet.FieldByName('DESAPROVE').asFloat;
    FieldByName('BOM').asFloat := DataSet.FieldByName('BOM').asFloat;
    FieldByName('PEQ').asFloat := DataSet.FieldByName('PEQ').asFloat;
    FieldByName('PO').asFloat := DataSet.FieldByName('PO').asFloat;
    FieldByName('MOFO').asFloat := DataSet.FieldByName('MOFO').asFloat;

    // tblAssocCalcFields(DataSet);
    RateieLote(adolote, adoProduto);

    // oexclua := TStringList.create;
    // oexclua.Add('CONVERSAO');
    // GraveRegistro(adolote, dmadofinan.PATH_TRABALHO, 'LOTE', oexclua);
    GraveRegistro(adolote, dmadofinan.PATH_TRABALHO, 'LOTE');
    Post;

    try
      if Reconstrua then
        ReconstruaSegue(adolote, adosegue);

    except
      ShowMessage('Erro ao reconstruir SEGUE');
    end;
  end;
end;

{ class procedure TAltereLote.NovoRateieLote(DataSet, adoProduto,
  adosegue: TAdoDataSet);
  begin
  //
  end;
}

procedure TAltereLote.RateieLote(DataSet, adoProduto: TAdoDataSet);
var
  tdif, tvalor, maiornquant, tot1: double;
  tquant_FR, tquant_PA, tquant_RI: double;
  bMark, bMarkMaior: TBytes;
  tregMaior: LongInt;
  totQuant, totquant_FR, totquant_PA, totquant_RI: double;
  nreg, i: integer;
  tdatap: TDateTime;
  tcodparc, tcontraparc: string;
  oExclua, oinclua: TStrings;
  str, strExecScript: string;
  oArray: TJSONArray;

  // rever
  operador: double;
  aprove, desaprove, bom, po, peq, mofo, kg_faz, kg_ent: double;

  tot_aprove, tot_desaprove, tot_bom, tot_po, tot_peq, tot_mofo, tot_kg_faz,
    tot_kg_ent: double;

  tipoprod: integer;

  // conversao, parceria, reinvest: double;

begin
  try

    if (adoProduto.RecordCount = 0) then
    begin
      DataSet.FieldByName('QUANT_FR').asFloat := 0;
      DataSet.FieldByName('QUANT_PA').asFloat := 0;
      DataSet.FieldByName('QUANT_RI').asFloat := 0;
      exit;

    end;
    tdatap := DataSet.FieldByName('APRONTE').asDateTime;

    // zera tudo para trabalhar o produto
    with adoProduto do
    begin
      first;
      // oArray := TJSONArray.Create;

      while not eof do
      begin
        Edit;
        FieldByName('QUANT').asFloat := 0;
        FieldByName('QUANT_FR').asFloat := 0;
        FieldByName('QUANT_PA').asFloat := 0;
        FieldByName('QUANT_RI').asFloat := 0;
        FieldByName('DATAP').asDateTime := tdatap;
        FieldByName('CODPARC').AsString := '';
        FieldByName('NCONTRA').AsString := '';
        FieldByName('TIPOPROD').AsInteger := DataSet.FieldByName('TIPOPROD')
          .AsInteger;
        // FieldByName('DTA_ENT').asDateTime := DataSet.FieldByName('DTA_ENT').asDateTime;
        FieldByName('DTA_APROVE').asDateTime :=
          DataSet.FieldByName('DTA_APROVE').asDateTime;
        FieldByName('DTA_BULK').asDateTime := DataSet.FieldByName('DTA_BULK')
          .asDateTime;
        FieldByName('DTA_CATAG').asDateTime := DataSet.FieldByName('DTA_CATAG')
          .asDateTime;
        FieldByName('DTA_RES').asDateTime := DataSet.FieldByName('DTA_RES')
          .asDateTime;
        FieldByName('DTA_MOFO').asDateTime := DataSet.FieldByName('DTA_MOFO')
          .asDateTime;

        FieldByName('KG_FAZ').asFloat := 0;
        // DataSet.FieldByName('KG_FAZ').asFloat;
        FieldByName('KG_ENT').asFloat := 0;
        // DataSet.FieldByName('KG_ENT').asFloat;
        FieldByName('APROVE').asFloat := 0;
        // DataSet.FieldByName('APROVE').asFloat;
        FieldByName('DESAPROVE').asFloat := 0;
        // DataSet.FieldByName('DESAPROVE').asFloat;
        FieldByName('BOM').asFloat := 0;
        // DataSet.FieldByName('BOM').asFloat;
        FieldByName('PEQ').asFloat := 0;
        // DataSet.FieldByName('PEQ').asFloat;
        FieldByName('PO').asFloat := 0;
        // DataSet.FieldByName('PO').asFloat;
        FieldByName('MOFO').asFloat := 0;

        Post;
        next;

      end;

    end;

    // end;

    with adoProduto do
    begin
      // se não houver caixas lancadas zera lote

      first;

      totQuant := 0;
      totquant_FR := 0;
      totquant_PA := 0;
      totquant_RI := 0;

      bMarkMaior := nil;
      maiornquant := 0;
      tot1 := DataSet.FieldByName('BENEF').asFloat;

      tot_aprove := DataSet.FieldByName('APROVE').asFloat;
      tot_desaprove := DataSet.FieldByName('DESAPROVE').asFloat;
      tot_bom := DataSet.FieldByName('BOM').asFloat;
      tot_peq := DataSet.FieldByName('PEQ').asFloat;
      tot_po := DataSet.FieldByName('PO').asFloat;
      tot_mofo := DataSet.FieldByName('MOFO').asFloat;

      // tdif := (DataSet.FieldByName('BENEF').asFloat / DataSet.FieldByName('NATURA').asFloat);
      while not eof do
      begin
        // tvalor := ptRound(FieldByName('NQUANT').asFloat * tdif, 2);

        tdif := round((FieldByName('NQUANT').asFloat /
          DataSet.FieldByName('NATURA').asFloat) * 100000000) / 100000000;

        tvalor := ptRound((tdif * DataSet.FieldByName('BENEF').asFloat), 4);

        if (DataSet.FieldByName('APROVE').asFloat <> 0) then
        begin
          aprove := ptRound((tdif * DataSet.FieldByName('APROVE').asFloat), 4);
          tot_aprove := tot_aprove - aprove;
        end
        else
          aprove := 0;

        if (DataSet.FieldByName('DESAPROVE').asFloat <> 0) then
        begin
          desaprove :=
            ptRound((tdif * DataSet.FieldByName('DESAPROVE').asFloat), 4);
          tot_desaprove := tot_desaprove - desaprove;
        end
        else
          desaprove := 0;

        if (DataSet.FieldByName('bom').asFloat <> 0) then
        begin
          bom := ptRound((tdif * DataSet.FieldByName('bom').asFloat), 4);
          tot_bom := tot_bom - bom;
        end
        else
          bom := 0;

        if (DataSet.FieldByName('peq').asFloat <> 0) then
        begin
          peq := ptRound((tdif * DataSet.FieldByName('peq').asFloat), 4);
          tot_peq := tot_peq - peq;
        end
        else
          peq := 0;

        if (DataSet.FieldByName('po').asFloat <> 0) then
        begin
          po := ptRound((tdif * DataSet.FieldByName('po').asFloat), 4);
          tot_po := tot_po - po;
        end
        else
          po := 0;

        if (DataSet.FieldByName('mofo').asFloat <> 0) then
        begin
          mofo := ptRound((tdif * DataSet.FieldByName('mofo').asFloat), 4);
          tot_mofo := tot_mofo - mofo;
        end
        else
          mofo := 0;

        tot1 := tot1 - tvalor;

        if FieldByName('NQUANT').asFloat > maiornquant then
        // o destino da sobra do rateio vai p/ o maior valor
        begin
          maiornquant := FieldByName('NQUANT').asFloat;
          bMarkMaior := GetBookMark;
          // tregmaior := recno;
        end;
        tquant_FR := 0;
        tquant_PA := 0;
        tquant_RI := 0;
        tcodparc := '';
        tcontraparc := '';
        //

        // if (oParceria <> nil) then
        // begin
        oParceria.VerifiqueParceria(FieldByName('QUA').AsString,
          FieldByName('GLEBA').AsString, FieldByName('PROD').AsString,
          FieldByName('DATAE').asDateTime);
        if oParceria.EParceiro then
        begin

          if parceria <> 0 then
          begin
            tquant_PA := ptRound(tvalor * parceria / 100, 3);
            tcodparc := oParceria.codParc;
            tcontraparc := oParceria.nContra;
          end;
          tquant_FR := (tvalor - tquant_PA);

          if reinvest <> 0 then
          begin
            tquant_RI := ptRound(tquant_PA * reinvest / 100, 3);
            tquant_PA := tquant_PA - tquant_RI;
          end;
        end
        else
          tquant_FR := tvalor;
        // end;

        Edit;
        FieldByName('QUANT').asFloat := tvalor;
        FieldByName('APROVE').asFloat := aprove;
        FieldByName('DESAPROVE').asFloat := desaprove;
        FieldByName('BOM').asFloat := bom;
        FieldByName('PEQ').asFloat := peq;
        FieldByName('PO').asFloat := po;
        FieldByName('MOFO').asFloat := mofo;
        FieldByName('QUANT_FR').asFloat := tquant_FR;
        FieldByName('QUANT_PA').asFloat := tquant_PA;
        FieldByName('QUANT_RI').asFloat := tquant_RI;
        FieldByName('CODPARC').AsString := tcodparc;
        FieldByName('NCONTRA').AsString := tcontraparc;
        FieldByName('DATAP').asDateTime := tdatap;
        Post;

        totQuant := totQuant + FieldByName('QUANT').asFloat;
        totquant_FR := totquant_FR + FieldByName('QUANT_FR').asFloat;
        totquant_PA := totquant_PA + FieldByName('QUANT_PA').asFloat;
        totquant_RI := totquant_RI + FieldByName('QUANT_RI').asFloat;

        next;

      end;

      // apura sobras de rateio
      if tot1 <> 0 then
      // Coloca no Maior Valor a Diferenca que Houver do Rateio
      begin
        // GotoNReg(tregMaior);
        GotoBookMark(bMarkMaior);
        tvalor := FieldByName('QUANT').asFloat + tot1;
        aprove := FieldByName('APROVE').asFloat + tot_aprove;
        desaprove := FieldByName('DESAPROVE').asFloat + tot_desaprove;
        bom := FieldByName('bom').asFloat + tot_bom;
        peq := FieldByName('peq').asFloat + tot_peq;
        po := FieldByName('po').asFloat + tot_po;
        mofo := FieldByName('mofo').asFloat + tot_mofo;

        tquant_FR := 0;
        tquant_PA := 0;
        tquant_RI := 0;
        totquant_FR := totquant_FR - FieldByName('QUANT_FR').asFloat;
        totquant_PA := totquant_PA - FieldByName('QUANT_PA').asFloat;
        totquant_RI := totquant_RI - FieldByName('QUANT_RI').asFloat;
        totQuant := totQuant - FieldByName('QUANT').asFloat;

        oParceria.VerifiqueParceria(FieldByName('QUA').AsString,
          FieldByName('GLEBA').AsString, FieldByName('PROD').AsString,
          FieldByName('DATAE').asDateTime);
        if oParceria.EParceiro then
        begin
          if parceria <> 0 then
            tquant_PA := ptRound(tvalor * (parceria / 100), 3);

          tquant_FR := (tvalor - tquant_PA);
          if reinvest <> 0 then
          begin
            tquant_RI := ptRound(tquant_PA * (reinvest / 100), 3);
            tquant_PA := tquant_PA - tquant_RI;
          end;
        end
        else
          tquant_FR := tvalor;

        Edit;
        FieldByName('QUANT').asFloat := tvalor;
        FieldByName('APROVE').asFloat := aprove;
        FieldByName('DESAPROVE').asFloat := desaprove;
        FieldByName('BOM').asFloat := bom;
        FieldByName('PEQ').asFloat := peq;
        FieldByName('PO').asFloat := po;
        FieldByName('MOFO').asFloat := mofo;
        FieldByName('QUANT_FR').asFloat := tquant_FR;
        FieldByName('QUANT_PA').asFloat := tquant_PA;
        FieldByName('QUANT_RI').asFloat := tquant_RI;
        FieldByName('DATAP').asDateTime := tdatap;
        Post;

        totQuant := totQuant + FieldByName('QUANT').asFloat;
        totquant_FR := totquant_FR + FieldByName('QUANT_FR').asFloat;
        totquant_PA := totquant_PA + FieldByName('QUANT_PA').asFloat;
        totquant_RI := totquant_RI + FieldByName('QUANT_RI').asFloat;
      end;
      // agora grave
      adoProduto.first;
      // construa grabação em bloco..
      oArray := TJSONArray.create;
      // oArray.Add('0');  // marca que vai ser tipo execscript

      while not adoProduto.eof do
      begin
        oExclua := TStringList.create;
        oExclua.Add('QUANT_PA_RI');
        str := ConstruaAlteracao(adoProduto, 'PRODUTO', 'ID', oExclua);
        oArray.Add(str);
        if oArray.Count > 3 then
        begin

           try
              GenericoAdoEditeDiversos(oArray, dmadofinan.PATH_TRABALHO);
           except
               ShowMessage('Erro ao Gravar Registros ');
           end;

           oArray.Destroy;
           oArray := TJSONArray.create;
        end;
        adoProduto.next;
      end;
      try
        // ShowMessage('numero'+inttostr(oArray.Count)+ ' ' +oArray.Items[0].Value );
        if oArray.Count > 0 then
          GenericoAdoEditeDiversos(oArray, dmadofinan.PATH_TRABALHO);

      except
        tvalor := 0;
        // ShowMessage('numero'+inttostr(oArray.Count)+ ' ' +oArray.Items[0].Value );
      end;
      // edit;
      DataSet.FieldByName('QUANT_FR').asFloat := totquant_FR;
      DataSet.FieldByName('QUANT_PA').asFloat := totquant_PA;
      DataSet.FieldByName('QUANT_RI').asFloat := totquant_RI;

    end;
  finally
  end;

end;

procedure TAltereLote.ReconstruaSegue(DataSet, adosegue: TAdoDataSet);
var

  tot1, tot_aprove, tot_desaprove, tot_bom, tot_po, tot_peq, tot_mofo,
    tot_kg_faz, tot_kg_ent: double;

  tipoprod: integer;
  dta_apronte, dta_faz, dta_ent: TDate;
begin
  //
  while adosegue.RecordCount > 0 do
  begin
    DeleteRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
    adosegue.Delete;
  end;

  tipoprod := DataSet.FieldByName('TIPOPROD').AsInteger;
  dta_apronte := DataSet.FieldByName('APRONTE').asDateTime;
  dta_faz := dta_apronte;
  // tbenef será o total beneficiado que será o kg_faz ou se diferente o kg_ent
  // kg_faz := DataSet.FieldByName('KG_FAZ').asFloat;
  // tbenef := kg_faz;
  // kg_ent := DataSet.FieldByName('KG_ENT').asFloat;
  dta_ent := DataSet.FieldByName('DTA_ENT').asDateTime;
  // if kg_ent <> 0 then
  // tbenef := kg_ent; // valor que será rateado

  // if (dta_ent > dta_faz) then
  // dta_apronte := dta_ent;

  tot1 := DataSet.FieldByName('BENEF').asFloat;

  tot_aprove := DataSet.FieldByName('APROVE').asFloat;
  tot_desaprove := DataSet.FieldByName('DESAPROVE').asFloat;
  tot_bom := DataSet.FieldByName('BOM').asFloat;
  tot_peq := DataSet.FieldByName('PEQ').asFloat;
  tot_po := DataSet.FieldByName('PO').asFloat;
  tot_mofo := DataSet.FieldByName('MOFO').asFloat;


  // Nova Situação (quando o bulk é triado)
  if (tipoprod = 0) and (tot_po <> 0)then
  begin
      adosegue.Insert;
      adosegue.FieldByName('DATA').asDateTime := dta_faz;
      adosegue.FieldByName('QUANT').asFloat := tot1-tot_po;
      adosegue.FieldByName('SAFRA').AsString :=
       DataSet.FieldByName('SAFRA').AsString;
      adosegue.FieldByName('LOTE').AsString :=
        DataSet.FieldByName('LOTE').AsString;
      adosegue.FieldByName('PROD').AsString :=
        DataSet.FieldByName('PROD').AsString;
      adosegue.FieldByName('SETOR').AsString :=
        DataSet.FieldByName('SETOR').AsString;
      GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
      adosegue.Post;

      adosegue.Insert;
      adosegue.FieldByName('DATA').asDateTime := dta_faz;
      adosegue.FieldByName('QUANT').asFloat := tot_po;
      adosegue.FieldByName('TIPOPROD').AsInteger := 33;
      adosegue.FieldByName('SAFRA').AsString :=
       DataSet.FieldByName('SAFRA').AsString;
      adosegue.FieldByName('LOTE').AsString :=
        DataSet.FieldByName('LOTE').AsString;
      adosegue.FieldByName('PROD').AsString :=
        DataSet.FieldByName('PROD').AsString;
      adosegue.FieldByName('SETOR').AsString :=
        DataSet.FieldByName('SETOR').AsString;
      GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
      adosegue.Post;
    exit;

  end;


  if (tot_aprove + tot_desaprove) = 0 then
  begin
    // um lançamento só
    adosegue.Insert;
    adosegue.FieldByName('DATA').asDateTime := dta_faz;
    adosegue.FieldByName('QUANT').asFloat := tot1;
    case tipoprod of
      1:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 10;
        end;
      11:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 20;
        end;
    end;
    adosegue.FieldByName('SAFRA').AsString :=
      DataSet.FieldByName('SAFRA').AsString;
    adosegue.FieldByName('LOTE').AsString :=
      DataSet.FieldByName('LOTE').AsString;
    adosegue.FieldByName('PROD').AsString :=
      DataSet.FieldByName('PROD').AsString;
    adosegue.FieldByName('SETOR').AsString :=
      DataSet.FieldByName('SETOR').AsString;
    GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
    adosegue.Post;
    exit;
  end;

  if (tot_bom + tot_peq + tot_po) = 0 then
  begin
    if (tot_aprove <> 0) then
    begin
      adosegue.Insert;
      adosegue.FieldByName('DATA').asDateTime := dta_faz;
      // DataSet.FieldByName('DTA_APROVE').asDateTime;
      adosegue.FieldByName('QUANT').asFloat := tot_aprove;
      case tipoprod of
        1:
          begin
            adosegue.FieldByName('TIPOPROD').AsInteger := 10;
          end;
        11:
          begin
            adosegue.FieldByName('TIPOPROD').AsInteger := 20;
          end;
      end;
      adosegue.FieldByName('SAFRA').AsString :=
        DataSet.FieldByName('SAFRA').AsString;
      adosegue.FieldByName('LOTE').AsString :=
        DataSet.FieldByName('LOTE').AsString;
      adosegue.FieldByName('PROD').AsString :=
        DataSet.FieldByName('PROD').AsString;
      adosegue.FieldByName('SETOR').AsString :=
        DataSet.FieldByName('SETOR').AsString;
      GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
      adosegue.Post;
    end;
    if (tot_desaprove <> 0) then
    begin
      adosegue.Insert;
      adosegue.FieldByName('DATA').asDateTime := dta_faz;
      // DataSet.FieldByName('DTA_APROVE').asDateTime;
      adosegue.FieldByName('QUANT').asFloat := tot_desaprove;
      case tipoprod of
        1:
          begin
            adosegue.FieldByName('TIPOPROD').AsInteger := 2;
          end;
        11:
          begin
            adosegue.FieldByName('TIPOPROD').AsInteger := 22;
          end;
      end;
      adosegue.FieldByName('SAFRA').AsString :=
        DataSet.FieldByName('SAFRA').AsString;
      adosegue.FieldByName('LOTE').AsString :=
        DataSet.FieldByName('LOTE').AsString;
      adosegue.FieldByName('PROD').AsString :=
        DataSet.FieldByName('PROD').AsString;
      adosegue.FieldByName('SETOR').AsString :=
        DataSet.FieldByName('SETOR').AsString;
      GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
      adosegue.Post;
    end;
  end;


  if (tot_bom + tot_peq + tot_po) <> 0 then
  begin
    if (tot_desaprove <> 0) then
    begin
      adosegue.Insert;
      adosegue.FieldByName('DATA').asDateTime := dta_faz;
      // DataSet.FieldByName('DTA_APROVE').asDateTime;
      adosegue.FieldByName('QUANT').asFloat := tot_desaprove;
      case tipoprod of
        1:
          begin
            adosegue.FieldByName('TIPOPROD').AsInteger := 2;
          end;
        11:
          begin
            adosegue.FieldByName('TIPOPROD').AsInteger := 22;
          end;
      end;
      adosegue.FieldByName('SAFRA').AsString :=
        DataSet.FieldByName('SAFRA').AsString;
      adosegue.FieldByName('LOTE').AsString :=
        DataSet.FieldByName('LOTE').AsString;
      adosegue.FieldByName('PROD').AsString :=
        DataSet.FieldByName('PROD').AsString;
      adosegue.FieldByName('SETOR').AsString :=
        DataSet.FieldByName('SETOR').AsString;
      GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
      adosegue.Post;
    end;
  end;







  if (tot_mofo <> 0) then
  begin
    adosegue.Insert;
    adosegue.FieldByName('DATA').asDateTime := dta_faz;
    adosegue.FieldByName('QUANT').asFloat := tot_mofo;
    case tipoprod of
      1:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 5;
        end;
      11:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 25;
        end;
    end;
    adosegue.FieldByName('SAFRA').AsString :=
      DataSet.FieldByName('SAFRA').AsString;
    adosegue.FieldByName('LOTE').AsString :=
      DataSet.FieldByName('LOTE').AsString;
    adosegue.FieldByName('PROD').AsString :=
      DataSet.FieldByName('PROD').AsString;
    adosegue.FieldByName('SETOR').AsString :=
      DataSet.FieldByName('SETOR').AsString;
    GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
    adosegue.Post;
  end;
  tot_bom := tot_bom - tot_mofo;
  if tot_bom > 0 then
  begin
    adosegue.Insert;
    adosegue.FieldByName('DATA').asDateTime := dta_faz;
    // DataSet.FieldByName('DTA_CATAG')
    // .asDateTime;
    adosegue.FieldByName('QUANT').asFloat := tot_bom;
    case tipoprod of
      1:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 1;
        end;
      11:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 11;
        end;
    end;
    adosegue.FieldByName('SAFRA').AsString :=
      DataSet.FieldByName('SAFRA').AsString;
    adosegue.FieldByName('LOTE').AsString :=
      DataSet.FieldByName('LOTE').AsString;
    adosegue.FieldByName('PROD').AsString :=
      DataSet.FieldByName('PROD').AsString;
    adosegue.FieldByName('SETOR').AsString :=
      DataSet.FieldByName('SETOR').AsString;
    GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
    adosegue.Post;


  end;
  if tot_peq > 0 then
  begin
    adosegue.Insert;
    adosegue.FieldByName('DATA').asDateTime := dta_faz;
    // DataSet.FieldByName('DTA_CATAG')
    // .asDateTime;
    adosegue.FieldByName('QUANT').asFloat := tot_peq;
    case tipoprod of
      1:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 3;
        end;
      11:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 23;
        end;
    end;
    adosegue.FieldByName('SAFRA').AsString :=
      DataSet.FieldByName('SAFRA').AsString;
    adosegue.FieldByName('LOTE').AsString :=
      DataSet.FieldByName('LOTE').AsString;
    adosegue.FieldByName('PROD').AsString :=
      DataSet.FieldByName('PROD').AsString;
    adosegue.FieldByName('SETOR').AsString :=
      DataSet.FieldByName('SETOR').AsString;
    GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
    adosegue.Post;
  end;
  if tot_po > 0 then
  begin
    adosegue.Insert;
    adosegue.FieldByName('DATA').asDateTime := dta_faz;
    // DataSet.FieldByName('DTA_CATAG')
    // .asDateTime;
    adosegue.FieldByName('QUANT').asFloat := tot_po;
    case tipoprod of
      1:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 4;
        end;
      11:
        begin
          adosegue.FieldByName('TIPOPROD').AsInteger := 24;
        end;
    end;
    adosegue.FieldByName('SAFRA').AsString :=
      DataSet.FieldByName('SAFRA').AsString;
    adosegue.FieldByName('LOTE').AsString :=
      DataSet.FieldByName('LOTE').AsString;
    adosegue.FieldByName('PROD').AsString :=
      DataSet.FieldByName('PROD').AsString;
    adosegue.FieldByName('SETOR').AsString :=
      DataSet.FieldByName('SETOR').AsString;
    GraveRegistro(adosegue, dmadofinan.PATH_TRABALHO, 'SEGUE');
    adosegue.Post;
  end;

end;

 */ 