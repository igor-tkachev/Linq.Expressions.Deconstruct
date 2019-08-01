using System;
using System.Linq.Expressions;

using Linq.Expressions.Deconstruct;

using NUnit.Framework;

using static Linq.Expressions.Deconstruct.Expr;

namespace Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Test1()
		{
			Expression<Func<int,int>> f = i => i * 2;

			switch (f.ToExpr())
			{
				case Lambda(Multiply(Parameter("i") p1, Constant(2)), (1, ("i") p2))
					when p1 == p2 :
					Console.WriteLine("Pattern Matched!");
					break;
				default:
					Console.WriteLine(f);
					Console.WriteLine(GetDebugView(f));
					Assert.Fail();
					break;
			}
		}

		[Test]
		public void Test2()
		{
			Expression<Func<int,string>> f = i => i == 0 ? i.ToString() : "2";

			switch (f.ToExpr())
			{
				case Lambda(
					Conditional(
						Equal(
							Parameter("i") p1,
							Constant(0)),
						Call({ Name : "ToString" }, Parameter("i") p2, { Count : 0 }),
						Constant("2")),
					(1, ("i") p3))
					when p1 == p2 && p1 == p3 :
					Console.WriteLine("Pattern Matched!");
					break;
				default:
					Console.WriteLine(f);
					Console.WriteLine(GetDebugView(f));
					Assert.Fail();
					break;
			}
		}

		static Func<Expression, string> _getDebugView;

		string GetDebugView(Expression expression)
		{
			if (_getDebugView == null)
			{
				var p = Expression.Parameter(typeof(Expression));

				try
				{
					var l = Expression.Lambda<Func<Expression, string>>(
						Expression.PropertyOrField(p, "DebugView"),
						p);

					_getDebugView = l.Compile();
				}
				catch (ArgumentException)
				{
					_getDebugView = e => e.ToString();
				}
			}

			return _getDebugView(expression);
		}
	}
}
