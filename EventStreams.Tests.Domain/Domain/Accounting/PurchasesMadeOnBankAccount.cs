using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Accounting {
    using Events;

    public class PurchasesMadeOnBankAccount {
        private readonly State _memento;
        
        public Guid Identity { get { return _memento.Identity; }}
        public decimal TotalValue { get { return _memento.TotalValue; } }
        public IEnumerable<string> ItemNames { get { return _memento.ItemNames.AsReadOnly(); } }

        public PurchasesMadeOnBankAccount(State memento) {
            if (memento == null) throw new ArgumentNullException("memento");
            _memento = memento;
        }

        protected void Apply(MadePurchase args) {
            _memento.ItemNames.Add(args.Name);
            _memento.TotalValue += args.Value;
        }

        [DataContract]
        public class State {
            [IgnoreDataMember]
            public Guid Identity { get; private set; }

            [DataMember(Order = 1)]
            public List<string> ItemNames { get; set; }

            [DataMember(Order = 2)]
            public decimal TotalValue { get; set; }

            public State(Guid identity) {
                Identity = identity;
                ItemNames = new List<string>();
            }
        }
    }
}
