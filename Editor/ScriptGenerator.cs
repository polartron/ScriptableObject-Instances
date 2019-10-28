using System;
using System.CodeDom;
using System.Collections.Generic;
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

        private static string clampedTemplate =
            @"using System;
using Fasteraune.Variables;

[Serializable]
public class #NAME#ClampedReference : BaseReference<#TYPE#, #NAME#Variable>
{
    [Serializable]
    public class ClampData
    {
        public #NAME#Reference Min = new #NAME#Reference();
        public #NAME#Reference Max = new #NAME#Reference();
    }
    
    public ClampData Clamp = new ClampData();
    
    public #NAME#ClampedReference(#TYPE# Value) : base(Value)
    {
    }

    public #NAME#ClampedReference()
    {
    }

    public override #TYPE# Value
    {
        get
        {
            #TYPE# value = base.Value;
            #TYPE# maxValue = Clamp.Max.Value;
                    
            if (value.CompareTo(maxValue) > 0)
            {
                base.Value = maxValue;
                return maxValue;
            }
            
            #TYPE# minValue = Clamp.Min.Value;
                 
            if (value.CompareTo(minValue) < 0)
            {
                base.Value = minValue;
                return minValue;
            }

            return value;
        }
        set
        {
            #TYPE# maxValue = Clamp.Max.Value;
                    
            if (value.CompareTo(maxValue) > 0)
            {
                base.Value = maxValue;
                return;
            }
            
            #TYPE# minValue = Clamp.Min.Value;
                 
            if (value.CompareTo(minValue) < 0)
            {
                base.Value = minValue;
                return;
            }
            
            base.Value = value; 
        }
    }

    private void OnMinValueChanged(#TYPE# minValue)
    {
        if (minValue.CompareTo(base.Value) < 0)
        {
            base.Value = minValue;
        }
    }
    
    private void OnMaxValueChanged(#TYPE# maxValue)
    {
        if (maxValue.CompareTo(base.Value) > 0)
        {
            base.Value = maxValue;
        }
    }

    public override void AddListener(Action<#TYPE#> listener)
    {
        Clamp.Min.AddListener(OnMinValueChanged);
        Clamp.Max.AddListener(OnMaxValueChanged);
        base.AddListener(listener);
    }
    
    public override void RemoveListener(Action<#TYPE#> listener)
    {
        Clamp.Min.RemoveListener(OnMinValueChanged);
        Clamp.Max.RemoveListener(OnMaxValueChanged);
        base.RemoveListener(listener);
    }
}";

        private static void CreateScriptFile(string template, string path, string typeName, string name,
            string fileNamePostfix)
        {
            CreateScriptFile(new[] {template}, path, typeName, name, fileNamePostfix);
        }
        
        private static void CreateScriptFile(IEnumerable<string> templates, string path, string typeName, string name,
            string fileNamePostfix)
        {
            using (var fs = File.Create(path + name + fileNamePostfix))
            {
                string finalContent = "";

                foreach (var template in templates)
                {
                    string content = template.Replace("#TYPE#", typeName);
                    content = content.Replace("#NAME#", name);

                    finalContent += content;
                }
                

                var info = new UTF8Encoding(true).GetBytes(finalContent);
                fs.Write(info, 0, info.Length);
            }
        }

        private class GenerateSettings
        {
            
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
                CreateScriptFile(proxyTemplate, path, typeName, name, "VariableProxy.cs");
                
                List<string> referenceTemplates = new List<string>();
                referenceTemplates.Add(referenceTemplate);

                if (typeof(IComparable).IsAssignableFrom(generateType))
                {
                    referenceTemplates.Add(clampedTemplate);
                }
                
                CreateScriptFile(referenceTemplates, path, typeName, name, "Reference.cs");
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