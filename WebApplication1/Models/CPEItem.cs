
namespace CPEApi.Models
{
    public class CPEItem
    {
        public int Id { get; set; }
        public string? CpeName { get; set; }
        public string? CpeTitle { get; set; }
        public string Part { get; set; }
        public string Vendor { get; set; }
        public string Product { get; set; }
        public string? Version { get; set; }
        public string? Update { get; set; }
        // Any / ignore
        public string? Edition { get; set; }
        public string? Language { get; set; }
        public string? Sw_edition { get; set; }
        public string? Target_sw { get; set; }
        public string? Target_hw { get; set; }
        public string? Other { get; set; }

        public float? LocalQueryScore { get; set; }
        public float? GlobalQueryScore { get; set; }

    }
}

