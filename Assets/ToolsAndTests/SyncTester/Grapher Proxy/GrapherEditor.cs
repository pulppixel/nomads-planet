using UnityEngine;
using System.Collections;

public class GrapherEditor : MonoBehaviour {

	public enum GraphOutputChoices {Distance,Speed};

	public GrapherComponent Target;
	public GrapherComponent[] Instances;

	public GraphOutputChoices GraphOutput;


	public void SetGraphOutput(GraphOutputChoices output)
	{
		GraphOutput = output;
	}
}
