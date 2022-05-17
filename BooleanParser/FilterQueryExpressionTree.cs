using System.Text.RegularExpressions;

namespace BooleanParser
{
    public sealed class FilterQueryExpressionTree
    {
        private readonly FilterQueryExpression _simpleExpression; 
        private readonly string _expression;
        private readonly string _postFixExpression;
        private readonly Dictionary<char, FilterQueryExpression> _expressionMap;

        public FilterQueryExpressionTree(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentException($"{nameof(expression)} is null. Invalid Filter Expression Tree.");
            }

            // Base Case: Simple Expression Without Parentheses.
            if (FilterQueryExpression.IsValidExpression(expression) && 
                (!expression.Contains("(") || expression.Contains(")")))
            {
                _simpleExpression = new(expression);
            }
            
            else
            {
                _expression = expression;

                // Prime Expression -> Post Fix
                string primedExpression = PrimeExpression(_expression, out var expressionMap);
                _expressionMap = expressionMap;
                _postFixExpression = ShuntingYard.ToPostFix(primedExpression);
            }
        }

        public bool Match(TraceEvent @event)
        {
            if (_simpleExpression != null)
            {
                return _simpleExpression.Match(@event);
            }

            Dictionary<string, bool> convertedExpressionMap = new();
            foreach(var kvp in _expressionMap)
            {
                convertedExpressionMap[kvp.Key.ToString()] = kvp.Value.Match(@event);
            }

            return ShuntingYard.Match(_postFixExpression, convertedExpressionMap);
        }

        public static string PrimeExpression(string expression, out Dictionary<char, FilterQueryExpression> expressionMap)
        {
            var returnExpression = expression;
            returnExpression = returnExpression.Replace("&&", "&").Replace("||", "|");

            expressionMap = new();
            expression = expression.Replace("&&", "`").Replace("||", "`");
            expression = expression.Replace("(", "").Replace(")", "");

            var splitExpression = expression.Split('`');
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
            for (int i = 0; i < splitExpression.Length; i++)
            {
                // Constrain to 26 expressions.
                FilterQueryExpression fe = new(splitExpression[i]);
                var alphabet = alpha[i];
                expressionMap[alphabet] = fe;
                returnExpression = returnExpression.Replace(splitExpression[i].TrimStart().TrimEnd(), alphabet.ToString());
            }

            return returnExpression;
        }
    }
}
