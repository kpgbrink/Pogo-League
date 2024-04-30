/// Credit zero3growlithe
/// sourced from: http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-2011648

/*USAGE:
Simply place the script on the ScrollRect that contains the selectable children we'll be scroling to
and drag'n'drop the RectTransform of the options "container" that we'll be scrolling.*/

using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/UIScrollToSelection")]
    public class UIScrollToSelection : MonoBehaviour
    {
        int? layoutListGroupDepth;

        //*** ATTRIBUTES ***//
        [Header("[ Settings ]")]
        [SerializeField]
        private ScrollType scrollDirection = ScrollType.BOTH;
        [SerializeField]
        private float scrollSpeed = 10f;

        [Header("[ Input ]")]
        [SerializeField]
        private bool cancelScrollOnInput = false;
        [SerializeField]
        private List<KeyCode> cancelScrollKeycodes = new List<KeyCode>();

        //*** PROPERTIES ***//
        // REFERENCES
        protected RectTransform LayoutListGroup
        {
            get { return TargetScrollRect != null ? TargetScrollRect.content : null; }
        }

        // SETTINGS
        protected ScrollType ScrollDirection
        {
            get { return scrollDirection; }
        }
        protected float ScrollSpeed
        {
            get { return scrollSpeed; }
        }

        // INPUT
        protected bool CancelScrollOnInput
        {
            get { return cancelScrollOnInput; }
        }
        protected List<KeyCode> CancelScrollKeycodes
        {
            get { return cancelScrollKeycodes; }
        }

        // CACHED REFERENCES
        protected RectTransform ScrollWindow { get; set; }
        protected ScrollRect TargetScrollRect { get; set; }

        // SCROLLING
        protected EventSystem CurrentEventSystem
        {
            get { return EventSystem.current; }
        }
        protected GameObject LastCheckedGameObject { get; set; }
        protected GameObject CurrentSelectedGameObject
        {
            get { return EventSystem.current.currentSelectedGameObject; }
        }
        protected RectTransform CurrentTargetRectTransform { get; set; }
        protected bool IsManualScrollingAvailable { get; set; }

        //*** METHODS - PUBLIC ***//


        //*** METHODS - PROTECTED ***//
        protected virtual void Awake()
        {
            TargetScrollRect = GetComponent<ScrollRect>();
            ScrollWindow = TargetScrollRect.GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            UpdateReferences();
            CheckIfScrollingShouldBeLocked();
            ScrollRectToLevelSelection();
        }

        //*** METHODS - PRIVATE ***//
        private void UpdateReferences()
        {
            // update current selected rect transform
            if (CurrentSelectedGameObject != LastCheckedGameObject)
            {
                layoutListGroupDepth = null;
                if (CurrentSelectedGameObject != null)
                {
                    CurrentTargetRectTransform = CurrentSelectedGameObject.GetComponent<RectTransform>();
                    SetLayoutListGroupDepth();
                    // unlock automatic scrolling
                    if (CurrentSelectedGameObject.transform.parent == LayoutListGroup.transform)
                    {
                        IsManualScrollingAvailable = false;
                    }
                }
            }
            LastCheckedGameObject = CurrentSelectedGameObject;
        }

        /// <summary>
        /// Set layoutListDepth CurrentTargetRectTransform makes sense
        /// </summary>
        void SetLayoutListGroupDepth()
        {
            if (!CurrentSelectedGameObject.transform.IsChildOf(transform)) return;
            layoutListGroupDepth = 0;
            var rectTransform = CurrentTargetRectTransform;
            // Keep getting until no parent or the object has LayoutListGroup
            while (true)
            {
                if (rectTransform == LayoutListGroup.transform) break;
                if (rectTransform.parent == null)
                {
                    layoutListGroupDepth = null;
                    return;
                }
                rectTransform = rectTransform.parent.GetComponent<RectTransform>();
                layoutListGroupDepth++;
            }
        }

        private void CheckIfScrollingShouldBeLocked()
        {
            if (CancelScrollOnInput == false || IsManualScrollingAvailable == true)
            {
                return;
            }

            for (var i = 0; i < CancelScrollKeycodes.Count; i++)
            {
                if (Input.GetKeyDown(CancelScrollKeycodes[i]) == true)
                {
                    IsManualScrollingAvailable = true;

                    break;
                }
            }
        }

        private void ScrollRectToLevelSelection()
        {
            // check main references
            var referencesAreIncorrect = (TargetScrollRect == null || LayoutListGroup == null || ScrollWindow == null);

            if (referencesAreIncorrect == true || IsManualScrollingAvailable == true) return;

            var selection = CurrentTargetRectTransform;

            // Check if should be scrolling
            if (layoutListGroupDepth == null)
            {
                return;
            }

            // depending on selected scroll direction move the scroll rect to selection
            switch (ScrollDirection)
            {
                case ScrollType.VERTICAL:
                    UpdateVerticalScrollPosition(selection);
                    break;
                case ScrollType.HORIZONTAL:
                    UpdateHorizontalScrollPosition(selection);
                    break;
                case ScrollType.BOTH:
                    UpdateVerticalScrollPosition(selection);
                    UpdateHorizontalScrollPosition(selection);
                    break;
            }
        }

        float GetToListLayout(RectTransform selection, Func<RectTransform, float> method)
        {
            var fAdd = 0f;
            for (var i = 0; i < layoutListGroupDepth; i++)
            {
                fAdd += method(selection); //selection.anchoredPosition.y;
                selection = selection.parent.GetComponent<RectTransform>();
            }
            return fAdd;
        }

        private void UpdateVerticalScrollPosition(RectTransform selection)
        {
            // move the current scroll rect to correct position
            var selectionPosition = GetToListLayout(selection, o => -o.anchoredPosition.y - o.rect.height * (1 - o.pivot.y));

            var elementHeight = selection.rect.height;
            var maskHeight = ScrollWindow.rect.height;
            var listAnchorPosition = LayoutListGroup.anchoredPosition.y;

            // get the element offset value depending on the cursor move direction
            var offlimitsValue = GetScrollOffset(selectionPosition, listAnchorPosition, elementHeight, maskHeight);

            // move the target scroll rect
            TargetScrollRect.verticalNormalizedPosition +=
                (offlimitsValue / LayoutListGroup.rect.height) * Time.unscaledDeltaTime * scrollSpeed;
        }

        private void UpdateHorizontalScrollPosition(RectTransform selection)
        {
            // move the current scroll rect to correct positionS
            var selectionPosition = GetToListLayout(selection, o => -o.anchoredPosition.x - (o.rect.width * (1 - o.pivot.x)));

            var elementWidth = selection.rect.width;
            var maskWidth = ScrollWindow.rect.width;
            var listAnchorPosition = -LayoutListGroup.anchoredPosition.x;

            // get the element offset value depending on the cursor move direction
            var offlimitsValue = -GetScrollOffset(selectionPosition, listAnchorPosition, elementWidth, maskWidth);

            // move the target scroll rect
            TargetScrollRect.horizontalNormalizedPosition +=
                (offlimitsValue / LayoutListGroup.rect.width) * Time.unscaledDeltaTime * scrollSpeed;
        }

        private float GetScrollOffset(float position, float listAnchorPosition, float targetLength, float maskLength)
        {
            if (position < listAnchorPosition /*+ (targetLength / 2)*/)
            {
                return (listAnchorPosition + maskLength) - (position);
            }
            else if (position + targetLength > listAnchorPosition + maskLength)
            {
                return (listAnchorPosition + maskLength) - (position + targetLength);
            }

            return 0;
        }

        //*** ENUMS ***//
        public enum ScrollType
        {
            VERTICAL,
            HORIZONTAL,
            BOTH
        }
    }
}