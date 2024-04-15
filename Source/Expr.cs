using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Linq.Expressions.Deconstruct
{
	public abstract partial class Expr
	{
		#region overrides

		protected abstract Expression GetExpression();

		public override string ToString   () => GetExpression().ToString()!;
		public override int    GetHashCode() => GetExpression().GetHashCode();

		public override bool Equals(object? obj) => obj switch
		{
			Expr       ex => GetExpression().Equals(ex.GetExpression()),
			Expression ex => GetExpression().Equals(ex),
			_             => false
		};

		public static bool operator ==(Expr left, Expr right) => Equals(left, right);
		public static bool operator !=(Expr left, Expr right) => !Equals(left, right);

		[return: NotNullIfNotNull(nameof(expr))]
		public static implicit operator Expression?(Expr? expr) => expr?.GetExpression();

		[return: NotNullIfNotNull(nameof(expr))]
		public static implicit operator Expr?(Expression? expr) => expr?.ToExpr();

		#endregion

		#region Call

		public partial class Call
		{
			[ToExpr]                  ReadOnlyCollection<Expr>? _arguments;
			[ToExpr(IsNullable=true)] Expr?                     _object;

			public void Deconstruct(out bool isGeneric, out Type type, out MethodInfo method, out Expr? @object, out ReadOnlyCollection<Expr> arguments)
			{
				isGeneric = Expr.Method.IsGenericMethod;
				type      = Expr.Type;
				method    = isGeneric ? Expr.Method.GetGenericMethodDefinition() : Expr.Method;
				@object   = Object;
				arguments = Arguments;
			}

			public void Deconstruct(out Type type, out MethodInfo method, out Expr? @object, out ReadOnlyCollection<Expr> arguments)
			{
				type      = Expr.Type;
				method    = Expr.Method;
				@object   = Object;
				arguments = Arguments;
			}

			public void Deconstruct(out MethodInfo method, out Expr? @object, out ReadOnlyCollection<Expr> arguments)
			{
				method    = Expr.Method;
				@object   = Object;
				arguments = Arguments;
			}
		}

		#endregion

		#region Conditional

		public partial class Conditional
		{
			[ToExpr] Expr? _test;
			[ToExpr] Expr? _ifTrue;
			[ToExpr] Expr? _ifFalse;

			public void Deconstruct(out Type type, out Expr test, out Expr ifTrue, out Expr ifFalse)
			{
				type    = Expr.Type;
				test    = Test;
				ifTrue  = IfTrue;
				ifFalse = IfFalse;
			}

			public void Deconstruct(out Expr test, out Expr ifTrue, out Expr ifFalse)
			{
				test    = Test;
				ifTrue  = IfTrue;
				ifFalse = IfFalse;
			}
		}

		#endregion

		#region Invoke

		public partial class Invoke
		{
			[ToExpr] Expr?                     _expression;
			[ToExpr] ReadOnlyCollection<Expr>? _arguments;

			public void Deconstruct(out Type type, out Expr expression, out ReadOnlyCollection<Expr> arguments)
			{
				type       = Expr.Type;
				expression = Expression;
				arguments  = Arguments;
			}

			public void Deconstruct(out Expr expression, out ReadOnlyCollection<Expr> arguments)
			{
				expression = Expression;
				arguments  = Arguments;
			}
		}

		#endregion

		#region Lambda

		public partial class Lambda
		{
			[ToExpr] Expr?                          _body;
			[ToExpr] ReadOnlyCollection<Parameter>? _parameters;

			public void Deconstruct(out Type type, out Expr body, out ReadOnlyCollection<Parameter> parameters)
			{
				type       = Expr.Type;
				body       = Body;
				parameters = Parameters;
			}

			public void Deconstruct(out Expr body, out ReadOnlyCollection<Parameter> parameters)
			{
				body       = Body;
				parameters = Parameters;
			}
		}

		#endregion

		#region ListInit

		public partial class ListInit
		{
			New? _newExpression;

			public New NewExpression => _newExpression ??= new (Expr.NewExpression);

			public void Deconstruct(out Type type, out New newExpression, out ReadOnlyCollection<ElementInit> initializers)
			{
				type          = Expr.Type;
				newExpression = NewExpression;
				initializers  = Expr.Initializers;
			}

			public void Deconstruct(out Expr newExpression, out ReadOnlyCollection<ElementInit> initializers)
			{
				newExpression = NewExpression;
				initializers  = Expr.Initializers;
			}
		}

		#endregion

		#region Member

		public partial class Member
		{
			[ToExpr] Expr? _expression;

			public void Deconstruct(out Type type, out Expr? expression)
			{
				type       = Expr.Type;
				expression = Expression;
			}

			public void Deconstruct(out Expr? expression)
			{
				expression = Expression;
			}
		}

		#endregion

		#region MemberInit

		public partial class MemberInit
		{
			New? _newExpression;

			public New NewExpression => _newExpression ??= new (Expr.NewExpression);

			public void Deconstruct(out Type type, out New newExpression, out IEnumerable<MemberBinding> bindings)
			{
				type          = Expr.Type;
				newExpression = NewExpression;
				bindings      = Expr.Bindings;
			}

			public void Deconstruct(out New newExpression, out IEnumerable<MemberBinding> bindings)
			{
				newExpression = NewExpression;
				bindings      = Expr.Bindings;
			}
		}

		#endregion

		#region New

		public partial class New
		{
			[ToExpr] ReadOnlyCollection<Expr>? _arguments;

			public void Deconstruct(
				out Type                            type,
				out ConstructorInfo                 constructor,
				out ReadOnlyCollection<MemberInfo>? members,
				out ReadOnlyCollection<Expr>        arguments)
			{
				type        = Expr.Type;
				constructor = Expr.Constructor;
				members     = Expr.Members;
				arguments   = Arguments;
			}

			public void Deconstruct(
				out ConstructorInfo                 constructor,
				out ReadOnlyCollection<MemberInfo>? members,
				out ReadOnlyCollection<Expr>        arguments)
			{
				constructor = Expr.Constructor;
				members     = Expr.Members;
				arguments   = Arguments;
			}

			public void Deconstruct(
				out ReadOnlyCollection<MemberInfo>? members,
				out ReadOnlyCollection<Expr>        arguments)
			{
				members   = Expr.Members;
				arguments = Arguments;
			}

			public void Deconstruct(
				out ReadOnlyCollection<Expr> arguments)
			{
				arguments = Arguments;
			}
		}

		#endregion

		#region NewArrayBounds

		public partial class NewArrayBounds
		{
			[ToExpr] ReadOnlyCollection<Expr>? _expressions;

			public void Deconstruct(out Type type, out ReadOnlyCollection<Expr> expressions)
			{
				type        = Expr.Type;
				expressions = Expressions;
			}

			public void Deconstruct(out ReadOnlyCollection<Expr> expressions)
			{
				expressions = Expressions;
			}
		}

		#endregion

		#region NewArrayInit

		public partial class NewArrayInit
		{
			[ToExpr] ReadOnlyCollection<Expr>? _expressions;

			public void Deconstruct(out Type type, out ReadOnlyCollection<Expr> expressions)
			{
				type        = Expr.Type;
				expressions = Expressions;
			}

			public void Deconstruct(out ReadOnlyCollection<Expr> expressions)
			{
				expressions = Expressions;
			}
		}

		#endregion

		#region TypeEqual

		public partial class TypeEqual
		{
			[ToExpr] Expr? _expression;

			public void Deconstruct(out Type type, out Expr expression)
			{
				type       = Expr.Type;
				expression = Expression;
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expression;
			}
		}

		#endregion

		#region TypeIs

		public partial class TypeIs
		{
			[ToExpr] Expr? _expression;

			public void Deconstruct(out Type type, out Expr expression)
			{
				type       = Expr.Type;
				expression = Expression;
			}

			public void Deconstruct(out Expr expression)
			{
				expression = Expression;
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
			[ToExpr] ReadOnlyCollection<Expr>? _arguments;

			public void Deconstruct(out Type delegateType, out ReadOnlyCollection<Expr> arguments)
			{
				delegateType = Expr.DelegateType;
				arguments    = Arguments;
			}
		}

		#endregion

		#region Goto

		public partial class Goto
		{
			[ToExpr] Expr? _value;

			public void Deconstruct(out Type type, out LabelTarget target, out Expr value)
			{
				type   = Expr.Type;
				target = Expr.Target;
				value  = Value;
			}

			public void Deconstruct(out LabelTarget target, out Expr value)
			{
				target = Expr.Target;
				value  = Value;
			}
		}

		#endregion

		#region Index

		public partial class Index
		{
			[ToExpr] Expr?                     _object;
			[ToExpr] ReadOnlyCollection<Expr>? _arguments;

			public void Deconstruct(out Type type, out Expr @object, out PropertyInfo indexer, out ReadOnlyCollection<Expr> arguments)
			{
				type      = Expr.Type;
				@object   = Object;
				indexer   = Expr.Indexer;
				arguments = Arguments;
			}

			public void Deconstruct(out Expr @object, out PropertyInfo indexer, out ReadOnlyCollection<Expr> arguments)
			{
				@object   = Object;
				indexer   = Expr.Indexer;
				arguments = Arguments;
			}

			public void Deconstruct(out Expr @object, out ReadOnlyCollection<Expr> arguments)
			{
				@object   = Object;
				arguments = Arguments;
			}
		}

		#endregion

		#region Label

		public partial class Label
		{
			[ToExpr(IsNullable=true)] Expr? _defaultValue;

			public void Deconstruct(out Type type, out LabelTarget target, out Expr? defaultValue)
			{
				type         = Expr.Type;
				target       = Expr.Target;
				defaultValue = DefaultValue;
			}

			public void Deconstruct(out LabelTarget target, out Expr? defaultValue)
			{
				target       = Expr.Target;
				defaultValue = DefaultValue;
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
			[ToExpr] Expr? _body;

			public void Deconstruct(out Type type, out LabelTarget breakLabel, out LabelTarget continueLabel, out Expr body)
			{
				type          = Expr.Type;
				breakLabel    = Expr.BreakLabel;
				continueLabel = Expr.ContinueLabel;
				body          = Body;
			}

			public void Deconstruct(out LabelTarget breakLabel, out LabelTarget continueLabel, out Expr body)
			{
				breakLabel    = Expr.BreakLabel;
				continueLabel = Expr.ContinueLabel;
				body          = Body;
			}
		}

		#endregion

		#region Switch

		public partial class Switch
		{
			[ToExpr]                  Expr? _switchValue;
			[ToExpr(IsNullable=true)] Expr? _defaultBody;

			public void Deconstruct(out Type type, out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr? defaultBody)
			{
				type        = Expr.Type;
				switchValue = SwitchValue;
				cases       = Expr.Cases;
				defaultBody = DefaultBody;
			}

			public void Deconstruct(out Expr switchValue, out IEnumerable<SwitchCase> cases, out Expr? defaultBody)
			{
				switchValue = SwitchValue;
				cases       = Expr.Cases;
				defaultBody = DefaultBody;
			}
		}

		#endregion

		#region Try

		public partial class Try
		{
			[ToExpr]                  Expr? _body;
			[ToExpr(IsNullable=true)] Expr? _finally;
			[ToExpr(IsNullable=true)] Expr? _fault;

			public void Deconstruct(out Type type, out Expr body, out IEnumerable<CatchBlock> handlers, out Expr? @finally, out Expr? fault)
			{
				type     = Expr.Type;
				body     = Body;
				handlers = Expr.Handlers;
				@finally = Finally;
				fault    = Fault;
			}

			public void Deconstruct(out Expr body, out IEnumerable<CatchBlock> handlers, out Expr? @finally, out Expr? fault)
			{
				body     = Body;
				handlers = Expr.Handlers;
				@finally = Finally;
				fault    = Fault;
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

		[return: NotNullIfNotNull(nameof(expr))]
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

		[return:NotNullIfNotNull(nameof(exprs))]
		public static ReadOnlyCollection<Expr>? ToExpr(this ReadOnlyCollection<Expression>? exprs)
		{
			if (exprs == null)
				return null;

			var list = new Expr[exprs.Count];

			for (var i = 0; i < exprs.Count; i++)
				list[i] = exprs[i].ToExpr();

			return new (list);
		}

		[return:NotNullIfNotNull(nameof(exprs))]
		public static ReadOnlyCollection<Expr.Parameter>? ToExpr(this ReadOnlyCollection<ParameterExpression>? exprs)
		{
			if (exprs == null)
				return null;

			var list = new List<Expr.Parameter>(exprs.Count);

			foreach (var item in exprs)
				list.Add(new (item));

			return new (list);
		}

		#endregion

		#region ElementInit

		public static void Deconstruct(this ElementInit elementInit, out MethodInfo addMethod, out ReadOnlyCollection<Expr> arguments)
		{
			addMethod = elementInit.AddMethod;
			arguments = elementInit.Arguments.ToExpr();
		}

		public static void Deconstruct(this ElementInit elementInit, out ReadOnlyCollection<Expr> arguments)
		{
			arguments = elementInit.Arguments.ToExpr();
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

		public static void Deconstruct(this SwitchCase switchCase, out Expr? body, out ReadOnlyCollection<Expr> testValues)
		{
			body       = switchCase.Body.      ToExpr();
			testValues = switchCase.TestValues.ToExpr();
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

		static void VisitInternal<T1,T2>(IEnumerable<T1> source, Action<T2> f, Action<Action<T2>,T1> func)
		{
			foreach (var item in source)
				func(f, item);
		}

		static void VisitInternal<T1,T2>(IEnumerable<T1> source, Func<T2,bool> f, Func<Func<T2,bool>,T1,bool> func)
		{
			foreach (var item in source)
				func(f, item);
		}

		static void VisitInternal<T1,T2>(IEnumerable<T1> source, Func<T2,bool> f, Action<Func<T2,bool>,T1> func)
		{
			foreach (var item in source)
				func(f, item);
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
					VisitInternal(e.Initializers, func, static (f,ex) => VisitInternal(ex.Arguments, f));

					break;
				}

				case ExpressionType.MemberAccess:
				{
					VisitInternal(((MemberExpression)expr).Expression, func);
					break;
				}

				case ExpressionType.MemberInit:
				{
					static void Action(Action<Expression> ff, MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment:
								VisitInternal(((MemberAssignment)b).Expression, ff);
								break;
							case MemberBindingType.ListBinding:
								VisitInternal(((MemberListBinding)b).Initializers, ff, static (f,p) => VisitInternal(p.Arguments, f));
								break;
							case MemberBindingType.MemberBinding:
								VisitInternal(((MemberMemberBinding)b).Bindings, ff, Action);
								break;
						}
					}

					var e = (MemberInitExpression)expr;

					VisitInternal(e.NewExpression, func);
					VisitInternal(e.Bindings,      func, Action);

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
						e.Cases, func, static (f,cs) =>
						{
							VisitInternal(cs.TestValues, f);
							VisitInternal(cs.Body,       f);
						});
					VisitInternal(e.DefaultBody, func);

					break;
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;

					VisitInternal(e.Body, func);
					VisitInternal(
						e.Handlers, func, static (f,h) =>
						{
							VisitInternal(h.Variable, f);
							VisitInternal(h.Filter,   f);
							VisitInternal(h.Body,     f);
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

		static void VisitInternal<T>(IEnumerable<T> source, Func<T, bool> func)
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
					VisitInternal(e.Initializers, func, static (f,ex) => VisitInternal(ex.Arguments, f));

					break;
				}

				case ExpressionType.MemberAccess:
					VisitInternal(((MemberExpression)expr).Expression, func);
					break;

				case ExpressionType.MemberInit:
				{
					static bool Modify(Func<Expression,bool> ff, MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment:
								VisitInternal(((MemberAssignment)b).Expression, ff);
								break;
							case MemberBindingType.ListBinding:
								VisitInternal(((MemberListBinding)b).Initializers, ff, static (f,p) => VisitInternal(p.Arguments, f));
								break;
							case MemberBindingType.MemberBinding:
								VisitInternal(((MemberMemberBinding)b).Bindings, ff, Modify);
								break;
						}

						return true;
					}

					var e = (MemberInitExpression)expr;

					VisitInternal(e.NewExpression, func);
					VisitInternal(e.Bindings,      func, Modify);

					break;
				}

				case ExpressionType.New              : VisitInternal(((NewExpression)             expr).Arguments,    func); break;
				case ExpressionType.NewArrayBounds   : VisitInternal(((NewArrayExpression)        expr).Expressions,  func); break;
				case ExpressionType.NewArrayInit     : VisitInternal(((NewArrayExpression)        expr).Expressions,  func); break;
				case ExpressionType.TypeEqual        :
				case ExpressionType.TypeIs           : VisitInternal(((TypeBinaryExpression)      expr).Expression,   func); break;
				case ExpressionType.Dynamic          : VisitInternal(((DynamicExpression)         expr).Arguments,    func); break;
				case ExpressionType.Goto             : VisitInternal(((GotoExpression)            expr).Value,        func); break;
				case ExpressionType.Label            : VisitInternal(((LabelExpression)           expr).DefaultValue, func); break;
				case ExpressionType.RuntimeVariables : VisitInternal(((RuntimeVariablesExpression)expr).Variables,    func); break;
				case ExpressionType.Loop             : VisitInternal(((LoopExpression)            expr).Body,         func); break;

				case ExpressionType.Block:
				{
					var e = (BlockExpression)expr;

					VisitInternal(e.Expressions, func);
					VisitInternal(e.Variables,   func);

					break;
				}

				case ExpressionType.Index:
				{
					var e = (IndexExpression)expr;

					VisitInternal(e.Object,    func);
					VisitInternal(e.Arguments, func);

					break;
				}

				case ExpressionType.Switch:
				{
					var e = (SwitchExpression)expr;

					VisitInternal(e.SwitchValue, func);
					VisitInternal(
						e.Cases, func, static (f,cs) =>
						{
							VisitInternal(cs.TestValues, f);
							VisitInternal(cs.Body,       f);
						});
					VisitInternal(e.DefaultBody, func);

					break;
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;

					VisitInternal(e.Body, func);
					VisitInternal(
						e.Handlers, func, static (f,h) =>
						{
							VisitInternal(h.Variable, f);
							VisitInternal(h.Filter,   f);
							VisitInternal(h.Body,     f);
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
			return FindInternal(expr, ex => func(ex.ToExpr()));
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
		[return: NotNullIfNotNull(nameof(expr))]
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

		static Expression? FindInternal<T1,T2>(IEnumerable<T1> source, Func<T2,Expression?> f, Func<Func<T2,Expression?>,T1,Expression?> func)
		{
			foreach (var item in source)
			{
				var ex = func(f, item);
				if (ex != null)
					return ex;
			}

			return null;
		}

		static Expression? FindInternal<T1,T2>(IEnumerable<T1> source, Func<T2,bool> f, Func<Func<T2,bool>,T1,Expression?> func)
		{
			foreach (var item in source)
			{
				var ex = func(f, item);
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

					return
						FindInternal(e.Conversion, func) ??
						FindInternal(e.Left,       func) ??
						FindInternal(e.Right,      func);
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
					return
						FindInternal(((MethodCallExpression)expr).Object,    func) ??
						FindInternal(((MethodCallExpression)expr).Arguments, func);

				case ExpressionType.Conditional:
				{
					var e = (ConditionalExpression)expr;
					return
						FindInternal(e.Test,    func) ??
						FindInternal(e.IfTrue,  func) ??
						FindInternal(e.IfFalse, func);
				}

				case ExpressionType.Invoke  : return FindInternal(((InvocationExpression)expr).Expression,  func) ?? FindInternal(((InvocationExpression)expr).Arguments, func);
				case ExpressionType.Lambda  : return FindInternal(((LambdaExpression)expr).Body,            func) ?? FindInternal(((LambdaExpression)expr).Parameters, func);
				case ExpressionType.ListInit: return FindInternal(((ListInitExpression)expr).NewExpression, func) ?? FindInternal<ElementInit,Expression>(((ListInitExpression)expr).Initializers, func, static (f,ex) => FindInternal(ex.Arguments, f));

				case ExpressionType.MemberAccess:
					return FindInternal(((MemberExpression)expr).Expression, func);

				case ExpressionType.MemberInit:
				{
					static Expression? Func(Func<Expression,bool> ff, MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment    : return FindInternal(((MemberAssignment)b).Expression,    ff);
							case MemberBindingType.ListBinding   : return FindInternal(((MemberListBinding)b).Initializers, ff, static (f,p) => FindInternal(p.Arguments, f));
							case MemberBindingType.MemberBinding : return FindInternal(((MemberMemberBinding)b).Bindings,   ff, Func);
						}

						return null;
					}

					var e = (MemberInitExpression)expr;

					return FindInternal(e.NewExpression, func) ?? FindInternal(e.Bindings, func, Func);
				}

				case ExpressionType.New             : return FindInternal(((NewExpression)expr).Arguments,              func);
				case ExpressionType.NewArrayBounds  : return FindInternal(((NewArrayExpression)expr).Expressions,       func);
				case ExpressionType.NewArrayInit    : return FindInternal(((NewArrayExpression)expr).Expressions,       func);
				case ExpressionType.TypeEqual       :
				case ExpressionType.TypeIs          : return FindInternal(((TypeBinaryExpression)expr).Expression,      func);
				case ExpressionType.Block           : return FindInternal(((BlockExpression)expr).Expressions,          func) ?? FindInternal(((BlockExpression)expr).Variables, func);
				case ExpressionType.Dynamic         : return FindInternal(((DynamicExpression)expr).Arguments,          func);
				case ExpressionType.Goto            : return FindInternal(((GotoExpression)expr).Value,                 func);
				case ExpressionType.Index           : return FindInternal(((IndexExpression)expr).Object,               func) ?? FindInternal(((IndexExpression)expr).Arguments, func);
				case ExpressionType.Label           : return FindInternal(((LabelExpression)expr).DefaultValue,         func);
				case ExpressionType.RuntimeVariables: return FindInternal(((RuntimeVariablesExpression)expr).Variables, func);
				case ExpressionType.Loop            : return FindInternal(((LoopExpression)expr).Body,                  func);

				case ExpressionType.Switch          :
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
							e.Handlers, func, static (f,h) => FindInternal(h.Variable, f) ?? FindInternal(h.Filter, f) ?? FindInternal(h.Body, f))
							??
							FindInternal(e.Finally, func)
							??
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

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <typeparam name="T">Type of expression.</typeparam>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <param name="forward">If True, scans expression tree forward.</param>
		/// <returns>Modified expression.</returns>
		public static T TransformEx<T>(this T expr, bool forward, Func<Expr,Expr> func)
			where T : LambdaExpression
		{
			return (T)TransformInternal(expr, forward, ex => func(ex.ToExpr()));
		}

		/// <summary>
		/// Transforms original expression. The expression tree is scanned backward.
		/// </summary>
		/// <typeparam name="T">Type of expression.</typeparam>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		public static T TransformEx<T>(this T expr, Func<Expr,Expr> func)
			where T : LambdaExpression
		{
			return (T)TransformInternal(expr, false, ex => func(ex.ToExpr()));
		}

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <param name="forward">If True, scans expression tree forward.</param>
		/// <returns>Modified expression.</returns>
		[return: NotNullIfNotNull(nameof(expr))]
		public static Expression? TransformEx(this Expression? expr, bool forward, Func<Expr,Expr> func)
		{
			return TransformInternal(expr, forward, ex => func(ex.ToExpr()));
		}

		/// <summary>
		/// Transforms original expression. The expression tree is scanned backward.
		/// </summary>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		[return: NotNullIfNotNull(nameof(expr))]
		[return: NotNullIfNotNull(nameof(expr))]
		public static Expression? TransformEx(this Expression? expr, Func<Expr,Expr> func)
		{
			return TransformInternal(expr, false, ex => func(ex.ToExpr()));
		}

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <typeparam name="T">Type of expression.</typeparam>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <param name="forward">If True, scans expression tree forward.</param>
		/// <returns>Modified expression.</returns>
		public static T Transform<T>(this T expr, bool forward, Func<Expression,Expression> func)
			where T : LambdaExpression
		{
			return (T)(TransformInternal(expr, forward, func));
		}

		/// <summary>
		/// Transforms original expression. The expression tree is scanned backward.
		/// </summary>
		/// <typeparam name="T">Type of expression.</typeparam>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		public static T Transform<T>(this T expr, Func<Expression,Expression> func)
			where T : LambdaExpression
		{
			return (T)(TransformInternal(expr, false, func));
		}

		/// <summary>
		/// Transforms original expression.
		/// </summary>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <param name="forward">If True, scans expression tree forward.</param>
		/// <returns>Modified expression.</returns>
		[return: NotNullIfNotNull(nameof(expr))]
		public static Expression? Transform(this Expression? expr, bool forward, Func<Expression,Expression> func)
		{
			return TransformInternal(expr, forward, func);
		}

		/// <summary>
		/// Transforms original expression. The expression tree is scanned backward.
		/// </summary>
		/// <param name="expr">Expression to transform.</param>
		/// <param name="func">Transform function.</param>
		/// <returns>Modified expression.</returns>
		[return: NotNullIfNotNull(nameof(expr))]
		public static Expression? Transform(this Expression? expr, Func<Expression,Expression> func)
		{
			return TransformInternal(expr, false, func);
		}

		static IEnumerable<T> TransformInternal<T>(ICollection<T> source, Func<T,T> func)
			where T : class
		{
			var modified = false;
			var list     = new List<T>();

			foreach (var item in source)
			{
				var e = func(item);
				list.Add(e);
				modified = modified || e != item;
			}

			return modified ? list : source;
		}

		static IEnumerable<T1> TransformInternal<T1,T2>(ICollection<T1> source, T2 f, Func<T2,T1,T1> func)
			where T1 : class
		{
			var modified = false;
			var list     = new List<T1>();

			foreach (var item in source)
			{
				var e = func(f, item);
				list.Add(e);
				modified = modified || e != item;
			}

			return modified ? list : source;
		}

		static IEnumerable<T> TransformInternal<T>(ICollection<T> source, Func<Expression,Expression> func, bool forward)
			where T : Expression
		{
			var modified = false;
			var list     = new List<T?>();

			foreach (var item in source)
			{
				var e = TransformInternal(item, forward, func);
				list.Add((T?)e);
				modified = modified || e != item;
			}

			return modified ? (IEnumerable<T>)list : source;
		}

		[return: NotNullIfNotNull(nameof(expr))]
		static Expression? TransformInternal(this Expression? expr, bool forward, Func<Expression,Expression> func)
		{
			if (expr == null)
				return null;

			if (forward)
			{
				var ex = func(expr);
				if (ex != expr)
					return ex;
			}

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
						TransformInternal(e.Left, forward, func),
						(LambdaExpression?)TransformInternal(e.Conversion, forward, func),
						TransformInternal(e.Right, forward, func));
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
					expr = e.Update(TransformInternal(e.Operand, forward, func));
					break;
				}

				case ExpressionType.Call:
				{
					var e = (MethodCallExpression)expr;
					expr = e.Update(
						TransformInternal(e.Object, forward, func),
						TransformInternal(e.Arguments, func, forward));
					break;
				}

				case ExpressionType.Conditional:
				{
					var e = (ConditionalExpression)expr;
					expr = e.Update(
						TransformInternal(e.Test, forward, func),
						TransformInternal(e.IfTrue, forward, func),
						TransformInternal(e.IfFalse, forward, func));
					break;
				}

				case ExpressionType.Invoke:
				{
					var e = (InvocationExpression)expr;
					expr = e.Update(
						TransformInternal(e.Expression, forward, func),
						TransformInternal(e.Arguments, func, forward));
					break;
				}

				case ExpressionType.Lambda:
				{
					var e = (LambdaExpression)expr;
					var b = TransformInternal(e.Body, forward, func);
					var p = TransformInternal(e.Parameters, func, forward);

					expr = b != e.Body || !ReferenceEquals(p, e.Parameters) ? Expression.Lambda(expr.Type, b, p.ToArray()) : expr;
					break;
				}

				case ExpressionType.ListInit:
				{
					var e = (ListInitExpression)expr;
					expr = e.Update(
						(NewExpression?)TransformInternal(e.NewExpression, forward, func),
						TransformInternal(
							e.Initializers, (func,forward), static (f,p) =>
							{
								var args = TransformInternal(p.Arguments, f.func, f.forward);
								return !ReferenceEquals(args, p.Arguments) ? Expression.ElementInit(p.AddMethod, args) : p;
							}));
					break;
				}

				case ExpressionType.MemberAccess:
				{
					var e = (MemberExpression)expr;
					expr = e.Update(TransformInternal(e.Expression, forward, func));
					break;
				}

				case ExpressionType.MemberInit:
				{
					static MemberBinding Modify((Func<Expression,Expression> func,bool forward) ff, MemberBinding b)
					{
						switch (b.BindingType)
						{
							case MemberBindingType.Assignment   : return ((MemberAssignment)b).   Update(TransformInternal(((MemberAssignment)   b).Expression,   ff.forward, ff.func));
							case MemberBindingType.MemberBinding: return ((MemberMemberBinding)b).Update(TransformInternal(((MemberMemberBinding)b).Bindings,     ff, Modify));
							case MemberBindingType.ListBinding  : return ((MemberListBinding)b).  Update(TransformInternal(((MemberListBinding)  b).Initializers, ff, static (f,p) =>
							{
								var args = TransformInternal(p.Arguments, f.func, f.forward);
								return !ReferenceEquals(args, p.Arguments) ? Expression.ElementInit(p.AddMethod, args) : p;
							}));
						}

						return b;
					}

					var e = (MemberInitExpression)expr;
					expr = e.Update(
						(NewExpression?)TransformInternal(e.NewExpression, forward, func),
						TransformInternal(e.Bindings, (func,forward), Modify));
					break;
				}

				case ExpressionType.New:
				{
					var e = (NewExpression)expr;
					expr = e.Update(TransformInternal(e.Arguments, func, forward));
					break;
				}

				case ExpressionType.NewArrayBounds:
				case ExpressionType.NewArrayInit:
				{
					var e = (NewArrayExpression)expr;
					expr = e.Update(TransformInternal(e.Expressions, func, forward));
					break;
				}

				case ExpressionType.TypeEqual:
				case ExpressionType.TypeIs:
				{
					var e = (TypeBinaryExpression)expr;
					expr = e.Update(TransformInternal(e.Expression, forward, func));
					break;
				}

				case ExpressionType.Block:
				{
					var e = (BlockExpression)expr;
					expr = e.Update(
						TransformInternal(e.Variables, func, forward),
						TransformInternal(e.Expressions, func, forward));
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
					expr = e.Update(TransformInternal(e.Arguments, func, forward));
					break;
				}

				case ExpressionType.Goto:
				{
					var e = (GotoExpression)expr;
					expr = e.Update(e.Target, TransformInternal(e.Value, forward, func));
					break;
				}

				case ExpressionType.Index:
				{
					var e = (IndexExpression)expr;
					expr = e.Update(
						TransformInternal(e.Object, forward, func),
						TransformInternal(e.Arguments, func, forward));
					break;
				}

				case ExpressionType.Label:
				{
					var e = (LabelExpression)expr;
					expr = e.Update(e.Target, TransformInternal(e.DefaultValue, forward, func));
					break;
				}

				case ExpressionType.RuntimeVariables:
				{
					var e = (RuntimeVariablesExpression)expr;
					expr = e.Update(TransformInternal(e.Variables, func, forward));
					break;
				}

				case ExpressionType.Loop:
				{
					var e = (LoopExpression)expr;
					expr = e.Update(e.BreakLabel, e.ContinueLabel, TransformInternal(e.Body, forward, func));
					break;
				}

				case ExpressionType.Switch:
				{
					var e = (SwitchExpression)expr;
					expr = e.Update(
						TransformInternal(e.SwitchValue, forward, func),
						TransformInternal(e.Cases, (func,forward), static (f,cs) => cs.Update(TransformInternal(cs.TestValues, f.func, f.forward), TransformInternal(cs.Body, f.forward, f.func))),
						TransformInternal(e.DefaultBody, forward, func));
					break;
				}

				case ExpressionType.Try:
				{
					var e = (TryExpression)expr;
					expr = e.Update(
						TransformInternal(e.Body, forward, func),
						TransformInternal(e.Handlers, (func, forward), static (f,h) =>
							h.Update(
								(ParameterExpression?)TransformInternal(h.Variable, f.forward, f.func), TransformInternal(h.Filter, f.forward, f.func),
								TransformInternal(h.Body, f.forward, f.func))),
						TransformInternal(e.Finally, forward, func),
						TransformInternal(e.Fault, forward, func));
					break;
				}
			}

			return expr == null || forward ? expr : func(expr);
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
			public readonly HashSet<Expression> Visited = [];
			public          bool                CompareConstantValues;
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
