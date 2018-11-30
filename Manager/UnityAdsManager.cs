using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace MyApp
{
	public class UnityAdsManager : SingletonMonoBehaviour<UnityAdsManager>
	{
        private const int ShowPlayCount = 10;
        [SerializeField]
        private int _playCounter = 0;



		// initialize
		protected override void Initialize()
		{

        }
		
		// start
		private void Start()
		{

		}

		// update
		private void Update()
		{

		}



        // 動画を開始する
        public void Show()
        {
            //return;
            _playCounter++;
            if (_playCounter < ShowPlayCount)
            {
                return;
            }
            _playCounter = 0;

            // 動画の再生
            if (Advertisement.IsReady())
            {
                Advertisement.Show();
            }
        }

        // リワード動画を開始する
        public void ShowRewarded(System.Action<ShowResult> handle)
        {
            if (Advertisement.IsReady("rewardedVideo"))
            {
                var options = new ShowOptions { resultCallback = handle };
                Advertisement.Show("rewardedVideo", options);
            }
        }
    }
}
