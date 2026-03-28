using System;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using System.DirectoryServices;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
//using FirebirdSql.Data.Firebird;


namespace ClassConexao//ClassLibTrabalho//WFAEstatiscaTrabalho
{
    

    public class TConnectionPooling
    {
        
      
        public TConnectionPooling()
        {
             CurrentDiretorio = Directory.GetCurrentDirectory();
            CurrentDiretorio = CurrentDiretorio + "/";
            diretoriodoinfofile = "";
        }
      /*  public TConnectionPooling(string odiretoriodoinfofile)
        {
            CurrentDiretorio = Directory.GetCurrentDirectory();
            CurrentDiretorio = CurrentDiretorio + "/";
            diretoriodoinfofile = odiretoriodoinfofile;
        } 
      */

       private    
       OdbcConnection FOdbcConnection,FParadoxOdbcConnection,FExcelOdbcConnection;
      // FbConnection FFbConnection;
       SqlConnection FSqlConnection;
       OleDbConnection FOleDbConnection;


       public string CurrentDiretorio;
       public string diretoriodoinfofile;

        // dpwnload VFPOLEBD.1 em:
        // https://www.microsoft.com/en-us/download/details.aspx?id=14839
        void ActiveConnectionOleDb()
        {
            string ConnectionString;
            ConnectionString = "Provider=VFPOLEDB.1;Data Source=" + PathInfo;//
            FOleDbConnection = new OleDbConnection(ConnectionString);
            try
            {
                FOleDbConnection.Open();
            }
            catch (Exception E)
            {

                throw;
            }
          

            //
        }

       

        void ActiveConnectionParadox()
        {
           string ConnectionString;

      
           ConnectionString ="CollatingSequence=ASCII;SafeTransactions=0;MaxBufferSize=2048;MaxScanRows=8;DefaultDir=;"+
                   "ParadoxNetPath="+ParadoxNETPathInfo+";DriverId=538;"+
           "Threads=3;ParadoxUserName=admin;UserCommitSync=Yes;FIL=Paradox 5.X;Driver={Microsoft Paradox Driver (*.db )};PageTimeout=600;ParadoxNetStyle=4.x;UID=admin";
           FParadoxOdbcConnection = new OdbcConnection(ConnectionString);//ConnectionString);
           FParadoxOdbcConnection.Open();

        }


        void ActiveConnectionExcel()
        {
           string connectionstring;

           connectionstring = "Driver={Microsoft Excel Driver(*.xls)};DQB="+ExcelPathInfo;//c:\teste\planilha.xls
           FExcelOdbcConnection = new OdbcConnection(connectionstring);
           FExcelOdbcConnection.Open();

        }

      /*  void ActiveConnectionFb()
        {
           string confb, nome_servidor,bancoplantas;
           bancoplantas = FbBancoPlantasPathInfo;

           if (bancoplantas == "")  bancoplantas = "BANCOPLANTAS";
           nome_servidor = FireBirdPathInfo;
           if (nome_servidor == "")   nome_servidor = "Servidor";

            confb = "User=SYSDBA;Password=masterkey;Database="+bancoplantas+";DataSource="+nome_servidor+";Port=3050;Dialect=3;"+
                "Charset=WIN1252;Role=;Connection lifetime=0;Connection timeout=15;Pooling=True;Packet Size=8192;Server Type=0";

            FFbConnection =  new FbConnection(confb);

        }*/
        void ActiveConnectionSql()
        {
            string consql;
            consql = Properties.Settings.Default.LocalServer;   //em Preperties (está vazio)        
            FSqlConnection = new SqlConnection(consql);

        }

        public  string InformaPathInfo(string palavrachave)
        {
            {
                FileStream myfilestream;
                StreamReader mystreamreader;
                int indpath;
                // pesquisa no diretorio a existencia do arquivo info.txt
                string result = "";
                // myfilestream = new FileStream(diretoriodoinfofile+"InfoFile.txt", FileMode.Open,
                //     FileAccess.Read);
                myfilestream = new FileStream("InfoFile.txt", FileMode.Open,
                     FileAccess.Read);
                try
                {
                    mystreamreader = new StreamReader(myfilestream);
                    try
                    {
                        mystreamreader.BaseStream.Seek(0, SeekOrigin.Begin);
                        indpath = -1;
                        while ((mystreamreader.Peek() != -1) && (indpath == -1))
                        {
                            result = mystreamreader.ReadLine();
                            indpath = result.IndexOf(palavrachave, 0);
                            if (indpath != -1)
                                result = result.Substring(indpath + palavrachave.Length);
                        }

                    }
                    finally
                    {
                        mystreamreader.Close();
                    }
                }
                finally
                {
                    myfilestream.Close();
                }
                if ((result !="") &&  (result.Substring(result.Length - 2, 2) != "\\"))
                    result += "\\"; 
                return result;
            }
        }

       
       /* public static void CopiaArquivos(string arqent, string arqsai)
        {
            if (!File.Exists(arqent))
            {
                MessageBox.Show("Falta:" + arqent);
                return;
            }
            FileStream strent = new FileStream(arqent, FileMode.Open,FileAccess.Read);
            //BinaryReader r = new BinaryReader(arqent);
           
            // Write data to Test.data.
            try
            {
                FileStream strsai = new FileStream(arqsai, FileMode.OpenOrCreate,FileAccess.Write);
              //  BinaryWriter w = new BinaryWriter(strsai);
                try
                {
                    strent.Seek(0, SeekOrigin.Begin);
                    //strent.Read(
                   for (Int32 i = 0; i < strent.Length; i++)
                      strsai.WriteByte(Convert.ToByte(strent.ReadByte()));
                   
                }
                finally
                {
                    strsai.Close();
                }
            }
            finally
            {
                strent.Close();
            }

        
        }
       */
        public static string PegDiretorio(string arqentrada, string codigo)
        {
            if (!File.Exists(arqentrada))
                return "";
            FileStream myfilestream;
            StreamReader mystreamreader;
            int indpath;
            // pesquisa no diretorio a existencia do arquivo info.txt
            string result = "";
            myfilestream = new FileStream(arqentrada, FileMode.Open,
                  FileAccess.Read);
            try
            {
                mystreamreader = new StreamReader(myfilestream);
                try
                {
                    mystreamreader.BaseStream.Seek(0, SeekOrigin.Begin);
                    indpath = -1;
                    while ((mystreamreader.Peek() != -1) && (indpath == -1))
                    {
                        result = mystreamreader.ReadLine();
                        indpath = result.IndexOf(codigo, 0);
                        if (indpath != -1)
                            result = result.Substring(indpath + codigo.Length+1);
                    }

                }
                finally
                {
                    mystreamreader.Close();
                }
            }
            finally
            {
                myfilestream.Close();
            }
            if ((result != "") && (result.Substring(result.Length - 2, 2) != "\\"))
               result += "\\";
            if (!Directory.Exists(result))
            {
                MessageBox.Show("Diretorio Não Existe:" + result);
                result = "";
            }
            //else
           //     if ((result != "") && (result.Substring(result.Length - 2, 2) != "\\"))
            //        result += "\\";
            //*/
            return result;
        }
        /*
         * function PEGDiretorio(ArqEnt,CODIGO : String):string;
var
F: TextFile;
S: string;


begin
 result := '';


  if not FileExists(ArqEnt) then begin
     exit;
  end;



 // if OpenDialog1.Execute then            { Display Open dialog box }
 // begin

 try
    AssignFile(F, arqent); { File selected in dialog }
    Reset(F);
    while not SeekEof(F) do
    begin
      Readln(F, S);
      if pos(codigo,s) <> 0 then
         result := trim(copy(s,Length(codigo)+2,Length(s)));
    end;
    CloseFile(F);
  except
    CloseFile(F);
  end;
  if result <> '' then
  begin
     if copy(result,length(result),1) <> '\' then
        result := result+'\';
     if not directoryexists(result) then
     begin
        showmessage('Diretorio Invalido:'+result);
        result := '';
     end;

  end
end;

         */





        /* public OdbcConnection GetConnection ()
        {
            if (FOdbcConnection == null) 
                ActiveConnection();
            return FOdbcConnection;
        }*/
        public OleDbConnection GetConnectionOleDb()
          {
              if (FOleDbConnection == null)
                  ActiveConnectionOleDb();
              return FOleDbConnection;

          }

         //SqlConnection GetConnectionSQL () {} 
       public  OdbcConnection GetConnectionParadox () 
         {
             if (FParadoxOdbcConnection == null) 
                 ActiveConnectionParadox();
             return  FParadoxOdbcConnection;
         }

        public string PathInfo 
        {
            get 
            {
                string path = ListaCaminhos.GetPath("CONTAB");
                if (path == "") { MessageBox.Show("Path para Contab Não Informado"); return ""; }
                return path;
                return InformaPathInfo("PATH=");
               }
            //set {Get_PathInfo = value;}
        }
       
      public string ParadoxNETPathInfo 
        {
            get { return InformaPathInfo("PATHNETPARADOX="); }
        }
      public string ExcelPathInfo 
        {
            get { return InformaPathInfo("PATHEXCEL="); }
        }
      //public string ExcelNETPathInfo 
       // {
        //    get {return Get_ExcelNETPathInfo();}
        //}
      public StringCollection Computadores 
        {
            get
            {
               DirectoryEntry entry, outroentry;
               DirectoryEntries entries ;
               IEnumerator ie,ie2;
                
               StringCollection result = new StringCollection();
               entry = new DirectoryEntry("WinNT:");
               entries = entry.Children;
               ie = entries.GetEnumerator();
               if (ie.MoveNext() == true) 
               { 
                 outroentry = (DirectoryEntry) ie.Current;
                 outroentry.Children.SchemaFilter.Add("Computer");
                 ie2 = outroentry.Children.GetEnumerator();
                 while (ie2.MoveNext() == true)
                 {
                     entry = (DirectoryEntry) ie2.Current;
                   result.Add(entry.Name.ToUpper());
                 }

               }
               entry = new DirectoryEntry("IIS:");
               ie = entry.Children.GetEnumerator();
               while (ie.MoveNext() == true) 
               {

                   outroentry = (DirectoryEntry) ie.Current;
                  if (outroentry.SchemaClassName == "IIsComputer") 
                     result.Add(outroentry.Name.ToUpper());
               }
              
               
              return result;
            }
        }

    
    }
     
}
