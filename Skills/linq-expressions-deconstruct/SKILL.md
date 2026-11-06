---
name: linq-expressions-deconstruct
description: Match, traverse, rewrite and structurally compare System.Linq.Expressions trees with Linq.Expressions.Deconstruct. Use when writing Expr patterns or using ToExpr, Visit, Find, Transform or EqualsTo in code that consumes this package.
---

# Linq.Expressions.Deconstruct

The package targets .NET Standard 2.0. It wraps `System.Linq.Expressions` nodes in
`Expr` classes with `Deconstruct` overloads for C# recursive patterns, and provides
traversal, rewriting and structural comparison extension methods on `Expression`.
It inspects expression trees; wrapping a node does not evaluate or compile it.

This skill works from the NuGet cache at
`<global-packages>/linq.expressions.deconstruct/<version>/skills/linq-expressions-deconstruct/`
or when copied to `.agents/skills/linq-expressions-deconstruct/` or
`.claude/skills/linq-expressions-deconstruct/`. The package currently does not ship
`Linq.Expressions.Deconstruct.xml`; use the public assembly API for signatures
not covered here.

## Match nodes

```csharp
using System;
using System.Linq.Expressions;
using Linq.Expressions.Deconstruct;
using static Linq.Expressions.Deconstruct.Expr;
using Ex = System.Linq.Expressions.Expression;

Expression<Func<int, int>> f = i => i * 2;

if (f.ToExpr() is Lambda(Multiply(Parameter("i") used, Constant(2)), [var declared])
	&& used == declared)
{
	Console.WriteLine("Matched the declared parameter.");
}
```

`ToExpr()` dispatches by `NodeType`; for example `MemberAccess` becomes `Expr.Member`.
Null input returns null. `Expression` and `Expr` convert implicitly in both
directions; converting back returns the original node. Each concrete wrapper has
an `Expr` property containing its underlying expression. Child wrappers and wrapped
child collections are created lazily and cached within a wrapper.

Use [pattern shapes](docs/patterns.md) when choosing positional arguments for a
node, matching calls, accessing metadata or working with statement nodes. Shapes
vary by node kind; do not infer positions from the underlying expression constructor.
The examples using list patterns require C# 11 or later.

`Constant(value)` matches the original boxed value. `Constant(value, intValue)`
and `Constant(value, intValue, type)` also expose an `int?` projection for integral
numeric values in the `int` range. Thus `Constant(_, 0)` matches `0`, `0L`, `0.0`
and `0m`, while `Constant(0)` matches only a boxed `int` zero. Fractions,
out-of-range values, null and booleans have no `intValue`; `char` is projected
to its numeric code. The third position is the expression's declared `Type`.

## Traverse and search

| API | Callback and behavior |
| --- | --- |
| `Visit(Action<Expression>)` | Post-order, children before the node |
| `VisitEx(Action<Expr>)` | Same order, wrapped callback argument |
| `Visit(Func<Expression, bool>)` | Pre-order; true descends, false skips only this node's children |
| `Find(Func<Expression, bool>)` | First match in pre-order, or null |
| `FindEx(Func<Expr, bool>)` | Same search, wrapped callback argument; returns an `Expression`, not an `Expr` |
| `Find(Expression)` | Searches for that node instance |

There is no predicate overload of `VisitEx`. To prune, use `Visit` and return a
boolean, optionally wrapping inside the callback. Returning false does not stop
visiting sibling branches. Use `Find` to stop at the first match.

## Rewrite trees

`Transform` takes `Func<Expression, Expression>`; `TransformEx` takes
`Func<Expr, Expr>`. Return the supplied node to keep it. Use expression factories
such as `Ex.Add` for replacements; implicit conversions allow these factories in
a `TransformEx` callback. Both APIs return expressions, and their generic lambda
overloads preserve the static lambda type.

```csharp
Expression tree = Ex.Add(Ex.Constant(2), Ex.Constant(3));

var rewritten = tree.TransformEx(ex => ex switch
{
	Add(Constant(int left), Constant(int right)) => Ex.Constant(unchecked(left + right)),
	_ => ex
});
```

Default traversal is post-order (`forward: false`), so the callback sees already
transformed children. The forward overload puts the boolean before the callback,
for example `tree.Transform(forward: true, func: ex => ex)`. With `forward: true`,
the callback runs first; returning a different expression uses it as is and skips
its children. Neither mode repeatedly rewrites a replacement to a fixed point.

Unchanged branches retain their instances; an identity transformation returns
the original root. Rebuilt lambdas retain `Name` and `TailCall`. A replacement for
a declared lambda parameter, block variable or catch variable is used in the
declaration only if it is a `ParameterExpression`; other replacements affect
uses while the declaration is retained.

Keep replacements valid for their parent and preserve parameter binding. If the
root may change from a typed lambda into another expression kind, call through
an `Expression` variable to avoid the generic overload's cast back to the lambda
type. Match operand types, overflow rules and operator metadata before simplifying;
an algebraic identity can change exceptions, side effects or overloaded operators.

## Compare trees

Call `left.EqualsTo(right)` for structural comparison. `Expr.Equals` and `==`
delegate to the underlying node's equality, which normally means node identity.

Lambda parameters, block variables and catch variables pair by declaration
position and type within their scopes. Names are ignored by default, so
`x => x * 2` equals `y => y * 2`; using the first parameter differs from using the
second even if their names match. Free parameters, whose declarations are outside
the compared trees, match only the same parameter object by default.

| `EqualsToOptions` flag | Effect |
| --- | --- |
| `None` | Default structural comparison |
| `CompareConstantValues` | Also compare other object constants by reference; equivalent to `compareConstantValues: true` |
| `FreeParametersByName` | Free parameters match by name and type; useful for fragments such as separate lambda bodies |
| `ParameterNamesMatter` | Positionally paired declarations must also have matching names |

Combine flags with `|`. For `Expression<Func<int, int>> a = x => x + 1` and
`Expression<Func<int, int>> b = x => x + 1`, `a.EqualsTo(b)` is true, but
`a.Body.EqualsTo(b.Body)` is false. To opt into name matching for these fragments,
use `a.Body.EqualsTo(b.Body, EqualsToOptions.FreeParametersByName)`.

Primitive, string and enum constants compare by value even with default options.
Query constants compare their expression trees; sequence constants are enumerated
and their elements compared with `object.Equals`. Other constants, including
closure objects, compare by type unless `CompareConstantValues` is enabled.
This is structural comparison, not proof of equivalent execution.

Label targets pair consistently by first occurrence and require matching names
and types. Custom `Extension` nodes require the same runtime type and a successful
`Equals` comparison; `DebugInfo` compares document information and source positions.

## Allocations and common mistakes

- Wrappers allocate. Each `ToExpr()` call creates a new wrapper; wrapping a child
  collection allocates a collection and its wrappers. Prefer plain `Expression`
  overloads of `Visit`, `Find` and `Transform` with native node/property patterns
  in hot paths. Calling `ToExpr()` inside those callbacks still allocates.
- Do not assume every positional `type` is first or means the tested/declaring
  type. `Constant` puts it last, `TypeIs` exposes the result type, and `Dynamic`
  exposes the delegate type. Consult the shape guide for exceptions.
- Names in patterns do not establish parameter identity. Bind the wrappers and
  compare them as in the matching example, or inspect their underlying nodes.
- Statement collections can contain raw `Expression` or `ParameterExpression`
  objects. Wrap an element before applying an `Expr` pattern.
- Preserve unmatched nodes with `_ => ex` in rewrites. Use `Expression.Constant(null, type)`
  for a null constant rather than returning null as a replacement.
