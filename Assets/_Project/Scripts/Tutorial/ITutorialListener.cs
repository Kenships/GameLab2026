using System;

namespace _Project.Scripts.Tutorial
{
    public interface ITutorialListener
    {
        public void Invoke(Action callback);
    }
}
