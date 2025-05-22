// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Globalization;
using System;

namespace Fungus
{
    /// <summary>
    /// 主要控制滑动条滑动的Writer
    /// </summary>
    public class ScrollWriter : Writer
    {
        [Tooltip("滑动条物体")]
        [SerializeField] protected Transform scrollObject;

        [Tooltip("滑动时间")]
        [SerializeField] protected float moveDuration = 0.5f;

        [Tooltip("直接显示完整文本，不进行跳字")]
        [SerializeField] protected bool showTextDerictly = true;

        public IEnumerator Write(TextItem item, bool clear, bool stopAudio, AudioClip audioClip, Action onComplete)
        {
            // if (clear)
            // {
            //     textAdapter.Text = "";
            //     visibleCharacterCount = 0;
            // }

            if (!textAdapter.HasTextObject())
            {
                yield break;
            }

            // If this clip is null then WriterAudio will play the default sound effect (if any)
            NotifyStart(audioClip);
            gameObject.SetActive(true);

            yield return StartCoroutine(ProcessScroll(item, stopAudio, onComplete));
        }

        public virtual IEnumerator ProcessScroll(TextItem item, bool stopAudio, Action onComplete)
        {
            isWriting = true;

            textAdapter.Text = item.Text;
            yield return null;
            item.SetContentSizeFilter(true);
            float height = 0f;
            if (item.transform is RectTransform itemRect)
            {
                height = itemRect.rect.height;
            }

            yield return StartCoroutine(DoScrollMove(height));

            yield return StartCoroutine(DoWaitForInput(false));

            isWaitingForInput = false;
            isWriting = false;

            NotifyEnd(stopAudio);

            if (onComplete != null)
            {
                onComplete();
            }
        }

        protected virtual IEnumerator DoScrollMove(float moveHeight)
        {
            float elapsedTime = 0f;
            Vector3 startPos = scrollObject.localPosition;
            Vector3 upPos = scrollObject.localPosition + new Vector3(0f, moveHeight, 0f);

            while (elapsedTime < moveDuration)
            {
                // 插值计算新位置
                scrollObject.localPosition = Vector3.Lerp(startPos, upPos, elapsedTime / moveDuration);

                // 增加已经经过的时间
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // 确保最终位置准确
            scrollObject.localPosition = upPos;
        }
    }
}
