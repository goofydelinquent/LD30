using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public static int TILE_SIZE = 2;

	public enum TileDirection {
		None = -1,
		North,
		South,
		West,
		East
	}

	public Tile m_tileNorth = null;
	public Tile m_tileSouth = null;
	public Tile m_tileWest  = null;
	public Tile m_tileEast  = null;

	public TileObject m_tileObject = null;

	// Use this for initialization
	void Start () {
		
	}
	

	
	// Update is called once per frame
	void Update () {
	
	}

	public Tile GetTileAt( Tile.TileDirection p_direction ) {
		
		switch( p_direction ) {
			case TileDirection.North:	return m_tileNorth;
			case TileDirection.South: 	return m_tileSouth;
			case TileDirection.West: 	return m_tileWest;
			case TileDirection.East: 	return m_tileEast;

			case TileDirection.None:
			default: 					return null;
		}
		
	}

	public bool CanMoveObject( Tile.TileDirection p_direction )
	{
		// If nothing to move...
		if ( m_tileObject == null ) { return false; }

		List<TileObject.ObjectType> list = new List<TileObject.ObjectType>();
		list.Add( m_tileObject.m_objectType );
		GetContinousObjectsAt( p_direction, ref list );

		int count = 0;

		foreach( TileObject.ObjectType type in list ) {

			switch ( type ) {

				// Crates can be pushed on to the goal but the goal cannot be pushed
				case TileObject.ObjectType.Goal: return count > 0;
							
				case TileObject.ObjectType.Human: // Can't squish humans
				case TileObject.ObjectType.Undefined: //Can't move past dead ends
				case TileObject.ObjectType.Wall : // Walls as well
				case TileObject.ObjectType.Lever:
					return false; 
			}
		}

		return true;

	}

	public void GetContinousObjectsAt( Tile.TileDirection p_direction, ref List<TileObject.ObjectType> p_list ) {

		if ( p_list == null ) return;

		Tile t = GetTileAt( p_direction );
		if ( t == null ) {

			//Add undefined to denote end of the world
			p_list.Add ( TileObject.ObjectType.Undefined );
			return;

		}
		if ( t.m_tileObject == null ) return;

		p_list.Add( t.m_tileObject.m_objectType );
		t.GetContinousObjectsAt( p_direction, ref p_list );

	}

	public TileObject CheckTormentorLineOfSight ( Tile.TileDirection p_direction ) {

		Tile t = GetTileAt( p_direction );

		if ( t == null ) return null;

		TileObject to = t.m_tileObject;
		if ( to != null) {

			//Objects that block
			if ( to.m_objectType == TileObject.ObjectType.Crate || to.m_objectType == TileObject.ObjectType.Wall ) {

				return null;

			} else if ( to.m_objectType == TileObject.ObjectType.Tormentor ) {

				// Kinda dirty code...
				// Basically check if tormentor is facing player.
				Vector3 forward = ( Player.Instance().transform.position - to.transform.position ).normalized;
				if ( forward == to.transform.forward ) {

					return to;

				}

			}

		}

		return  t.CheckTormentorLineOfSight( p_direction );

	}

	public bool SetTileObject ( TileObject p_obj ) {

		if ( p_obj != null && m_tileObject != null ) {

			return false;

		}

		m_tileObject = p_obj;
		return true;

	}

	public void MoveObject( Tile.TileDirection p_direction )
	{
		//Note this assumes that a check has already been made.

		Tile target = GetTileAt( p_direction );
		if ( target == null || m_tileObject == null ) { return; }

		TileObject to = m_tileObject;
		if ( to.m_objectType != TileObject.ObjectType.Crate ) { return; }

		//Chain move first
		target.MoveObject( p_direction );

		// Now do the move
		target.m_tileObject = to;
		m_tileObject = null;

		to.MoveTo( target.transform.position );

	}

	public void TriggerTile ()
	{
		if ( m_tileObject != null ) {

			if ( m_tileObject.m_objectType == TileObject.ObjectType.Goal 
			    && Player.Instance().GetCurrentForm() == TileObject.ObjectType.Ghost ) {

				Player.Instance().Warp();
				InputManager.Instance().SetInputEnabled( false );

			} else if ( m_tileObject.m_objectType == TileObject.ObjectType.Human ) {

				Player.Instance().Possess( m_tileObject );
				//Let go of the object
				m_tileObject = null;

			}

		}

		List<TileObject> tormentorList = new List<TileObject>();
		tormentorList.Add( CheckTormentorLineOfSight( TileDirection.North ) );
		tormentorList.Add( CheckTormentorLineOfSight( TileDirection.South ) );
		tormentorList.Add( CheckTormentorLineOfSight( TileDirection.West ) );
		tormentorList.Add( CheckTormentorLineOfSight( TileDirection.East ) );

		bool bDidHaveTormentor = false;
		foreach( TileObject to in tormentorList ) {

			if ( to == null ) { continue; }

			InputManager.Instance().SetInputEnabled( false );
			to.MoveTormentor( Player.Instance().transform.position );
			bDidHaveTormentor = true;

			//If first tormentor
			if ( bDidHaveTormentor == false ) {
			}

			bDidHaveTormentor = true;

		}

	}
}
