using System;
using System.CodeDom;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using UnityEditor;

namespace Fasteraune.Variables.Editor
{
    public class ScriptGenerator
    {
        private static string variableTemplate =
            @"using UnityEngine;
using Fasteraune.Variables;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = ""Variables/#NAME#"")]
public class #NAME#Variable : Variable<#TYPE#>
{
#if UNITY_EDITOR

    private #NAME#VariableProxy cachedProxy;

    public override SerializedObject GetRuntimeValueWrapper()
    {
        if (cachedProxy == null)
        {
            cachedProxy = CreateInstance(typeof(#NAME#VariableProxy)) as #NAME#VariableProxy;
        }
        
        cachedProxy.ProxyValue = Value;
        return new SerializedObject(cachedProxy);
    }
    
    public override SerializedObject GetInitialValueWrapper()
    {
        if (cachedProxy == null)
        {
            cachedProxy = CreateInstance(typeof(#NAME#VariableProxy)) as #NAME#VariableProxy;
        }
        
        cachedProxy.ProxyValue = InitialValue;
        return new SerializedObject(cachedProxy);
    }

    public override void ApplyModifiedValue(SerializedObject serializedObject)
    {
        #NAME#VariableProxy proxy = serializedObject.targetObject as #NAME#VariableProxy;
        RuntimeValue = proxy.ProxyValue;
    }
    
#endif
}";

        private static string referenceTemplate =
            @"using System;
using Fasteraune.Variables;

[Serializable]
public class #NAME#Reference : BaseReference<#TYPE#, #NAME#Variable>
{
    public #NAME#Reference(#TYPE# Value) : base(Value)
    {
    }

    public #NAME#Reference()
    {
        
    }
}";

        private static string proxyTemplate =
            @"using UnityEngine;
using Fasteraune.Variables;

#if UNITY_EDITOR

class #NAME#VariableProxy : ScriptableObject
{
    public #TYPE# ProxyValue;
}

#endif";

        private static void CreateScriptFile(string content, string path, string typeName, string name,
            string fileNamePostfix)
        {
            using (var fs = File.Create(path + name + fileNamePostfix))
            {
                content = content.Replace("#TYPE#", typeName);
                content = content.Replace("#NAME#", name);

                var info = new UTF8Encoding(true).GetBytes(content);
                fs.Write(info, 0, info.Length);
            }
        }

        public static void GenerateScripts(Type[] types, string path)
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

                CreateScriptFile(variableTemplate, path, typeName, name, "Variable.cs");
                CreateScriptFile(referenceTemplate, path, typeName, name, "Reference.cs");
                CreateScriptFile(proxyTemplate, path, typeName, name, "VariableProxy.cs");
            }

            AssetDatabase.Refresh();
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
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}