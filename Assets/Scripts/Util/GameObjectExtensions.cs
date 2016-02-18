using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameObjectExtensions {
    public static GameObject FirstDescendant(this MonoBehaviour v, string name) {
        return GetGameObject(v.gameObject, name);
    }

    public static GameObject FirstDescendant(this GameObject v, string name) {
        return GetGameObject(v, name);
    }

    private static GameObject GetGameObject(this GameObject v, string name) {
        Queue<Transform> list = new Queue<Transform>();

        for (int i=0; i<v.transform.childCount; i++) {
            list.Enqueue(v.transform.GetChild(i));
        }

        while (list.Count > 0) {
            Transform trans = list.Dequeue();

            if (trans.gameObject.name == name){
                return trans.gameObject;
            }
            else {
                for (int j=0;j<trans.childCount;j++) {
                    list.Enqueue(trans.GetChild(j));
                }
            }
        }

        return null;
    }

    public static T FirstDescendantComponent<T>(this MonoBehaviour v, string name) where T : Component {
        GameObject obj = v.FirstDescendant(name);
        if (obj == null) {
            return null;
        }
     
        return obj.GetComponent<T>();
    }

    public static T FirstDescendantComponent<T>(this GameObject v, string name) where T : Component {
        GameObject obj = v.FirstDescendant(name);
        if (obj == null) {
            return null;
        }

        return obj.GetComponent<T>();
    }

    public static T GetGameObjectComponent<T>(this MonoBehaviour v, string name) where T : Component {
        GameObject obj = GameObject.Find(name);
        if (obj == null) {
            return null;
        }
     
        return obj.GetComponent<T>();
    }

    public static T InstantiateUI<T>(this MonoBehaviour v, string path, GameObject parent) where T : Component {
        path = "Prefabs/UI/" + path;
        GameObject obj = MonoBehaviour.Instantiate(Resources.Load<GameObject>(path)) as GameObject;
        obj.transform.parent = parent.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        return obj.GetComponent<T>();
    }

    public static T Instantiate<T>(this MonoBehaviour v, string path, GameObject parent, bool findChilden) where T : Component {
        path = "Prefabs/" + path;
        GameObject obj = MonoBehaviour.Instantiate(Resources.Load<GameObject>(path)) as GameObject;
        obj.transform.parent = parent.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        if (findChilden) {
            return obj.GetComponentInChildren<T>();
        }
        return obj.GetComponent<T>();
    }

    public static void ClearChildren(this MonoBehaviour v) {
        for (int i=0; i<v.transform.childCount; i++) {
            UnityEngine.Object.Destroy(v.transform.GetChild(i).gameObject);
        }
    }    

    public static void ClearChildren(this GameObject v) {
        for (int i=0; i<v.transform.childCount; i++) {
            UnityEngine.Object.Destroy(v.transform.GetChild(i).gameObject);
        }
    }    

    public static Color HexToColor(this Color v, string hex) {
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r,g,b, 255);
    }

    public static bool HasTouch(this MonoBehaviour v) {
        if (Application.platform == RuntimePlatform.IPhonePlayer || 
            Application.platform == RuntimePlatform.Android) {
            return Input.touchCount >= 1;
        }

        return Input.GetMouseButton(0);
    }

    public static bool HasTouch1(this MonoBehaviour v) {
        if (Application.platform == RuntimePlatform.IPhonePlayer || 
            Application.platform == RuntimePlatform.Android) {
            return Input.touchCount == 1;
        }

        return Input.GetMouseButton(0);
    }

    public static bool HasTouch2(this MonoBehaviour v) {
        if (Application.platform == RuntimePlatform.IPhonePlayer || 
            Application.platform == RuntimePlatform.Android) {
            return Input.touchCount == 2;
        }

        return false;
    }

    public static Vector3 GetTouchPosition1(this MonoBehaviour v) {
        if (Application.platform == RuntimePlatform.IPhonePlayer || 
            Application.platform == RuntimePlatform.Android) {
            return Input.GetTouch(0).position;
        }

        return Input.mousePosition;
    }

#if BOOT_NGUI_SUPPORT
    public static Vector3 GetUITouchPosition1(this MonoBehaviour v) {
        UIRoot root = UIRoot.list[0];
        float height = root.manualHeight;
        float width = (Screen.width * height) / Screen.height;
        float scale = height / Screen.height;

        Vector3 pt = v.GetTouchPosition1() * scale;
        pt.x -= width * 0.5f;
        pt.y -= height * 0.5f;
        return pt;
    }    

    public static float GetUIWidth(this MonoBehaviour v) {
        UIRoot root = UIRoot.list[0];
        float height = root.manualHeight;
        return (Screen.width * height) / Screen.height;
    }

    public static void StopCoroutineSafe(this MonoBehaviour v, Coroutine coroutine) {
        if (coroutine != null) {
            v.StopCoroutine(coroutine);
        }
    }
#endif    
}