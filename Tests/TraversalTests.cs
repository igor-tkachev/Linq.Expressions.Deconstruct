using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;

using Ex = System.Linq.Expressions.Expression;

namespace Linq.Expressions.Deconstruct.Tests
{
	[TestFixture]
	public class TraversalTests
	{
		static SwitchExpression MakeSwitch()
		{
			return Ex.Switch(Ex.Constant(0), Ex.Constant(-1),
				Ex.SwitchCase(Ex.Add(Ex.Constant(10), Ex.Constant(11)), Ex.Constant(1), Ex.Constant(2)),
				Ex.SwitchCase(Ex.Add(Ex.Constant(20), Ex.Constant(21)), Ex.Constant(3), Ex.Constant(4)));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void IdentityTransformPreservesCollectionTrees(bool forward)
		{
			var parameter = Ex.Parameter(typeof(int), "value");
			var variable  = Ex.Variable(typeof(int), "local");
			var exception = Ex.Parameter(typeof(Exception), "error");
			Expression<Func<List<int>>> list = () => new List<int> { 1, 2, 3 };
			Expression<Func<List<int>>> member = () => new List<int> { Capacity = 4 };
			var tree = Ex.Lambda(Ex.Block(new[] { variable },
				Ex.NewArrayInit(typeof(int), Ex.Constant(1), Ex.Constant(2)),
				Ex.NewArrayInit(typeof(int)),
				list.Body, member.Body,
				Ex.TryCatch(Ex.Constant(1), Ex.Catch(exception, Ex.Constant(2))),
				MakeSwitch()), parameter);

			var result = tree.Transform(forward, static e => e);

			Assert.That(result, Is.SameAs(tree));
			Assert.That(result.Parameters, Is.SameAs(tree.Parameters));
			Assert.That(((BlockExpression)result.Body).Expressions, Is.SameAs(((BlockExpression)tree.Body).Expressions));
		}

		[TestCase(0, false)]
		[TestCase(1, false)]
		[TestCase(2, false)]
		[TestCase(0, true)]
		[TestCase(1, true)]
		[TestCase(2, true)]
		public void TransformCopiesPrefixAndVisitsEachElementOnce(int changedIndex, bool forward)
		{
			var items = new[] { Ex.Constant(1), Ex.Constant(2), Ex.Constant(3) };
			var tree = Ex.NewArrayInit(typeof(int), items);
			var replacement = Ex.Constant(42);
			var visited = new List<Expression>();

			var result = (NewArrayExpression)tree.Transform(forward, e =>
			{
				if (e is ConstantExpression)
					visited.Add(e);
				return ReferenceEquals(e, items[changedIndex]) ? replacement : e;
			});

			Assert.That(result, Is.Not.SameAs(tree));
			Assert.That(visited, Is.EqualTo(items));
			for (var i = 0; i < items.Length; i++)
			{
				Assert.That(result.Expressions[i], Is.SameAs(i == changedIndex ? replacement : items[i]));
				Assert.That(tree.Expressions[i], Is.SameAs(items[i]));
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void TransformPreservesUnchangedCasesAndParameters(bool forward)
		{
			var tree = MakeSwitch();
			var replacement = Ex.Constant(42);
			var result = (SwitchExpression)tree.Transform(forward, e => ReferenceEquals(e, tree.Cases[1].Body) ? replacement : e);

			Assert.That(result.Cases[0], Is.SameAs(tree.Cases[0]));
			Assert.That(result.Cases[1].Body, Is.SameAs(replacement));
			Assert.That(result.Cases[1].TestValues[0], Is.SameAs(tree.Cases[1].TestValues[0]));
			Assert.That(result.Cases[1].TestValues[1], Is.SameAs(tree.Cases[1].TestValues[1]));

			var first  = Ex.Parameter(typeof(int), "first");
			var second = Ex.Parameter(typeof(int), "second");
			var third  = Ex.Parameter(typeof(int), "third");
			var updated = Ex.Parameter(typeof(int), "updated");
			var lambda = Ex.Lambda(second, first, second, third);
			var transformed = lambda.Transform(forward, e => ReferenceEquals(e, second) ? updated : e);

			Assert.That(transformed.Parameters[0], Is.SameAs(first));
			Assert.That(transformed.Parameters[1], Is.SameAs(updated));
			Assert.That(transformed.Parameters[2], Is.SameAs(third));
			Assert.That(transformed.Body, Is.SameAs(updated));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void FindSwitchStopsAtTestValueOrCaseBody(bool findBody)
		{
			var tree = MakeSwitch();
			var firstBody = (BinaryExpression)tree.Cases[0].Body;
			var target = findBody ? tree.Cases[1].Body : tree.Cases[1].TestValues[1];
			var visited = new List<Expression>();
			var expected = new List<Expression>
			{
				tree, tree.SwitchValue,
				tree.Cases[0].TestValues[0], tree.Cases[0].TestValues[1],
				firstBody, firstBody.Left, firstBody.Right,
				tree.Cases[1].TestValues[0], tree.Cases[1].TestValues[1]
			};
			if (findBody)
				expected.Add(target);

			var found = tree.Find(e =>
			{
				visited.Add(e);
				return ReferenceEquals(e, target);
			});

			Assert.That(found, Is.SameAs(target));
			Assert.That(visited, Is.EqualTo(expected));
			Assert.That(tree.Find(static e => false), Is.Null);
		}

		[Test]
		public void VisitActionPreservesPostOrder()
		{
			var tree = MakeSwitch();
			var first = (BinaryExpression)tree.Cases[0].Body;
			var second = (BinaryExpression)tree.Cases[1].Body;
			var visited = new List<Expression>();

			tree.Visit((Action<Expression>)visited.Add);

			Assert.That(visited, Is.EqualTo(new Expression[]
			{
				tree.SwitchValue,
				tree.Cases[0].TestValues[0], tree.Cases[0].TestValues[1], first.Left, first.Right, first,
				tree.Cases[1].TestValues[0], tree.Cases[1].TestValues[1], second.Left, second.Right, second,
				tree.DefaultBody!, tree
			}));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void VisitPredicatePreservesPreOrderAndPrunesOnlyChildren(bool skipFirstBody)
		{
			var tree = MakeSwitch();
			var first = (BinaryExpression)tree.Cases[0].Body;
			var second = (BinaryExpression)tree.Cases[1].Body;
			var visited = new List<Expression>();
			var expected = new List<Expression>
			{
				tree, tree.SwitchValue, tree.Cases[0].TestValues[0], tree.Cases[0].TestValues[1], first
			};
			if (!skipFirstBody)
			{
				expected.Add(first.Left);
				expected.Add(first.Right);
			}
			expected.AddRange(new Expression[]
			{
				tree.Cases[1].TestValues[0], tree.Cases[1].TestValues[1], second, second.Left, second.Right,
				tree.DefaultBody!
			});

			tree.Visit(e =>
			{
				visited.Add(e);
				return !skipFirstBody || !ReferenceEquals(e, first);
			});

			Assert.That(visited, Is.EqualTo(expected));
		}
	}
}
