using System;
using System.Collections.Generic;

namespace CPEApi.Models
{
    public partial class Anfragen
    {
        public Anfragen()
        {
            Antworten2s = new HashSet<Antworten2>();
            Antwortens = new HashSet<Antworten>();
        }

        public int Id { get; set; }
        public string? Part { get; set; }
        public string? Vendor { get; set; }
        public string Product { get; set; } = null!;
        public string? Version { get; set; }
        public DateTime? Created { get; set; }

        public virtual ICollection<Antworten2> Antworten2s { get; set; }
        public virtual ICollection<Antworten> Antwortens { get; set; }
    }
}
