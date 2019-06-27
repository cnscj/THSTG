
using UnityEngine;
using UnityEditor;
using THGame;

namespace THEditor
{
    [CustomEditor(typeof(CameraShakerEditor))]
    public class CameraShakerEditorInspector : Editor
    {
        private static float s_scaleRate = 10f;
        private static string[] s_propertys = { "posX", "posY", "posZ", "angleX", "angleY", "angleZ" };
        private static string[] s_editModes = { "简单", "详细" };
        private static string[] s_simpleMode = { "角度", "位移" };
        private static int[] s_simpleIndex = { 2, 3, 5 };
        private static string[] s_simpleName = { "前后", "上下", "摇头" };

        private bool m_foldFinalCurve = false;
        private Transform m_relCamera;
        private CameraShakerEditor effectShakeCameraInfo;


        //详细
        private SerializedProperty[] m_curCurves = new SerializedProperty[6];

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            m_relCamera = EditorGUILayout.ObjectField("Camera", m_relCamera, typeof(Transform), true) as Transform;

            //属性序列化
            serializedObject.Update();
            m_foldFinalCurve = EditorGUILayout.Foldout(m_foldFinalCurve, "Curves");
            if (m_foldFinalCurve)
            {
                for (int i = 0; i < m_curCurves.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(m_curCurves[i]);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Space();
            effectShakeCameraInfo.selectIndex = EditorGUILayout.Popup("模式", effectShakeCameraInfo.selectIndex, s_editModes);
            if (effectShakeCameraInfo.selectIndex == 0)
            {
                //只取pos.z,rotation.x,rotation.z就好了,分别对应前后,上下,摇头
                for (int i = 0; i < s_simpleIndex.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.CurveField(s_simpleName[i], effectShakeCameraInfo.eachTimeLines[i]);
                    if (GUILayout.Button("对齐"))
                    {
                        Align(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                effectShakeCameraInfo.cycTime = EditorGUILayout.FloatField("周期", effectShakeCameraInfo.cycTime);
                effectShakeCameraInfo.cycCount = EditorGUILayout.IntField("次数", effectShakeCameraInfo.cycCount);
                effectShakeCameraInfo.isFixTime = EditorGUILayout.Toggle("振幅不减", effectShakeCameraInfo.isFixTime);
                //effectShakeCameraInfo.modeRidio = EditorGUILayout.Popup("Mode", effectShakeCameraInfo.modeRidio, s_simpleMode);
               
                if (GUILayout.Button("保存"))
                {
                    GenAnim1();
                }
            }
            else if (effectShakeCameraInfo.selectIndex == 1)
            {
                EditorGUILayout.CurveField("TimeLine", effectShakeCameraInfo.timeLine);
                effectShakeCameraInfo.isShowAllFrames = EditorGUILayout.Foldout(effectShakeCameraInfo.isShowAllFrames, string.Format("Frames({0})", effectShakeCameraInfo.timeLine.length - 1));
                if (effectShakeCameraInfo.isShowAllFrames)
                {
                    for (int i = 0; i < effectShakeCameraInfo.timeLine.length - 1; i++)
                    {
                        Keyframe frame = effectShakeCameraInfo.timeLine[i];
                        CameraShakerEditor.KeyExInfo keyExinfo = null;
                        if (i < effectShakeCameraInfo.keyExInfos.Count)
                        {
                            keyExinfo = effectShakeCameraInfo.keyExInfos[i];
                            if (System.Math.Abs(keyExinfo.keyTime - frame.time) > 0.00001f)
                            {
                                keyExinfo.keyTime = frame.time;
                                keyExinfo.shakePosition = Vector3.zero;
                                keyExinfo.shakeAngle = new Vector3(frame.value * s_scaleRate, 0, 0);
                                keyExinfo.cycleTime = 0.12f;
                                keyExinfo.cycleCount = 6;
                                keyExinfo.fixShake = false;
                            }

                            //振幅调整
                            if (i + 1 < effectShakeCameraInfo.timeLine.length)
                            {
                                float totalTime = effectShakeCameraInfo.timeLine[i + 1].time - effectShakeCameraInfo.timeLine[i].time;
                                if (keyExinfo.cycleTime * keyExinfo.cycleCount > totalTime)
                                {
                                    keyExinfo.cycleTime = totalTime / keyExinfo.cycleCount;
                                }
                            }
                            else if (i == effectShakeCameraInfo.timeLine.length - 1)
                            {
                                keyExinfo.cycleTime = 0;
                                keyExinfo.cycleCount = 0;
                            }
                        }
                        else
                        {
                            keyExinfo = new CameraShakerEditor.KeyExInfo();
                            keyExinfo.isShow = false;
                            keyExinfo.keyTime = frame.time;
                            keyExinfo.shakePosition = Vector3.zero;
                            keyExinfo.shakeAngle = new Vector3(frame.value * s_scaleRate, 0, 0);
                            keyExinfo.cycleTime = 0.12f;
                            keyExinfo.cycleCount = 6;
                            keyExinfo.fixShake = false;

                            effectShakeCameraInfo.keyExInfos.Add(keyExinfo);
                        }

                        keyExinfo.isShow = EditorGUILayout.Foldout(keyExinfo.isShow, string.Format("{0:F2}", keyExinfo.keyTime));
                        if (keyExinfo.isShow)
                        {
                            keyExinfo.shakePosition = EditorGUILayout.Vector3Field("位移", keyExinfo.shakePosition);
                            keyExinfo.shakeAngle = EditorGUILayout.Vector3Field("角度", keyExinfo.shakeAngle);
                            keyExinfo.cycleTime = EditorGUILayout.FloatField("周期", keyExinfo.cycleTime);
                            keyExinfo.cycleCount = EditorGUILayout.IntField("次数", keyExinfo.cycleCount);
                            keyExinfo.fixShake = EditorGUILayout.Toggle("振幅不减", keyExinfo.fixShake);
                            EditorGUILayout.Space();
                        }

                    }
                }


                if (GUILayout.Button("保存"))
                {
                    GenAnim2();
                }
            }

            if (GUILayout.Button("测试播放"))
            {
                Play();
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        void Align(int num)
        {
            var xFrames = effectShakeCameraInfo.eachTimeLines[num].keys;
            for (int j = 0; j < effectShakeCameraInfo.eachTimeLines.Length; j++)
            {
                if (j == num) continue;
                var curFrames = effectShakeCameraInfo.eachTimeLines[j].keys;
                int curLength = curFrames.Length;
                //移除多余的
                if (curLength > xFrames.Length)
                {
                    for (int k = xFrames.Length; k < curLength; k++)
                    {
                        effectShakeCameraInfo.eachTimeLines[j].RemoveKey(k);
                    }
                }
                for (int i = 0; i < xFrames.Length; i++)
                {
                    if (i < curLength)   //有就对齐
                    {
                        Keyframe frame = new Keyframe(xFrames[i].time, curFrames[i].value);
                        effectShakeCameraInfo.eachTimeLines[j].MoveKey(i, frame);
                    }
                    else //没有就创建
                    {
                        Keyframe frame = new Keyframe(xFrames[i].time, xFrames[i].value);
                        effectShakeCameraInfo.eachTimeLines[j].AddKey(frame);
                    }
                }
            }
        }
        void GenAnim1()
        {
            AnimationCurve[] eachCurves = new AnimationCurve[6];
            for (int u = 0; u < eachCurves.Length; u++)
            {
                if (effectShakeCameraInfo.modeRidio == 0)
                {
                    eachCurves[u] = (u < 3) ? AnimationCurve.Linear(0, 0, 0, 0) : new AnimationCurve();
                }
                else if (effectShakeCameraInfo.modeRidio == 1)
                {
                    eachCurves[u] = (u < 3) ? new AnimationCurve() : AnimationCurve.Linear(0, 0, 0, 0);
                }

            }

            for (int i = 0; i < effectShakeCameraInfo.eachTimeLines.Length; i++)
            {
                var timeLine = effectShakeCameraInfo.eachTimeLines[i];
                for (int j = 0; j < timeLine.keys.Length - 1; j++)
                {
                    var timeLineFrame = timeLine.keys[j];
                    float starTime = timeLineFrame.time;
                    float motionAmpl = timeLineFrame.value * s_scaleRate;
                    float cycTime = effectShakeCameraInfo.cycTime;
                    if (j + 1 < timeLine.keys.Length)
                    {
                        float totalTime = timeLine.keys[j + 1].time - starTime;
                        if (effectShakeCameraInfo.cycTime * effectShakeCameraInfo.cycCount > totalTime)
                        {
                            cycTime = totalTime / effectShakeCameraInfo.cycCount;
                        }
                    }
                    AnimationCurve calCurve = CameraShakerUtil.CreateSimpleHarmonicCurve(motionAmpl, cycTime, effectShakeCameraInfo.cycCount, effectShakeCameraInfo.isFixTime);

                    foreach (var frame in calCurve.keys)
                    {
                        Keyframe newFrame = new Keyframe(starTime + frame.time, frame.value);
                        eachCurves[s_simpleIndex[i]].AddKey(newFrame);
                    }
                }
            }
            for (int k = 0; k < eachCurves.Length; k++)
            {
                effectShakeCameraInfo.GetType().GetField(s_propertys[k]).SetValue(effectShakeCameraInfo, eachCurves[k]);
            }

        }

        void GenAnim2()
        {
            AnimationCurve[] eachCurves = new AnimationCurve[6];
            for (int u = 0; u < eachCurves.Length; u++)
            {
                eachCurves[u] = new AnimationCurve();
            }
            for (int i = 0; i < effectShakeCameraInfo.timeLine.length; i++)
            {
                Keyframe frame = effectShakeCameraInfo.timeLine[i];
                CameraShakerEditor.KeyExInfo keyExinfo;
                if (i < effectShakeCameraInfo.keyExInfos.Count)
                {
                    keyExinfo = effectShakeCameraInfo.keyExInfos[i];
                    var calCurves = CameraShakerUtil.CreateShakeCurve(keyExinfo.shakePosition, keyExinfo.shakeAngle, keyExinfo.cycleTime, keyExinfo.cycleCount, keyExinfo.fixShake);
                    for (int j = 0; j < 6; j++)
                    {
                        float startTime = keyExinfo.keyTime;
                        foreach (var calframe in calCurves[j].keys)
                        {
                            Keyframe newFrame = new Keyframe(startTime + calframe.time, calframe.value);
                            eachCurves[j].AddKey(newFrame);
                        }
                    }
                }
            }
            for (int k = 0; k < eachCurves.Length; k++)
            {
                effectShakeCameraInfo.GetType().GetField(s_propertys[k]).SetValue(effectShakeCameraInfo, eachCurves[k]);
            }

        }
        void Play()
        {
            Transform[] cameras = { m_relCamera };
            effectShakeCameraInfo.Shake(cameras);
        }

        void OnEnable()
        {
            effectShakeCameraInfo = (CameraShakerEditor)target;
            for (int i = 0; i < m_curCurves.Length; i++)
            {
                m_curCurves[i] = serializedObject.FindProperty(s_propertys[i]);
            }

            m_relCamera = Camera.main.transform;

        }

    }
}
