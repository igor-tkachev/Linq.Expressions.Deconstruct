using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

using static Linq.Expressions.Deconstruct.Expr;
using Ex = System.Linq.Expressions.Expression;

namespace Linq.Expressions.Deconstruct.Tests
{
	[TestFixture]
	public class StatementTests
	{
		class Item
		{
			public int          A { get; set; }
			public string?      B { get; set; }
			public List<int>    List { get; set; } = new();
		}

		static Expression Replace(Expression e)
		{
			return e is ConstantExpression { Value: 1 } ? Ex.Constant(100) : e;
		}

		static IEnumerable<int> IntConstants(Expression expr)
		{
			var list = new List<int>();

			expr.Visit(e =>
			{
				if (e is ConstantExpression { Value: int i })
					list.Add(i);
			});

			return list;
		}

		#region Samples

		static Expression MakeBlock(int value)
		{
			var v = Ex.Variable(typeof(int), "v");

			return Ex.Block(new[] { v }, Ex.Assign(v, Ex.Constant(value)), Ex.Add(v, Ex.Constant(2)));
		}

		static Expression MakeSwitch(int value)
		{
			var p = Ex.Parameter(typeof(int), "p");

			return Ex.Lambda(
				Ex.Switch(p, Ex.Constant("d"), Ex.SwitchCase(Ex.Constant("one"), Ex.Constant(value)), Ex.SwitchCase(Ex.Constant("two"), Ex.Constant(2), Ex.Constant(3))),
				p);
		}

		static Expression MakeTry(int value)
		{
			var e = Ex.Parameter(typeof(Exception), "e");

			return Ex.TryCatchFinally(
				Ex.Constant(value),
				Ex.Call(typeof(GC), nameof(GC.Collect), Type.EmptyTypes),
				Ex.Catch(e, Ex.Constant(-1), Ex.Equal(Ex.Property(e, nameof(Exception.HResult)), Ex.Constant(value))));
		}

		static Expression MakeLoop(int value)
		{
			var i     = Ex.Variable(typeof(int), "i");
			var @break = Ex.Label(typeof(int), "break");

			return Ex.Block(
				new[] { i },
				Ex.Assign(i, Ex.Constant(value)),
				Ex.Loop(
					Ex.IfThenElse(Ex.GreaterThan(i, Ex.Constant(10)), Ex.Break(@break, i), Ex.PreIncrementAssign(i)),
					@break));
		}

		static Expression MakeGoto(int value)
		{
			var target = Ex.Label(typeof(int), "end");

			return Ex.Block(Ex.Goto(target, Ex.Constant(value)), Ex.Label(target, Ex.Constant(0)));
		}

		static Expression MakeIndex(int value)
		{
			var arr = Ex.Parameter(typeof(int[]), "arr");
			var lst = Ex.Parameter(typeof(List<int>), "lst");

			return Ex.Lambda(
				Ex.Add(
					Ex.ArrayAccess(arr, Ex.Constant(value)),
					Ex.MakeIndex(lst, typeof(List<int>).GetProperty("Item"), new[] { Ex.Constant(2) })),
				arr, lst);
		}

		static Expression MakeMemberInit(int value)
		{
			var a = typeof(Item).GetProperty(nameof(Item.A))!;
			var b = typeof(Item).GetProperty(nameof(Item.B))!;
			var l = typeof(Item).GetProperty(nameof(Item.List))!;

			return Ex.MemberInit(
				Ex.New(typeof(Item)),
				Ex.Bind(a, Ex.Constant(value)),
				Ex.Bind(b, Ex.Constant("x")),
				Ex.ListBind(l, Ex.ElementInit(typeof(List<int>).GetMethod("Add")!, Ex.Constant(2))));
		}

		static Expression MakeListInit(int value)
		{
			return Ex.ListInit(
				Ex.New(typeof(List<int>)),
				Ex.ElementInit(typeof(List<int>).GetMethod("Add")!, Ex.Constant(value)),
				Ex.ElementInit(typeof(List<int>).GetMethod("Add")!, Ex.Constant(2)));
		}

		static readonly (string Name, Func<int,Expression> Make)[] _samples =
		{
			("Block",      MakeBlock),
			("Switch",     MakeSwitch),
			("Try",        MakeTry),
			("Loop",       MakeLoop),
			("Goto",       MakeGoto),
			("Index",      MakeIndex),
			("MemberInit", MakeMemberInit),
			("ListInit",   MakeListInit),
		};

		public static IEnumerable<TestCaseData> Samples()
		{
			foreach (var (name, make) in _samples)
				yield return new TestCaseData(make).SetName(name);
		}

		#endregion

		#region EqualsTo

		[TestCaseSource(nameof(Samples))]
		public void EqualsToEqualCopies(Func<int,Expression> make)
		{
			Assert.That(make(1).EqualsTo(make(1)), Is.True);
		}

		[TestCaseSource(nameof(Samples))]
		public void EqualsToDifferentConstant(Func<int,Expression> make)
		{
			Assert.That(make(1).EqualsTo(make(5)), Is.False);
		}

		[Test]
		public void EqualsToBlockDifferentShape()
		{
			var v = Ex.Variable(typeof(int), "v");

			Assert.That(Ex.Block(new[] { v }, Ex.Constant(1)).EqualsTo(Ex.Block(Ex.Constant(1))), Is.False);
			Assert.That(Ex.Block(Ex.Constant(1), Ex.Constant(2)).EqualsTo(Ex.Block(Ex.Constant(1))),       Is.False);
		}

		[Test]
		public void EqualsToSwitchDifferentShape()
		{
			var p = Ex.Parameter(typeof(int), "p");

			var s1 = Ex.Switch(p, Ex.Constant(0), Ex.SwitchCase(Ex.Constant(1), Ex.Constant(1)));
			var s2 = Ex.Switch(p, Ex.Constant(0), Ex.SwitchCase(Ex.Constant(1), Ex.Constant(1), Ex.Constant(2)));
			var s3 = Ex.Switch(p, Ex.Constant(0), Ex.SwitchCase(Ex.Constant(1), Ex.Constant(1)), Ex.SwitchCase(Ex.Constant(2), Ex.Constant(2)));
			var s4 = Ex.Switch(p, Ex.Constant(0), Ex.SwitchCase(Ex.Constant(9), Ex.Constant(1)));

			Assert.That(s1.EqualsTo(s2), Is.False);
			Assert.That(s1.EqualsTo(s3), Is.False);
			Assert.That(s1.EqualsTo(s4), Is.False);
		}

		[Test]
		public void EqualsToTryDifferentShape()
		{
			var e1 = Ex.Parameter(typeof(Exception), "e");
			var e2 = Ex.Parameter(typeof(InvalidOperationException), "e");

			var t1 = Ex.TryCatch(Ex.Constant(1), Ex.Catch(e1, Ex.Constant(2)));
			var t2 = Ex.TryCatch(Ex.Constant(1), Ex.Catch(e2, Ex.Constant(2)));
			var t3 = Ex.TryCatch(Ex.Constant(1), Ex.Catch(typeof(Exception), Ex.Constant(2)));
			var t4 = Ex.TryFinally(Ex.Constant(1), Ex.Empty());
			var t5 = Ex.TryFault(Ex.Constant(1), Ex.Empty());

			Assert.That(t1.EqualsTo(t2), Is.False);
			Assert.That(t1.EqualsTo(t3), Is.False);
			Assert.That(t4.EqualsTo(t5), Is.False);
			Assert.That(t4.EqualsTo(Ex.TryFinally(Ex.Constant(1), Ex.Empty())), Is.True);
			Assert.That(t5.EqualsTo(Ex.TryFault  (Ex.Constant(1), Ex.Empty())), Is.True);
		}

		[Test]
		public void EqualsToLoopLabels()
		{
			var l1 = Ex.Loop(Ex.Empty());
			var l2 = Ex.Loop(Ex.Empty());
			var l3 = Ex.Loop(Ex.Empty(), Ex.Label("brk"));

			Assert.That(l1.EqualsTo(l2), Is.True);
			Assert.That(l1.EqualsTo(l3), Is.False);
		}

		[Test]
		public void EqualsToGotoKindAndTarget()
		{
			var t1 = Ex.Label("a");
			var t2 = Ex.Label("b");

			Assert.That(Ex.Goto(t1).EqualsTo(Ex.Goto(Ex.Label("a"))), Is.True);
			Assert.That(Ex.Goto(t1).EqualsTo(Ex.Goto(t2)),            Is.False);
			Assert.That(Ex.Goto(t1).EqualsTo(Ex.Break(t1)),           Is.False);
		}

		[Test]
		public void EqualsToMemberInitBindings()
		{
			var a = typeof(Item).GetProperty(nameof(Item.A))!;
			var b = typeof(Item).GetProperty(nameof(Item.B))!;

			var m1 = Ex.MemberInit(Ex.New(typeof(Item)), Ex.Bind(a, Ex.Constant(1)));
			var m2 = Ex.MemberInit(Ex.New(typeof(Item)), Ex.Bind(b, Ex.Constant("1")));
			var m3 = Ex.MemberInit(Ex.New(typeof(Item)), Ex.Bind(a, Ex.Constant(1)), Ex.Bind(b, Ex.Constant("1")));

			Assert.That(m1.EqualsTo(m2), Is.False);
			Assert.That(m1.EqualsTo(m3), Is.False);
		}

		[Test]
		public void EqualsToIndexerDifference()
		{
			var arr = Ex.Parameter(typeof(int[]), "arr");

			Assert.That(Ex.ArrayAccess(arr, Ex.Constant(1)).EqualsTo(Ex.ArrayAccess(arr, Ex.Constant(2))), Is.False);
			Assert.That(Ex.ArrayAccess(arr, Ex.Constant(1)).EqualsTo(Ex.ArrayAccess(arr, Ex.Constant(1))), Is.True);
		}

		#endregion

		#region Transform

		[TestCaseSource(nameof(Samples))]
		public void TransformReplacesConstants(Func<int,Expression> make)
		{
			foreach (var forward in new[] { false, true })
			{
				var transformed = make(1).Transform(forward, Replace);
				var transformedEx = make(1).TransformEx(forward, e => e is Constant { Expr.Value : 1 } ? Ex.Constant(100) : e);

				Assert.That(transformed.EqualsTo(make(100)),   Is.True, $"Transform, forward={forward}");
				Assert.That(transformedEx.EqualsTo(make(100)), Is.True, $"TransformEx, forward={forward}");
			}
		}

		[TestCaseSource(nameof(Samples))]
		public void TransformWithoutChangesKeepsInstance(Func<int,Expression> make)
		{
			var expr = make(1);

			Assert.That(expr.Transform(e => e),  Is.SameAs(expr));
			Assert.That(expr.Transform(true, e => e), Is.SameAs(expr));
		}

		[Test]
		public void TransformSwitchKeepsComparison()
		{
			var p = Ex.Parameter(typeof(string), "p");
			var s = Ex.Switch(p, Ex.Constant(0), typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(string) }),
				Ex.SwitchCase(Ex.Constant(1), Ex.Constant("a")));

			var r = (SwitchExpression)s.Transform(e => e is ConstantExpression { Value: 1 } ? Ex.Constant(2) : e);

			Assert.That(r.Comparison, Is.EqualTo(s.Comparison));
			Assert.That(r.Cases[0].Body.EqualsTo(Ex.Constant(2)), Is.True);
		}

		[Test]
		public void TransformLoopKeepsLabels()
		{
			var @break    = Ex.Label(typeof(void), "brk");
			var @continue = Ex.Label(typeof(void), "cnt");
			var loop      = Ex.Loop(Ex.Block(Ex.Constant(1), Ex.Break(@break)), @break, @continue);

			var r = (LoopExpression)loop.Transform(Replace);

			Assert.That(r.BreakLabel,    Is.SameAs(@break));
			Assert.That(r.ContinueLabel, Is.SameAs(@continue));
			Assert.That(IntConstants(r), Is.EqualTo(new[] { 100 }));
		}

		[Test]
		public void TransformGotoKeepsTargetAndKind()
		{
			var target = Ex.Label(typeof(int), "t");
			var @goto  = Ex.Return(target, Ex.Constant(1));

			var r = (GotoExpression)@goto.Transform(Replace);

			Assert.That(r.Kind,   Is.EqualTo(GotoExpressionKind.Return));
			Assert.That(r.Target, Is.SameAs(target));
			Assert.That(r.Value.EqualsTo(Ex.Constant(100)), Is.True);
		}

		[Test]
		public void TransformBlockKeepsVariables()
		{
			var v     = Ex.Variable(typeof(int), "v");
			var block = Ex.Block(new[] { v }, Ex.Assign(v, Ex.Constant(1)), v);

			var r = (BlockExpression)block.Transform(Replace);

			Assert.That(r.Variables, Is.EqualTo(new[] { v }));
			Assert.That(IntConstants(r), Is.EqualTo(new[] { 100 }));
		}

		[Test]
		public void TransformTryKeepsHandlers()
		{
			var r = (TryExpression)MakeTry(1).Transform(Replace);

			Assert.That(r.Handlers.Count,       Is.EqualTo(1));
			Assert.That(r.Handlers[0].Variable, Is.Not.Null);
			Assert.That(r.Handlers[0].Filter,   Is.Not.Null);
			Assert.That(r.Finally,              Is.Not.Null);
			Assert.That(IntConstants(r),        Is.EquivalentTo(new[] { 100, 100, -1 }));
		}

		#endregion

		#region Visit

		[TestCaseSource(nameof(Samples))]
		public void VisitReachesAllConstants(Func<int,Expression> make)
		{
			var expr = make(7);

			var viaAction = new List<Expression>();
			var viaFunc   = new List<Expression>();
			var viaEx     = new List<Expr>();

			expr.Visit(viaAction.Add);
			expr.Visit(e => { viaFunc.Add(e); return true; });
			expr.VisitEx(viaEx.Add);

			Assert.That(viaAction.OfType<ConstantExpression>().Select(c => c.Value).Contains(7), Is.True);
			// Visit(Action) is post-order (children first), Visit(Func) is pre-order.
			Assert.That(viaAction.Last(), Is.SameAs(expr));
			Assert.That(viaFunc.First(),  Is.SameAs(expr));
			Assert.That(viaFunc,  Is.EquivalentTo(viaAction));
			Assert.That(viaEx.Select(e => (Expression)e), Is.EqualTo(viaAction));
			Assert.That(expr.Find(e => e is ConstantExpression { Value: 7 }), Is.Not.Null);
			Assert.That(expr.FindEx(e => e is Constant { Expr.Value: 7 }),    Is.Not.Null);
			Assert.That(expr.Find(e => e is ConstantExpression { Value: -77 }), Is.Null);
		}

		[Test]
		public void VisitFuncFalseSkipsChildren()
		{
			var expr    = Ex.Block(Ex.Add(Ex.Constant(1), Ex.Constant(2)), Ex.Constant(3));
			var visited = new List<ExpressionType>();

			expr.Visit(e =>
			{
				visited.Add(e.NodeType);
				return e.NodeType != ExpressionType.Add;
			});

			Assert.That(visited, Is.EqualTo(new[] { ExpressionType.Block, ExpressionType.Add, ExpressionType.Constant }));
		}

		[Test]
		public void VisitTryVisitsEveryPart()
		{
			var seen = new List<string>();

			MakeTry(7).Visit(e =>
			{
				if (e is ParameterExpression p)
					seen.Add(p.Name!);
				if (e is MethodCallExpression m)
					seen.Add(m.Method.Name);
			});

			Assert.That(seen, Does.Contain("Collect"));
			Assert.That(seen, Does.Contain("e"));
		}

		[Test]
		public void VisitMemberInitVisitsBindings()
		{
			Assert.That(IntConstants(MakeMemberInit(7)), Is.EquivalentTo(new[] { 7, 2 }));
		}

		[Test]
		public void VisitListInitVisitsInitializers()
		{
			Assert.That(IntConstants(MakeListInit(7)), Is.EquivalentTo(new[] { 7, 2 }));
		}

		[Test]
		public void VisitSwitchVisitsValueCasesAndDefault()
		{
			var strings = new List<string>();

			MakeSwitch(1).Visit(e =>
			{
				if (e is ConstantExpression { Value: string s })
					strings.Add(s);
			});

			Assert.That(strings, Is.EquivalentTo(new[] { "d", "one", "two" }));
			Assert.That(IntConstants(MakeSwitch(1)), Is.EquivalentTo(new[] { 1, 2, 3 }));
		}

		[Test]
		public void VisitLoopGotoIndex()
		{
			Assert.That(IntConstants(MakeLoop(7)),  Is.EquivalentTo(new[] { 7, 10 }));
			Assert.That(IntConstants(MakeGoto(7)),  Is.EquivalentTo(new[] { 7, 0 }));
			Assert.That(IntConstants(MakeIndex(7)), Is.EquivalentTo(new[] { 7, 2 }));
		}

		#endregion

		#region Pattern matching

		[Test]
		public void StatementPatterns()
		{
			Assert.That(MakeBlock(1).ToExpr() is Block(var vars, var exprs), Is.True);

			var (variables, expressions) = (MakeBlock(1).ToExpr() as Block)!.Expr is var b ? (b.Variables, b.Expressions) : default;

			Assert.That(variables.Count(), Is.EqualTo(1));
			Assert.That(expressions.Count(), Is.EqualTo(2));
			Assert.That(expressions.First().ToExpr() is Assign(Parameter("v"), Constant(1)), Is.True);
			Assert.That(expressions.Last().ToExpr()  is Add(Parameter("v"), Constant(2)),   Is.True);

			Assert.That(((Block)MakeGoto(1).ToExpr()).Expr.Expressions[0].ToExpr() is Goto({ Name : "end" }, Constant(1)), Is.True);
		}

		#endregion
	}
}
