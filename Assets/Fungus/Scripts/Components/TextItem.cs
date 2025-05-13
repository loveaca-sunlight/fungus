using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// A single line of dialog
    /// </summary>
    public class TextItem : MonoBehaviour
    {
        [Tooltip("TextAdapter will search for appropriate output on this GameObject if nameText is null")]
        [SerializeField] protected GameObject nameTextGO;
        protected string nameText;
        protected ContentSizeFitter myContentSizeFitter;

        public virtual string NameText
        {
            get
            {
                return nameText;
            }
        }

        /// <summary>
        /// Sets the character name to display on the Say Dialog.
        /// Supports variable substitution e.g. John {$surname}
        /// </summary>
        public virtual void SetCharacterName(string name, Color color)
        {
            // 移除 null 检查直接处理字符串替换
            var subbedName = stringSubstituter.SubstituteStrings(name);
            // 将颜色转换为 HEX 格式并包装成富文本标签
            string colorHex = ColorUtility.ToHtmlStringRGBA(color);
            nameText = $"<color=#{colorHex}>{subbedName}</color>";
        }

        [Tooltip("TextAdapter will search for appropriate output on this GameObject if storyText is null")]
        [SerializeField] protected GameObject storyTextGO;
        protected string storyText;
        public virtual string StoryText
        {
            get
            {
                return storyText;
            }
            set
            {
                storyText = value;
            }
        }
 
        public virtual string Text
        {
            get
            {
                return nameText + ": " + storyText;
            }
        }

        protected StringSubstituter stringSubstituter = new StringSubstituter();

        protected virtual void Awake()
		{
            myContentSizeFitter = gameObject.GetComponent<ContentSizeFitter>();
        }

        protected virtual void Start()
        {
            if (nameText == "")
            {
                SetCharacterName("", Color.white);
            }
        }

        protected virtual void ClearStoryText()
        {
            StoryText = "";
        }

        
        /// <summary>
        /// Stops writing text and clears the Say Dialog.
        /// </summary>
        public virtual void Clear()
        {
            ClearStoryText();

            // Kill any active write coroutine
            StopAllCoroutines();
        }

        public virtual void SetContentSizeFilter()
        {
            myContentSizeFitter.enabled = true;
        }
    }
}