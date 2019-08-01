# Linq.Expressions.Deconstruct


[![Build status](https://ci.appveyor.com/api/projects/status/j4dym9acp0i9aau0/branch/master?svg=true)](https://ci.appveyor.com/project/igor-tkachev/linq-expressions-deconstruct/branch/master)


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
