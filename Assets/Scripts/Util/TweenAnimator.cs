using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweenAnimator : MonoBehaviour {
    [SerializeField] TextAsset animations = null;

    Dictionary<string, TweenSet> clips = new Dictionary<string, TweenSet>();
    DescendantMap descendant = null;

    void Awake() {
        Load();
        if (clips.ContainsKey("Ready")) {
            StartCoroutine(Play("Ready"));
        }
    }

    bool Load() {
        if (animations == null) {
            return false;
        }

        descendant = new DescendantMap(this);
        string text = animations.text;
        CsvParser csv = new CsvParser();
        csv.Parse(text, " ,():;");
        TweenSet tweenSet = null;

        for (int index=0; index<csv.Count; index++) {
            CsvRow row = csv.GetRow(index);
            string first = row.AsString(0);
            if (first[0] == '@') {
                tweenSet = new TweenSet();
                clips.Add(row.AsString(1), tweenSet);
                continue;
            }

            TweenType type = row.NextEnum<TweenType>();
            TweenBase tween = null;

            switch(type) {
                case TweenType.Move: tween = new TweenMove(); break;
                case TweenType.Scale: tween = new TweenScale(); break;
                case TweenType.Rotation: tween = new TweenRotation(); break;
                case TweenType.MoveTo: tween = new TweenMoveTo(); break;
                case TweenType.ScaleTo: tween = new TweenScaleTo(); break;
                case TweenType.RotationTo: tween = new TweenRotationTo(); break;
                case TweenType.WaitSec: tween = new TweenWaitSec(); break;
                case TweenType.Active: tween = new TweenActive(); break;
#if BOOT_NGUI_SUPPORT
                case TweenType.Alpha: tween = new TweenAlpha(); break;
                case TweenType.Color: tween = new TweenColor(); break;
                case TweenType.AlphaTo: tween = new TweenAlphaTo(); break;
                case TweenType.ColorTo: tween = new TweenColorTo(); break;
#endif
            }

            if (tween.Parse(row) == false) {
                Debug.LogError("can not load tween:" + first);
                return false;
            }

            tweenSet.Add(tween);
        }

        return true;
    }

    public IEnumerator Play(string clip) {
        if (clips.ContainsKey(clip) == false) {
            Debug.LogError("no tween clip to play:" + clip);
            yield break;
        }

        var tweenSet = clips[clip];
        foreach (TweenBase tween in tweenSet.tweens) {
            Coroutine routine = StartCoroutine(tween.Play(this, descendant));
            if (tween.wait) {
                yield return routine;
            }
        }
    }


    // Tween Type
    //-------------------------------------------------------------------------
    public enum TweenType {
        Move,
        Scale,
        Rotation,
        Alpha,
        Color,
        MoveTo,
        ScaleTo,
        RotationTo,
        AlphaTo,
        ColorTo,
        WaitSec,
        Active,
    }

    // Tween Set
    //-------------------------------------------------------------------------
    class TweenSet {
        public List<TweenBase> tweens = new List<TweenBase>();

        public void Add(TweenBase tween) {
            tweens.Add(tween);
        }

        public int Size() {
            return tweens.Count;
        }
    }

    // Tween Base
    //-------------------------------------------------------------------------
    class TweenBase {
        public bool wait { get; set; }
        public EaseType easeType { get; set; }
        public float duration { get; set; }

        public virtual bool Parse(CsvRow row) { return false; }
        public virtual IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) { yield break; }
        protected void ParseYield(CsvRow row) {
            if (row.HasNext()) {
                wait = (row.NextString() == "yield");
            }
        }
    }

    // Move
    //-------------------------------------------------------------------------
    class TweenMove : TweenBase {
        protected Vector3 to = Vector3.zero;
        protected string target = "";

        public override bool Parse(CsvRow row) {
            target = row.NextString();
            to.x = row.NextFloat();
            to.y = row.NextFloat();
            to.z = row.NextFloat();
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            GameObject gameObject = descendant.Get(target);
            if (gameObject != null) {
                gameObject.transform.localPosition = to;
            }
            yield break;
        }
    }

    // Scale
    //-------------------------------------------------------------------------
    class TweenScale : TweenMove {
        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            GameObject gameObject = descendant.Get(target);
            if (gameObject != null) {
                gameObject.transform.localScale = to;
            }
            yield break;
        }
    }

    // Rotation
    //-------------------------------------------------------------------------
    class TweenRotation : TweenMove {
        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            GameObject gameObject = descendant.Get(target);
            if (gameObject != null) {
                gameObject.transform.localEulerAngles = to;
            }
            yield break;
        }
    }

#if BOOT_NGUI_SUPPORT
    // Alpha
    //-------------------------------------------------------------------------
    class TweenAlpha : TweenBase {
        protected float to = 0;
        protected string target = "";

        public override bool Parse(CsvRow row) {
            target = row.NextString();
            to = row.NextFloat();
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            UIPanel panel = descendant.Get<UIPanel>(target);
            UIWidget widget = descendant.Get<UIWidget>(target);

            if (panel != null) {
                panel.alpha = to;
                #if UNITY_EDITOR
                NGUITools.SetDirty(panel);
                #endif
            }
            else if (widget != null) {
                widget.alpha = to;
                #if UNITY_EDITOR
                NGUITools.SetDirty(widget);
                #endif
            }
            yield break;
        }
    }

    // Color
    //-------------------------------------------------------------------------
    class TweenColor : TweenBase {
        protected Color to = Color.white;
        protected string target = "";

        public override bool Parse(CsvRow row) {
            target = row.NextString();
            to = to.HexToColor(row.NextString());
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            UIWidget widget = descendant.Get<UIWidget>(target);
            if (widget != null) {
                widget.color = to;
                #if UNITY_EDITOR
                NGUITools.SetDirty(widget);
                #endif
            }
            yield break;
        }
    }
#endif

    // MoveTo
    //-------------------------------------------------------------------------
    class TweenMoveTo : TweenBase {
        protected Vector3 to = Vector3.zero;
        protected string target = null;

        public override bool Parse(CsvRow row) {
            target = row.NextString();
            easeType = row.NextEnum<EaseType>();
            duration = row.NextFloat();
            to.x = row.NextFloat();
            to.y = row.NextFloat();
            to.z = row.NextFloat();
            ParseYield(row);
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            GameObject gameObject = descendant.Get(target);
            if (gameObject != null) {
                yield return mono.StartCoroutine(gameObject.MoveTo(easeType, duration, to));
            }
        }
    }

    // ScaleTo
    //-------------------------------------------------------------------------
    class TweenScaleTo : TweenMoveTo {
        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            GameObject gameObject = descendant.Get(target);
            if (gameObject != null) {
                yield return mono.StartCoroutine(gameObject.ScaleTo(easeType, duration, to));
            }
        }
    }

    // RotationTo
    //-------------------------------------------------------------------------
    class TweenRotationTo : TweenMoveTo {
        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            GameObject gameObject = descendant.Get(target);
            if (gameObject != null) {
                yield return mono.StartCoroutine(gameObject.RotationTo(easeType, duration, to));
            }
        }
    }

#if BOOT_NGUI_SUPPORT
    // AlphaTo
    //-------------------------------------------------------------------------
    class TweenAlphaTo : TweenBase {
        protected float to = 0;
        protected string target = "";

        public override bool Parse(CsvRow row) {
            target = row.NextString();
            easeType = row.NextEnum<EaseType>();
            duration = row.NextFloat();
            to = row.NextFloat();
            ParseYield(row);
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            UIPanel panel = descendant.Get<UIPanel>(target);
            UIWidget widget = descendant.Get<UIWidget>(target);

            if (panel != null) {
                yield return mono.StartCoroutine(panel.AlphaTo(easeType, duration, to));
            }
            else if (widget != null) {
                yield return mono.StartCoroutine(widget.AlphaTo(easeType, duration, to));
            }
        }
    }

    // ColorTo
    //-------------------------------------------------------------------------
    class TweenColorTo : TweenBase {
        protected Color to = Color.white;
        protected string target = "";

        public override bool Parse(CsvRow row) {
            target = row.NextString();
            easeType = row.NextEnum<EaseType>();
            duration = row.NextFloat();
            to = to.HexToColor(row.NextString());
            ParseYield(row);
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            UIWidget widget = descendant.Get<UIWidget>(target);
            if (widget != null) {
                yield return mono.StartCoroutine(widget.ColorTo(easeType, duration, to));
            }
        }
    }
#endif

    // WaitSec
    //-------------------------------------------------------------------------
    class TweenWaitSec : TweenBase {
        public override bool Parse(CsvRow row) {
            duration = row.NextFloat();
            wait = true;
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            yield return new WaitForSeconds(duration);
        }
    }

    // Active
    //-------------------------------------------------------------------------
    class TweenActive : TweenBase {
        string target = "";
        bool active = false;
        public override bool Parse(CsvRow row) {
            target = row.NextString();
            active = row.NextBool();
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono, DescendantMap descendant) {
            GameObject gameObject = descendant.Get(target);
            if (gameObject != null) {
                gameObject.SetActive(active);
            }

            yield break;
        }
    }
}