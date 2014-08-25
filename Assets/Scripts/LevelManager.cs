using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	public GameObject m_prefabPlayer = null;
	public Tile m_startingTile = null;

	private Player m_player = null;
	private InputManager m_inputManager = null;

	public static int m_currentLevel = -1;
	public static int m_maxLevel = 1;

	private const int MAX_LEVEL = 12;

	public static void ResetLevel() {

		Debug.Log( "Current Level " + m_currentLevel );

		if ( m_currentLevel > 0 ) {

			Application.LoadLevel( Application.loadedLevelName );

		}
	}

	public static void LoadLevel ( int p_level ) {

		if ( p_level < 1 ) {

			m_currentLevel = -1;
			Application.LoadLevel( "title" );
			return;

		}

		if ( p_level <= MAX_LEVEL ) {

			m_currentLevel = p_level;
			m_maxLevel = Mathf.Max( m_currentLevel, m_maxLevel );
			Application.LoadLevel( string.Format( "lvl_{0}", p_level ) );
			return;

		}

		Debug.LogError( "Did not find level" );

	}

	public static void LoadNextLevel () {

		m_currentLevel++;
		m_maxLevel = Mathf.Max( m_currentLevel, m_maxLevel );

		if ( m_currentLevel > MAX_LEVEL ) {

			Application.LoadLevel( "win" );
			return;
		}

		LoadLevel( m_currentLevel );

	}

	// Use this for initialization
	void Awake () {

		if ( m_startingTile == null ) {

			Debug.LogError( "Unable to start level. Did not set starting tile." );
			return;

		}

		if ( m_prefabPlayer == null ) {

			Debug.LogError( "Unable to start level. Did not set player prefab." );
			return;

		}

		GameObject playerObject = GameObject.Instantiate( m_prefabPlayer ) as GameObject;
		m_player = playerObject.GetComponent<Player>();
		playerObject.name = "Player";

		m_player.SetCurrentTile( m_startingTile );


		GameObject inputObject = new GameObject();
		m_inputManager = inputObject.AddComponent<InputManager>();

		m_inputManager.SetPlayer( m_player );
		m_inputManager.transform.parent = this.transform;
		m_inputManager.name = "InputManager";

		m_player.transform.forward = Vector3.back;

		CameraFollow followCam = Camera.main.gameObject.AddComponent<CameraFollow>();
		followCam.SetTarget( m_player.gameObject );

	}


	#region Persistence functions - for later, maybe.
	/*
	#region Prefabs
	public GameObject m_objCrystal;

	public GameObject m_objBaseTile;
	public GameObject m_objWall;
	public GameObject m_objCorner;

	public GameObject m_objHuman;
	public GameObject m_objGhost;

	public GameObject m_objGoal;

	public GameObject m_objCrate;
	public GameObject m_objLever;
	public GameObject m_objTormentor;
	#endregion

	#region Instances
	//Required objects for game
	//private GameObject m_crystal = null;

	//Required objects per level
	//private GameObject m_human = null;
	//private GameObject m_ghost = null;
	//private GameObject m_goal = null;

	private List<GameObject> m_tiles = new List<GameObject>();

	//Optional objects with possible multiple instances
	private List<GameObject> m_crates = new List<GameObject>();
	private List<GameObject> m_levers = new List<GameObject>();
	private List<GameObject> m_tormentors = new List<GameObject>();
	//TODO walls
	//TODO corners
	#endregion
	
	#region Utils

	public void ResetLevel() {

		// Single-instance objects - required stuff
		if ( m_crystal == null ) { m_crystal = GameObject.Instantiate( m_objCrystal, Vector3.zero, Quaternion.identity ) as GameObject; }
		if ( m_human == null ) 	{ m_human = GameObject.Instantiate( m_objHuman, Vector3.zero, Quaternion.identity ) as GameObject; }
		if ( m_ghost == null ) 	{ m_ghost = GameObject.Instantiate( m_objGhost, Vector3.zero, Quaternion.identity ) as GameObject; }
		if ( m_goal == null ) 	{ m_goal = GameObject.Instantiate( m_objGoal, Vector3.zero, Quaternion.identity ) as GameObject; }

		// Multiple-instance objects - the rest
		if ( m_tiles == null ) 		{ m_tiles = new List<GameObject>(); }
		if ( m_crates == null ) 	{ m_crates = new List<GameObject>(); }
		if ( m_levers == null ) 	{ m_levers = new List<GameObject>(); }
		if ( m_tormentors == null ) { m_tormentors = new List<GameObject>(); }

		DestroyObjectsInList( m_tiles );
		DestroyObjectsInList( m_crates );
		DestroyObjectsInList( m_levers );
		DestroyObjectsInList( m_tormentors );

	}
	
	private static void DestroyObjectsInList( List<GameObject> p_list ) {
		while( p_list.Count > 0 ) {
			GameObject current = p_list[ 0 ];
#if UNITY_EDITOR
			DestroyImmediate( current );
#else
			Destroy ( current );
#endif
			p_list.RemoveAt( 0 );
		}
	}


	public static List<int> BuildPositionList( GameObject p_obj ) {
		return new List<int> { (int)p_obj.transform.position.x / TILE_SIZE, (int)p_obj.transform.position.z / TILE_SIZE };
	}

	public static Dictionary<string, object> BuildBaseDictionary( GameObject p_obj ) {

		Dictionary<string, object> dataDictionary = new Dictionary<string, object>();

		dataDictionary[ "position" ] = BuildPositionList( p_obj );
		dataDictionary[ "rotation" ] = p_obj.transform.eulerAngles.y;

		return dataDictionary;

	}

	public static void SetPositionFromList( GameObject p_obj, List<int> p_positionList ) {

		p_obj.transform.position = new Vector3( p_positionList[ 0 ], 0, p_positionList[ 1 ] );

	}

	public static void SetTransformFromDictionary( GameObject p_obj, Dictionary<string, object> p_dict ) {

		List<int> positionList = p_dict[ "position" ] as List<int>;
		float rotation = (float)p_dict[ "rotation" ];

		SetPositionFromList( p_obj, positionList );
		p_obj.transform.rotation = Quaternion.Euler( 0, rotation, 0 );

	}
	#endregion
	
	public string Serialize() {

		Dictionary<string, object> dataDictionary = new Dictionary<string, object>();

		if ( m_human != null ) {
			dataDictionary[ "human" ] = BuildBaseDictionary( m_human );
		} else {
			Debug.LogError( "Level data incomplete - no human" );
		}

		if ( m_ghost != null ) {
			dataDictionary[ "ghost" ] = BuildBaseDictionary( m_ghost );
		} else {
			Debug.LogError( "Level data incomplete - no ghost" );
		}

		if ( m_goal != null ) {
			dataDictionary[ "goal" ] = BuildBaseDictionary( m_goal );
		} else {
			Debug.LogError( "Level data incomplete - no goal" );
		}


		List<object> tileData = new List<object>();
		if ( m_tiles != null ) {
			foreach( GameObject tile in m_tiles ) {
				tileData.Add( BuildPositionList( tile ) );
			}
		}
		dataDictionary[ "tiles" ] = tileData;

		List<object> crateData = new List<object>();
		if ( m_crates != null ) {
			foreach( GameObject crate in m_crates ) {
				crateData.Add( BuildPositionList( crate ) );
			}
		}
		dataDictionary[ "crates" ] = crateData;

		List<object> leverData = new List<object>();
		if ( m_levers != null ) {
			foreach( GameObject lever in m_levers ) {
				leverData.Add( BuildPositionList( lever ) );
			}
		}
		dataDictionary[ "levers" ] = leverData;

		List<object> tormentorData = new List<object>();
		if ( m_tormentors != null ) {
			foreach( GameObject tormentor in m_tormentors ) {
				tormentorData.Add( BuildBaseDictionary( tormentor ) );
			}
		}
		dataDictionary[ "tormentors" ] = tormentorData;

		return MiniJSON.Json.Serialize( dataDictionary );

	}

	public void Deserialize( string p_json ) {

		ResetLevel();

		Dictionary<string, object> dict = MiniJSON.Json.Deserialize( p_json ) as Dictionary<string, object>;
		if ( dict == null ) {
			return;
		}

		List<int> posData;
		Dictionary<string, object> baseData;
		List<object> collectionData;

		posData = dict[ "human" ] as List<int>;
		if( posData != null ) {
			SetPositionFromList( m_human, posData );
		}

		posData = dict[ "ghost" ] as List<int>;
		if ( posData != null ) {
			SetPositionFromList( m_ghost, posData );
		}

		posData = dict[ "goal" ] as List<int>;
		if ( posData != null ) {
			SetPositionFromList( m_goal, posData );
		}

		collectionData = dict[ "tiles" ] as List<object>;
		foreach( object obj in collectionData ) {
			posData = obj as List<int>;
			if ( posData != null ) {
				GameObject go = GameObject.Instantiate( m_objBaseTile ) as GameObject;
				SetPositionFromList( go, posData );
				m_tiles.Add( go );
			}
		}

		collectionData = dict[ "crates" ] as List<object>;
		foreach( object obj in collectionData ) {
			posData = obj as List<int>;
			if ( posData != null ) {
				GameObject go = GameObject.Instantiate( m_objCrate ) as GameObject;
				SetPositionFromList( go, posData );
				m_crates.Add ( go );
			}
		}

		collectionData = dict[ "levers" ] as List<object>;
		foreach( object obj in collectionData ) {
			posData = obj as List<int>;
			if ( posData != null ) {
				GameObject go = GameObject.Instantiate( m_objLever ) as GameObject;
				SetPositionFromList( go, posData );
				m_levers.Add ( go );
			}
		}

		collectionData = dict[ "tormentors" ] as List<object>;
		foreach( object obj in collectionData ) {
			baseData = obj as Dictionary<string, object>;
			if ( baseData != null ) {
				GameObject go = GameObject.Instantiate( m_objTormentor ) as GameObject;
				SetTransformFromDictionary( go, baseData );
				m_tormentors.Add( go );
			}
		}
	}

	public void Save( string p_filename ) {
	}

	public void Load( string p_filename ) {
	}
	*/
	#endregion
}
