using System.Text.RegularExpressions;

namespace BooleanParser
{
    public static class ShuntingYard
    {
        private static readonly Dictionary<char, (char symbol, int precedence, bool rightAssociative)> operators =
        new (char symbol, int precedence, bool rightAssociative)[]
        {
            ('&', 2, false),
            ('|', 1, false),
        }
        .ToDictionary(op => op.symbol);

        public static string ToPostFix(this string infix)
        {
            var stack = new Stack<char>();
            var output = new List<char>();

            foreach(var token in infix)
            {
                if (char.IsLetter(token))
                {
                    output.Add(token);
                }    

                if (operators.TryGetValue(token, out var opt1))
                {
                    while (stack.Count > 0 && operators.TryGetValue(stack.Peek(), out var op2))
                    {
                        int c = opt1.precedence.CompareTo(op2.precedence);
                        if (c < 0 || !opt1.rightAssociative && c <= 0)
                        {
                            output.Add(stack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    stack.Push(token);
                }

                else if (token == '(')
                {
                    stack.Push(token);
                }

                else if (token == ')')
                {
                    char top = '\0';
                    while (stack.Count > 0 && (top = stack.Pop()) != '(')
                    {
                        output.Add(top);
                    }

                    if (top != '(')
                        throw new ArgumentException("No matching left parentheses.");
                }
            }

            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (!operators.ContainsKey(top)) throw new ArgumentException("No matching right parentheses");
                output.Add(top);
            }

            return String.Join(" ", output);
        }

        private static readonly Regex _operandRegex = new Regex(@"-?[a-z]+");

        public static bool Match(string postfixNotation, Dictionary<string, bool> expressionMap)
        {
            // Handle fast path.
            if (postfixNotation.Length == 1)
            {
                return expressionMap["a"];
            }

            var tokens = new Stack<string>();
            string[] rawTokens = postfixNotation.Split(' ');
            foreach(var rawToken in rawTokens)
            {
                if (_operandRegex.IsMatch(rawToken))
                {
                    tokens.Push(rawToken);
                }

                else if (rawToken == "|" || rawToken == "&")
                {
                    var operand1 = tokens.Pop();
                    var operand2 = tokens.Pop();
                    var @operator = rawToken;
                    var result = EvaluateSingleExpression(operand1, operand2, @operator, expressionMap);
                    tokens.Push(result.ToString());
                }
            }

            if (tokens.Count > 0)
            {
                return bool.Parse(tokens.Pop());
            }

            throw new ArgumentException("Shouldn't get here!");
        }

        public static bool EvaluateSingleExpression(string operand1, string operand2, string @operator, Dictionary<string, bool> expressionMap)
        {
            var exp1 = expressionMap.ContainsKey(operand1) ? expressionMap[operand1] : bool.Parse(operand1);
            var exp2 = expressionMap.ContainsKey(operand2) ? expressionMap[operand2] : bool.Parse(operand2);

            switch (@operator)
            {
                case "|":
                    return exp1 || exp2;
                case "&":
                    return exp1;
                default:
                    throw new ArgumentException($"Operator: {@operator} not found.");
            }
        }
    }
}
