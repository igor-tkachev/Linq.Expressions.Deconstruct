using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using NUnit.Framework;

using static Linq.Expressions.Deconstruct.Expr;
using static System.Linq.Expressions.Expression;

// ReSharper disable UselessBinaryOperation

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
				case Lambda(Multiply(Parameter("i") p1, Constant(2)), [("i") p2])
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
			Expression<Func<int,string>> f = i => i == 0 ? i.ToString("C") : "2";

			switch (f.ToExpr())
			{
				case Lambda(
					Conditional(
						Equal(
							Parameter("i") p1,
							Constant(0)),
						Call({ Name : "ToString" }, Parameter("i") p2, [Constant("C")]),
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
						[{ Name : "i"},  { Name : "a" }],
						[Parameter("i"), Multiply(Parameter("i"), Constant(4))]),
					[("i") _]) :
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
					NewArrayInit([Parameter("s"), Add(Parameter("s"), Constant("4"))]),
					[("s") _]) :
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
			Expression<Func<string,System.Collections.Generic.List<string>>> f = s => new (2) { s, s + "4" };

			switch (f.ToExpr())
			{
				case Lambda(
					ListInit(
						New((1, Constant(2))),
						[([Parameter("s")]) _, ([Add(Parameter("s"), Constant("4"))]) _]),
					[Parameter("s")]) :
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

#if NETCOREAPP3_1

		public static T Fold<T>(T expr)
			where T : notnull, LambdaExpression
		{
			return expr.TransformEx(FoldExpr);
		}

		[return: NotNullIfNotNull("expr")]
		public static Expression? Fold(Expression? expr)
		{
			return expr.TransformEx(FoldExpr);
		}

		static Expr FoldExpr(Expr ex)
		{
			return ex switch
			{
				Multiply(Constant(0) e,   _)               => e,                // 0 * e => 0
				Multiply(_,               Constant(0) e)   => e,                // e * 0 => 0
				Multiply(Constant(1),     var e)           => e,                // 1 * e => e
				Multiply(var e,           Constant(1))     => e,                // e * 1 => e
				Divide  (Constant(0) e,   _)               => e,                // 0 / e => 0
				Divide  (var e,           Constant(1))     => e,                // e / 1 => e
				Add     (Constant(0),     var e)           => e,                // 0 + e => e
				Add     (var e,           Constant(0))     => e,                // e + 0 => e
				Subtract(Constant(0),     var e)           => Negate(e)!,       // 0 - e => -e
				Subtract(var e,           Constant(0))     => e,                // e - 0 => e
				Multiply(Constant(int x), Constant(int y)) => Constant(x * y)!, // x * y => e
				Divide  (Constant(int x), Constant(int y)) => Constant(x / y)!, // x / y => e
				Add     (Constant(int x), Constant(int y)) => Constant(x + y)!, // x + y => e
				Subtract(Constant(int x), Constant(int y)) => Constant(x - y)!, // x - y => e

				Add     (Add     (Constant(int c1), var e), Constant(int c2)) => Fold(Add     (Constant(c1 + c2), e)),  // (c1 + e) + c2 = (c1 + e2) + e
				Multiply(Multiply(Constant(int c1), var e), Constant(int c2)) => Fold(Multiply(Constant(c1 * c2), e)),  // (c1 + e) + c2 = (c1 + e2) + e

				Add     (var e,           Constant c)      => Add     (c, e),            // e + c => c + e
				Multiply(var e,           Constant c)      => Multiply(c, e),            // e * c => c * e
				Subtract(var e,           Constant(int x)) => Add     (Constant(-x), e), // e - c => (-c) + e

				Add     (var e1,          Add     (var e2, var e3)) => Fold(Add     (Add     (e1, e2), e3)),  // e1 + (e2 + e3) = (e1 + e2) + e3
				Multiply(var e1,          Multiply(var e2, var e3)) => Fold(Multiply(Multiply(e1, e2), e3)),  // e1 + (e2 + e3) = (e1 + e2) + e3

				Subtract(Add(var e1, var e2), var e3) when e2 == e3 => e1,      // (e1 + e2) - e2 => e1

				_                                          => ex
			};
		}

		[Test]
		public void ConstantFoldingExTest1()
		{
			Expression<Func<int,int>> f = i => i + 1 - 1 + 2 - i;

			var f1 = Fold(f);

			Console.WriteLine(f);
			Console.WriteLine(f1);

			Assert.IsTrue(f1.EqualsTo(i => 2));
		}

#endif

		[Test]
		public void ConstantFoldingExTest2()
		{
			Expression<Func<long,long>> f = i => i * 0L + 0 + i + 10L * (i * 0 + 2);

			static Expr Add(Expr ex, object xv, object yv)
			{
				return (xv, yv) switch
				{
					(char x, char y) => Constant(System.Convert.ChangeType(x + y, ((Expression)ex).Type)),
					(int  x, int  y) => Constant(System.Convert.ChangeType(x + y, ((Expression)ex).Type)),
					//(long x, long y) => Constant(System.Convert.ChangeType(x + y, ((Expression)ex).Type)),
					_ => ex!
				};
			}

			var f1 = f.TransformEx(ex => ex switch
			{
				Multiply(Constant(_, 0) e,   _)                => e,                // 0 * e => 0
				Multiply(_,                  Constant(_, 0) e) => e,                // e * 0 => 0
				Multiply(Constant(_, 1),     var e)            => e,                // 1 * e => e
				Multiply(var e,              Constant(_, 1))   => e,                // e * 1 => e
				Divide  (_,                  Constant(_, 0))   => ex,
				Divide  (Constant(_, 0) e,   _)                => e,                // 0 / e => 0
				Divide  (var e,              Constant(_, 1))   => e,                // e / 1 => e
				Add     (Constant(_, 0),     var e)            => e,                // 0 + e => e
				Add     (var e,              Constant(_, 0))   => e,                // e + 0 => e
				Subtract(Constant(_, 0),     var e)            => Negate(e),        // 0 - e => -e
				Subtract(var e,              Constant(_, 0))   => e,                // e - 0 => e
				Multiply(Constant(long x),   Constant(long y))  => Constant(x * y), // x * y => e
				Divide  (Constant(long x),   Constant(long y))  => Constant(x / y), // x / y => e
				Add     (Constant(var  x),   Constant(var  y))  => Add(ex, x, y),   // x + y => e
				Subtract(Constant(long x),   Constant(long y))  => Constant(x - y), // x - y => e
				_                                              => ex
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

		[Test]
		public void ReplaceMethodTest()
		{
			Expression<Func<string[]>> f = () => "dog,cat".Split(new[] { ',' });

			var f1 = f.TransformEx(ex => ex switch
			{
				Call({ Name : "Split" }, Constant("dog,cat"), (1, _)) => NewArrayInit(typeof(string), Constant("dog"), Constant("cat")),
				_ => ex
			});

			Console.WriteLine(f);
			Console.WriteLine(f1);

			Assert.IsTrue(f1.EqualsTo(() => new[] { "dog", "cat" }));
		}

		static Func<Expression,string>? _getDebugView;

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
