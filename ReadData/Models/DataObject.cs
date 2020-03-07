namespace ReadData.Models
{
    public class DataObject
    {
        public string id { get; set; }
        public string storyPoints { get; set; }
        public string type { get; set; }
        public double dev_sitting_time { get; set; }
        public double in_dev_time { get; set; }
        public double dev_total_done_time { get; set; }
        public double qa_sitting_time { get; set; }
        public double in_qa_time { get; set; }
        public double qa_total_done_time { get; set; }
        public string doneDate { get; set; }
    }
}
