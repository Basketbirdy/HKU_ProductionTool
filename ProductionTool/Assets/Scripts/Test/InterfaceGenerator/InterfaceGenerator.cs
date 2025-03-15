using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEditor;

public class InterfaceGenerator : MonoBehaviour
{
    Type exampleType = typeof(ExampleClass);

    [ContextMenu("Generate example")]
    public void GenerateExample()
    {
        GenerateInterface(exampleType);
    }

    public void GenerateInterface(Type type)
    {
        string nameSpace = "InterfaceGeneration";

        string interfaceName = "I" + type.Name;
        MethodInfo[] publicMethods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);

        StringBuilder sb = new StringBuilder();

        EvaluateCode(sb, nameSpace, interfaceName, publicMethods);
    }

    private void EvaluateCode(StringBuilder sb, string nameSpace, string interfaceName, MethodInfo[] methods)
    {
        Include(sb);
        Header(sb, nameSpace, interfaceName);
        {
            Body(sb, methods);
        }
        Footer(sb);


        string url = Path.Combine(Application.dataPath, interfaceName + ".cs"); // Application.dataPath = asset folder
        StreamWriter streamWriter = new StreamWriter(url);

        streamWriter.Write(sb);
        streamWriter.Flush();
        streamWriter.Close();

        //AssetDatabase.Refresh();
    }

    private void Include(StringBuilder sb)
    {
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("");
    }

    private void Header(StringBuilder sb, string nameSpace, string name)
    {
        sb.AppendLine($"namespace {nameSpace} {{");
        sb.AppendLine($"\tpublic interface {name} {{");
    }

    private void Body(StringBuilder sb, MethodInfo[] methods)
    {
        foreach ( MethodInfo method in methods )
        {
            ParameterInfo[] paramters = method.GetParameters();
            List<Type> parameterTypes = new List<Type>();
            List<string> parameterNames = new List<string>();
            string returnType = method.ReturnType.Name;
            returnType = returnType.ToLower();
            foreach ( ParameterInfo param in paramters )
            {
                parameterTypes.Add(param.GetType());
                parameterNames.Add(param.Name);
            }

            sb.AppendLine($"\t\tpublic {returnType} {method.Name}();");
        }
        sb.AppendLine($"\t}}");
    }

    private void Footer(StringBuilder sb)
    {
        sb.AppendLine("}");
    }
}
