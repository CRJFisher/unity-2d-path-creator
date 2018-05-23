using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathSectionsHolder
{
    readonly List<PathSection> pathSections;

    public PathSectionsHolder() {
        pathSections = new List<PathSection>();
    }

    public List<PathSection> PathSections
    {
        get
        {
            return pathSections;
        }
    }
}

