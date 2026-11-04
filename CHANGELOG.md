# Changelog

## Unreleased

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
- `Transform` keeps `Name` and `TailCall` of a rebuilt lambda.
- Fixed the documentation of `Visit(Func<Expression, bool>)` (false skips the children of a node),
  the nullability annotation of `Find(Func<Expression, bool>)` (it returns null when nothing is found)
  and a duplicated attribute on `TransformEx`.

### Changed

- The tests run on .NET 8 and .NET 10 (.NET 9 is dropped). Building from source requires the .NET 10 SDK (`global.json`).
- Warnings are treated as errors.
- CI moved to GitHub Actions (restore, build and test in Release); the AppVeyor configuration and badge are removed.

### Added

- Tests for `EqualsTo`, `Transform` and `Visit` on statement nodes, and for the README examples.
- CI checks that `Source/Expr.generated.cs` matches `Source/Expr.tt` (`dotnet-t4` as a local tool).
- README: documentation of `ToExpr`, `Visit`, `Find`, `Transform` and `EqualsTo`.
