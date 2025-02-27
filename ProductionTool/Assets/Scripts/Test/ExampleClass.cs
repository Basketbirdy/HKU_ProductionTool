using UnityEngine;

public class ExampleClass : IPreviewInterface
{
    public string someData = "DATA";
    protected string name = "ExampleClass";
    private string secret = "I love snacks";

    public string GetName() {
        return name;
    }

    public void SetName(string newName) {
        name = newName;
    }

    public void Prepare() {
        Debug.Log("Preparing...");
	}

    public void DoSomething() {
        Debug.Log("DO SOMETHING!");
	}

    private void SetSecret( string newSecret ) {
        secret = newSecret;
	}

    private string GetSecret() {
        return secret;
    }
}
