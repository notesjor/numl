namespace numl.Serialization
{
  internal static class JsonConstants
  {
    //begin-array     = ws %x5B ws  ; [ left square bracket
    internal const int BEGIN_ARRAY = '[';
    //begin-object    = ws %x7B ws; { left curly bracket
    internal const int BEGIN_OBJECT = '{';
    //name-separator  = ws %x3A ws; : colon
    internal const int COLON = ':';
    //value-separator = ws %x2C ws; , comma
    internal const int COMMA = ',';
    //end-array       = ws %x5D ws; ] right square bracket
    internal const int END_ARRAY = ']';
    //end-object      = ws %x7D ws; } right curly bracket
    internal const int END_OBJECT = '}';
    // \
    internal const int ESCAPE = '\\';
    // "
    internal const int QUOTATION = '"';

    internal static readonly char[] FALSE = {'f', 'a', 'l', 's', 'e'};
    internal static readonly char[] NULL = {'n', 'u', 'l', 'l'};
    internal static readonly char[] NUMBER = {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '-', '+', 'e', 'E'};
    internal static readonly char[] TRUE = {'t', 'r', 'u', 'e'};
    internal static readonly char[] WHITESPACE = {' ', '\t', '\n', '\r'};
  }
}