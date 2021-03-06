﻿using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Accounting.Events {
    [DataContract(Namespace = "")]
    public class Credited : EventArgs {

        [DataMember]
        public decimal Value { get; private set; }

        public Credited() { }

        public Credited(decimal value) {
            Value = value;
        }
    }
}
