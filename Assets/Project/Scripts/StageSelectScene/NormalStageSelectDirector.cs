using UnityEngine;

namespace Project.Scripts.StageSelectScene
{
	public class NormalStageSelectDirector : StageSelectDirector
	{
		protected override Color SetButtonColor()
		{
			return new Color(1.0f, 0.75f, 0.75f);
		}
	}
}
