using UnityEngine;

public interface IPreviewInterface
{
    public string GetName();
    public void SetName(string newName);

    public void Prepare();
    public void DoSomething();
}
