//#define FADE_MANAGER_USE_SHADER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApp
{
	//	シーン遷移管理クラス（キャンバスに付与する）
	public class FadeManager : SingletonMonoBehaviour<FadeManager>
	{
		public enum Fade
		{
			None,
			In,
			Out,
		}

		private Fade m_Fade = Fade.None;
		public Fade FadeState { get { return m_Fade; } }

		private string m_NextSceneName = string.Empty;
		private float m_Param = 0.0f;

		[SerializeField]
		private float m_Speed = 0.05f;

		[SerializeField]
		private bool m_IsSetup = false;
		[SerializeField]
		private Shader m_FadeShader = null;


		//	フェードイメージ
		private Image m_Image;


		//	シーンが切り替わる前に呼びたい処理を登録する
		public delegate void Delegate();
		public Delegate EventBeforeChangeScene = delegate { };
		public Delegate EventAfterChangeScene = delegate { };


		//	初期化処理
		protected override void Initialize()
		{
			if (m_IsSetup)
			{
				m_Image = transform.GetChild(0).GetComponent<Image>();
				m_Image.material = new Material(m_FadeShader);
				return;
			}

			//	キャンバスを設定
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 999;

			//	キャンバスのスケール設定
			CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

			//	フェードイメージの作成
			GameObject fadeObject = new GameObject("Image");
			fadeObject.transform.parent = transform;

			//	イメージコンポーネント設定
			m_Image = fadeObject.AddComponent<Image>();
			m_Image.rectTransform.offsetMin = Vector2.zero;
			m_Image.rectTransform.offsetMax = Vector2.zero;
			m_Image.rectTransform.anchorMin = Vector2.zero;
			m_Image.rectTransform.anchorMax = Vector2.one;
			m_Image.rectTransform.pivot = Vector2.one * 0.5f;
			m_Image.color = Color.black;
#if FADE_MANAGER_USE_SHADER
			m_Image.sprite = Resources.Load<Sprite> ("FadeImage/Image0");
			m_Image.material = new Material (Shader.Find ("Custom/Fade"));
#else
			m_Image.color = Color.clear;
#endif
			m_Image.raycastTarget = false;
		}

		//	更新処理
		private void Update()
		{
			switch (m_Fade)
			{
				case Fade.None:
					break;

				case Fade.In:
					m_Param -= m_Speed * Time.deltaTime * Define.BaseFps;
#if FADE_MANAGER_USE_SHADER
				m_Image.material.SetFloat("_Param", m_Param);
#else
					m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, m_Param);
#endif

					if (m_Param <= 0.0f)
					{
						m_Param = 0.0f;
						m_Fade = Fade.None;
					}
					break;

				case Fade.Out:
					m_Param += m_Speed * Time.deltaTime * Define.BaseFps;
#if FADE_MANAGER_USE_SHADER
				m_Image.material.SetFloat("_Param", m_Param);
#else
					m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, m_Param);
#endif

					if (m_Param >= 1.0f)
					{
						m_Param = 1.0f;
						m_Fade = Fade.In;

						EventBeforeChangeScene();
						UnityEngine.SceneManagement.SceneManager.LoadScene(m_NextSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

						// ガベージコレクション
						System.GC.Collect();

						EventAfterChangeScene();
					}
					break;
			}
		}

		//	シーンの読み込み
		public bool LoadScene(string name)
		{
			if (m_Fade == Fade.None)
			{
				m_Fade = Fade.Out;
				m_NextSceneName = name;
				return true;
			}
			return false;
		}
	}
}
