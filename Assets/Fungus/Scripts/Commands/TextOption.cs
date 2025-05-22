// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// 在 SayDialog 中显示选项按钮
    /// </summary>
    [CommandInfo("Narrative", 
                 "TextOption", 
                 "Displays a button in SayDialog")]
    [AddComponentMenu("")]
    public class TextOption : Command, ILocalizable, IBlockCaller
    {
        [Tooltip("Text to display on the menu button")]
        [TextArea()]
        [SerializeField] protected string text = "Option Text";

        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("Block to execute when this option is selected")]
        [SerializeField] protected Block targetBlock;

        [Tooltip("Hide this option if the target block has been executed previously")]
        [SerializeField] protected bool hideIfVisited;

        [Tooltip("If false, the menu option will be displayed but will not be selectable")]
        [SerializeField] protected BooleanData interactable = new BooleanData(true);

        [Tooltip("A custom Say Dialog to use to display this menu. All subsequent TextOption commands will use this dialog.")]
        [SerializeField] protected SayDialog setSayDialog;

        [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
        [SerializeField] protected BooleanData hideThisOption = new BooleanData(false);

        #region Public members

        public override void OnEnter()
        {
            if (setSayDialog != null)
            {
                // Override the active menu dialog
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog != null)
            {
                //sayDialog.SetActive(true);

                var flowchart = GetFlowchart();
                string displayText = flowchart.SubstituteVariables(text);

                sayDialog.AddOption(displayText, interactable, hideOption, targetBlock);
            }
            
            Continue();
        }

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (targetBlock != null)
            {
                connectedBlocks.Add(targetBlock);
            }       
        }

        public override string GetSummary()
        {
            if (targetBlock == null)
            {
                return "Error: No target block selected";
            }

            if (text == "")
            {
                return "Error: No button text selected";
            }

            return text + " : " + targetBlock.BlockName;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return interactable.booleanRef == variable || hideThisOption.booleanRef == variable ||
                base.HasReference(variable);
        }

        public bool MayCallBlock(Block block)
        {
            return block == targetBlock;
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return text;
        }

        public virtual void SetStandardText(string standardText)
        {
            text = standardText;
        }
        
        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Menu commands is MENU.<Localization Id>.<Command id>
            return "MENU." + GetFlowchartLocalizationId() + "." + itemId;
        }

        #endregion

        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(text, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}