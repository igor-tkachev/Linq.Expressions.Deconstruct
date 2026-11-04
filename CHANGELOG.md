# Changelog

## Unreleased

### Breaking changes

- `EqualsTo` no longer treats free parameters (parameters that are not declared by a lambda, block or catch block
  inside the compared trees) as equal when their names match: a free parameter is equal only to itself.
  Comparing lambda bodies of two different lambdas, `l1.Body.EqualsTo(l2.Body)`, used to be true for
  `a => a + 1` and `a => a + 1` and is now false, because the two `a` are different objects.
  Pass `EqualsToOptions.FreeParametersByName` to get the old behaviour back.
- `EqualsTo` on `Extension` nodes: two extension nodes are equal when they have the same type and
  `Equals` returns true (before, any two extension nodes of the same type were equal).
  `DebugInfo` nodes are compared by document and position (before, any two were equal).
- `compareConstantValues` is now honoured inside `Goto`, `Label`, `Loop` and `Index` nodes
  (these nodes used to compare their children with default options).
- Label targets are paired by their first occurrence, like parameters, so two jumps to one label are
  no longer equal to jumps to two different labels with the same name and type.

### Fixed

- `EqualsTo` on `Switch` expressions returned false for every pair of distinct but equal nodes
  (missing negation in the `SwitchValue` and `DefaultBody` checks).
- `EqualsTo` compares parameters by their position in the enclosing lambda, block or catch block instead of by name:
  `(a, b) => a` is no longer equal to `(a, b) => b` when the parameters share a name,
  and `x => x * 2` is now equal to `y => y * 2`.
  A parameter declared outside of the compared trees is equal only to itself.
- `EqualsTo` no longer treats any two `Extension` nodes or any two `DebugInfo` nodes of the same type as equal.
  An extension node is equal only to itself, debug info is compared by document and position.
- `EqualsTo` passes its comparison state through `Goto`, `Label`, `Loop` and `Index` nodes;
  null loop labels and `RuntimeVariables` of different length no longer throw.
- `Constant` no longer reports an `intValue` for `long` values below `int.MinValue`
  (and for the `float` value 2^31 on .NET 9 and later).
- `Transform` no longer throws `InvalidCastException` when the function replaces a lambda parameter, block variable or
  catch variable with a non-parameter expression: the declaration is kept and its uses in the body are replaced.
- `EqualsTo` handles a parameter object reused in a nested scope symmetrically, and comparing trees with shared subnodes
  (DAGs) no longer takes exponential time.
- `Transform` keeps `Name` and `TailCall` of a rebuilt lambda.
- Fixed the documentation of `Visit(Func<Expression, bool>)` (false skips the children of a node),
  the nullability annotation of `Find(Func<Expression, bool>)` (it returns null when nothing is found)
  and a duplicated attribute on `TransformEx`.

### Changed

- The tests run on .NET 8 and .NET 10 (.NET 9 is dropped). Building from source requires the .NET 10 SDK (`global.json`).
- Warnings are treated as errors.
- CI workflow updated (actions v4, SDK 10, restore/build/test in Release, manual runs); the `Appveyor.TestLogger` package and the AppVeyor badge are removed.

### Added

- `EqualsTo(expr1, expr2, EqualsToOptions)` with `CompareConstantValues`, `FreeParametersByName` and `ParameterNamesMatter`.
- Tests for `EqualsTo`, `Transform` and `Visit` on statement nodes, and for the README examples.
- CI checks that `Source/Expr.generated.cs` matches `Source/Expr.tt` (`dotnet-t4` as a local tool).
- README: documentation of `ToExpr`, `Visit`, `Find`, `Transform` and `EqualsTo`.
