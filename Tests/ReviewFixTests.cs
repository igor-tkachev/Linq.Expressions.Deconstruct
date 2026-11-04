using System;
using System.Diagnostics;
using System.Linq.Expressions;

using NUnit.Framework;

using Ex = System.Linq.Expressions.Expression;

namespace Linq.Expressions.Deconstruct.Tests
{
	[TestFixture]
	public class ReviewFixTests
	{
		#region Reused parameter objects in nested scopes

		[Test]
		public void NestedLambdaReusingParameterIsSymmetric()
		{
			var x = Ex.Parameter(typeof(int), "x");
			var a = Ex.Parameter(typeof(int), "a");
			var b = Ex.Parameter(typeof(int), "b");

			// x => (x => x)
			var l1 = Ex.Lambda(Ex.Lambda(x, x), x);
			// a => (b => a)
			var l2 = Ex.Lambda(Ex.Lambda(a, b), a);

			Assert.That(l1.EqualsTo(l2), Is.False);
			Assert.That(l2.EqualsTo(l1), Is.False);

			// a => (b => b) is the same as x => (x => x)
			var l3 = Ex.Lambda(Ex.Lambda(b, b), a);

			Assert.That(l1.EqualsTo(l3), Is.True);
			Assert.That(l3.EqualsTo(l1), Is.True);
		}

		[Test]
		public void NestedBlockReusingVariableIsSymmetric()
		{
			var x = Ex.Variable(typeof(int), "x");
			var a = Ex.Variable(typeof(int), "a");
			var b = Ex.Variable(typeof(int), "b");

			var b1 = Ex.Block(new[] { x }, Ex.Assign(x, Ex.Constant(1)), Ex.Block(new[] { x }, Ex.Assign(x, Ex.Constant(2)), x));
			var b2 = Ex.Block(new[] { a }, Ex.Assign(a, Ex.Constant(1)), Ex.Block(new[] { b }, Ex.Assign(b, Ex.Constant(2)), a));
			var b3 = Ex.Block(new[] { a }, Ex.Assign(a, Ex.Constant(1)), Ex.Block(new[] { b }, Ex.Assign(b, Ex.Constant(2)), b));

			Assert.That(b1.EqualsTo(b2), Is.False);
			Assert.That(b2.EqualsTo(b1), Is.False);
			Assert.That(b1.EqualsTo(b3), Is.True);
			Assert.That(b3.EqualsTo(b1), Is.True);
		}

		#endregion

		#region Shared subtrees

		static Expression MakeDag(Expression leaf, int depth)
		{
			var n = leaf;

			for (var i = 0; i < depth; i++)
				n = Ex.Add(n, n);

			return n;
		}

		[Test]
		public void SharedSubtreesAreComparedQuickly()
		{
			var x    = Ex.Parameter(typeof(int), "x");
			var body = MakeDag(x, 27);

			var l1 = Ex.Lambda(body, x);
			var l2 = Ex.Lambda(body, x);

			var sw = Stopwatch.StartNew();

			Assert.That(l1.EqualsTo(l2), Is.True);
			Assert.That(l1.EqualsTo(l1), Is.True);

			sw.Stop();

			Assert.That(sw.ElapsedMilliseconds, Is.LessThan(1000));
		}

		[Test]
		public void SharedSubtreeUnderRenamedParametersIsNotEqual()
		{
			var a = Ex.Parameter(typeof(int), "a");
			var b = Ex.Parameter(typeof(int), "b");

			var shared = Ex.Subtract(a, b);

			// (a, b) => a - b  versus  (b, a) => a - b: the same node means different things.
			var l1 = Ex.Lambda(shared, a, b);
			var l2 = Ex.Lambda(shared, b, a);

			Assert.That(l1.EqualsTo(l2), Is.False);
			Assert.That(l2.EqualsTo(l1), Is.False);
			Assert.That(l1.EqualsTo(Ex.Lambda(shared, a, b)), Is.True);
		}

		#endregion

		#region Extension

		class EqExtension : Expression
		{
			readonly int _key;

			public EqExtension(int key)
			{
				_key = key;
			}

			public override ExpressionType NodeType  => ExpressionType.Extension;
			public override Type           Type      => typeof(int);
			public override bool           CanReduce => false;

			public override bool Equals(object? obj)
			{
				return obj is EqExtension other && other._key == _key;
			}

			public override int GetHashCode()
			{
				return _key;
			}

			protected override Expression VisitChildren(ExpressionVisitor visitor)
			{
				return this;
			}
		}

		class OtherExtension : EqExtension
		{
			public OtherExtension(int key) : base(key) { }
		}

		[Test]
		public void ExtensionNodesUseEquals()
		{
			Assert.That(new EqExtension(1).EqualsTo(new EqExtension(1)), Is.True);
			Assert.That(new EqExtension(1).EqualsTo(new EqExtension(2)), Is.False);
			Assert.That(new EqExtension(1).EqualsTo(new OtherExtension(1)), Is.False);
		}

		#endregion

		#region Labels

		[Test]
		public void UnnamedLabelsArePairedByFirstOccurrence()
		{
			Expression Make(LabelTarget first, LabelTarget second)
			{
				return Ex.Block(
					Ex.Goto(first),
					Ex.Label(first),
					Ex.Goto(second),
					Ex.Label(second));
			}

			var a1 = Ex.Label();
			var a2 = Ex.Label();
			var b1 = Ex.Label();
			var b2 = Ex.Label();

			// jump to the first label twice versus jump to the first and to the second
			var same  = Ex.Block(Ex.Goto(a1), Ex.Label(a1), Ex.Goto(a1), Ex.Label(a2));
			var other = Ex.Block(Ex.Goto(b1), Ex.Label(b1), Ex.Goto(b2), Ex.Label(b2));

			Assert.That(same.EqualsTo(other), Is.False);
			Assert.That(other.EqualsTo(same), Is.False);
			Assert.That(Make(a1, a2).EqualsTo(Make(b1, b2)), Is.True);
		}

		[Test]
		public void LoopLabelsArePaired()
		{
			var brk1 = Ex.Label();
			var brk2 = Ex.Label();

			var l1 = Ex.Loop(Ex.Break(brk1), brk1);
			var l2 = Ex.Loop(Ex.Break(brk2), brk2);
			var l3 = Ex.Loop(Ex.Break(brk2), brk1);

			Assert.That(l1.EqualsTo(l2), Is.True);
			Assert.That(l1.EqualsTo(l3), Is.False);
		}

		#endregion

		#region Transform replacing a parameter

		[Test]
		public void TransformReplacingLambdaParameterWithNonParameter()
		{
			var x      = Ex.Parameter(typeof(int), "x");
			var lambda = Ex.Lambda<Func<int,int>>(Ex.Add(x, Ex.Constant(1)), x);

			var r = lambda.Transform(e => e == x ? Ex.Constant(5) : e);

			Assert.That(r.Parameters, Is.EqualTo(new[] { x }));
			Assert.That(r.Body.EqualsTo(Ex.Add(Ex.Constant(5), Ex.Constant(1))), Is.True);

			var rf = lambda.Transform(true, e => e == x ? Ex.Constant(5) : e);

			Assert.That(rf.Parameters, Is.EqualTo(new[] { x }));
		}

		[Test]
		public void TransformReplacingBlockAndCatchVariablesWithNonParameter()
		{
			var v     = Ex.Variable(typeof(int), "v");
			var block = Ex.Block(new[] { v }, Ex.Add(v, Ex.Constant(1)));

			var r = (BlockExpression)block.Transform(e => e == v ? Ex.Constant(5) : e);

			Assert.That(r.Variables, Is.EqualTo(new[] { v }));

			var ex  = Ex.Parameter(typeof(Exception), "ex");
			var try_ = Ex.TryCatch(Ex.Constant(1), Ex.Catch(ex, Ex.Constant(2)));

			var t = (TryExpression)try_.Transform(e => e == ex ? Ex.Constant(null, typeof(Exception)) : e);

			Assert.That(t.Handlers[0].Variable, Is.SameAs(ex));
		}

		#endregion

		#region DebugInfo

		[Test]
		public void DebugInfoComparesDocumentDetails()
		{
			var lang1  = Guid.NewGuid();
			var lang2  = Guid.NewGuid();
			var vendor = Guid.NewGuid();
			var type   = Guid.NewGuid();

			Expression Make(Guid language, Guid languageVendor, Guid documentType)
			{
				return Ex.DebugInfo(Ex.SymbolDocument("a.cs", language, languageVendor, documentType), 1, 1, 2, 2);
			}

			Assert.That(Make(lang1, vendor, type).EqualsTo(Make(lang1, vendor, type)),        Is.True);
			Assert.That(Make(lang1, vendor, type).EqualsTo(Make(lang2, vendor, type)),        Is.False);
			Assert.That(Make(lang1, vendor, type).EqualsTo(Make(lang1, Guid.NewGuid(), type)), Is.False);
			Assert.That(Make(lang1, vendor, type).EqualsTo(Make(lang1, vendor, Guid.NewGuid())), Is.False);
		}

		#endregion

		#region EqualsToOptions

		[Test]
		public void OptionsFreeParametersByName()
		{
			var x1 = Ex.Parameter(typeof(int), "x");
			var x2 = Ex.Parameter(typeof(int), "x");
			var y  = Ex.Parameter(typeof(int), "y");
			var l1 = Ex.Parameter(typeof(long), "x");

			Assert.That(Ex.Add(x1, x1).EqualsTo(Ex.Add(x2, x2)), Is.False);
			Assert.That(Ex.Add(x1, x1).EqualsTo(Ex.Add(x2, x2), EqualsToOptions.FreeParametersByName), Is.True);
			Assert.That(Ex.Add(x1, x1).EqualsTo(Ex.Add(y,  y),  EqualsToOptions.FreeParametersByName), Is.False);
			Assert.That(Ex.Convert(x1, typeof(long)).EqualsTo(Ex.Convert(l1, typeof(long)), EqualsToOptions.FreeParametersByName), Is.False);

			// the lambda-body fragments case
			Expression<Func<int,int>> f1 = a => a + 1;
			Expression<Func<int,int>> f2 = a => a + 1;

			Assert.That(f1.Body.EqualsTo(f2.Body), Is.False);
			Assert.That(f1.Body.EqualsTo(f2.Body, EqualsToOptions.FreeParametersByName), Is.True);

			// a declared parameter never matches a free one by name
			var declared = Ex.Lambda(x1, x1);
			var free     = Ex.Lambda(x2, y);

			Assert.That(declared.EqualsTo(free, EqualsToOptions.FreeParametersByName), Is.False);
		}

		[Test]
		public void OptionsParameterNamesMatter()
		{
			Expression<Func<int,int>> f1 = a => a * 2;
			Expression<Func<int,int>> f2 = b => b * 2;
			Expression<Func<int,int>> f3 = a => a * 2;

			Assert.That(f1.EqualsTo(f2), Is.True);
			Assert.That(f1.EqualsTo(f2, EqualsToOptions.ParameterNamesMatter), Is.False);
			Assert.That(f1.EqualsTo(f3, EqualsToOptions.ParameterNamesMatter), Is.True);

			var v1 = Ex.Variable(typeof(int), "v");
			var v2 = Ex.Variable(typeof(int), "w");

			Assert.That(Ex.Block(new[] { v1 }, v1).EqualsTo(Ex.Block(new[] { v2 }, v2)), Is.True);
			Assert.That(Ex.Block(new[] { v1 }, v1).EqualsTo(Ex.Block(new[] { v2 }, v2), EqualsToOptions.ParameterNamesMatter), Is.False);
		}

		[Test]
		public void OptionsCompareConstantValues()
		{
			var o1 = new object();
			var o2 = new object();

			Assert.That(Ex.Constant(o1).EqualsTo(Ex.Constant(o2), EqualsToOptions.None),                  Is.True);
			Assert.That(Ex.Constant(o1).EqualsTo(Ex.Constant(o2), EqualsToOptions.CompareConstantValues), Is.False);
			Assert.That(Ex.Constant(o1).EqualsTo(Ex.Constant(o2), compareConstantValues: true),           Is.False);

			Expression<Func<object>> l1 = () => o1;
			Expression<Func<object>> l2 = () => o2;

			Assert.That(l1.EqualsTo(l2, EqualsToOptions.CompareConstantValues), Is.False);
		}

		#endregion
	}
}
