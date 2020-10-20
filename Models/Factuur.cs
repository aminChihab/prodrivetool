using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace postersopmetaal.Models
{

    public partial class Factuur
    {
        public KnCourseMember KnCourseMember { get; set; }
    }

    public partial class KnCourseMember
    {
        public Element Element { get; set; }
    }

    public partial class Element
    {
        public long? CrId { get; set; }
        public long? CdId { get; set; }
        public Fields Fields { get; set; }
    }

    public partial class Fields
    {
        public long? BcCo { get; set; }
        public string SuDa { get; set; }
        public string Invo { get; set; }
        public long? DeId { get; set; }
        public float? DfPr { get; set; }
    }

}
