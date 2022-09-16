using System;
using UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Staging
{
	public class PlayerListItem : GridCell
	{
		[SerializeField] private Text _name;
		[SerializeField] private Image _color;
		[SerializeField] private GameObject _ready;
		[SerializeField] private Text _ping;

		public void Setup(Player ply)
		{
			_name.text = ply.Name;
			_color.color = ply.Color;
			_ready.SetActive(ply.Ready);
			_ping.text = Mathf.RoundToInt((float)ply.RTT * 1000) + " ms";
		}
	}
}