using System;
using System.Collections.Generic;

namespace CPEApi.Models
{
    public partial class Tfproduct
    {
        public string Term { get; set; } = null!;
        public int RawCount { get; set; }
        public int Id { get; set; }
        public float? TermFrequency { get; set; }
        public float? LogNormalized { get; set; }
        public float? DoubleNormalized { get; set; }
        public float? Idf { get; set; }
        public float? Idfsmooth { get; set; }
        public float? Idfmax { get; set; }
        public float? Idfprob { get; set; }
        public int? Occurence { get; set; }
        public float? TfIdfCount { get; set; }
        public float? TfIdf { get; set; }
        public float? TfIdfdoubleNorm { get; set; }
        public float? TfIdfLogNorm { get; set; }
    }
}
