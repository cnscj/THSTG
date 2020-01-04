using FairyGUI;

namespace STGService.UI
{

    public class FRichText : FLabel
    {

        public void SetAutoSize(AutoSizeType type)
        {
            _obj.asRichTextField.autoSize = type;
        }


    }

}
