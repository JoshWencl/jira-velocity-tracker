namespace ReadData.Utilities
{
    /// <summary>
    /// Codes for different transitions
    /// </summary>
    public static class TransitionCodes
    {
        /// <summary>
        /// Code for when a ticket is ready for dev
        /// </summary>
        public static string[] ReadyForDev = { "11435", "10401", "10937" };

        /// <summary>
        /// Code for when a ticket is actively worked on by dev
        /// </summary>
        public static string[] InDev = { "10405" };

        /// <summary>
        /// Code for when a ticket is ready for QA
        /// </summary>
        public static string[] ReadyForQA = { "11437", "10402", "10406" };

        /// <summary>
        /// Code for when a ticket is in QA
        /// </summary>
        public static string[] InQA = { "10403"};

        /// <summary>
        /// Code for when a ticket is considered "QA Done"
        /// </summary>
        public static string[] QADone = { "11072", "10404", "11126"};
    }
}
