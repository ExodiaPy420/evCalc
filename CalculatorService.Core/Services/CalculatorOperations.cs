using CalculatorService.Core.Interfaces;

namespace CalculatorService.Core.Services
{
    public class CalculatorOperations : ICalculatorOperations
    {

        //public double Add(IEnumerable<double> addends) => addends.Sum();

        public uint Add(IEnumerable<uint> addends)
        {
            if (addends == null || addends.Take(2).Count() < 2) //addends.Take(2).Count() < 2???????????
                throw new ArgumentException("At least two operands are required.");

            // the below workaround on uints not being allowed to perform LINQ operations
            // is the one proposed by AI, i'm still looking into it to see if it really is best
            // choice or not and i plan on reviewing and changing this since i don't usually like casting.
            // Will look for future alternatives.
            // Other alternative require different validations or changing the variable type we will use,
            // could use regular int but then we need to add validation/checker for numbers to be only positive
            using var enumerator = addends.GetEnumerator(); //i guess enumerator/getenumerator is an object that iterates through the list's values kind of like using a for loop over an array in java
            var first = enumerator.Current; //and what the whole logic of operation is as we iterate over the ienumerable we extract each position's value and add them into a sum one by one
            ulong sum = first; //????????
            do
            {
                sum += enumerator.Current;
            } while (enumerator.MoveNext());


            //i don't like casting
            return checked((uint)sum);



            //return addends.Sum();
        }

        //public double Subtract(double minuend, double subtrahend) => minuend - subtrahend;


        //i had some trouble trying to conceptualize the subtraction operation instructions since the instructions said to allow more than two operands
        //and at first i was thinking that a subtraction is, by definition, one thing minus other thing, so in order to try and fit the model i chose
        //to adapt the subtraction as a left chained operation, meaning the user will be first asked for the minuend and then will be prompted to input
        //the rest of the values the user wants to subtract from that minuend.
        public double Subtract(double minuend, IEnumerable<double> subtrahends) 
        {
            if (minuend == null || subtrahends.Count() < 1)
                throw new ArgumentException("At least two operands are required.");

            double result = minuend;

            foreach (var value in subtrahends)
            {
                result -= value;
            }

            return result;


            //return operands.Sum();

        }



        //public double Multiply(IEnumerable<double> factors) => factors.Aggregate(1.0, (acc, x) => acc * x);

        public double Multiply(IEnumerable<double> factors) 
        {
            if (factors == null || factors.Count() < 2)
                throw new ArgumentException("At least two operands are required.");

            return factors.Aggregate(1.0, (acc, x) => acc * x);

        }



        public (double Quotient, double Remainder) Divide(double dividend, double divisor)
        {
            if (divisor == 0) throw new DivideByZeroException("Can't divide by zero.");



            double Quotient = Math.Floor(dividend / divisor);
            double Remainder = dividend % divisor;

            return (Quotient, Remainder);
            //return Math.Floor(dividend / divisor), dividend % divisor);
        }

        public double Sqrt(double number)
        {
            if (number < 0) throw new ArgumentException("cannot calculate square root of a negative number.");
            return Math.Sqrt(number);
        }


    }
}
