using UnityEngine;
using System.Collections;

// Shares game data such as score
public class SharedData : MonoBehaviour {

	int myScore = 0;
	int theirScore = 0;

	//maybe try
// http://answers.unity3d.com/questions/625411/view-pvp-scores-on-screen-in-real-time.html

	public int getMyScore(){
		return myScore;
	}
	public int getTheirScore(){
		return theirScore;
	}

	public void win(){
		myScore += 1;
	}
	public void lose(){
		theirScore += 1;
	}
	[RPC] public void registerLose(){
		Debug.Log ("lose RPC, myscore=" + myScore);
		lose ();
	}
	[RPC] public void registerWin(){
		Debug.Log ("win RPC, myscore=" + myScore);
		win ();
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			stream.Serialize(ref theirScore);
			Debug.Log("writing shared theirScore = "+theirScore);
		} else {
			Debug.Log("receiving shared myScore = " + myScore);
			stream.Serialize(ref myScore);
		}
	}
}
