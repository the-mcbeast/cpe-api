using System;
using System.Collections.Generic;

namespace CPEApi.Models
{
    public partial class Antworten
    {
        public int Id { get; set; }
        public int AnfrageId { get; set; }
        public int? Cpeid { get; set; }
        public int Typ { get; set; }
        public int ResultNr { get; set; }
        public float? LocalScore { get; set; }
        public float? GlobalScore { get; set; }
        public DateTime? Created { get; set; }

        public virtual Anfragen Anfrage { get; set; } = null!;
        public virtual Cpe? Cpe { get; set; }
        public virtual Typ TypNavigation { get; set; } = null!;
    }
}
