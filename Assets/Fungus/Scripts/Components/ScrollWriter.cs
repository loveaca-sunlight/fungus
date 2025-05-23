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
        [SerializeField] protected RectTransform scrollObject;

        [Tooltip("用于控制文字出现初期的滑动")]
        [SerializeField] protected RectTransform viewPort;

        [Tooltip("滑动时间")]
        [SerializeField] protected float moveDuration = 0.5f;

        [Tooltip("直接显示完整文本，不进行跳字")]
        [SerializeField] protected bool showTextDerictly = true;

        private float viewPortMaxHeight;
        private float viewPortCurHeight;

        private float spaceHeight = 10f;

        protected override void Awake()
        {
            base.Awake();
            viewPortMaxHeight = viewPort.rect.height;
            viewPort.offsetMax = new Vector2(0f, -viewPortMaxHeight);
            viewPortCurHeight = 0;
        }

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

        public IEnumerator AddOptions(List<ButtonItem> options, Action onComplete = null)
        {
            yield return null;
            float height = 0f;
            foreach (var option in options)
            {
                if (option.transform is RectTransform optionRect)
                {
                    height += optionRect.rect.height + spaceHeight;
                }
            }
            yield return StartCoroutine(DoScrollMove(height));
        }

        public virtual IEnumerator ProcessScroll(TextItem item, bool stopAudio, Action onComplete)
        {
            isWriting = true;

            textAdapter.Text = item.Text;
            yield return null;
            item.SetContentSizeFilter(true);
            yield return null;
            float height = 0f;
            if (item.transform is RectTransform itemRect)
            {
                height = itemRect.rect.height + spaceHeight;
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
            float elapsedTime2 = 0f;

            viewPortCurHeight += moveHeight;

            Vector2 startSize = viewPort.offsetMax;
            Vector2 endSize = startSize + new Vector2(0f, moveHeight);
            Vector3 startPos = scrollObject.localPosition;
            Vector3 upPos = scrollObject.localPosition + new Vector3(0f, moveHeight, 0f);
            float viewPortDuration = moveDuration;
            float scrollDuration = moveDuration;
            // 触发viewPort和Scroll同时滑动的情况
            if (viewPort.rect.height < viewPortMaxHeight && startSize.y + moveHeight > 0f)
            {
                endSize = new Vector2(0f, 0f);
                upPos = new Vector3(0f, moveHeight + startSize.y, 0f);
                viewPortDuration = moveDuration * (-startSize.y / moveHeight);
                scrollDuration -= viewPortDuration;
            }

            while (elapsedTime + elapsedTime2 < moveDuration)
            {
                if (viewPort.rect.height < viewPortMaxHeight)
                {
                    viewPort.offsetMax = Vector2.Lerp(startSize, endSize, elapsedTime / viewPortDuration);
                    elapsedTime += Time.deltaTime;

                    yield return null;
                }
                else
                {
                    scrollObject.localPosition = Vector3.Lerp(startPos, upPos, elapsedTime2 / scrollDuration);
                    elapsedTime2 += Time.deltaTime;

                    yield return null;
                }
            }
            // 确保最终位置准确
            // viewPort.offsetMax = endSize;
            // scrollObject.localPosition = upPos;
        }
    }
}
