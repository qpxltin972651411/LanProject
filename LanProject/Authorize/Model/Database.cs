using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Configuration;
using System.Windows;

namespace LanProject.Authorize.Model
{
    public class Database
    {
        private string ConnectionString { get; set; }
        private MongoClient client = null;
        public IMongoDatabase conn = null;
        private string username { get; set; }
        private string pwd { get; set; }
        private string databasename { get; set; }
        private void GetConnection()
        {
            username = ConfigurationManager.AppSettings["username"].ToString();
            pwd = ConfigurationManager.AppSettings["password"].ToString();
            databasename = ConfigurationManager.AppSettings["database"].ToString();
            ConnectionString = ConfigurationManager.AppSettings["authorizeApplication.Properties.Settings.mongodbConnectionstring"].ToString();
            ConnectionString = ConnectionString.Replace("replaceusername", username).Replace("replacepassword", pwd);
            var settings = MongoClientSettings.FromConnectionString(ConnectionString);
            client = new MongoClient(settings);
            conn = client.GetDatabase(databasename);
        }
        public Database()
        {
            try
            {
                GetConnection();
                bool isMongoLive = conn.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(3000);
                if (!isMongoLive)
                    MessageBox.Show("無法連接");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
