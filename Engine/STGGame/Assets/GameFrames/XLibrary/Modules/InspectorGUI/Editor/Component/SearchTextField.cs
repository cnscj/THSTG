using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace XLibEditor
{
    public class SearchTextField : GUIComponent
    {
        private GUIStyle TextFieldRoundEdge;
        private GUIStyle TextFieldRoundEdgeCancelButton;
        private GUIStyle TextFieldRoundEdgeCancelButtonEmpty;
        private GUIStyle TransparentTextField;
        private string m_InputSearchText;
        /// <summary>
        /// 绘制输入框，放在OnGUI函数里
        /// </summary>
        private void DrawInputTextField()
        {
            if (TextFieldRoundEdge == null)
            {
                TextFieldRoundEdge = new GUIStyle("SearchTextField");
                TextFieldRoundEdgeCancelButton = new GUIStyle("SearchCancelButton");
                TextFieldRoundEdgeCancelButtonEmpty = new GUIStyle("SearchCancelButtonEmpty");
                TransparentTextField = new GUIStyle(EditorStyles.whiteLabel);
                TransparentTextField.normal.textColor = EditorStyles.textField.normal.textColor;
            }

            //获取当前输入框的Rect(位置大小)
            Rect position = EditorGUILayout.GetControlRect();
            //设置圆角style的GUIStyle
            GUIStyle textFieldRoundEdge = TextFieldRoundEdge;
            //设置输入框的GUIStyle为透明，所以看到的“输入框”是TextFieldRoundEdge的风格
            GUIStyle transparentTextField = TransparentTextField;
            //选择取消按钮(x)的GUIStyle
            GUIStyle gUIStyle = (m_InputSearchText != "") ? TextFieldRoundEdgeCancelButton : TextFieldRoundEdgeCancelButtonEmpty;

            //输入框的水平位置向左移动取消按钮宽度的距离
            position.width -= gUIStyle.fixedWidth;
            //放大镜的大小
            float magnifierWidth = textFieldRoundEdge.CalcSize(new GUIContent("")).x;
            Rect rect = position;
            rect.width = magnifierWidth;
            if ((Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)))
            {
                //当放大镜被点击
                Debug.LogError("放大镜被点击");
            }

            //如果面板重绘
            if (Event.current.type == EventType.Repaint)
            {
                //根据是否是专业版来选取颜色
                GUI.contentColor = (EditorGUIUtility.isProSkin ? new Color(0.706f, 0.706f, 0.706f, 0.5f) : new Color(0f, 0f, 0f, 0.5f));
                //当没有输入的时候提示“请输入”
                if (string.IsNullOrEmpty(m_InputSearchText))
                {
                    textFieldRoundEdge.Draw(position, new GUIContent("请输入"), 0);
                }
                else
                {
                    textFieldRoundEdge.Draw(position, new GUIContent(""), 0);
                }
                //因为是“全局变量”，用完要重置回来
                GUI.contentColor = Color.white;
            }
            rect = position;
            //为了空出左边那个放大镜的位置
            float num = textFieldRoundEdge.CalcSize(new GUIContent("")).x - 2f;
            rect.width -= num;
            rect.x += num;
            rect.y += 1f;//为了和后面的style对其

            m_InputSearchText = EditorGUI.TextField(rect, m_InputSearchText, transparentTextField);
            //绘制取消按钮，位置要在输入框右边
            position.x += position.width;
            position.width = gUIStyle.fixedWidth;
            position.height = gUIStyle.fixedHeight;
            if (GUI.Button(position, GUIContent.none, gUIStyle) && m_InputSearchText != "")
            {
                m_InputSearchText = "";
                //用户是否做了输入
                GUI.changed = true;
                //把焦点移开输入框
                GUIUtility.keyboardControl = 0;
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();
            DrawInputTextField();
        }
    }
}