using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace BooleanParser.UnitTests
{
    [TestClass]
    public class ShuntingYardTests
    {
        [TestMethod]
        public void ToPostFix_ComplexQuery_PostFixNotationSuccessful()
        {
            string postfix = ShuntingYard.ToPostFix("((a & e) | b) & (c & d)");
            postfix.Should().BeEquivalentTo("a e & b | c d & &");
        }

        [TestMethod]
        public void ToPostFix_ImbalancedParentheses_Exception()
        {
            var action = () => ShuntingYard.ToPostFix("((a & b)");
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Match_SuccessfulMatching_False()
        {
            Dictionary<string, bool> expressionMap = new()
            {
                { "a",  true },
                { "b",  false },
            };

            var eval = ShuntingYard.Match(ShuntingYard.ToPostFix("(a & b)"), expressionMap);
            eval.Should().BeFalse();
        }

        [TestMethod]
        public void Match_SuccessfulMatching_True()
        {
            Dictionary<string, bool> expressionMap = new()
            {
                { "a",  true },
                { "c",  true },
            };
            var eval = ShuntingYard.Match(ShuntingYard.ToPostFix("(a & c)"), expressionMap);
            eval.Should().BeTrue();
        }

        [TestMethod]
        public void Match_ComplexQuery1_True()
        {
            Dictionary<string, bool> expressionMap = new()
            {
                { "a",  true },
                { "b",  false },
                { "c",  true },
                { "d",  false },
                { "e",  true },
            };

            var eval = ShuntingYard.Match(ShuntingYard.ToPostFix("(a & c) | (d & e)"), expressionMap);
            eval.Should().BeTrue();
        }

        [TestMethod]
        public void Match_ComplexQuery2_True()
        {
            Dictionary<string, bool> expressionMap = new()
            {
                { "a",  true },
                { "b",  false },
                { "c",  true },
                { "d",  false },
                { "e",  true },
            };
            var eval = ShuntingYard.Match(ShuntingYard.ToPostFix("((a & (a & e)) | (d & e | (e | a)))"), expressionMap);
            eval.Should().BeTrue();
        }

        [TestMethod]
        public void Match_ComplexQuery3_True()
        {
            Dictionary<string, bool> expressionMap = new()
            {
                { "a",  true },
                { "b",  false },
                { "c",  true },
                { "d",  false },
                { "e",  true },
            };
            var eval = ShuntingYard.Match(ShuntingYard.ToPostFix("((a & (a & e) | b) | (d & e & a | (e | a)))"), expressionMap);
            eval.Should().BeTrue();
        }
    }
}