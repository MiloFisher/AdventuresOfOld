using UnityEngine;

public class UIQuestChoice : MonoBehaviour
{
    public int id;

    public void OnClick()
    {
        QuestManager.Instance.Choice(id);
    }
}
