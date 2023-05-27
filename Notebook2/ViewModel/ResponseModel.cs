using System.Text.Json.Serialization;

namespace Notebook2.ViewModel
{
    public class ResponseModel
    {
        public string Data { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
    }
}
