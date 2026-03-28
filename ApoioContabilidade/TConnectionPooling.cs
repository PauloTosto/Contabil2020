using System;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using FirebirdSql.Data.Firebird;


namespace ApoioContabilidade
{
    

    class TConnectionPooling
    {
        
      
        public TConnectionPooling()
        {
             CurrentDiretorio = Directory.GetCurrentDirectory();
            CurrentDiretorio = CurrentDiretorio + "/"; 
        } 

       private    
       OdbcConnection FOdbcConnection,FParadoxOdbcConnection,FExcelOdbcConnection;
       FbConnection FFbConnection;
       SqlConnection FSqlConnection;
       OleDbConnection FOleDbConnection;


       string CurrentDiretorio;


 
        void ActiveConnection()
        {
            string ConnectionString;
           ConnectionString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB="+PathInfo+";Exclusive=No;Collate=Machine;NULL=NO;DELETED=Yes;BACKGROUNDFETCH=NO;";
           FOdbcConnection = new OdbcConnection(ConnectionString);
           FOdbcConnection.Open();
        }

        void ActiveConnectionOleDb()
        {
            string ConnectionString;
            ConnectionString = "Provider=VFPOLEDB.1;Data Source=" + PathInfo;//c:\novo\trabalho 
            FOleDbConnection = new OleDbConnection(ConnectionString);
            FOleDbConnection.Open();

            //
        }



        void ActiveConnectionParadox()
        {
           string ConnectionString;

           //string cc = "Driver={Microsoft Paradox Driver (*.db )}; DriverID=538; Fil=Paradox 5.X;DefaultDir=Z:\\novo\\campo; Dbq=Z:\\novo\\campo; CollatingSequence=ASCII"; 
                
              /*  
                "Provider=Microsoft.Jet.OLEDB.4.0;" + 
           "Data Source=c:\\novo\\campo\\;" +
           "Extended Properties=Paradox 5.x;"; 
            */

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

        void ActiveConnectionFb()
        {
           string confb, nome_servidor,bancoplantas;
           bancoplantas = FbBancoPlantasPathInfo;

           if (bancoplantas == "")  bancoplantas = "BANCOPLANTAS";
           nome_servidor = FireBirdPathInfo;
           if (nome_servidor == "")   nome_servidor = "Servidor";

            confb = "User=SYSDBA;Password=masterkey;Database="+bancoplantas+";DataSource="+nome_servidor+";Port=3050;Dialect=3;"+
                "Charset=WIN1252;Role=;Connection lifetime=0;Connection timeout=15;Pooling=True;Packet Size=8192;Server Type=0";

            FFbConnection =  new FbConnection(confb);

        }
        void ActiveConnectionSql()
        {
           // string consql;
            //consql = Properties.Settings.Default.LocalSqlConnection;           
           // FSqlConnection = new SqlConnection(consql);

        }

        string InformaPathInfo(string palavrachave)
        {
            {
                FileStream myfilestream;
                StreamReader mystreamreader;
                int indpath;
                // pesquisa no diretorio a existencia do arquivo info.txt
                string result = "";
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




  public
        
         OdbcConnection GetConnection ()
         {
             if (FOdbcConnection == null) 
                 ActiveConnection();
             return FOdbcConnection;
         }
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

        OdbcConnection GetConnectionExcel () 
        {
            if (FExcelOdbcConnection == null)
                ActiveConnectionExcel();
            return FExcelOdbcConnection;
        }

        FbConnection GetConnectionFb()
        {
            if (FFbConnection == null)
                ActiveConnectionFb();
            return FFbConnection;
        }
       public SqlConnection GetConnectionSql()
        {
            if (FSqlConnection == null)
                ActiveConnectionSql();
             return FSqlConnection;
        }
        public string PathInfo 
        {
            get 
            {
               return InformaPathInfo("PATH=");
               }
            //set {Get_PathInfo = value;}
        }
        public string ParadoxPathInfo 
        {
            get { return InformaPathInfo("PATHPARADOX="); }
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

      public string FireBirdPathInfo
      {
          get { return InformaPathInfo("SERVIDOR="); }
      }

      public string FbBancoPlantasPathInfo
      {
          get { return InformaPathInfo("BANCOPLANTAS="); }
      }


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
