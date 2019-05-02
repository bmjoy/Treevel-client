using UnityEngine;

namespace Project.Scripts.StageSelectScene
{
	public class HardStageSelectDirector : StageSelectDirector
	{
		protected override Color SetButtonColor()
		{
			return new Color(1.0f, 0.50f, 0.50f);
		}
	}
}
