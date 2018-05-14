using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathHolder
{

    public Path this[int i] {
        get {
            return paths[i];
        }
    }

    public void Add (Path p) {
        paths.Add(p);
    }
    
    public List<Path> paths = new List<Path>();

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}
}
