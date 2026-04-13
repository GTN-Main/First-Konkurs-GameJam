using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MovableAnimationController : MonoBehaviour
{
    Animator animator;
    public Dictionary<string, AnimationClip> animationActions = new Dictionary<string, AnimationClip>();

    string currentActionName, previousActionName;

    protected void Init(Animator animator, Dictionary<string, AnimationClip> animationActions)
    {
        this.animator = animator;
        this.animationActions = animationActions;
    }

    protected void ChangeAnim(string actionName)
    {
        currentActionName = actionName;
    }

    protected void UpdateController()
    {
        if (currentActionName == previousActionName)
            return;

        AnimationClip clip;
        if (animationActions.TryGetValue(currentActionName, out clip))
        {
            animator.Play(clip.name);
        }
        else
        {
            Debug.LogWarning($"Animation action '{currentActionName}' not found in the animation actions dictionary.");
        }
        
        previousActionName = currentActionName;
    }
}