using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace USSDMonitor
{
    public class Historico
    {
        public Historico(ObjectId id, DateTime inicio, DateTime fim, String operadora, String resultado, String output, bool success)
        {
            this.Id = id;
            this.Inicio = inicio;
            this.Fim = fim;
            this.Resultado = resultado;
            this.Output = output;
            this.Operadora = operadora;
            this.Success = success;
        }

        public Historico()
        {


        }

        public ObjectId Id { set; get; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Inicio { set; get; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Fim { set; get; }
        public String Resultado { set; get; }
        public String Output { set; get; }
        public String Operadora { set; get; }
        public bool Success
        {
            set; get;
        }
    }
}