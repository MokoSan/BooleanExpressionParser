using BooleanParser;
using System.Linq.Expressions;

var expression1 = new FilterQueryExpression("ThreadID = 1,001");
var matched1 = expression1.Match(new TraceEvent
{
    EventName = "ThreadID",
    EventProperty = "ThreadID",
    EventPropertyValue = "1,001"
});
var matched2 = expression1.Match(new TraceEvent
{
    EventName = "ThreadID",
    EventProperty = "ThreadID",
    EventPropertyValue = "1,002"
});
var matched3 = expression1.Match(new TraceEvent
{
    EventName = "ThreadID",
    EventProperty = "ThreadID",
    EventPropertyValue = "1,004"
});

List<bool> matches = new List<bool>();
matches.Add(matched1);
matches.Add(matched2);
matches.Add(matched3);

foreach(var match in matches)
{
}

var a = true;
var ax = Expression.Variable(typeof(bool), "a");
var aOra = Expression.Or(ax, ax);
var reducate = Expression.Lambda<Func<bool>>(aOra).Compile()();
Console.WriteLine(reducate);

// match1 || match2 || match3
var exp1     = Expression.Or(Expression.Constant(matched1), Expression.Constant(matched2));
var reduced1 = exp1.Reduce();
var result1 = Expression.Lambda<Func<bool>>(reduced1).Compile()();
Console.WriteLine(result1);
