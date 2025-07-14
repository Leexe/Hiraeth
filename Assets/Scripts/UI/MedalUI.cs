using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MedalUI : MonoBehaviour {
	[Header("References")]
	[SerializeField] private Sprite bronzeMedal;
	[SerializeField] private Sprite silverMedal;
	[SerializeField] private Sprite goldMedal;
	[SerializeField] private Image _image;

	[Header("Required Field For Main Menu")]
	[SerializeField] private bool _isInMainMenu = false;
	[ShowIf("_isInMainMenu")]
	[SerializeField] private LevelManger.Level _level;

	private void OnEnable() {
		LevelManger.Medal medal;

		// Get the medal for a level
		if (LevelManger.Instance.CurrentLevel == LevelManger.Level.mainMenu) {
			medal = LevelManger.Instance.GetMedalWithHighScore(_level);
		}
		else {
			medal = LevelManger.Instance.GetMedal(LevelManger.Instance.CurrentLevel, GameManager.Instance.TimeOnLevel);
		}

		// Show the medal
		if (medal == LevelManger.Medal.none) {
			_image.color = new Color(0, 0, 0, 0);
		}
		else if (medal == LevelManger.Medal.bronze) {
			_image.sprite = bronzeMedal;
		}
		else if (medal == LevelManger.Medal.silver) {
			_image.sprite = silverMedal;
		}
		else {
			_image.sprite = goldMedal;
		}
	}
}
