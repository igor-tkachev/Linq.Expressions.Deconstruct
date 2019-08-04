using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable

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

		public static implicit operator Expression?(Expr? expr) => expr?.GetExpression();
		public static implicit operator Expr?(Expression? expr) => expr.ToExpr();

		#endregion

		#region Call

		public partial class Call
		{
			public void Deconstruct(out Type type, out MethodInfo method, out Expr? @object, out ReadOnlyCollection<Expression> arguments)
			{
				type      = Expr.Type;
				method    = Expr.Method;
				@object   = Expr.Object.ToExpr();
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out MethodInfo method, out Expr? @object, out ReadOnlyCollection<Expression> arguments)
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
				test    = Expr.Test.   ToExpr2();
				ifTrue  = Expr.IfTrue. ToExpr2();
				ifFalse = Expr.IfFalse.ToExpr2();
			}

			public void Deconstruct(out Expr test, out Expr ifTrue, out Expr ifFalse)
			{
				test    = Expr.Test.   ToExpr2();
				ifTrue  = Expr.IfTrue. ToExpr2();
				ifFalse = Expr.IfFalse.ToExpr2();
			}
		}

		#endregion

		#region Invoke

		public partial class Invoke
		{
			public void Deconstruct(out Type type, out Expr expression, out ReadOnlyCollection<Expression> arguments)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr2();
				arguments  = Expr.Arguments;
			}

			public void Deconstruct(out Expr expression, out ReadOnlyCollection<Expression> arguments)
			{
				expression = Expr.Expression.ToExpr2();
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
				body       = Expr.Body.ToExpr2();
				parameters = Expr.Parameters;
			}

			public void Deconstruct(out Expr body, out ReadOnlyCollection<ParameterExpression> parameters)
			{
				body       = Expr.Body.ToExpr2();
				parameters = Expr.Parameters;
			}
		}

		#endregion

		#region ListInit

		public partial class ListInit
		{
			public void Deconstruct(out Type type, out New newExpression, out ReadOnlyCollection<ElementInit> initializers)
			{
				type          = Expr.Type;
				newExpression = new New(Expr.NewExpression);
				initializers  = Expr.Initializers;
			}

			public void Deconstruct(out Expr newExpression, out ReadOnlyCollection<ElementInit> initializers)
			{
				newExpression = new New(Expr.NewExpression);
				initializers  = Expr.Initializers;
			}
		}

		#endregion

		#region Member

		public partial class Member
		{
			public void Deconstruct(out Type type, out Expr? expression)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr();
			}

			public void Deconstruct(out Expr? expression)
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
				out Type                            type,
				out ConstructorInfo                 constructor,
				out ReadOnlyCollection<MemberInfo>? members,
				out ReadOnlyCollection<Expression>  arguments)
			{
				type        = Expr.Type;
				constructor = Expr.Constructor;
				members     = Expr.Members;
				arguments   = Expr.Arguments;
			}

			public void Deconstruct(
				out ConstructorInfo                 constructor,
				out ReadOnlyCollection<MemberInfo>? members,
				out ReadOnlyCollection<Expression>  arguments)
			{
				constructor = Expr.Constructor;
				members     = Expr.Members;
				arguments   = Expr.Arguments;
			}

			public void Deconstruct(
				out ReadOnlyCollection<MemberInfo>? members,
				out ReadOnlyCollection<Expression>  arguments)
			{
				members     = Expr.Members;
				arguments   = Expr.Arguments;
			}

			public void Deconstruct(
				out ReadOnlyCollection<Expression> arguments)
			{
				arguments = Expr.Arguments;
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
				expression = Expr.Expression.ToExpr2();
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expr.Expression.ToExpr2();
			}
		}

		#endregion

		#region TypeIs

		public partial class TypeIs
		{
			public void Deconstruct(out Type type, out Expr expression)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr2();
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expr.Expression.ToExpr2();
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
				value  = Expr.Value.ToExpr2();
			}

			public void Deconstruct(out LabelTarget target, out Expr value)
			{
				target = Expr.Target;
				value  = Expr.Value.ToExpr2();
			}
		}

		#endregion

		#region Index

		public partial class Index
		{
			public void Deconstruct(out Type type, out Expr @object, out PropertyInfo indexer, out ReadOnlyCollection<Expression> arguments)
			{
				type      = Expr.Type;
				@object   = Expr.Object.ToExpr2();
				indexer   = Expr.Indexer;
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out Expr @object, out PropertyInfo indexer, out ReadOnlyCollection<Expression> arguments)
			{
				@object   = Expr.Object.ToExpr2();
				indexer   = Expr.Indexer;
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out Expr @object, out ReadOnlyCollection<Expression> arguments)
			{
				@object   = Expr.Object.ToExpr2();
				arguments = Expr.Arguments;
			}
		}

		#endregion

		#region Label

		public partial class Label
		{
			public void Deconstruct(out Type type, out LabelTarget target, out Expr? defaultValue)
			{
				type         = Expr.Type;
				target       = Expr.Target;
				defaultValue = Expr.DefaultValue.ToExpr();
			}

			public void Deconstruct(out LabelTarget target, out Expr? defaultValue)
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
				body          = Expr.Body.ToExpr2();
			}

			public void Deconstruct(out LabelTarget breakLabel, out LabelTarget continueLabel, out Expr body)
			{
				breakLabel    = Expr.BreakLabel;
				continueLabel = Expr.ContinueLabel;
				body          = Expr.Body.ToExpr2();
			}
		}

		#endregion

		#region Switch

		public partial class Switch
		{
			public void Deconstruct(out Type type, out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr? defaultBody)
			{
				type        = Expr.Type;
				switchValue = Expr.SwitchValue.ToExpr2();
				cases       = Expr.Cases;
				defaultBody = Expr.DefaultBody.ToExpr();
			}

			public void Deconstruct(out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr? defaultBody)
			{
				switchValue = Expr.SwitchValue.ToExpr2();
				cases       = Expr.Cases;
				defaultBody = Expr.DefaultBody.ToExpr();
			}
		}

		#endregion

		#region Try

		public partial class Try
		{
			public void Deconstruct(out Type type, out Expr body, out IEnumerable<CatchBlock> handlers, out Expr? @finally, out Expr? fault)
			{
				type     = Expr.Type;
				body     = Expr.Body.   ToExpr2();
				handlers = Expr.Handlers;
				@finally = Expr.Finally.ToExpr();
				fault    = Expr.Fault.  ToExpr();
			}

			public void Deconstruct(out Expr body, out IEnumerable<CatchBlock> handlers, out Expr? @finally, out Expr? fault)
			{
				body     = Expr.Body.   ToExpr2();
				handlers = Expr.Handlers;
				@finally = Expr.Finally.ToExpr();
				fault    = Expr.Fault.  ToExpr();
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
		public static Expr? ToExpr(this Expression? expr)
		{
			return expr?.ToExpr2();
		}

		internal static Expr ToExpr2(this Expression expr)
		{
			if (expr == null)
				throw new ArgumentNullException(nameof(expr));

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

		public static void Deconstruct(this ElementInit elementInit, out ReadOnlyCollection<Expression> arguments)
		{
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

		public static void Deconstruct(this SwitchCase switchCase, out Expr? body, out ReadOnlyCollection<Expression> testValues)
		{
			body       = switchCase.Body.ToExpr();
			testValues = switchCase.TestValues;
		}

		#endregion

		#region Transform

		static IEnumerable<T> TransformInternal<T>(ICollection<T> source, Func<T,T> func)
			where T : class
		{
			var modified = false;
			var list = new List<T>();

			foreach (var item in source)
			{
				var e = func(item);
				list.Add(e);
				modified = modified || e != item;
			}

			return modified ? list : source;
		}

		static IEnumerable<T?> TransformInternal<T>(ICollection<T> source, Func<Expression?,Expression?> func)
			where T : Expression
		{
			var modified = false;
			var list     = new List<T?>();

			foreach (var item in source)
			{
				var e = TransformInternal(item, func);
				list.Add((T?)e);
				modified = modified || e != item;
			}

			return modified ? (IEnumerable<T?>)list : (IEnumerable<T?>)source;
		}

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <typeparam name="T">Type of expression.</typeparam>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		public static T Transform<T>(this T expr, Func<Expression?,Expression?> func)
			where T : LambdaExpression
		{
			return (T)(TransformInternal(expr, func) ?? throw new InvalidOperationException());
		}

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		public static Expression? Transform(this Expression? expr, Func<Expression?,Expression?> func)
		{
			return TransformInternal(expr, func);
		}

		static Expression? TransformInternal(this Expression? expr, Func<Expression?,Expression?> func)
		{
			var ex = TransformInternal2(expr, func);
			return ex == null ? null : func(ex);
		}

		static Expression? TransformInternal2(this Expression? expr, Func<Expression?,Expression?> func)
		{
			if (expr == null)
				return null;

			switch (expr.NodeType)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.ArrayIndex:
				case ExpressionType.Assign:
				case ExpressionType.Coalesce:
				case ExpressionType.Divide:
				case ExpressionType.Equal:
				case ExpressionType.ExclusiveOr:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LeftShift:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.NotEqual:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.Power:
				case ExpressionType.RightShift:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.AddAssign:
				case ExpressionType.AndAssign:
				case ExpressionType.DivideAssign:
				case ExpressionType.ExclusiveOrAssign:
				case ExpressionType.LeftShiftAssign:
				case ExpressionType.ModuloAssign:
				case ExpressionType.MultiplyAssign:
				case ExpressionType.OrAssign:
				case ExpressionType.PowerAssign:
				case ExpressionType.RightShiftAssign:
				case ExpressionType.SubtractAssign:
				case ExpressionType.AddAssignChecked:
				case ExpressionType.MultiplyAssignChecked:
				case ExpressionType.SubtractAssignChecked:
				{
					var e = (BinaryExpression)expr;
					return e.Update(
						TransformInternal(e.Left, func),
						(LambdaExpression?)TransformInternal(e.Conversion, func),
						TransformInternal(e.Right, func));
				}

				case ExpressionType.ArrayLength:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
				case ExpressionType.UnaryPlus:
				case ExpressionType.Decrement:
				case ExpressionType.Increment:
				case ExpressionType.IsFalse:
				case ExpressionType.IsTrue:
				case ExpressionType.Throw:
				case ExpressionType.Unbox:
				case ExpressionType.PreIncrementAssign:
				case ExpressionType.PreDecrementAssign:
				case ExpressionType.PostIncrementAssign:
				case ExpressionType.PostDecrementAssign:
				case ExpressionType.OnesComplement:
				{
					var e = (UnaryExpression)expr;
					return e.Update(TransformInternal(e.Operand, func));
				}

				case ExpressionType.Call:
				{
					var e = (MethodCallExpression)expr;
					return e.Update(
						TransformInternal(e.Object, func),
						TransformInternal(e.Arguments, func));
				}

				case ExpressionType.Conditional:
				{
					var e = (ConditionalExpression)expr;
					return e.Update(
						TransformInternal(e.Test, func),
						TransformInternal(e.IfTrue, func),
						TransformInternal(e.IfFalse, func));
				}

				case ExpressionType.Invoke:
				{
					var e = (InvocationExpression)expr;
					return e.Update(
						TransformInternal(e.Expression, func),
						TransformInternal(e.Arguments, func));
				}

				case ExpressionType.Lambda:
				{
					var e = (LambdaExpression)expr;
					var b = TransformInternal(e.Body, func);
					var p = TransformInternal(e.Parameters, func);

					return b != e.Body || !ReferenceEquals(p, e.Parameters) ? Expression.Lambda(expr.Type, b, p.ToArray()) : expr;
				}

				case ExpressionType.ListInit:
				{
					var e = (ListInitExpression)expr;
					return e.Update(
						(NewExpression?)TransformInternal(e.NewExpression, func),
						TransformInternal(
							e.Initializers, p =>
							{
								var args = TransformInternal(p.Arguments, func);
								return !ReferenceEquals(args, p.Arguments) ? Expression.ElementInit(p.AddMethod, args) : p;
							}));
				}

				case ExpressionType.MemberAccess:
				{
					var e = (MemberExpression)expr;
					return e.Update(TransformInternal(e.Expression, func));
				}

				case ExpressionType.MemberInit:
				{
					MemberBinding Modify(MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment:
							{
								var ma = (MemberAssignment)b;
								return ma.Update(TransformInternal(ma.Expression, func));
							}

							case MemberBindingType.ListBinding:
							{
								var ml = (MemberListBinding)b;
								return ml.Update(TransformInternal(ml.Initializers, p =>
								{
									var args = TransformInternal(p.Arguments, func);
									return !ReferenceEquals(args, p.Arguments) ? Expression.ElementInit(p.AddMethod, args) : p;
								}));
							}

							case MemberBindingType.MemberBinding:
							{
								var mm = (MemberMemberBinding)b;
								return mm.Update(TransformInternal(mm.Bindings, Modify));
							}
						}

						return b;
					}

					var e = (MemberInitExpression)expr;
					return e.Update(
						(NewExpression?)TransformInternal(e.NewExpression, func),
						TransformInternal(e.Bindings, Modify));
				}

				case ExpressionType.New:
				{
					var e = (NewExpression)expr;
					return e.Update(TransformInternal(e.Arguments, func));
				}

				case ExpressionType.NewArrayBounds:
				case ExpressionType.NewArrayInit:
				{
					var e = (NewArrayExpression)expr;
					return e.Update(TransformInternal(e.Expressions, func));
				}

				case ExpressionType.TypeEqual:
				case ExpressionType.TypeIs:
				{
					var e = (TypeBinaryExpression)expr;
					return e.Update(TransformInternal(e.Expression, func));
				}

				case ExpressionType.Block:
				{
					var e = (BlockExpression)expr;
					return e.Update(
						TransformInternal(e.Variables, func),
						TransformInternal(e.Expressions, func));
				}

				case ExpressionType.DebugInfo:
				case ExpressionType.Default:
				case ExpressionType.Extension:
				case ExpressionType.Constant:
				case ExpressionType.Parameter:
					return expr;

				case ExpressionType.Dynamic:
				{
					var e = (DynamicExpression)expr;
					return e.Update(TransformInternal(e.Arguments, func));
				}

				case ExpressionType.Goto:
				{
					var e = (GotoExpression)expr;
					return e.Update(e.Target, TransformInternal(e.Value, func));
				}

				case ExpressionType.Index:
				{
					var e = (IndexExpression)expr;
					return e.Update(
						TransformInternal(e.Object, func),
						TransformInternal(e.Arguments, func));
				}

				case ExpressionType.Label:
				{
					var e = (LabelExpression)expr;
					return e.Update(e.Target, TransformInternal(e.DefaultValue, func));
				}

				case ExpressionType.RuntimeVariables:
				{
					var e = (RuntimeVariablesExpression)expr;
					return e.Update(TransformInternal(e.Variables, func));
				}

				case ExpressionType.Loop:
				{
					var e = (LoopExpression)expr;
					return e.Update(e.BreakLabel, e.ContinueLabel, TransformInternal(e.Body, func));
				}

				case ExpressionType.Switch:
				{
					var e = (SwitchExpression)expr;
					return e.Update(
						TransformInternal(e.SwitchValue, func),
						TransformInternal(
							e.Cases, cs => cs.Update(TransformInternal(cs.TestValues, func), TransformInternal(cs.Body, func))),
						TransformInternal(e.DefaultBody, func));
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;
					return e.Update(
						TransformInternal(e.Body, func),
						TransformInternal(
							e.Handlers,
							h =>
								h.Update(
									(ParameterExpression?)TransformInternal(h.Variable, func), TransformInternal(h.Filter, func),
									TransformInternal(h.Body, func))),
						TransformInternal(e.Finally, func),
						TransformInternal(e.Fault, func));
				}
			}

			throw new InvalidOperationException();
		}

		#endregion
	}
}
