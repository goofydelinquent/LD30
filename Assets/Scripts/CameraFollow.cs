using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	private GameObject m_target = null;

	public float m_offsetX = 0;
	public float m_offsetZ = -7.5f;
	public float m_maxDistance = 2;
	public float m_speed = 20;


	public void SetTarget ( GameObject p_target ) {

		m_target = p_target;
		transform.position = p_target.transform.position + new Vector3( 0, 10f, -7.5f );
		transform.rotation = Quaternion.Euler( 55, 0, 0 );

	}

	void Update () {

		if ( m_target == null ) { return; }

		float x = ( ( m_target.transform.position.x + m_offsetX - this.transform.position.x ) ) / m_maxDistance;
		float z = ( ( m_target.transform.position.z + m_offsetZ - this.transform.position.z ) ) / m_maxDistance;

		this.transform.position += new Vector3( ( x * m_speed * Time.deltaTime ), 0, ( z * m_speed * Time.deltaTime ) );
	
	}
}
