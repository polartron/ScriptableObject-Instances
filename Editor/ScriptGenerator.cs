using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace Fasteraune.SO.Instances.Editor
{
    public class ScriptGenerator
    {
        private List<KeyValuePair<Type, string>> typesWithNames = new List<KeyValuePair<Type, string>>();
        private string outputPath;
        private string generatedNamespace;

        public ScriptGenerator(string outputPath, string generatedNamespace = "Generated")
        {
            this.outputPath = outputPath;
            this.generatedNamespace = generatedNamespace;
        }

        public void Add(Type type)
        {
            var fullTypeName = GetTypeAliasName(type);
            var name = UppercaseFirst(fullTypeName);
            
            typesWithNames.Add(new KeyValuePair<Type, string>(type, name));
        }
        
        public void Add(Type type, string name)
        {
            typesWithNames.Add(new KeyValuePair<Type, string>(type, name));
        }
        
        public void Add(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var fullTypeName = GetTypeAliasName(type);
                var name = UppercaseFirst(fullTypeName);
            
                typesWithNames.Add(new KeyValuePair<Type, string>(type, name));
            }
        }

        public void Add(IEnumerable<KeyValuePair<Type, string>> types)
        {
            typesWithNames.AddRange(types);
        }
        
        public void Generate()
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            foreach (var file in new DirectoryInfo(outputPath).GetFiles())
            {
                if (file.Extension.Equals("meta"))
                {
                    continue;
                }

                if (file.Extension.Equals("cs"))
                {
                    file.Delete();
                }
            }

            foreach (var generateType in typesWithNames)
            {
                var type = generateType.Key;
                var typeName = GetTypeAliasName(type, true);
                var name = generateType.Value;

                CreateScriptFile("Variable", typeName, generatedNamespace, name);
                CreateScriptFile("VariableProxy", typeName, generatedNamespace, name);
                CreateScriptFile("VariableReference", typeName, generatedNamespace, name);

                if (typeof(IComparable).IsAssignableFrom(type))
                {
                    CreateScriptFile("ReferenceClamped", typeName, generatedNamespace, name);
                }

                if (IsNumericType(type))
                {
                    CreateScriptFile("ReferenceExpression", typeName, generatedNamespace, name);
                }
                
                CreateScriptFile("Event", typeName, generatedNamespace, name);
                CreateScriptFile("EventReference", typeName, generatedNamespace, name);
                CreateScriptFile("EventInvokable", typeName, generatedNamespace, name);
            }

            AssetDatabase.Refresh();
        }
        
        private void CreateScriptFile(string template, string typeName, string nameSpace, string name)
        {
            string content = GetTemplate(template);
            string postfix = template + ".cs";
            
            using (var fs = File.Create(outputPath + name + postfix))
            {
                content = content.Replace("#NAMESPACE#", nameSpace);
                content = content.Replace("#TYPE#", typeName);
                content = content.Replace("#NAME#", name);

                var bytes = new UTF8Encoding(true).GetBytes(content);
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        private static string TemplatePath = "Packages/com.fasteraune.scriptableobjectinstances/Editor/Templates/";

        private static string GetTemplate(string template)
        {
            var path = TemplatePath + template + ".txt";
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            if (asset == null)
            {
                Debug.LogError($"Template does not exist at {path}");
                return null;
            }

            return asset.text;
        }

        //https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number/1750093#1750093
        public static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private static string GetTypeAliasName(Type type, bool fullName = false)
        {
            string typeName;
            using (var provider = new CSharpCodeProvider())
            {
                var typeRef = new CodeTypeReference(type);
                typeName = provider.GetTypeOutput(typeRef);
            }

            if (!fullName)
            {
                var index = typeName.LastIndexOf(".", StringComparison.Ordinal);
                return typeName.Substring(index + 1);
            }

            return typeName;
        }

        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
