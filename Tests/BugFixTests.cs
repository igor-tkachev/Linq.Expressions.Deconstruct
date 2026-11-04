using System;
using System.Linq.Expressions;

using NUnit.Framework;

using static Linq.Expressions.Deconstruct.Expr;
using Ex = System.Linq.Expressions.Expression;

namespace Linq.Expressions.Deconstruct.Tests
{
	[TestFixture]
	public class BugFixTests
	{
		#region Switch equality

		static SwitchExpression MakeSwitch(ParameterExpression p, int caseValue, int defaultValue)
		{
			return Ex.Switch(p, Ex.Constant(defaultValue), Ex.SwitchCase(Ex.Constant(caseValue + 100), Ex.Constant(caseValue)));
		}

		[Test]
		public void SwitchEqualsToDistinctInstances()
		{
			var p = Ex.Parameter(typeof(int), "p");

			Assert.That(MakeSwitch(p, 1, 0).EqualsTo(MakeSwitch(p, 1, 0)), Is.True);
		}

		[Test]
		public void SwitchEqualsToDifferentDefault()
		{
			var p = Ex.Parameter(typeof(int), "p");

			Assert.That(MakeSwitch(p, 1, 0).EqualsTo(MakeSwitch(p, 1, 5)), Is.False);
		}

		[Test]
		public void SwitchEqualsToDifferentSwitchValue()
		{
			var p1 = Ex.Parameter(typeof(int), "p1");
			var p2 = Ex.Parameter(typeof(int), "p2");

			Assert.That(MakeSwitch(p1, 1, 0).EqualsTo(MakeSwitch(p2, 1, 0)), Is.False);
		}

		[Test]
		public void SwitchEqualsToDifferentCase()
		{
			var p = Ex.Parameter(typeof(int), "p");

			Assert.That(MakeSwitch(p, 1, 0).EqualsTo(MakeSwitch(p, 2, 0)), Is.False);
		}

		[Test]
		public void SwitchEqualsToWithoutDefault()
		{
			var p = Ex.Parameter(typeof(int), "p");

			var s1 = Ex.Switch(typeof(void), p, null, null, Ex.SwitchCase(Ex.Empty(), Ex.Constant(1)));
			var s2 = Ex.Switch(typeof(void), p, null, null, Ex.SwitchCase(Ex.Empty(), Ex.Constant(1)));

			Assert.That(s1.EqualsTo(s2), Is.True);
		}

		#endregion

		#region Constant

		[Test]
		public void ConstantInt64OutOfIntRange()
		{
			Assert.That(Ex.Constant(-5_000_000_000L).ToExpr() is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant((long)int.MinValue - 1).ToExpr() is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant((long)int.MaxValue + 1).ToExpr() is Constant(_, int _), Is.False);
		}

		[Test]
		public void ConstantInt64InIntRange()
		{
			Assert.That(Ex.Constant(-5L).ToExpr()                 is Constant(_, -5),            Is.True);
			Assert.That(Ex.Constant((long)int.MinValue).ToExpr() is Constant(_, int.MinValue), Is.True);
			Assert.That(Ex.Constant((long)int.MaxValue).ToExpr() is Constant(_, int.MaxValue), Is.True);
		}

		[Test]
		public void ConstantOtherNumericTypes()
		{
			Assert.That(Ex.Constant((ulong)int.MaxValue + 1).ToExpr() is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant((uint)int.MaxValue + 1).ToExpr()  is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant(ulong.MaxValue).ToExpr()          is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant(-5_000_000_000d).ToExpr()         is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant(2147483648f).ToExpr()             is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant(-5_000_000_000m).ToExpr()         is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant(1.5).ToExpr()                     is Constant(_, int _), Is.False);
			Assert.That(Ex.Constant(-5.0).ToExpr()                    is Constant(_, -5),    Is.True);
			Assert.That(Ex.Constant(5UL).ToExpr()                     is Constant(_, 5),     Is.True);
			Assert.That(Ex.Constant(int.MinValue * 1.0f).ToExpr()     is Constant(_, int.MinValue), Is.True);
		}

		#endregion

		#region Transform lambda

		[Test]
		public void TransformKeepsLambdaNameAndTailCall()
		{
			var p      = Ex.Parameter(typeof(int), "p");
			var lambda = Ex.Lambda<Func<int,int>>(Ex.Add(p, Ex.Constant(1)), "myLambda", true, new[] { p });

			var r1 = lambda.Transform(e => e is ConstantExpression ? Ex.Constant(2) : e);
			var r2 = lambda.TransformEx(e => e is Constant ? Ex.Constant(2) : e);
			var r3 = lambda.Transform(true, e => e is ConstantExpression ? Ex.Constant(2) : e);

			foreach (var r in new[] { r1, r2, r3 })
			{
				Assert.That(r,          Is.Not.SameAs(lambda));
				Assert.That(r.Name,     Is.EqualTo("myLambda"));
				Assert.That(r.TailCall, Is.True);
				Assert.That(r.Body.EqualsTo(Ex.Add(p, Ex.Constant(2))), Is.True);
			}
		}

		[Test]
		public void TransformKeepsLambdaNameOnParameterChange()
		{
			var p      = Ex.Parameter(typeof(int), "p");
			var lambda = Ex.Lambda(Ex.Add(p, Ex.Constant(1)), "named", true, new[] { p });

			var r = (LambdaExpression)((Expression)lambda).Transform(e => e is ConstantExpression ? Ex.Constant(3) : e);

			Assert.That(r.Name,     Is.EqualTo("named"));
			Assert.That(r.TailCall, Is.True);
		}

		#endregion

		#region Parameter equality

		[Test]
		public void LambdaParametersMatchedByPosition()
		{
			var a = Ex.Parameter(typeof(int), "x");
			var b = Ex.Parameter(typeof(int), "x");

			var l1 = Ex.Lambda<Func<int,int,int>>(a, a, b);
			var l2 = Ex.Lambda<Func<int,int,int>>(b, a, b);

			Assert.That(l1.EqualsTo(l2), Is.False);
			Assert.That(l1.EqualsTo(Ex.Lambda<Func<int,int,int>>(a, a, b)), Is.True);
		}

		[Test]
		public void LambdaParameterNamesAreIrrelevant()
		{
			Expression<Func<int,int,int>> l1 = (x, y) => x * 2 + y;
			Expression<Func<int,int,int>> l2 = (a, b) => a * 2 + b;
			Expression<Func<int,int,int>> l3 = (a, b) => b * 2 + a;

			Assert.That(l1.EqualsTo(l2), Is.True);
			Assert.That(l1.EqualsTo(l3), Is.False);
		}

		[Test]
		public void NestedLambdaParameters()
		{
			Expression<Func<int,Func<int,int>>> l1 = x => y => x + y;
			Expression<Func<int,Func<int,int>>> l2 = a => b => a + b;
			Expression<Func<int,Func<int,int>>> l3 = a => b => b + a;

			Assert.That(l1.EqualsTo(l2), Is.True);
			Assert.That(l1.EqualsTo(l3), Is.False);
		}

		[Test]
		public void FreeParametersAreComparedByReference()
		{
			var x1 = Ex.Parameter(typeof(int), "x");
			var x2 = Ex.Parameter(typeof(int), "x");

			Assert.That(Ex.Add(x1, x1).EqualsTo(Ex.Add(x1, x1)), Is.True);
			Assert.That(Ex.Add(x1, x1).EqualsTo(Ex.Add(x2, x2)), Is.False);
			Assert.That(Ex.Add(x1, x2).EqualsTo(Ex.Add(x1, x1)), Is.False);
		}

		[Test]
		public void LambdaParameterTypesMustMatch()
		{
			Expression<Func<int,object>>  l1 = x => x;
			Expression<Func<long,object>> l2 = x => x;

			Assert.That(((Expression)l1).EqualsTo(l2), Is.False);
		}

		[Test]
		public void BlockVariablesMatchedByPosition()
		{
			Expression Make(string n1, string n2, bool second)
			{
				var v1 = Ex.Variable(typeof(int), n1);
				var v2 = Ex.Variable(typeof(int), n2);

				return Ex.Block(new[] { v1, v2 },
					Ex.Assign(v1, Ex.Constant(1)),
					Ex.Assign(v2, Ex.Constant(2)),
					second ? v2 : v1);
			}

			Assert.That(Make("a", "b", false).EqualsTo(Make("c", "d", false)), Is.True);
			Assert.That(Make("a", "a", false).EqualsTo(Make("a", "a", true)),  Is.False);
		}

		[Test]
		public void CatchVariableMatchedByPosition()
		{
			Expression Make(string name)
			{
				var ex = Ex.Parameter(typeof(Exception), name);

				return Ex.TryCatch(Ex.Constant(1), Ex.Catch(ex, Ex.Property(ex, nameof(Exception.HResult))));
			}

			Assert.That(Make("e1").EqualsTo(Make("e2")), Is.True);
		}

		[Test]
		public void ParameterScopeIsRestored()
		{
			var p1 = Ex.Parameter(typeof(int), "p");
			var p2 = Ex.Parameter(typeof(int), "p");

			// p1 is declared by the first lambda, but is a free variable of the second one.
			var inner1 = Ex.Lambda<Func<int>>(p1);
			var inner2 = Ex.Lambda<Func<int>>(p2);

			var l1 = Ex.Lambda<Func<int,Func<int>>>(inner1, p1);
			var l2 = Ex.Lambda<Func<int,Func<int>>>(inner2, p1);

			Assert.That(l1.EqualsTo(l2), Is.False);
		}

		#endregion

		#region Extension and DebugInfo

		class MyExtension : Expression
		{
			public override ExpressionType NodeType => ExpressionType.Extension;
			public override Type           Type     => typeof(int);
			public override bool           CanReduce => false;

			protected override Expression VisitChildren(ExpressionVisitor visitor)
			{
				return this;
			}
		}

		[Test]
		public void ExtensionNodesAreEqualOnlyToThemselves()
		{
			var e1 = new MyExtension();
			var e2 = new MyExtension();

			Assert.That(e1.EqualsTo(e1), Is.True);
			Assert.That(e1.EqualsTo(e2), Is.False);
			Assert.That(Ex.Add(e1, Ex.Constant(1)).EqualsTo(Ex.Add(e2, Ex.Constant(1))), Is.False);
		}

		[Test]
		public void DebugInfoIsComparedByContent()
		{
			var doc = Ex.SymbolDocument("a.cs");

			var d1 = Ex.DebugInfo(doc, 1, 1, 2, 2);
			var d2 = Ex.DebugInfo(doc, 1, 1, 2, 2);
			var d3 = Ex.DebugInfo(doc, 5, 1, 6, 2);
			var d4 = Ex.DebugInfo(Ex.SymbolDocument("b.cs"), 1, 1, 2, 2);
			var d5 = Ex.ClearDebugInfo(doc);

			Assert.That(d1.EqualsTo(d2), Is.True);
			Assert.That(d1.EqualsTo(d3), Is.False);
			Assert.That(d1.EqualsTo(d4), Is.False);
			Assert.That(d1.EqualsTo(d5), Is.False);
		}

		#endregion

		#region Info pass-through

		[Test]
		public void GotoValueUsesLambdaParameters()
		{
			LambdaExpression Make(string name)
			{
				var p      = Ex.Parameter(typeof(int), name);
				var target = Ex.Label(typeof(int), "L");

				return Ex.Lambda(Ex.Block(Ex.Return(target, p), Ex.Label(target, Ex.Constant(0))), p);
			}

			Assert.That(Make("a").EqualsTo(Make("b")), Is.True);
		}

		[Test]
		public void LabelDefaultValueUsesLambdaParameters()
		{
			LambdaExpression Make(string name)
			{
				var p      = Ex.Parameter(typeof(int), name);
				var target = Ex.Label(typeof(int), "L");

				return Ex.Lambda(Ex.Label(target, p), p);
			}

			Assert.That(Make("a").EqualsTo(Make("b")), Is.True);
		}

		[Test]
		public void LoopBodyUsesLambdaParameters()
		{
			LambdaExpression Make(string name)
			{
				var p      = Ex.Parameter(typeof(int), name);
				var @break = Ex.Label(typeof(int), "brk");

				return Ex.Lambda(Ex.Loop(Ex.Break(@break, p), @break), p);
			}

			Assert.That(Make("a").EqualsTo(Make("b")), Is.True);
		}

		[Test]
		public void IndexObjectUsesLambdaParameters()
		{
			LambdaExpression Make(string name)
			{
				var p = Ex.Parameter(typeof(int[]), name);

				return Ex.Lambda(Ex.ArrayAccess(p, Ex.Constant(0)), p);
			}

			Assert.That(Make("a").EqualsTo(Make("b")), Is.True);
		}

		[Test]
		public void IndexerObjectUsesLambdaParameters()
		{
			LambdaExpression Make(string name)
			{
				var p = Ex.Parameter(typeof(System.Collections.Generic.List<int>), name);

				return Ex.Lambda(Ex.MakeIndex(p, p.Type.GetProperty("Item"), new[] { Ex.Constant(0) }), p);
			}

			Assert.That(Make("a").EqualsTo(Make("b")), Is.True);
		}

		[Test]
		public void GotoValueHonoursCompareConstantValues()
		{
			Expression Make(object value)
			{
				var target = Ex.Label(typeof(object), "L");

				return Ex.Block(Ex.Return(target, Ex.Constant(value)), Ex.Label(target, Ex.Constant(null)));
			}

			Assert.That(Make(new object()).EqualsTo(Make(new object()), compareConstantValues: true),  Is.False);
			Assert.That(Make(new object()).EqualsTo(Make(new object()), compareConstantValues: false), Is.True);
		}

		#endregion
	}
}
