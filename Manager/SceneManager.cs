using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyApp.Scene;

namespace MyApp
{
	/// <summary>
	/// シーン管理クラス
	/// </summary>
	public class SceneManager : SingletonMonoBehaviour<SceneManager>
	{
		[SerializeField]
		public SceneBase _nowScene = null;
		public SceneBase NowScene { get { return _nowScene; } set { _nowScene = value; } }


		public void Load(SceneDefine.Info info, bool fade = true)
		{
			//fade = false;
			if (fade)
			{
				if (FadeManager.Instance.LoadScene(info.Name))
				{
				}
			}
			else
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene(info.Name);
			}
		}
		public void AddScene(string name)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
		}





		protected override void Initialize()
		{
		}

		// 初期化
		private void Start()
		{

		}

		// 更新処理
		private void Update()
		{
		}
	}
}
