# Linq.Expressions.Deconstruct

[![CI](https://github.com/igor-tkachev/Linq.Expressions.Deconstruct/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/igor-tkachev/Linq.Expressions.Deconstruct/actions/workflows/dotnet.yml) [![NuGet](https://img.shields.io/nuget/v/Linq.Expressions.Deconstruct.svg)](https://www.nuget.org/packages/Linq.Expressions.Deconstruct/)

Pattern matching support for `System.Linq.Expressions`: every expression node can be wrapped into an `Expr`
object that has a `Deconstruct` method, so expression trees can be matched with C# recursive patterns.
On top of that the package provides `Visit`, `Find`, `Transform` and a structural `EqualsTo`.

```c#
using Linq.Expressions.Deconstruct;

using static Linq.Expressions.Deconstruct.Expr;
using Ex = System.Linq.Expressions.Expression;
```

## Pattern matching

```c#
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
```

## Transforming

`TransformEx` passes wrapped `Expr` nodes to the function; `Transform` passes plain `Expression` nodes.

```c#
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
```

`Constant` deconstructs into `(value, intValue)` or `(value, intValue, type)`. `intValue` is not null
for any integer or floating point constant whose value is an integer that fits into `int`,
so `Constant(_, 0)` matches `0`, `0L`, `0.0` and `0m`, while `Constant(0)` matches only a boxed `int` zero.

## API

All methods are extension methods of `Expression` (`Linq.Expressions.Deconstruct.Extensions`).

### ToExpr

`expression.ToExpr()` wraps an `Expression` into the matching `Expr.<NodeType>` class
(`Expr.Add`, `Expr.Lambda`, `Expr.Constant`, ...) and returns `null` for `null`.
`Expr` converts to `Expression` and back implicitly. Children of a node are wrapped lazily when they are first accessed
(by `Deconstruct` or by the property of the same name).

```c#
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
```

### Visit and Find

* `Visit(Action<Expression>)` and `VisitEx(Action<Expr>)` call the action for every node, children first (post-order).
* `Visit(Func<Expression, bool>)` calls the function for a node before its children (pre-order).
  Return `true` to visit the children of the node, `false` to skip them.
* `Find(Func<Expression, bool>)` and `FindEx(Func<Expr, bool>)` return the first node (in pre-order)
  for which the function returns `true`, or `null`. `Find(Expression)` searches for a given node.

```c#
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
```

### Transform

`Transform` and `TransformEx` rebuild the tree: the function is called for every node and returns the node
to use instead of it (return the argument to keep it). Nodes whose children did not change are not recreated,
so a transformation that changes nothing returns the original instance. Lambdas keep their name and `TailCall`.

* By default (`forward: false`) the tree is processed post-order: the function is called for a node after its children,
  and sees the already transformed children.
* With `forward: true` the function is called for a node before its children. If it returns a different node,
  that node is used as is and its children are not visited; otherwise the children are transformed.

```c#
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
```

### EqualsTo

`expr1.EqualsTo(expr2)` compares two expression trees structurally.

* Parameters declared by lambdas, blocks (variables) and catch blocks are matched by position, not by name:
  `x => x * 2` equals `y => y * 2`, but `(a, b) => a` does not equal `(a, b) => b`.
  A parameter that is not declared inside the compared trees is equal only to itself.
* Label targets (`goto`, `label`, `loop`) are paired by their first occurrence in the same way.
* Constants of primitive types, strings, enums, etc. are always compared by value; constants that wrap
  queries or sequences are compared by their content. Other constants (closure objects, ...) are considered
  equal when their types are equal, unless `compareConstantValues: true` is passed, which compares them by reference.
* Two `Extension` nodes (custom node types) are equal when they have the same type and `Equals` returns true. `DebugInfo` nodes are compared by document and position.

```c#
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
```

`EqualsToOptions` (a `[Flags]` enum) can be passed instead of the `compareConstantValues` flag:

* `CompareConstantValues` - the same as `compareConstantValues: true`.
* `FreeParametersByName` - parameters that are not declared inside the compared trees are equal when
  their names and types match. Use it to compare fragments of different lambdas, for example `l1.Body.EqualsTo(l2.Body, EqualsToOptions.FreeParametersByName)`.
* `ParameterNamesMatter` - lambda parameters and block/catch variables matched by position must also have the same name.

```c#
Expression<Func<int,int>> f1 = a => a + 1;
Expression<Func<int,int>> f2 = a => a + 1;

f1.Body.EqualsTo(f2.Body);                                           // false: the two 'a' are different parameters
f1.Body.EqualsTo(f2.Body, EqualsToOptions.FreeParametersByName);     // true

Expression<Func<int,int>> f3 = b => b + 1;

f1.EqualsTo(f3);                                                     // true
f1.EqualsTo(f3, EqualsToOptions.ParameterNamesMatter);               // false
```

## Supported frameworks

The library targets `netstandard2.0`. The tests run on .NET 8 and .NET 10; building from source requires the .NET 10 SDK.

The examples above are covered by tests (`Tests/ReadmeTests.cs`).
