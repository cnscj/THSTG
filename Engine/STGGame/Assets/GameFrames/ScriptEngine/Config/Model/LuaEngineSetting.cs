
using UnityEngine;
namespace SEGame
{
    public class LuaEngineSetting : ScriptableObject
    {
        public string default_doString = "require 'Main'";                //执行代码

        public string luaBundlePath = "";                                 //ab文件(资源编辑器用
        public string scriptSPath = "../../Game/Script/Client";           //首包脚本路径
   
        public string scriptUPath = "";                                   //更新脚本路径
        public string trunkRoot = "";                                     //主干目录
        public string branchRoot = "";                                    //更新目录
    }

}
