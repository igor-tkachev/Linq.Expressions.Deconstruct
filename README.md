# Linq.Expressions.Deconstruct


[![Build status](https://ci.appveyor.com/api/projects/status/j4dym9acp0i9aau0/branch/master?svg=true)](https://ci.appveyor.com/project/igor-tkachev/linq-expressions-deconstruct/branch/master) [![NuGet Version and Downloads count](https://buildstats.info/nuget/Linq.Expressions.Deconstruct)](https://www.nuget.org/packages/Linq.Expressions.Deconstruct/)


```c#
[Test]
public void Test1()
{
    Expression<Func<int,int>> f = i => i * 2;

    switch (f.ToExpr())
    {
        case Lambda(Multiply(Parameter("i") p1, Constant(2)), (1, ("i") p2))
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
public void ConstantFolding()
{
    Expression<Func<int,int>> f = i => i * 0 + 0 + i + 10 * (i * 0 + 2);

    var f1 = f.Transform(ex => ex.ToExpr() switch
    {
        Multiply(Constant(0) e,   _)               => (Expression)e,              // 0 * e => 0
        Multiply(_,               Constant(0) e)   => (Expression)e,              // e * 0 => 0
        Multiply(Constant(1),     var e)           => (Expression)e,              // 1 * e => e
        Multiply(var e,           Constant(1))     => (Expression)e,              // e * 1 => 1
        Divide  (Constant(0) e,   _)               => (Expression)e,              // 0 / e => 0
        Divide  (var e,           Constant(1))     => (Expression)e,              // e / 1 => e
        Add     (Constant(0),     var e)           => (Expression)e,              // 0 + e => e
        Add     (var e,           Constant(0))     => (Expression)e,              // e + 0 => e
        Subtract(Constant(0),     var e)           => Expression.Negate(e),       // 0 - e => -e
        Subtract(var e,           Constant(0))     => (Expression)e,              // e - 0 => e
        Multiply(Constant(int x), Constant(int y)) => Expression.Constant(x * y), // x * y => e
        Divide  (Constant(int x), Constant(int y)) => Expression.Constant(x / y), // x / y => e
        Add     (Constant(int x), Constant(int y)) => Expression.Constant(x + y), // x + y => e
        Subtract(Constant(int x), Constant(int y)) => Expression.Constant(x - y), // x - y => e
        _                                          => ex
    });

    void Test(Expression<Func<int,int>> ex)
    {
        switch (ex.ToExpr())
        {
            case Lambda(Add(Parameter("i"), Constant(20)), (1, Parameter("i"))) :
                Console.WriteLine("Pattern Matched!");
                break;
            default:
                Console.WriteLine(f);
                Assert.Fail();
                break;
        };
    }

    Test(f1);
    Test(i => i + 20);
}
```
