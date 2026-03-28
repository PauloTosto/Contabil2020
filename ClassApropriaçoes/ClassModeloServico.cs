using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;

using ClassConexao;


namespace ClassApropriaçoes
{
    public class ClassModelo
    {
        DataTable modelo;

        public ClassModelo()
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb = 
                " SELECT        num, prod, tipo, cod, seq, modelo.desc as descricao, cult "+
                " FROM         "+path+"modelo "+
                " where (alltrim(modelo.desc) <> '') AND ( ALLTRIM(NUM) <> '') ";
                

            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "MODELO");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                modelo = odataset.Tables[0].Copy();
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = modelo.Columns["NUM"];
                modelo.PrimaryKey = PrimaryKeyColumns;

                
            }
            catch (Exception)
            {
                modelo = null;
                MessageBox.Show("Não pude ler a Tabela Modelo");
                throw;
            } 
        }
        public string ModeloCultura(string tnum)
        {
            if (modelo == null) return "";
            DataRow orow = modelo.Rows.Find(tnum);
            if (orow == null) return "";
            return orow["PROD"].ToString();
        }
        public string ModeloProduto(string tnum)
        {
            if (modelo == null) return "";
            DataRow orow = modelo.Rows.Find(tnum);
            if (orow == null) return "";
            return orow["CULT"].ToString();
        }
        public string ModeloTipo(string tnum)
        {
            if (modelo == null) return "";
            DataRow orow = modelo.Rows.Find(tnum);
            if (orow == null) return "";
            return orow["TIPO"].ToString();
        }
        public string ModeloDescricao(string tnum)
        {
            if (modelo == null) return "";
            DataRow orow = modelo.Rows.Find(tnum);
            if (orow == null) return "";
            return orow["Descricao"].ToString();
        }



        public string ModeloNatureza(string tnum)
        {
            if (modelo == null) return "";
            DataRow orow = modelo.Rows.Find(tnum);
            if (orow == null) return "";
            string result = "";
            if (orow["TIPO"].ToString() == "I")
                result = "INVESTIMENTO ";
            else if (orow["TIPO"].ToString() == "M")
                result = "MANUT.INVEST.";
            else if (orow["TIPO"].ToString() == "C")
                result = "CUSTEIO      ";
            else if (orow["TIPO"].ToString() == "R")
                result = "RECEITA      ";
            else if (orow["TIPO"].ToString() == "T")
                result = "TRANSFERENCIA";
            else if (orow["TIPO"].ToString() == "P")
                result = "CUST.PARCEIRO";
            else if (orow["TIPO"].ToString() == "O")
                result = "CUST.OUTORGAN";

            else if (orow["TIPO"].ToString() == "")
                result = "Sem Definição";
            return result;
        }




    }
    public class ClassServico
    {
        DataTable servico;

        public ClassServico()
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb =
                " SELECT        cod, unid, tipo, indice, servic.desc as descricao " +
                " FROM         " + path + "Servic ";


            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "SERVIC");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                servico = odataset.Tables[0].Copy();
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = servico.Columns["COD"];
                servico.PrimaryKey = PrimaryKeyColumns;


            }
            catch (Exception)
            {
                servico = null;
                MessageBox.Show("Não pude ler a Tabela servico");
                throw;
            }
        }
        public string ServicoDescricao(string tnum)
        {
            if (servico == null) return "";
            DataRow orow = servico.Rows.Find(tnum);
            if (orow == null) return "";
            return orow["Descricao"].ToString();
        }
    }

    public class ClassCentro
    {
        DataTable centro;

        public ClassCentro()
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            stroledb =  "SELECT ID,  COD,`DESC` as Descricao,inicio,fim,setor,setorant,setorpos,class  FROM " + path + "GLEBAS where fim = ctod('  /  /  ') ";
            stroledb += " ORDER BY COD";
           
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "GLEBA");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                centro = odataset.Tables[0].Copy();
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = centro.Columns["COD"];
                centro.PrimaryKey = PrimaryKeyColumns;


            }
            catch (Exception)
            {
                centro = null;
                MessageBox.Show("Não pude ler a Tabela Glebas");
                throw;
            }
        }
        public string GlebaDescricao(string tnum)
        {
            if (centro == null) return "";
            DataRow orow = centro.Rows.Find(tnum);
            if (orow == null) return "";
            return orow["Descricao"].ToString();
        }
    }






    /*
 * function ModeloCultura(tcod:string):string;
var
i : integer;
bMark :TBookMark;
velhoIndex : string;
begin
result := '';
with DmTrab.tblModelo do
begin
if active = false then active :=true;
IndexDefs.Update;
velhoindex := IndexName;
IndexName := 'IMOD3';
bMark := GetBookMark;
SetKey;
FieldbyName('NUM').AsString := tcod;
FieldbyName('COD').AsString := '';
GotoNearest;
if (not eof) and (FieldbyName('NUM').AsString = tcod) then
begin
result := FieldbyName('PROD').AsString;
end;
GoToBookMark(BMark);
FreeBookMark(bMark);
IndexName := velhoIndex;
end;
end;

function ModeloProduto(tcod:string):string;
var
i : integer;
bMark :TBookMark;
velhoIndex : string;
begin
result := '';
with DmTrab.tblModelo do
begin
if active = false then active :=true;
IndexDefs.Update;
velhoindex := IndexName;
IndexName := 'IMOD3';
bMark := GetBookMark;
SetKey;
FieldbyName('NUM').AsString := tcod;
FieldbyName('COD').AsString := '';
GotoNearest;
if (not eof) and (FieldbyName('NUM').AsString = tcod) then
begin
result := FieldbyName('CULT').AsString;
end;
GoToBookMark(BMark);
FreeBookMark(bMark);
IndexName := velhoIndex;
end;
end;

function ModeloCod(tcod:string):string;
var
i : integer;
bMark :TBookMark;
velhoIndex : string;
begin
result := '';
with DmTrab.tblModelo do
begin
if active = false then active :=true;

velhoindex := IndexName;
IndexName := 'IMOD5';
IndexDefs.Update;
bMark := GetBookMark;
SetKey;
FieldbyName('DESC').AsString := tcod;
GotoKey;
if (not eof) then
begin
result := FieldbyName('NUM').AsString;
end;
GoToBookMark(BMark);
FreeBookMark(bMark);
IndexName := velhoIndex;
end;
end;



function ModeloNatureza(tcod:string):string;
var
i : integer;
bMark :TBookMark;
velhoIndex : string;
begin
result := '';
with DmTrab.tblModelo do
begin
if active = false then active :=true;
IndexDefs.Update;
velhoindex := IndexName;
IndexName := 'IMOD3';
bMark := GetBookMark;
SetKey;
FieldbyName('NUM').AsString := tcod;
FieldbyName('COD').AsString := '';
IF GotoKey then
begin
if trim(FieldbyName('Tipo').asString) = 'I' then
result := 'INVESTIMENTO '
else if trim(FieldbyName('Tipo').asString) = 'M' then
result := 'MANUT.INVEST.'
else if trim(FieldbyName('Tipo').asString) = 'C' then
result := 'CUSTEIO      '
else if trim(FieldbyName('Tipo').asString) = 'R' then
result := 'RECEITA      '
else if trim(FieldbyName('Tipo').asString) = 'T' then
result := 'TRANSFERENCIA'
else   if trim(FieldbyName('Tipo').asString) = 'P' then
result := 'CUST.PARCEIRO'
else if trim(FieldbyName('Tipo').asString) = '' then
result := 'Sem Definição';

end;
GoToBookMark(BMark);
FreeBookMark(bMark);
IndexName := velhoIndex;
end;
end;

*/


}
