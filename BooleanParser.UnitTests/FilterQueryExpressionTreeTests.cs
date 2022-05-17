using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace BooleanParser.UnitTests
{
    [TestClass]
    public class FilterQueryExpressionTreeTests
    {
        [TestMethod]
        public void SimpleFilterQueryExpressionTree_NoParentheses_True()
        {
            FilterQueryExpressionTree filterQueryExpressionTree = new FilterQueryExpressionTree("ThreadID = 1,000");

            var matched = filterQueryExpressionTree.Match(new TraceEvent
            {
                EventName = "ThreadID",
                EventProperty = "ThreadID",
                EventPropertyValue = "1,000"
            });

            matched.Should().BeTrue();
        }

        [TestMethod]
        public void SimpleFilterQueryExpressionTree_Parentheses_True()
        {
            FilterQueryExpressionTree filterQueryExpressionTree = new FilterQueryExpressionTree("Depth >= 1000");

            var matched = filterQueryExpressionTree.Match(new TraceEvent
            {
                EventName = "Depth",
                EventProperty = "Depth",
                EventPropertyValue = "1001"
            });

            matched.Should().BeTrue();
        }

        [TestMethod]
        public void Match_SimpleExpression_True()
        {
            string expression = "(Depth = 100)";
            FilterQueryExpressionTree tree = new(expression);

            var traceEvent = new TraceEvent
            {
                EventName = "Depth",
                EventProperty = "Depth",
                EventPropertyValue = "100"
            };

            tree.Match(traceEvent).Should().BeTrue();
        }

        [TestMethod]
        public void Match_Simple2Expression_True()
        {
            string expression = "(Depth = 100) || (ProcessName = Test)";
            FilterQueryExpressionTree tree = new(expression);

            var traceEvent0 = new TraceEvent
            {
                EventName = "Depth",
                EventProperty = "Depth",
                EventPropertyValue = "100"
            };

            var traceEvent1 = new TraceEvent
            {
                EventName = "ProcessName",
                EventProperty = "ProcessName",
                EventPropertyValue = "Test"
            };

            tree.Match(traceEvent0).Should().BeTrue();
            tree.Match(traceEvent1).Should().BeTrue();
        }

        [TestMethod]
        public void Match_ComplexExpression0_True()
        {
            string expression = "((Depth = 100) || (ProcessName = Test)) && (ProcessName != Test)";
            FilterQueryExpressionTree tree = new(expression);

            var traceEvent0 = new TraceEvent
            {
                EventName = "Depth",
                EventProperty = "Depth",
                EventPropertyValue = "100"
            };

            var traceEvent1 = new TraceEvent
            {
                EventName = "ProcessName",
                EventProperty = "ProcessName",
                EventPropertyValue = "Test"
            };

            tree.Match(traceEvent0).Should().BeFalse();
            tree.Match(traceEvent1).Should().BeFalse();
        }

        [TestMethod]
        public void Match_HeavyNesting_TrueAndFalse()
        {
            string expression = "((Depth = 100 && (Depth = 100 && (Depth = 100 && (Depth = 100 & ( Depth = 100 ))))))";
            FilterQueryExpressionTree tree = new(expression);

            var traceEvent0 = new TraceEvent
            {
                EventName = "Depth",
                EventProperty = "Depth",
                EventPropertyValue = "100"
            };

            var traceEvent1 = new TraceEvent
            {
                EventName = "ProcessName",
                EventProperty = "ProcessName",
                EventPropertyValue = "Test"
            };

            tree.Match(traceEvent0).Should().BeTrue();
            tree.Match(traceEvent1).Should().BeFalse();
        }
    }
}
