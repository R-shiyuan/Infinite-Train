using NodeCanvas.Framework;
using ParadoxNotion.Design;
using DG.Tweening;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Tweening")]
    [Description("移动物体到目标位置")]
    public class EaseTransformTween : ActionTask<Transform>
    {

        public BBParameter<Vector3> targetPosition;
        public BBParameter<float> duration = 1f;
        public Ease easeType = Ease.Linear;

        protected override void OnExecute()
        {
            agent.DOMove(targetPosition.value, duration.value)
                .SetEase(easeType)
                .OnComplete(() => EndAction(true));
        }
    }
}