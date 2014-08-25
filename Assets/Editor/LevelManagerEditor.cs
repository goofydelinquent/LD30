using UnityEngine;
using System.Collections;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor 
{
	private string KeyFor( int p_xIndex, int p_zIndex ) {

		return string.Format( "{0},{1}", p_xIndex, p_zIndex );

	}

	string m_filename = string.Empty;
	public override void OnInspectorGUI()
	{
		LevelManager myTarget = (LevelManager)target;
		DrawDefaultInspector();

		//m_filename = EditorGUILayout.TextField( "Filename", m_filename );


		if ( GUILayout.Button( "Clean & Compute Tiles" ) ) {

			// Fugly, but hell, this is fast and it's sure to work
			// ...and we only do this in the editor so it's ok.
			Dictionary<string, Tile> lookup = new Dictionary<string, Tile>();
			Dictionary<string, TileObject> objectLookup = new Dictionary<string, TileObject>();


			// Cleanup tile positions & Build lookup of tiles
			Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

			foreach( Tile t in tiles ) {

				Vector3 position = t.transform.position;
				
				int xIndex = Mathf.RoundToInt( position.x / 2f );
				int zIndex = Mathf.RoundToInt( position.z / 2f );
				
				position.x = xIndex * 2;
				position.z = zIndex * 2;
				
				t.transform.position = position;
				string key = KeyFor( xIndex, zIndex );

				try {

					lookup.Add( key, t );

				} catch {

					Debug.LogError( "Level contains tile with duplicate key: " + key );

				}

			}

			// Do the same for our TileObjects
			TileObject[] tileObjects = GameObject.FindObjectsOfType<TileObject>();
			foreach ( TileObject to in tileObjects ) {

				Vector3 position = to.transform.position;
				
				int xIndex = Mathf.RoundToInt( position.x / 2f );
				int zIndex = Mathf.RoundToInt( position.z / 2f );
				
				position.x = xIndex * 2;
				position.z = zIndex * 2;
				
				to.transform.position = position;
				string key = KeyFor( xIndex, zIndex );
				
				try {
					
					objectLookup.Add( key, to );
					
				} catch {
					
					Debug.LogError( "Level contains tile object with duplicate key: " + key );
					
				}

			}

			// Update each tile with their neighbors and their tileobject
			foreach( Tile t in tiles ) {

				// Told you it's dirty! We compute this again...
				Vector3 position = t.transform.position;
				int xIndex = Mathf.RoundToInt( position.x / 2f );
				int zIndex = Mathf.RoundToInt( position.z / 2f );

				// Get neighboring keys
				string current = KeyFor( xIndex, zIndex );
				string north = KeyFor( xIndex, zIndex + 1 );
				string south = KeyFor( xIndex, zIndex - 1 );
				string west = KeyFor( xIndex - 1, zIndex );
				string east = KeyFor( xIndex + 1, zIndex );

				// And some lazy code to assign the neighbors
				lookup.TryGetValue( north, out t.m_tileNorth );
				lookup.TryGetValue( south, out t.m_tileSouth );
				lookup.TryGetValue( west, out t.m_tileWest );
				lookup.TryGetValue( east, out t.m_tileEast );

				t.m_tileObject = null;

				// Do the same for the TileObject
				objectLookup.TryGetValue( current, out t.m_tileObject );

				EditorUtility.SetDirty( t );

			}

		}

	}
}