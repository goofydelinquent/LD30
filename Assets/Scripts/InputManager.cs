using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	private Queue<Tile.TileDirection> m_commandQueue = new Queue<Tile.TileDirection>();
	private const float INPUT_DELAY = 0.25f; //NOTE: Must be > 0
	private static float m_currentDelay = 0f;
	private Player m_player = null;
	private bool m_bIsInputEnabled = true;

	private static InputManager m_manager = null;

	public static InputManager Instance () {

		return m_manager;

	}

	// Use this for initialization
	void Awake () {

		m_manager = this;

		if ( m_commandQueue == null ) {
			m_commandQueue = new Queue<Tile.TileDirection>();
		}

		m_commandQueue.Clear();

	}

	void OnDestroy () {

		m_manager = null;

	}

	public void SetPlayer ( Player p_player ) {

		m_player = p_player;

	}

	private void AddCommand ( Tile.TileDirection p_direction ) {

		if ( m_commandQueue.Count < 5 ) {
			m_commandQueue.Enqueue( p_direction );
		}
	}

	public static float GetInputInterval () {
		return 1f - ( Mathf.Max( m_currentDelay, 0.0f ) / INPUT_DELAY );
	}
	
	// Update is called once per frame
	void Update () {

		if ( m_bIsInputEnabled == false ) { return; }

		if ( Input.GetButtonDown( "reset" ) ) {

			LevelManager.ResetLevel();
			return;

		}


		if ( Input.GetButtonDown( "up" ) ) 		{ AddCommand( Tile.TileDirection.North ); }
		if ( Input.GetButtonDown( "down" ) ) 	{ AddCommand( Tile.TileDirection.South ); }
		if ( Input.GetButtonDown( "left" ) ) 	{ AddCommand( Tile.TileDirection.West ); }
		if ( Input.GetButtonDown( "right" ) ) 	{ AddCommand( Tile.TileDirection.East ); }

		if ( m_commandQueue.Count > 0 && m_currentDelay <= 0f ) {

			ProcessInput();

		}

		// Do this after, so that others that depend on the input interval can be processed
		if ( m_currentDelay > 0f ) {

			m_currentDelay -= Time.deltaTime;

		}	

	}

	public void ClearInput()
	{
		m_commandQueue.Clear();
	}

	public void SetInputEnabled( bool p_bIsEnabled ) {

		m_bIsInputEnabled = p_bIsEnabled;
		if ( m_bIsInputEnabled == false ) {

			m_commandQueue.Clear();

		}
	}

	void ProcessInput () {

		Player.MoveResult result = Player.MoveResult.Blocked;
		while( true ) {

			if ( m_commandQueue.Count <= 0 ) { return; }
		
			Tile.TileDirection dir = m_commandQueue.Peek();
			result = m_player.MoveTo( dir );

			if ( result == Player.MoveResult.Blocked ) {
				m_commandQueue.Dequeue();
				continue;
			}

			if ( result == Player.MoveResult.IsMoving ) {
				return;
			}

			if ( result == Player.MoveResult.Success ) {
				m_commandQueue.Dequeue();
				m_currentDelay = INPUT_DELAY;
				break;
			}

		} 
	}
}
