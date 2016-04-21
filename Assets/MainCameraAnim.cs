using UnityEngine;
using System.Collections;

public class MainCameraAnim : MonoBehaviour {
    private static MainCameraAnim _instance = null;
    public static MainCameraAnim instance {
        get {
            return _instance;
        }
    }


}
