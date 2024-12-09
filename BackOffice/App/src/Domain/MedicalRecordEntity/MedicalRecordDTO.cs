using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sempi5.Domain.MedicalRecordEntity
{
    public class MedicalRecordDTO
    {
        public string Patient { get; set; }
        public List<RecordLineDTO> RecordLine { get; set; }
    }  
}