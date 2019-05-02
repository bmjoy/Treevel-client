using UnityEngine;

namespace Project.Scripts.StageSelectScene
{
	public class EasyStageSelectDirector : StageSelectDirector
	{
		protected override Color SetButtonColor()
		{
			return new Color(1.0f, 1.0f, 1.0f);
		}
	}
}
