using System;
using System.Collections.Generic;

namespace CPEApi.Models
{
    public partial class Cpe
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Part { get; set; }
        public string? Vendor { get; set; }
        public string? Product { get; set; }
        public string? Version { get; set; }
        public string? Update { get; set; }
        public string? Edition { get; set; }
        public string? Language { get; set; }
        public string? SwEdition { get; set; }
        public string? TargetSw { get; set; }
        public string? TargetHw { get; set; }
        public string? Other { get; set; }
    }
}
