using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace BooleanParser.UnitTests
{
    [TestClass]
    public class FilterQueryExpressionTests
    {
        [TestMethod]
        public void IsValidExpression_Valid_True()
        {
            var expression = "ThreadID = 1,001";
            var isValid    = FilterQueryExpression.IsValidExpression(expression);
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void IsValidExpression_InvalidEmptyString_False()
        {
            var expression = "";
            var isValid    = FilterQueryExpression.IsValidExpression(expression);
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidExpression_InvalidOperatorMissing_False()
        {
            var expression = "Depth 1000";
            var isValid    = FilterQueryExpression.IsValidExpression(expression);
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidExpression_InvalidLeftOperandMissing_False()
        {
            var expression = "< 1000";
            var isValid    = FilterQueryExpression.IsValidExpression(expression);
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidExpression_InvalidRightOperandMissing_False()
        {
            var expression = "Depth >";
            var isValid    = FilterQueryExpression.IsValidExpression(expression);
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidExpression_InvalidOperator_False()
        {
            var expression = "Depth ^ 1000";
            var isValid    = FilterQueryExpression.IsValidExpression(expression);
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void SimpleFilterQueryExpressionString_EqualsProperty_True()
        {
            FilterQueryExpression expression = new FilterQueryExpression("ThreadID = 1,001");
            var matched = expression.Match(new TraceEvent
            {
                EventName = "ThreadID",
                EventProperty = "ThreadID",
                EventPropertyValue = "1,001"
            });
            matched.Should().BeTrue();
        }

        [TestMethod]
        public void SimpleFilterQueryExpressionString_NotEqualsProperty_True()
        {
            FilterQueryExpression expression = new FilterQueryExpression("ThreadID != 1,001");
            var matched = expression.Match(new TraceEvent
            {
                EventName = "ThreadID",
                EventProperty = "ThreadID",
                EventPropertyValue = "1,002"
            });
            matched.Should().BeTrue();
        }

        [TestMethod]
        public void SimpleFilterQueryExpressionString_ContainsProperty_True()
        {
            FilterQueryExpression expression = new FilterQueryExpression("ThreadID Contains ,");
            var matched = expression.Match(new TraceEvent
            {
                EventName = "ThreadID",
                EventProperty = "ThreadID",
                EventPropertyValue = "1,001"
            });
            matched.Should().BeTrue();
        }

        [TestMethod]
        public void SimpleFilterQueryExpressionDouble_LessThanEqualsProperty_True()
        {
            FilterQueryExpression expression = new FilterQueryExpression("Depth <= 1");
            var matched = expression.Match(new TraceEvent
            {
                EventName = "Depth",
                EventProperty = "Depth",
                EventPropertyValue = "0"
            });
            matched.Should().BeTrue();
        }

        [TestMethod]
        public void SimpleFilterQueryExpressionDouble_GreaterThanEqualsProperty_True()
        {
            FilterQueryExpression expression = new FilterQueryExpression("Depth >= 1");
            var matched = expression.Match(new TraceEvent
            {
                EventName = "Depth",
                EventProperty = "Depth",
                EventPropertyValue = "2"
            });
            matched.Should().BeTrue();
        }
    }
}