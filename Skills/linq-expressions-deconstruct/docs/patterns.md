Back to [SKILL.md](../SKILL.md).

# Pattern shapes

> You are here if you need the positional shape for a node, a method-call match,
> or access to metadata and statement children that are not wrapped.

Import `Linq.Expressions.Deconstruct` for extension methods and
`using static Linq.Expressions.Deconstruct.Expr;` for short wrapper names.
Each shape below lists positional arguments in order. Names such as `value`
describe positions; use actual C# patterns such as `var value` when writing code.
`type` means the node's result `System.Type` unless stated otherwise.

## Binary and unary nodes

Every binary wrapper below has `(left, right)` and `(type, left, right)` shapes.
Both operands are `Expr` objects.

```text
Add                 AddChecked           And                  AndAlso
ArrayIndex          Assign               Coalesce             Divide
Equal               ExclusiveOr          GreaterThan          GreaterThanOrEqual
LeftShift           LessThan             LessThanOrEqual      Modulo
Multiply            MultiplyChecked      NotEqual             Or
OrElse              Power                RightShift           Subtract
SubtractChecked     AddAssign            AndAssign            DivideAssign
ExclusiveOrAssign   LeftShiftAssign      ModuloAssign         MultiplyAssign
OrAssign            PowerAssign          RightShiftAssign     SubtractAssign
AddAssignChecked    MultiplyAssignChecked SubtractAssignChecked
```

Every unary wrapper below has `(operand)` and `(type, operand)` shapes. The
operand is an `Expr?`; it can be null for a rethrow (`Throw`).

```text
ArrayLength         Convert              ConvertChecked       Negate
NegateChecked       Not                  Quote                TypeAs
UnaryPlus           Decrement            Increment            IsFalse
IsTrue              Throw                Unbox                PreIncrementAssign
PreDecrementAssign  PostIncrementAssign  PostDecrementAssign  OnesComplement
```

For example, `Add(Parameter("x"), Constant(_, 1))` matches the operands, and
`Convert(var resultType, Parameter("x"))` also captures the conversion's result
type. Lifted flags, operator `Method` and binary `Conversion` are available on
the underlying `wrapper.Expr`, not as extra positional arguments. Generated
binary/unary child accessors are private; use deconstruction or `wrapper.Expr`
to access those children.

## Other expression nodes

These are all supported arities. A semicolon separates complete alternatives;
it does not indicate optional individual arguments.

| Wrapper | Positional shapes |
| --- | --- |
| `Constant` | `(value)`; `(value, intValue)`; `(value, intValue, type)` |
| `Parameter` | `(name)`; `(type, name)` |
| `Default` | `(type)` |
| `Lambda` | `(body, parameters)`; `(type, body, parameters)` |
| `Call` | `(method, object, arguments)`; `(type, method, object, arguments)`; `(isGeneric, type, method, object, arguments)` |
| `Conditional` | `(test, ifTrue, ifFalse)`; `(type, test, ifTrue, ifFalse)` |
| `Invoke` | `(expression, arguments)`; `(type, expression, arguments)` |
| `Member` | `(expression)`; `(type, expression)` |
| `New` | `(arguments)`; `(members, arguments)`; `(constructor, members, arguments)`; `(type, constructor, members, arguments)` |
| `NewArrayBounds`, `NewArrayInit` | `(expressions)`; `(type, expressions)` |
| `ListInit` | `(newExpression, initializers)`; `(type, newExpression, initializers)` |
| `MemberInit` | `(newExpression, bindings)`; `(type, newExpression, bindings)` |
| `TypeIs`, `TypeEqual` | `(expression)`; `(type, expression)` |
| `Block` | `(variables, expressions)`; `(type, variables, expressions)` |
| `Dynamic` | `(delegateType, arguments)` |
| `Goto` | `(target, value)`; `(type, target, value)` |
| `Index` | `(object, arguments)`; `(object, indexer, arguments)`; `(type, object, indexer, arguments)` |
| `Label` | `(target, defaultValue)`; `(type, target, defaultValue)` |
| `RuntimeVariables` | `(variables)`; `(type, variables)` |
| `Loop` | `(breakLabel, continueLabel, body)`; `(type, breakLabel, continueLabel, body)` |
| `Switch` | `(switchValue, cases, defaultBody)`; `(type, switchValue, cases, defaultBody)` |
| `Try` | `(body, handlers, finally, fault)`; `(type, body, handlers, finally, fault)` |
| `DebugInfo` | `(document)`; `(type, document)` |
| `Extension` | No `Deconstruct` overload; inspect `Expr` |

`Constant.value` is the original boxed `object` and can be null; `intValue` is
`int?`, as described in [SKILL.md](../SKILL.md#match-nodes). `Parameter.name` can
be null. For `Lambda`, `type` is the delegate type, not the body's return type.

`Lambda.parameters` is `ReadOnlyCollection<Expr.Parameter>`. Call/invoke/new/index/
dynamic arguments and new-array expressions are `ReadOnlyCollection<Expr>`.
`ListInit.newExpression` is exposed as `Expr` in the two-position overload and
`Expr.New` in the three-position overload; `MemberInit.newExpression` is `Expr.New`.

## Calls and metadata

```csharp
Expression<Func<string, bool>> f = s => s.StartsWith("A");

if (f.ToExpr() is Lambda(
	Call({ Name: "StartsWith", DeclaringType: var owner }, Parameter("s"), [Constant("A")]),
	[_]) && owner == typeof(string))
{
	Console.WriteLine("Matched string.StartsWith.");
}
```

The three- and four-position `Call` overloads return the actual `MethodInfo`.
Only the five-position overload returns `GetGenericMethodDefinition()` when
`isGeneric` is true. Its `type` is still the call result type, not the method's
declaring type. The receiver is null for a static call. Check declaring type,
signature or exact `MethodInfo` when a name alone would match unrelated methods.

`Member` has no member-name positional argument. Inspect `member.Expr.Member`,
for example `Member { Expr: { Member: { Name: "Length" } } }`. Its expression is
null for a static member. `TypeIs` and `TypeEqual` expose the result type
(`bool`); inspect `wrapper.Expr.TypeOperand` for the type being tested.
`Dynamic` exposes `DelegateType`, not `Type`. Inspect underlying nodes for
`Binder`, `Goto.Kind`, `Switch.Comparison` and debug source positions.

`New.constructor` can be null for value-type default construction;
`New.members` can be null. An array `Index` can have a null indexer. `Goto.value`,
loop labels, label defaults, switch defaults and try `finally`/`fault` can be
null even where legacy annotations on overloads do not express it.

## Statement collections and helper shapes

`Block.variables` and `RuntimeVariables.variables` are
`IEnumerable<ParameterExpression>`; `Block.expressions` is
`IEnumerable<Expression>`. These elements are raw nodes, not `Expr` wrappers.
`Switch.cases` is `IEnumerable<SwitchCase>`, `Try.handlers` is
`IEnumerable<CatchBlock>`, `MemberInit.bindings` is `IEnumerable<MemberBinding>`
and `ListInit.initializers` is `ReadOnlyCollection<ElementInit>`.

The package supplies these extension deconstructors for helper objects:

| Raw object | Shapes |
| --- | --- |
| `ElementInit` | `(arguments)`; `(addMethod, arguments)` with wrapped arguments |
| `LabelTarget` | `(type, name)` |
| `SwitchCase` | `(body, testValues)` with wrapped body and test values |

There is no `CatchBlock` or `MemberBinding` positional helper. Use their native
properties, wrapping contained expressions explicitly when needed.

Use C# list patterns such as `[Constant(1), var second]` on wrapped read-only
collections. Properties typed only as `IEnumerable<T>` do not support list
patterns directly. The package supplies positional extension deconstructors
`(count)` through `(count, item0, ..., item9)` for the collection element types
above, plus `MemberInfo`, `Expression`, `ParameterExpression`, `Expr` and
`Expr.Parameter`. Items retain their element type; deconstructing a raw
expression collection does not wrap it. Missing items are default values.
Count is exact for arrays and `ICollection<T>`; for other enumerables it is
bounded by the requested item count plus one (null reports -1). Do not use
that bounded count as a general collection length.
