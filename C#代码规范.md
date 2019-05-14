## C#代码规范

* 私有/保护非静态变量

  ```c#
  private string m_varName;
  protected bool m_isRunning;
  ```

* 私有/保护静态变量

  ```c#
  private static Object s_xxxXxx;
  ```

* 公共类及成员变量

  ```c#
  //成员变量驼峰式
  public int varName;
  //类可读写变量驼峰式
  public static int varName;
  //类只读变量全大写
  public static readonly int VAR_NAME = 100;
  public const int VAR_NAME = 100;
  ```

* 方法名大写开头

  ```c#
  private void DoSomething()
  {
      //TODO
  }
  
  public static Object GetObject()
  {
      return null;
  }
  ```

* 类名大写开头XxxXxxXxx

* 最外层用THGame作为命名空间，扩展编辑器的代码用THEditor作为命名空间

* 大括号另起一行写（新建数组除外）

* 方法之间空一行

* 哪怕代码只有一行，if，for都要带大括号{}

* 格式化代码（=前后空一格，逗号后空一格，缩进等等。。。）

* 单例采用get属性写法来实现

  ```c#
  private static Xxx s_xxx;
  
  public static Xxx instance
  {
      get
      {
      	return s_xxx;
      }
  }
  ```

* 成员变量名称较长时例如m_skinMeshRendererList，不要缩写成只有一个字母m_smrList，上下文没有其他renderer时可简写成m_rendererList

* 变量统一置于类的最上面或者最下面

* 如果存在某些成员需要被特别关注时（公有的、静态的、常量的、只读的等。。）建议统一最上面写，否则建议统一最下面写

* 不能出现警告（建议使用vs2017工具）

* 对修饰符排序，例如public static，不能写成static public，这也是vs2017的警告之一

* 枚举名使用大写E+名字首字母大写开头，如EXxxXxx，枚举项使用大驼峰式

  ```c#
  enum EXxxXxx
  {
  	AaaBbb,
  	Bbb,
  	Ccc
  }
  ```

* 