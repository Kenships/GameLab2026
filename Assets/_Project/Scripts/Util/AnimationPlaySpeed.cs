using UnityEngine;

public class AnimationPlaySpeed : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private float animSpeed = 1f;

    private void Awake()
    {

        anim[anim.clip.name].speed = animSpeed;
    }
}
