using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MunicipalityTaxApi.Models
{
    public partial class MunicipalityTaxDetail
    {
       [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; set; }
        public long MunicipalityId { get; set; }
        public string Frequency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Tax { get; set; }
    }
}
