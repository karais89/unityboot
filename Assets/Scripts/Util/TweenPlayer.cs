using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweenPlayer {
    Dictionary<string, TweenSet> scenes = new Dictionary<string, TweenSet>();

    public bool Load(MonoBehaviour mono, string name) {
        DescendantMap descendant = new DescendantMap(mono);
        string text = PersistenceUtil.LoadTextResource("Tween/" + name);
        CsvParser csv = new CsvParser();
        csv.Parse(text, " ,():;");
        TweenSet tweenSet = null;

        for (int index=0; index<csv.Count; index++) {
            CsvRow row = csv.GetRow(index);
            string first = row.AsString(0);
            if (first[0] == '@') {
                tweenSet = new TweenSet();
                scenes.Add(row.AsString(1), tweenSet);
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

            if (tween.Parse(descendant, row) == false) {
                Debug.LogError("can not load tween:" + first);
                return false;
            }

            tweenSet.Add(tween);
        }

        return true;
    }

    public IEnumerator Play(MonoBehaviour mono, string scene) {
        if (scenes.ContainsKey(scene) == false) {
            Debug.LogError("no tween scene to play:" + scene);
            yield break;
        }

        var tweenSet = scenes[scene];
        foreach (TweenBase tween in tweenSet.tweens) {
            Coroutine routine = mono.StartCoroutine(tween.Play(mono));
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
        MoveTo,
        ScaleTo,
        RotationTo,
        WaitSec,
        Active,
#if BOOT_NGUI_SUPPORT
        Alpha,
        Color,
        AlphaTo,
        ColorTo,
#endif
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

        public virtual bool Parse(DescendantMap descendant, CsvRow row) { return false; }
        public virtual IEnumerator Play(MonoBehaviour mono) { yield break; }
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
        protected GameObject target = null;

        public override bool Parse(DescendantMap descendant, CsvRow row) {
            target = descendant.Get(row.NextString());
            to.x = row.NextFloat();
            to.y = row.NextFloat();
            to.z = row.NextFloat();
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
            if (target != null) {
                target.transform.localPosition = to;
            }
            yield break;
        }
    }

    // Scale
    //-------------------------------------------------------------------------
    class TweenScale : TweenMove {
        public override IEnumerator Play(MonoBehaviour mono) {
            if (target != null) {
                target.transform.localScale = to;
            }
            yield break;
        }
    }

    // Rotation
    //-------------------------------------------------------------------------
    class TweenRotation : TweenMove {
        public override IEnumerator Play(MonoBehaviour mono) {
            if (target != null) {
                target.transform.localEulerAngles = to;
            }
            yield break;
        }
    }

    // MoveTo
    //-------------------------------------------------------------------------
    class TweenMoveTo : TweenBase {
        protected Vector3 to = Vector3.zero;
        protected GameObject target = null;

        public override bool Parse(DescendantMap descendant, CsvRow row) {
            target = descendant.Get(row.NextString());
            easeType = row.NextEnum<EaseType>();
            duration = row.NextFloat();
            to.x = row.NextFloat();
            to.y = row.NextFloat();
            to.z = row.NextFloat();
            ParseYield(row);
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
            if (target != null) {
                yield return mono.StartCoroutine(target.MoveTo(easeType, duration, to));
            }
        }
    }

    // ScaleTo
    //-------------------------------------------------------------------------
    class TweenScaleTo : TweenMoveTo {
        public override IEnumerator Play(MonoBehaviour mono) {
            if (target != null) {
                yield return mono.StartCoroutine(target.ScaleTo(easeType, duration, to));
            }
        }
    }

    // RotationTo
    //-------------------------------------------------------------------------
    class TweenRotationTo : TweenMoveTo {
        public override IEnumerator Play(MonoBehaviour mono) {
            if (target != null) {
                yield return mono.StartCoroutine(target.RotationTo(easeType, duration, to));
            }
        }
    }

    // WaitSec
    //-------------------------------------------------------------------------
    class TweenWaitSec : TweenBase {
        public override bool Parse(DescendantMap descendant, CsvRow row) {
            duration = row.NextFloat();
            wait = true;
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
            yield return new WaitForSeconds(duration);
        }
    }

    // Active
    //-------------------------------------------------------------------------
    class TweenActive : TweenBase {
        GameObject target = null;
        bool active = false;
        public override bool Parse(DescendantMap descendant, CsvRow row) {
            target = descendant.Get(row.NextString());
            active = row.NextBool();
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
            if (target != null) {
                target.SetActive(active);
            }

            yield break;
        }
    }

#if BOOT_NGUI_SUPPORT
    // Alpha
    //-------------------------------------------------------------------------
    class TweenAlpha : TweenBase {
        protected float to = 0;
        protected UIPanel panel = null;
        protected UIWidget widget = null;

        public override bool Parse(DescendantMap descendant, CsvRow row) {
            string target = row.NextString();
            panel = descendant.Get<UIPanel>(target);
            widget = descendant.Get<UIWidget>(target);
            to = row.NextFloat();
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
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
        protected UIWidget widget = null;

        public override bool Parse(DescendantMap descendant, CsvRow row) {
            string target = row.NextString();
            widget = descendant.Get<UIWidget>(target);
            to = to.HexToColor(row.NextString());
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
            if (widget != null) {
                widget.color = to;
                #if UNITY_EDITOR
                NGUITools.SetDirty(widget);
                #endif
            }
            yield break;
        }
    }

    // AlphaTo
    //-------------------------------------------------------------------------
    class TweenAlphaTo : TweenBase {
        protected float to = 0;
        protected UIPanel panel = null;
        protected UIWidget widget = null;

        public override bool Parse(DescendantMap descendant, CsvRow row) {
            string target = row.NextString();
            panel = descendant.Get<UIPanel>(target);
            widget = descendant.Get<UIWidget>(target);
            easeType = row.NextEnum<EaseType>();
            duration = row.NextFloat();
            to = row.NextFloat();
            ParseYield(row);
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
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
        protected UIWidget widget = null;

        public override bool Parse(DescendantMap descendant, CsvRow row) {
            string target = row.NextString();
            widget = descendant.Get<UIWidget>(target);
            easeType = row.NextEnum<EaseType>();
            duration = row.NextFloat();
            to = to.HexToColor(row.NextString());
            ParseYield(row);
            return true;
        }

        public override IEnumerator Play(MonoBehaviour mono) {
            if (widget != null) {
                yield return mono.StartCoroutine(widget.ColorTo(easeType, duration, to));
            }
        }
    }
#endif
}