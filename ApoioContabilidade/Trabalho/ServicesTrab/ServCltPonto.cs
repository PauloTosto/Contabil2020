using ApoioContabilidade.Trabalho.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho.ServicesTrab
{
    public class ServCltPonto
    {
        public DataTable tabListaTrab;
        public Dictionary<string,DateTime> CadAdmi;
        public Dictionary<string, CltCodigo> ListaCodigo;
        public DateTime DataSemana;
        public void AdoHoras_Valores(DataRow orow)
        {
            DataRow dataRowlst = null;
            dataRowlst = tabListaTrab.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim()
                   == orow["TRAB"].ToString().Trim()).FirstOrDefault();
            if (dataRowlst == null)
            {
                dataRowlst = tabListaTrab.NewRow();
                dataRowlst["ADMI"] = CadAdmi[orow["TRAB"].ToString()];    // orow["ADMI"];
                dataRowlst["CODCAD"] = orow["TRAB"];
                tabListaTrab.Rows.Add(dataRowlst);
                tabListaTrab.AcceptChanges();
            }
            dataRowlst.BeginEdit();
            if (orow["TIPOMOV"].ToString() == "D")
            {
                for (int i = 1; i <= 7; i++)
                {
                    string campo = "DIA" + i.ToString();
                    string horas = "HORAS" + i.ToString();
                    string dia_fe = "DIA_FE" + i.ToString();
                    string compativel = "COMPATIVEL" + i.ToString();
                    if (orow.IsNull(campo) || orow.Field<string>(campo).Trim() == "") continue;
                    string ttext = orow.Field<string>(campo).PadRight(2);
                    if (ListaCodigo.ContainsKey(ttext))
                    {
                        CltCodigo ocod = ListaCodigo[ttext];
                        if (ocod.Diarias > 0)
                        {
                            if (orow["NOTURNO"].ToString().Trim() != "X")
                            {
                                dataRowlst["DIARIAS"] = Convert.ToDouble(dataRowlst["DIARIAS"]) +
                                    Math.Round(ocod.Diarias / 8,4);
                                dataRowlst[campo] = Convert.ToDouble(dataRowlst[campo]) + ocod.Diarias;
                            }
                            else
                            {
                                dataRowlst["DIARIASN"] = Convert.ToDouble(dataRowlst["DIARIASN"]) +
                                    Math.Round(ocod.Diarias / 8,4);
                                dataRowlst[campo] = Convert.ToDouble(dataRowlst[campo]) + ocod.Diarias;
                            }
                        }
                        else
                        {
                            if (ocod.IndCod.Trim() != "")
                            {
                                dataRowlst[dia_fe] = 8;
                            }
                        }
                        /// HORAS
                        if (ocod.Horas50 > 0)
                        {
                            dataRowlst["HORAS50"] = Convert.ToDouble(dataRowlst["HORAS50"]) +
                           ocod.Horas50;
                            dataRowlst[horas] = Convert.ToDouble(dataRowlst[horas]) + ocod.Horas50;
                        }
                        if (ocod.HorasFe > 0)
                        {
                            if (i == 4)
                            {
                                dataRowlst["HORAS100"] = Convert.ToDouble(dataRowlst["HORAS100"]) +
                                             ocod.HorasFe;
                            }
                            else
                            {
                                dataRowlst["HORASFE"] = Convert.ToDouble(dataRowlst["HORASFE"]) +
                               ocod.HorasFe;
                            }
                        }
                        if (ocod.Compati.Trim() == "N")
                        {
                            dataRowlst[compativel] = ocod.Compati.Trim();
                        }
                        if (ocod.IndCod == "FI")
                        {
                            if ((campo == "DIA1") || (campo == "DIA2") || (campo == "DIA3"))
                            {
                                dataRowlst["FI_1_2_3"] = Convert.ToInt16(dataRowlst["FI_1_2_3"]) + 1;
                            }
                            else
                                dataRowlst["FI_5_6_7"] = Convert.ToInt16(dataRowlst["FI_5_6_7"]) + 1;
                        }
                        if ((campo == "DIA4") && (ocod.IndCod.Trim() == "R"))
                        {
                            dataRowlst["REMUN"] = true;
                        }
                        if ((campo == "DIA4") && (ocod.IndCod.Trim() == "X"))
                        {
                            dataRowlst["DOMINGO_TRAB"] = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tabela de Codigo Incoerente :" + ttext);
                    }
                }
            }
            else
            {
                dataRowlst["VALOREMP"] = Convert.ToDouble(dataRowlst["VALOREMP"]) +
                    Convert.ToDouble(orow["VALOR"]);
            }
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
        }
        public void AdoHoras_SemanaAnteriorNovo(DataRow orow)
        {
            DataRow dataRowlst = null;
            try
            {
                dataRowlst = tabListaTrab.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim() == orow["TRAB"].ToString().Trim()).FirstOrDefault();

              
            if (dataRowlst == null)
            {
                dataRowlst = tabListaTrab.NewRow();
                dataRowlst["ADMI"] = CadAdmi[orow["TRAB"].ToString()]; // orow["ADMI"];
                dataRowlst["CODCAD"] = orow["TRAB"];
                tabListaTrab.Rows.Add(dataRowlst);
                tabListaTrab.AcceptChanges();
            }
            dataRowlst.BeginEdit();
            if ((orow["DIA5"].ToString().Trim() == "FI") ||
                 (orow["DIA6"].ToString().Trim() == "FI") ||
                (orow["DIA7"].ToString().Trim() == "FI"))
            {
                dataRowlst["FI_ANTERIOR"] = true;
            }
            if (orow["DIA4"].ToString().Trim() == "X")
            {
                dataRowlst["DOMINGOTRAB_ANTERIOR"] = true;

            }


            if (orow["TIPOMOV"].ToString() == "D")
            {
                for (int i = 5; i <= 7; i++)
                {
                    string campo = "DIA" + i.ToString();
                    string dia_ant = "DIA_ANT" + i.ToString();
                    string dia_fe_ant = "DIA_FE_ANT" + i.ToString();
                    if (orow.IsNull(campo) || orow.Field<string>(campo).Trim() == "") continue;
                    string ttext = orow.Field<string>(campo).PadRight(2);
                    if (ListaCodigo.ContainsKey(ttext))
                    {
                        CltCodigo ocod = ListaCodigo[ttext];
                        if (ocod.Diarias > 0)
                        {
                            dataRowlst[dia_ant] = Convert.ToDouble(dataRowlst[dia_ant]) +
                                   ocod.Diarias;
                        }
                        else
                        {
                            if (ocod.IndCod.Trim() != "")
                            {
                                dataRowlst[dia_fe_ant] = 8;
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("Tabela de Codigo Incoerente :" + ttext);
                    }
                }
            }
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
            }
            catch (Exception E)
            {

                MessageBox.Show("Erro Processo ");
            }
        }

        // opera com o cltcad

        public void Reinicialize_RegistroListaTrab(DataRow orow)
        {
            DataRow dataRowlst = null;
            dataRowlst = tabListaTrab.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim() == orow["CODCAD"].ToString().Trim()).FirstOrDefault();
            dataRowlst.BeginEdit();
            dataRowlst["DIARIAS"] = 0;
            dataRowlst["HORAS50"] = 0;
            dataRowlst["HORAS100"] = 0;
            dataRowlst["HORASFE"] = 0;
            dataRowlst["VALOREMP"] = 0;
            dataRowlst["DIARIASN"] = 0;
            dataRowlst["HORASN"] = 0;
            dataRowlst["HORAS50N"] = 0;
            dataRowlst["HORASFEN"] = 0;
            dataRowlst["DIA1"] = 0;
            dataRowlst["DIA2"] = 0;
            dataRowlst["DIA3"] = 0;
            dataRowlst["DIA4"] = 0;
            dataRowlst["DIA5"] = 0;
            dataRowlst["DIA6"] = 0;
            dataRowlst["DIA7"] = 0;
            dataRowlst["HORAS1"] = 0;
            dataRowlst["HORAS2"] = 0;
            dataRowlst["HORAS3"] = 0;
            dataRowlst["HORAS4"] = 0;
            dataRowlst["HORAS5"] = 0;
            dataRowlst["HORAS6"] = 0;
            dataRowlst["HORAS7"] = 0;
            dataRowlst["DIA_FE1"] = 0;
            dataRowlst["DIA_FE2"] = 0;
            dataRowlst["DIA_FE3"] = 0;
            dataRowlst["DIA_FE4"] = 0;
            dataRowlst["DIA_FE5"] = 0;
            dataRowlst["DIA_FE6"] = 0;
            dataRowlst["DIA_FE7"] = 0;
            dataRowlst["COMPATIVEL1"] = "";
            dataRowlst["COMPATIVEL2"] = "";
            dataRowlst["COMPATIVEL3"] = "";
            dataRowlst["COMPATIVEL4"] = "";
            dataRowlst["COMPATIVEL5"] = "";
            dataRowlst["COMPATIVEL6"] = "";
            dataRowlst["COMPATIVEL7"] = "";
            dataRowlst["FI_1_2_3"] = 0;
            dataRowlst["FI_5_6_7"] = 0;
            dataRowlst["DOMINGO_TRAB"] = false;
            dataRowlst["REMUN"] = false;
            /*taRowlst["DOMINGOTRAB_ANTERIOR", Type.GetType("System.Boolean"));
            dataRowlst["EMFERIAS", Type.GetType("System.Boolean"));
            dataRowlst["FI_ANTERIOR", Type.GetType("System.Boolean"));

            */


            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
            return;
        }
        public void AdotblAssocCalcFields(DataRow orow)
        {
            DataRow dataRowlst = null;

            //if (tabListaTrab.Rows.Count > 0)
            // {
            dataRowlst = tabListaTrab.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim() == orow["CODCAD"].ToString().Trim()).FirstOrDefault();

           /*if (orow["CODCAD"].ToString().Trim() == "8298")
            {
                string foo = orow["CODCAD"].ToString().Trim();
            }*/
           
            //}
            if (dataRowlst == null)
            {
                orow.BeginEdit();
                orow["DIARIAS"] = 0;
                orow["HORAS50"] = 0;
                orow["HORAS100"] = 0;
                orow["HORASFE"] = 0;
                orow["VALOREMP"] = 0;
                orow["DIARIASN"] = 0;
                orow["CRITICA"] = "";
                orow.EndEdit();
                orow.AcceptChanges();
                return;
            }
            orow.BeginEdit();
            try
            {
                for (int i = 5; i <= 7; i++)
                {
                    string dia_ant = "DIA_ANT" + i.ToString();
                    string dia_fe_ant = "DIA_FE_ANT" + i.ToString();
                    if ((Math.Round(Convert.ToDouble(dataRowlst[dia_ant]),3) < 8)
                         && (Math.Round(Convert.ToDouble(dataRowlst[dia_fe_ant]),3) < 8)
                        && (!(Convert.ToDateTime(dataRowlst["ADMI"]).CompareTo(DataSemana.AddDays((-7 + (i - 1)))) > 0)))
                    {
                        dataRowlst.BeginEdit();
                        dataRowlst["FI_Anterior"] = true;
                        dataRowlst.EndEdit();
                        dataRowlst.AcceptChanges();
                    }
                }
                orow["DIARIAS"] = dataRowlst["DIARIAS"];
                orow["HORAS50"] = dataRowlst["HORAS50"];
                orow["HORAS100"] = dataRowlst["HORAS100"];
                orow["HORASFE"] = dataRowlst["HORASFE"];
                orow["DIARIASN"] = dataRowlst["DIARIASN"];
                orow["VALOREMP"] = 0;
                orow["CRITICA"] = "";
                if (Convert.ToBoolean(dataRowlst["DOMINGO_TRAB"])
                         && Convert.ToBoolean(dataRowlst["DOMINGOTRAB_ANTERIOR"]))
                    orow["CRITICA"] = "Domingo Trab.Seguido";
                if (Convert.ToBoolean(dataRowlst["REMUN"])
                         && Convert.ToBoolean(dataRowlst["FI_ANTERIOR"]))
                    orow["CRITICA"] = "Remunerado Com FI Anterior";
                if (Convert.ToBoolean(dataRowlst["REMUN"])
                         && (Convert.ToInt16(dataRowlst["FI_1_2_3"]) > 0))
                    orow["CRITICA"] = "Remunerado Com FI Anterior";
                for (int i = 1; i <= 7; i++)
                {
                    string horas = "HORAS" + i.ToString();
                    if (Math.Round(Convert.ToDouble(dataRowlst[horas]),3) > 2)
                        orow["CRITICA"] = "Horas Extras > 2 ";
                }
                if (Convert.ToBoolean(dataRowlst["REMUN"]))
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        string dia = "DIA" + i.ToString();
                        string dia_fe = "DIA_FE" + i.ToString();
                        if ((Math.Round(Convert.ToDouble(dataRowlst[dia]),3) < 8)
                             && (Math.Round(Convert.ToDouble(dataRowlst[dia_fe]),3) < 8)
                            && (!(Convert.ToDateTime(dataRowlst["ADMI"]).CompareTo(DataSemana.AddDays((i - 1))) > 0)))
                        {
                            orow["CRITICA"] = "Remunerado Indevido";
                        }
                    }
                    // sabado DIA3
                    string dia3 = "DIA3";
                    string dia_fe3 = "DIA_FE3";
                    if ((Math.Round(Convert.ToDouble(dataRowlst[dia3]),3) < 4)
                         && (Math.Round(Convert.ToDouble(dataRowlst[dia_fe3]),3) < 8)
                        && (!(Convert.ToDateTime(dataRowlst["ADMI"]).CompareTo(DataSemana.AddDays((3 - 1))) > 0)))
                    {
                        orow["CRITICA"] = "Remunerado Indevido";
                    }
                }

                for (int i = 1; i <= 7; i++)
                {
                    string dia = "DIA" + i.ToString();
                    if (i != 4)
                    {
                        if (Math.Round(Convert.ToDouble(dataRowlst[dia]),3) > 8)
                            orow["CRITICA"] = "Horas Normais Excedentes ";
                    }
                    else
                    {
                        if (!Convert.ToBoolean(dataRowlst["REMUN"]))
                        {
                            if (Math.Round(Convert.ToDouble(dataRowlst[dia]),3) > 8)
                                orow["CRITICA"] = "Horas Normais Excedentes ";
                        }
                        else
                        {
                            if (Math.Round(Convert.ToDouble(dataRowlst[dia]),3) > 16)
                                orow["CRITICA"] = "Horas Normais Excedentes ";
                        }
                    }
                }
                for (int i = 1; i <= 7; i++)
                {
                    string dia = "DIA" + i.ToString();
                    string horas = "HORAS" + i.ToString();
                    if (i == 3) // sabado
                    {
                        if ((Convert.ToDouble(dataRowlst[horas]) > 0)
                             && (Math.Round(Convert.ToDouble(dataRowlst[dia]),3) < (8 / 2)))
                            orow["CRITICA"] = "Dia Incompl. + Extra? ";
                        
                         if (Math.Round(Convert.ToDouble(dataRowlst[dia]),3) > 8 )
                            orow["CRITICA"] = "Horas Normais Excedentes (sabado)";
                    }
                    else
                    {
                        if ((Math.Round(Convert.ToDouble(dataRowlst[horas]),3) > 0)
                         && (Math.Round(Convert.ToDouble(dataRowlst[dia]),3) < 8))
                            orow["CRITICA"] = "Dia Incompl. + Extra? ";
                    }
                }

            }
            catch (Exception E)
            {
                MessageBox.Show("Erro Ao Criticar Trab "+ orow["CODCAD"].ToString() +" Msg:" + E.Message);
                throw;
            }
            orow.EndEdit();
            orow.AcceptChanges();
        }

        public void AdoZere_Horas_ExcetoSemanaAnterior(DataRow orow)
        {
            DataRow dataRowlst = null;

            if (tabListaTrab.Rows.Count > 0)
            {
                tabListaTrab.AsDataView().RowFilter = "CODCAD = '" + orow["TRAB"] + "'";
                if (tabListaTrab.AsDataView().Count > 0)
                    dataRowlst = tabListaTrab.AsDataView()[0].Row;
                tabListaTrab.AsDataView().RowFilter = "";
            }
            if (dataRowlst == null)
            {
                dataRowlst = tabListaTrab.NewRow();
                dataRowlst["ADMI"] = CadAdmi[orow["TRAB"].ToString()]; // orow["ADMI"];
                dataRowlst["CODCAD"] = orow["TRAB"];
                tabListaTrab.Rows.Add(dataRowlst);
                tabListaTrab.AcceptChanges();
            }

            dataRowlst.BeginEdit();
            dataRowlst["DIARIAS"] = 0;
            dataRowlst["HORAS50"] = 0;
            dataRowlst["HORAS100"] = 0;
            dataRowlst["HORASFE"] = 0;
            dataRowlst["VALOREMP"] = 0;
            dataRowlst["DIARIASN"] = 0;
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
            for (int i = 1; i <= 7; i++)
            {
                string campo = "DIA" + i.ToString();
                string horas = "HORAS" + i.ToString();
                string dia_fe = "DIA_FE" + i.ToString();
                string compativel = "COMPATIVEL" + i.ToString();
                dataRowlst[campo] = 0;
                dataRowlst[horas] = 0;
                dataRowlst[dia_fe] = 0;
                dataRowlst[compativel] = "";
            }
            dataRowlst["FI_1_2_3"] = 0;
            dataRowlst["FI_5_6_7"] = 0;
            dataRowlst["DOMINGO_TRAB"] = false;
            dataRowlst["REMUN"] = false;
            dataRowlst["EmFerias"] = false;
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
        }
        public void AdoZere_Horas_ExcetoSemanaAnterior_casoExcluidosTodos(string codcad)
        {
            DataRow dataRowlst = null;

            if (tabListaTrab.Rows.Count > 0)
            {
                tabListaTrab.AsDataView().RowFilter = "CODCAD = '" + codcad + "'";
                if (tabListaTrab.AsDataView().Count > 0)
                    dataRowlst = tabListaTrab.AsDataView()[0].Row;
                tabListaTrab.AsDataView().RowFilter = "";
            }
            if (dataRowlst == null)
            {
                return;
            }

            dataRowlst.BeginEdit();
            dataRowlst["DIARIAS"] = 0;
            dataRowlst["HORAS50"] = 0;
            dataRowlst["HORAS100"] = 0;
            dataRowlst["HORASFE"] = 0;
            dataRowlst["VALOREMP"] = 0;
            dataRowlst["DIARIASN"] = 0;
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
            for (int i = 1; i <= 7; i++)
            {
                string campo = "DIA" + i.ToString();
                string horas = "HORAS" + i.ToString();
                string dia_fe = "DIA_FE" + i.ToString();
                string compativel = "COMPATIVEL" + i.ToString();
                dataRowlst[campo] = 0;
                dataRowlst[horas] = 0;
                dataRowlst[dia_fe] = 0;
                dataRowlst[compativel] = "";
            }
            dataRowlst["FI_1_2_3"] = 0;
            dataRowlst["FI_5_6_7"] = 0;
            dataRowlst["DOMINGO_TRAB"] = false;
            dataRowlst["REMUN"] = false;
            dataRowlst["EmFerias"] = false;
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
        }

        public void AdoZere_Horas_Valores(DataRow orow)
        {
            DataRow dataRowlst = null;

            if (tabListaTrab.Rows.Count > 0)
            {
                tabListaTrab.AsDataView().RowFilter = "CODCAD = '" + orow["TRAB"] + "'";
                if (tabListaTrab.AsDataView().Count > 0)
                    dataRowlst = tabListaTrab.AsDataView()[0].Row;
                tabListaTrab.AsDataView().RowFilter = "";
            }
            if (dataRowlst == null)
            {
                dataRowlst = tabListaTrab.NewRow();
                dataRowlst["ADMI"] = CadAdmi[orow["TRAB"].ToString()]; // orow["ADMI"];
                dataRowlst["CODCAD"] = orow["TRAB"];
                tabListaTrab.Rows.Add(dataRowlst);
                tabListaTrab.AcceptChanges();
            }

            dataRowlst.BeginEdit();
            dataRowlst["DIARIAS"] = 0;
            dataRowlst["HORAS50"] = 0;
            dataRowlst["HORAS100"] = 0;
            dataRowlst["HORASFE"] = 0;
            dataRowlst["VALOREMP"] = 0;
            dataRowlst["DIARIASN"] = 0;
            dataRowlst["CRITICA"] = "";
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
            for (int i = 1; i <= 7; i++)
            {
                string campo = "DIA" + i.ToString();
                string horas = "HORAS" + i.ToString();
                string dia_fe = "DIA_FE" + i.ToString();
                string compativel = "COMPATIVEL" + i.ToString();
                string dia_ant = "DIA_ANT" + i.ToString();
                string dia_fe_ant = "DIA_FE_ANT" + i.ToString();

                dataRowlst[campo] = 0;
                dataRowlst[horas] = 0;
                dataRowlst[dia_fe] = 0;
                dataRowlst[dia_ant] = 0;
                dataRowlst[dia_fe_ant] = 0;

                dataRowlst[compativel] = "";
            }
            dataRowlst["FI_1_2_3"] = 0;
            dataRowlst["FI_5_6_7"] = 0;
            dataRowlst["DOMINGO_TRAB"] = false;
            dataRowlst["REMUN"] = false;
            dataRowlst["EmFerias"] = false;
            dataRowlst.EndEdit();
            dataRowlst.AcceptChanges();
        }
        public DataTable ConstruaCltCad(DataTable cltCad)
        {
            DataTable tabCltCad = cltCad.Copy();
            tabCltCad.Columns.Add("DIARIAS", Type.GetType("System.Double"));
            tabCltCad.Columns[tabCltCad.Columns.Count -1].DefaultValue = 0;
            tabCltCad.Columns.Add("HORAS50", Type.GetType("System.Double"));
            tabCltCad.Columns[tabCltCad.Columns.Count - 1].DefaultValue = 0;
            tabCltCad.Columns.Add("HORAS100", Type.GetType("System.Double"));
            tabCltCad.Columns[tabCltCad.Columns.Count - 1].DefaultValue = 0;
            tabCltCad.Columns.Add("HORASFE", Type.GetType("System.Double"));
            tabCltCad.Columns[tabCltCad.Columns.Count - 1].DefaultValue = 0;
            tabCltCad.Columns.Add("VALOREMP", Type.GetType("System.Double"));
            tabCltCad.Columns[tabCltCad.Columns.Count -1].DefaultValue = 0;
            tabCltCad.Columns.Add("DIARIASN", Type.GetType("System.Double"));
            tabCltCad.Columns[tabCltCad.Columns.Count - 1].DefaultValue = 0;
            tabCltCad.Columns.Add("CRITICA", Type.GetType("System.String"));
            tabCltCad.Columns["CRITICA"].MaxLength = 50;
            tabCltCad.Columns.Add("APONTAMENTOPROV", Type.GetType("System.String"));
            tabCltCad.Columns["APONTAMENTOPROV"].MaxLength = 50;

            tabCltCad.Columns[tabCltCad.Columns.Count - 1].DefaultValue = "";
            tabCltCad.Columns.Add("ULT_ATUAL", Type.GetType("System.DateTime"));
            return tabCltCad;
        }
        public DataTable AdoLista()
        {
            DataTable tabListaTrab = new DataTable();
            tabListaTrab.Columns.Add("CODCAD", Type.GetType("System.String"));
            tabListaTrab.Columns["CODCAD"].MaxLength = 4;
            tabListaTrab.Columns.Add("NOMECAD", Type.GetType("System.String"));
            tabListaTrab.Columns["NOMECAD"].MaxLength = 40;
            tabListaTrab.Columns.Add("SETOR", Type.GetType("System.String"));
            tabListaTrab.Columns["SETOR"].MaxLength = 2;
            tabListaTrab.Columns.Add("SALBASE", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("ADMI", Type.GetType("System.DateTime"));
            tabListaTrab.Columns.Add("ULT_ATUAL", Type.GetType("System.DateTime"));

            tabListaTrab.Columns.Add("DIARIAS", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS50", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS100", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORASFE", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIARIASN", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORASN", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS50N", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORASFEN", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("VLRDIARIA", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("VALOREMP", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA1", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA2", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA3", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA4", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA5", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA6", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA7", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_ANT1", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_ANT2", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_ANT3", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_ANT4", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_ANT5", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_ANT6", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_ANT7", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS1", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS2", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS3", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS4", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS5", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS6", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("HORAS7", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE1", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE2", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE3", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE4", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE5", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE6", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE7", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE_ANT1", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE_ANT2", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE_ANT3", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE_ANT4", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE_ANT5", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE_ANT6", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("DIA_FE_ANT7", Type.GetType("System.Double"));
            tabListaTrab.Columns.Add("COMPATIVEL1", Type.GetType("System.String"));
            tabListaTrab.Columns.Add("COMPATIVEL2", Type.GetType("System.String"));
            tabListaTrab.Columns.Add("COMPATIVEL3", Type.GetType("System.String"));
            tabListaTrab.Columns.Add("COMPATIVEL4", Type.GetType("System.String"));
            tabListaTrab.Columns.Add("COMPATIVEL5", Type.GetType("System.String"));
            tabListaTrab.Columns.Add("COMPATIVEL6", Type.GetType("System.String"));
            tabListaTrab.Columns.Add("COMPATIVEL7", Type.GetType("System.String"));
            tabListaTrab.Columns["COMPATIVEL1"].MaxLength = 1;
            tabListaTrab.Columns["COMPATIVEL2"].MaxLength = 1;
            tabListaTrab.Columns["COMPATIVEL3"].MaxLength = 1;
            tabListaTrab.Columns["COMPATIVEL4"].MaxLength = 1;
            tabListaTrab.Columns["COMPATIVEL5"].MaxLength = 1;
            tabListaTrab.Columns["COMPATIVEL6"].MaxLength = 1;
            tabListaTrab.Columns["COMPATIVEL7"].MaxLength = 1;
            tabListaTrab.Columns.Add("FI_1_2_3", Type.GetType("System.Int16"));
            tabListaTrab.Columns.Add("FI_5_6_7", Type.GetType("System.Int16"));
            tabListaTrab.Columns.Add("DOMINGO_TRAB", Type.GetType("System.Boolean"));
            tabListaTrab.Columns.Add("DOMINGOTRAB_ANTERIOR", Type.GetType("System.Boolean"));
            tabListaTrab.Columns.Add("REMUN", Type.GetType("System.Boolean"));
            tabListaTrab.Columns.Add("EMFERIAS", Type.GetType("System.Boolean"));
            tabListaTrab.Columns.Add("FI_ANTERIOR", Type.GetType("System.Boolean"));

            foreach (DataColumn col in tabListaTrab.Columns)
            {
                if ((col.DataType == Type.GetType("System.Int16"))
                   || (col.DataType == Type.GetType("System.Double")))
                    col.DefaultValue = 0;
                if (col.DataType == Type.GetType("System.Boolean"))
                    col.DefaultValue = false;
            }

            return tabListaTrab;
        }



     

    }
}
