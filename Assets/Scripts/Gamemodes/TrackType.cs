using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Track", menuName = "Track/TrackType", order = 1)]
public class TrackType : ScriptableObject {

	public int length;

	public GameObject Car;

	public GameObject[] Tiles;
}
