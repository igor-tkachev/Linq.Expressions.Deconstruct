using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Linq.Expressions.Deconstruct
{
	public abstract partial class Expr
	{
		#region overrides

		protected abstract Expression GetExpression();

		public override string ToString   () => GetExpression().ToString();
		public override int    GetHashCode() => GetExpression().GetHashCode();

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case Expr       ex   : return GetExpression().Equals(ex.GetExpression());
				case Expression expr : return GetExpression().Equals(expr);
				default              : return false;
			}
		}

		public static bool operator ==(Expr left, Expr right) => Equals(left, right);
		public static bool operator !=(Expr left, Expr right) => !Equals(left, right);

		public static implicit operator Expression(Expr expr) => expr.GetExpression();
		public static implicit operator Expr(Expression expr) => expr.ToExpr();

		#endregion

		#region Call

		public partial class Call
		{
			public void Deconstruct(out Type type, out MethodInfo method, out Expr @object, out ReadOnlyCollection<Expression> arguments)
			{
				type      = Expr.Type;
				method    = Expr.Method;
				@object   = Expr.Object.ToExpr();
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out MethodInfo method, out Expr @object, out ReadOnlyCollection<Expression> arguments)
			{
				method    = Expr.Method;
				@object   = Expr.Object.ToExpr();
				arguments = Expr.Arguments;
			}
		}

		#endregion

		#region Conditional

		public partial class Conditional
		{
			public void Deconstruct(out Type type, out Expr test, out Expr ifTrue, out Expr ifFalse)
			{
				type    = Expr.Type;
				test    = Expr.Test.   ToExpr();
				ifTrue  = Expr.IfTrue. ToExpr();
				ifFalse = Expr.IfFalse.ToExpr();
			}

			public void Deconstruct(out Expr test, out Expr ifTrue, out Expr ifFalse)
			{
				test    = Expr.Test.   ToExpr();
				ifTrue  = Expr.IfTrue. ToExpr();
				ifFalse = Expr.IfFalse.ToExpr();
			}
		}

		#endregion

		#region Invoke

		public partial class Invoke
		{
			public void Deconstruct(out Type type, out Expr expression, out ReadOnlyCollection<Expression> arguments)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr();
				arguments  = Expr.Arguments;
			}

			public void Deconstruct(out Expr expression, out ReadOnlyCollection<Expression> arguments)
			{
				expression = Expr.Expression.ToExpr();
				arguments  = Expr.Arguments;
			}
		}

		#endregion

		#region Lambda

		public partial class Lambda
		{
			public void Deconstruct(out Type type, out Expr body, out ReadOnlyCollection<ParameterExpression> parameters)
			{
				type       = Expr.Type;
				body       = Expr.Body.ToExpr();
				parameters = Expr.Parameters;
			}

			public void Deconstruct(out Expr body, out ReadOnlyCollection<ParameterExpression> parameters)
			{
				body       = Expr.Body.ToExpr();
				parameters = Expr.Parameters;
			}
		}

		#endregion

		#region ListInit

		public partial class ListInit
		{
			public void Deconstruct(out Type type, out Expr newExpression, out ReadOnlyCollection<ElementInit> initializers)
			{
				type          = Expr.Type;
				newExpression = Expr.NewExpression.ToExpr();
				initializers  = Expr.Initializers;
			}

			public void Deconstruct(out Expr newExpression, out ReadOnlyCollection<ElementInit> initializers)
			{
				newExpression = Expr.NewExpression.ToExpr();
				initializers  = Expr.Initializers;
			}
		}

		#endregion

		#region Member

		public partial class Member
		{
			public void Deconstruct(out Type type, out Expr expression)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr();
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expr.Expression.ToExpr();
			}
		}

		#endregion

		#region MemberInit

		public partial class MemberInit
		{
			public void Deconstruct(out Type type, out New newExpression, out IEnumerable<MemberBinding> bindings)
			{
				type          = Expr.Type;
				newExpression = new New(Expr.NewExpression);
				bindings      = Expr.Bindings;
			}

			public void Deconstruct(out New newExpression, out IEnumerable<MemberBinding> bindings)
			{
				newExpression = new New(Expr.NewExpression);
				bindings      = Expr.Bindings;
			}
		}

		#endregion

		#region New

		public partial class New
		{
			public void Deconstruct(
				out Type                           type,
				out ConstructorInfo                constructor,
				out ReadOnlyCollection<Expression> arguments,
				out ReadOnlyCollection<MemberInfo> members)
			{
				type        = Expr.Type;
				constructor = Expr.Constructor;
				arguments   = Expr.Arguments;
				members     = Expr.Members;
			}

			public void Deconstruct(
				out ConstructorInfo                constructor,
				out ReadOnlyCollection<Expression> arguments,
				out ReadOnlyCollection<MemberInfo> members)
			{
				constructor = Expr.Constructor;
				arguments   = Expr.Arguments;
				members     = Expr.Members;
			}

			public void Deconstruct(out ConstructorInfo constructor, out ReadOnlyCollection<Expression> arguments)
			{
				constructor = Expr.Constructor;
				arguments   = Expr.Arguments;
			}
		}

		#endregion

		#region NewArrayBounds

		public partial class NewArrayBounds
		{
			public void Deconstruct(out Type type, out ReadOnlyCollection<Expression> expressions)
			{
				type        = Expr.Type;
				expressions = Expr.Expressions;
			}

			public void Deconstruct(out ReadOnlyCollection<Expression> expressions)
			{
				expressions = Expr.Expressions;
			}
		}

		#endregion

		#region NewArrayInit

		public partial class NewArrayInit
		{
			public void Deconstruct(out Type type, out ReadOnlyCollection<Expression> expressions)
			{
				type        = Expr.Type;
				expressions = Expr.Expressions;
			}

			public void Deconstruct(out ReadOnlyCollection<Expression> expressions)
			{
				expressions = Expr.Expressions;
			}
		}

		#endregion

		#region TypeEqual

		public partial class TypeEqual
		{
			public void Deconstruct(out Type type, out Expr expression)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr();
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expr.Expression.ToExpr();
			}
		}

		#endregion

		#region TypeIs

		public partial class TypeIs
		{
			public void Deconstruct(out Type type, out Expr expression)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr();
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expr.Expression.ToExpr();
			}
		}

		#endregion

		#region Block

		public partial class Block
		{
			public void Deconstruct(
				out Type                             type,
				out IEnumerable<ParameterExpression> variables,
				out IEnumerable<Expression>          expressions)
			{
				type        = Expr.Type;
				variables   = Expr.Variables;
				expressions = Expr.Expressions;
			}

			public void Deconstruct(out IEnumerable<ParameterExpression> variables, out IEnumerable<Expression> expressions)
			{
				variables   = Expr.Variables;
				expressions = Expr.Expressions;
			}
		}

		#endregion

		#region Dynamic

		public partial class Dynamic
		{
			public void Deconstruct(out Type delegateType, out ReadOnlyCollection<Expression> arguments)
			{
				delegateType = Expr.DelegateType;
				arguments    = Expr.Arguments;
			}
		}

		#endregion

		#region Goto

		public partial class Goto
		{
			public void Deconstruct(out Type type, out LabelTarget target, out Expr value)
			{
				type   = Expr.Type;
				target = Expr.Target;
				value  = Expr.Value.ToExpr();
			}

			public void Deconstruct(out LabelTarget target, out Expr value)
			{
				target = Expr.Target;
				value  = Expr.Value.ToExpr();
			}
		}

		#endregion

		#region Index

		public partial class Index
		{
			public void Deconstruct(out Type type, out Expr @object, out PropertyInfo indexer, out IReadOnlyList<Expression> arguments)
			{
				type      = Expr.Type;
				@object   = Expr.Object.ToExpr();
				indexer   = Expr.Indexer;
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out Expr @object, out PropertyInfo indexer, out IReadOnlyList<Expression> arguments)
			{
				@object   = Expr.Object.ToExpr();
				indexer   = Expr.Indexer;
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out Expr @object, out IReadOnlyList<Expression> arguments)
			{
				@object   = Expr.Object.ToExpr();
				arguments = Expr.Arguments;
			}
		}

		#endregion

		#region Label

		public partial class Label
		{
			public void Deconstruct(out Type type, out LabelTarget target, out Expr defaultValue)
			{
				type         = Expr.Type;
				target       = Expr.Target;
				defaultValue = Expr.DefaultValue.ToExpr();
			}

			public void Deconstruct(out LabelTarget target, out Expr defaultValue)
			{
				target       = Expr.Target;
				defaultValue = Expr.DefaultValue.ToExpr();
			}
		}

		#endregion

		#region RuntimeVariables

		public partial class RuntimeVariables
		{
			public void Deconstruct(out Type type, out IEnumerable<ParameterExpression> variables)
			{
				type      = Expr.Type;
				variables = Expr.Variables;
			}

			public void Deconstruct(out IEnumerable<ParameterExpression> variables)
			{
				variables = Expr.Variables;
			}
		}

		#endregion

		#region Loop

		public partial class Loop
		{
			public void Deconstruct(out Type type, out LabelTarget breakLabel, out LabelTarget continueLabel, out Expr body)
			{
				type          = Expr.Type;
				breakLabel    = Expr.BreakLabel;
				continueLabel = Expr.ContinueLabel;
				body          = Expr.Body.ToExpr();
			}

			public void Deconstruct(out LabelTarget breakLabel, out LabelTarget continueLabel, out Expr body)
			{
				breakLabel    = Expr.BreakLabel;
				continueLabel = Expr.ContinueLabel;
				body          = Expr.Body.ToExpr();
			}
		}

		#endregion

		#region Switch

		public partial class Switch
		{
			public void Deconstruct(out Type type, out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr defaultBody)
			{
				type        = Expr.Type;
				switchValue = Expr.SwitchValue.ToExpr();
				cases       = Expr.Cases;
				defaultBody = Expr.DefaultBody.ToExpr();
			}

			public void Deconstruct(out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr defaultBody)
			{
				switchValue = Expr.SwitchValue.ToExpr();
				cases       = Expr.Cases;
				defaultBody = Expr.DefaultBody.ToExpr();
			}
		}

		#endregion

		#region Try

		public partial class Try
		{
			public void Deconstruct(out Type type, out Expr body, out IEnumerable<CatchBlock> handlers, out Expr @finally, out Expr fault)
			{
				type     = Expr.Type;
				body     = Expr.Body.ToExpr();
				handlers = Expr.Handlers;
				@finally = Expr.Finally.ToExpr();
				fault    = Expr.Fault.ToExpr();
			}

			public void Deconstruct(out Expr body, out IEnumerable<CatchBlock> handlers, out Expr @finally, out Expr fault)
			{
				body     = Expr.Body.ToExpr();
				handlers = Expr.Handlers;
				@finally = Expr.Finally.ToExpr();
				fault    = Expr.Fault.ToExpr();
			}
		}

		#endregion

		#region Extension

		public partial class Extension
		{
		}

		#endregion

		#region DebugInfo

		public partial class DebugInfo
		{
			public void Deconstruct(out Type type, out SymbolDocumentInfo document)
			{
				type     = Expr.Type;
				document = Expr.Document;
			}

			public void Deconstruct(out SymbolDocumentInfo document)
			{
				document = Expr.Document;
			}
		}

		#endregion

		#region Parameter

		public partial class Parameter
		{
			public void Deconstruct(out Type type, out string name)
			{
				type = Expr.Type;
				name = Expr.Name;
			}

			public void Deconstruct(out string name)
			{
				name = Expr.Name;
			}
		}

		#endregion

		#region Constant

		public partial class Constant
		{
			public void Deconstruct(out Type type, out object value)
			{
				type  = Expr.Type;
				value = Expr.Value;
			}

			public void Deconstruct(out object value)
			{
				value = Expr.Value;
			}
		}

		#endregion

		#region Default

		public partial class Default
		{
			public void Deconstruct(out Type type)
			{
				type  = Expr.Type;
			}
		}

		#endregion
	}

	public static partial class Extensions
	{
		public static Expr ToExpr(this Expression expr)
		{
			if (expr == null)
				return null;

			switch (expr.NodeType)
			{
				case ExpressionType.Add                   : return new Expr.Add                  ((BinaryExpression)expr);
				case ExpressionType.AddChecked            : return new Expr.AddChecked           ((BinaryExpression)expr);
				case ExpressionType.And                   : return new Expr.And                  ((BinaryExpression)expr);
				case ExpressionType.AndAlso               : return new Expr.AndAlso              ((BinaryExpression)expr);
				case ExpressionType.ArrayIndex            : return new Expr.ArrayIndex           ((BinaryExpression)expr);
				case ExpressionType.Assign                : return new Expr.Assign               ((BinaryExpression)expr);
				case ExpressionType.Coalesce              : return new Expr.Coalesce             ((BinaryExpression)expr);
				case ExpressionType.Divide                : return new Expr.Divide               ((BinaryExpression)expr);
				case ExpressionType.Equal                 : return new Expr.Equal                ((BinaryExpression)expr);
				case ExpressionType.ExclusiveOr           : return new Expr.ExclusiveOr          ((BinaryExpression)expr);
				case ExpressionType.GreaterThan           : return new Expr.GreaterThan          ((BinaryExpression)expr);
				case ExpressionType.GreaterThanOrEqual    : return new Expr.GreaterThanOrEqual   ((BinaryExpression)expr);
				case ExpressionType.LeftShift             : return new Expr.LeftShift            ((BinaryExpression)expr);
				case ExpressionType.LessThan              : return new Expr.LessThan             ((BinaryExpression)expr);
				case ExpressionType.LessThanOrEqual       : return new Expr.LessThanOrEqual      ((BinaryExpression)expr);
				case ExpressionType.Modulo                : return new Expr.Modulo               ((BinaryExpression)expr);
				case ExpressionType.Multiply              : return new Expr.Multiply             ((BinaryExpression)expr);
				case ExpressionType.MultiplyChecked       : return new Expr.MultiplyChecked      ((BinaryExpression)expr);
				case ExpressionType.NotEqual              : return new Expr.NotEqual             ((BinaryExpression)expr);
				case ExpressionType.Or                    : return new Expr.Or                   ((BinaryExpression)expr);
				case ExpressionType.OrElse                : return new Expr.OrElse               ((BinaryExpression)expr);
				case ExpressionType.Power                 : return new Expr.Power                ((BinaryExpression)expr);
				case ExpressionType.RightShift            : return new Expr.RightShift           ((BinaryExpression)expr);
				case ExpressionType.Subtract              : return new Expr.Subtract             ((BinaryExpression)expr);
				case ExpressionType.SubtractChecked       : return new Expr.SubtractChecked      ((BinaryExpression)expr);
				case ExpressionType.AddAssign             : return new Expr.AddAssign            ((BinaryExpression)expr);
				case ExpressionType.AndAssign             : return new Expr.AndAssign            ((BinaryExpression)expr);
				case ExpressionType.DivideAssign          : return new Expr.DivideAssign         ((BinaryExpression)expr);
				case ExpressionType.ExclusiveOrAssign     : return new Expr.ExclusiveOrAssign    ((BinaryExpression)expr);
				case ExpressionType.LeftShiftAssign       : return new Expr.LeftShiftAssign      ((BinaryExpression)expr);
				case ExpressionType.ModuloAssign          : return new Expr.ModuloAssign         ((BinaryExpression)expr);
				case ExpressionType.MultiplyAssign        : return new Expr.MultiplyAssign       ((BinaryExpression)expr);
				case ExpressionType.OrAssign              : return new Expr.OrAssign             ((BinaryExpression)expr);
				case ExpressionType.PowerAssign           : return new Expr.PowerAssign          ((BinaryExpression)expr);
				case ExpressionType.RightShiftAssign      : return new Expr.RightShiftAssign     ((BinaryExpression)expr);
				case ExpressionType.SubtractAssign        : return new Expr.SubtractAssign       ((BinaryExpression)expr);
				case ExpressionType.AddAssignChecked      : return new Expr.AddAssignChecked     ((BinaryExpression)expr);
				case ExpressionType.MultiplyAssignChecked : return new Expr.MultiplyAssignChecked((BinaryExpression)expr);
				case ExpressionType.SubtractAssignChecked : return new Expr.SubtractAssignChecked((BinaryExpression)expr);

				case ExpressionType.ArrayLength           : return new Expr.ArrayLength          ((UnaryExpression)expr);
				case ExpressionType.Convert               : return new Expr.Convert              ((UnaryExpression)expr);
				case ExpressionType.ConvertChecked        : return new Expr.ConvertChecked       ((UnaryExpression)expr);
				case ExpressionType.Negate                : return new Expr.Negate               ((UnaryExpression)expr);
				case ExpressionType.NegateChecked         : return new Expr.NegateChecked        ((UnaryExpression)expr);
				case ExpressionType.Not                   : return new Expr.Not                  ((UnaryExpression)expr);
				case ExpressionType.Quote                 : return new Expr.Quote                ((UnaryExpression)expr);
				case ExpressionType.TypeAs                : return new Expr.TypeAs               ((UnaryExpression)expr);
				case ExpressionType.UnaryPlus             : return new Expr.UnaryPlus            ((UnaryExpression)expr);
				case ExpressionType.Decrement             : return new Expr.Decrement            ((UnaryExpression)expr);
				case ExpressionType.Increment             : return new Expr.Increment            ((UnaryExpression)expr);
				case ExpressionType.IsFalse               : return new Expr.IsFalse              ((UnaryExpression)expr);
				case ExpressionType.IsTrue                : return new Expr.IsTrue               ((UnaryExpression)expr);
				case ExpressionType.Throw                 : return new Expr.Throw                ((UnaryExpression)expr);
				case ExpressionType.Unbox                 : return new Expr.Unbox                ((UnaryExpression)expr);
				case ExpressionType.PreIncrementAssign    : return new Expr.PreIncrementAssign   ((UnaryExpression)expr);
				case ExpressionType.PreDecrementAssign    : return new Expr.PreDecrementAssign   ((UnaryExpression)expr);
				case ExpressionType.PostIncrementAssign   : return new Expr.PostIncrementAssign  ((UnaryExpression)expr);
				case ExpressionType.PostDecrementAssign   : return new Expr.PostDecrementAssign  ((UnaryExpression)expr);
				case ExpressionType.OnesComplement        : return new Expr.OnesComplement       ((UnaryExpression)expr);

				case ExpressionType.Call                  : return new Expr.Call                 ((MethodCallExpression)      expr);
				case ExpressionType.Conditional           : return new Expr.Conditional          ((ConditionalExpression)     expr);
				case ExpressionType.Invoke                : return new Expr.Invoke               ((InvocationExpression)      expr);
				case ExpressionType.Lambda                : return new Expr.Lambda               ((LambdaExpression)          expr);
				case ExpressionType.ListInit              : return new Expr.ListInit             ((ListInitExpression)        expr);
				case ExpressionType.MemberAccess          : return new Expr.Member               ((MemberExpression)          expr);
				case ExpressionType.MemberInit            : return new Expr.MemberInit           ((MemberInitExpression)      expr);
				case ExpressionType.New                   : return new Expr.New                  ((NewExpression)             expr);
				case ExpressionType.NewArrayBounds        : return new Expr.NewArrayBounds       ((NewArrayExpression)        expr);
				case ExpressionType.NewArrayInit          : return new Expr.NewArrayInit         ((NewArrayExpression)        expr);
				case ExpressionType.TypeEqual             : return new Expr.TypeEqual            ((TypeBinaryExpression)      expr);
				case ExpressionType.TypeIs                : return new Expr.TypeIs               ((TypeBinaryExpression)      expr);
				case ExpressionType.Block                 : return new Expr.Block                ((BlockExpression)           expr);
				case ExpressionType.Dynamic               : return new Expr.Dynamic              ((DynamicExpression)         expr);
				case ExpressionType.Goto                  : return new Expr.Goto                 ((GotoExpression)            expr);
				case ExpressionType.Index                 : return new Expr.Index                ((IndexExpression)           expr);
				case ExpressionType.Label                 : return new Expr.Label                ((LabelExpression)           expr);
				case ExpressionType.RuntimeVariables      : return new Expr.RuntimeVariables     ((RuntimeVariablesExpression)expr);
				case ExpressionType.Loop                  : return new Expr.Loop                 ((LoopExpression)            expr);
				case ExpressionType.Switch                : return new Expr.Switch               ((SwitchExpression)          expr);
				case ExpressionType.Try                   : return new Expr.Try                  ((TryExpression)             expr);
				case ExpressionType.Extension             : return new Expr.Extension            (expr);
				case ExpressionType.DebugInfo             : return new Expr.DebugInfo            ((DebugInfoExpression)       expr);
				case ExpressionType.Parameter             : return new Expr.Parameter            ((ParameterExpression)       expr);
				case ExpressionType.Constant              : return new Expr.Constant             ((ConstantExpression)        expr);
				case ExpressionType.Default               : return new Expr.Default              ((DefaultExpression)        expr);
			}

			throw new InvalidOperationException();
		}

		#region ElementInit

		public static void Deconstruct(this ElementInit elementInit, out MethodInfo addMethod, out ReadOnlyCollection<Expression> arguments)
		{
			addMethod = elementInit.AddMethod;
			arguments = elementInit.Arguments;
		}

		#endregion

		#region LabelTarget

		public static void Deconstruct(this LabelTarget label, out Type type, out string name)
		{
			type = label.Type;
			name = label.Name;
		}

		#endregion

		#region SwitchCase

		public static void Deconstruct(this SwitchCase switchCase, out Expr body, out ReadOnlyCollection<Expression> testValues)
		{
			body       = switchCase.Body.ToExpr();
			testValues = switchCase.TestValues;
		}

		#endregion
	}
}
