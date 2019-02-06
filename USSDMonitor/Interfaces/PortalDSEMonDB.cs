using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using USSDMonitor;

namespace PortalDSEMonitorizacao.Models
{
    public class PortalDSEMonDB
    {
        public IMongoDatabase Database;
        public String DataBaseName = "PortalDSEMonDB";
        string conexaoMongoDB = "";
        string connectionString = "mongodb://wemz10kad0:27017";

        public PortalDSEMonDB()
        {

            conexaoMongoDB = connectionString;
            var cliente = new MongoClient(conexaoMongoDB);
            Database = cliente.GetDatabase(DataBaseName);
        }

        public IMongoCollection<Historico> Historicos
        {
            get
            {
                var Historicos = Database.GetCollection<Historico>("Historicos");
                return Historicos;
            }
        }
        public IMongoCollection<Tentativa> Tentativas
        {
            get
            {
                var Tentativas = Database.GetCollection<Tentativa>("Tentativas");
                return Tentativas;
            }
        }

    }
}