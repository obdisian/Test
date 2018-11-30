using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

namespace MyApp
{
	public class FacebookManager : SingletonMonoBehaviour<FacebookManager>
	{
		// initialize
		protected override void Initialize()
		{
            FBInitialize();
        }
		
		// start
		private void Start()
		{

		}

		// update
		private void Update()
		{

		}


        // Facebook初期化
        private void FBInitialize()
        {
            if (!FB.IsInitialized)
            {
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                    {
                        FB.ActivateApp();
                        Log($"FB.ActivateApp");
                    }
                },
                isShown =>
                {
                    // ゲームなんかの場合はここでゲームを止めたり
                    Log($"isShown");
                });
            }
            else
            {
                FB.ActivateApp();
                Log($"FB.ActivateApp");
            }
        }

        private void Log(string message)
        {
            Debug.Log($"[Facebook]: {message}");
        }
    }
}
