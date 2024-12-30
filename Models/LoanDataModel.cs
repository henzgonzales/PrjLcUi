using System.ComponentModel.DataAnnotations;

namespace PrjLcUi.Models
{
    public class LoanDataModel
    {
        public decimal Id { get; set; }
        [Display(Name = "What is the value of the property?")]
        public decimal PropertyValue { get; set; }
        [Display(Name = "How much are you planning to borrow?")]
        public decimal LoanAmount { get; set; }
        [Display(Name = "LVR")]
        public string? Lvr { get; set; }
    }
}
