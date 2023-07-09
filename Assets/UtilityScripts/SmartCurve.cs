// made by Oliver Beebe 2023
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary> An animation curve wrapper.
/// <list type="bullet">
///     <item> Internal timer with start/stop methods. </item>
///     <item> Serialized fields for time/value scale. </item>
///     <item> Evaluate for any order derivative. </item>
/// </list>
/// </summary>
[System.Serializable]
public class SmartCurve {

    public AnimationCurve curve;
    /// <summary> Time to complete the curve will be scaled by this value. </summary>
    public float timeScale = 1;
    /// <summary> The evaluated values of the curve will be scaled by this value. </summary>
    public float valueScale = 1;

    public float timer = 0;

    /// <summary> Constructs a new SmartCurve with the provided curve and scaling values. </summary>
    /// <param name="curve"> The animation curve to be evaulated. </param>
    /// <param name="timeScale"> Time to complete the curve will be scaled by this value. </param>
    /// <param name="valueScale"> The evaluated values of the curve will be scaled by this value. </param>
    public SmartCurve(AnimationCurve curve, float timeScale, float valueScale) {
        this.curve      = curve;
        this.valueScale = valueScale;
        this.timeScale  = timeScale;
        timer = 0;
    }

    /// <summary> Constructs a new SmartCurve by copying the contents of the supplied SmartCurve. </summary>
    /// <param name="smartCurve"> The SmartCurve to copy. </param>
    public SmartCurve(SmartCurve smartCurve) {
        curve = new(smartCurve.curve.keys);
        valueScale = smartCurve.valueScale;
        timeScale = smartCurve.timeScale;
        timer = 0;
    }

    /// <summary> Increments the timer and evalutes the curve at that time. </summary>
    public float Evaluate() => Evaluate(Time.deltaTime, 0);
    /// <summary> Increments the timer and evalutes the curve at that time. </summary>
    /// <param name="derivative"> The order derivative to evaulate the curve with.
    /// <list type="table">
    ///     <item>
    ///         <term> 1 </term>
    ///         <description> Velocity </description>
    ///     </item>
    ///     <item>
    ///         <term> 2 </term>
    ///         <description> Acceleration </description>
    ///     </item>
    ///     <item>
    ///         <term> 3 </term>
    ///         <description> Jerk </description>
    ///     </item>
    ///     <item> etc. </item>
    /// </list>
    /// </param>
    public float Evaluate(int derivative) => Evaluate(Time.deltaTime, derivative);
    /// <summary> Increments the timer and evalutes the curve at that time. </summary>
    /// <param name="deltaTime"> The deltaTime to increment the timer by. </param>
    public float Evaluate(float deltaTime) => Evaluate(deltaTime, 0);
    /// <summary> Increments the timer and evalutes the curve at that time. </summary>
    /// <param name="deltaTime"> The deltaTime to increment the timer by. </param>
    /// <param name="derivative"> The order derivative to evaulate the curve with.
    /// <list type="table">
    ///     <item>
    ///         <term> 1 </term>
    ///         <description> Velocity </description>
    ///     </item>
    ///     <item>
    ///         <term> 2 </term>
    ///         <description> Acceleration </description>
    ///     </item>
    ///     <item>
    ///         <term> 3 </term>
    ///         <description> Jerk </description>
    ///     </item>
    ///     <item> etc. </item>
    /// </list>
    /// </param>
    public float Evaluate(float deltaTime, int derivative) {
        if (curve == null) return 0;
        timer += deltaTime / timeScale;
        return (derivative > 0 ? Derivative(derivative) / timeScale : curve.Evaluate(timer)) * valueScale;
    }

    /// <summary> Starts the curve's timer. </summary>
    public void Start() => timer = 0;
    /// <summary> Stops the curve's timer. </summary>
    public void Stop() => timer = Mathf.Infinity;
    /// <summary> Specifies whether curve's timer has finished. </summary>
    public bool Done => curve == null || timer > curve.keys[^1].time;

    private const float delta = 0.000001f;
    /// <summary> Evaluates the curve at the timer, with the specified derivative order. </summary>
    /// <param name="order"> The order derivative to evaulate the curve with.
    /// <list type="table">
    ///     <item>
    ///         <term> 1 </term>
    ///         <description> Velocity </description>
    ///     </item>
    ///     <item>
    ///         <term> 2 </term>
    ///         <description> Acceleration </description>
    ///     </item>
    ///     <item>
    ///         <term> 3 </term>
    ///         <description> Jerk </description>
    ///     </item>
    ///     <item> etc. </item>
    /// </list>
    /// </param>
    public float Derivative(int order) => Derivative(timer, order);
    /// <summary> Evaluates the curve at the provided time, with the specified derivative order. </summary>
    /// <param name="time"> The time at which to evaulate the curve (unscaled). </param>
    /// <param name="order"> The order derivative to evaulate the curve with.
    /// <list type="table">
    ///     <item>
    ///         <term> 1 </term>
    ///         <description> Velocity </description>
    ///     </item>
    ///     <item>
    ///         <term> 2 </term>
    ///         <description> Acceleration </description>
    ///     </item>
    ///     <item>
    ///         <term> 3 </term>
    ///         <description> Jerk </description>
    ///     </item>
    ///     <item> etc. </item>
    /// </list>
    /// </param>
    public float Derivative(float time, int order) {

        if (order < 1) return curve.Evaluate(time);

        float x1 = time - delta,
              x2 = time + delta,
              y1, y2;

        if (order - 1 > 0) {
            y1 = Derivative(x1, order - 1);
            y2 = Derivative(x2, order - 1);
        } else {
            y1 = curve.Evaluate(x1);
            y2 = curve.Evaluate(x2);
        }

        return (y2 - y1) / (2f * delta);
    }

    #region Editor
    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SmartCurve))]
    public class SmartCurvePropertyDrawer : PropertyDrawer {

        private static readonly System.Collections.Generic.Dictionary<SmartCurvePropertyDrawer, bool> foldouts = new();

        private bool foldoutActive {
            get {
                if (foldouts.TryGetValue(this, out bool active)) return active;
                foldouts.Add(this, false);
                return false;
            } set {
                if (foldouts.ContainsKey(this)) foldouts[this] = value;
                else foldouts.Add(this, value);
            }
        }
        private bool condensed => EditorGUIUtility.currentViewWidth < 332.1f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            float line = EditorGUIUtility.singleLineHeight,
                  spacing = EditorGUIUtility.standardVerticalSpacing,
                  indent = 15;
            Rect rect = new(position.min.x, position.min.y, position.size.x, line);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("curve"), label);

            rect.width /= 2;
            foldoutActive = EditorGUI.Foldout(rect, foldoutActive, GUIContent.none, true);

            if (foldoutActive) {

                rect.y += line + spacing;
                rect.width *= 2;

                GUIContent
                    timeLabel = new("Time Scale", "Time to complete the curve will be scaled by this value."),
                    valueLabel = new("Value Scale", "The evaluated values of the curve will be scaled by this value.");
                SerializedProperty
                    timeProperty = property.FindPropertyRelative("timeScale"),
                    valueProperty = property.FindPropertyRelative("valueScale");

                if (condensed) { // condensed mode

                    rect.width -= spacing + indent;
                    rect.width /= 2;

                    rect.x += indent;
                    EditorGUI.PrefixLabel(rect, timeLabel);

                    rect.x += rect.width + spacing;
                    EditorGUI.PropertyField(rect, timeProperty, GUIContent.none);

                    rect.y += line + spacing;
                    rect.x = position.min.x + indent;
                    EditorGUI.PrefixLabel(rect, valueLabel);

                    rect.x += rect.width + spacing;
                    EditorGUI.PropertyField(rect, valueProperty, GUIContent.none);

                } else { // long mode

                    rect.width -= spacing * 4 + indent;
                    rect.width /= 4;

                    rect.x += indent;
                    EditorGUI.PrefixLabel(rect, timeLabel);

                    rect.x += rect.width + spacing;
                    EditorGUI.PropertyField(rect, timeProperty, GUIContent.none);

                    rect.x += rect.width + spacing * 2;
                    EditorGUI.PrefixLabel(rect, valueLabel);

                    rect.x += rect.width + spacing;
                    EditorGUI.PropertyField(rect, valueProperty, GUIContent.none); 
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight        * (foldoutActive ? condensed ? 3 : 2 : 1)
             + EditorGUIUtility.standardVerticalSpacing * (foldoutActive ? condensed ? 2 : 1 : 0);
    }

    #endif
    #endregion
}
