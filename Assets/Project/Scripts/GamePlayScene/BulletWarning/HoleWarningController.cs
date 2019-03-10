using System.Collections;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public abstract class HoleWarningController : BulletWarningController
	{
		public abstract void Initialize(int row, int column);
	}
}
