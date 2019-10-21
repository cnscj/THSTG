using UnityEngine;
using System.Collections.Generic;

namespace STGGame.UI
{

    public class FLabel : FComponent
    {

        public new string GetText()
        {
            return _obj.asTextField.text;
        }

        public new string SetText(string value)
        {
            return _obj.asTextField.text = value;
        }

        public void SetColor(Color color)
        {
            _obj.asTextField.color = color;
        }
        public Color GetClolor()
        {
            return _obj.asTextField.color;
        }

        public void SetFont(string font)
        {
            var textFormat = _obj.asTextField.textFormat;
            textFormat.font = font;
        }
        public void SetFontSize(int size)
        {
            var textFormat = _obj.asTextField.textFormat;
            textFormat.size = size;
        }

        public int GetFontSize()
        {
            var textFormat = _obj.asTextField.textFormat;
            return textFormat.size;
        }

        public void SetTemplateVars(Dictionary<string,string> values)
        {
            _obj.asTextField.templateVars = values;
        }

        public void SetVar(string key, string value)
        {
            _obj.asTextField.SetVar(key, value);
        }

        public void SetLineSpacing(int lineSpacing)
        {
            var textFormat = _obj.asTextField.textFormat;
            textFormat.lineSpacing = lineSpacing;
        }


    }

}
