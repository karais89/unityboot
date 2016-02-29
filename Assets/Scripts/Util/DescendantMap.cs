using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class DescendantMap {
    Dictionary<string, List<Transform>> map = new Dictionary<string, List<Transform>>();
    public GameObject root { get; private set; }

    public DescendantMap(MonoBehaviour mono) {
        root = mono.gameObject;
        Initialize(mono.gameObject);
    }

    public DescendantMap(GameObject gameObject) {
        root = gameObject;
        Initialize(gameObject);
    }

    public T Get<T>(string name) where T : Component {
        if (map.ContainsKey(name) == false) {
            return null;
        }

        List<Transform> transList = map[name];
        if (transList.Count == 0) {
            return null;
        }

        return transList[0].GetComponent<T>();
    }

    public List<T> GetAll<T>(string name) where T : Component {
        if (map.ContainsKey(name) == false) {
            return null;
        }

        List<Transform> transList = map[name];
        if (transList.Count == 0) {
            return null;
        }

        List<T> res = new List<T>();
        foreach (Transform trans in transList) {
            res.Add(trans.GetComponent<T>());
        }

        return res;
    }

    public GameObject Get(string name){
        Transform res = Get<Transform>(name);
        if (res != null) {
            return res.gameObject;
        }

        return null;
    }

    public List<GameObject> GetAll(string name){
        List<Transform> transforms = GetAll<Transform>(name);
        List<GameObject> res = new List<GameObject>();
        foreach (Transform trans in transforms) {
            res.Add(trans.gameObject);
        }

        return res;
    }

    public List<T> QueryAll<T>(string query) where T : Component {
        char[] columnDelimiters = {' '};
        string[] token = query.Split(columnDelimiters, StringSplitOptions.RemoveEmptyEntries);

        if (token.Length == 0) {
            return null;
        }

        if (token.Length == 1) {
            return GetAll<T>(query);
        }

        bool findParent = false;
        List<T> candidates = GetAll<T>(token[token.Length-1]);
        List<T> result = null;
        Dictionary<Transform, Transform> cursors = new Dictionary<Transform, Transform>();

        for (int index = token.Length-2; index >= 0; index--) {
            string name = token[index];
            result = new List<T>();

            if (name == ">" && findParent == false) {
                findParent = true;
                continue;
            }

            foreach (T candidate in candidates) {
                Transform parent = null;
                Transform from = candidate.transform;
                if (cursors.ContainsKey(from) == true) {
                    from = cursors[from];
                }

                if (findParent) {
                    parent = GetParent(from, name);
                }
                else {
                    parent = GetAncestor(from, name);
                }

                if (parent != null) {
                    result.Add(candidate);
                    cursors[candidate.transform] = parent;
                }
            }

            candidates = result;
            findParent = false;
            if (result.Count == 0) {
                break;
            }
        }

        return result;
    }

    public T Query<T>(string query) where T : Component {
        List<T> result = QueryAll<T>(query);
        if (result.Count > 0) {
            return result[0];
        }

        return null;
    }

    public GameObject Query(string query) {
        Transform result = Query<Transform>(query);
        if (result != null) {
            return result.gameObject;
        }
        return null;
    }

    public List<GameObject> QueryAll(string query) {
        List<Transform> transforms = QueryAll<Transform>(query);
        List<GameObject> res = new List<GameObject>();
        foreach (Transform trans in transforms) {
            res.Add(trans.gameObject);
        }

        return res;
    }

    void Initialize(GameObject gameObject) {
        Queue<Transform> list = new Queue<Transform>();

        for (int i=0; i<gameObject.transform.childCount; i++) {
            list.Enqueue(gameObject.transform.GetChild(i));
        }
        
        while (list.Count > 0) {
            Transform trans = list.Dequeue();
            string key = trans.gameObject.name;

            if (map.ContainsKey(key) == false) {
                map.Add(key, new List<Transform>());
            }

            map[key].Add(trans);

            for (int j=0;j<trans.childCount;j++) {
                list.Enqueue(trans.GetChild(j));
            }
        }
    }

    Transform GetParent(Transform trans, string name) {
        Transform parent = trans.parent;
        if (parent != null && parent.gameObject.name == name) {
            return parent;
        }

        return null;
    }

    Transform GetAncestor(Transform trans, string name) {
        trans = trans.parent;
        while (trans != null) {
            if (trans.gameObject.name == name) {
                return trans;
            }
            trans = trans.parent;
        }

        return null;
    }
}