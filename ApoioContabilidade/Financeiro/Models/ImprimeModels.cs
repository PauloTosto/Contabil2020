using ApoioContabilidade.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Financeiro.Models
{
    class ImprimeModels
    {
    }
    public class ChequeBradesco
    {
        //float 
        public List<Extendidos> cheques { get; set; }
        public ChequeBradesco()
        {
            cheques = new List<Extendidos>();
        }

    }
    public class Extendidos
    {
        public string local { get; set; }
        public Decimal valor { get; set; }
        public DateTime data { get; set; }
        public string dataExtendida { get; set; }
        public List<string> nomes { get; set; }
        public List<string> valores { get; set; }
        public Extendidos()
        {
            nomes = new List<string>();
            valores = new List<string>();
        }
    }


    public static class NumerosLiteral
    {
        static private string[,] Ordem = new string[3, 2] {{" mil", " mil"},
    {" mi-lhao", " mi-lho-es"}, {" bi-lhao", " bi-lho-es"}};
        static private string[] vet_cem = new string[9] {" cen-to", " du-zen-tos", " tre-zen-tos",
     " qua-tro-cen-tos", " qui-nhen-tos", " se-is-cen-tos", " se-te-cen-tos",
     " oi-to-cen-tos", " no-ve-cen-tos"};
        static private string[] vet_dez = new string[8]  {" vin-te", " trin-ta", " qua-ren-ta",
     " cin-quen-ta", " ses-sen-ta", " se-ten-ta", " oi-ten-ta", " no-ven-ta"};

        static private string[] vet_19 = new string[19] {" um", " dois", " tres", " qua-tro",
     " cin-co", " se-is", " se-te", " oi-to", " no-ve", " dez", " on-ze",
     " do-ze", " tre-ze", " qua-tor-ze", " quin-ze", " de-zes-se-is",
     " de-ze-sse-te", " de-zoi-to", " de-ze-no-ve"};
        const string moeda = " re-ais";
        const string cents = " cen-ta-vos";
        const string moeda_sing = " re-al";
        const string cent_sing = " cen-ta-vo";


        static string Dezenas(string campo)
        {
            campo = campo.Trim();
            string result = "";
            int numero = checkStrtoInt(campo);
            if (numero < 20)
            {
                result = vet_19[numero - 1];
                return result;
            }
            numero = checkStrtoInt(campo.Substring(0, 1));
            result = vet_dez[numero - 2];
            if (checkStrtoInt(campo.Substring(1, 1)) != 0)
                result = result + " e" + vet_19[checkStrtoInt(campo.Substring(1, 1)) - 1];
            return result;
        }

        static string Centavos(string campo, string tresp_ant)
        {
            campo = campo.Trim();
            string result = "";
            if (checkStrtoInt(campo.Substring(0, 2)) == 0)
                return result;
            if (tresp_ant.Trim() != "") result = result + " e ";
            string tnum = Dezenas(campo.Substring(0, 2));
            result = result + tnum;
            if (tnum.Trim() == "um")
                result = result + cent_sing;
            else
                result = result + cents;
            return result;
        }



        static string Extenso(string campo, string tresp_ant)
        {
            //campo = campo.Trim();
            string result = "";
            if (checkStrtoInt(campo.Substring(0, 1)) != 0)
            {
                if (tresp_ant.Trim() != "") result = result + " ,";
                result = result + vet_cem[checkStrtoInt(campo.Substring(0, 1)) - 1];
                if (checkStrtoInt(campo.Substring(1, 2)) != 0)
                    result = result + " e";
                else
                {
                    if (campo.Substring(0, 1) == "1")
                    {
                        result = result + "cem";

                    }
                    return result;
                }
            }
            else
            {
                if (checkStrtoInt(campo.Substring(1, 2)) == 0) return result;
                if (tresp_ant.Trim() != "") result = result + " ,";

            }
            result = result + Dezenas(campo.Substring(1, 2));

            return result;
        }

        static int checkStrtoInt(string campo)
        {
            int result = 0;
            campo.Trim();
            if (campo.Trim() == "") return result;
            try
            {
                result = Convert.ToInt32(campo.Trim());
            }
            catch (Exception)
            {
                MessageBox.Show("problemas de Converter inteiro");
                result = 0;
            }
            return result;
        }


        static public string valorLiteral(string tvalor)
        {
            tvalor = tvalor.Trim();
            int digitos = tvalor.IndexOf(".");           //   Pos('.', tvalor);
            if (digitos == -1)
                digitos = tvalor.IndexOf(",");
            //if (digitos != -1) digitos--;

            int j = (digitos % 3);
            if (j != 0)
            {
                j = 3 - j;
                while (j != 0)
                {
                    tvalor = " " + tvalor;
                    j--;
                }
            }
            int tordem = ((tvalor.Length - 3) / 3) - 1;
            string result = "";
            int i = 0; ;
            while (i < (tvalor.Length - 3))
            {
                result = result + Extenso(tvalor.Substring(i, 3), result);
                int retornado = checkStrtoInt(tvalor.Substring(i, 3));
                if ((tordem != 0) && (retornado != 0))
                {
                    if (retornado == 1)
                        result = result + Ordem[tordem - 1, 0];
                    else
                        result = result + Ordem[tordem - 1, 1];
                }
                tordem--;
                i = i + 3;
            }
            if (result != "")
            {
                if (result.Trim() == "um")
                    result = result + moeda_sing;
                else
                    result = result + moeda;
            }
            result = result + Centavos(tvalor.Substring(i + 1), result);
            return result;
        }
        // Cheque
        public static List<string> Posicione(string tcampo, int tam1, int tam2)
        {
            List<string> result = new List<string>();
            if (tam1 == 0)
                tam1 = 80;
            int padrao = tam1;
            string tdisplay = "";
            int i = 0;
            while (i < tcampo.Length)
            {
                if ((tdisplay == "") && (tcampo.Substring(i, 1) == " ")) { i++; continue; }
                if (tcampo.Substring(i, 1) != "-")
                {
                    tdisplay = tdisplay + tcampo.Substring(i, 1);
                    padrao--;
                    if ((padrao < 1) || ((i == (tcampo.Length - 1)) && (tdisplay != "")))
                    {
                        if ((i == (tcampo.Length - 1)) || (tcampo.Substring(i + 1, 1) == " "))
                        {
                            result.Add(tdisplay);
                        }
                        else
                        {
                            int j = i;
                            while (j != 0)
                            {
                                if (tcampo.Substring(i, 1) != "-")
                                {
                                    int tam = (tdisplay.Length - 1) - (i - j);
                                    result.Add(tdisplay.Substring(0, tam) + "-");
                                    break;
                                }
                                else if (tcampo.Substring(i, 1) == " ")
                                {
                                    int tam = (tdisplay.Length - 1) - (i - j);
                                    result.Add(tdisplay.Substring(0, tam));
                                    break;
                                }
                                j--;
                            }
                            i = j;
                        }
                        tdisplay = "";
                        padrao = tam2;
                    }
                }
                i++;
            }
            if (result.Count < 2)
                result.Add("");
            for (i = 0; i < result.Count; i++)
            {
                string campo = result[i];
                if (i == 0) campo = campo + " ";
                padrao = tam2;
                if (i == 0) padrao = tam1;
                if (campo.Length < padrao)
                {
                    string dig = "x";
                    while (campo.Length < padrao)
                    {
                        campo = campo + dig;
                        if (dig == "x") dig = "-";
                        else dig = "x";
                    }
                }
                result[i] = campo;
            }
            return result;
        }
        // recibo
        /* public static List<string> PosicioneRecibos(int tipo, string tvalor, string tcampo, int tam1, int tam2, string tempresa,
             string tcheque, string tbco)
         {
             List<string> result = new List<string>();
             if (tam1 == 0)
                 tam1 = 80;
             int padrao = tam1;
             string tdisplay = "";
             string tresp = "";

             tresp = tresp + "Recebi(emos) de " + tempresa + " a im-por-tan-cia de R$" + tvalor + " " + tcampo;
             // tipo == 2 == VAUCHES
             if ((tipo == 2) && (tcheque.Length > 0))
             {
                 tresp = tresp + " con-for-me co-pia de che-que n.: ";
                 if (tcheque.Substring(0, 2) == "CH")
                     tresp = tresp + tcheque.Substring(2); //, tcheque.Length - 2);
                 else
                 {
                     tresp = tresp + tcheque;
                 }
                 tresp = tresp + " do " + tbco.Trim() + " (a-ne-xa)" + ", re-fe-ren-te ao qua-dro de-mons-tra-ti-vo a se-guir:";
             }
             else
                 tresp = tresp + ", re-fe-ren-te ao qua-dro de-mons-tra-ti-vo a se-guir:";

             tcampo = tresp;

             int i = 0;
             while (i < tcampo.Length)
             {
                 //  if ((tdisplay == "") && (tcampo.Substring(i, 1) == " ")) { i++; continue; }
                 if (tcampo.Substring(i, 1) != "-")
                 {
                     tdisplay = tdisplay + tcampo.Substring(i, 1);
                     padrao--;
                     if ((padrao < 1) || ((i == (tcampo.Length - 1)) && (tdisplay != "")))
                     {
                         if ((i == (tcampo.Length - 1)) || (tcampo.Substring(i + 1, 1) == " "))
                         {
                             result.Add(tdisplay);
                         }
                         else
                         {
                             int j = i;
                             while (j != 0)
                             {
                                 if (tcampo.Substring(i, 1) != "-")
                                 {
                                     int tam = (tdisplay.Length - 0) - (i - j);
                                     tdisplay = tdisplay.Substring(0, tam) + "-";
                                     while (tdisplay.Length < tam2) { tdisplay = tdisplay + " "; }
                                     result.Add(tdisplay);
                                     break;
                                 }
                                 else if (tcampo.Substring(i, 1) == " ")
                                 {
                                     // int tam = (tdisplay.Length - 1) - (i - j);
                                     result.Add(tdisplay);
                                     break;
                                 }
                                 j--;
                             }
                             i = j;
                         }
                         tdisplay = "";
                         padrao = tam2;
                     }
                 }
                 i++;
             }
             if (tdisplay.Trim() != "")
             {
                 while (tdisplay.Length < tam2) { tdisplay = tdisplay + " "; }
                 result.Add(tdisplay);
             }
             return result;
         }
        */


        
        public static List<string> PosicioneRecibos(int tipo, string tvalor, string tcampo, List<int> tam, string tempresa,
            string tcheque, string tbco)
        {
            List<string> result = new List<string>();
            string tdisplay = "";
            string tresp = "";

            tresp = tresp + "Recebi(emos) de " + tempresa + " a im-por-tan-cia de R$" + tvalor + " " + tcampo;
            // tipo == 2 == VAUCHES
            if ((tipo == 2) && (tcheque.Length > 0))
            {
                tresp = tresp + " con-for-me co-pia de che-que n.: ";
                if (tcheque.Substring(0, 2) == "CH")
                    tresp = tresp + tcheque.Substring(2); //, tcheque.Length - 2);
                else
                {
                    tresp = tresp + tcheque;
                }
                tresp = tresp + " do " + tbco.Trim() + " (a-ne-xa)" + ", re-fe-ren-te ao qua-dro de-mons-tra-ti-vo a se-guir:";
            }
            else
                tresp = tresp + ", re-fe-ren-te ao qua-dro de-mons-tra-ti-vo a se-guir:";

            tcampo = tresp;


            // if (tam1 == 0)
            //      tam1 = 80;
            // int padrao = tam1;
            int indPadrao = 0;
            int i = 0;
            int padrao_resto = tam[indPadrao];
            while (i < tcampo.Length)
            {
                if (tcampo.Substring(i)  == "")
                {
                    i = tcampo.Length;
                    continue;
                }
                if (tcampo.Substring(i, 1) == "-")
                {
                    i++;
                    continue;
                }

                if (tcampo.Substring(i, 1) == " ")
                {
                    tdisplay = tdisplay + tcampo.Substring(i, 1);
                    padrao_resto--;
                    i++;
                    continue;
                }
                Palavras palavra = palavraProxima(tcampo, i);
                if (palavra.campo.Length > 0)
                {
                    if ((tdisplay.Length + palavra.campo.Length) < tam[indPadrao])
                    {
                        tdisplay = tdisplay + palavra.campo;
                        padrao_resto = padrao_resto - palavra.i;
                        i = i + palavra.i;
                       // if (acrescente == 1) { acrescente = 0; i++; }
                    }
                    else
                    {
                        palavra = palavraEmendada(tcampo, i, (tam[indPadrao] - tdisplay.Length));
                        if (palavra.i != 0)
                        {
                            tdisplay = tdisplay + palavra.campo;
                            padrao_resto = padrao_resto - palavra.i;
                            i = i + palavra.i;
                        }
                       // if (palavra.Length > 0) i++;
                        while (tdisplay.Length < tam[indPadrao]) { tdisplay = tdisplay + " "; }
                        result.Add(tdisplay);
                        tdisplay = "";
                        indPadrao++;
                        if (indPadrao < tam.Count)
                        {
                            padrao_resto = tam[indPadrao];
                        }
                        else
                        {
                            i = tcampo.Length;
                           // tdisplay = "";
                        }

                    }
                }
                else
                {
                    i = tcampo.Length;
                  //  tdisplay = "";
                   // padrao_resto = tam[indPadrao];
                }
                /*else
                {
                    tdisplay = tdisplay + tcampo.Substring(i, 1);
                    padrao_resto--;
                    i++;
                }*/

                /*   if ((padrao_resto < 1) || ((i == (tcampo.Length - 1)) && (tdisplay != "")))
                   {
                       if ((i == (tcampo.Length - 1)) || (tcampo.Substring(i + 1, 1) == " "))
                       {
                           result.Add(tdisplay);
                       }
                       else
                       {
                           // saida (ultimos dados deste padrao(linha)
                           int j = i;
                           while (j > 0)
                           {
                               if ((tcampo.Substring(i, 1) != "-") && (tcampo.Substring(i, 1) != " "))

                               {
                                   if ((i + 1) < (tcampo.Length - 1))
                                   {
                                       string resto = tcampo.Substring(i + 1);
                                       int ind_branco = resto.IndexOf(" ");
                                       int ind_traco = resto.IndexOf("-");

                                       if (ind_branco >= 1)
                                       {
                                           string pedfalta = "";
                                           foreach (Char ch in resto.Substring(0, ind_branco))
                                           {
                                               if (ch == Convert.ToChar("-")) continue;
                                               pedfalta = pedfalta + ch;
                                           }

                                           if ((pedfalta.Length + tdisplay.Length) < tam[indPadrao])
                                           {
                                               tdisplay = tdisplay + pedfalta;
                                               while (tdisplay.Length < tam[indPadrao]) { tdisplay = tdisplay + " "; }
                                               result.Add(tdisplay);
                                               j = j + ind_branco;
                                               break;
                                           }
                                       }

                                       if ((ind_branco >= 0) && (tdisplay.Length < tam[indPadrao]))
                                       {
                                           ind_branco++;
                                           tdisplay = tdisplay + resto.Substring(0, ind_branco);
                                           j = j + ind_branco;
                                       }

                                       if ((ind_branco == -1) && (ind_traco >= 0))
                                       {
                                           ind_traco++;
                                           tdisplay = tdisplay + resto.Substring(0, ind_traco);
                                           j = j + ind_traco;
                                       }

                                       else if ((ind_branco >= 0) && (ind_traco == -1))
                                       {
                                           ind_branco++;
                                           tdisplay = tdisplay + resto.Substring(0, ind_branco);
                                           j = j + ind_branco;
                                       }
                                       else if (ind_branco < ind_traco)
                                       {
                                           ind_branco++;
                                           tdisplay = tdisplay + resto.Substring(0, ind_branco);
                                           j = j + ind_branco;
                                       }
                                       else if (ind_traco < ind_branco)
                                       {
                                           ind_traco++;
                                           tdisplay = tdisplay + resto.Substring(0, ind_traco);
                                           j = j + ind_traco;
                                       }

                                   }
                                   //   int len = (tdisplay.Length - 0) - (i - j);
                                   //  tdisplay = tdisplay.Substring(0, len) + "-";
                                   // complete com espaços até o tamanho da linha
                                   while (tdisplay.Length < tam[indPadrao]) { tdisplay = tdisplay + " "; }
                                   result.Add(tdisplay);
                                   break;
                               }
                               else if (tcampo.Substring(i, 1) == " ")
                               {
                                   // int tam = (tdisplay.Length - 1) - (i - j);
                                   result.Add(tdisplay);
                                   break;
                               }
                               j--;
                           }
                           if (j < 0) j = 0;
                           i = j;

                       }
                       tdisplay = "";
                       indPadrao++;
                       if (indPadrao < tam.Count)
                       {
                           padrao_resto = tam[indPadrao];
                       }
                       else
                       {
                           padrao_resto = 0;
                           indPadrao--;
                       }
                   }
                */

            }
            if (tdisplay.Trim() != "")
            {
                while (tdisplay.Length < tam[indPadrao]) { tdisplay = tdisplay + " "; }
                result.Add(tdisplay);
            }
            return result;
        }
        static private Palavras palavraProxima(string tcampo, int i)
        {
            Palavras palavra = new Palavras();
            palavra.campo = "";
            palavra.i = 0;

            string resto = tcampo.Substring(i);
            int ind_branco = resto.IndexOf(" ");
         //   int ind_traco = resto.IndexOf("-");
            string pedfalta = "";

            if (ind_branco == -1) // situação de ultimo registro
            {
                ind_branco = resto.Length;
            }

            if (ind_branco >= 1)
            {
                foreach (Char ch in resto.Substring(0, ind_branco))
                {
                    if (ch == Convert.ToChar("-")) continue;
                    pedfalta = pedfalta + ch;
                }
            }
            palavra.campo = pedfalta;
            palavra.i = ind_branco;
            return palavra;
        }

        static private Palavras palavraEmendada(string tcampo, int i, int max)
        {
            Palavras palavra = new Palavras();
            palavra.campo = "";
            palavra.i = 0;
            string resto = tcampo.Substring(i);
            int ind_traco = resto.IndexOf("-");
            string pedfalta = "";
            if (ind_traco == 0 ) return palavra;
            ind_traco++;
            pedfalta = resto.Substring(0, ind_traco);
            palavra.i = ind_traco;
            palavra.campo = pedfalta;
            if (ind_traco > max)
            {
                palavra.campo = "";
                palavra.i = 0;
            }
            return palavra;
        }
        public struct Palavras
        {
            public int i;
            public string  campo;
        }

    }



    /*
     * function TfrmImpRecibo.PosicioneLinhas(tvalor,tcampo:string;tam1,tam2:integer;tempresa,tcheque,tbco:string):TStrings;
var
i,j, z,padrao : integer;
tdisplay,tresp : string;
begin
result := TstringList.Create;
if tam1 = 0 then  tam1 := 80;

padrao := tam2;
tdisplay := '';
tresp := '';

tresp := tresp + 'Recebi(emos) de '+ tempresa + ' a im-por-tan-cia de R$'+ tvalor + ' ' + tcampo ;

if (CBTipo.Text = 'Vauches') and (length(tcheque) > 0) then
begin
tresp := tresp + ' con-for-me co-pia de che-que n.: ';
if copy(tcheque,1,2) = 'CH' then
  tresp := tresp +  copy(tcheque,3,length(tcheque)-2)
else
  tresp := tresp +  tcheque;
tresp := tresp + ' do '+trim(tbco)+' (a-ne-xa)'+ ', re-fe-ren-te ao qua-dro de-mons-tra-ti-vo a se-guir:';
end
else
tresp := tresp +  ', re-fe-ren-te ao qua-dro de-mons-tra-ti-vo a se-guir:';

TCAMPO := TRESP;
i := 1;
while (i  <= length(tcampo)) do
begin
if copy(tcampo,i,1) <> '-' then
begin
  tdisplay  := tdisplay +  copy(tcampo,i,1);
  DEC(padrao,1);
  if (padrao < 1) or ( (i = Length(tcampo)) and (tdisplay<>'') ) then
  begin
     if (i = length(tcampo)) or (copy(tcampo,i + 1,1) = ' ') then
     begin
        while length(tdisplay) < tam2 do tdisplay := tdisplay + ' ';
        result.Add(tdisplay)
     end
     else
     begin
       j := i;
       while j <> 1 do
       begin
         if copy(tcampo,j,1) = '-'  then
         begin
            tdisplay := copy(tdisplay,1, length(tdisplay) - (i-j)  ) +'-';
            while length(tdisplay) < tam2 do tdisplay := tdisplay + ' ';
            result.Add(tdisplay);
            break;
         end
         else
         begin
            if copy(tdisplay,j,1) = ' ' then
            begin
               result.Add(tdisplay);
               break;
            end;
         end;
         Dec(j,1);
       end;
       i := j;
     end;
     tdisplay := '';
     padrao := tam2;
  end;
END;
INC(i,1);
end;
if tdisplay <> '' then
begin
while length(tdisplay) < tam2 do tdisplay := tdisplay + ' ';
result.Add(tdisplay);
end;

     */


    public class Recibos
    {
        //float 
        public List<RecibosDet> recibos { get; set; }
        public int tipoGeral { get; set; }
        public string empresa { get; set; }
        public Recibos()
        {
            recibos = new List<RecibosDet>();
        }

    }
    public class RecibosDet
    {
        public int tipo { get; set; }
        public string local { get; set; }
        public string titular { get; set; }
        public Decimal valor { get; set; }
        public DateTime data { get; set; }
        public string dataExtendida { get; set; }
        public string valorExtendido { get; set; }
        public List<ItemsRecibo> Items { get; set; }
        public List<string> Linhas { get; set; }
        public RecibosDet()
        {

            Linhas = new List<string>();
            Items = new List<ItemsRecibo>();
        }

    }
    public class ItemsRecibo
    {
        public string Historico { get; set; }
        public string LancContab { get; set; }

        public Decimal Valor { get; set; }
        public string DocFisc { get; set; }
    }

    public static class ChequesImp
    {
        static public ChequeBradesco PreencheListaCheques(DataGridView dgvSelect,  BindingSource bmCheques)
        {
            List<double> mov_ids = new List<double>();
            foreach (DataGridViewRow dgvrow in dgvSelect.SelectedRows)
            {
                DataRowView orow = (dgvrow.DataBoundItem as DataRowView);
                mov_ids.Add(Convert.ToDouble(orow["MOV_ID"]));
            }


            ChequeBradesco chequeBradesco = new ChequeBradesco();
            foreach (DataRowView orowview in (bmCheques.DataSource as DataView).Table.AsEnumerable().Where(
                   row => mov_ids.Contains(row.Field<double>("MOV_ID"))).AsDataView())
            {
                Decimal valor = Convert.ToDecimal(orowview["VALOR"]);
                string nome = orowview["FORN"].ToString();
                DateTime data = Convert.ToDateTime(orowview["DATA"]);
                string literal = NumerosLiteral.valorLiteral(valor.ToString("0.00"));
                Extendidos oext = new Extendidos();
                oext.local = "Gandu - Ba,";
                oext.data = data;
                oext.dataExtendida = data.ToString("dd/MMMM/yyyy");
                //
                oext.valor = valor;
                oext.nomes.Add(nome);
                oext.valores = NumerosLiteral.Posicione(literal, 70, 40);
                chequeBradesco.cheques.Add(oext);
            }
            return chequeBradesco;
        }
    }
    static public class RecibosImp
    {

        static public Recibos MonteRecibos(DataGridView dgvSelect,  BindingSource registrosSelecionados, BindingSource registrosDetalhes, string local, int tipo)
        {

            Recibos result = new Recibos();
            result.tipoGeral = tipo;
            string nomeFirma = TabelasIniciais.NomeDaFirma();
            if (nomeFirma == "") nomeFirma = "Djalma Lisboa da Silva";

            // o 
            List<double> mov_ids = new List<double>();
            foreach(DataGridViewRow dgvrow in dgvSelect.SelectedRows)
            {
                DataRowView orow = (dgvrow.DataBoundItem as DataRowView);
                mov_ids.Add(Convert.ToDouble(orow["MOV_ID"]));
            }


            foreach (DataRowView orow in (registrosSelecionados.DataSource as DataView).Table.AsEnumerable().Where(
                   row => mov_ids.Contains(row.Field<double>("MOV_ID"))).AsDataView())
            {
                // if (!(mov_ids.Contains(Convert.ToSingle(orow["MOV_ID"])))) continue;

                RecibosDet orec = new RecibosDet();
                orec.tipo = tipo;
                orec.valor = Convert.ToDecimal(orow["VALOR"]);
                orec.data = Convert.ToDateTime(orow["DATA"]);
                orec.dataExtendida = orec.data.Day.ToString().PadLeft(2) + " de " + orec.data.ToString("MMMM") + " de " +
                                                                orec.data.ToString("yyyy");
                orec.valorExtendido = NumerosLiteral.valorLiteral(orec.valor.ToString("0.00"));
                string tbco = TabelasIniciais.NomeBanco(orow["CREDITO"].ToString());
                // orec.Linhas
                List<int> tam = new List<int>();
                tam.Add(110);
                tam.Add(110);
                tam.Add(90);
                List<string> lstPosicione = NumerosLiteral.PosicioneRecibos(tipo, "(" + orec.valor.ToString("##,###,##0.00").Trim() + ")",
                       orec.valorExtendido
                      , tam,
                      nomeFirma.Trim(), orow["DOC"].ToString(), tbco.Trim());
                orec.Linhas = lstPosicione;
                orec.titular = orow["FORN"].ToString();
                orec.local = local;
                foreach (DataRowView orowdet in (registrosDetalhes.DataSource as DataView).Table.AsEnumerable().
                    Where(row=>row.Field<int>("IDLOCAL") == Convert.ToInt32(orow["IDLOCAL"]) ).AsDataView())
                {
                    ItemsRecibo oitems = new ItemsRecibo();
                    oitems.Historico = orowdet["HIST"].ToString();
                    oitems.Valor = Convert.ToDecimal(orowdet["VALOR"]);
                    oitems.LancContab = orowdet["DEBITO"].ToString();
                    oitems.DocFisc = orowdet["DOC_FISC"].ToString().Trim();
                    orec.Items.Add(oitems);
                }
                result.recibos.Add(orec);
            }
            return result;
        }
    }
}    

 







