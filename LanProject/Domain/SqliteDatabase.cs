using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LanProject.Domain
{
    public class SqliteDatabase
    {
        private string connString { get; set; }
        private SQLiteConnection source { get; set; }
        private Dictionary<string, string> initSchemas { get; set; }
        private readonly string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);//Method.Configuration.ReadSetting("dataPath");
        private readonly string foldername = Method.Configuration.ReadSetting("appfoldername");
        private readonly string databasename = Method.Configuration.ReadSetting("databasename");
        public void initConnectionstring()
        {
            connString = Path.Combine("URI=file:", programFilesPath, foldername, String.Format("{0}.sqlite", databasename));
            initSchemas = new Dictionary<string, string>();
            initSchemas.Add("user", "CREATE TABLE user(acc TEXT PRIMARY KEY NOT NULL,pwd TEXT NOT NULL);");
            initSchemas.Add("userlist", "CREATE TABLE userlist(nickname TEXT PRIMARY KEY NOT NULL,isLocked INTEGER NOT NULL DEFAULT 0,password TEXT,imagePath TEXT,email TEXT);");

            initSchemas.Add("customunit", "CREATE TABLE customunit(name TEXT NOT NULL,tax TEXT NOT NULL DEFAULT '',cel TEXT NOT NULL,telareacode TEXT,telnumber TEXT,faxareacode TEXT,faxnumber TEXT,country TEXT,city TEXT,address TEXT,PRIMARY KEY(name,tax,cel));");
            initSchemas.Add("customuniteditrecord", "CREATE TABLE customuniteditrecord(action TEXT NOT NULL,newproperty TEXT NOT NULL,oldproperty TEXT,user TEXT NOT NULL,name TEXT NOT NULL,tax TEXT NOT NULL DEFAULT '',cel TEXT NOT NULL,Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,FOREIGN KEY(name,tax,cel) REFERENCES customunit(name,tax,cel) ON DELETE CASCADE ON UPDATE CASCADE);");

            initSchemas.Add("myunit", "CREATE TABLE myunit(name TEXT NOT NULL,tax TEXT NOT NULL DEFAULT '',cel TEXT NOT NULL,telareacode TEXT,telnumber TEXT,faxareacode TEXT,faxnumber TEXT,country TEXT,city TEXT,address TEXT,PRIMARY KEY(name,tax,cel));");
            initSchemas.Add("myuniteditrecord", "CREATE TABLE myuniteditrecord(action TEXT NOT NULL,newproperty TEXT NOT NULL,oldproperty TEXT,user TEXT NOT NULL,name TEXT NOT NULL,tax TEXT NOT NULL DEFAULT '',cel TEXT NOT NULL,Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,FOREIGN KEY(name,tax,cel) REFERENCES myunit(name,tax,cel) ON DELETE CASCADE ON UPDATE CASCADE);");

            initSchemas.Add("formtable", "CREATE TABLE formtable(id TEXT PRIMARY KEY NOT NULL,type INTEGER NOT NULL DEFAULT 0,mname TEXT NOT NULL,mtax TEXT NOT NULL,mcel TEXT NOT NULL,oname TEXT NOT NULL,otax TEXT NOT NULL,ocel TEXT NOT NULL,note TEXT NOT NULL,country TEXT NOT NULL,city TEXT NOT NULL,address TEXT NOT NULL,total REAL NOT NULL,FOREIGN KEY(mname,mtax,mcel) REFERENCES myunit(name,tax,cel) ON DELETE CASCADE ON UPDATE CASCADE,FOREIGN KEY(oname,otax,ocel) REFERENCES customunit(name,tax,cel) ON DELETE CASCADE ON UPDATE CASCADE);");
            initSchemas.Add("formdetails", "CREATE TABLE formdetails(id TEXT NOT NULL,pname TEXT NOT NULL,length REAL NOT NULL,width REAL NOT NULL,ironmold REAL NOT NULL,powercoating REAL NOT NULL,ironslips REAL NOT NULL,nut REAL NOT NULL,FOREIGN KEY(id) REFERENCES formtable(id) ON DELETE CASCADE ON UPDATE CASCADE);");


            initSchemas.Add("formeditrecord", "CREATE TABLE formeditrecord(id TEXT NOT NULL,title TEXT NOT NULL,user TEXT NOT NULL,editid TEXT PRIMARY KEY NOT NULL,Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,FOREIGN KEY(id) REFERENCES formtable(id) ON DELETE CASCADE ON UPDATE CASCADE);");
            initSchemas.Add("formeditinfo", "CREATE TABLE formeditinfo(editid TEXT NOT NULL,action TEXT NOT NULL,property TEXT NOT NULL,oldvalue TEXT NOT NULL,newvalue TEXT NOT NULL,FOREIGN KEY(editid) REFERENCES formeditrecord(editid) ON DELETE CASCADE ON UPDATE CASCADE);");
            initSchemas.Add("producteditinfo", "CREATE TABLE producteditinfo(editid TEXT NOT NULL,action TEXT NOT NULL,property TEXT NOT NULL,oldvalue TEXT NOT NULL,newvalue TEXT NOT NULL,FOREIGN KEY(editid) REFERENCES formeditrecord(editid) ON DELETE CASCADE ON UPDATE CASCADE);");
            CreateDatabase();
        }
        public void CreateDatabase()
        {
            try
            {
                source = new SQLiteConnection(String.Format("Data Source={0};foreign keys=true;", connString));
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "連線錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
        }
        public SqliteDatabase() => initConnectionstring();
        private bool tableExists(string tableName)
        {
            var sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "';";
            SQLiteCommand command = new SQLiteCommand(sql, source);
            try
            {
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "execute tableExists query error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public bool CheckDatabaseSchema()
        {
            int count = 0;
            if (source.State == ConnectionState.Closed) source.Open();
            foreach (KeyValuePair<string, string> item in initSchemas)
            {
                if (tableExists(item.Key))
                    count += 1;
            }
            if (source.State == ConnectionState.Open) source.Close();
            if (count == initSchemas.Count)
                return true;
            return false;
        }
        public void InitDatabaseSchemas()
        {
            foreach (KeyValuePair<string, string> item in initSchemas)
                CreateTable(item.Value);
        }
        /// <summary>建立新資料表</summary>
        /// <param name="sqlCreateTable">建立資料表的 SQL 語句</param>
        public void CreateTable(string sqlCreateTable)
        {
            if (source.State == ConnectionState.Closed) source.Open();
            var command = new SQLiteCommand(sqlCreateTable, source);
            var mySqlTransaction = source.BeginTransaction();
            try
            {
                command.Transaction = mySqlTransaction;
                command.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                MessageBox.Show(ex.Message, "Create Table Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (source.State == ConnectionState.Open) source.Close();
        }


        /// <summary>新增\修改\刪除資料</summary>
        /// <param name="sqlManipulate">資料操作的 SQL 語句</param>
        public bool Manipulate(string sqlManipulate)
        {
            if (source.State == ConnectionState.Closed) source.Open();
            var command = new SQLiteCommand(sqlManipulate, source);
            //command.CommandText = "PRAGMA foreign_keys = ON;";
            //command.ExecuteNonQuery();
            var mySqlTransaction = source.BeginTransaction();
            try
            {
                command.Transaction = mySqlTransaction;
                command.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                MessageBox.Show(ex.Message, "Manipulate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (source.State == ConnectionState.Open) source.Close();
                return false;
            }
            if (source.State == ConnectionState.Open) source.Close();
            return true;
        }
        public bool Manipulate(string sqlManipulate, Dictionary<string, object> replaceValues, List<int> DataTypeList)
        {
            if (source.State == ConnectionState.Closed) source.Open();
            var command = new SQLiteCommand(sqlManipulate, source);
            int count = 0;
            foreach (var item in replaceValues)
            {
                if (DataTypeList[count] == 0)
                    command.Parameters.AddWithValue(item.Key, item.Value.ToString());
                else if (DataTypeList[count] == 1)
                    command.Parameters.AddWithValue(item.Key, Convert.ToInt32(item.Value));
                else if (DataTypeList[count] == 2)
                    command.Parameters.AddWithValue(item.Key, Convert.ToDouble(item.Value));
                else if (DataTypeList[count] == 3)
                    command.Parameters.AddWithValue(item.Key, DBNull.Value);
                count++;
            }
            //command.CommandText = "PRAGMA foreign_keys = ON;";
            //command.ExecuteNonQuery();
            var mySqlTransaction = source.BeginTransaction();
            try
            {
                command.Transaction = mySqlTransaction;
                command.ExecuteNonQuery();
                mySqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlTransaction.Rollback();
                MessageBox.Show(ex.Message, "Manipulate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (source.State == ConnectionState.Open) source.Close();
                return false;
            }
            if (source.State == ConnectionState.Open) source.Close();
            return true;
        }


        /// <summary>讀取資料</summary>
        /// <param name="sqlQuery">資料查詢的 SQL 語句</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sqlQuery)
        {
            if (source.State == ConnectionState.Closed) source.Open();
            var dataAdapter = new SQLiteDataAdapter(sqlQuery, source);
            var myDataTable = new DataTable();
            var myDataSet = new DataSet();
            myDataSet.Clear();
            dataAdapter.Fill(myDataSet);
            myDataTable = myDataSet.Tables[0];
            if (source.State == ConnectionState.Open) source.Close();
            return myDataTable;
        }
        public DataTable GetDataTable(string sqlQuery, Dictionary<string, object> replaceValues, List<int> DataTypeList)
        {
            if (source.State == ConnectionState.Closed) source.Open();
            var dataAdapter = new SQLiteDataAdapter(sqlQuery, source);
            int count = 0;
            foreach (var item in replaceValues)
            {
                if (DataTypeList[count] == 0)
                    dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, item.Value.ToString());
                else if (DataTypeList[count] == 1)
                    dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, Convert.ToInt32(item.Value));
                else if (DataTypeList[count] == 2)
                    dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, Convert.ToDouble(item.Value));
                else if (DataTypeList[count] == 3)
                    dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, DBNull.Value);
                count += 1;
            }
            var myDataTable = new DataTable();
            var myDataSet = new DataSet();
            myDataSet.Clear();
            dataAdapter.Fill(myDataSet);
            myDataTable = myDataSet.Tables[0];
            if (source.State == ConnectionState.Open) source.Close();
            return myDataTable;
        }
    }
}
