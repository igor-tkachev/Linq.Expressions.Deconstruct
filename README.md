# Linq.Expressions.Deconstruct


[![Build status](https://ci.appveyor.com/api/projects/status/j4dym9acp0i9aau0/branch/master?svg=true)](https://ci.appveyor.com/project/igor-tkachev/linq-expressions-deconstruct/branch/master) [![NuGet Version and Downloads count](https://buildstats.info/nuget/Linq.Expressions.Deconstruct)](https://www.nuget.org/packages/Linq.Expressions.Deconstruct/)


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


```c#
[Test]
public void ConstantFoldingTest()
{
    Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

    var f1 = f.TransformEx(ex => ex switch
    {
        Multiply(Constant(0) e,   _)               => e,               // 0 * e => 0
        Multiply(_,               Constant(0) e)   => e,               // e * 0 => 0
        Multiply(Constant(1),     var e)           => e,               // 1 * e => e
        Multiply(var e,           Constant(1))     => e,               // e * 1 => e
        Divide  (Constant(0) e,   _)               => e,               // 0 / e => 0
        Divide  (var e,           Constant(1))     => e,               // e / 1 => e
        Add     (Constant(0),     var e)           => e,               // 0 + e => e
        Add     (var e,           Constant(0))     => e,               // e + 0 => e
        Subtract(Constant(0),     var e)           => Negate(e),       // 0 - e => -e
        Subtract(var e,           Constant(0))     => e,               // e - 0 => e
        Multiply(Constant(int x), Constant(int y)) => Constant(x * y), // x * y => e
        Divide  (Constant(int x), Constant(int y)) => Constant(x / y), // x / y => e
        Add     (Constant(int x), Constant(int y)) => Constant(x + y), // x + y => e
        Subtract(Constant(int x), Constant(int y)) => Constant(x - y), // x - y => e
        _                                          => ex
    });

    Assert.IsTrue(f1.EqualsTo(i => i + 20));
}
```
