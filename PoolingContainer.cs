namespace  TakashiCompany.Unity
{

	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// GameObjectを再利用するコンテイナー
	/// GameObject.activeSelf で再利用可/利用中を管理
	/// </summary>
	[System.Serializable]
	public class PoolingContainer<T> where T : Behaviour
	{
		[SerializeField]
		private T _prefab;

		public T prefab { get; private set; }

		[SerializeField]
		private Transform _container;

		[SerializeField]
		private bool _switchIsActive = true;

		private List<T> _list = new List<T>();

		public List<T> list { get{ return _list; } }

		public void Setup(T prefab, Transform container, bool switchIsActive = true)
		{
			_prefab = prefab;
			_container = container;
			_switchIsActive = switchIsActive;
		}

		/// <summary>
		/// 指定した個数までオブジェクトを生成しておく
		/// </summary>
		public void Prepare(int amount)
		{
			while(_list.Count < amount)
			{
				Generate();
			}
		}

		/// <summary>
		/// 全てのオブジェクトを回収する
		/// </summary>
		public void CollectAll()
		{
			foreach (var item in _list)
			{
				item.transform.SetParent(_container);

				if (_switchIsActive)
				{
					item.gameObject.SetActive(false);
				}
				else
				{
					item.enabled = false;
				}
			}
		}

		/// <summary>
		/// activeがtrue/falseのオブジェクトを一つ取得する
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		private T Find(bool flag)
		{
			if (_list == null) _list = new List<T>();
			
			if (_switchIsActive)
			{
				return _list.Find(m => m.gameObject.activeSelf == flag);
			}
			else
			{
				return _list.Find(m => m.enabled == flag);
			}
		}

		/// <summary>
		/// activeがtrue/falseのオブジェクトリストを取得する
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		private List<T> FindList(bool flag)
		{
			if (_list == null) _list = new List<T>();

			if (_switchIsActive)
			{
				return _list.FindAll(m => m.gameObject.activeSelf == flag);
			}
			else
			{
				return _list.FindAll(m => m.enabled == flag);
			}
		}

		/// <summary>
		/// オブジェクトを取得する
		/// </summary>
		/// <returns></returns>
		public T Get()
		{
			var myObject = Find(false);

			if (myObject == null)
			{
				myObject = Generate();
			}
			if (_switchIsActive)
			{
				myObject.gameObject.SetActive(true);
			}
			else
			{
				myObject.enabled = true;
			}
			return myObject;
		}

		/// <summary>
		/// アクティブなオブジェクトを取得する
		/// </summary>
		/// <returns></returns>
		public List<T> GetActives()
		{
			return FindList(true);
		}

		/// <summary>
		/// オブジェクトを生成する
		/// </summary>
		/// <returns></returns>
		private T Generate()
		{
			var myObject = GameObject.Instantiate(_prefab, _container);

			if (_switchIsActive)
			{
				myObject.gameObject.SetActive(false);
			}
			else
			{
				myObject.enabled = false;
			}
			myObject.name = _prefab.name + "_" + _list.Count;
			myObject.transform.localPosition = Vector3.zero;
			myObject.transform.localScale = _prefab.transform.localScale;
			_list.Add(myObject);

			return myObject;
		}

		/// <summary>
		/// オブジェクトが利用されているか
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsUsed(T obj)
		{
			if (_switchIsActive)
			{
				return obj.gameObject.activeSelf;
			}
			else
			{
				return obj.enabled;
			}
		}
	}
}