using FairyGUI;

namespace STGGame.UI
{

    public class FController
    {
        protected Controller _obj;
        public FController(Controller ctrl)
        {
            _obj = ctrl;
        }

        public Controller GetObject()
        {
            return _obj;
        }

        public void SetSelectedIndex(int index)
        {
            _obj.selectedIndex = index;
        }

        public int GetSelectedIndex()
        {
            return _obj.selectedIndex;
        }

        public int GetPreviousIndex()
        {
            return _obj.previsousIndex;
        }

        public void SetSelectedName(string name)
        {
            _obj.selectedPage = name;
        }

        public string GetSelectedName()
        {
            var id = GetSelectedIndex();
            return _obj.GetPageName(id);
         }

        //获取页面数量
        public int GetPageCount()
        {
            return _obj.pageCount;
        }

        public void AddPage(string name)
        {
            _obj.AddPage(name);
        }

        public void OnChanged(EventCallback1 func)
        {
            _obj.onChanged.Add(func);
        }

        public void ClearPages()
        {
            _obj.ClearPages();
        }
    }

}
