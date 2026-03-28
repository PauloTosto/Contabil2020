using ApoioContabilidade.Almoxarifado.Model;
using ApoioContabilidade.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Almoxarifado.ServicesAlmox
{
    public static class MovEstStatic
    {
     //   static DataTable tabMovEst = null;
      //  static private bool inprocess = false;
       // static public DateTime dataRef = new DateTime(2000, 1, 1); // inicio do ESTOQUE DO ALMOXARIFADO 
       /* static public DataTable MovEst()
        {
            return tabMovEst;
        }
        static public bool MovEstOk()
        {
            bool retorno = false;
            if ((tabMovEst == null) || (tabMovEst.Rows.Count == 0) || inprocess)
            {
                return retorno;
            }
            retorno = true;

            return retorno;
        }
        static public void LibereMovest()
        {
            if (tabMovEst != null)
            {
                tabMovEst.Rows.Clear();
                tabMovEst = null;
                inprocess = false;
            }
        }
        static public InicioFim AlgoritmoParaPMedio(string tcod, DateTime UltimoPonto, DateTime dtData)
        {
            try
            {

                InicioFim result = null;
                if (!MovEstStatic.MovEstOk()) return result;

                List<string> lst = new List<string>();
                var dadoCompras = (from gr in tabMovEst.AsEnumerable().Where(row =>
                             tcod.Trim() == "" ? true : row.Field<string>("COD").Trim() == tcod.Trim()
                             && row.Field<string>("TIPO").Trim() == "E"
                             && row.Field<string>("TIPO2").Trim() != "T"
                             && row.Field<double>("QUANT") > 0
                             && row.Field<double>("VALOR") > 0
                             && (row.Field<DateTime>("DATA").CompareTo(dataRef) >= 0)
                             && (row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0)
                             )
                                   group gr by new
                                   {
                                       cod = gr.Field<string>("COD"),
                                       data = gr.Field<DateTime>("DATA"),
                                   } into g
                                   select new
                                   {
                                       cod = g.Key.cod,
                                       data = g.Key.data,
                                       quant = g.Sum(row => row.Field<double>("QUANT")),
                                       valor = g.Sum(row => row.Field<double>("VALOR"))
                                   }
                                 );
                var dadoSaidas = (from gr in MovEstStatic.MovEst().AsEnumerable().Where(row =>
                             tcod.Trim() == "" ? true : row.Field<string>("COD").Trim() == tcod.Trim()
                             && row.Field<string>("TIPO").Trim() == "S"
                             && row.Field<string>("TIPO2").Trim() != "T"
                             && row.Field<double>("QUANT") > 0
                             && row.Field<double>("VALOR") > 0
                             && (row.Field<DateTime>("DATA").CompareTo(dataRef) >= 0)
                             && (row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0)
                             )
                                  group gr by new
                                  {
                                      cod = gr.Field<string>("COD"),
                                      data = gr.Field<DateTime>("DATA"),
                                  } into g
                                  select new
                                  {
                                      cod = g.Key.cod,
                                      data = g.Key.data,
                                      quant = g.Sum(row => row.Field<double>("QUANT")),
                                  }
                                 ).ToList();
                DataTable tabSaidas = new DataTable();
                tabSaidas.Columns.Add("DATA", Type.GetType("System.DateTime"));
                tabSaidas.Columns.Add("COD", Type.GetType("System.String"));
                tabSaidas.Columns["COD"].MaxLength = 4;
                tabSaidas.Columns.Add("QUANT", Type.GetType("System.Double"));
                foreach (var dado in dadoSaidas)
                {
                    DataRow orow = tabSaidas.NewRow();
                    orow["DATA"] = dado.data;
                    orow["COD"] = dado.cod;
                    orow["QUANT"] = dado.quant;
                    tabSaidas.Rows.Add(orow);
                }
                tabSaidas.AcceptChanges();


                var codigos = (from gr in dadoCompras.AsEnumerable()
                               group gr by new
                               {
                                   cod = gr.cod,
                               } into g
                               select new
                               {
                                   cod = g.Key.cod,
                                   lstData = g.Select(obj =>
                                   new {
                                       data = obj.data,
                                       quant = obj.quant,
                                       valor = obj.valor,
                                   }).OrderBy(obj => obj.data).ToList()
                               });
                Dictionary<string, List<InicioFim>> dict = new Dictionary<string, List<InicioFim>>();
                // coloque inicio da compra e vespera da nova compra
                // o CUSTO MEDIO É ALTERADO A CADA NOVA COMPRA, Daí a necessidade de ajustar este periodo
                foreach (var dado in codigos)
                {
                    List<InicioFim> lstInicioFims = new List<InicioFim>();

                    for (int i = 0; i < dado.lstData.Count; i++)
                    {
                        InicioFim inicioFim = new InicioFim();
                        inicioFim.inicio = dado.lstData[i].data;
                        if (i == (dado.lstData.Count - 1)) // ultima data de compra
                        {
                            inicioFim.fim = dtData;
                        }
                        else
                            inicioFim.fim = dado.lstData[i + 1].data.AddDays(-1);
                        inicioFim.quant = dado.lstData[i].quant;
                        inicioFim.valor = dado.lstData[i].valor;
                        // saidas do almox no periodo
                        double quantSaidaPeriodo = tabSaidas.AsEnumerable().Where(obj =>
                          (obj.Field<DateTime>("DATA").CompareTo(inicioFim.inicio) >= 0)
                          && (obj.Field<DateTime>("DATA").CompareTo(inicioFim.fim) <= 0)
                          ).Sum(obj => obj.Field<Double>("QUANT"));
                        inicioFim.quantSaida = quantSaidaPeriodo;
                        lstInicioFims.Add(inicioFim);
                    }
                    // quantidade saida no periodo
                    dict.Add(dado.cod, lstInicioFims);
                }
                // calculo do custo medio

                foreach (KeyValuePair<string, List<InicioFim>> dado in dict)
                {
                    double custoMedio = 0;
                    double quantAcum = 0;
                    double valAcum = 0;
                    InicioFim obj = null;
                    for (int i = 0; i < dado.Value.Count; i++)
                    {
                        obj = dado.Value[i];
                        quantAcum = quantAcum + obj.quant;
                        valAcum = valAcum + Math.Round(obj.valor, 2);
                        // novo Custo Medio
                        custoMedio = Math.Round(valAcum / quantAcum, 5);
                        obj.custoMedio = custoMedio;
                        if (obj.quantSaida < 0)
                            MessageBox.Show("Erro Saida < 0 " + obj.inicio.ToString("d"));

                        double valorSaidasPeriodo = Math.Round(obj.quantSaida * custoMedio, 4);
                        // abatendo a saida
                        quantAcum = Math.Round(quantAcum - obj.quantSaida, 5);
                        valAcum = Math.Round(valAcum - valorSaidasPeriodo, 2);
                        obj.quantAcum = quantAcum;
                        obj.valAcum = valAcum;
                        obj.valorSaida = valorSaidasPeriodo;
                        if ((valAcum < 0) || (quantAcum < 0))
                            MessageBox.Show("Erro < 0 " + obj.inicio.ToString("d"));

                    }
                    result = obj;
                }
                return result;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return null;
            }
        }

        static public async void Execute(DateTime ultData)
        {
            // se já estiver em Processo, não começa outro
            if (inprocess) return;
             
            inprocess = true;
            try
            {
                string str = "SELECT Movest.*, ALLTRIM(CADEST.DESCRI) as DESCRICAO, CADEST.UNID  " +
           " FROM movest, CADEST WHERE " +
           " movest.cod = cadest.cod AND " +
          " MOVEST.DATA >= CTOD('" + dataRef.ToString("MM/dd/yyyy") + "') " +
         " AND MOVEST.DATA <= CTOD('" + ultData.ToString("MM/dd/yyyy") + "') " +
        "  ORDER BY movest.cod ";
                List<string> lstStr = new List<string>();


                lstStr.Add(str);
                DataSet dsDados = await ApiServices.Api_QueryMulti(lstStr);

                if ((dsDados == null) || (dsDados.Tables.Count == 0)) return;
                // Tabela virtual
                tabMovEst = dsDados.Tables[0].Copy();
                tabMovEst.TableName = "MOVESTPURO";

            }
            catch (Exception)
            {
                inprocess = false;
                return;
            }
            inprocess = false;
            return;
        }
        static public bool AltereMovest(DataRow orow, DataRowState estado, Int64 ID = -1)
        {   // o ID será usado só no caso das exclusões
            bool result = false;
            if ((tabMovEst == null) || (tabMovEst.Rows.Count == 0) || inprocess)
            {
                return result;
            }


            if ((estado == DataRowState.Modified) || (estado == DataRowState.Unchanged) )
            {
                DataRow orowAlt = tabMovEst.AsEnumerable().Where(row => row.Field<Int64>("ID") == Convert.ToInt64(orow["ID"])).FirstOrDefault();
                if (orowAlt == null) return result;
                orowAlt.BeginEdit();
                foreach (DataColumn ocol in tabMovEst.Columns)
                {
                    if (ocol.ColumnName.ToUpper() == "ID") continue;
                    if (orow.Table.Columns.Contains(ocol.ColumnName.ToUpper()))
                    {
                        orowAlt[ocol.ColumnName] = orow[ocol.ColumnName];
                    }
                }
                orowAlt.EndEdit();
                orowAlt.AcceptChanges();
            }
            else if ((estado == DataRowState.Added) || (estado == DataRowState.Detached))
            {
                DataRow orowAlt = tabMovEst.NewRow();
                foreach (DataColumn ocol in tabMovEst.Columns)
                {
                    if (orow.Table.Columns.Contains(ocol.ColumnName.ToUpper()))
                    {
                        orowAlt[ocol.ColumnName] = orow[ocol.ColumnName];
                    }
                }
                tabMovEst.Rows.Add(orowAlt);
            }
            else if (estado == DataRowState.Deleted)
            {
                DataRow orowAlt = tabMovEst.AsEnumerable().Where(row => row.Field<Int64>("ID") == ID).FirstOrDefault();
                if (orowAlt == null) return result;
                orowAlt.Delete();
                
            }
            // FALTA DELETE
            tabMovEst.AcceptChanges();
            return true;
        }
       */

        // RELATORIO ESTOQUE
       



    }



}
