namespace LoanCalculatorApp.Models
{
    public class AmortizationEntry
    {
        public int Month { get; set; }
        public double EMI { get; set; }
        public double InterestPaid { get; set; }
        public double PrincipalPaid { get; set; }
        public double RemainingBalance { get; set; }
    }
}
