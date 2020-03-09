using System;

namespace ReadData.Models
{
    public class Transition
    {
        public DateTime created { get; set; }
        public string fromString { get; set; }
        public string toString { get; set; }
        public TimeSpan timeInStatus { get; set; }
    }
}
