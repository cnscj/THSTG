
namespace THGame
{
    public interface ISkillTriggerFactory
    {
        /// <summary>
        /// 取得触发器类型
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// 生成触发器
        /// </summary>
        /// <returns></returns>
        AbstractSkillTrigger CreateTrigger();

        /// <summary>
        /// 回收触发器
        /// </summary>
        /// <param name="instance"></param>
        void RecycleTrigger(AbstractSkillTrigger instance);
    }
}
