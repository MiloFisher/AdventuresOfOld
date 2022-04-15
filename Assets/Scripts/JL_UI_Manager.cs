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

    public void OnPointerDown(PointerEventData eventData) {
        isHighlighted(0);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHighlighted(1);
    }

    private void isHighlighted(int status) {
        if (status == 0) {
            this.GetComponent<Image>().sprite = asyncSprite.Result;
            this.GetComponent<Image>().transform.localScale = new Vector2((float)1.2, (float)1.2);
            GameObject dagger = this.gameObject.transform.GetChild(1).gameObject;
            dagger.SetActive(false);
        }
        else if (status == 1) {
            this.GetComponent<Image>().sprite = asyncSpriteSelected.Result;
            this.GetComponent<Image>().transform.localScale = new Vector2((float)1.4, (float)1.4);
            GameObject dagger = this.gameObject.transform.GetChild(1).gameObject;
            dagger.SetActive(true);
        }
    }
}
