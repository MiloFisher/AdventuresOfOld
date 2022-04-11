using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class JL_UI_Manager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    AsyncOperationHandle<Sprite> asyncSprite;
    AsyncOperationHandle<Sprite> asyncSpriteSelected;

    void Start() {
        asyncSprite = Addressables.LoadAssetAsync<Sprite>("Assets/Game_Resources/UI/scroll_button.png");
        asyncSpriteSelected = Addressables.LoadAssetAsync<Sprite>("Assets/Game_Resources/UI/scroll_button_selected.png");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = asyncSpriteSelected.Result;
        this.GetComponent<Image>().transform.localScale = new Vector2((float)1.2, (float)1.2);
        Transform daggerTransform = this.gameObject.transform.GetChild(1);
        GameObject dagger = daggerTransform.gameObject;
        dagger.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData) {
        this.GetComponent<Image>().sprite = asyncSprite.Result;
        this.GetComponent<Image>().transform.localScale = new Vector2(1, 1);
        Transform daggerTransform = this.gameObject.transform.GetChild(1);
        GameObject dagger = daggerTransform.gameObject;
        dagger.SetActive(false);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = asyncSprite.Result;
        this.GetComponent<Image>().transform.localScale = new Vector2(1, 1);
        Transform daggerTransform = this.gameObject.transform.GetChild(1);
        GameObject dagger = daggerTransform.gameObject;
        dagger.SetActive(false);
    }
}
