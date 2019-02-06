using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSDMonitor.Classes
{
    public class Contacto
    {
        public Contacto(String id, String nome, String telefone)
        {
            this.Id = id;
            this.Nome = nome;
            this.Telefone = telefone;
        }
        public Contacto()
        {
        }

        public String Id { get; set; }
        public String Nome { set; get; }
        public String Telefone { set; get; }
        public String Email { set; get; }
    }
}
