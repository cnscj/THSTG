/*************************
 * 
 * 其他一些混杂的工具类
 * 
 * 
 **************************/
using System.Collections.Generic;
using UnityEngine;
namespace XLibrary
{
    public static class XMiscTools
    {
        /// <summary>
        /// 把内容弄到剪贴板
        /// </summary>
        /// <param name="input"></param>
        public static void CopyToClipboard(string input)
        {
#if UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.text = input;
            t.OnFocus();
            t.Copy();
#elif UNITY_IPHONE
        CopyTextToClipboard_iOS(input);  
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass tool = new AndroidJavaClass("com.my.ugcf.Tool");
        tool.CallStatic("CopyTextToClipboard", currentActivity, input);
#endif
        }
    }
}