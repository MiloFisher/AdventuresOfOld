using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JL_UI_Manager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public Sprite button;
    public Sprite buttonSelected;

    public void OnPointerDown(PointerEventData eventData) {
        //this.GetComponent<AudioSource>().Play();
        isHighlighted(0);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        JLAudioManager.Instance.PlaySound("OnHover");
        isHighlighted(1);
    }

    private void isHighlighted(int status) {
        if (this.name == "Profile_BUTTON" || this.name == "SaveProfile_BUTTON") {
            //spriteScale(status);
        }
        else if (this.name == "ReturnToMenu_BUTTON") {
            if (status == 0) {
                this.GetComponent<Image>().transform.localScale = new Vector2(2, 2);
                this.GetComponent<Image>().sprite = button;
            }
            else if (status == 1) {
                this.GetComponent<Image>().transform.localScale = new Vector2((float)2.2, (float)2.2);
                this.GetComponent<Image>().sprite = buttonSelected;
            }
        }
        else {
            if (status == 0) {
            this.GetComponent<Image>().sprite = button;
            spriteScale(status);
            GameObject dagger = this.gameObject.transform.GetChild(1).gameObject;
            dagger.SetActive(false);
            }
            else if (status == 1) {
                this.GetComponent<Image>().sprite = buttonSelected;
                spriteScale(status);
                GameObject dagger = this.gameObject.transform.GetChild(1).gameObject;
                dagger.SetActive(true);
            }
        }
    }

    private void spriteScale(int status) {
        if (status == 0) {
            this.GetComponent<Image>().transform.localScale = new Vector2((float)1.2, (float)1.2);
        }
        else if (status == 1) {
            this.GetComponent<Image>().transform.localScale = new Vector2((float)1.4, (float)1.4);
        }
    }
}
