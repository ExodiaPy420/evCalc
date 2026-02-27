using System.ComponentModel.DataAnnotations;

namespace CalculatorService.Core.Models
{
	public class AddRequest
	{
		[MinLength(2)]
		public IEnumerable<double> Addends { get; set; }
	}
}
