using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public interface IGrapher
{
	void OnSetLocalPosition(Vector3 position);

	void OnSetRemotePosition(Vector3 position,string name,Color color);

	void OnSetValue(float value, string name, Color color);

	void OnSetTrigger(string name, Color color);
}

public interface IGrapherComponent
{
	void SetGrapherInterface(IGrapher grapher);
}

public abstract class GrapherComponent : MonoBehaviour, IGrapherComponent
{
	protected IGrapher GrapherInterface;

	#region ITriangle implementation
	public void SetGrapherInterface(IGrapher grapher)
	{
		GrapherInterface = grapher;
	}
	#endregion
}

public class GrapherProxy : GrapherComponent
{
	public new string name = "Grapher";
	public Color color;
	public bool IsLocal = false;


	public void LateUpdate()
	{
		if (GrapherInterface != null)
		{
			if (IsLocal)
			{
				GrapherInterface.OnSetLocalPosition (this.transform.position);
			}
			else
			{
				GrapherInterface.OnSetRemotePosition (this.transform.position, name, color);
			}
		}
	}
}