namespace ReadData.Models
{
    public class Issue
    {
        public string id { get; set; }
        public string storyPoints { get; set; }
        public string type { get; set; }
        public Transition[] transitions { get; set; }
    }
}
