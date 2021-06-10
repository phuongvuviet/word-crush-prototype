using System;
using System.Collections;
using System.Collections.Generic;

public static class StringExtension 
{
    public static string ReverseString(this string myStr) {
        char[] strArr = myStr.ToCharArray();
        Array.Reverse(strArr);
        return new string(strArr);
    }
}
