using System.Text.RegularExpressions;

namespace DaikinNet;

public class StringUtils
{
    public static string CamelCaseToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Use a regular expression to find the places where a lowercase letter is followed by an uppercase letter
        var regex = new Regex("([a-z])([A-Z])");
        
        // Replace such occurrences with the lowercase letter followed by an underscore and the uppercase letter in lowercase
        var result = regex.Replace(input, "$1_$2").ToLower();

        return result;
    }
}