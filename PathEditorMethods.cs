using UnityEngine;
using System.Collections;

static public class PathEditorMethods
{

    public static System.String GetString(this Path.PathType path)
    {
        switch (path)
        {
            case Path.PathType.ARC:
                return "Yeah!";
            case Path.PathType.BEZIER:
                return "Okay!";
            default:
                return "What?!";
        }
    }

}
