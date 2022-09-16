using UnityEngine;

namespace GameUI
{
	public class GameOver : MonoBehaviour
	{
		public void OnContinue()
		{
			App.Instance.Session.LoadMap(MapIndex.Staging);
		}

		public void OnBackMenu()
		{
			App.Instance.Disconnect();
		}

		public void OnQuit()
		{
			App.Instance.Disconnect();
			Application.Quit();
		}
	}
}