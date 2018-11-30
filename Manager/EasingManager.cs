using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApp.Interpolation
{
	public static class Extension
	{
		public static BaseInfo Easing_Position(this Transform transform, Easing.Type type, Vector3 end, float sec) { return new PosInfo (type, transform, transform.position, end, sec); }
		public static BaseInfo Easing_Position(this Transform transform, Easing.Type type, Vector3 start, Vector3 end, float sec) { return new PosInfo (type, transform, start, end, sec); }
		public static BaseInfo Easing_LocalPosition(this Transform transform, Easing.Type type, Vector3 end, float sec) { return new LocalPosInfo(type, transform, transform.position, end, sec); }
		public static BaseInfo Easing_LocalPosition(this Transform transform, Easing.Type type, Vector3 start, Vector3 end, float sec) { return new LocalPosInfo(type, transform, start, end, sec); }
		public static BaseInfo Easing_LocalScale(this Transform transform, Easing.Type type, Vector3 end, float sec) { return new ScaleInfo (type, transform, transform.localScale, end, sec); }
		public static BaseInfo Easing_LocalScale(this Transform transform, Easing.Type type, Vector3 start, Vector3 end, float sec) { return new ScaleInfo (type, transform, start, end, sec); }

		public static BaseInfo Easing_LocalPosition(this RectTransform transform, Easing.Type type, Vector3 end, float sec) { return new RectPosInfo (type, transform, transform.localPosition, end, sec); }
		public static BaseInfo Easing_LocalPosition(this RectTransform transform, Easing.Type type, Vector3 start, Vector3 end, float sec) { return new RectPosInfo (type, transform, start, end, sec); }
		public static BaseInfo Easing_LocalScale(this RectTransform transform, Easing.Type type, Vector3 end, float sec) { return new RectScaleInfo (type, transform, transform.localScale, end, sec); }
		public static BaseInfo Easing_LocalScale(this RectTransform transform, Easing.Type type, Vector3 start, Vector3 end, float sec) { return new RectScaleInfo(type, transform, start, end, sec); }
		public static BaseInfo Easing_SizeDelta(this RectTransform transform, Easing.Type type, Vector3 start, Vector3 end, float sec) { return new RectSizeDeltaInfo(type, transform, start, end, sec); }
		public static BaseInfo Easing_SizeDelta(this RectTransform transform, Easing.Type type, Vector3 end, float sec) { return new RectSizeDeltaInfo(type, transform, transform.sizeDelta, end, sec); }

		public static BaseInfo Easing_Color(this SpriteRenderer spriteRenderer, Easing.Type type, Color end, float sec) { return new SpriteColorInfo (type, spriteRenderer, spriteRenderer.color, end, sec); }
		public static BaseInfo Easing_Color(this SpriteRenderer spriteRenderer, Easing.Type type, Color start, Color end, float sec) { return new SpriteColorInfo (type, spriteRenderer, start, end, sec); }
		public static BaseInfo Easing_Color(this Image image, Easing.Type type, Color end, float sec) { return new ImageColorInfo (type, image, image.color, end, sec); }
		public static BaseInfo Easing_Color(this Image image, Easing.Type type, Color start, Color end, float sec) { return new ImageColorInfo (type, image, start, end, sec); }
		public static BaseInfo Easing_Color(this Text text, Easing.Type type, Color end, float sec) { return new TextColorInfo (type, text, text.color, end, sec); }
		public static BaseInfo Easing_Color(this Text text, Easing.Type type, Color start, Color end, float sec) { return new TextColorInfo (type, text, start, end, sec); }
		public static BaseInfo Easing_Color(this Camera camera, Easing.Type type, Color end, float sec) { return new CameraBackColorInfo(type, camera, camera.backgroundColor, end, sec); }
		public static BaseInfo Easing_Color(this Camera camera, Easing.Type type, Color start, Color end, float sec) { return new CameraBackColorInfo(type, camera, start, end, sec); }

		public static BaseInfo Easing_TimeScale(this Time time, Easing.Type type, float end, float sec) { return new TimeScaleInfo(type, Time.timeScale, end, sec); }
		public static BaseInfo Easing_TimeScale(this Time time, Easing.Type type, float start, float end, float sec) { return new TimeScaleInfo(type, start, end, sec); }
	}

	// 補間用情報クラス
	public abstract class BaseInfo
	{
		public float Sec { get; set; }
		public Easing.Type Type { get; set; }
		public IEnumerator Enumerator { get; set; }
        public abstract bool IsEnable { get; }
        public abstract GameObject GameObject { get; }

        public enum EventType
        {
            Update,
            Destroy,
        }
        public BaseInfo AddEvent(EventType type, Action action)
        {
            switch (type)
            {
                case EventType.Update: UpdateHandler += action; break;
                case EventType.Destroy: DestroyHandler += action; break;
            }
            return this;
        }
        public Action UpdateHandler { get; set; }   // アニメーション実行中に呼びたい処理
        public Action DestroyHandler { get; set; }   // アニメーション終了時に呼びたい処理

        protected abstract void ApplyEasing(float t);
		protected IEnumerator Calculate()
		{
			float t = 0.0f;
			float timer = 0.0f;
			while (!IsEnable || t != 1.0f)
			{
				timer += Time.deltaTime;
				if (timer > Sec) { timer = Sec; }
				t = 1.0f / Sec * timer;
				if (t > 1.0f) { t = 1.0f; }
				ApplyEasing(t);
                if (UpdateHandler != null) { UpdateHandler.Invoke(); }
                yield return null;
			}
		}

		public BaseInfo(Easing.Type type, float sec)
		{
			Type = type;
			Sec = sec;
			Enumerator = Calculate();

			EasingManager.Instance.InfoList.Add (this);
		}
	}


	// Transform補間ベースクラス
	public abstract class TransformInfo : BaseInfo
	{
		public Transform Transform { get; set; }
		public Vector3 Start { get; set; }
		public Vector3 End { get; set; }
		public TransformInfo(Easing.Type type, Transform transform, Vector3 start, Vector3 end, float sec)
			: base (type, sec)
		{
			Transform = transform;
			Start = start;
			End = end;
		}
        public override bool IsEnable { get { return Transform; } }
        public override GameObject GameObject { get { return Transform.gameObject; } }
    }
    // 座標補間クラス
    public class PosInfo : TransformInfo
	{
		public PosInfo (Easing.Type type, Transform transform, Vector3 start, Vector3 end, float sec)
			: base (type, transform, start, end, sec) { }
		protected override void ApplyEasing(float t) { Transform.position = Easing.Curve(Type, t, Start, End); }
	}
	// 座標補間クラス
	public class LocalPosInfo : TransformInfo
	{
		public LocalPosInfo(Easing.Type type, Transform transform, Vector3 start, Vector3 end, float sec)
			: base(type, transform, start, end, sec) { }
		protected override void ApplyEasing(float t) { Transform.localPosition = Easing.Curve(Type, t, Start, End); }
	}
	// スケール補間クラス
	public class ScaleInfo : TransformInfo
	{
		public ScaleInfo(Easing.Type type, Transform transform, Vector3 start, Vector3 end, float sec)
			: base (type, transform, start, end, sec) { }
		protected override void ApplyEasing(float t) { Transform.localScale = Easing.Curve(Type, t, Start, End); }
	}


	// RectTransform補間ベースクラス
	public abstract class RectTransformInfo : BaseInfo
	{
		public RectTransform Transform { get; set; }
		public Vector3 Start { get; set; }
		public Vector3 End { get; set; }
		public RectTransformInfo(Easing.Type type, RectTransform transform, Vector3 start, Vector3 end, float sec)
			: base (type, sec)
		{
			Transform = transform;
			Start = start;
			End = end;
		}
		public override bool IsEnable { get { return Transform; } }
        public override GameObject GameObject { get { return Transform.gameObject; } }
    }
    // 座標補間クラス
    public class RectPosInfo : RectTransformInfo
	{
		public RectPosInfo (Easing.Type type, RectTransform transform, Vector3 start, Vector3 end, float sec)
			: base (type, transform, start, end, sec) { }
		protected override void ApplyEasing(float t) { Transform.localPosition = Easing.Curve(Type, t, Start, End); }
	}
	// スケール補間クラス
	public class RectScaleInfo : RectTransformInfo
	{
		public RectScaleInfo(Easing.Type type, RectTransform transform, Vector3 start, Vector3 end, float sec)
			: base (type, transform, start, end, sec) { }
		protected override void ApplyEasing(float t) { Transform.localScale = Easing.Curve(Type, t, Start, End); }
	}
	// スケール補間クラス
	public class RectSizeDeltaInfo : RectTransformInfo
	{
		public RectSizeDeltaInfo(Easing.Type type, RectTransform transform, Vector3 start, Vector3 end, float sec)
			: base(type, transform, start, end, sec) { }
		protected override void ApplyEasing(float t) { Transform.sizeDelta = Easing.Curve(Type, t, Start, End); }
	}


	// カラー補間ベースクラス
	public abstract class ColorInfo : BaseInfo
	{
		public Color Start { get; set; }
		public Color End { get; set; }
		public ColorInfo(Easing.Type type, Color start, Color end, float sec)
			: base (type, sec) { Start = start; End = end; }
	}
	// スプライトの色補間クラス
	public class SpriteColorInfo : ColorInfo
	{
		public SpriteRenderer SpriteRenderer { get; set; }
		public SpriteColorInfo(Easing.Type type, SpriteRenderer spriteRenderer, Color start, Color end, float sec)
			: base (type, start, end, sec) { SpriteRenderer = spriteRenderer; }
		protected override void ApplyEasing(float t) { SpriteRenderer.color = Easing.Curve(Type, t, Start, End); }
		public override bool IsEnable { get { return SpriteRenderer; } }
        public override GameObject GameObject { get { return SpriteRenderer.gameObject; } }
    }
    // イメージの色補間クラス
    public class ImageColorInfo : ColorInfo
	{
		public Image Image { get; set; }
		public ImageColorInfo(Easing.Type type, Image image, Color start, Color end, float sec)
			: base (type, start, end, sec) { Image = image; }
		protected override void ApplyEasing(float t) { Image.color = Easing.Curve(Type, t, Start, End); }
		public override bool IsEnable { get { return Image; } }
        public override GameObject GameObject { get { return Image.gameObject; } }
    }
    // テキストの色補間クラス
    public class TextColorInfo : ColorInfo
	{
		public Text Text { get; set; }
		public TextColorInfo(Easing.Type type, Text text, Color start, Color end, float sec)
			: base (type, start, end, sec) { Text = text; }
		protected override void ApplyEasing(float t) { Text.color = Easing.Curve(Type, t, Start, End); }
		public override bool IsEnable { get { return Text; } }
        public override GameObject GameObject { get { return Text.gameObject; } }
    }
    // カメラの色補間クラス
    public class CameraBackColorInfo : ColorInfo
	{
		public Camera Camera { get; set; }
		public CameraBackColorInfo(Easing.Type type, Camera camera, Color start, Color end, float sec)
			: base(type, start, end, sec) { Camera = camera; }
		protected override void ApplyEasing(float t) { Camera.backgroundColor = Easing.Curve(Type, t, Start, End); }
		public override bool IsEnable { get { return Camera; } }
        public override GameObject GameObject { get { return Camera.gameObject; } }
    }

    // タイムスケールの補間クラス
    public class TimeScaleInfo : BaseInfo
	{
		private float Start = 0f;
		private float End = 0f;
		public TimeScaleInfo(Easing.Type type, float start, float end, float sec) : base(type, sec) { Start = start; End = end; }
		protected override void ApplyEasing(float t) { Time.timeScale = Easing.Curve(Type, t, Start, End); }
		public override bool IsEnable { get { return true; } }
        public override GameObject GameObject { get { return null; } }
    }



    public static class Easing
	{
		// 補間タイプ
		public enum Type
		{
			Linear,
			Cubic_In,
			Cubic_Out,
			Cubic_InOut,
			Quintic_In,
			Quintic_Out,
			Quintic_InOut,
			Elastic_In,
			Elastic_Out,
			Elastic_L_In,
			Elastic_L_Out,
		}
		public static float Curve (Type type, float t, float b, float c)
		{
			switch(type)
			{
			case Type.Linear:        return Linear (t, b, c);
			case Type.Cubic_In:      return CubicIn (t, b, c);
			case Type.Cubic_Out:     return CubicOut (t, b, c);
			case Type.Cubic_InOut:   return CubicInOut (t, b, c);
			case Type.Quintic_In:    return QuinticIn (t, b, c);
			case Type.Quintic_Out:   return QuinticOut (t, b, c);
			case Type.Quintic_InOut: return QuinticInOut (t, b, c);
			case Type.Elastic_In:   return ElasticIn(t, b, c);
			case Type.Elastic_Out: return ElasticOut(t, b, c);
			case Type.Elastic_L_In:   return ElasticLIn(t, b, c);
			case Type.Elastic_L_Out: return ElasticLOut(t, b, c);
			}
			Debug.LogError ("Interpolation.Curve [ " + type + " ]");
			return 0;
		}
		public static Vector2 Curve (Type type, float t, Vector2 b, Vector2 c)
		{
			return new Vector2 (Curve (type, t, b.x, c.x), Curve (type, t, b.y, c.y));
		}
		public static Vector3 Curve (Type type, float t, Vector3 b, Vector3 c)
		{
			return new Vector3 (Curve (type, t, b.x, c.x), Curve (type, t, b.y, c.y), Curve (type, t, b.z, c.z));
		}
		public static Color Curve (Type type, float t, Color b, Color c)
		{
			return new Color (Curve (type, t, b.r, c.r), Curve (type, t, b.g, c.g), Curve (type, t, b.b, c.b), Curve (type, t, b.a, c.a));
		}

		// Linear
		public static float Linear (float t, float b, float c) {
			c = c - b;
			return c * t / 1.0f + b;
		}
		// Cubic
		public static float CubicIn (float t, float b, float c) {
			c = c - b;
			t /= 1.0f;
			return c * t * t * t + b;
		}
		public static float CubicOut (float t, float b, float c) {
			c = c - b;
			t /= 1.0f;
			t--;
			return c * (t * t * t + 1) + b;
		}
		public static float CubicInOut (float t, float b, float c) {
			c = c - b;
			t /= 1.0f / 2;
			if (t < 1) return c / 2 * t * t * t + b;
			t -= 2;
			return c / 2 * (t * t * t + 2) + b;
		}
		// Quintic
		public static float QuinticIn (float t, float b, float c) {
			c = c - b;
			t /= 1.0f;
			return c * t * t * t * t * t + b;
		}
		public static float QuinticOut (float t, float b, float c) {
			c = c - b;
			t /= 1.0f;
			t--;
			return c * (t * t * t * t * t + 1) + b;
		}
		public static float QuinticInOut (float t, float b, float c) {
			c = c - b;
			t /= 1.0f / 2;
			if (t < 1) return c / 2 * t * t * t * t * t + b;
			t -= 2;
			return c / 2 * (t * t * t * t * t + 2) + b;
		}
		// Elastic
		public static float ElasticIn(float t, float b, float c) {
			float ts = (t /= 1.0f) * t;
			float tc = ts * t;
			return b + c * (33 * tc * ts + -59 * ts * ts + 32 * tc + -5 * ts);
		}
		public static float ElasticOut(float t, float b, float c) {
			float ts = (t /= 1.0f) * t;
			float tc = ts * t;
			return b + c * (33 * tc * ts + -106 * ts * ts + 126 * tc + -67 * ts + 15 * t);
		}
		// Elastic L
		public static float ElasticLIn(float t, float b, float c)
		{
			float ts = (t /= 1.0f) * t;
			float tc = ts * t;
			return b + c * (56 * tc * ts + -105 * ts * ts + 60 * tc + -10 * ts);
		}
		public static float ElasticLOut(float t, float b, float c)
		{
			float ts = (t /= 1.0f) * t;
			float tc = ts * t;
			return b + c * (56 * tc * ts + -175 * ts * ts + 200 * tc + -100 * ts + 20 * t);
		}
	}
}

namespace MyApp
{
	// 補間管理クラス
	public class EasingManager : SingletonMonoBehaviour<EasingManager>
	{
		public List<Interpolation.BaseInfo> InfoList { get; set; }

		public int _calcCounter = 0;

		// 初期化処理
		protected override void Initialize()
		{
			InfoList = new List<Interpolation.BaseInfo>();

			// シーン切り替えの直前に登録してあるアニメーションを削除する
			FadeManager.Instance.EventBeforeChangeScene += () => { InfoList.Clear(); };
		}

		// 更新処理
		private void Update()
		{
			_calcCounter = InfoList.Count;

			// 要素がないときは通らない
			if (InfoList.Count <= 0) { return; }

			// 補間アニメーションの更新処理
			foreach (var unit in InfoList)
			{
				if (unit.IsEnable)
				{
					if (!unit.Enumerator.MoveNext())
					{
						unit.Enumerator = null;
					}
				}
			}

			// 処理が終了したものを削除する
			for (int i = InfoList.Count - 1; i >= 0; i--)
			{
				var info = InfoList[i];
				if (!info.IsEnable || info.Enumerator == null)
				{
					InfoList.Remove(info);
                    if (info.DestroyHandler != null) { info.DestroyHandler.Invoke(); }
				}
			}

			//		// Enumeratorがnullのものを全て削除
			//		InfoList.RemoveAll(InfoList => InfoList.Enumerator == null);
		}



        /// <summary>
        /// アニメーション中のゲームオブジェクトか確かめます
        /// </summary>
        /// <returns><c>true</c>, if animation was ised, <c>false</c> otherwise.</returns>
        /// <param name="obj">Object.</param>
        public bool IsAnimation(GameObject obj)
        {
            foreach (var info in InfoList)
            {
                if (obj == info.GameObject) { return true; }
            }
            return false;
        }
	}
}
