using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace b2c_ApiConnector.Models
{
    public class RequestObject
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Step { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        public string ObjectId { get; set; }
    }
}
