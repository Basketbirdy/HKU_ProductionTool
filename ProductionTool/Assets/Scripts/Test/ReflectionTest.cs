using UnityEngine;
using System.Reflection;

public class ReflectionTest : MonoBehaviour
{
    ExampleClass myClass = new ExampleClass();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MethodInfo[] nonPublicMethods = typeof(ExampleClass).GetMethods(BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly);
        MethodInfo[] publicMethods = typeof(ExampleClass).GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);

        LoopOverArray(nonPublicMethods);
        LoopOverArray(publicMethods);

        typeof(ExampleClass).GetMethod("SetSecret", BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly).Invoke(myClass, new object[] { "I'm always angry" });
        string secret = (string)typeof(ExampleClass).GetMethod("GetSecret", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Invoke(myClass, null);
        Debug.Log($"Secret: {secret}");
    }

    private void LoopOverArray(MethodInfo[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log($"[{i}] Public method: {array[i].Name}");

            ParameterInfo[] methodParameters = array[i].GetParameters();

            if (methodParameters.Length == 0) { Debug.Log($"[{i}] {array[i].Name} output: {array[i].Invoke(myClass, null)}"); }
        }
    }
}
