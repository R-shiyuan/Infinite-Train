using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#if DOTWEEN_ENABLED // 增加条件编译，防止报错
using DG.Tweening;
#endif

namespace NodeCanvas.Tasks.Actions
{
    [Category("Tweening")]
    [Description("移动物体到目标位置")]
    public class EaseTransformTween : ActionTask<Transform>
    {
        public BBParameter<Vector3> targetPosition;
        public BBParameter<float> duration = 1f;
#if DOTWEEN_ENABLED
        public Ease easeType = Ease.Linear;
#endif

        protected override void OnExecute()
        {
#if DOTWEEN_ENABLED
            agent.DOMove(targetPosition.value, duration.value)
                .SetEase(easeType)
                .OnComplete(() => EndAction(true));
#else
            Debug.LogError("DOTween 未安装，无法执行移动动作！");
            EndAction(false);
#endif
        }
    }
}