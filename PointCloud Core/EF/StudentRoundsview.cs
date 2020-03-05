using System;
using System.Collections.Generic;

namespace PointCloudCore.EF
{
    public partial class StudentRoundsview
    {
        public string Name { get; set; }
        public int? Rounds { get; set; }
        public string Challenger2 { get; set; }
        public string Result { get; set; }
        public string Winner { get; set; }
    }
}
