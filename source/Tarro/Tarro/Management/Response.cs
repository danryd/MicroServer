namespace Tarro.Management
{
    public class Response
    {
        public string Content
        {
            get; set;
        }
        public string ContentType { get; set; }

        public int HttpStatusCode { get; set; }
    }
}