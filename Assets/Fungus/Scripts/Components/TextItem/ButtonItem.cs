using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// A single line of dialog
    /// </summary>
    public class ButtonItem : AbstractTextItem
    {
        public Button myButton;
        protected string buttonText;

        public override string Text
        {
            get
            {
                return buttonText;
            }
        }

        /// <summary>
        /// 设置按钮内容
        /// 原版的MenuDialog中对于按钮有一个缓存优化。原本只维护一个始终存在的MenuDialog，每次进行选项的时候显示MenuDialog，并替换按钮显示文字内容。
        /// 由于选项是直接出现在文本中，这里我们只能根据textItem的具体设置来动态创建了。暂时想不到什么可以优化的点。如果之后有性能问题再进行优化。
        /// </summary>
        /// <returns><c>true</c>, if the option was added successfully.</returns>
        /// <param name="text">The option text to display on the button.</param>
        /// <param name="interactable">If false, the option is displayed but is not selectable.</param>
        /// <param name="hideOption">If true, the option is not displayed but the menu knows that option can or did exist</param>
        /// <param name="action">Action attached to the button on the menu item</param>
        public virtual void SetButton(string text, bool interactable, bool hideOption, UnityEngine.Events.UnityAction action)
        {
            if (myButton == null)
            {
                myButton = gameObject.AddComponent<Button>();
            }

            //don't need to set anything on it
            if (hideOption)
                return;

            myButton.interactable = interactable;

            TextAdapter textAdapter = new TextAdapter();
            textAdapter.InitFromGameObject(myButton.gameObject, true);
            if (textAdapter.HasTextObject())
            {
                text = TextVariationHandler.SelectVariations(text);

                textAdapter.Text = text;
            }

            myButton.onClick.AddListener(action);

            return;
        }
    }
}