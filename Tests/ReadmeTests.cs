using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

using static Linq.Expressions.Deconstruct.Expr;
using Ex = System.Linq.Expressions.Expression;

// The examples in README.md are copied from the methods of this class (the code between the braces).
// Keep them in sync: the point of this file is that every README example compiles and passes.
namespace Linq.Expressions.Deconstruct.Tests
{
	[TestFixture]
	public class ReadmeTests
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
					Assert.Fail();
					break;
			}
		}

		[Test]
		public void ConstantFoldingTest()
		{
			Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

			var f1 = f.TransformEx(ex => ex switch
			{
				Multiply(Constant(_, 0) e,   _)               => e,                  // 0 * e => 0
				Multiply(_,                  Constant(_, 0) e) => e,                 // e * 0 => 0
				Multiply(Constant(_, 1),     var e)           => e,                  // 1 * e => e
				Multiply(var e,              Constant(_, 1))  => e,                  // e * 1 => e
				Divide  (Constant(_, 0) e,   _)               => e,                  // 0 / e => 0
				Divide  (var e,              Constant(_, 1))  => e,                  // e / 1 => e
				Add     (Constant(_, 0),     var e)           => e,                  // 0 + e => e
				Add     (var e,              Constant(_, 0))  => e,                  // e + 0 => e
				Subtract(Constant(_, 0),     var e)           => Ex.Negate(e),       // 0 - e => -e
				Subtract(var e,              Constant(_, 0))  => e,                  // e - 0 => e
				Multiply(Constant(int x),    Constant(int y)) => Ex.Constant(x * y), // x * y => e
				Divide  (Constant(int x),    Constant(int y)) => Ex.Constant(x / y), // x / y => e
				Add     (Constant(int x),    Constant(int y)) => Ex.Constant(x + y), // x + y => e
				Subtract(Constant(int x),    Constant(int y)) => Ex.Constant(x - y), // x - y => e
				_                                             => ex
			});

			Assert.That(f1.EqualsTo(i => i + 20), Is.True);
		}

		[Test]
		public void ToExprTest()
		{
			Expression expression = Ex.Add(Ex.Constant(1), Ex.Constant(2));

			// Expression => Expr, Expr => Expression.
			Expr   expr = expression.ToExpr();
			Expression back = expr;

			Assert.That(back, Is.SameAs(expression));

			// Children are wrapped on demand.
			if (expr is Add(var left, var right))
				Console.WriteLine($"{left} + {right}");
			else
				Assert.Fail();

			// Constant(value, intValue): intValue is set for any numeric constant that fits into int.
			Assert.That(Ex.Constant(5L).ToExpr()  is Constant(_, 5), Is.True);
			Assert.That(Ex.Constant(5.0).ToExpr() is Constant(_, 5), Is.True);
			Assert.That(Ex.Constant(0L).ToExpr()  is Constant(_, 0), Is.True);
			Assert.That(Ex.Constant(0L).ToExpr()  is Constant(0),    Is.False); // Constant(value) matches the boxed value as is
		}

		[Test]
		public void VisitAndFindTest()
		{
			Expression<Func<int,int>> f = i => i * 2 + 1;

			var constants = new List<int>();

			// Visit(Action) calls the action for every node, children first.
			f.Visit(ex =>
			{
				if (ex is ConstantExpression { Value: int value })
					constants.Add(value);
			});

			Assert.That(constants, Is.EqualTo(new[] { 2, 1 }));

			// Visit(Func) calls the function for a node before its children;
			// returning false skips the children of that node.
			var multiplies = 0;

			f.Visit(ex =>
			{
				if (ex.NodeType == ExpressionType.Multiply)
					multiplies++;

				return ex.NodeType != ExpressionType.Multiply;
			});

			Assert.That(multiplies, Is.EqualTo(1));

			// Find returns the first expression for which the function returns true, or null.
			var found = f.FindEx(ex => ex is Multiply(_, Constant(2)));

			Assert.That(found, Is.Not.Null);
			Assert.That(f.Find(ex => ex.NodeType == ExpressionType.Divide), Is.Null);
		}

		[Test]
		public void TransformOrderTest()
		{
			Expression f = Ex.Multiply(Ex.Add(Ex.Constant(1), Ex.Constant(2)), Ex.Constant(3));

			var log = new List<string>();

			// By default children are transformed first (post-order):
			// the function sees the already transformed children.
			f.Transform(ex =>
			{
				log.Add(ex.NodeType.ToString());
				return ex;
			});

			Assert.That(log, Is.EqualTo(new[] { "Constant", "Constant", "Add", "Constant", "Multiply" }));

			// With forward: true the function is called for a node before its children,
			// and a replaced node is not descended into.
			log.Clear();

			f.Transform(true, ex =>
			{
				log.Add(ex.NodeType.ToString());
				return ex;
			});

			Assert.That(log, Is.EqualTo(new[] { "Multiply", "Add", "Constant", "Constant", "Constant" }));
		}

		[Test]
		public void EqualsToTest()
		{
			Expression<Func<int,int>> f1 = x => x * 2;
			Expression<Func<int,int>> f2 = y => y * 2;
			Expression<Func<int,int>> f3 = y => y * 3;

			// Structural comparison; names of lambda parameters and block variables do not matter.
			Assert.That(f1.EqualsTo(f2), Is.True);
			Assert.That(f1.EqualsTo(f3), Is.False);

			// Constants of primitive types, strings, enums etc. are always compared by value.
			// compareConstantValues also compares other constants (closures, objects) by reference;
			// by default such constants are considered equal if their types are equal.
			var o1 = new object();
			var o2 = new object();

			Assert.That(Ex.Constant(o1).EqualsTo(Ex.Constant(o2)),                              Is.True);
			Assert.That(Ex.Constant(o1).EqualsTo(Ex.Constant(o2), compareConstantValues: true), Is.False);
		}
	}
}
