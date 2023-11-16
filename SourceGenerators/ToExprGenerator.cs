﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators
{
	[Generator]
	public class ToExprGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
#if DEBUG1
			if (!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
#endif

			context.RegisterForPostInitialization(i => i.AddSource("ToExprAttribute.g.cs",
@"// <auto-generated/>
using System;

namespace Linq.Expressions.Deconstruct
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	[System.Diagnostics.Conditional(""ToExprGenerator_DEBUG"")]
	sealed class ToExprAttribute : Attribute
	{
		public string PropertyName { get; set; }
		public bool   IsNullable   { get; set; }
	}
}"));

			context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
				return;

			var attributeSymbol = context.Compilation.GetTypeByMetadataName("Linq.Expressions.Deconstruct.ToExprAttribute");

			foreach (var group in receiver.Fields.GroupBy<IFieldSymbol,INamedTypeSymbol>(f => f.ContainingType, SymbolEqualityComparer.Default))
			{
				var classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, context);
				context.AddSource($"{group.Key.Name}.ToExpr.g.cs", classSource);
			}
		}

		string ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attributeSymbol, GeneratorExecutionContext context)
		{
//			if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
//			{
//				return null; //TODO: issue a diagnostic that it must be top level
//			}

			var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

			var source = new StringBuilder($$"""
#nullable enable

namespace {{namespaceName}}
{
	partial class Expr
	{
		partial class {{classSymbol.Name}}
		{
""");

			foreach (var fieldSymbol in fields)
				ProcessField(source, classSymbol, fieldSymbol, attributeSymbol);

			source
				.AppendLine()
				.AppendLine("		}")
				.AppendLine("	}")
				.AppendLine("}")
				;

			return source.ToString();
		}

		void ProcessField(StringBuilder source, INamedTypeSymbol classSymbol, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
		{
			var fieldName = fieldSymbol.Name;
			var fieldType = fieldSymbol.Type.ToDisplayString();

			var attributeData = fieldSymbol.GetAttributes().Single(ad => ad.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default));
			var overridenName = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;
			var isNullable    = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "IsNullable").  Value;

			string propertyName;

			if (overridenName.IsNull)
			{
				propertyName = fieldName.TrimStart('_');

				if (propertyName.Length > 0)
					propertyName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
			}
			else
			{
				propertyName = overridenName.Value!.ToString();
			}

			if (propertyName.Length == 0 || propertyName == fieldName)
			{
				source.AppendLine($"#error Generator failed for : {fieldName}.");
				return;
			}

			// if the class doesn't implement INotifyPropertyChanged already, add it
			if (classSymbol.MemberNames.Contains(propertyName))
			{
				source.AppendLine($"#error Generator failed for : {fieldName}.");
				return;
			}

			if (isNullable.IsNull || isNullable.Value is false)
				fieldType = fieldType.TrimEnd('?');

			source.Append($$"""

			public {{fieldType}} {{propertyName}} => {{fieldName}} ??= Expr.{{propertyName}}.ToExpr();
""");
		}

		public class SyntaxReceiver : ISyntaxContextReceiver
		{
			public List<IFieldSymbol> Fields { get; } = new ();

			public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
			{
				if (context.Node is FieldDeclarationSyntax { AttributeLists.Count: > 0 } field)
				{
					foreach (var variable in field.Declaration.Variables)
					{
						var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
						var attributes  = fieldSymbol!.GetAttributes();

						if (attributes.Any(ad => ad.AttributeClass!.ToDisplayString() == "Linq.Expressions.Deconstruct.ToExprAttribute"))
							Fields.Add(fieldSymbol);
					}
				}
			}
		}
	}
}