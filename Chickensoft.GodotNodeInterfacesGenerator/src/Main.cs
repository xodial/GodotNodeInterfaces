namespace Chickensoft.GodotNodeInterfacesGenerator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Godot;
using Microsoft.CodeAnalysis.CSharp;
using Towel;

public static class GodotNodeInterfacesGenerator {
  // public const string GODOT_SHARP_XML_PATH = ".output/net6.0/GodotSharp.xml";
  public const string INTERFACES_PATH = "../Chickensoft.GodotNodeInterfaces/src/interfaces";
  public const string ADAPTERS_PATH = "../Chickensoft.GodotNodeInterfaces/src/adapters";

  public static readonly string CrefTagRegex = new(
    @"(<see cref="")([A-Z]:Godot\.)([^""]*"" />)"
  );

  private static readonly HashSet<string> _usings = new() { "Godot" };

  private static readonly HashSet<string> _ambiguousGodotTypes = new() {
    "Range", "Environment"
  };

  public static void Main(string[] args) {
    Console.WriteLine("Hello, world!");

    var godotAssembly = typeof(Node).Assembly;

    var typesThatExtendGodotNode = godotAssembly.GetTypes()
      .Where(t => t.IsSubclassOf(typeof(Node)));

    foreach (var type in typesThatExtendGodotNode) {
      // Look at each type of Godot node.

      _usings.Clear();
      _usings.Add("Godot");

      var mainDocumentation = XmlDocToDocComment(type.GetDocumentation(), 0);

      var interfaceName = $"I{type.Name}";
      var interfaceFilename = $"{interfaceName}.cs";

      var adapterName = $"{type.Name}Adapter";
      var adapterFilename = $"{adapterName}.cs";

      var members = type.GetMembers(
        BindingFlags.DeclaredOnly |
        BindingFlags.Public |
        BindingFlags.Instance
      ).OrderBy(m => m.Name);

      var interfaceMembers = new StringBuilder();
      var adapterMembers = new StringBuilder();

      foreach (var member in members) {
        if (member is MethodInfo methodInfo) {
          // For methods
          if (!methodInfo.IsPublic) { continue; }
          if (methodInfo.IsSpecialName) { continue; }

          var methodDocumentation = XmlDocToDocComment(methodInfo.GetDocumentation() ?? "", 2);
          var methodName = methodInfo.Name;
          var returnType = TypeName(methodInfo.ReturnType);
          var parameters = methodInfo.GetParameters();

          var typeParameters = methodInfo.GetGenericArguments();
          var typeParameterList = string.Join(
            ", ",
            typeParameters.Select(p => $"{p.Name}")
          );

          var typeParameterConstraints = "";
          if (typeParameters.Length > 0) {
            methodName += $"<{typeParameterList}>";

            var genericArgs = methodInfo
              .GetGenericMethodDefinition()
              .GetGenericArguments();

            Console.WriteLine("Generic args: " + genericArgs.Length);

            var hasClassConstraint = false;
            var hasNotNullConstraint = false;
            var hasDefaultConstructorConstraint = false;

            // compute type constraints
            var typeConstraints = genericArgs
              .Select(p => {
                var constraints = p.GetGenericParameterConstraints();
                hasClassConstraint = hasClassConstraint ||
                  p.GenericParameterAttributes.HasFlag(
                    GenericParameterAttributes.ReferenceTypeConstraint
                  );
                hasNotNullConstraint = hasNotNullConstraint ||
                  p.GenericParameterAttributes.HasFlag(
                    GenericParameterAttributes.NotNullableValueTypeConstraint
                  );
                hasDefaultConstructorConstraint = hasDefaultConstructorConstraint ||
                  p.GenericParameterAttributes.HasFlag(
                    GenericParameterAttributes.DefaultConstructorConstraint
                  );
                if (constraints.Length == 0) { return ""; }
                var constraintList = string.Join(
                  ", ",
                  constraints.Select(c => TypeName(c))
                );
                return constraints.Length > 0
                  ? $" where {p.Name} : {constraintList}"
                  : "";
              });

            typeParameterConstraints = string.Join("\n", typeConstraints);

            if (hasClassConstraint) {
              typeParameterConstraints =
                " where T : class" + typeParameterConstraints;
            }
            if (hasNotNullConstraint) {
              typeParameterConstraints =
                " where T : struct " + typeParameterConstraints;
            }
            if (hasDefaultConstructorConstraint) {
              typeParameterConstraints =
                " where T : new() " + typeParameterConstraints;
            }
          }

          var parameterList = string.Join(
            ", ",
            parameters.Select(p => $"{TypeName(p.ParameterType)} {GetId(p)}")
          );

          var signature = $"{returnType} {methodName}({parameterList}){typeParameterConstraints}";
          var interfaceSignature = $"{methodDocumentation}\n{"  ".Repeat(2)}{signature};";
          var adapterSignature = $"{methodDocumentation}\n{"  ".Repeat(2)}public {signature}";
          interfaceMembers.AppendLine(interfaceSignature);
          var adapterCode = adapterSignature + " => _node." + methodName + "(" + string.Join(", ", parameters.Select(GetId)) + ");";
          adapterMembers.AppendLine(adapterCode);
        }
        else if (member is PropertyInfo propertyInfo) {
          // Ignore set_ and get_ properties
          if (propertyInfo.Name.StartsWith("set_") || propertyInfo.Name.StartsWith("get_")) { continue; }

          var getMethod = propertyInfo.GetMethod;
          var isGettable =
            getMethod is MethodInfo getMethodInfo && getMethodInfo.IsPublic;

          // Only show public properties.
          if (!isGettable) { continue; }

          var setMethod = propertyInfo.SetMethod;
          var isSettable =
            setMethod is MethodInfo setMethodInfo && setMethodInfo.IsPublic;

          var writeable = isSettable ? "get; set;" : "get;";

          var propertyType = TypeName(propertyInfo.PropertyType);

          // For properties
          var propertyDocumentation = XmlDocToDocComment(
            propertyInfo.GetDocumentation() ?? "", 2
          );
          var propertyName = GetId(propertyInfo);

          var propertyPrefix = $"{propertyDocumentation}\n{"  ".Repeat(2)}{propertyType} {propertyName}";
          var propertySignature = $"{propertyPrefix} {{ {writeable} }}";

          var adapterCode = $"{propertyDocumentation}\n{"  ".Repeat(2)}public {propertyType} {propertyName}";
          if (isSettable && isGettable) {
            adapterCode += $" {{ get => _node.{propertyName}; set => _node.{propertyName} = value; }}";
          }
          else if (isGettable) {
            adapterCode += $" {{ get => _node.{propertyName}; }}";
          }
          else if (isSettable) {
            adapterCode += $" {{ set => _node.{propertyName} = value; }}";
          }

          interfaceMembers.AppendLine(propertySignature);
          adapterMembers.AppendLine(adapterCode);
        }
      }

      var baseType = type.BaseType!;
      // see if base type also extends Godot.Node
      var extendsAnotherNode = baseType.IsSubclassOf(typeof(Node));

      var interfaceParent = extendsAnotherNode
        ? $" : I{baseType.Name}"
        : "";

      var adapterParent = extendsAnotherNode
        ? $"{baseType.Name}Adapter, "
        : "";

      var adapterBaseCall = extendsAnotherNode
        ? " : base(node) "
        : " ";

      var adapterAbstract = type.IsAbstract ? " abstract " : " ";

      var interfaceMemberCode = interfaceMembers.ToString();
      var adapterMemberCode = adapterMembers.ToString();

      var usings = string.Join("\n", _usings.Select(u => $"using {u};"));

      var hasPublicParameterlessConstructor = type
        .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
        .Any(c => c.GetParameters().Length == 0);
      var canBeInstantiated =
        !type.IsAbstract && hasPublicParameterlessConstructor;

      var partialClassForVerification = $$"""

      // Apply interface to a Godot node implementation to make sure the
      // generated interface is correct.
      internal partial class {{type.Name}}Node : {{TypeName(type)}}, {{interfaceName}} { }

      """;

      var interfaceContents = $$"""
      namespace Chickensoft.GodotNodeInterfaces;

      {{usings}}
      {{(canBeInstantiated ? partialClassForVerification : "\n")}}
      {{mainDocumentation}}
      public interface {{interfaceName}}{{interfaceParent}} {
      {{interfaceMemberCode}}
      }
      """;

      var adapterContents = $$"""
      namespace Chickensoft.GodotNodeInterfaces;

      {{usings}}
      {{mainDocumentation}}
      public{{adapterAbstract}}class {{adapterName}} : {{adapterParent}}{{interfaceName}} {
        private readonly {{TypeName(type)}} _node;

        public {{adapterName}}({{TypeName(type)}} node){{adapterBaseCall}}{ _node = node; }

      {{adapterMemberCode}}
      }
      """;

      var interfaceFilePath = Path.Join(INTERFACES_PATH, interfaceFilename);
      File.WriteAllText(interfaceFilePath, interfaceContents);

      var adapterFilePath = Path.Join(ADAPTERS_PATH, adapterFilename);
      File.WriteAllText(adapterFilePath, adapterContents);
    }
  }

  private static string GetId(MemberInfo type) =>
    !IsValidId(type.Name) ? $"@{type.Name}" : type.Name;

  private static string GetId(ParameterInfo type) =>
    !IsValidId(type.Name!) ? $"@{type.Name}" : type.Name!;

  private static bool IsValidId(string id) {
    var validId = SyntaxFacts.IsValidIdentifier(id);
    var isReserved = SyntaxFacts.GetKeywordKind(id) != SyntaxKind.None;
    var isContextualReserved =
      SyntaxFacts.GetContextualKeywordKind(id) != SyntaxKind.None;
    return validId && !isReserved && !isContextualReserved;
  }

  // This is the set of types from the C# keyword list.
  private static readonly Dictionary<Type, string> _typeAlias =
    new() {
    { typeof(bool), "bool" },
    { typeof(byte), "byte" },
    { typeof(char), "char" },
    { typeof(decimal), "decimal" },
    { typeof(double), "double" },
    { typeof(float), "float" },
    { typeof(int), "int" },
    { typeof(long), "long" },
    { typeof(object), "object" },
    { typeof(sbyte), "sbyte" },
    { typeof(short), "short" },
    { typeof(string), "string" },
    { typeof(uint), "uint" },
    { typeof(ulong), "ulong" },
    { typeof(void), "void" },
  };

  private static string TypeName(Type type) {
    var name = TypeNameOrAlias(type);
    // see if type comes from a namespace
    if (type.Namespace is string @namespace) {
      // add namespace to using list
      _usings.Add(@namespace);
    }

    // sometimes, Godot collection types are ambiguous between .net collections
    var isAmbiguousCollectionType = type.Namespace == "Godot.Collections";
    var prefix = isAmbiguousCollectionType ? "Godot.Collections." : "";

    if (_ambiguousGodotTypes.Contains(type.Name)) {
      return $"Godot.{name}";
    }

    return type.IsGenericMethodParameter
      ? type.Name
      : prefix + (
        type.DeclaringType is Type dec
          ? $"{TypeName(dec)}.{name}"
          : name
        );
  }

  private static string TypeNameOrAlias(Type type) {
    // Handle generic types
    if (type.IsGenericType) {
      var name = type.Name.Split('`').FirstOrDefault();
      var @params =
          type.GetGenericArguments()
          .Select(a => type.IsConstructedGenericType ? TypeNameOrAlias(a) : a.Name);
      return $"{name}<{string.Join(",", @params)}>";
    }

    // Handle nullable value types
    var nullableBase = Nullable.GetUnderlyingType(type);
    if (nullableBase != null) {
      return TypeNameOrAlias(nullableBase) + "?";
    }

    // Handle arrays
    if (type.BaseType == typeof(Array)) {
      return TypeNameOrAlias(type.GetElementType()!) + "[]";
    }

    // Lookup alias for type
    if (_typeAlias.TryGetValue(type, out var alias)) {
      return alias;
    }

    // Default to CLR type name
    return type.Name;
  }

  private static string XmlDocToDocComment(string? xmlDocs, int indent) {
    if (xmlDocs is not string xmlDoc) { return ""; }

    var lines = xmlDoc
      .Split("\n")
      .Select(l => l.Trim())
      .Where(l => l != "")
      // Remove unneeded prefixes like `T:Godot.` and `E:Godot.`, etc,
      // leaving the rest of the cref contents alone
      .Select(l => Regex.Replace(
        l, CrefTagRegex, m => m.Groups[1].Value + m.Groups[3].Value))
      // indent 2 spaces
      .Select(l => $"{" ".Repeat(indent * 2)}/// {l}");

    return string.Join("\n", lines);
  }
}
