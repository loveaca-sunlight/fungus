using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// A single line of dialog
    /// </summary>
    public class TextItem : AbstractTextItem
    {
        [SerializeField] GameObject textObject;
        protected TextAdapter textAdapter = new TextAdapter();
        protected string nameText;

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

        protected string storyText;
        public virtual string StoryText
        {
            get
            {
                return storyText;
            }
            set
            {
                storyText = stringSubstituter.SubstituteStrings(value);;
            }
        }

        public override string Text
        {
            get
            {
                if (nameText == null || nameText == "")
                    return storyText;
                else
                    return nameText + ": " + storyText;
            }
        }

        public virtual void ShowTextDerictly(string text)
        {
            if (textAdapter == null) return;
            storyText = text;
            textAdapter.Text = Text;
        }

        protected virtual void Awake()
        {
            if (textObject == null)
            {
                textObject = gameObject;
            }
            textAdapter.InitFromGameObject(textObject);
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
        public override void Clear()
        {
            base.Clear();
            ClearStoryText();
        }
    }
}