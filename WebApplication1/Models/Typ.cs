using System;
using System.Collections.Generic;

namespace CPEApi.Models
{
    public partial class Typ
    {
        public Typ()
        {
            Antworten2s = new HashSet<Antworten2>();
            Antwortens = new HashSet<Antworten>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Antworten2> Antworten2s { get; set; }
        public virtual ICollection<Antworten> Antwortens { get; set; }
    }
}
