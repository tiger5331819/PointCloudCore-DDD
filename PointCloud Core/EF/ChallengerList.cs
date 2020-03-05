using System;
using System.Collections.Generic;

namespace PointCloudCore.EF
{
    public partial class ChallengerList
    {
        public int Id { get; set; }
        public int? Rounds { get; set; }
        public int? Count { get; set; }
        public string Challenger1 { get; set; }
        public string Challenger2 { get; set; }
    }
}
