using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using ClassConexao;
using ClassFiltroEdite;

namespace ClassLibTrabalho
{
    public class ClassCLTFOLHA
    {
        static public DataTable dtCLTFOLHA()
        {
            DataTable otable = new DataTable("CLTFOLHA");
            otable.Columns.Add("DATA", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("SETOR", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CENTRO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUADRA", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TRAB", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TIPO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NUM_MOD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CODSER", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUANT", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("SALARIO", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("VLR_HXS", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("VLR_HXA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("VLR_HXN", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("FGTS", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("SALFAM", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("INSS", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("FERIAS", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("DECIMO", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("FGTS_FE", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("FGTS_DEC", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("EDUC", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("TERC", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("EDUC_FE", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("EDUC_DEC", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("MULTA_FGTS", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("HX", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("HXS", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("HXA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("HXN", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("HEFETIVA", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            otable.Columns.Add("PONTO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 62;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("IRFONTE", System.Type.GetType("System.Decimal"));
            otable.Columns[otable.Columns.Count - 1].DefaultValue = 0.0;
            return otable;
        }
        static public OleDbDataAdapter CLTFOLHAConstruaAdaptador(string opath)
        {
            OleDbDataAdapter OleDbda = new OleDbDataAdapter();

            // Construção do insert padrao 

            string path = TDataControlReduzido.Get_Path(opath);

            OleDbCommand cmd = new OleDbCommand(
            "INSERT INTO " + path + "CLTFOLHA ( " +
            " DATA,  SETOR,  CENTRO,  QUADRA,  TRAB,  TIPO, " +
            " NUM_MOD,  CODSER,  QUANT,  SALARIO,  VLR_HXS, " +
            " VLR_HXA,  VLR_HXN,  FGTS,  SALFAM,  INSS,  FERIAS, " +
            " DECIMO,  FGTS_FE,  FGTS_DEC,  EDUC,  TERC,  EDUC_FE, " +
            " EDUC_DEC,  MULTA_FGTS,  HX,  HXS,  HXA,  HXN, " +
            " HEFETIVA,  PONTO,  IRFONTE ) VALUES (  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());


            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 4, "CENTRO");
            cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@TRAB", OleDbType.Char, 4, "TRAB");
            cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@SALARIO", OleDbType.Numeric, 0, "SALARIO");
            cmd.Parameters.Add("@VLR_HXS", OleDbType.Numeric, 0, "VLR_HXS");
            cmd.Parameters.Add("@VLR_HXA", OleDbType.Numeric, 0, "VLR_HXA");
            cmd.Parameters.Add("@VLR_HXN", OleDbType.Numeric, 0, "VLR_HXN");
            cmd.Parameters.Add("@FGTS", OleDbType.Numeric, 0, "FGTS");
            cmd.Parameters.Add("@SALFAM", OleDbType.Numeric, 0, "SALFAM");
            cmd.Parameters.Add("@INSS", OleDbType.Numeric, 0, "INSS");
            cmd.Parameters.Add("@FERIAS", OleDbType.Numeric, 0, "FERIAS");
            cmd.Parameters.Add("@DECIMO", OleDbType.Numeric, 0, "DECIMO");
            cmd.Parameters.Add("@FGTS_FE", OleDbType.Numeric, 0, "FGTS_FE");
            cmd.Parameters.Add("@FGTS_DEC", OleDbType.Numeric, 0, "FGTS_DEC");
            cmd.Parameters.Add("@EDUC", OleDbType.Numeric, 0, "EDUC");
            cmd.Parameters.Add("@TERC", OleDbType.Numeric, 0, "TERC");
            cmd.Parameters.Add("@EDUC_FE", OleDbType.Numeric, 0, "EDUC_FE");
            cmd.Parameters.Add("@EDUC_DEC", OleDbType.Numeric, 0, "EDUC_DEC");
            cmd.Parameters.Add("@MULTA_FGTS", OleDbType.Numeric, 0, "MULTA_FGTS");
            cmd.Parameters.Add("@HX", OleDbType.Numeric, 0, "HX");
            cmd.Parameters.Add("@HXS", OleDbType.Numeric, 0, "HXS");
            cmd.Parameters.Add("@HXA", OleDbType.Numeric, 0, "HXA");
            cmd.Parameters.Add("@HXN", OleDbType.Numeric, 0, "HXN");
            cmd.Parameters.Add("@HEFETIVA", OleDbType.Numeric, 0, "HEFETIVA");
            cmd.Parameters.Add("@PONTO", OleDbType.Char, 62, "PONTO");
            cmd.Parameters.Add("@IRFONTE", OleDbType.Numeric, 0, "IRFONTE");
            OleDbda.InsertCommand = cmd;
            // Construção do altera padrao 
            cmd = new OleDbCommand(
           "UPDATE " + path + "CLTFOLHA SET " +
           " DATA = ? ,  SETOR = ? ,  CENTRO = ? ,  QUADRA = ? , " +
           " TRAB = ? ,  TIPO = ? ,  NUM_MOD = ? ,  CODSER = ? , " +
           " QUANT = ? ,  SALARIO = ? ,  VLR_HXS = ? ,  VLR_HXA = ? , " +
           " VLR_HXN = ? ,  FGTS = ? ,  SALFAM = ? ,  INSS = ? , " +
           " FERIAS = ? ,  DECIMO = ? ,  FGTS_FE = ? ,  FGTS_DEC = ? , " +
           " EDUC = ? ,  TERC = ? ,  EDUC_FE = ? ,  EDUC_DEC = ? , " +
           " MULTA_FGTS = ? ,  HX = ? ,  HXS = ? ,  HXA = ? , " +
           " HXN = ? ,  HEFETIVA = ? ,  PONTO = ? ,  IRFONTE = ? " +
           " WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@CENTRO", OleDbType.Char, 4, "CENTRO");
            cmd.Parameters.Add("@QUADRA", OleDbType.Char, 3, "QUADRA");
            cmd.Parameters.Add("@TRAB", OleDbType.Char, 4, "TRAB");
            cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@SALARIO", OleDbType.Numeric, 0, "SALARIO");
            cmd.Parameters.Add("@VLR_HXS", OleDbType.Numeric, 0, "VLR_HXS");
            cmd.Parameters.Add("@VLR_HXA", OleDbType.Numeric, 0, "VLR_HXA");
            cmd.Parameters.Add("@VLR_HXN", OleDbType.Numeric, 0, "VLR_HXN");
            cmd.Parameters.Add("@FGTS", OleDbType.Numeric, 0, "FGTS");
            cmd.Parameters.Add("@SALFAM", OleDbType.Numeric, 0, "SALFAM");
            cmd.Parameters.Add("@INSS", OleDbType.Numeric, 0, "INSS");
            cmd.Parameters.Add("@FERIAS", OleDbType.Numeric, 0, "FERIAS");
            cmd.Parameters.Add("@DECIMO", OleDbType.Numeric, 0, "DECIMO");
            cmd.Parameters.Add("@FGTS_FE", OleDbType.Numeric, 0, "FGTS_FE");
            cmd.Parameters.Add("@FGTS_DEC", OleDbType.Numeric, 0, "FGTS_DEC");
            cmd.Parameters.Add("@EDUC", OleDbType.Numeric, 0, "EDUC");
            cmd.Parameters.Add("@TERC", OleDbType.Numeric, 0, "TERC");
            cmd.Parameters.Add("@EDUC_FE", OleDbType.Numeric, 0, "EDUC_FE");
            cmd.Parameters.Add("@EDUC_DEC", OleDbType.Numeric, 0, "EDUC_DEC");
            cmd.Parameters.Add("@MULTA_FGTS", OleDbType.Numeric, 0, "MULTA_FGTS");
            cmd.Parameters.Add("@HX", OleDbType.Numeric, 0, "HX");
            cmd.Parameters.Add("@HXS", OleDbType.Numeric, 0, "HXS");
            cmd.Parameters.Add("@HXA", OleDbType.Numeric, 0, "HXA");
            cmd.Parameters.Add("@HXN", OleDbType.Numeric, 0, "HXN");
            cmd.Parameters.Add("@HEFETIVA", OleDbType.Numeric, 0, "HEFETIVA");
            cmd.Parameters.Add("@PONTO", OleDbType.Char, 62, "PONTO");
            cmd.Parameters.Add("@IRFONTE", OleDbType.Numeric, 0, "IRFONTE");
            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;
            OleDbda.UpdateCommand = cmd;

            // Construção do deleta padrao 

           /* cmd = new OleDbCommand(
           " DELETE FROM " + path + " CLTFOLHA" +
           " WHERE ( RECNO() = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@RECNO()", OleDbType.Numeric, 10, "RECNO()").SourceVersion = DataRowVersion.Original;*/
            cmd = new OleDbCommand(
            "DELETE FROM " + path + "CLTFOLHA WHERE (DATA  BETWEEN ? AND ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@INICIO", OleDbType.DBDate, 0);
            cmd.Parameters.Add("@FIM", OleDbType.DBDate, 0);
            OleDbda.DeleteCommand = cmd;
            
            return OleDbda;
        }

        // Folha de Araque
        #region

        static public DataSet Get_CltFolha(DataTable ocltfolha, DateTime data1, DateTime data2, List<LinhaSolucao> oLista,DataTable cltcad, DataTable cltadiant)
        {
     
            string path;//, strclttrab;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("TRABALHO");

            try
            {

                DataSet odataset = new DataSet();//TDataControlTrabalho.Get_CltCadastroFolha(data1, data2, oLista);
                if (cltcad == null)
                {
                    odataset = TDataControlTrabalho.Get_CltCadastroFolha(data1, data2, oLista);
                    cltcad = odataset.Tables[0];
                }

                odataset.Tables.Add(ocltfolha.Copy());

                //DataSet odataset = new DataSet();
                //oda

                if (cltadiant == null)
                {
                    DataSet dsAdiant = TDataControlTrabalho.Get_CltAdiant(data1, oLista);
                    cltadiant = dsAdiant.Tables[0];
                    cltadiant.TableName = "CLTADIAN";
                }
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = cltcad.Columns["CODCAD"];     // odataset.Tables["CLTCAD"].Columns["CODCAD"];
                cltcad.PrimaryKey = PrimaryKeyColumns;


                DataTable cltFolhaTot = odataset.Tables["CLTFOLHA"].Clone();
                cltFolhaTot.TableName = "CLTFOLHATOT";

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "NOME", 45, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ADIANT", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALLIQ", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SBRUTO", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "HE50", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALARIOREAL", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ISIND", 0, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CONTABCO", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "AGENCIA", 8, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "BANCO", 3, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "OBSBANCO", 21, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CPF", 14, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "END", 50, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CIDADE", 25, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CEP", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "UF", 2, false));


                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CBO", 20, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CATEGORIA", 30, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "ADMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "DEMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "MENSALISTA", 1, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "SETORCAD", 2, false));

                result = new DataSet();
                result.Tables.Add(cltFolhaTot);

                PrimaryKeyColumns[0] = result.Tables["CLTFOLHATOT"].Columns["TRAB"];
                result.Tables["CLTFOLHATOT"].PrimaryKey = PrimaryKeyColumns;

                DataRow odatarow;

                for (int i = 0; i < odataset.Tables["CLTFOLHA"].Rows.Count; i++)
                {
                    odatarow = odataset.Tables["CLTFOLHA"].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                    DataRow totrow = cltFolhaTot.Rows.Find(odatarow["TRAB"]);
                    if (totrow == null)
                    {
                        totrow = cltFolhaTot.NewRow();
                        totrow["TRAB"] = odatarow["TRAB"];

                        DataRow ocadrow = cltcad.Rows.Find(odatarow["TRAB"]);
                        if (ocadrow == null)
                            continue;
                        if (((Convert.ToDateTime(ocadrow["ADMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["ADMI"]) >= data2))
                            continue;

                        if (((Convert.ToDateTime(ocadrow["DEMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["DEMI"]) <= data2))
                            if (Convert.ToDateTime(ocadrow["DEMI"]).ToString("yyyyMM") == data2.ToString("yyyyMM"))
                                continue;

                        cltFolhaTot.Rows.Add(totrow);
                        totrow["Mensalista"] = ocadrow["Mensalista"];
                        totrow["Nome"] = ocadrow["Nomecad"];
                        totrow["SetorCad"] = ocadrow["Setor"];

                        System.Nullable<decimal> totadiant =
                         (from linha in cltadiant.AsEnumerable()
                          where (linha.Field<string>("TRAB") == Convert.ToString(totrow["TRAB"]))
                          select linha.Field<decimal>("ADIANT")).Sum();


                        totrow["ADIANT"] = Convert.ToSingle(totadiant);

                        totrow["CBO"] = Convert.ToString(ocadrow["CBO"]);
                        totrow["Categoria"] = Convert.ToString(ocadrow["Categoria"]);
                        totrow["ADMI"] = ocadrow["ADMI"];
                        totrow["DEMI"] = ocadrow["DEMI"];
                        totrow["CONTABCO"] = ocadrow["CONTA1"];
                        totrow["AGENCIA"] = ocadrow["AGENCIA1"];
                        totrow["BANCO"] = ocadrow["BANCO1"];
                        if (Convert.IsDBNull(ocadrow["BCOAGCC"]))
                            totrow["OBSBANCO"] = "";
                        else
                            totrow["OBSBANCO"] = ocadrow["BCOAGCC"];


                        totrow["CPF"] = ocadrow["CPF"];
                        totrow["END"] = ocadrow["END_RUA"];
                        totrow["CIDADE"] = ocadrow["END_CID"];
                        totrow["CEP"] = ocadrow["END_CEP"];
                        totrow["UF"] = ocadrow["END_UF"];
                        totrow["ISIND"] = Convert.ToSingle(0);




                         if (odatarow["PONTO"].ToString().Trim() != "")
                            totrow["PONTO"] = odatarow["PONTO"]; 
                        totrow["SALARIO"] = Convert.ToSingle(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToSingle(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToSingle(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToSingle(odatarow["SALFAM"]);
                        // lembrar do salario educa
                        totrow["EDUC"] = Convert.ToSingle(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToSingle(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToSingle(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToSingle(odatarow["DECIMO"]) + Convert.ToSingle(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToSingle(odatarow["FERIAS"]) + Convert.ToSingle(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToSingle(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToSingle(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToSingle(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToSingle(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToSingle(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToSingle(odatarow["VLR_HXS"]);
                        totrow["SALLIQ"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["SALFAM"]) - Convert.ToSingle(totrow["INSS"]) - Convert.ToSingle(totrow["ADIANT"]) - Convert.ToSingle(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["VLR_HXN"]);

                    }
                    else
                    {
                        if (odatarow["PONTO"].ToString().Trim() != "")
                            totrow["PONTO"] = odatarow["PONTO"]; 
                  
                        totrow["SALARIO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToSingle(totrow["INSS"]) + Convert.ToSingle(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToSingle(totrow["IRFONTE"]) + Convert.ToSingle(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToSingle(totrow["SALFAM"]) + Convert.ToSingle(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToSingle(totrow["EDUC"]) + Convert.ToSingle(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToSingle(totrow["TERC"]) + Convert.ToSingle(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToSingle(totrow["FGTS"]) + Convert.ToSingle(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToSingle(totrow["DECIMO"]) + Convert.ToSingle(odatarow["DECIMO"]) + Convert.ToSingle(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToSingle(totrow["FERIAS"]) + Convert.ToSingle(odatarow["FERIAS"]) + Convert.ToSingle(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToSingle(totrow["VLR_HXA"]) + Convert.ToSingle(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToSingle(totrow["HX"]) + Convert.ToSingle(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToSingle(totrow["HXA"]) + Convert.ToSingle(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToSingle(totrow["HXN"]) + Convert.ToSingle(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(odatarow["VLR_HXS"]);

                        totrow["SALLIQ"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["SALFAM"]) - Convert.ToSingle(totrow["INSS"]) - Convert.ToSingle(totrow["ADIANT"]) - Convert.ToSingle(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["VLR_HXN"]);
                    }
                }

                result.Tables["CLTFOLHATOT"].AcceptChanges();
                for (int i = 0; i < result.Tables["CLTFOLHATOT"].Rows.Count; i++)
                {
                    if (Convert.ToSingle(result.Tables["CLTFOLHATOT"].Rows[i]["SALLIQ"]) < 0.00)
                    {
                        result.Tables["CLTFOLHATOT"].Rows[i].Delete();
                    }

                }
                result.Tables["CLTFOLHATOT"].AcceptChanges();
                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }

        static public DataSet Get_CltAdiant(DataTable ocltadiant, DateTime data1, DateTime data2, List<LinhaSolucao> oLista, DataTable cltcad)
        {

            string path;//, strclttrab;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("TRABALHO");

            try
            {
                DataSet odataset = new DataSet();//TDataControlTrabalho.Get_CltCadastroFolha(data1, data2, oLista);
                if (cltcad == null)
                {
                    odataset = TDataControlTrabalho.Get_CltCadastroFolha(data1, data2, oLista);
                    cltcad = odataset.Tables[0];
                }

               // odataset.Tables.Add(ocltadiant);

                //DataSet odataset = new DataSet();
                //oda

               DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = cltcad.Columns["CODCAD"];     // odataset.Tables["CLTCAD"].Columns["CODCAD"];
                cltcad.PrimaryKey = PrimaryKeyColumns;


                DataTable cltFolhaTot = ocltadiant.Clone();
                cltFolhaTot.TableName = "CLTFOLHATOT";

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "NOME", 45, false));
               /* cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ADIANT", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALLIQ", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SBRUTO", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "HE50", 0, false));
                * 
                */
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ISIND", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CONTABCO", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "AGENCIA", 8, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "BANCO", 3, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "OBSBANCO", 21, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CPF", 14, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "END", 50, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CIDADE", 25, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CEP", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "UF", 2, false));


                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CBO", 20, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CATEGORIA", 30, false));
                
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "ADMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "DEMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "MENSALISTA", 1, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "SETORCAD", 2, false));

                result = new DataSet();
                result.Tables.Add(cltFolhaTot);

                PrimaryKeyColumns[0] = result.Tables["CLTFOLHATOT"].Columns["TRAB"];
                result.Tables["CLTFOLHATOT"].PrimaryKey = PrimaryKeyColumns;

                DataRow odatarow;

                for (int i = 0; i < ocltadiant.Rows.Count; i++)
                {
                    odatarow = ocltadiant.Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                    DataRow totrow = cltFolhaTot.Rows.Find(odatarow["TRAB"]);
                    if (totrow == null)
                    {
                        totrow = cltFolhaTot.NewRow();
                        totrow["TRAB"] = odatarow["TRAB"];

                        DataRow ocadrow = cltcad.Rows.Find(odatarow["TRAB"]);
                        if (ocadrow == null)
                            continue;
                        if (((Convert.ToDateTime(ocadrow["ADMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["ADMI"]) >= data2))
                            continue;

                        if (((Convert.ToDateTime(ocadrow["DEMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["DEMI"]) <= data2))
                            if (Convert.ToDateTime(ocadrow["DEMI"]).ToString("yyyyMM") == data2.ToString("yyyyMM"))
                                continue;

                        cltFolhaTot.Rows.Add(totrow);
                        totrow["Mensalista"] = ocadrow["Mensalista"];
                        totrow["Nome"] = ocadrow["Nomecad"];
                        totrow["SetorCad"] = ocadrow["Setor"];

                      
                        totrow["CBO"] = Convert.ToString(ocadrow["CBO"]);
                        totrow["Categoria"] = Convert.ToString(ocadrow["Categoria"]);
                        totrow["ADMI"] = ocadrow["ADMI"];
                        totrow["DEMI"] = ocadrow["DEMI"];
                        totrow["CONTABCO"] = ocadrow["CONTA1"];
                        totrow["AGENCIA"] = ocadrow["AGENCIA1"];
                        totrow["BANCO"] = ocadrow["BANCO1"];
                        if (Convert.IsDBNull(ocadrow["BCOAGCC"]))
                            totrow["OBSBANCO"] = "";
                        else
                            totrow["OBSBANCO"] = ocadrow["BCOAGCC"];


                        totrow["CPF"] = ocadrow["CPF"];
                        totrow["END"] = ocadrow["END_RUA"];
                        totrow["CIDADE"] = ocadrow["END_CID"];
                        totrow["CEP"] = ocadrow["END_CEP"];
                        totrow["UF"] = ocadrow["END_UF"];

                        if (odatarow["PONTO"].ToString().Trim() != "")
                            totrow["PONTO"] = odatarow["PONTO"];
                      
                        totrow["ADIANT"] = Convert.ToSingle(odatarow["ADIANT"]);
                      /*  totrow["INSS"] = Convert.ToSingle(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToSingle(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToSingle(odatarow["SALFAM"]);
                        // lembrar do salario educa
                        totrow["EDUC"] = Convert.ToSingle(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToSingle(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToSingle(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToSingle(odatarow["DECIMO"]) + Convert.ToSingle(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToSingle(odatarow["FERIAS"]) + Convert.ToSingle(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToSingle(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToSingle(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToSingle(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToSingle(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToSingle(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToSingle(odatarow["VLR_HXS"]);
                        totrow["SALLIQ"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["SALFAM"]) - Convert.ToSingle(totrow["INSS"]) - Convert.ToSingle(totrow["ADIANT"]) - Convert.ToSingle(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["VLR_HXN"]);
                        */
                    }
                    else
                    {
                        if (odatarow["PONTO"].ToString().Trim() != "")
                            totrow["PONTO"] = odatarow["PONTO"];

                        totrow["ADIANT"] = Convert.ToSingle(totrow["ADIANT"]) + Convert.ToSingle(odatarow["ADIANT"]);
                     /*   totrow["INSS"] = Convert.ToSingle(totrow["INSS"]) + Convert.ToSingle(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToSingle(totrow["IRFONTE"]) + Convert.ToSingle(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToSingle(totrow["SALFAM"]) + Convert.ToSingle(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToSingle(totrow["EDUC"]) + Convert.ToSingle(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToSingle(totrow["TERC"]) + Convert.ToSingle(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToSingle(totrow["FGTS"]) + Convert.ToSingle(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToSingle(totrow["DECIMO"]) + Convert.ToSingle(odatarow["DECIMO"]) + Convert.ToSingle(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToSingle(totrow["FERIAS"]) + Convert.ToSingle(odatarow["FERIAS"]) + Convert.ToSingle(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToSingle(totrow["VLR_HXA"]) + Convert.ToSingle(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToSingle(totrow["HX"]) + Convert.ToSingle(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToSingle(totrow["HXA"]) + Convert.ToSingle(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToSingle(totrow["HXN"]) + Convert.ToSingle(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(odatarow["VLR_HXS"]);

                        totrow["SALLIQ"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["SALFAM"]) - Convert.ToSingle(totrow["INSS"]) - Convert.ToSingle(totrow["ADIANT"]) - Convert.ToSingle(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["VLR_HXN"]);*/
                    }
                }

                result.Tables["CLTFOLHATOT"].AcceptChanges();
                for (int i = 0; i < result.Tables["CLTFOLHATOT"].Rows.Count; i++)
                {
                    if (Convert.ToSingle(result.Tables["CLTFOLHATOT"].Rows[i]["ADIANT"]) < 0.00)
                    {
                        result.Tables["CLTFOLHATOT"].Rows[i].Delete();
                    }

                }
                result.Tables["CLTFOLHATOT"].AcceptChanges();
                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }



        #endregion;

        static public DataSet Get_CltCad_CalcFolha(DateTime data1, DateTime data2, List<LinhaSolucao> oLista,DataSet dstrabalhista)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            DataSet result = null;
            string criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                        " ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                         " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ) ";

            stroledb = " SELECT CODCAD, NOMECAD, GLECAD, SETOR,  CARTIDENT, ADMI, DEMI, PRAZO, SALBASE, OPCAO, AVULSO, DEPEND, IRDEPEND, INSCPIS, CTA_FGTS," +
                         "  MENSALISTA, BANCO1, CONTA1, AGENCIA1, BCOAGCC, BANCO_OK, CPF, NUMERO, " +
                         " SEXO, NASC, COD_ADMI, CARTTRAB, SERIE,ULT_ATUAL, CBO, CATEGORIA, VLRFGTS, DTAFGTS, TIPODEMI, AVISO, SALRESC,MAE, PAI, CONJUGE, END_RUA, END_CID, END_UF, END_CEP, NCIDADE, NUF, " + 
                         // " EMICID, EMICTRAB, EMIPIS, ESTCIVIL, COR, TITELEITOR, RESERV, RESERV_CAT, TPSANGUE, DEFIC, TPDEFIC, APRENDIZ, " +
                         " TPMOV, DTAAVISO " +
                       "FROM " + path + "CLTCAD  WHERE " + criterio_admi_demi;
            //campos fora:
            
            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            stroledb += " ORDER by CLTCAD.codcad";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTCAD");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                DataSet dsDependentes = TDataControlTrabalho.Get_CltDependentes(data1, data2, null);
                //
                //
                //dsDependentes.Tables["CLTDEPENDENTES"].PrimaryKey = new DataColumn[1] { dsDependentes.Tables["CLTDEPENDENTES"].Columns["CODCAD"] };
               // DataSet dsTabelasTrabalhistas = TDataControlTrabalho.TabelasTrabalhistas();
                DataSet dsReajustes = TDataControlTrabalho.Get_CltReajuste(data1, data2, null, criterio_admi_demi);
                odataset.Tables[0].Columns.Add("SALARIOREAL", Type.GetType("System.Decimal"));
                // odataset.Tables[0].Columns.Add("SALLIQUIDO", Type.GetType("System.Decimal"));
                //odataset.Tables[0].Columns.Add("INSS", Type.GetType("System.Decimal"));

                DataSet dsFerias = TDataControlTrabalho.Get_CltFerias(data1, data2, null, criterio_admi_demi);
                //odataset.Tables[0].Columns.Add("FERIASDATA_PG", Type.GetType("System.DateTime"));
                /// odataset.Tables[0].Columns.Add("AQUIS_FIM", Type.GetType("System.DateTime"));
                // odataset.Tables[0].Columns.Add("AQUIS_INI", Type.GetType("System.DateTime"));
                odataset.Tables[0].Columns.Add("GOZO_FIM", Type.GetType("System.DateTime"));
                odataset.Tables[0].Columns.Add("GOZO_INI", Type.GetType("System.DateTime"));

                // odataset.Tables[0].Columns.Add("FERIASVENCIDAS", Type.GetType("System.Int16"));
                // odataset.Tables[0].Columns.Add("FERIASPROP", Type.GetType("System.Int16"));


                DataTable tabDeb = odataset.Tables[0].Clone();

                Decimal salmin = TDataControlTrabalho.CalcSalarioMinimo(data2, dstrabalhista.Tables["TABSAL"]);


                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;
                   
                    odatarow["DEPEND"] = odatarow["DEPEND"] = TDataControlTrabalho.DependentesClt(Convert.ToString(odatarow["CODCAD"]), data2, dsDependentes.Tables["CLTDEPENDENTES"]);
                    DataRow[] odatarows = dsDependentes.Tables["IRDEPENDENTES"].Select("CODCAD = '" + Convert.ToString(odatarow["CODCAD"]) + "'", "DATA DESC");
                    if (odatarows.Length > 0)
                    {
                        odatarow["IRDEPEND"] = Convert.ToInt16(odatarows[0]["NDEPEND"]);
                    }
                    odatarow.BeginEdit();
              
                    odatarow["SALARIOREAL"] = TDataControlTrabalho.CalcSalBase(Convert.ToString(odatarow["CODCAD"]), Convert.ToDateTime(odatarow["ADMI"]),
                        Convert.ToDecimal(odatarow["SALBASE"]),salmin, data2,   dsReajustes.Tables[0]);
                    

                    TDataControlTrabalho.informeferias oferias = TDataControlTrabalho.UltimaFeriasGozadas(Convert.ToString(odatarow["CODCAD"]), data2, dsFerias.Tables[0]);
                    if (oferias != null)
                    {
                        // oferias.CalculeFeriasVencidas(data2);
                        // odatarow["FERIASDATA_PG"] = Convert.ToDateTime(oferias.Data_pg);
                        // odatarow["AQUIS_FIM"] = Convert.ToDateTime(oferias.Aquis_fim);
                        // odatarow["AQUIS_INI"] = Convert.ToDateTime(oferias.Aquis_ini);
                        odatarow["GOZO_INI"] = Convert.ToDateTime(oferias.Gozo_ini);
                        odatarow["GOZO_FIM"] = Convert.ToDateTime(oferias.Gozo_fim);
                        // odatarow["FERIASVENCIDAS"] = Convert.ToInt16(oferias.Vencidas);
                        // odatarow["FERIASPROP"] = Convert.ToInt16(oferias.Proporcionais);
                    }

                    odatarow.EndEdit();
                    odatarow.AcceptChanges();
                    tabDeb.Rows.Add(odatarow.ItemArray);
                }
                tabDeb.PrimaryKey = new DataColumn[1] { tabDeb.Columns["CODCAD"] };
                result = new DataSet();
                result.Tables.Add(tabDeb);
                DataTable dtFerias = dsFerias.Tables[0].Copy();
                dtFerias.TableName = dsFerias.Tables[0].TableName;
                result.Tables.Add(dtFerias);
                return result;
            }
            catch (Exception)
            { throw; }
        }


        static public DataSet Get_CltSalbase_ISind(DateTime data1, DateTime data2, List<LinhaSolucao> oLista, DataSet dstrabalhista)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            DataSet result = null;
            string criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                        " ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                         " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ) ";

            stroledb = " SELECT CODCAD, NOMECAD, GLECAD, SETOR,  CARTIDENT, ADMI, DEMI, PRAZO, SALBASE, OPCAO, AVULSO, DEPEND, IRDEPEND, INSCPIS, CTA_FGTS," +
                         "  MENSALISTA, BANCO1, CONTA1, AGENCIA1, BCOAGCC, BANCO_OK, CPF, NUMERO, " +
                         " SEXO, NASC, COD_ADMI, CARTTRAB, SERIE,ULT_ATUAL, CBO, CATEGORIA, VLRFGTS, DTAFGTS, TIPODEMI, AVISO, SALRESC,MAE, PAI, CONJUGE, END_RUA, END_CID, END_UF, END_CEP, NCIDADE, NUF, " +
                // " EMICID, EMICTRAB, EMIPIS, ESTCIVIL, COR, TITELEITOR, RESERV, RESERV_CAT, TPSANGUE, DEFIC, TPDEFIC, APRENDIZ, " +
                         " TPMOV, DTAAVISO " +
                       "FROM " + path + "CLTCAD  WHERE " + criterio_admi_demi;
            //campos fora:

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            stroledb += " ORDER by CLTCAD.codcad";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTCAD");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
               
                DataSet dsReajustes = TDataControlTrabalho.Get_CltReajuste(data1, data2, null, criterio_admi_demi);
                odataset.Tables[0].Columns.Add("SALARIOREAL", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("ISIND", Type.GetType("System.Decimal"));
            
                DataTable tabDeb = odataset.Tables[0].Clone();

                Decimal salmin = TDataControlTrabalho.CalcSalarioMinimo(data2, dstrabalhista.Tables["TABSAL"]);


                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                    odatarow.BeginEdit();

                    odatarow["SALARIOREAL"] = TDataControlTrabalho.CalcSalBase(Convert.ToString(odatarow["CODCAD"]), Convert.ToDateTime(odatarow["ADMI"]),
                        Convert.ToDecimal(odatarow["SALBASE"]), salmin, data2, dsReajustes.Tables[0]);
                    odatarow["ISIND"] = Math.Round(Convert.ToDecimal(odatarow["SALARIOREAL"]) / 30,2);
                       // ptRound(Salbase/30,2);
                    odatarow.EndEdit();
                    odatarow.AcceptChanges();
                    tabDeb.Rows.Add(odatarow.ItemArray);
                }
                tabDeb.PrimaryKey = new DataColumn[1] { tabDeb.Columns["CODCAD"] };
                result = new DataSet();
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            { throw; }
        }



        static public DataSet Get_TotalCltAdiant(DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            //DataSet result = null;
            stroledb = "SELECT data, sum(adiant) totadiant  FROM " + path + "CLTADIAN"
            + " WHERE (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") AND CTOD(" +
                    TDataControlReduzido.FormatDataGravar(data2) + ") ) ";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }

            stroledb += " GROUP by DATA";

            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTADIAN");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;
            }

        }

        static public DataSet Get_TotalCltFolha(DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
           // DataSet result = null;
            stroledb = "SELECT CLTFOLHA.data, sum(CLTFOLHA.SALARIO) totsalario  FROM " + path + "CLTFOLHA," + path + "CLTCAD"
            + " WHERE (CLTFOLHA.DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") AND CTOD(" +
                    TDataControlReduzido.FormatDataGravar(data2) + ") ) AND ( (CLTFOLHA.TRAB = CLTCAD.CODCAD) AND "+
                       "( (CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + 
                       ") ) OR (CLTCAD.DEMI > CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ) ) ";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }

            stroledb += " GROUP by DATA";

            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTFOLHA");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;
            }

        }




    }

    public class FolhaTeste
    {
        static public DataSet Get_CltFolhaComparada(DataTable ocltfolha, DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {


            string  path;//, strclttrab;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("TRABALHO");

            try
            {
                DataSet odataset = TDataControlTrabalho.Get_CltCadastroFolha(data1, data2, oLista);
                odataset.Tables.Add(ocltfolha);
               
                DataSet dsAdiant = TDataControlTrabalho.Get_CltAdiant(data1, oLista);

                DataTable cltAdiant = dsAdiant.Tables[0];
                cltAdiant.TableName = "CLTADIAN";
               
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = odataset.Tables["CLTCAD"].Columns["CODCAD"];
                odataset.Tables["CLTCAD"].PrimaryKey = PrimaryKeyColumns;


                DataTable cltFolhaTot = odataset.Tables["CLTFOLHA"].Clone();
                cltFolhaTot.TableName = "CLTFOLHATOT";


                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "NOME", 45, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ADIANT", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALLIQ", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SBRUTO", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "HE50", 0, false));

               /* cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CONTABCO", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "AGENCIA", 8, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "BANCO", 3, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "OBSBANCO", 21, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CPF", 14, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "END", 50, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CIDADE", 25, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CEP", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "UF", 2, false));


                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CBO", 20, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CATEGORIA", 30, false));
               */
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "ADMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "DEMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "MENSALISTA", 1, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "SETORCAD", 2, false));

                result = new DataSet();
                result.Tables.Add(cltFolhaTot);


                PrimaryKeyColumns[0] = result.Tables["CLTFOLHATOT"].Columns["TRAB"];
                result.Tables["CLTFOLHATOT"].PrimaryKey = PrimaryKeyColumns;


                DataRow odatarow;

                for (int i = 0; i < odataset.Tables["CLTFOLHA"].Rows.Count; i++)
                {
                    odatarow = odataset.Tables["CLTFOLHA"].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                    DataRow totrow = cltFolhaTot.Rows.Find(odatarow["TRAB"]);
                    if (totrow == null)
                    {
                        totrow = cltFolhaTot.NewRow();
                        totrow["TRAB"] = odatarow["TRAB"];

                        DataRow ocadrow = odataset.Tables["CLTCAD"].Rows.Find(odatarow["TRAB"]);
                        if (ocadrow == null)
                            continue;
                        if (((Convert.ToDateTime(ocadrow["ADMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["ADMI"]) >= data2))
                            continue;

                        if (((Convert.ToDateTime(ocadrow["DEMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["DEMI"]) <= data2))
                            if (Convert.ToDateTime(ocadrow["DEMI"]).ToString("yyyyMM") == data2.ToString("yyyyMM"))
                                continue;

                        cltFolhaTot.Rows.Add(totrow);
                        totrow["Mensalista"] = ocadrow["Mensalista"];
                        totrow["Nome"] = ocadrow["Nomecad"];
                        totrow["SetorCad"] = ocadrow["Setor"];
                        System.Nullable<decimal> totadiant =
                         (from linha in cltAdiant.AsEnumerable()
                          where (linha.Field<string>("TRAB") == Convert.ToString(totrow["TRAB"]))
                          select linha.Field<decimal>("ADIANT")).Sum();


                        totrow["ADIANT"] = Convert.ToDecimal(totadiant);
                       
                        totrow["SALARIO"] = Convert.ToDecimal(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToDecimal(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToDecimal(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToDecimal(odatarow["SALFAM"]);
                        // lembrar do salario educa
                        totrow["EDUC"] = Convert.ToDecimal(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToDecimal(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToDecimal(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToDecimal(odatarow["DECIMO"]) + Convert.ToDecimal(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToDecimal(odatarow["FERIAS"]) + Convert.ToDecimal(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToDecimal(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToDecimal(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToDecimal(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToDecimal(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToDecimal(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToDecimal(odatarow["VLR_HXS"]);
                        //totrow["SALLIQ"] = 0.00;
                        //totrow["SBRUTO"] = 0.00;
                        //totrow["HE50"] = 0.00;
                        totrow["SALLIQ"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["SALFAM"]) - Convert.ToDecimal(totrow["INSS"]) - Convert.ToDecimal(totrow["ADIANT"]) - Convert.ToDecimal(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["VLR_HXN"]);

                    }
                    else
                    {
                        totrow["SALARIO"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToDecimal(totrow["INSS"]) + Convert.ToDecimal(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToDecimal(totrow["IRFONTE"]) + Convert.ToDecimal(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToDecimal(totrow["SALFAM"]) + Convert.ToDecimal(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToDecimal(totrow["EDUC"]) + Convert.ToDecimal(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToDecimal(totrow["TERC"]) + Convert.ToDecimal(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToDecimal(totrow["FGTS"]) + Convert.ToDecimal(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToDecimal(totrow["DECIMO"]) + Convert.ToDecimal(odatarow["DECIMO"]) + Convert.ToDecimal(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToDecimal(totrow["FERIAS"]) + Convert.ToDecimal(odatarow["FERIAS"]) + Convert.ToDecimal(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToDecimal(totrow["VLR_HXA"]) + Convert.ToDecimal(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToDecimal(totrow["HX"]) + Convert.ToDecimal(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToDecimal(totrow["HXA"]) + Convert.ToDecimal(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToDecimal(totrow["HXN"]) + Convert.ToDecimal(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(odatarow["VLR_HXS"]);

                        totrow["SALLIQ"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["SALFAM"]) - Convert.ToDecimal(totrow["INSS"]) - Convert.ToDecimal(totrow["ADIANT"]) - Convert.ToDecimal(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["VLR_HXN"]);
                    }
                }

                result.Tables["CLTFOLHATOT"].AcceptChanges();
                for (int i = 0; i < result.Tables["CLTFOLHATOT"].Rows.Count; i++)
                {
                    if (Convert.ToSingle(result.Tables["CLTFOLHATOT"].Rows[i]["SALLIQ"]) < 0.00)
                    {
                        result.Tables["CLTFOLHATOT"].Rows[i].Delete();
                    }

                }
                result.Tables["CLTFOLHATOT"].AcceptChanges();
               


                DataSet dsFolhaVelha = Get_CltFolhaVelha(data1, data2, oLista,cltAdiant,odataset.Tables["CLTCAD"]);

                DataTable cltFolhaDif = result.Tables["CLTFOLHATOT"].Copy();
                cltFolhaDif.TableName = "CLTFOLHADIF";

                result.Tables.Add(dsFolhaVelha.Tables["CLTFOLHATOTVELHA"].Copy());
                result.Tables.Add(cltFolhaDif);
                PrimaryKeyColumns[0] = result.Tables["CLTFOLHADIF"].Columns["TRAB"];
                result.Tables["CLTFOLHADIF"].PrimaryKey = PrimaryKeyColumns;
                
                for (int i = 0; i < dsFolhaVelha.Tables["CLTFOLHATOTVELHA"].Rows.Count; i++)
                {
                    odatarow = dsFolhaVelha.Tables["CLTFOLHATOTVELHA"].Rows[i];
                 
                    DataRow totrow = cltFolhaDif.Rows.Find(odatarow["TRAB"]);
                    if (totrow == null)
                    {
                        MessageBox.Show("Erro " + odatarow["TRAB"].ToString());
                    }
                    else
                    {
                        totrow.BeginEdit();
                        totrow["SALARIO"] = Convert.ToDecimal(totrow["SALARIO"]) - Convert.ToDecimal(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToDecimal(totrow["INSS"]) - Convert.ToDecimal(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToDecimal(totrow["IRFONTE"]) - Convert.ToDecimal(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToDecimal(totrow["SALFAM"]) - Convert.ToDecimal(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToDecimal(totrow["EDUC"]) - Convert.ToDecimal(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToDecimal(totrow["TERC"]) - Convert.ToDecimal(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToDecimal(totrow["FGTS"]) - Convert.ToDecimal(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToDecimal(totrow["DECIMO"]) - Convert.ToDecimal(odatarow["DECIMO"]);
                        totrow["FERIAS"] = Convert.ToDecimal(totrow["FERIAS"]) - Convert.ToDecimal(odatarow["FERIAS"]);// -Convert.ToDecimal(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToDecimal(totrow["VLR_HXA"]) - Convert.ToDecimal(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToDecimal(totrow["HX"]) - Convert.ToDecimal(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToDecimal(totrow["VLR_HXN"]) - Convert.ToDecimal(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToDecimal(totrow["HXA"]) - Convert.ToDecimal(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToDecimal(totrow["HXN"]) - Convert.ToDecimal(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToDecimal(totrow["VLR_HXS"]) - Convert.ToDecimal(odatarow["VLR_HXS"]);
                        totrow["SALLIQ"] = Convert.ToDecimal(totrow["SALLIQ"]) - Convert.ToDecimal(totrow["SALLIQ"]);
                        totrow["SBRUTO"] = Convert.ToDecimal(totrow["SBRUTO"]) - Convert.ToDecimal(totrow["SBRUTO"]);
                        totrow.EndEdit();
                    }
                }

                cltFolhaDif.AcceptChanges();



                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }
        static public DataSet Get_CltFolhaVelha(DateTime data1, DateTime data2, List<LinhaSolucao> oLista,DataTable cltAdiant,DataTable cltCad)
        {


            OleDbCommand oledbcomm;
            string stroledb, path;//, strclttrab;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            stroledb = "SELECT * FROM " + path + "CLTFOLHA"
              + " WHERE (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ";
            // 


            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }

            stroledb += " ORDER by trab";


            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTFOLHA");

                DataSet odataset = new DataSet();// Get_CltCadastroFolha(data1, data2, oLista);       //new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

           
                DataTable cltFolhaTot = odataset.Tables["CLTFOLHA"].Clone();
                cltFolhaTot.TableName = "CLTFOLHATOTVELHA";


                 cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "NOME", 45, false));
                 cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ADIANT", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALLIQ", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SBRUTO", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "HE50", 0, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "ADMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "DEMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "MENSALISTA", 1, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "SETORCAD", 2, false));
                
                result = new DataSet();
                result.Tables.Add(cltFolhaTot);

                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["CLTFOLHATOTVELHA"].Columns["TRAB"];
                result.Tables["CLTFOLHATOTVELHA"].PrimaryKey = PrimaryKeyColumns;




                for (int i = 0; i < odataset.Tables["CLTFOLHA"].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables["CLTFOLHA"].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                    DataRow totrow = cltFolhaTot.Rows.Find(odatarow["TRAB"]);
                    if (totrow == null)
                    {
                        totrow = cltFolhaTot.NewRow();
                        totrow["TRAB"] = odatarow["TRAB"];
                        DataRow ocadrow = cltCad.Rows.Find(odatarow["TRAB"]);
                        if (ocadrow == null)
                            continue;
                        if (((Convert.ToDateTime(ocadrow["ADMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["ADMI"]) >= data2))
                            continue;

                        if (((Convert.ToDateTime(ocadrow["DEMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["DEMI"]) <= data2))
                            if (Convert.ToDateTime(ocadrow["DEMI"]).ToString("yyyyMM") == data2.ToString("yyyyMM"))
                                continue;

                        totrow["Mensalista"] = ocadrow["Mensalista"];
                        totrow["Nome"] = ocadrow["Nomecad"];
                        totrow["SetorCad"] = ocadrow["Setor"];
                        System.Nullable<decimal> totadiant =
                         (from linha in cltAdiant.AsEnumerable()
                          where (linha.Field<string>("TRAB") == Convert.ToString(totrow["TRAB"]))
                          select linha.Field<decimal>("ADIANT")).Sum();


                        totrow["ADIANT"] = Convert.ToDecimal(totadiant);
                       
                  
                        cltFolhaTot.Rows.Add(totrow);

                        totrow["SALARIO"] = Convert.ToDecimal(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToDecimal(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToDecimal(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToDecimal(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToDecimal(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToDecimal(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToDecimal(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToDecimal(odatarow["DECIMO"]) + Convert.ToDecimal(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToDecimal(odatarow["FERIAS"]) + Convert.ToDecimal(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToDecimal(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToDecimal(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToDecimal(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToDecimal(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToDecimal(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToDecimal(odatarow["VLR_HXS"]);
                       
                        totrow["SALLIQ"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["SALFAM"]) - Convert.ToDecimal(totrow["INSS"]) - Convert.ToDecimal(totrow["ADIANT"]) - Convert.ToDecimal(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["VLR_HXN"]);
                    }
                    else
                    {
                        totrow["SALARIO"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToDecimal(totrow["INSS"]) + Convert.ToDecimal(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToDecimal(totrow["IRFONTE"]) + Convert.ToDecimal(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToDecimal(totrow["SALFAM"]) + Convert.ToDecimal(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToDecimal(totrow["EDUC"]) + Convert.ToDecimal(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToDecimal(totrow["TERC"]) + Convert.ToDecimal(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToDecimal(totrow["FGTS"]) + Convert.ToDecimal(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToDecimal(totrow["DECIMO"]) + Convert.ToDecimal(odatarow["DECIMO"]) + Convert.ToDecimal(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToDecimal(totrow["FERIAS"]) + Convert.ToDecimal(odatarow["FERIAS"]) + Convert.ToDecimal(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToDecimal(totrow["VLR_HXA"]) + Convert.ToDecimal(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToDecimal(totrow["HX"]) + Convert.ToDecimal(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToDecimal(totrow["HXA"]) + Convert.ToDecimal(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToDecimal(totrow["HXN"]) + Convert.ToDecimal(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(odatarow["VLR_HXS"]);

                        totrow["SALLIQ"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["SALFAM"]) - Convert.ToDecimal(totrow["INSS"]) - Convert.ToDecimal(totrow["ADIANT"]) - Convert.ToDecimal(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToDecimal(totrow["SALARIO"]) + Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXN"]) + Convert.ToDecimal(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToDecimal(totrow["VLR_HXA"])
                            + Convert.ToDecimal(totrow["VLR_HXS"]) + Convert.ToDecimal(totrow["VLR_HXN"]);
                    }
                }
                result.Tables["CLTFOLHATOTVELHA"].AcceptChanges();
                for (int i = 0; i < result.Tables["CLTFOLHATOTVELHA"].Rows.Count; i++)
                {
                    if (Convert.ToSingle(result.Tables["CLTFOLHATOTVELHA"].Rows[i]["SALLIQ"]) < 0.00)
                    {
                        result.Tables["CLTFOLHATOTVELHA"].Rows[i].Delete();
                    }
                }
                result.Tables["CLTFOLHATOTVELHA"].AcceptChanges();
                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }

      


    
    }


}
