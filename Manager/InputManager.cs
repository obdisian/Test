using UnityEngine;
using System.Collections;

namespace MyApp
{
	//	入力管理クラス
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public enum TouchState
		{
			None,
			Move,
			Pinch,
		}

		//	正直二本以上は必要なさそう
		public static readonly int TouchCountMax = 2;


		//	各状態へ以降した際の初期化フラグ
		private bool isInit = false;
		private bool IsInit
		{
			get
			{
				if (isInit)
				{
					isInit = false;
					return true;
				}
				return false;
			}
		}

		//	タッチの状態
		private TouchState state = TouchState.None;
		public TouchState State
		{
			get { return state; }
			private set
			{
				if (state == value)
				{
					return;
				}

				isInit = true;
				state = value;
			}
		}


		//	ピンチした時の大きさ（前フレームとの差分）
		public float MovePinchScale { get; set; }

		//	スワイプのベクトル
		public Vector3 SwipeVector { get; set; }
		//	前フレームとの差分
		public Vector3 MoveVector { get; set; }


		//	1f前のピンチの大きさ
		private float prevMovePinchScale = 0;
		//	1f前の座標
		private Vector3 prevPos = Vector3.zero;
		//	押した瞬間の座標
		private Vector3 downPos = Vector3.zero;



		// 初期化
		protected override void Initialize()
		{
			State = TouchState.None;
			MovePinchScale = 0;
			SwipeVector = Vector3.zero;
			MoveVector = Vector3.zero;
		}


		// 更新
		void Update()
		{
			//	何も押していない時
			if (Input.touchCount <= 0)
			{
				State = TouchState.None;
			}

			//	シングルタッチではない場合
			if (Input.touchCount >= TouchCountMax)
			{
				State = TouchState.Pinch;
			}


			//	状態毎の処理
			switch (InputManager.Instance.State)
			{
				case TouchState.None: TouchStateNone(); break;
				case TouchState.Move: TouchStateMove(); break;
				case TouchState.Pinch: TouchStatePinch(); break;
			}
		}


		// StateがNoneの場合の更新処理
		void TouchStateNone()
		{
			//	初期化
			if (IsInit)
			{
				SwipeVector = Vector3.zero;
				MoveVector = Vector3.zero;
				prevPos = Vector3.zero;
				downPos = Vector3.zero;

				MovePinchScale = 0;
				prevMovePinchScale = 0;
			}

			//	何かしらが押している場合
			if (Input.GetMouseButton(0))
			{
				State = TouchState.Move;
			}
		}


		// StateがMoveの場合の更新処理
		void TouchStateMove()
		{
			Vector3 mousePos = Input.mousePosition;

			//	初期化
			if (IsInit)
			{
				downPos = mousePos;
				prevPos = mousePos;
			}

			if (mousePos != prevPos)
			{
				MoveVector = mousePos - prevPos;
				prevPos = mousePos;
			}
			else
			{
				MoveVector = Vector3.zero;
			}
			SwipeVector = mousePos - downPos;
		}


		// StateがPinchの場合の更新処理
		void TouchStatePinch()
		{
			//	指の数が減った時は状態を戻す
			if (Input.touchCount < TouchCountMax)
			{
				State = TouchState.None;
				return;
			}

			//	ピンチスケール
			float scale = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

			//	初期化
			if (IsInit)
			{
				prevMovePinchScale = scale;
			}

			//	ピンチスケールに差分がある時
			if (scale != prevMovePinchScale)
			{
				MovePinchScale = scale - prevMovePinchScale;
				prevMovePinchScale = scale;
			}
			else
			{
				MovePinchScale = 0;
			}
		}
	}
}
