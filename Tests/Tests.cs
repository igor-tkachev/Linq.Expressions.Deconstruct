using System;
using System.Linq.Expressions;

using NUnit.Framework;

using static Linq.Expressions.Deconstruct.Expr;
using static System.Linq.Expressions.Expression;

namespace Linq.Expressions.Deconstruct.Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void MatchTest()
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
		public void ConditionalMatchTest()
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
		public void NewMatchTest()
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
		public void NewArrayInitMatchTest()
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
		public void ListInitMatchTest()
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

		[Test]
		public void ConstantFoldingTest()
		{
			Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

			var f1 = f.Transform(ex => ex.ToExpr() switch
			{
				Multiply(Constant(0) e,   _)               => (Expression)e,   // 0 * e => 0
				Multiply(_,               Constant(0) e)   => (Expression)e,   // e * 0 => 0
				Multiply(Constant(1),     var e)           => (Expression)e,   // 1 * e => e
				Multiply(var e,           Constant(1))     => (Expression)e,   // e * 1 => e
				Divide  (Constant(0) e,   _)               => (Expression)e,   // 0 / e => 0
				Divide  (var e,           Constant(1))     => (Expression)e,   // e / 1 => e
				Add     (Constant(0),     var e)           => (Expression)e,   // 0 + e => e
				Add     (var e,           Constant(0))     => (Expression)e,   // e + 0 => e
				Subtract(Constant(0),     var e)           => Negate(e),       // 0 - e => -e
				Subtract(var e,           Constant(0))     => (Expression)e,   // e - 0 => e
				Multiply(Constant(int x), Constant(int y)) => Constant(x * y), // x * y => e
				Divide  (Constant(int x), Constant(int y)) => Constant(x / y), // x / y => e
				Add     (Constant(int x), Constant(int y)) => Constant(x + y), // x + y => e
				Subtract(Constant(int x), Constant(int y)) => Constant(x - y), // x - y => e
				_                                          => ex
			});

			Console.WriteLine(f);
			Console.WriteLine(f1);

			Assert.IsTrue(f1.EqualsTo(i => i + 20));
		}

		[Test]
		public void ConstantFoldingExTest()
		{
			Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

			var f1 = f.TransformEx(ex => ex switch
			{
				Multiply(Constant(0) e,   _)               => e,               // 0 * e => 0
				Multiply(_,               Constant(0) e)   => e,               // e * 0 => 0
				Multiply(Constant(1),     var e)           => e,               // 1 * e => e
				Multiply(var e,           Constant(1))     => e,               // e * 1 => e
				Divide  (Constant(0) e,   _)               => e,               // 0 / e => 0
				Divide  (var e,           Constant(1))     => e,               // e / 1 => e
				Add     (Constant(0),     var e)           => e,               // 0 + e => e
				Add     (var e,           Constant(0))     => e,               // e + 0 => e
				Subtract(Constant(0),     var e)           => Negate(e),       // 0 - e => -e
				Subtract(var e,           Constant(0))     => e,               // e - 0 => e
				Multiply(Constant(int x), Constant(int y)) => Constant(x * y), // x * y => e
				Divide  (Constant(int x), Constant(int y)) => Constant(x / y), // x / y => e
				Add     (Constant(int x), Constant(int y)) => Constant(x + y), // x + y => e
				Subtract(Constant(int x), Constant(int y)) => Constant(x - y), // x - y => e
				_                                          => ex
			});

			Console.WriteLine(f);
			Console.WriteLine(f1);

			Assert.IsTrue(f1.EqualsTo(i => i + 20));
		}

		[Test]
		public void VisitTest()
		{
			Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

			var count = 0;

			f.Body.Visit(ex =>
			{
				if (ex is ParameterExpression)
					count++;
			});

			Assert.That(count, Is.EqualTo(3));
		}

		[Test]
		public void VisitExTest()
		{
			Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

			var count = 0;

			f.Body.VisitEx(ex =>
			{
				if (ex is Parameter)
					count++;
			});

			Assert.That(count, Is.EqualTo(3));
		}

		[Test]
		public void FindTest()
		{
			Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

			var r = f.Body.Find(ex => ex.ToExpr() is Constant(10));

			Console.WriteLine(r);

			Assert.IsTrue(r.EqualsTo(Constant(10)));
		}

		[Test]
		public void FindExTest()
		{
			Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

			var r = f.Body.FindEx(ex => ex is Constant(10));

			Console.WriteLine(r);

			Assert.IsTrue(r.EqualsTo(Constant(10)));
		}

		static Func<Expression,string> _getDebugView;

		static string GetDebugView(Expression expression)
		{
			if (_getDebugView == null)
			{
				var p = Parameter(typeof(Expression));

				try
				{
					var l = Lambda<Func<Expression, string>>(PropertyOrField(p, "DebugView"), p);

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
