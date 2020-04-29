using UnityEngine;

namespace Game.Scripts
{
    public class Ctrl : MonoBehaviour {
    
        public Animator ani;
        public string clipName;
        private void Start()
        {
            var c = ani.runtimeAnimatorController;
            var e = new AnimationEvent();
            e.functionName = "Destroy";
            e.time = 1-ani.speed/10;
            e.intParameter = 3;
            var clips = c.animationClips;
            foreach(var clip in clips)
            {
                if(clip.name== clipName)
                {
                    clip.AddEvent(e);
                }
            }
            ani.Rebind();
        }
        public void Destroy(int x)
        {
            GameObject.Destroy(this.gameObject);
        }
    }


}
