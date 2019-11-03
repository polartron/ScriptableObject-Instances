using System;
using System.CodeDom;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace Fasteraune.SO.Instances.Editor
{
    public class ScriptGenerator
    {
        private static void CreateScriptFile(string content, string path, string typeName, string nameSpace,
            string name,
            string fileNamePostfix)
        {
            using (var fs = File.Create(path + name + fileNamePostfix))
            {
                content = content.Replace("#NAMESPACE#", nameSpace);
                content = content.Replace("#TYPE#", typeName);
                content = content.Replace("#NAME#", name);

                var info = new UTF8Encoding(true).GetBytes(content);
                fs.Write(info, 0, info.Length);
            }
        }

        private static string TemplatePath = "Packages/com.fasteraune.scriptableobjectvariables/Editor/Templates/";

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

        public static void GenerateScripts(Type[] types, string path, string nameSpace = "Generated")
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var file in new DirectoryInfo(path).GetFiles())
            {
                if (file.Extension.Contains("meta"))
                {
                    continue;
                }

                file.Delete();
            }

            foreach (var generateType in types)
            {
                var typeName = GetTypeAliasName(generateType, true);
                var fullTypeName = GetTypeAliasName(generateType);
                var name = UppercaseFirst(fullTypeName);

                CreateScriptFile(GetTemplate("Variable"), path, typeName, nameSpace, name, "Variable.cs");
                CreateScriptFile(GetTemplate("VariableProxy"), path, typeName, nameSpace, name, "VariableProxy.cs");
                CreateScriptFile(GetTemplate("VariableReference"), path, typeName, nameSpace, name, "VariableReference.cs");

                if (typeof(IComparable).IsAssignableFrom(generateType))
                {
                    CreateScriptFile(GetTemplate("ReferenceClamped"), path, typeName, nameSpace, name,
                        "ReferenceClamped.cs");
                }

                if (IsNumericType(generateType))
                {
                    CreateScriptFile(GetTemplate("ReferenceExpression"), path, typeName, nameSpace, name,
                        "ReferenceExpression.cs");
                }
                
                CreateScriptFile(GetTemplate("Event"), path, typeName, nameSpace, name, "Event.cs");
                CreateScriptFile(GetTemplate("EventReference"), path, typeName, nameSpace, name, "EventReference.cs");
                CreateScriptFile(GetTemplate("EventInvokable"), path, typeName, nameSpace, name, "EventInvokable.cs");
            }

            AssetDatabase.Refresh();
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
                var index = typeName.LastIndexOf(".");
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
