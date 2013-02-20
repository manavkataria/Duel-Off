using UnityEngine;
using System;
using System.Collections;

/* Animation Extensions
 * -- Used to extend functionality of the Unity Animation Class by way of CoRoutine
 */
public static class AnimationExtensions {
	
    public static IEnumerator PlayWithOptions(this Animation animation, string animName, Action onComplete)
    {
        animation.Play(animName);

        while (animation.isPlaying) { yield return null; }

        onComplete();
    }

    public static IEnumerator PlayWithOptions(this Animation animation, string animName, float delay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);

        animation.Play(animName);

        while (animation.isPlaying) { yield return null; }

        onComplete();
    }

    public static IEnumerator PlayWithOptions(this Animation animation, string animName, Action onUpdate, Action onComplete)
    {
        animation.Play(animName);

        while (animation.isPlaying)
        {
            onUpdate();
            yield return null;
        }

        onComplete();
    }

    public static IEnumerator PlayWithOptions(this Animation animation, string animName, float delay, Action onUpdate, Action onComplete)
    {
        yield return new WaitForSeconds(delay);

        animation.Play(animName);

        while (animation.isPlaying)
        {
            onUpdate();
            yield return null;
        }

        onComplete();
    }
}
