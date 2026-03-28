using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ClassLibTrabalho
{
    public class ClassCriticaPonto
    {
        public class TRegAssoc : Object
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
            public Decimal valoremp;
            public Decimal[] dia;
            public Decimal[] horas;
            public string[] compativel;
            public int fi_1_2_3;
            public int fi_5_6_7;
            public Boolean domingo_trab;
            public Boolean remun;
            public Boolean domingotrab_anterior;
            public Boolean fi_anterior;

            public TRegAssoc()
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
                dia = new decimal[7] { 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M };
                horas = new decimal[7] { 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M };
                compativel = new string[7] { "", "", "", "", "", "", "" };
                fi_1_2_3 = 0;
                fi_5_6_7 = 0;
                domingo_trab = false;
                remun = false;
                domingotrab_anterior = false;
                fi_anterior = false; ;
            }
        }

        private DataTable cadastro;
        private DataTable pontosemanaanterior;
        private DataTable pontotrab;
        private DataTable pontonoturno;
        private Dictionary<string, TCLTCodigo> cltcodigo;
        
        public  Dictionary<string, TRegAssoc> ListaAssociado;

        public DataTable Cadastro
        {
            set { cadastro = value; }        
        }
        public Dictionary<string, TCLTCodigo> CltCodigo
        {
            set { cltcodigo = value; }
        }

        public DataTable SemanaAnterior
        {
            set { pontosemanaanterior = value; }
        }
        public DataTable PontoTrab
        {
            set { pontotrab = value; }
        }
        public DataTable PontoNoturno
        {
            set { pontonoturno = value; }
        }
       

        public ClassCriticaPonto()
        {
            ListaAssociado = new Dictionary<string, TRegAssoc>();
        }
        public void Clear()
        {
            ListaAssociado.Clear();
        }
        /// Rotinas para criticas 
        /// 
        public void PreenchaAssociado()
        { 
            if (cadastro == null) return;
            foreach (DataRow orow in cadastro.Rows)
            {
                TRegAssoc oassoc = PreenchaAssociadoLinha(orow["CODCAD"].ToString(), Convert.ToDecimal(orow["SALARIOREAL"]));
                if (pontotrab != null)
                {
                    IEnumerable<DataRow> query =
                        from linha in pontotrab.AsEnumerable()
                        where linha.Field<string>("TRAB") == orow["CODCAD"].ToString()
                        select linha;
                    foreach (DataRow orowq in query)
                    {
                        Horas_Valores(orowq);
                    }
                }
                if (pontonoturno != null)
                {
                    IEnumerable<DataRow> query =
                        from linha in pontonoturno.AsEnumerable()
                        where linha.Field<string>("TRAB") == orow["CODCAD"].ToString()
                        select linha;
                    if (query.Count() > 0)
                    foreach (DataRow orowq in query)
                    {
                        Horas_Valores(orowq);
                    }
                }
                AtualizeLinhaCadastro(orow, oassoc);
            }
        }
        private void AtualizeLinhaCadastro(DataRow orow, TRegAssoc oassoc)
        {
            orow.BeginEdit();
            orow["Diarias"] = oassoc.diarias;
            orow["Horas50"] = oassoc.horas50;
            orow["Horas100"] = oassoc.horas100;
            orow["HorasFE"] = oassoc.horasfe;
            //orow["ValorEmp"] = oassoc.valoremp;
            orow["CRITICA"] = ConstruaCritica(oassoc);
            orow.EndEdit();
  
        }

        public Boolean TestaCompatibilidade(string trab,string dia)
        {
            Boolean result = true;
            if (ListaAssociado.ContainsKey(trab))
            { 
               int inddia = -1;
               try
               {
                   inddia = Convert.ToInt32(dia.Substring(3,1));
                   if ((inddia > 0) && (inddia < 8))
                       result = (ListaAssociado[trab].compativel[inddia - 1] != "N");
               }
                catch
               {
                    return result;
                }
            }
            return result;
        }
        public void QuandoDataTableAlterar(string trab)
        {
             TRegAssoc assoc;
            if (!ListaAssociado.ContainsKey(trab))
                PreenchaAssociadoLinha(trab, 0.0M);
            if (!ListaAssociado.ContainsKey(trab))
                return;

            assoc = ListaAssociado[trab];
            
            ZereValores(assoc);
           
            if (pontotrab != null)
            {
                IEnumerable<DataRow> query =
                    from linha in pontotrab.AsEnumerable()
                    where linha.Field<string>("TRAB") == trab
                    select linha;
                foreach (DataRow orowq in query)
                {
                    Horas_Valores(orowq);
                }
            }
            if (pontonoturno != null)
            {
                IEnumerable<DataRow> query =
                    from linha in pontonoturno.AsEnumerable()
                    where linha.Field<string>("TRAB") == trab
                    select linha;
                foreach (DataRow orowq in query)
                {
                    Horas_Valores(orowq);
                }
            }
           DataRow orow = cadastro.Rows.Find(trab);
           if (orow != null)
           {
               AtualizeLinhaCadastro(orow, assoc);
           }

        }
        // dados cadastro e da semana anterior
        private TRegAssoc PreenchaAssociadoLinha(string codcad, Decimal odiarias)
        {
            TRegAssoc assoc;
            if (!ListaAssociado.ContainsKey(codcad))
            {
                assoc = new TRegAssoc();
                ListaAssociado.Add(codcad, assoc);
                if ((odiarias == 0.0M) )
                {
                    DataRow rowcad = cadastro.Rows.Find(codcad);
                    if (rowcad != null)
                        odiarias = Convert.ToDecimal(rowcad["SALARIOREAL"]);
                }
            }
            else
            {
                assoc = ListaAssociado[codcad];
            }
            assoc.vlrdiaria = odiarias;
            if (pontosemanaanterior == null) return assoc;
            
            DataRow[] orows = pontosemanaanterior.Select("TRAB = '" + codcad + "'");
            bool ret = false;
            Decimal[] DiaTeste = new decimal[3] { 0.0M, 0.0M, 0.0M }; 
            foreach (DataRow orowponto in orows)
            {
                if ((orowponto["DIA5"].ToString() == "FI") || (orowponto["DIA6"].ToString() == "FI") ||
                    (orowponto["DIA7"].ToString() == "FI"))
                    assoc.fi_anterior = true;
                if (orowponto["DIA4"].ToString().Trim() == "X")
                    assoc.domingotrab_anterior = true;

                for (int i = 0; i < 3; i++)
                {
                    string dias = "DIA" + (i+5).ToString().Trim();
                    if (orowponto[dias].ToString().Trim() == "") continue;
                    if (cltcodigo.ContainsKey(orowponto[dias].ToString()))
                    {
                        TCLTCodigo ocod = cltcodigo[orowponto[dias].ToString()];
                        if (ocod.diarias > 0)
                            DiaTeste[i] = DiaTeste[i] + ocod.diarias;
                    }
                }
            }
            if (ret)
            {
                for (int i = 0; i < 3; i++)
                    if (DiaTeste[i] < 8)
                        assoc.fi_anterior = true;
            }
            return assoc;
        }
        private void ZereValores(TRegAssoc oassoc)
        {
            oassoc.diarias = 0;
            oassoc.horasfe = 0;
            oassoc.horas100 = 0;
            oassoc.horas50 = 0;
            for (int i = 0; i < 7; i++)
            {
                oassoc.dia[i] = 0;
                oassoc.horas[i] = 0;
                oassoc.compativel[i] = "";
            }
            oassoc.fi_1_2_3 = 0;
            oassoc.fi_5_6_7 = 0;
            oassoc.domingo_trab = false;
            oassoc.remun = false;
        }
        private void Horas_Valores(DataRow orow)
        {
            if (!ListaAssociado.ContainsKey(orow["TRAB"].ToString()))
            {
                PreenchaAssociadoLinha(orow["TRAB"].ToString(), 0);
            }
            TRegAssoc assoc;
            if (!ListaAssociado.ContainsKey(orow["TRAB"].ToString()))  
                return;
            else
            {
                assoc = ListaAssociado[orow["TRAB"].ToString()];
            }
            if (orow["TIPOMOV"].ToString().Trim() == "D")
            {
                for (int i = 1; i < 8; i++)
                {
                    string dias = "DIA" + i.ToString().Trim();
                    if (orow[dias].ToString().Trim() == "") continue;
                    if (cltcodigo.ContainsKey(orow[dias].ToString()))
                    {
                        TCLTCodigo ocod = cltcodigo[orow[dias].ToString()];
                        if (ocod.diarias > 0)
                        {
                            assoc.diarias += (ocod.diarias / 8);
                            assoc.dia[i - 1] = assoc.dia[i - 1] + ocod.diarias;
                        }
                        if (ocod.horas50 > 0)
                        {
                            assoc.horas50 += ocod.horas50;
                            assoc.horas[i - 1] = assoc.horas[i - 1] + ocod.horas50;
                        }
                        if (ocod.horas100 > 0)
                        {
                            assoc.horas100 += ocod.horas100;
                            assoc.horas[i - 1] = assoc.horas[i - 1] + ocod.horas100;
                        }
                        if (ocod.horasfe > 0)
                        {
                            assoc.horasfe += ocod.horasfe;
                        }
                        if (ocod.compati == "N")
                        {
                            assoc.compativel[i - 1] = ocod.compati;
                        }
                        if (ocod.indcod == "FI")
                        {
                            if ((dias == "DIA1") || (dias == "DIA2") || (dias == "DIA3") )
                                assoc.fi_1_2_3 += 1;
                            if ((dias == "DIA5") || (dias == "DIA6") || (dias == "DIA7"))
                                assoc.fi_5_6_7 += 1;
                        }
                        if ((dias == "DIA4") && (ocod.indcod.Trim() == "R"))
                            assoc.remun = true;
                        if ((dias == "DIA4") && (ocod.indcod.Trim() == "X"))
                            assoc.domingo_trab = true;
                    }
                }
            }
        }
        private string ConstruaCritica(TRegAssoc oassoc)
        {
            string result = "";
            if (oassoc.domingo_trab && oassoc.domingotrab_anterior) 
                   result = "Domingo Trab.Seguido";
              if (oassoc.remun &&  oassoc.fi_anterior) 
                   result = "Remunerado Com FI Anterior";
              if (oassoc.remun &&   (oassoc.fi_1_2_3 > 0)) 
                   result = "Remunerado Com FI";
              for (int i = 0;i <7;i++)
              {
                  if (oassoc.horas[i] > 2) 
                      result = "Horas Extras > 2 ";
              }
              // quinta e sexta testa
              for (int i = 0;i <2;i++)
              {
                  if ((oassoc.dia[i] < 8) &&  oassoc.remun)       
                      result = "Remunerado Indevido";
              }  // sabado
              if ((oassoc.dia[2] < 4) &&
                      oassoc.remun)       
                      result = "Remunerado Indevido";
            for (int i = 0;i <7;i++)
            
              {
                  if (i != 3) 
                  {
                      if (oassoc.dia[i] > 8) 
                         result = "Horas Normais Excedentes ";
                  }
                  else
                  {
                     if (!oassoc.remun) 
                     {
                        if  (oassoc.dia[i] > 8) 
                           result = "Horas Normais Excedentes ";
                     }
                     else
                     {
                        if  (oassoc.dia[i] > 16) 
                            result = "Horas Normais Excedentes ";
                     }
                  }
              }
            for (int i = 0;i <7;i++)  
              {
                  if (i == 2) 
                  {
                     if ((oassoc.horas[i] > 0) && (oassoc.dia[i] < (8/2))) 
                        result = "Dia Incompl. + Extra? ";
                  }
                  else
                     if ((oassoc.horas[i] > 0) && (oassoc.dia[i] < 8) )
                       result = "Dia Incompl. + Extra? ";
              }
            return result;
            /*  with DataSet as TTable do
       begin
        try
           ind := ListaAssociado.IndexOf(FieldbyName('CODCAD').asString);
           if ind = -1 then
           begin
              FieldbyName('Diarias').asCurrency := 0;
              FieldbyName('Horas50').asCurrency := 0;
              FieldbyName('Horas100').asCurrency := 0;
              FieldbyName('HorasFE').asCurrency := 0;
              FieldbyName('ValorEmp').asCurrency := 0;
              FieldbyName('Critica').asString := '';
           end
           else
           begin
              FieldbyName('Diarias').asCurrency :=  TRegAssoc(ListaAssociado.Objects[ind]).diarias;
              FieldbyName('Horas50').asCurrency :=  TRegAssoc(ListaAssociado.Objects[ind]).Horas50;
              FieldbyName('Horas100').asCurrency := TRegAssoc(ListaAssociado.Objects[ind]).Horas100;
              FieldbyName('HorasFE').asCurrency :=  TRegAssoc(ListaAssociado.Objects[ind]).HorasFe;
              FieldbyName('ValorEmp').asCurrency := TRegAssoc(ListaAssociado.Objects[ind]).ValorEmp;
              if (TRegAssoc(ListaAssociado.Objects[ind]).DOMINGO_TRAB and
                  TRegAssoc(ListaAssociado.Objects[ind]).DomingoTrab_Anterior) then
                   FieldbyName('Critica').asString := 'Domingo Trab.Seguido';
              if TRegAssoc(ListaAssociado.Objects[ind]).REMUN and
                  TRegAssoc(ListaAssociado.Objects[ind]).FI_Anterior then
                   FieldbyName('Critica').asString := 'Remunerado Com FI Anterior';
              if (TRegAssoc(ListaAssociado.Objects[ind]).REMUN and
                  (TRegAssoc(ListaAssociado.Objects[ind]).FI_1_2_3 > 0)) then
                   FieldbyName('Critica').asString := 'Remunerado Com FI';
              for i := 1 to 7 do
              begin
                  if (TRegAssoc(ListaAssociado.Objects[ind]).Horas[i] > 2) then
                      FieldbyName('Critica').asString := 'Horas Extras > 2 ';
              end;
              // quinta e sexta testa
              for i := 1 to 2 do
              begin
                  if (TRegAssoc(ListaAssociado.Objects[ind]).Dia[i] < 8) and
                      TRegAssoc(ListaAssociado.Objects[ind]).REMUN       then
                      FieldbyName('Critica').asString := 'Remunerado Indevido';
              end;  // sabado
              if (TRegAssoc(ListaAssociado.Objects[ind]).Dia[3] < 4) and
                      TRegAssoc(ListaAssociado.Objects[ind]).REMUN       then
                      FieldbyName('Critica').asString := 'Remunerado Indevido';

              for i := 1 to 7 do
              begin
                  if i <> 4 then
                  begin
                      if (TRegAssoc(ListaAssociado.Objects[ind]).Dia[i] > 8) then
                         FieldbyName('Critica').asString := 'Horas Normais Excedentes ';
                  end
                  else
                  begin
                     if not TRegAssoc(ListaAssociado.Objects[ind]).REMUN then
                     begin
                        if  (TRegAssoc(ListaAssociado.Objects[ind]).Dia[i] > 8) then
                           FieldbyName('Critica').asString := 'Horas Normais Excedentes ';
                     end
                     else
                     begin
                        if  (TRegAssoc(ListaAssociado.Objects[ind]).Dia[i] > 16) then
                            FieldbyName('Critica').asString := 'Horas Normais Excedentes ';
                     end
                  end;
              end;
              for i := 1 to 7 do
              begin
                  if i = 3 then
                  begin
                     if (TRegAssoc(ListaAssociado.Objects[ind]).horas[i] > 0) and (TRegAssoc(ListaAssociado.Objects[ind]).dia[i] < (8/2)) then
                        FieldbyName('Critica').asString := 'Dia Incompl. + Extra? ';
                  end
                  else
                     if (TRegAssoc(ListaAssociado.Objects[ind]).horas[i] > 0) and (TRegAssoc(ListaAssociado.Objects[ind]).dia[i] < 8) then
                       FieldbyName('Critica').asString := 'Dia Incompl. + Extra? ';
              end;

           end;

 
             */ 
        }
    }
}
