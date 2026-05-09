using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{

    [Category("Custom")]
    [Description("从 CSV 播放指定 ID 的剧情")]
    public class PlayCSVPlot : ActionTask
    {

        [RequiredField]
        public BBParameter<string> plotID; // 这里填 parent_D1_0

        protected override void OnExecute()
        {
            if (DialogueBridge.Instance == null)
            {
                EndAction(false);
                return;
            }

            DialogueBridge.Instance.PlayPlot(plotID.value, () => {
                EndAction(true);
            });
        }
    }
}