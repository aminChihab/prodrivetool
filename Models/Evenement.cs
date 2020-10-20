using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace postersopmetaal.Models
{  
    public class Evenement
    {
        public string PJmanager { get; set; }

        public string Debiteurnaam { get; set; }

        public DateTimeOffset Cursusdatum { get; set; }

        public string Project { get; set; }

        public int? Beschikbare_plaatsen { get; set; }

        public long? BcCo { get; set; }

        public long? CrId { get; set; }

        public long? DeId { get; set; }

        public string Invo { get; set; }

        public float? DfPr { get; set; }

        public bool? Selected { get; set; }
    }

    
}
