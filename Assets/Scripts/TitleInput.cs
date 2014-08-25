using UnityEngine;
using System.Collections;

public class TitleInput : MonoBehaviour {
	
	void Update () {

		if ( Input.GetButtonDown( "start" ) ) {

			LevelManager.LoadLevel( 1 );

		}
	}
}
