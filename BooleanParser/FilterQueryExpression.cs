namespace BooleanParser
{
    public class FilterQueryExpression
    {
        public enum Operator 
        { 
            NotValid, 
            Equal,
            GreaterThan,
            GreaterThanOrEqualTo,
            LessThan,
            LessThanOrEqualTo,
            NotEqualTo,
            Contains
        };

        public FilterQueryExpression(string exp)
        {
            // TODO: Handle the case where there are no spaces. 
            var splits = exp.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            LeftOperand = splits[0];
            Op = splits[1].ToLower();
            switch(Op)
            {
                case "=":
                    O = Operator.Equal;
                    break;
                case "!=":
                    O = Operator.NotEqualTo;
                    break;
                case "contains":
                    O = Operator.Contains;
                    break;
                case ">":
                    O = Operator.GreaterThan;
                    break;
                case ">=":
                    O = Operator.GreaterThanOrEqualTo;
                    break;
                case "<":
                    O = Operator.LessThan;
                    break;
                case "<=":
                    O = Operator.LessThanOrEqualTo;
                    break;
                default:
                    O = Operator.NotValid;
                    break;
            }

            RightOperand = splits[2];
            IsDouble = double.TryParse(RightOperand, out var rhsAsDouble);
            if (IsDouble)
            {
                RightOperandAsDouble = rhsAsDouble;
            }
        }

        public static bool IsValidExpression(string expression)
        {
            var split = expression.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 3)
                return false;

            var @operator = split[1].ToLower();
            return 
                split.Length == 3 &&
                !split[0].Contains("(") &&
                !split[2].Contains(")") &&
                (@operator == "=" ||
                @operator == "!=" ||
                @operator == ">=" ||
                @operator == ">" ||
                @operator == "<=" ||
                @operator == "<" ||
                @operator == "contain");
        }

        public bool Match(TraceEvent @event)
        {
            var rhsOperand = @event.EventPropertyValue;

            if (@event.EventProperty == LeftOperand)
            {
                switch(O)
                {
                    case Operator.Equal:
                        return rhsOperand == RightOperand;
                    case Operator.NotEqualTo:
                        return rhsOperand != RightOperand;
                    case Operator.Contains:
                        return rhsOperand.Contains(RightOperand);

                    case Operator.LessThan:
                    case Operator.LessThanOrEqualTo:
                    case Operator.GreaterThan:
                    case Operator.GreaterThanOrEqualTo:
                        return HandleDoubleComparisons(this, rhsOperand, O);
                    default:
                        throw new ArgumentException("Unidentified Operator.");
                }
            } 

            return false;
        }

        internal static bool HandleDoubleComparisons(FilterQueryExpression expression, string rhsOperand, Operator o)
        {
            // TODO: Decide to throw exception?
            if (!expression.IsDouble)
            {
                throw new ArgumentException($"Right Operand: {rhsOperand} is not a double.");
            }

            if (!double.TryParse(rhsOperand, out double rhsOperandAsDouble))
                return false;

            switch(o)
            {
                case Operator.LessThan:
                    return rhsOperandAsDouble < expression.RightOperandAsDouble;
                case Operator.LessThanOrEqualTo:
                    return rhsOperandAsDouble <= expression.RightOperandAsDouble;
                case Operator.GreaterThan:
                    return rhsOperandAsDouble > expression.RightOperandAsDouble;
                case Operator.GreaterThanOrEqualTo:
                    return rhsOperandAsDouble >= expression.RightOperandAsDouble;
                default:
                    throw new ArgumentException("Unidentified Operator.");
            }
        }

        public string LeftOperand { get; }
        public string Op { get; }
        public Operator O { get; } 
        public string RightOperand { get; }
        public double RightOperandAsDouble { get; } = double.NaN;
        public bool IsDouble { get; }
    }
}
