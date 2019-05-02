using UnityEngine;

namespace Project.Scripts.StageSelectScene
{
	public class VeryHardStageSelectDirector : StageSelectDirector
	{
		protected override Color SetButtonColor()
		{
			return new Color(1.0f, 0.25f, 0.25f);
		}
	}
}
