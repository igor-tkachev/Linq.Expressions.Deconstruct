using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#if !NETCOREAPP3_1

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	internal sealed class NotNullIfNotNullAttribute : Attribute
	{
		public NotNullIfNotNullAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class NotNullAttribute : Attribute
	{
	}

}

#endif

namespace Linq.Expressions.Deconstruct
{
	public abstract partial class Expr
	{
		#region overrides

		protected abstract Expression GetExpression();

		public override string ToString   () => GetExpression().ToString();
		public override int    GetHashCode() => GetExpression().GetHashCode();

		public override bool Equals(object? obj)
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

		[return: NotNullIfNotNull("expr")]
		public static implicit operator Expression?(Expr? expr) => expr?.GetExpression();

		[return: NotNullIfNotNull("expr")]
		public static implicit operator Expr?(Expression? expr) => expr?.ToExpr();

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
				test    = Expr.Test.   ToExpr()!;
				ifTrue  = Expr.IfTrue. ToExpr()!;
				ifFalse = Expr.IfFalse.ToExpr()!;
			}

			public void Deconstruct(out Expr test, out Expr ifTrue, out Expr ifFalse)
			{
				test    = Expr.Test.   ToExpr()!;
				ifTrue  = Expr.IfTrue. ToExpr()!;
				ifFalse = Expr.IfFalse.ToExpr()!;
			}
		}

		#endregion

		#region Invoke

		public partial class Invoke
		{
			public void Deconstruct(out Type type, out Expr expression, out ReadOnlyCollection<Expression> arguments)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr()!;
				arguments  = Expr.Arguments;
			}

			public void Deconstruct(out Expr expression, out ReadOnlyCollection<Expression> arguments)
			{
				expression = Expr.Expression.ToExpr()!;
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
				body       = Expr.Body.ToExpr()!;
				parameters = Expr.Parameters;
			}

			public void Deconstruct(out Expr body, out ReadOnlyCollection<ParameterExpression> parameters)
			{
				body       = Expr.Body.ToExpr()!;
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
				expression = Expr.Expression.ToExpr()!;
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expr.Expression.ToExpr()!;
			}
		}

		#endregion

		#region TypeIs

		public partial class TypeIs
		{
			public void Deconstruct(out Type type, out Expr expression)
			{
				type       = Expr.Type;
				expression = Expr.Expression.ToExpr()!;
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expr.Expression.ToExpr()!;
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
				value  = Expr.Value.ToExpr()!;
			}

			public void Deconstruct(out LabelTarget target, out Expr value)
			{
				target = Expr.Target;
				value  = Expr.Value.ToExpr()!;
			}
		}

		#endregion

		#region Index

		public partial class Index
		{
			public void Deconstruct(out Type type, out Expr @object, out PropertyInfo indexer, out ReadOnlyCollection<Expression> arguments)
			{
				type      = Expr.Type;
				@object   = Expr.Object.ToExpr()!;
				indexer   = Expr.Indexer;
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out Expr @object, out PropertyInfo indexer, out ReadOnlyCollection<Expression> arguments)
			{
				@object   = Expr.Object.ToExpr()!;
				indexer   = Expr.Indexer;
				arguments = Expr.Arguments;
			}

			public void Deconstruct(out Expr @object, out ReadOnlyCollection<Expression> arguments)
			{
				@object   = Expr.Object.ToExpr()!;
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
				body          = Expr.Body.ToExpr()!;
			}

			public void Deconstruct(out LabelTarget breakLabel, out LabelTarget continueLabel, out Expr body)
			{
				breakLabel    = Expr.BreakLabel;
				continueLabel = Expr.ContinueLabel;
				body          = Expr.Body.ToExpr()!;
			}
		}

		#endregion

		#region Switch

		public partial class Switch
		{
			public void Deconstruct(out Type type, out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr? defaultBody)
			{
				type        = Expr.Type;
				switchValue = Expr.SwitchValue.ToExpr()!;
				cases       = Expr.Cases;
				defaultBody = Expr.DefaultBody.ToExpr();
			}

			public void Deconstruct(out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr? defaultBody)
			{
				switchValue = Expr.SwitchValue.ToExpr()!;
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
				body     = Expr.Body.   ToExpr()!;
				handlers = Expr.Handlers;
				@finally = Expr.Finally.ToExpr();
				fault    = Expr.Fault.  ToExpr();
			}

			public void Deconstruct(out Expr body, out IEnumerable<CatchBlock> handlers, out Expr? @finally, out Expr? fault)
			{
				body     = Expr.Body.   ToExpr()!;
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
			public void Deconstruct(out object value)
			{
				value = Expr.Value;
			}

			public void Deconstruct(out object origValue, out int? intValue, out Type type)
			{
				Deconstruct(out origValue, out intValue);
				type = Expr.Type;
			}

			public void Deconstruct(out object origValue, out int? intValue)
			{
				origValue = Expr.Value;
				intValue  = null;

				if (Expr.Value != null)
				{
					switch (Type.GetTypeCode(Expr.Value.GetType()))
					{
						//case TypeCode.Boolean : intValue = (Boolean)Expr.Value ? 1 : 0; break;
						case TypeCode.Char    : intValue = (Char)  Expr.Value; break;
						case TypeCode.SByte   : intValue = (SByte) Expr.Value; break;
						case TypeCode.Byte    : intValue = (Byte)  Expr.Value; break;
						case TypeCode.Int16   : intValue = (Int16) Expr.Value; break;
						case TypeCode.UInt16  : intValue = (UInt16)Expr.Value; break;
						case TypeCode.Int32   : intValue = (Int32) Expr.Value; break;
						case TypeCode.UInt32  :
							{
								var value = (UInt32)Expr.Value;
								if (value <= int.MaxValue)
									intValue = (int)value;
								break;
							}
						case TypeCode.Int64   :
							{
								var value = (Int64)Expr.Value;
								if (value <= int.MaxValue)
									intValue = (int)value;
								break;
							}
						case TypeCode.UInt64  :
							{
								var value = (UInt64)Expr.Value;
								if (value <= int.MaxValue)
									intValue = (int)value;
								break;
							}
						case TypeCode.Single  :
							{
								var value = (Single)Expr.Value;
								if (value >= int.MinValue && value <= int.MaxValue)
								{
									var v = (int)value;
									if ((Single)v == value)
										intValue = v;
								}
								break;
							}
						case TypeCode.Double  :
							{
								var value = (Double)Expr.Value;
								if (value >= int.MinValue && value <= int.MaxValue)
								{
									var v = (int)value;
									if ((Double)v == value)
										intValue = v;
								}
								break;
							}
						case TypeCode.Decimal :
							{
								var value = (Decimal)Expr.Value;
								if (value >= int.MinValue && value <= int.MaxValue)
								{
									var v = (int)value;
									if ((Decimal)v == value)
										intValue = v;
								}
								break;
							}
					}
				}
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
		#region ToExpr

		[return: NotNullIfNotNull("expr")]
		public static Expr? ToExpr(this Expression? expr)
		{
			if (expr == null)
				return null;

			return expr.NodeType switch
			{
				ExpressionType.Add                   => new Expr.Add                       ((BinaryExpression)expr),
				ExpressionType.AddChecked            => new Expr.AddChecked                ((BinaryExpression)expr),
				ExpressionType.And                   => new Expr.And                       ((BinaryExpression)expr),
				ExpressionType.AndAlso               => new Expr.AndAlso                   ((BinaryExpression)expr),
				ExpressionType.ArrayIndex            => new Expr.ArrayIndex                ((BinaryExpression)expr),
				ExpressionType.Assign                => new Expr.Assign                    ((BinaryExpression)expr),
				ExpressionType.Coalesce              => new Expr.Coalesce                  ((BinaryExpression)expr),
				ExpressionType.Divide                => new Expr.Divide                    ((BinaryExpression)expr),
				ExpressionType.Equal                 => new Expr.Equal                     ((BinaryExpression)expr),
				ExpressionType.ExclusiveOr           => new Expr.ExclusiveOr               ((BinaryExpression)expr),
				ExpressionType.GreaterThan           => new Expr.GreaterThan               ((BinaryExpression)expr),
				ExpressionType.GreaterThanOrEqual    => new Expr.GreaterThanOrEqual        ((BinaryExpression)expr),
				ExpressionType.LeftShift             => new Expr.LeftShift                 ((BinaryExpression)expr),
				ExpressionType.LessThan              => new Expr.LessThan                  ((BinaryExpression)expr),
				ExpressionType.LessThanOrEqual       => new Expr.LessThanOrEqual           ((BinaryExpression)expr),
				ExpressionType.Modulo                => new Expr.Modulo                    ((BinaryExpression)expr),
				ExpressionType.Multiply              => new Expr.Multiply                  ((BinaryExpression)expr),
				ExpressionType.MultiplyChecked       => new Expr.MultiplyChecked           ((BinaryExpression)expr),
				ExpressionType.NotEqual              => new Expr.NotEqual                  ((BinaryExpression)expr),
				ExpressionType.Or                    => new Expr.Or                        ((BinaryExpression)expr),
				ExpressionType.OrElse                => new Expr.OrElse                    ((BinaryExpression)expr),
				ExpressionType.Power                 => new Expr.Power                     ((BinaryExpression)expr),
				ExpressionType.RightShift            => new Expr.RightShift                ((BinaryExpression)expr),
				ExpressionType.Subtract              => new Expr.Subtract                  ((BinaryExpression)expr),
				ExpressionType.SubtractChecked       => new Expr.SubtractChecked           ((BinaryExpression)expr),
				ExpressionType.AddAssign             => new Expr.AddAssign                 ((BinaryExpression)expr),
				ExpressionType.AndAssign             => new Expr.AndAssign                 ((BinaryExpression)expr),
				ExpressionType.DivideAssign          => new Expr.DivideAssign              ((BinaryExpression)expr),
				ExpressionType.ExclusiveOrAssign     => new Expr.ExclusiveOrAssign         ((BinaryExpression)expr),
				ExpressionType.LeftShiftAssign       => new Expr.LeftShiftAssign           ((BinaryExpression)expr),
				ExpressionType.ModuloAssign          => new Expr.ModuloAssign              ((BinaryExpression)expr),
				ExpressionType.MultiplyAssign        => new Expr.MultiplyAssign            ((BinaryExpression)expr),
				ExpressionType.OrAssign              => new Expr.OrAssign                  ((BinaryExpression)expr),
				ExpressionType.PowerAssign           => new Expr.PowerAssign               ((BinaryExpression)expr),
				ExpressionType.RightShiftAssign      => new Expr.RightShiftAssign          ((BinaryExpression)expr),
				ExpressionType.SubtractAssign        => new Expr.SubtractAssign            ((BinaryExpression)expr),
				ExpressionType.AddAssignChecked      => new Expr.AddAssignChecked          ((BinaryExpression)expr),
				ExpressionType.MultiplyAssignChecked => new Expr.MultiplyAssignChecked     ((BinaryExpression)expr),
				ExpressionType.SubtractAssignChecked => new Expr.SubtractAssignChecked     ((BinaryExpression)expr),
				ExpressionType.ArrayLength           => new Expr.ArrayLength               ((UnaryExpression)expr),
				ExpressionType.Convert               => new Expr.Convert                   ((UnaryExpression)expr),
				ExpressionType.ConvertChecked        => new Expr.ConvertChecked            ((UnaryExpression)expr),
				ExpressionType.Negate                => new Expr.Negate                    ((UnaryExpression)expr),
				ExpressionType.NegateChecked         => new Expr.NegateChecked             ((UnaryExpression)expr),
				ExpressionType.Not                   => new Expr.Not                       ((UnaryExpression)expr),
				ExpressionType.Quote                 => new Expr.Quote                     ((UnaryExpression)expr),
				ExpressionType.TypeAs                => new Expr.TypeAs                    ((UnaryExpression)expr),
				ExpressionType.UnaryPlus             => new Expr.UnaryPlus                 ((UnaryExpression)expr),
				ExpressionType.Decrement             => new Expr.Decrement                 ((UnaryExpression)expr),
				ExpressionType.Increment             => new Expr.Increment                 ((UnaryExpression)expr),
				ExpressionType.IsFalse               => new Expr.IsFalse                   ((UnaryExpression)expr),
				ExpressionType.IsTrue                => new Expr.IsTrue                    ((UnaryExpression)expr),
				ExpressionType.Throw                 => new Expr.Throw                     ((UnaryExpression)expr),
				ExpressionType.Unbox                 => new Expr.Unbox                     ((UnaryExpression)expr),
				ExpressionType.PreIncrementAssign    => new Expr.PreIncrementAssign        ((UnaryExpression)expr),
				ExpressionType.PreDecrementAssign    => new Expr.PreDecrementAssign        ((UnaryExpression)expr),
				ExpressionType.PostIncrementAssign   => new Expr.PostIncrementAssign       ((UnaryExpression)expr),
				ExpressionType.PostDecrementAssign   => new Expr.PostDecrementAssign       ((UnaryExpression)expr),
				ExpressionType.OnesComplement        => new Expr.OnesComplement            ((UnaryExpression)expr),
				ExpressionType.Call                  => new Expr.Call                      ((MethodCallExpression)expr),
				ExpressionType.Conditional           => new Expr.Conditional               ((ConditionalExpression)expr),
				ExpressionType.Invoke                => new Expr.Invoke                    ((InvocationExpression)expr),
				ExpressionType.Lambda                => new Expr.Lambda                    ((LambdaExpression)expr),
				ExpressionType.ListInit              => new Expr.ListInit                  ((ListInitExpression)expr),
				ExpressionType.MemberAccess          => new Expr.Member                    ((MemberExpression)expr),
				ExpressionType.MemberInit            => new Expr.MemberInit                ((MemberInitExpression)expr),
				ExpressionType.New                   => new Expr.New                       ((NewExpression)expr),
				ExpressionType.NewArrayBounds        => new Expr.NewArrayBounds            ((NewArrayExpression)expr),
				ExpressionType.NewArrayInit          => new Expr.NewArrayInit              ((NewArrayExpression)expr),
				ExpressionType.TypeEqual             => new Expr.TypeEqual                 ((TypeBinaryExpression)expr),
				ExpressionType.TypeIs                => new Expr.TypeIs                    ((TypeBinaryExpression)expr),
				ExpressionType.Block                 => new Expr.Block                     ((BlockExpression)expr),
				ExpressionType.Dynamic               => new Expr.Dynamic                   ((DynamicExpression)expr),
				ExpressionType.Goto                  => new Expr.Goto                      ((GotoExpression)expr),
				ExpressionType.Index                 => new Expr.Index                     ((IndexExpression)expr),
				ExpressionType.Label                 => new Expr.Label                     ((LabelExpression)expr),
				ExpressionType.RuntimeVariables      => new Expr.RuntimeVariables          ((RuntimeVariablesExpression)expr),
				ExpressionType.Loop                  => new Expr.Loop                      ((LoopExpression)expr),
				ExpressionType.Switch                => new Expr.Switch                    ((SwitchExpression)expr),
				ExpressionType.Try                   => new Expr.Try                       ((TryExpression)expr),
				ExpressionType.Extension             => new Expr.Extension                 (expr),
				ExpressionType.DebugInfo             => new Expr.DebugInfo                 ((DebugInfoExpression)expr),
				ExpressionType.Parameter             => new Expr.Parameter                 ((ParameterExpression)expr),
				ExpressionType.Constant              => new Expr.Constant                  ((ConstantExpression)expr),
				ExpressionType.Default               => new Expr.Default                   ((DefaultExpression)expr),
				_                                    => throw new InvalidOperationException()
			};
		}

		#endregion

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

		#region Visit

		/// <summary>
		/// Visits expression tree.
		/// </summary>
		/// <param name="expr"><see cref="Expression"/> to visit.</param>
		/// <param name="func">Visit action.</param>
		public static void Visit(this Expression? expr, Action<Expression> func)
		{
			VisitInternal(expr, func);
		}

		/// <summary>
		/// Visits expression tree.
		/// </summary>
		/// <param name="expr"><see cref="Expression"/> to visit.</param>
		/// <param name="func">Visit action.</param>
		public static void VisitEx(this Expression? expr, Action<Expr> func)
		{
			VisitInternal(expr, ex => func(ex.ToExpr()!));
		}

		static void VisitInternal<T>(IEnumerable<T> source, Action<T> func)
		{
			foreach (var item in source)
				func(item);
		}

		static void VisitInternal<T>(IEnumerable<T> source, Action<Expression> func)
			where T : Expression
		{
			foreach (var item in source)
				VisitInternal(item, func);
		}

		static void VisitInternal(this Expression? expr, Action<Expression> func)
		{
			if (expr == null)
				return;

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

					VisitInternal(e.Conversion, func);
					VisitInternal(e.Left,       func);
					VisitInternal(e.Right,      func);

					break;
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
					VisitInternal(((UnaryExpression)expr).Operand, func);
					break;
				}

				case ExpressionType.Call:
				{
					var e = (MethodCallExpression)expr;

					VisitInternal(e.Object,    func);
					VisitInternal(e.Arguments, func);

					break;
				}

				case ExpressionType.Conditional:
				{
					var e = (ConditionalExpression)expr;

					VisitInternal(e.Test,    func);
					VisitInternal(e.IfTrue,  func);
					VisitInternal(e.IfFalse, func);

					break;
				}

				case ExpressionType.Invoke:
				{
					var e = (InvocationExpression)expr;

					VisitInternal(e.Expression, func);
					VisitInternal(e.Arguments,  func);

					break;
				}

				case ExpressionType.Lambda:
				{
					var e = (LambdaExpression)expr;

					VisitInternal(e.Body, func);
					VisitInternal(e.Parameters, func);

					break;
				}

				case ExpressionType.ListInit:
				{
					var e = (ListInitExpression)expr;

					VisitInternal(e.NewExpression, func);
					VisitInternal(e.Initializers, ex => VisitInternal(ex.Arguments, func));

					break;
				}

				case ExpressionType.MemberAccess:
				{
					VisitInternal(((MemberExpression)expr).Expression, func);
					break;
				}

				case ExpressionType.MemberInit:
				{
					void Action(MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment:
								VisitInternal(((MemberAssignment)b).Expression, func);
								break;
							case MemberBindingType.ListBinding:
								VisitInternal(((MemberListBinding)b).Initializers, p => VisitInternal(p.Arguments, func));
								break;
							case MemberBindingType.MemberBinding:
								VisitInternal(((MemberMemberBinding)b).Bindings, Action);
								break;
						}
					}

					var e = (MemberInitExpression)expr;

					VisitInternal(e.NewExpression, func);
					VisitInternal(e.Bindings,      Action);

					break;
				}

				case ExpressionType.New:
					VisitInternal(((NewExpression)expr).Arguments, func);
					break;
				case ExpressionType.NewArrayBounds:
					VisitInternal(((NewArrayExpression)expr).Expressions, func);
					break;
				case ExpressionType.NewArrayInit:
					VisitInternal(((NewArrayExpression)expr).Expressions, func);
					break;
				case ExpressionType.TypeEqual:
				case ExpressionType.TypeIs:
					VisitInternal(((TypeBinaryExpression)expr).Expression, func);
					break;

				case ExpressionType.Block:
				{
					var e = (BlockExpression)expr;

					VisitInternal(e.Expressions, func);
					VisitInternal(e.Variables,   func);

					break;
				}

				case ExpressionType.Dynamic:
				{
					var e = (DynamicExpression)expr;

					VisitInternal(e.Arguments, func);

					break;
				}

				case ExpressionType.Goto:
				{
					var e = (GotoExpression)expr;

					VisitInternal(e.Value, func);

					break;
				}

				case ExpressionType.Index:
				{
					var e = (IndexExpression)expr;

					VisitInternal(e.Object,    func);
					VisitInternal(e.Arguments, func);

					break;
				}

				case ExpressionType.Label:
				{
					var e = (LabelExpression)expr;

					VisitInternal(e.DefaultValue, func);

					break;
				}

				case ExpressionType.RuntimeVariables:
				{
					var e = (RuntimeVariablesExpression)expr;

					VisitInternal(e.Variables, func);

					break;
				}

				case ExpressionType.Loop:
				{
					var e = (LoopExpression)expr;

					VisitInternal(e.Body, func);

					break;
				}

				case ExpressionType.Switch:
				{
					var e = (SwitchExpression)expr;

					VisitInternal(e.SwitchValue, func);
					VisitInternal(
						e.Cases, cs =>
						{
							VisitInternal(cs.TestValues, func);
							VisitInternal(cs.Body,       func);
						});
					VisitInternal(e.DefaultBody, func);

					break;
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;

					VisitInternal(e.Body, func);
					VisitInternal(
						e.Handlers, h =>
						{
							VisitInternal(h.Variable, func);
							VisitInternal(h.Filter,   func);
							VisitInternal(h.Body,     func);
						});
					VisitInternal(e.Finally, func);
					VisitInternal(e.Fault,   func);

					break;
				}

				case ExpressionType.Extension:
				{
					if (expr.CanReduce)
						VisitInternal(expr.Reduce(), func);
					break;
				}
			}

			func(expr);
		}

		private static void VisitInternal<T>(IEnumerable<T> source, Func<T, bool> func)
		{
			foreach (var item in source)
				func(item);
		}

		static void VisitInternal<T>(IEnumerable<T> source, Func<Expression, bool> func)
			where T : Expression
		{
			foreach (var item in source)
				VisitInternal(item, func);
		}

		/// <summary>
		/// Visits expression tree.
		/// </summary>
		/// <param name="expr"><see cref="Expression"/> to visit.</param>
		/// <param name="func">Visit function. Return true to stop.</param>
		public static void Visit(this Expression? expr, Func<Expression, bool> func)
		{
			VisitInternal(expr, func);
		}

		static void VisitInternal(this Expression? expr, Func<Expression, bool> func)
		{
			if (expr == null || !func(expr))
				return;

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

					VisitInternal(e.Conversion, func);
					VisitInternal(e.Left,       func);
					VisitInternal(e.Right,      func);

					break;
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
					VisitInternal(((UnaryExpression)expr).Operand, func);
					break;
				}

				case ExpressionType.Call:
				{
					var e = (MethodCallExpression)expr;

					VisitInternal(e.Object,    func);
					VisitInternal(e.Arguments, func);

					break;
				}

				case ExpressionType.Conditional:
				{
					var e = (ConditionalExpression)expr;

					VisitInternal(e.Test,    func);
					VisitInternal(e.IfTrue,  func);
					VisitInternal(e.IfFalse, func);

					break;
				}

				case ExpressionType.Invoke:
				{
					var e = (InvocationExpression)expr;

					VisitInternal(e.Expression, func);
					VisitInternal(e.Arguments,  func);

					break;
				}

				case ExpressionType.Lambda:
				{
					var e = (LambdaExpression)expr;

					VisitInternal(e.Body,       func);
					VisitInternal(e.Parameters, func);

					break;
				}

				case ExpressionType.ListInit:
				{
					var e = (ListInitExpression)expr;

					VisitInternal(e.NewExpression, func);
					VisitInternal(e.Initializers, ex => VisitInternal(ex.Arguments, func));

					break;
				}

				case ExpressionType.MemberAccess:
					VisitInternal(((MemberExpression)expr).Expression, func);
					break;

				case ExpressionType.MemberInit:
				{
					bool Modify(MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment:
								VisitInternal(((MemberAssignment)b).Expression, func);
								break;
							case MemberBindingType.ListBinding:
								VisitInternal(((MemberListBinding)b).Initializers, p => VisitInternal(p.Arguments, func));
								break;
							case MemberBindingType.MemberBinding:
								VisitInternal(((MemberMemberBinding)b).Bindings, Modify);
								break;
						}

						return true;
					}

					var e = (MemberInitExpression)expr;

					VisitInternal(e.NewExpression, func);
					VisitInternal(e.Bindings, Modify);

					break;
				}

				case ExpressionType.New:
					VisitInternal(((NewExpression)expr).Arguments, func);
					break;
				case ExpressionType.NewArrayBounds:
					VisitInternal(((NewArrayExpression)expr).Expressions, func);
					break;
				case ExpressionType.NewArrayInit:
					VisitInternal(((NewArrayExpression)expr).Expressions, func);
					break;
				case ExpressionType.TypeEqual:
				case ExpressionType.TypeIs:
					VisitInternal(((TypeBinaryExpression)expr).Expression, func);
					break;

				case ExpressionType.Block:
				{
					var e = (BlockExpression)expr;

					VisitInternal(e.Expressions, func);
					VisitInternal(e.Variables,   func);

					break;
				}

				case ExpressionType.Dynamic:
				{
					var e = (DynamicExpression)expr;

					VisitInternal(e.Arguments, func);

					break;
				}

				case ExpressionType.Goto:
				{
					var e = (GotoExpression)expr;

					VisitInternal(e.Value, func);

					break;
				}

				case ExpressionType.Index:
				{
					var e = (IndexExpression)expr;

					VisitInternal(e.Object,    func);
					VisitInternal(e.Arguments, func);

					break;
				}

				case ExpressionType.Label:
				{
					var e = (LabelExpression)expr;

					VisitInternal(e.DefaultValue, func);

					break;
				}

				case ExpressionType.RuntimeVariables:
				{
					var e = (RuntimeVariablesExpression)expr;

					VisitInternal(e.Variables, func);

					break;
				}

				case ExpressionType.Loop:
				{
					var e = (LoopExpression)expr;

					VisitInternal(e.Body, func);

					break;
				}

				case ExpressionType.Switch:
				{
					var e = (SwitchExpression)expr;

					VisitInternal(e.SwitchValue, func);
					VisitInternal(
						e.Cases, cs =>
						{
							VisitInternal(cs.TestValues, func);
							VisitInternal(cs.Body,       func);
						});
					VisitInternal(e.DefaultBody, func);

					break;
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;

					VisitInternal(e.Body, func);
					VisitInternal(
						e.Handlers, h =>
						{
							VisitInternal(h.Variable, func);
							VisitInternal(h.Filter,   func);
							VisitInternal(h.Body,     func);
						});
					VisitInternal(e.Finally, func);
					VisitInternal(e.Fault,   func);

					break;
				}

				case ExpressionType.Extension:
				{
					if (expr.CanReduce)
						VisitInternal(expr.Reduce(), func);
					break;
				}
			}
		}
		#endregion

		#region Find

		/// <summary>
		/// Finds and expression in expression tree.
		/// </summary>
		/// <param name="expr"><see cref="Expression"/> to VisitInternal.</param>
		/// <param name="func">Find function. Return true if expression is found.</param>
		/// <returns>Found expression or null.</returns>
		public static Expression? FindEx(this Expression? expr, Func<Expr,bool> func)
		{
			return FindInternal(expr, ex => func(ex.ToExpr()!));
		}

		/// <summary>
		/// Finds an expression in expression tree.
		/// </summary>
		/// <param name="expr"><see cref="Expression"/> to VisitInternal.</param>
		/// <param name="exprToFind">Expression to find.</param>
		/// <returns>Found expression or null.</returns>
		public static Expression? Find(this Expression? expr, Expression exprToFind)
		{
			return expr.FindInternal(e => e == exprToFind);
		}

		/// <summary>
		/// Finds and expression in expression tree.
		/// </summary>
		/// <param name="expr"><see cref="Expression"/> to VisitInternal.</param>
		/// <param name="func">Find function. Return true if expression is found.</param>
		/// <returns>Found expression or null.</returns>
		[return: NotNullIfNotNull("expr")]
		public static Expression? Find(this Expression? expr, Func<Expression, bool> func)
		{
			return FindInternal(expr, func);
		}

		static Expression? FindInternal<T>(IEnumerable<T> source, Func<T,Expression?> func)
		{
			foreach (var item in source)
			{
				var ex = func(item);
				if (ex != null)
					return ex;
			}

			return null;
		}

		static Expression? FindInternal<T>(IEnumerable<T> source, Func<Expression,bool> func)
			where T : Expression
		{
			foreach (var item in source)
				return FindInternal(item, func);
			return null;
		}

		static Expression? FindInternal(this Expression? expr, Func<Expression,bool> func)
		{
			if (expr == null || func(expr))
				return expr;

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

					return FindInternal(e.Conversion, func) ??
						FindInternal(e.Left, func) ??
							FindInternal(e.Right, func);
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
					return FindInternal(((UnaryExpression)expr).Operand, func);

				case ExpressionType.Call:
				{
					var e = (MethodCallExpression)expr;
					return FindInternal(e.Object, func) ?? FindInternal(e.Arguments, func);
				}

				case ExpressionType.Conditional:
				{
					var e = (ConditionalExpression)expr;
					return FindInternal(e.Test, func) ?? FindInternal(e.IfTrue, func) ?? FindInternal(e.IfFalse, func);
				}

				case ExpressionType.Invoke:
				{
					var e = (InvocationExpression)expr;
					return FindInternal(e.Expression, func) ?? FindInternal(e.Arguments, func);
				}

				case ExpressionType.Lambda:
				{
					var e = (LambdaExpression)expr;
					return FindInternal(e.Body, func) ?? FindInternal(e.Parameters, func);
				}

				case ExpressionType.ListInit:
				{
					var e = (ListInitExpression)expr;
					return FindInternal(e.NewExpression, func) ?? FindInternal(e.Initializers, ex => FindInternal(ex.Arguments, func));
				}

				case ExpressionType.MemberAccess:
					return FindInternal(((MemberExpression)expr).Expression, func);

				case ExpressionType.MemberInit:
				{
					Expression? Func(MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment:
								return FindInternal(((MemberAssignment)b).Expression, func);
							case MemberBindingType.ListBinding:
								return FindInternal(((MemberListBinding)b).Initializers, p => FindInternal(p.Arguments, func));
							case MemberBindingType.MemberBinding:
								return FindInternal(((MemberMemberBinding)b).Bindings, Func);
						}

						return null;
					}

					var e = (MemberInitExpression)expr;

					return FindInternal(e.NewExpression, func) ?? FindInternal(e.Bindings, Func);
				}

				case ExpressionType.New:
					return FindInternal(((NewExpression)expr).Arguments, func);
				case ExpressionType.NewArrayBounds:
					return FindInternal(((NewArrayExpression)expr).Expressions, func);
				case ExpressionType.NewArrayInit:
					return FindInternal(((NewArrayExpression)expr).Expressions, func);
				case ExpressionType.TypeEqual:
				case ExpressionType.TypeIs:
					return FindInternal(((TypeBinaryExpression)expr).Expression, func);

				case ExpressionType.Block:
				{
					var e = (BlockExpression)expr;
					return FindInternal(e.Expressions, func) ?? FindInternal(e.Variables, func);
				}

				case ExpressionType.Dynamic:
				{
					var e = (DynamicExpression)expr;
					return FindInternal(e.Arguments, func);
				}

				case ExpressionType.Goto:
				{
					var e = (GotoExpression)expr;
					return FindInternal(e.Value, func);
				}

				case ExpressionType.Index:
				{
					var e = (IndexExpression)expr;
					return FindInternal(e.Object, func) ?? FindInternal(e.Arguments, func);
				}

				case ExpressionType.Label:
				{
					var e = (LabelExpression)expr;
					return FindInternal(e.DefaultValue, func);
				}

				case ExpressionType.RuntimeVariables:
				{
					var e = (RuntimeVariablesExpression)expr;
					return FindInternal(e.Variables, func);
				}

				case ExpressionType.Loop:
				{
					var e = (LoopExpression)expr;
					return FindInternal(e.Body, func);
				}

				case ExpressionType.Switch:
				{
					var e = (SwitchExpression)expr;
					return FindInternal(e.SwitchValue, func) ??
						FindInternal(e.Cases, cs => FindInternal(cs.TestValues, func) ?? FindInternal(cs.Body, func)) ??
							FindInternal(e.DefaultBody, func);
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;
					return FindInternal(e.Body, func) ??
						FindInternal(
							e.Handlers, h => FindInternal(h.Variable, func) ?? FindInternal(h.Filter, func) ?? FindInternal(h.Body, func))
							??
							FindInternal(e.Finally, func) ??
								FindInternal(e.Fault, func);
				}

				case ExpressionType.Extension:
					if (expr.CanReduce)
						return FindInternal(expr.Reduce(), func);
					break;
			}

			return null;
		}

		#endregion

		#region Transform

		public static T TransformEx<T>(this T expr, Func<Expr,Expr> func)
			where T : LambdaExpression
		{
			return (T)TransformInternal(expr, ex => func(ex.ToExpr()));
		}

		[return: NotNullIfNotNull("expr")]
		public static Expression? TransformEx(this Expression? expr, Func<Expr,Expr> func)
		{
			return TransformInternal(expr, ex => func(ex.ToExpr()));
		}

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <typeparam name="T">Type of expression.</typeparam>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		public static T Transform<T>(this T expr, Func<Expression,Expression> func)
			where T : LambdaExpression
		{
			return (T)(TransformInternal(expr, func));
		}

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		[return: NotNullIfNotNull("expr")]
		public static Expression? Transform(this Expression? expr, Func<Expression,Expression> func)
		{
			return TransformInternal(expr, func);
		}

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

		static IEnumerable<T> TransformInternal<T>(ICollection<T> source, Func<Expression,Expression> func)
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

			return modified ? (IEnumerable<T>)list : (IEnumerable<T>)source;
		}

		[return: NotNullIfNotNull("expr")]
		static Expression? TransformInternal(this Expression? expr, Func<Expression,Expression> func)
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
					expr = e.Update(
						TransformInternal(e.Left, func),
						(LambdaExpression?)TransformInternal(e.Conversion, func),
						TransformInternal(e.Right, func));
					break;
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
					expr = e.Update(TransformInternal(e.Operand, func));
					break;
				}

				case ExpressionType.Call:
				{
					var e = (MethodCallExpression)expr;
					expr = e.Update(
						TransformInternal(e.Object, func),
						TransformInternal(e.Arguments, func));
					break;
				}

				case ExpressionType.Conditional:
				{
					var e = (ConditionalExpression)expr;
					expr = e.Update(
						TransformInternal(e.Test, func),
						TransformInternal(e.IfTrue, func),
						TransformInternal(e.IfFalse, func));
					break;
				}

				case ExpressionType.Invoke:
				{
					var e = (InvocationExpression)expr;
					expr = e.Update(
						TransformInternal(e.Expression, func),
						TransformInternal(e.Arguments, func));
					break;
				}

				case ExpressionType.Lambda:
				{
					var e = (LambdaExpression)expr;
					var b = TransformInternal(e.Body, func);
					var p = TransformInternal(e.Parameters, func);

					expr = b != e.Body || !ReferenceEquals(p, e.Parameters) ? Expression.Lambda(expr.Type, b, p.ToArray()) : expr;
					break;
				}

				case ExpressionType.ListInit:
				{
					var e = (ListInitExpression)expr;
					expr = e.Update(
						(NewExpression?)TransformInternal(e.NewExpression, func),
						TransformInternal(
							e.Initializers, p =>
							{
								var args = TransformInternal(p.Arguments, func);
								return !ReferenceEquals(args, p.Arguments) ? Expression.ElementInit(p.AddMethod, args) : p;
							}));
					break;
				}

				case ExpressionType.MemberAccess:
				{
					var e = (MemberExpression)expr;
					expr = e.Update(TransformInternal(e.Expression, func));
					break;
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
					expr = e.Update(
						(NewExpression?)TransformInternal(e.NewExpression, func),
						TransformInternal(e.Bindings, Modify));
					break;
				}

				case ExpressionType.New:
				{
					var e = (NewExpression)expr;
					expr = e.Update(TransformInternal(e.Arguments, func));
					break;
				}

				case ExpressionType.NewArrayBounds:
				case ExpressionType.NewArrayInit:
				{
					var e = (NewArrayExpression)expr;
					expr = e.Update(TransformInternal(e.Expressions, func));
					break;
				}

				case ExpressionType.TypeEqual:
				case ExpressionType.TypeIs:
				{
					var e = (TypeBinaryExpression)expr;
					expr = e.Update(TransformInternal(e.Expression, func));
					break;
				}

				case ExpressionType.Block:
				{
					var e = (BlockExpression)expr;
					expr = e.Update(
						TransformInternal(e.Variables, func),
						TransformInternal(e.Expressions, func));
					break;
				}

				case ExpressionType.DebugInfo:
				case ExpressionType.Default:
				case ExpressionType.Extension:
				case ExpressionType.Constant:
				case ExpressionType.Parameter:
					break;

				case ExpressionType.Dynamic:
				{
					var e = (DynamicExpression)expr;
					expr = e.Update(TransformInternal(e.Arguments, func));
					break;
				}

				case ExpressionType.Goto:
				{
					var e = (GotoExpression)expr;
					expr = e.Update(e.Target, TransformInternal(e.Value, func));
					break;
				}

				case ExpressionType.Index:
				{
					var e = (IndexExpression)expr;
					expr = e.Update(
						TransformInternal(e.Object, func),
						TransformInternal(e.Arguments, func));
					break;
				}

				case ExpressionType.Label:
				{
					var e = (LabelExpression)expr;
					expr = e.Update(e.Target, TransformInternal(e.DefaultValue, func));
					break;
				}

				case ExpressionType.RuntimeVariables:
				{
					var e = (RuntimeVariablesExpression)expr;
					expr = e.Update(TransformInternal(e.Variables, func));
					break;
				}

				case ExpressionType.Loop:
				{
					var e = (LoopExpression)expr;
					expr = e.Update(e.BreakLabel, e.ContinueLabel, TransformInternal(e.Body, func));
					break;
				}

				case ExpressionType.Switch:
				{
					var e = (SwitchExpression)expr;
					expr = e.Update(
						TransformInternal(e.SwitchValue, func),
						TransformInternal(
							e.Cases, cs => cs.Update(TransformInternal(cs.TestValues, func), TransformInternal(cs.Body, func))),
						TransformInternal(e.DefaultBody, func));
					break;
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;
					expr = e.Update(
						TransformInternal(e.Body, func),
						TransformInternal(
							e.Handlers,
							h =>
								h.Update(
									(ParameterExpression?)TransformInternal(h.Variable, func), TransformInternal(h.Filter, func),
									TransformInternal(h.Body, func))),
						TransformInternal(e.Finally, func),
						TransformInternal(e.Fault, func));
					break;
				}
			}

			return expr == null ? null : func(expr);
		}

		#endregion

		#region EqualsTo

		public static bool EqualsTo<T>(this T expr1, T expr2, bool compareConstantValues = false)
			where T : notnull, LambdaExpression
		{
			return EqualsTo(expr1, expr2, new EqualsToInfo
			{
				CompareConstantValues = compareConstantValues
			});
		}

		public static bool EqualsTo(this Expression? expr1, Expression? expr2, bool compareConstantValues = false)
		{
			if (expr1 == null || expr2 == null)
				return expr1 == expr2;

			return EqualsTo(expr1, expr2, new EqualsToInfo
			{
				CompareConstantValues = compareConstantValues
			});
		}

		class EqualsToInfo
		{
			public readonly HashSet<Expression> Visited = new HashSet<Expression>();
			public bool                         CompareConstantValues;
		}

		static bool EqualsTo(this Expression? expr1, Expression? expr2, EqualsToInfo info)
		{
			if (expr1 == expr2)
				return true;

			if (expr1 == null || expr2 == null || expr1.NodeType != expr2.NodeType || expr1.Type != expr2.Type)
				return false;

			switch (expr1.NodeType)
			{
				case ExpressionType.Add                   :
				case ExpressionType.AddChecked            :
				case ExpressionType.And                   :
				case ExpressionType.AndAlso               :
				case ExpressionType.ArrayIndex            :
				case ExpressionType.Assign                :
				case ExpressionType.Coalesce              :
				case ExpressionType.Divide                :
				case ExpressionType.Equal                 :
				case ExpressionType.ExclusiveOr           :
				case ExpressionType.GreaterThan           :
				case ExpressionType.GreaterThanOrEqual    :
				case ExpressionType.LeftShift             :
				case ExpressionType.LessThan              :
				case ExpressionType.LessThanOrEqual       :
				case ExpressionType.Modulo                :
				case ExpressionType.Multiply              :
				case ExpressionType.MultiplyChecked       :
				case ExpressionType.NotEqual              :
				case ExpressionType.Or                    :
				case ExpressionType.OrElse                :
				case ExpressionType.Power                 :
				case ExpressionType.RightShift            :
				case ExpressionType.Subtract              :
				case ExpressionType.SubtractChecked       :
				case ExpressionType.AddAssign             :
				case ExpressionType.AndAssign             :
				case ExpressionType.DivideAssign          :
				case ExpressionType.ExclusiveOrAssign     :
				case ExpressionType.LeftShiftAssign       :
				case ExpressionType.ModuloAssign          :
				case ExpressionType.MultiplyAssign        :
				case ExpressionType.OrAssign              :
				case ExpressionType.PowerAssign           :
				case ExpressionType.RightShiftAssign      :
				case ExpressionType.SubtractAssign        :
				case ExpressionType.AddAssignChecked      :
				case ExpressionType.MultiplyAssignChecked :
				case ExpressionType.SubtractAssignChecked :
					return
						((BinaryExpression)expr1).Method ==           ((BinaryExpression)expr2).Method &&
						((BinaryExpression)expr1).Conversion.EqualsTo(((BinaryExpression)expr2).Conversion, info) &&
						((BinaryExpression)expr1).Left.      EqualsTo(((BinaryExpression)expr2).Left,       info) &&
						((BinaryExpression)expr1).Right.     EqualsTo(((BinaryExpression)expr2).Right,      info);

				case ExpressionType.ArrayLength           :
				case ExpressionType.Convert               :
				case ExpressionType.ConvertChecked        :
				case ExpressionType.Negate                :
				case ExpressionType.NegateChecked         :
				case ExpressionType.Not                   :
				case ExpressionType.Quote                 :
				case ExpressionType.TypeAs                :
				case ExpressionType.UnaryPlus             :
				case ExpressionType.Decrement             :
				case ExpressionType.Increment             :
				case ExpressionType.IsFalse               :
				case ExpressionType.IsTrue                :
				case ExpressionType.Throw                 :
				case ExpressionType.Unbox                 :
				case ExpressionType.PreIncrementAssign    :
				case ExpressionType.PreDecrementAssign    :
				case ExpressionType.PostIncrementAssign   :
				case ExpressionType.PostDecrementAssign   :
				case ExpressionType.OnesComplement        :
					return
						((UnaryExpression)expr1).Method == ((UnaryExpression)expr2).Method &&
						((UnaryExpression)expr1).Operand.EqualsTo(((UnaryExpression)expr2).Operand, info);

				case ExpressionType.Conditional           :
					return
						((ConditionalExpression)expr1).Test.   EqualsTo(((ConditionalExpression)expr2).Test,    info) &&
						((ConditionalExpression)expr1).IfTrue. EqualsTo(((ConditionalExpression)expr2).IfTrue,  info) &&
						((ConditionalExpression)expr1).IfFalse.EqualsTo(((ConditionalExpression)expr2).IfFalse, info);

				case ExpressionType.TypeEqual             :
				case ExpressionType.TypeIs                :
					return
						((TypeBinaryExpression)expr1).TypeOperand == ((TypeBinaryExpression)expr2).TypeOperand &&
						((TypeBinaryExpression)expr1).Expression.EqualsTo(((TypeBinaryExpression)expr2).Expression, info);

				case ExpressionType.Goto                  :
					return
						((GotoExpression)expr1).Target.Type == ((GotoExpression)expr2).Target.Type &&
						((GotoExpression)expr1).Target.Name == ((GotoExpression)expr2).Target.Name &&
						((GotoExpression)expr1).Kind        == ((GotoExpression)expr2).Kind        &&
						((GotoExpression)expr1).Value.EqualsTo(((GotoExpression)expr2).Value);

				case ExpressionType.Label                 :
					return
						((LabelExpression)expr1).Target.Type == ((LabelExpression)expr2).Target.Type &&
						((LabelExpression)expr1).Target.Name == ((LabelExpression)expr2).Target.Name &&
						((LabelExpression)expr1).DefaultValue.EqualsTo(((LabelExpression)expr2).DefaultValue);

				case ExpressionType.Loop                  :
					return
						((LoopExpression)expr1).BreakLabel.   Type == ((LoopExpression)expr2).BreakLabel.   Type &&
						((LoopExpression)expr1).BreakLabel.   Name == ((LoopExpression)expr2).BreakLabel.   Name &&
						((LoopExpression)expr1).ContinueLabel.Type == ((LoopExpression)expr2).ContinueLabel.Type &&
						((LoopExpression)expr1).ContinueLabel.Name == ((LoopExpression)expr2).ContinueLabel.Name &&
						((LoopExpression)expr1).Body.EqualsTo(((LoopExpression)expr2).Body);

				case ExpressionType.Default               :
				case ExpressionType.Extension             :
				case ExpressionType.DebugInfo             : return true;

				case ExpressionType.Parameter             : return ((ParameterExpression) expr1).Name == ((ParameterExpression)expr2).Name;

				case ExpressionType.Call                  : return EqualsTo(info, (MethodCallExpression)      expr1, (MethodCallExpression)      expr2);
				case ExpressionType.Constant              : return EqualsTo(info, (ConstantExpression)        expr1, (ConstantExpression)        expr2);
				case ExpressionType.Invoke                : return EqualsTo(info, (InvocationExpression)      expr1, (InvocationExpression)      expr2);
				case ExpressionType.Lambda                : return EqualsTo(info, (LambdaExpression)          expr1, (LambdaExpression)          expr2);
				case ExpressionType.ListInit              : return EqualsTo(info, (ListInitExpression)        expr1, (ListInitExpression)        expr2);
				case ExpressionType.MemberAccess          : return EqualsTo(info, (MemberExpression)          expr1, (MemberExpression)          expr2);
				case ExpressionType.MemberInit            : return EqualsTo(info, (MemberInitExpression)      expr1, (MemberInitExpression)      expr2);
				case ExpressionType.New                   : return EqualsTo(info, (NewExpression)             expr1, (NewExpression)             expr2);
				case ExpressionType.NewArrayBounds        :
				case ExpressionType.NewArrayInit          : return EqualsTo(info, (NewArrayExpression)        expr1, (NewArrayExpression)        expr2);
				case ExpressionType.Block                 : return EqualsTo(info, (BlockExpression)           expr1, (BlockExpression)           expr2);
				case ExpressionType.Dynamic               : return EqualsTo(info, (DynamicExpression)         expr1, (DynamicExpression)         expr2);
				case ExpressionType.Index                 : return EqualsTo(info, (IndexExpression)           expr1, (IndexExpression)           expr2);
				case ExpressionType.RuntimeVariables      : return EqualsTo(info, (RuntimeVariablesExpression)expr1, (RuntimeVariablesExpression)expr2);
				case ExpressionType.Switch                : return EqualsTo(info, (SwitchExpression)          expr1, (SwitchExpression)          expr2);
				case ExpressionType.Try                   : return EqualsTo(info, (TryExpression)             expr1, (TryExpression)             expr2);
			}

			throw new InvalidOperationException();
		}

		static bool EqualsTo(EqualsToInfo info, TryExpression expr1, TryExpression expr2)
		{
			if (expr1.Handlers.Count != expr2.Handlers.Count ||
				!expr1.Body.   EqualsTo(expr2.Body,    info) ||
				!expr1.Finally.EqualsTo(expr2.Finally, info) ||
				!expr1.Fault.  EqualsTo(expr2.Fault,   info))
				return false;

			for (var i = 0; i < expr1.Handlers.Count; i++)
			{
				var h1 = expr1.Handlers[i];
				var h2 = expr2.Handlers[i];

				if (h1.Test != h2.Test ||
					!h1.Body.    EqualsTo(h2.Body,     info) ||
					!h1.Variable.EqualsTo(h2.Variable, info) ||
					!h1.Filter.  EqualsTo(h2.Filter,   info))
					return false;
			}

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, SwitchExpression expr1, SwitchExpression expr2)
		{
			if (expr1.Comparison  != expr2.Comparison  ||
				expr1.Cases.Count != expr2.Cases.Count ||
				expr1.SwitchValue.EqualsTo(expr2.SwitchValue) ||
				expr1.DefaultBody.EqualsTo(expr2.DefaultBody))
				return false;

			for (var i = 0; i < expr1.Cases.Count; i++)
			{
				var c1 = expr1.Cases[i];
				var c2 = expr2.Cases[i];

				if (c1.TestValues.Count != c2.TestValues.Count || !c1.Body.EqualsTo(c2.Body, info))
					return false;

				for (var j = 0; j < c1.TestValues.Count; j++)
					if (!c1.TestValues[j].EqualsTo(c2.TestValues[j], info))
						return false;
			}

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, RuntimeVariablesExpression expr1, RuntimeVariablesExpression expr2)
		{
			for (var i = 0; i < expr1.Variables.Count; i++)
				if (!expr1.Variables[i].EqualsTo(expr2.Variables[i], info))
					return false;

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, IndexExpression expr1, IndexExpression expr2)
		{
			if (expr1.Indexer         != expr2.Indexer ||
				expr1.Arguments.Count != expr2.Arguments.Count)
				return false;

			for (var i = 0; i < expr1.Arguments.Count; i++)
				if (!expr1.Arguments[i].EqualsTo(expr2.Arguments[i], info))
					return false;

			return expr1.Object.EqualsTo(expr2.Object);
		}

		static bool EqualsTo(EqualsToInfo info, DynamicExpression expr1, DynamicExpression expr2)
		{
			if (expr1.DelegateType    != expr2.DelegateType ||
				expr1.Arguments.Count != expr2.Arguments.Count)
				return false;

			for (var i = 0; i < expr1.Arguments.Count; i++)
				if (!expr1.Arguments[i].EqualsTo(expr2.Arguments[i], info))
					return false;

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, BlockExpression expr1, BlockExpression expr2)
		{
			if (expr1.Expressions.Count != expr2.Expressions.Count ||
				expr1.Variables.Count   != expr2.Variables.Count)
				return false;

			for (var i = 0; i < expr1.Expressions.Count; i++)
				if (!expr1.Expressions[i].EqualsTo(expr2.Expressions[i], info))
					return false;

			for (var i = 0; i < expr1.Variables.Count; i++)
				if (!expr1.Variables[i].EqualsTo(expr2.Variables[i], info))
					return false;

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, NewArrayExpression expr1, NewArrayExpression expr2)
		{
			if (expr1.Expressions.Count != expr2.Expressions.Count)
				return false;

			for (var i = 0; i < expr1.Expressions.Count; i++)
				if (!expr1.Expressions[i].EqualsTo(expr2.Expressions[i], info))
					return false;

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, NewExpression expr1, NewExpression expr2)
		{
			if (expr1.Arguments.Count != expr2.Arguments.Count)
				return false;

			if (expr1.Members == null && expr2.Members != null)
				return false;

			if (expr1.Members != null && expr2.Members == null)
				return false;

			if (expr1.Constructor != expr2.Constructor)
				return false;

			if (expr1.Members != null)
			{
				if (expr1.Members.Count != expr2.Members?.Count)
					return false;

				for (var i = 0; i < expr1.Members.Count; i++)
					if (expr1.Members[i] != expr2.Members[i])
						return false;
			}

			for (var i = 0; i < expr1.Arguments.Count; i++)
				if (!expr1.Arguments[i].EqualsTo(expr2.Arguments[i], info))
					return false;

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, MemberInitExpression expr1, MemberInitExpression expr2)
		{
			if (expr1.Bindings.Count != expr2.Bindings.Count || !expr1.NewExpression.EqualsTo(expr2.NewExpression, info))
				return false;

			bool CompareBindings(MemberBinding? b1, MemberBinding? b2)
			{
				if (b1 == b2)
					return true;

				if (b1 == null || b2 == null || b1.BindingType != b2.BindingType || b1.Member != b2.Member)
					return false;

				switch (b1.BindingType)
				{
					case MemberBindingType.Assignment:
						return ((MemberAssignment)b1).Expression.EqualsTo(((MemberAssignment)b2).Expression, info);

					case MemberBindingType.ListBinding:
						var ml1 = (MemberListBinding)b1;
						var ml2 = (MemberListBinding)b2;

						if (ml1.Initializers.Count != ml2.Initializers.Count)
							return false;

						for (var i = 0; i < ml1.Initializers.Count; i++)
						{
							var ei1 = ml1.Initializers[i];
							var ei2 = ml2.Initializers[i];

							if (ei1.AddMethod != ei2.AddMethod || ei1.Arguments.Count != ei2.Arguments.Count)
								return false;

							for (var j = 0; j < ei1.Arguments.Count; j++)
								if (!ei1.Arguments[j].EqualsTo(ei2.Arguments[j], info))
									return false;
						}

						break;

					case MemberBindingType.MemberBinding:
						var mm1 = (MemberMemberBinding)b1;
						var mm2 = (MemberMemberBinding)b2;

						if (mm1.Bindings.Count != mm2.Bindings.Count)
							return false;

						for (var i = 0; i < mm1.Bindings.Count; i++)
							if (!CompareBindings(mm1.Bindings[i], mm2.Bindings[i]))
								return false;

						break;
				}

				return true;
			}

			for (var i = 0; i < expr1.Bindings.Count; i++)
			{
				var b1 = expr1.Bindings[i];
				var b2 = expr2.Bindings[i];

				if (!CompareBindings(b1, b2))
					return false;
			}

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, MemberExpression expr1, MemberExpression expr2)
		{
			return expr1.Member == expr2.Member && expr1.Expression.EqualsTo(expr2.Expression, info);
		}

		static bool EqualsTo(EqualsToInfo info, ListInitExpression expr1, ListInitExpression expr2)
		{
			if (expr1.Initializers.Count != expr2.Initializers.Count || !expr1.NewExpression.EqualsTo(expr2.NewExpression, info))
				return false;

			for (var i = 0; i < expr1.Initializers.Count; i++)
			{
				var i1 = expr1.Initializers[i];
				var i2 = expr2.Initializers[i];

				if (i1.Arguments.Count != i2.Arguments.Count || i1.AddMethod != i2.AddMethod)
					return false;

				for (var j = 0; j < i1.Arguments.Count; j++)
					if (!i1.Arguments[j].EqualsTo(i2.Arguments[j], info))
						return false;
			}

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, LambdaExpression expr1, LambdaExpression expr2)
		{
			if (expr1.Parameters.Count != expr2.Parameters.Count || !expr1.Body.EqualsTo(expr2.Body, info))
				return false;

			for (var i = 0; i < expr1.Parameters.Count; i++)
				if (!expr1.Parameters[i].EqualsTo(expr2.Parameters[i], info))
					return false;

			return true;
		}

		static bool EqualsTo(EqualsToInfo info, InvocationExpression expr1, InvocationExpression expr2)
		{
			if (expr1.Arguments.Count != expr2.Arguments.Count || !expr1.Expression.EqualsTo(expr2.Expression, info))
				return false;

			for (var i = 0; i < expr1.Arguments.Count; i++)
				if (!expr1.Arguments[i].EqualsTo(expr2.Arguments[i], info))
					return false;

			return true;
		}

		static bool IsConstantable(this Type type)
		{
			if (type.IsEnum)
				return true;

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Int16   :
				case TypeCode.Int32   :
				case TypeCode.Int64   :
				case TypeCode.UInt16  :
				case TypeCode.UInt32  :
				case TypeCode.UInt64  :
				case TypeCode.SByte   :
				case TypeCode.Byte    :
				case TypeCode.Decimal :
				case TypeCode.Double  :
				case TypeCode.Single  :
				case TypeCode.Boolean :
				case TypeCode.String  :
				case TypeCode.Char    : return true;
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return type.GetGenericArguments()[0].IsConstantable();

			return false;
		}

		static bool EqualsTo(EqualsToInfo info, ConstantExpression expr1, ConstantExpression expr2)
		{
			if (expr1.Value == null && expr2.Value == null)
				return true;

			if (IsConstantable(expr1.Type))
				return Equals(expr1.Value, expr2.Value);

			if (expr1.Value == null || expr2.Value == null)
				return false;

			if (expr1.Value is IQueryable queryable)
			{
				var eq1 = queryable.Expression;
				var eq2 = ((IQueryable)expr2.Value).Expression;

				if (!info.Visited.Contains(eq1))
				{
					info.Visited.Add(eq1);
					return eq1.EqualsTo(eq2, info);
				}
			}
			else if (expr1.Value is IEnumerable list1 && expr2.Value is IEnumerable list2)
			{
				var enum1 = list1.GetEnumerator();
				var enum2 = list2.GetEnumerator();
				using (enum1 as IDisposable)
				using (enum2 as IDisposable)
				{
					while (enum1.MoveNext())
						if (!enum2.MoveNext() || !object.Equals(enum1.Current, enum2.Current))
							return false;

					if (enum2.MoveNext())
						return false;
				}

				return true;
			}

			return !info.CompareConstantValues || expr1.Value == expr2.Value;
		}

		static bool EqualsTo(EqualsToInfo info, MethodCallExpression expr1, MethodCallExpression expr2)
		{
			if (expr1.Arguments.Count != expr2.Arguments.Count || expr1.Method != expr2.Method)
				return false;

			if (!expr1.Object.EqualsTo(expr2.Object, info))
				return false;

			for (var i = 0; i < expr1.Arguments.Count; i++)
				if (!expr1.Arguments[i].EqualsTo(expr2.Arguments[i], info))
					return false;

			return true;
		}

		#endregion
	}
}
