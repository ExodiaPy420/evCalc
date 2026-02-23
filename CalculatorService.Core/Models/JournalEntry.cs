
namespace CalculatorService.Core.Models
{
    public class JournalEntry
    {


        public string Operation {  get; /*set;*/ }

        public string Calculation { get; /*set;*/ }
        
        public DateTime Date { get; /*set;*/ }

       
        public JournalEntry(string operation, string calculation)
        {
            Operation = operation;
            Calculation = calculation;
            Date = DateTime.UtcNow;
        }
    
    
    }
}
