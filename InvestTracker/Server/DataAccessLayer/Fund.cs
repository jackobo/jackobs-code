//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvestTracker.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Fund
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fund()
        {
            this.BuyTransaction = new HashSet<BuyTransaction>();
        }
    
        public System.Guid Fund_ID { get; set; }
        public string FundName { get; set; }
        public string Currency { get; set; }
        public decimal CurrentPrice { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyTransaction> BuyTransaction { get; set; }
    }
}
