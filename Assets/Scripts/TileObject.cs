using UnityEngine;
using System.Collections;

public class TileObject : MonoBehaviour {

	public enum ObjectType {
		Undefined = -1,
		Ghost,
		Human,
		Tormentor,
		Crate,
		Goal,
		Lever,
		Wall
	};

	public bool m_bIsPassableIfGhost = true;
	public bool m_bIsPassableIfHuman = false;
	public bool m_bIsPassableIfTormentor = false;

	private bool m_bIsMoving = false;
	private Vector3 m_previousPosition = Vector3.zero;
	private Vector3 m_targetPosition = Vector3.zero;

	private bool m_bIsTormentorMovement = false;

	public AudioSource m_objectSound = null;


	public ObjectType m_objectType = ObjectType.Undefined;

	public bool CanPassThroughIf( TileObject.ObjectType p_type ) {

		switch( p_type ) {

			case ObjectType.Ghost:		return m_bIsPassableIfGhost;
			case ObjectType.Human: 		return m_bIsPassableIfHuman;
			case ObjectType.Tormentor: 	return m_bIsPassableIfTormentor;
			case ObjectType.Crate: 		return false;
			default: 					return false;

		}
	}

	public void MoveTo( Vector3 p_position ) {

		m_previousPosition = transform.position;
		m_targetPosition = p_position;
		m_bIsMoving = true;

	}

	public void MoveTormentor( Vector3 p_position ) {

		m_bIsTormentorMovement = true;
		MoveTo( p_position );

	}

	public void Update () {

		if ( ! m_bIsMoving ) { return; }

		float interval = InputManager.GetInputInterval();

		//Compute stuff for tormentor movement
		if ( m_bIsTormentorMovement ) {

			interval = 0.25f;
			m_previousPosition = this.transform.position;

		}
		if ( interval >= 1f ) {

			m_bIsMoving = false;
			transform.position = m_targetPosition;
			m_previousPosition = m_targetPosition;
					
		} else {

			transform.position = Vector3.Lerp( m_previousPosition, m_targetPosition, interval );

				
		}

		//End tormentor movement here
		if ( m_bIsTormentorMovement ) {

			float distance = ( m_targetPosition - transform.position ).magnitude;
			if ( distance < 0.5f ) {
			Debug.Log( "Caught!" );
				m_bIsMoving = false;
				m_bIsTormentorMovement = false;
				Invoke( "RepeatLevel", 2.0f );
				m_objectSound.Play();
				Player.Instance().PlayCrystal();

			}

		}
	}

	private void RepeatLevel() {

		LevelManager.ResetLevel();

	}
}
