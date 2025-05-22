using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// A single line of dialog
    /// </summary>
    public abstract class AbstractTextItem : MonoBehaviour
    {
        [SerializeField] protected ContentSizeFitter myContentSizeFitter;
        protected StringSubstituter stringSubstituter = new StringSubstituter();

        public abstract string Text
        {
            get;
        }

        /// <summary>
        /// Stops writing text and clears the Say Dialog.
        /// </summary>
        public virtual void Clear()
        {
            // Kill any active write coroutine
            StopAllCoroutines();
        }

        public virtual void SetContentSizeFilter(bool flag)
        {
            myContentSizeFitter.enabled = flag;
        }
    }
}