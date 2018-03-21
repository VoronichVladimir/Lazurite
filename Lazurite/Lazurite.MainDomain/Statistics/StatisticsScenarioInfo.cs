﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain.Statistics
{
    /// <summary>
    /// Object - statistic target (now it can be only scenario)
    /// </summary>
    [DataContract]
    public class StatisticsScenarioInfo
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ValueTypeName { get; set; }
        [DataMember]
        public DateTime Since { get; set; }
        [DataMember]
        public DateTime To { get; set; }
    }
}