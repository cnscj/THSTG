﻿
namespace STGRuntime.UI
{

    public class FImage : FComponent
    {
        public void SetPrecent(float value)
        {
            _obj.asImage.fillAmount = value;
        }
    }

}