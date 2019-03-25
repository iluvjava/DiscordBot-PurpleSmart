using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MathyStuff
{


    ///This class will compute precise integer fraction. 
    ///It's immutable. 
   public class ImproperFraction : ICloneable, IComparable, IFraction
    {
        public void Main(string[] args)
        {

        }
        //public readonly bool sign; // ture-> positive; false-> negative. 
        protected Int64 d; // Denominator
        protected Int64 n; // Numerator 

        /// <summary>
        ///    <b> First</b> parameter is the numerator and the second is denominator. 
        /// </summary>
        /// <param name="n"></param>
        /// A positive Integer. 
        /// <param name="d"></param>
        /// A positive Integer. 
        public ImproperFraction(Int64 n, Int64 d)
        {
            //check for vadility: 
            bool d_iszero = d == 0;
            if (d_iszero) throw new DividebyZeroException("Numerator is zero. ");

            if (n < 0 || d < 0) throw new Exception("Input can not be neg in ImproperFraction class. ");

            this.n = n;
            this.d = d;

            // Simplify this number by GCD; 
            this.simplify();
        }

        ~ImproperFraction()
        {
            // THis is a dectructor you know. 
        }

        //******************************
        // Implementing some object methods. 
        public override string ToString()
        {
            if (n == 0) return "0";
            String res = n + "/" + d;
            return res;
        }

        /// <summary>
        /// <para>
        /// -----Test needed-----
        /// </para>
        /// Compares instances directly by its fields. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ImproperFraction frac)
            {
                return this.getFloatValue() == frac.getFloatValue();
            }
            return false;
        }

        /// <summary>
        /// The sum of neumerator and denominator and then cast to int. 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {

            return (int)(d * n);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// A deep copy of the instance. 
        public object Clone()
        {
            return new ImproperFraction(this.d, this.n);
        }

        public int CompareTo(object obj)
        {
            if (obj is ImproperFraction frac)
            {

                return Math.Sign(this.getFloatValue() - frac.getFloatValue());
            }
            throw new NotImplementedException();
        }
        //*****************************
        // Methods that makes this class actually useful. 

        public static Int64 GCD(Int64 a, Int64 b)
        {
            if (b == 0)
            {
                return a;
            }
            return GCD(b, a % b);
        }

        protected void simplify()
        {
            Int64 gcd = GCD(n, d);
            this.n /= gcd;
            this.d /= gcd;
        }


        /// <summary>
        /// Approximate the number using a float value. 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public float getFloatValue()
        {
            // Just to make sure it returns exact 0 when n is zero...
            if (n == 0) return 0;
            return n / (float)(d);
        }

        /// <summary>
        /// 
        /// This method return a approximate value for the fraction using a double. 
        /// </summary>
        /// <returns></returns>
        public double getDoubleValue()
        {
            return n / (double)(d);
        }

        public Int64 getNumerator()
        {
            return this.n;
        }

        public Int64 getDenominator()
        {
            return this.d;
        }


        //**********************************************
        //Some helpful static methods: 

        /// <summary>
        /// <para>
        /// This method take in a double and return a integer fraction that 
        /// is approximately the same value. </para>
        /// Double inputs will be scaled up by a factor of 10^15. 
        /// </summary>
        /// <returns></returns>
        /// null if the double is shit like infinity of Nan. 
        public static ImproperFraction doubleapproximate(double arg)
        {
               
            return null; 
        }

        /// <summary>
        /// 
        /// This method take in a decimal and return a integer fraction that 
        /// is approximately the same value 
        /// </summary>
        /// <returns></returns>
        /// null if the double is shit like infinity of Nan. 
        public static ProperFraction doubleapproximate(decimal arg)
        {

            return null;
        }

    }


    /// <summary>
    /// <para>
    /// Number in the form of i+f where i is an integer and f is a fraction value smaller
    /// smaller than one. 
    /// </para>
    /// This class will have some override operator. 
    /// </summary>
    public class ProperFraction: IFraction,ICloneable
    {

        public static ProperFraction ZERO = new ProperFraction(0, 1); //Some useful constant. 

        public readonly bool  sign; // <- True is positive and false is negative. 
        protected ImproperFraction decimalpart;
        protected Int64 integerpart;


        /// <summary>
        /// construct an instance of proper fraction by inputing numerator and denominator. 
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// Numerator.
        /// <param name="d"></param>
        /// Denominator
        /// If this is zero, then this class will throw an zero 
        /// denominator exception. 
        /// 
         public ProperFraction(Int64 n,Int64 d)
        {
            if (d == 0) throw new DividebyZeroException();
            if (n == 0)
            {
                this.integerpart = 0;
                decimalpart = new ImproperFraction(0, Math.Abs(d));
                return;
            }

            sign = n < 0 == d < 0;
            n = Math.Abs(n);d = Math.Abs(d);
      
            Int64 integerdivide = n / d;
            integerpart = integerdivide; 
            n = n % d;

            decimalpart = new ImproperFraction(n, d);

        }
        /// <summary>
        /// The construct properfraction will be number/1, which is a 
        /// fraction but actually an integer. 
        /// </summary>
        /// <param name="number"></param>
        public ProperFraction(Int64 number): this(number,1)
        {

        }

        /// <summary>
        /// Internal use only! 
        /// </summary>
        protected ProperFraction(bool sign)
        {
            this.sign = sign; 
        }


        /// <summary>
        /// Display the number in the form of integer plus some fraction. 
        /// Function might return empty string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //There is only integer part. 
            if (decimalpart.getFloatValue() == 0)
            {
                return (sign?"":"-" )+ this.integerpart;
            }

            //The number is zero. 
            if (integerpart == 0 && decimalpart.getNumerator() == 0) return "0";

            //decimal part is not zero and both are not zero. 
            String res = sign ? "" : "-";
            res = res+"("+ (integerpart == 0 ? "" : integerpart+"+");
            res+=decimalpart.ToString()+")";
            return res;
        }


        /// <summary>
        /// 
        /// The double number that is closest to the fraction. 
        /// </summary>
        /// <returns></returns>
        public  double getDoubleValue()
        {

            double res = this.decimalpart.getDoubleValue();
            return sign?(res+this.integerpart):-(res+this.integerpart);

        }

        public decimal getDecimalValue()
        {
            decimal n = this.decimalpart.getNumerator();
            decimal d = this.decimalpart.getDenominator();
            return sign?n / d:-n/d;
        }


        /// <summary>
        /// 
        /// It will throw devide by zero error. 
        /// </summary>
        /// <returns></returns>
        public ProperFraction getReciprecal()
        {

            if (decimalpart.getNumerator() == 0 && this.integerpart ==0)
            throw new DivideByZeroException();

            //After above statement: 
            //decimal part or integer part is not zero. 

            Int64 newN = integerpart * decimalpart.getDenominator()+decimalpart.getNumerator();
            if (!sign) newN = -newN;

            return new ProperFraction(decimalpart.getDenominator(),newN);

        }


        public object Clone()
        {
            throw new NotImplementedException();
        }

        public static implicit operator double(ProperFraction f)
        {
            return f.getDoubleValue();
        }
        public static implicit operator decimal(ProperFraction f)
        {
            return f.getDecimalValue();
        }

        /// <summary>
        /// This function will not work properly if there is some sort of overflow. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ProperFraction operator +(ProperFraction a, ProperFraction b)
        {
            if (a == null || b == null) throw new Exception();
            Int64 aN = a.decimalpart.getNumerator(), aD = a.decimalpart.getDenominator();
            Int64 bN = b.decimalpart.getNumerator(), bD = b.decimalpart.getDenominator();
            Int64 aI = a.integerpart, bI = b.integerpart;
            Int64 NewDenominator = aD * bD;
            aN = aI*aD+aN;
            bN = bI * bD+bN;
            //Take care of the sign. 
            if (!a.sign) aN = -aN;
            if (!b.sign) bN = -bN;

            aN = aN * bD;
            bN = bN * aD;
            return new ProperFraction(aN + bN, NewDenominator);

        }

        public static ProperFraction operator -(ProperFraction a)
        {
            if (a == null) throw new Exception();
            ProperFraction pf = new ProperFraction(!a.sign);
            pf.decimalpart = new ImproperFraction(a.decimalpart.getNumerator(),a.decimalpart.getDenominator());
            pf.integerpart = a.integerpart;
            return pf;
        }
        public static ProperFraction operator -(ProperFraction a,ProperFraction b)
        {
            if (a == null) throw new Exception();
            return a+(-b);
        }

        /// <summary>
        /// Works fine, after moderate testing. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ProperFraction operator *(ProperFraction a, ProperFraction b)
        {
            if (a == null || b == null) throw new Exception();
            Int64 aN = a.decimalpart.getNumerator(), aD = a.decimalpart.getDenominator();
            Int64 bN = b.decimalpart.getNumerator(), bD = b.decimalpart.getDenominator();
            Int64 aI = a.integerpart, bI = b.integerpart;

            //Covert to improper fraction to elimiate integer part. 
            aN += aI * aD; bN += bI * bD;

            //Does numerator of a share factor with denominator of b? 
            Int64 factor1 = ImproperFraction.GCD(aN,bD);
            aN /= factor1;bD /= factor1;

            //Does the numerator of b share factor with the denominator of a?
            Int64 factor2 = ImproperFraction.GCD(bN,aD);
            bN /= factor2; aD /= factor2;
           
            //take care of the sign: 
            bool negative = a.sign != b.sign;

            if (negative) aN = -aN;

            return new ProperFraction(aN*bN,aD*bD); 
        }

        public static ProperFraction operator /(ProperFraction a, ProperFraction b)
        {
            return a * (b.getReciprecal());
        }
    }


    /// <summary>
    /// This is a marker interface. At least for now. 
    /// </summary>
     public interface IFraction
    {
        
    }


    [Serializable]
    class DividebyZeroException : Exception
    {
        public DividebyZeroException()
        {
        }

        public DividebyZeroException(string message) : base(message)
        {
        }

        public DividebyZeroException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DividebyZeroException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


   
}
