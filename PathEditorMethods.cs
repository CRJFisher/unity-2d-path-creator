using UnityEngine;
using System.Collections;

static public class PathEditorMethods
{

    public static System.String GetString(this PathSection.PathType path)
    {
        switch (path)
        {
            case PathSection.PathType.ARC:
                return "Yeah!";
            case PathSection.PathType.BEZIER:
                return "Okay!";
            default:
                return "What?!";
        }
    }

}
