﻿/*************************
 * 
 * 单例模板
 * 
 * 
 **************************/
namespace THGame
{
	public class XSingleton<T> where T : class, new()
	{
	    private static T _instance = null;

	    public static T Instance
	    {
	        get
	        {
	            if (_instance == null)
	            {
	                _instance = new T();
	            }
	            return _instance;
	        }
	    }
	}
}