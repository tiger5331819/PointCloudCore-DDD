using System;
using System.Collections.Generic;

namespace PointCloudCore.EF
{
    public partial class Rounds
    {
        public int Id { get; set; }
        public int? Rounds1 { get; set; }
        public string Challenger1 { get; set; }
        public string Challenger2 { get; set; }
        public string Result { get; set; }
        public string Winner { get; set; }
    }
}
