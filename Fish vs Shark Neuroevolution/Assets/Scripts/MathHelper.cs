using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class MathHelper {

    protected MathHelper() { } //Makes sure we cant construct an object of the class


    public static void Derivative(string input)
    {
        
    }
    /// <summary>
    /// Splits a string into a list of strings according to a set char divider
    /// </summary>
    static IEnumerable<string> SplitString(string str, char dividerChar)
    {
        List<string> dividerList = new List<string>();
        int temp = str.IndexOf(dividerChar); //First Occurence of the char
        return null;


    }

    /*Parse the input string into a list of coefficients to x^n
Take that list of coefficients and convert them into a new list of coefficients according to the rules for deriving a polynomial.
Take the list of coefficients for the derivative and create a nice string describing the derivative polynomial function.*/
}
