// UI Canvas|UI|80010
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Denotes a Unity World UI Canvas can be interacted with a UIPointer script.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_UICanvas` script on the Unity World UI Canvas to allow UIPointer interactions with.
    ///
    /// **Script Dependencies:**
    ///  * A UI Pointer attached to another GameObject (e.g. controller script alias) to interact with the UICanvas script.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/034_Controls_InteractingWithUnityUI` uses the `VRTK_UICanvas` script on two of the canvases to show how the UI Pointer can interact with them.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/UI/VRTK_UICanvas")]
    public class VRTK_UICanvas : MonoBehaviour
    {
        [Tooltip("Determines if a UI Click action should happen when a UI Pointer game object collides with this canvas.")]
        public bool clickOnPointerCollision = true;
        [Tooltip("Determines if a UI Pointer will be auto activated if a UI Pointer game object comes within the given distance of this canvas. If a value of `0` is given then no auto activation will occur.")]
        public float autoActivateWithinDistance = 0f;

        protected BoxCollider canvasBoxCollider;
        protected Rigidbody canvasRigidBody;
        protected Coroutine draggablePanelCreation;
        protected const string CANVAS_DRAGGABLE_PANEL = "VRTK_UICANVAS_DRAGGABLE_PANEL";
        protected const string ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT = "VRTK_UICANVAS_ACTIVATOR_FRONT_TRIGGER";

        private bool canDrawCard, canPlayHandCard, canUseDiskCard;
        private bool canSelectEnemyCard, canOpenGraveyard, canCloseGraveyard;
        private bool canShowPlayerGraveyard, canShowEnemyGraveyard, canInteractWithGraveyardCard;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && canDrawCard)
            {
                canDrawCard = false;
                GameManager.Get().DrawCard();
            }

            if (canPlayHandCard)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    canPlayHandCard = false;
                    gameObject.GetComponent<HandCardScript>().HandleClick(true);
                }
                if(Input.GetMouseButtonDown(1))
                {
                    gameObject.GetComponent<HandCardScript>().HandleClick(false);
                }
            }

            if(canUseDiskCard && Input.GetMouseButtonDown(0))
            {
                canUseDiskCard = false;
                gameObject.GetComponent<DiskCardScript>().HandleClick();
            }

            if(canSelectEnemyCard && Input.GetMouseButtonDown(0))
            {
                canSelectEnemyCard = false;
                gameObject.GetComponent<EnemyFieldCardScript>().HandleClick();
            }

            if(canOpenGraveyard && Input.GetMouseButtonDown(0))
            {
                canOpenGraveyard = false;
                gameObject.GetComponentInChildren<GraveyardButton>().TaskOnClick();
            }

            if (canCloseGraveyard && Input.GetMouseButtonDown(0))
            {
                canCloseGraveyard = false;
                transform.parent.GetComponentInParent<GraveyardTab>().CancelGraveyard();
            }

            if(canShowPlayerGraveyard && Input.GetMouseButtonDown(0))
            {
                canShowPlayerGraveyard = false;
                transform.parent.GetComponentInParent<GraveyardTab>().ActivatePlayerGraveyardTab();
            }

            if (canShowEnemyGraveyard && Input.GetMouseButtonDown(0))
            {
                canShowEnemyGraveyard = false;
                transform.parent.GetComponentInParent<GraveyardTab>().ActivateEnemyGraveyardTab();
            }

            if(canInteractWithGraveyardCard)
            {
                canInteractWithGraveyardCard = false;
                if(Input.GetMouseButtonDown(0) && GameManager.Get().IsInGraveyardSelMode())
                {
                    gameObject.GetComponent<GraveyardCard>().InteractWithElement();
                }

                if(Input.GetMouseButtonDown(2))
                {
                    gameObject.GetComponent<GraveyardCard>().PreviewImage();
                }
            }
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            switch(gameObject.name)
            {
                case "DeckPlaceholder":
                    if(GameManager.Get().GetTurnPhase() == Turn.Phase.Draw)
                    {
                        canDrawCard = true;
                    }
                    break;
                case "HandCard(Clone)":
                    HandCardScript script = gameObject.GetComponent<HandCardScript>();
                    script.OnPointerEnter();
                    if (script.IsHighlightable())
                    {
                        canPlayHandCard = true;
                    }
                    break;
                case "Card1":
                case "Card2":
                case "Card3":
                case "Card4":
                case "Card5":
                    DiskCardScript diskScript = gameObject.GetComponent<DiskCardScript>();
                    diskScript.OnPointerEnter();
                    if(diskScript.IsHighlightable())
                    {
                        canUseDiskCard = true;
                    }
                    break;
                case "FieldCard1":
                case "FieldCard2":
                case "FieldCard3":
                case "FieldCard4":
                case "FieldCard5":
                    gameObject.GetComponent<PlayerFieldCardScript>().OnMouseEnter();
                    break;
                case "EnemyFieldCard1":
                case "EnemyFieldCard2":
                case "EnemyFieldCard3":
                case "EnemyFieldCard4":
                case "EnemyFieldCard5":
                    EnemyFieldCardScript fieldScript = gameObject.GetComponent<EnemyFieldCardScript>();
                    fieldScript.OnPointerEnter();
                    if(fieldScript.IsHighlightable())
                    {
                        canSelectEnemyCard = true;
                    }
                    break;
                case "GraveyardButtonCanvas":
                    canOpenGraveyard = true;
                    break;
                case "CancelGraveyard":
                    canCloseGraveyard = true;
                    break;
                case "PlayerTab":
                    canShowPlayerGraveyard = true;
                    break;
                case "EnemyTab":
                    canShowEnemyGraveyard = true;
                    break;
                case "Card":
                    canInteractWithGraveyardCard = true;
                    break;
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            switch (gameObject.name)
            {
                case "DeckPlaceholder":
                    canDrawCard = false;
                    break;
                case "HandCard(Clone)":
                    gameObject.GetComponent<HandCardScript>().OnPointerExit();
                    canPlayHandCard = false;
                    break;
                case "Card1":
                case "Card2":
                case "Card3":
                case "Card4":
                case "Card5":
                    canUseDiskCard = false;
                    break;
                case "FieldCard1":
                case "FieldCard2":
                case "FieldCard3":
                case "FieldCard4":
                case "FieldCard5":
                    gameObject.GetComponent<PlayerFieldCardScript>().OnMouseExit();
                    break;
                case "EnemyFieldCard1":
                case "EnemyFieldCard2":
                case "EnemyFieldCard3":
                case "EnemyFieldCard4":
                case "EnemyFieldCard5":
                    gameObject.GetComponent<EnemyFieldCardScript>().OnPointerExit();
                    canSelectEnemyCard = false;
                    break;
                case "GraveyardButtonCanvas":
                    canOpenGraveyard = false;
                    break;
                case "CancelGraveyard":
                    canCloseGraveyard = false;
                    break;
                case "PlayerTab":
                    canShowPlayerGraveyard = false;
                    break;
                case "EnemyTab":
                    canShowEnemyGraveyard = false;
                    break;
                case "Card":
                    canInteractWithGraveyardCard = false;
                    break;
            }
        }

        protected virtual void SetupCanvas()
        {
            Canvas canvas = GetComponent<Canvas>();

            if (canvas == null || canvas.renderMode != RenderMode.WorldSpace)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_UICanvas", "Canvas", "the same", " that is set to `Render Mode = World Space`"));
                return;
            }

            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRectTransform.sizeDelta;
            //copy public params then disable existing graphic raycaster
            GraphicRaycaster defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            VRTK_UIGraphicRaycaster customRaycaster = canvas.gameObject.GetComponent<VRTK_UIGraphicRaycaster>();

            //if it doesn't already exist, add the custom raycaster
            if (customRaycaster == null)
            {
                customRaycaster = canvas.gameObject.AddComponent<VRTK_UIGraphicRaycaster>();
            }

            if (defaultRaycaster != null && defaultRaycaster.enabled)
            {
                customRaycaster.ignoreReversedGraphics = defaultRaycaster.ignoreReversedGraphics;
                customRaycaster.blockingObjects = defaultRaycaster.blockingObjects;
                
                //Use Reflection to transfer the BlockingMask
                customRaycaster.GetType().GetField("m_BlockingMask", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(customRaycaster,defaultRaycaster.GetType().GetField("m_BlockingMask", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(defaultRaycaster));

                defaultRaycaster.enabled = false;
            }

            //add a box collider and background image to ensure the rays always hit
            if (canvas.gameObject.GetComponent<BoxCollider>() == null)
            {
                Vector2 pivot = canvasRectTransform.pivot;
                float zSize = 0.1f;
                float zScale = zSize / canvasRectTransform.localScale.z;

                canvasBoxCollider = canvas.gameObject.AddComponent<BoxCollider>();
                canvasBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, zScale);
                canvasBoxCollider.center = new Vector3(canvasSize.x / 2 - canvasSize.x * pivot.x, canvasSize.y / 2 - canvasSize.y * pivot.y, zScale / 2f);
                canvasBoxCollider.isTrigger = true;
            }

            if (canvas.gameObject.GetComponent<Rigidbody>() == null)
            {
                canvasRigidBody = canvas.gameObject.AddComponent<Rigidbody>();
                canvasRigidBody.isKinematic = true;
            }

            draggablePanelCreation = StartCoroutine(CreateDraggablePanel(canvas, canvasSize));
            CreateActivator(canvas, canvasSize);
        }

        protected virtual IEnumerator CreateDraggablePanel(Canvas canvas, Vector2 canvasSize)
        {
            if (canvas != null && !canvas.transform.Find(CANVAS_DRAGGABLE_PANEL))
            {
                yield return null;

                GameObject draggablePanel = new GameObject(CANVAS_DRAGGABLE_PANEL, typeof(RectTransform));
                draggablePanel.AddComponent<LayoutElement>().ignoreLayout = true;
                draggablePanel.AddComponent<Image>().color = Color.clear;
                draggablePanel.AddComponent<EventTrigger>();
                draggablePanel.transform.SetParent(canvas.transform);
                draggablePanel.transform.localPosition = Vector3.zero;
                draggablePanel.transform.localRotation = Quaternion.identity;
                draggablePanel.transform.localScale = Vector3.one;
                draggablePanel.transform.SetAsFirstSibling();

                draggablePanel.GetComponent<RectTransform>().sizeDelta = canvasSize;
            }
        }

        protected virtual void CreateActivator(Canvas canvas, Vector2 canvasSize)
        {
            //if autoActivateWithinDistance is greater than 0 then create the front collider sub object
            if (autoActivateWithinDistance > 0f && canvas != null && !canvas.transform.Find(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT))
            {
                RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
                Vector2 pivot = canvasRectTransform.pivot;

                GameObject frontTrigger = new GameObject(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT);
                frontTrigger.transform.SetParent(canvas.transform);
                frontTrigger.transform.SetAsFirstSibling();
                frontTrigger.transform.localPosition = new Vector3(canvasSize.x / 2 - canvasSize.x * pivot.x, canvasSize.y / 2 - canvasSize.y * pivot.y);
                frontTrigger.transform.localRotation = Quaternion.identity;
                frontTrigger.transform.localScale = Vector3.one;

                float actualActivationDistance = autoActivateWithinDistance / canvasRectTransform.localScale.z;
                BoxCollider frontTriggerBoxCollider = frontTrigger.AddComponent<BoxCollider>();
                frontTriggerBoxCollider.isTrigger = true;
                frontTriggerBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, actualActivationDistance);
                frontTriggerBoxCollider.center = new Vector3(0f, 0f, -(actualActivationDistance / 2));

                frontTrigger.AddComponent<Rigidbody>().isKinematic = true;
                frontTrigger.AddComponent<VRTK_UIPointerAutoActivator>();
                frontTrigger.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        protected virtual void RemoveCanvas()
        {
            Canvas canvas = GetComponent<Canvas>();

            if (canvas == null)
            {
                return;
            }

            GraphicRaycaster defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            VRTK_UIGraphicRaycaster customRaycaster = canvas.gameObject.GetComponent<VRTK_UIGraphicRaycaster>();
            //if a custom raycaster exists then remove it
            if (customRaycaster != null)
            {
                Destroy(customRaycaster);
            }

            //If the default raycaster is disabled, then re-enable it
            if (defaultRaycaster != null && !defaultRaycaster.enabled)
            {
                defaultRaycaster.enabled = true;
            }

            //Check if there is a collider and remove it if there is
            if (canvasBoxCollider != null)
            {
                Destroy(canvasBoxCollider);
            }

            if (canvasRigidBody != null)
            {
                Destroy(canvasRigidBody);
            }

            if (draggablePanelCreation != null)
            {
                StopCoroutine(draggablePanelCreation);
            }

            Transform draggablePanel = canvas.transform.Find(CANVAS_DRAGGABLE_PANEL);
            if (draggablePanel != null)
            {
                Destroy(draggablePanel.gameObject);
            }

            Transform frontTrigger = canvas.transform.Find(ACTIVATOR_FRONT_TRIGGER_GAMEOBJECT);
            if (frontTrigger != null)
            {
                Destroy(frontTrigger.gameObject);
            }
        }
    }

    public class VRTK_UIPointerAutoActivator : MonoBehaviour
    {
        protected virtual void OnTriggerEnter(Collider collider)
        {
            VRTK_PlayerObject colliderCheck = collider.GetComponentInParent<VRTK_PlayerObject>();
            VRTK_UIPointer pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck != null && colliderCheck != null && colliderCheck.objectType == VRTK_PlayerObject.ObjectTypes.Collider)
            {
                pointerCheck.autoActivatingCanvas = gameObject;
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            VRTK_UIPointer pointerCheck = collider.GetComponentInParent<VRTK_UIPointer>();
            if (pointerCheck != null && pointerCheck.autoActivatingCanvas == gameObject)
            {
                pointerCheck.autoActivatingCanvas = null;
            }
        }
    }
}