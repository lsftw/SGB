using UnityEngine;
using System.Collections;

public class SharedData : MonoBehaviour {

	int myScore = 0;
	int theirScore = 0;


	public void win(){
		myScore += 1;
	}
	public void lose(){
		theirScore += 1;
	}
	public int getMyScore(){
		return myScore;
	}
	public int getTheirScore(){
		return theirScore;
	}

	[RPC] public void registerLose(){
		Debug.Log ("lose RPC, myscore=" + myScore);
		lose ();
	}
	[RPC] public void registerWin(){
		Debug.Log ("win RPC, myscore=" + myScore);
		win ();
	}

	/*
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Debug.Log("writing shared myScore = " + myScore);
			stream.Serialize(ref myScore);
		}
		else
		{

			stream.Serialize(ref theirScore);
			Debug.Log("receiving shared theirScore = "+theirScore);
		}
	}*/
}
