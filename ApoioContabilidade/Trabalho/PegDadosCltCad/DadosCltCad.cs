using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Trabalho.PegDadosCltCad
{
    public class DadosCltCad
    {
        MonteGrid filtro;

        public DadosCltCad(MonteGrid ofiltro)
        {
            filtro = ofiltro;
        }
        public void PegDados()
        {

         /*datastring:= oPesquisa.Criterio['CADASTRO'].dump;
            if (datastring <> '') then
              datastring := datastring + ' AND ';*/
       
        }


    }

    /*
     * function TListaNovos.AdoPegDadoFiltro(oFiltro: TAdoFiltroEspecial): TAdoDataSet;
var

  mselecao: TGridRect;
  tRecno: LongInt;
  z, ent_sai, i, ind1, colunas, linhas: integer;
  Inicio: TPoint;
  velhocursor: TCursor;
  // oreg: TRegAssoc;
  omark: TobjMark;
begin
  if BuscaAuto then
    EditParent := Edit.Parent;

  result := nil;
  if oFiltro.oTable.Eof then
    exit;

  result := TAdoDataSet.Create(nil);

  with oFiltro do
  begin
    if oTable.active = false then
      oTable.active := true;
    oTable.First;
  end;

  Inicio := oWinControl.ClientOrigin; // o parent é tabshet

  Application.CreateForm(TForm3, Form3);
  with Form3 do
  begin
    if cabecalhos.Count > 0 then
      Caption := cabecalhos[0];
    Align := alnone;
    ClientWidth := 500;
    ClientHeight := 450;
    Left := Inicio.x + 50;
    Top := Inicio.y + 50;
  end;

  if InibeRadioGroup1 then
    Form3.RadioGroup1.Visible := false;

  
  with Form3.AdoSgDados do
  begin
    for ind1 := 0 to RowCount - 1 do
      Rows[ind1].Clear;
    for ind1 := 0 to ColCount - 1 do
      Cols[ind1].Clear;

    ColCount := campos.Count + 1;
    RowCount := oFiltro.oTable.RecordCount;

    if RowCount < 2 then
      RowCount := 2;
    FixedRows := 1;
    FixedCols := 0;
    // tam da coluna fixa
    ColWidths[0] := 50;
    // Montagem do cabecário de 2 linhas
    // ind2
    colunas := 1;
    linhas := 0;
    width := 50;
    with oFiltro.oTable do
    begin
      First;

      for i := 0 to campos.Count - 1 do
      begin
        if ((i + 1) < cabecalhos.Count) then
          Cells[colunas, linhas] := cabecalhos[i + 1];

        ColWidths[colunas] :=
          (FindField(campos[i]).displaywidth * incremento_tam);
        if ColWidths[colunas] < (Length(Cells[colunas, linhas]) * incremento_tam)
        then
          ColWidths[colunas] :=
            (Length(Cells[colunas, linhas]) * incremento_tam);
        Form3.AdoSgDados.width := Form3.AdoSgDados.width + ColWidths[colunas] +
          (incremento_tam + (incremento_tam div 2));
        INC(colunas, 1);
      end;
    end;

    linhas := FixedRows;
    colunas := FixedCols;

    // prepare o adosgdados para filtrar...
    Form3.oFiltro := oFiltro;
    Form3.campos := campos;
    Cells[0, 0] := 'X';
    with oFiltro.oTable do
    begin
      First;
      while not Eof do
      begin
        if (RowCount - 1) < linhas then
          RowCount := linhas + 1;
        for i := 0 to campos.Count - 1 do
        begin
          if (FindField(campos[i]).Datatype in [ftFloat]) then
            Cells[i + 1, linhas] := Completestring(Canvas,
              floattostrf(fieldbyname(campos[i]).asFloat, ffnumber, 12, 2),
              ColWidths[i + 1], false)
          else
            Cells[i + 1, linhas] := fieldbyname(campos[i]).asString;
        end;
        omark := TobjMark.Create;
        omark.bMark := GetBookMark;
        omark.mudou := false;
        Objects[1, linhas] := omark;
        {
        }
        INC(linhas, 1);
        next;
      end;
    end;
  end;
  with Form3.AdoSgDados do
  begin
    if width > Form3.ClientWidth then
      Form3.ClientWidth := width + 30;
    if (width + 30) < Form3.ClientWidth then
      Form3.ClientWidth := width + 30;
    Height := Form3.ClientHeight - 100;
    Align := alLeft;
    Left := 0;
    if BuscaAuto then
    begin
      Edit.Parent := Form3;
      Edit.Visible := true;
      Edit.Text := '';
      Edit.Top := 5;
      Edit.Left := 10;
      Top := 30;
      Edit.TabOrder := 0;
      TabOrder := 1;
    end
    else
    begin
      Top := 10;
      TabOrder := 0;
    end;
    // Left :=  ((SGDADOS.parent.width - width) div 2) - 1;
  end;
  Form3.AdoSgDados.Options := Form3.AdoSgDados.Options +
    [goRangeSelect, goRowSelect] - [goEditing];
  Form3.AdoSgDados.ScrollBars := ssBoth;
  if Form3.ShowModal = mrOK then
  begin
    oFiltro.oTable.Filtered := false;
    with oFiltro.oTable do
      for i := 0 to FieldDefs.Count - 1 do
      begin
        result.FieldDefs.Add(FieldDefs[i].Name, FieldDefs[i].Datatype,
          FieldDefs[i].Size);
      end;
    result.CreateDataSet;

    with oFiltro.oTable do
      for i := 1 to Form3.AdoSgDados.RowCount - 1 do
      begin
        if not TobjMark(Form3.AdoSgDados.Objects[1, i]).mudou then
          continue;
        GotoBookmark(TobjMark(Form3.AdoSgDados.Objects[1, i]).bMark);
        try
          result.Insert;
          for z := 0 to FieldCount - 1 do
          begin
            if (Fields[z].FieldKind = fkdata) then
              result.fieldbyname(Fields[z].DisplayName).Value :=
                Fields[z].Value;
          end;
          result.Post;
        except
          ShowMessage('Erro Linha 9280 frelcltprod');
        end;
        // result.Add(SgDados.Objects[1, i]);
      end;
  end
  else
    result := nil;
  if BuscaAuto then
    Edit.Parent := EditParent;
  // SgDados.Destroy;
  Form3.oFiltro := nil;
  Form3.campos := nil;

  Form3.Destroy;

end;

     */


}
