﻿using System;
using System.Collections.Generic;

namespace CPEApi.Models
{
    public partial class Cpe
    {
        public Cpe()
        {
            Antworten2s = new HashSet<Antworten2>();
            Antwortens = new HashSet<Antworten>();
        }

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

        public virtual ICollection<Antworten2> Antworten2s { get; set; }
        public virtual ICollection<Antworten> Antwortens { get; set; }
    }
}
