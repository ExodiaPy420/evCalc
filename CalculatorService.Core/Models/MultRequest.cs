using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorService.Core.Models
{
    public class MultRequest
    {
        public IEnumerable<uint> Factors { get; set; }
    }
}
