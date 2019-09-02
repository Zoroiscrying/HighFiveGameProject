using Game.Control.PersonSystem;

namespace Game.Control.SkillSystem
{
    public interface ISkillTrigger
    {
        float StartTime { get; set; }
        string SkillType { get; set; }
        float LastTime { get; set; }

        int id { get; set; }

        void LoadTxt(string args);
        //        void Init(string type,int id,float startTime,float lastTime,string args);
        void Release();
        void Execute(AbstractPerson self);
    }
}
