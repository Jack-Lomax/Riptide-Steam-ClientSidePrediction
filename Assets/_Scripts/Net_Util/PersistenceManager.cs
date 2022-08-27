using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(this);
	}
}
