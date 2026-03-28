using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Fiscal.Model
{
    public class NFiscal
    {

        public int Id { get; set; }
        public string Nome { get; set; } //58
        public string NATOP { get; set; } //25
        public DateTime DtaEmi { get; set; }
        public string TPNF { get; set; } //1
        public string IDDEST { get; set; } // 15
        public string FINNF { get; set; } //10
        public string evento { get; set; } //30
        public int Nfiscal { get; set; }
        public int Serie { get; set; }
        public double VlrNF { get; set; }
        public double Quant_NF { get; set; }
        public string CFOP { get; set; } // 10
        public string DEST_CNPJ { get; set; } //14
        public string DEST_TPDOC { get; set; } //4
        public string DEST_NOME { get; set; } // 80
        public string DESC2 { get; set; } //25
        public string CODFAZ { get; set; } //2
        public string FIRMA { get; set; } //8
        public string xLgr { get; set; } //25
        public string dest_IE { get; set; } //14

        public string PRODFIS { get; set; } //3
        public string PROD_EXC { get; set; } //1
        public string CRITICA { get; set; } //25
        public string ObsEvento { get; set; } // 600
    }
    public class ItensFis
    {
        public int Id { get; set; }
        public int Id_NF { get; set; }
        public string cProd { get; set; } //3
        public string xProd { get; set; } //40
        public string cUnid { get; set; } //10
        public double cQuant { get; set; }
        public double cPUnit { get; set; }
        public double cVlr { get; set; }
        public string CFOP { get; set; } //10

        public string InfAdPROD { get; set; } //255
        public double sdoQuant { get; set; }
    }

    public class ADOCNPJ
    {
        public int Id { get; set; }
        public string CNPJ { get; set; }
        public string Codfaz { get; set; }
        public DateTime dta_Ini_Rain { get; set; }
        public DateTime dta_Fim_Rain { get; set; }
        public DateTime dta_Ini_Utz { get; set; }
        public DateTime dta_Fim_Utz { get; set; }
    }

    /*static public class Tam_Campos
    {  // tamanhos maximos das strings
        static public Dictionary<string, int> Nfiscal { get; set; }
        static public Dictionary<string, int> ItensFis { get; set; }
        static public void Preencha()
        { 
            Nfiscal = new Dictionary<string, int>();
            ItensFis = new Dictionary<string, int>();
            Nfiscal.Add("Nome", 58);
            Nfiscal.Add("NATOP", 25);
            Nfiscal.Add("TPNF", 1);
            Nfiscal.Add("IDDEST", 15);
            Nfiscal.Add("FINNF", 10);
            Nfiscal.Add("evento", 30);
            Nfiscal.Add("CFOP", 10);
            Nfiscal.Add("DEST_CNPJ", 14);
            Nfiscal.Add("DEST_TPDOC", 4);
            Nfiscal.Add("DEST_NOME", 80);
            Nfiscal.Add("DESC2", 25);
            Nfiscal.Add("CODFAZ", 2);
            Nfiscal.Add("FIRMA", 8);
            Nfiscal.Add("xLgr", 25);
            Nfiscal.Add("dest_IE", 14);
            Nfiscal.Add("PRODFIS", 3);
            Nfiscal.Add("PROD_EXC", 1);
            Nfiscal.Add("CRITICA", 25);
           // Nfiscal.Add("ObsEvento", 600);

            ItensFis.Add("cProd", 3);
            ItensFis.Add("cUnid", 10);
            ItensFis.Add("CFOP", 10);
           // ItensFis.Add("InfAdPROD", 255);
        }
    }
    */
    public class ListaAdoCNPJ
    {
        public List<ADOCNPJ> lista { get; set; }
        public ListaAdoCNPJ()
        {
            lista = new List<ADOCNPJ>();
            ADOCNPJ novo = new ADOCNPJ();
            novo.Id = 1;
            novo.CNPJ = "14512735000109";
            novo.Codfaz  = "";
            lista.Add(novo);
            novo = new ADOCNPJ();
            novo.Id = 2;
            novo.CNPJ = "14512735000109";
            novo.Codfaz = " 5";
            novo.dta_Ini_Rain =  Convert.ToDateTime("2010-01-01");
            novo.dta_Ini_Utz = Convert.ToDateTime("2013-01-01");
            lista.Add(novo);
            novo = new ADOCNPJ();
            novo.Id = 3;
            novo.CNPJ = "14512735000885";
            novo.Codfaz = " 2";
            lista.Add(novo);
            novo = new ADOCNPJ();
            novo.Id = 4;
            novo.CNPJ = "14512735000532";
            novo.Codfaz = " 3";
            novo.dta_Ini_Utz = Convert.ToDateTime("2013-01-01");
            lista.Add(novo);
            novo = new ADOCNPJ();
            novo.Id = 5;
            novo.CNPJ = "14512735000451";
            novo.Codfaz = " 4";
            novo.dta_Ini_Utz = Convert.ToDateTime("2013-01-01");
            lista.Add(novo);
            novo = new ADOCNPJ();
            novo.Id = 6;
            novo.CNPJ = "14512735000702";
            novo.Codfaz = " 6";
            novo.dta_Ini_Rain = Convert.ToDateTime("2010-01-01") ;
            novo.dta_Ini_Utz  = Convert.ToDateTime("2013-01-01");
            lista.Add(novo);
            novo = new ADOCNPJ();
            novo.Id = 7;
            novo.CNPJ = "14512735000613";
            novo.Codfaz = " 8";
            novo.dta_Ini_Rain = Convert.ToDateTime("2010-01-01");
            novo.dta_Ini_Utz = Convert.ToDateTime("2013-01-01");
            lista.Add(novo);
            novo = new ADOCNPJ();
            novo.Id = 8;
            novo.CNPJ = "14512735000370";
            novo.Codfaz = " 9";
            novo.dta_Ini_Rain = Convert.ToDateTime("2010-01-01");
            novo.dta_Ini_Utz = Convert.ToDateTime("2013-01-01");
            lista.Add(novo);
        }
    }


    public class Insere_Edita
    {
        public string query { get; set; }
    }


}
/*
 * 

  adoItensFis := TAdoDataset.Create(self);
  with adoItensFis do

  adoCNPJ := TAdoDataset.Create(self);
  with adoCNPJ do
  begin
    with FieldDefs.AddFieldDef do
    begin
      Name := 'DTA_INI_rain';
      DataType := ftDate;
    end;
    with FieldDefs.AddFieldDef do
    begin
      Name := 'DTA_FIM_rain';
      DataType := ftDate;
    end;
    with FieldDefs.AddFieldDef do
    begin
      Name := 'DTA_INI_UTZ';
      DataType := ftDate;
    end;
    with FieldDefs.AddFieldDef do
    begin
      Name := 'DTA_FIM_UTZ';
      DataType := ftDate;
    end;

  end;
  adoCNPJ.CreateDataSet;
  adoCNPJ.Insert;
  novo.ID').asInteger := 1;
  novo.CNPJ').AsString := '14512735000109';
  novo.CODFAZ').AsString := '';
  adoCNPJ.Post;
  adoCNPJ.Insert;
  novo.ID').asInteger := 2;
  novo.CNPJ').AsString := '14512735000109';
  novo.CODFAZ').AsString := ' 5';
  novo.DTA_INI_RAIN').asDateTime := strtodate('01/01/2010');
  novo.DTA_INI_UTZ').asDateTime := strtodate('01/01/2013');
  adoCNPJ.Post;
  adoCNPJ.Insert;
  novo.ID').asInteger := 3;
  novo.CNPJ').AsString := '14512735000885';
  novo.CODFAZ').AsString := ' 2';
  adoCNPJ.Post;
  adoCNPJ.Insert;
  novo.ID').asInteger := 4;
  novo.CNPJ').AsString := '14512735000532';
  novo.CODFAZ').AsString := ' 3';
  // novo.DTA_INI_RAIN').AsDateTime := strtodate('01/01/2010');
  novo.DTA_INI_UTZ').asDateTime := strtodate('01/01/2013');
  adoCNPJ.Post;
  adoCNPJ.Insert;
  novo.ID').asInteger := 5;
  novo.CNPJ').AsString := '14512735000451';
  novo.CODFAZ').AsString := ' 4';
  // novo.DTA_INI_RAIN').AsDateTime := strtodate('01/01/2010');
  novo.DTA_INI_UTZ').asDateTime := strtodate('01/01/2013');
  adoCNPJ.Post;
  adoCNPJ.Insert;
  novo.ID').asInteger := 6;
  novo.CNPJ').AsString := '14512735000702';
  novo.CODFAZ').AsString := ' 6';
  novo.DTA_INI_RAIN').asDateTime := strtodate('01/01/2010');
  novo.DTA_INI_UTZ').asDateTime := strtodate('01/01/2013');
  adoCNPJ.Post;
  adoCNPJ.Insert;
  novo.ID').asInteger := 7;
  novo.CNPJ').AsString := '14512735000613';
  novo.CODFAZ').AsString := ' 8';
  novo.DTA_INI_RAIN').asDateTime := strtodate('01/01/2010');
  novo.DTA_INI_UTZ').asDateTime := strtodate('01/01/2013');
  adoCNPJ.Post;
  adoCNPJ.Insert;
  novo.ID').asInteger := 8;
  novo.CNPJ').AsString := '14512735000370';
  novo.CODFAZ').AsString := ' 9';
  novo.DTA_INI_RAIN').asDateTime := strtodate('01/01/2010');
  novo.DTA_INI_UTZ').asDateTime := strtodate('01/01/2013');
  adoCNPJ.Post;


 */
