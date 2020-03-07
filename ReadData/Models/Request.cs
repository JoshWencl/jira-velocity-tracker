using Newtonsoft.Json.Linq;

namespace ReadData.Models
{
    public class Request
    {
        public int id { get; set; }
        public JObject issueRequestResponse { get; set; }
        public JObject[] issueChangeLogRequestResponses { get; set; }
    }
}
