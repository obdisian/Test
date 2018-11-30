using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyApp
{
	public class AudioManager : SingletonMonoBehaviour<AudioManager>
	{
		// 種類
		public enum Type
		{
			Bgm,
			Se,
			Max,
		}

		// 保持するデータ
		private class Data
		{
			public string Key;
			public string ResourceName;
			public AudioClip Clip;
			public Data(string key, string res)
			{
				Key = key;
				ResourceName = res;
				Clip = Resources.Load(ResourceName) as AudioClip;
			}
		}

		// サウンドリソース
		private AudioSource m_SourceBgm = null;
		private AudioSource m_SourceSe = null;
		private Dictionary<string, Data>[] m_DataTable = new Dictionary<string, Data>[(int)Type.Max];
		private Dictionary<string, Data> GetDataTable(Type type) { return m_DataTable[(int)type]; }

		// 初期化
		protected override void Initialize()
		{
			m_SourceBgm = gameObject.AddComponent<AudioSource>();
			m_SourceSe = gameObject.AddComponent<AudioSource>();
			m_DataTable[(int)Type.Bgm] = new Dictionary<string, Data>();
			m_DataTable[(int)Type.Se] = new Dictionary<string, Data>();
		}

		// 使用するAudioSourceを取得する
		private AudioSource UseAudioSource(Type type)
		{
			switch (type)
			{
				case Type.Bgm: return m_SourceBgm;
				case Type.Se: return m_SourceSe;
			}
			return null;
		}

		// BGM/SEの読み込み
		public void Setup(Type type, string key, string resourceName)
		{
			var table = GetDataTable(type);
			if (table.ContainsKey(key)) { table.Remove(key); }
			table.Add(key, new Data(key, resourceName));
		}

		// BGM/SEの再生
		public bool Play(Type type, string key)
		{
			var table = GetDataTable(type);
			if (!table.ContainsKey(key)) { return false; }
			AudioSource source = UseAudioSource(type);

			switch (type)
			{
				case Type.Bgm:
					StopBgm();
					source.loop = true;
					source.clip = table[key].Clip;
					source.Play();
					return true;

				case Type.Se:
					source.PlayOneShot(table[key].Clip);
					return true;
			}
			return false;
		}

		// BGMの停止
		public bool StopBgm()
		{
			UseAudioSource(Type.Bgm).Stop();
			return true;
		}
	}
}
