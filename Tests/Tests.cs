using System;
using System.Linq.Expressions;

using NUnit.Framework;

using static Linq.Expressions.Deconstruct.Expr;

namespace Linq.Expressions.Deconstruct.Tests
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

		[Test]
		public void Test3()
		{
			Expression<Func<int,object>> f = i => new { i, a = i * 4 };

			switch (f.ToExpr())
			{
				case Lambda(
					New(
						(2, { Name : "i"},  { Name : "a" }),
						(2, Parameter("i"), Multiply(Parameter("i"), Constant(4)))),
					(1, Parameter("i"))) :
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
		public void Test4()
		{
			Expression<Func<string,string[]>> f = s => new[] { s, s + "4" };

			switch (f.ToExpr())
			{
				case Lambda(
					NewArrayInit(
						(2, Parameter("s"), Add(Parameter("s"), Constant("4")))),
					(1, Parameter("s"))) :
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
		public void Test5()
		{
			Expression<Func<string,System.Collections.Generic.List<string>>> f =
				s => new System.Collections.Generic.List<string>(2) { s, s + "4" };

			switch (f.ToExpr())
			{
				case Lambda(
					ListInit(
						New((1, Constant(2))),
						(2, ElementInit((1, Parameter("s"))), ElementInit((1, Add(Parameter("s"), Constant("4")))))),
					(1, Parameter("s"))) :
					Console.WriteLine("Pattern Matched!");
					break;
				default:
					Console.WriteLine(f);
					Console.WriteLine(GetDebugView(f));
					Assert.Fail();
					break;
			}
		}

		static Func<Expression,string> _getDebugView;

		static string GetDebugView(Expression expression)
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
