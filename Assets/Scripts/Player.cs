using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour {

	public GameObject m_crystal = null;
	public TileObject m_ghost = null;
	private TileObject m_human = null;

	private Tile m_currentTile = null;
	private Tile m_nextTile = null;

	private bool m_bIsMoving = false;
	private Vector3 m_targetForward = Vector3.back;

	public Animator m_animatorCrystal = null;
	public Animator m_animatorGhost = null;

	private bool m_bIsWarping = false;

	public AudioSource m_warpSound = null;

	public enum MoveResult {
		Blocked = -1,
		IsMoving = 0,
		Success = 1,

	};

	private static Player m_player = null;

	public static Player Instance () {

		return m_player;

	}

	void Awake () {

		m_player = this;

	}

	void OnDestroy () {

		m_player = null;

	}


	public void SetCurrentTile ( Tile p_tile ) {

		m_currentTile = p_tile;
		transform.position = m_currentTile.transform.position;

	}

	public TileObject.ObjectType GetCurrentForm () {

		return m_human == null ? TileObject.ObjectType.Ghost : TileObject.ObjectType.Human;

	}

	public void Possess( TileObject p_obj ) {

		m_human = p_obj;
		m_human.transform.parent = this.transform;
		// TODO maybe animation later on?
		m_ghost.gameObject.SetActive( false );
	}

	public MoveResult MoveTo( Tile.TileDirection p_direction ) {

		if ( m_nextTile != null ) { return MoveResult.IsMoving; }
	
		Tile next = m_currentTile.GetTileAt( p_direction );
		if ( next == null ) { return MoveResult.Blocked; }

		if ( next.m_tileObject != null ) {

			TileObject.ObjectType currentForm = GetCurrentForm();
			bool bCanPass = next.m_tileObject.CanPassThroughIf( currentForm );
			if ( currentForm == TileObject.ObjectType.Ghost ) {

				if ( ! bCanPass ) { 

					return MoveResult.Blocked;

				}
				//else continue

			} else if ( ! bCanPass ) {

				//Check if wall, then release the human!
				if ( next.m_tileObject.m_objectType == TileObject.ObjectType.Wall ) {

					m_human.transform.parent = null;
					m_currentTile.SetTileObject( m_human );
					m_human = null;
					// TODO maybe animation later on?
					m_ghost.gameObject.SetActive( true );

				} else if ( next.CanMoveObject( p_direction ) ) {

					next.MoveObject( p_direction );

				} else {

					return MoveResult.Blocked;

				}

			}
		}

		m_nextTile = next;
		m_targetForward = ( m_nextTile.transform.position - m_currentTile.transform.position ).normalized;

		return MoveResult.Success;

	}

	public void Warp () {

		m_animatorCrystal.SetTrigger( "Warp" );
		m_animatorGhost.SetTrigger( "Warp" );
		m_warpSound.Play();

	}

	public void PlayCrystal () {

		m_animatorGhost.SetTrigger( "Warp" );

	}

	// Update is called once per frame
	void Update () {

		if ( m_bIsWarping ) { return; }

		if ( m_nextTile != null ) {

			float interval = InputManager.GetInputInterval();
			if ( interval >= 1f ) {

				m_currentTile = m_nextTile;
				m_nextTile = null;
				transform.position = m_currentTile.transform.position;
				transform.forward = m_targetForward;
				m_currentTile.TriggerTile();


			} else {

				Vector3 start = m_currentTile.transform.position;
				Vector3 end = m_nextTile.transform.position;

				transform.position = Vector3.Lerp( start, end, interval );
				transform.forward = Vector3.Slerp( transform.forward, m_targetForward, interval );

			}

		}

	}
}
