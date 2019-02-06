using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace USSDMonitor
{
    public class Tentativa
    {
        public Tentativa(String tentativaid, DateTime inicio, DateTime fim, int nr ,bool success, bool alertou)
        {
            this.tentativaID = tentativaid;
            this.Inicio = inicio;
            this.Fim = fim;
            this.Success = success;
            this.Nr = nr;
            this.Alertou = alertou;
        }

        public Tentativa()
        {


        }

        [BsonId]
        public String tentativaID { set; get; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Inicio { set; get; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Fim { set; get; }
        public int Nr { set; get; }
        public bool Success {set; get;}
        public bool Alertou { set; get; }
    }
}