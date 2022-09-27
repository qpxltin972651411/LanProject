using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.Authorize.Model
{
    public class MongoDBModel
    {
        public ObjectId Id { get; set; }
        public string licenseKey { get; set; }
        public string pcID { get; set; }
    }
}
