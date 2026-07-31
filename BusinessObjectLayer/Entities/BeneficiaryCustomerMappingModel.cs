using System.Collections.Generic;

namespace BusinessObjectsLayer.Entities
{
    // Junction model for the many-to-many BeneficiaryCustomerMapping table.
    public class BeneficiaryCustomerMappingModel
    {
        public int? CustomerId { get; set; }
        public int? BeneficiaryId { get; set; }
        public List<int>? BeneficiaryIds { get; set; }
        public List<int>? CustomerIds { get; set; }
    }
}
