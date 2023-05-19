using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public class PunProfilerGraphPanel : MonoBehaviour
	{
		public GameObject GraphView;
		public PunProfiler Manager;
		
		public void ToggleGraphView()
		{
			GraphView.SetActive(!GraphView.activeSelf);
		}

		public void DeleteGraphView()
		{
			Manager.DeleteGraph(this);
		}
	}
}
