namespace Fraxiinus.ReplayBook.Requests.Models
{
    public class RuneRequest : RequestBase
    {
        public string RuneKey { get; set; }

        public string TargetPath { get; set; }
    }
}
