using UnityEngine;
using System.Collections;

public class SharedData : MonoBehaviour {

	int myScore = 0;
	int theirScore = 0;


	public void registerWin(){
		myScore += 1;
	}
	public void registerLose(){
		theirScore += 1;
	}
	public int getMyScore(){
		return myScore;
	}
	public int getTheirScore(){
		return theirScore;
	}

	[RPC] void lose(){
		registerLose ();
	}
	[RPC] void win(){
		registerWin ();
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
