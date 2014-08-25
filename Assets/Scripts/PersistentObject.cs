using UnityEngine;
using System.Collections;

public class PersistentObject : MonoBehaviour {

	private static PersistentObject m_instance = null;

	public static PersistentObject Instance () {
		return m_instance;
	}

	void Awake () {

		if (m_instance != null && m_instance != this ) {

			Destroy( this.gameObject );
			return;

		} else {

			m_instance = this;

		}
		DontDestroyOnLoad(this.gameObject);
	}
}