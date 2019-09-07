namespace Rofl.Reader.Models
{
    public class InferredData
    {
        /// <summary>
        /// Used as the ID for the database
        /// </summary>
        public ulong Id { get; set; }

        public Map MapID { get; set; }

        public bool BlueVictory { get; set; }
    }
}
