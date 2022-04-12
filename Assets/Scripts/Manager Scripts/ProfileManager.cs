using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AdventuresOfOldMultiplayer
{
    public class ProfileManager : Singleton<ProfileManager>
    {
        public Image profilePicture;
        public TMP_InputField usernameInputfield;
        public TMP_Text uuidText;

        public string profilePic;
        public string username;
        public string uuid;

        // Start is called before the first frame update
        void Start()
        {
            //PlayerPrefs.DeleteAll();
            CancelChanges();
        }

        public void ChangeUsername()
        {
            username = usernameInputfield.text;
        }

        public void SaveChanges()
        {
            PlayerPrefs.SetString("profilePic", profilePic);
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetString("uuid", uuid);
        }

        public void CancelChanges()
        {
            profilePic = PlayerPrefs.GetString("profilePic", "default");
            username = PlayerPrefs.GetString("username", "New Player");
            uuid = PlayerPrefs.GetString("uuid", GenerateUUID());
            usernameInputfield.text = username;
            uuidText.text = uuid;
        }

        public string GenerateUUID()
        {
            char[] characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            string code = "";
            for(int i = 0; i < 6; i++)
            {
                code += characters[Random.Range(0, characters.Length)];
            }
            code += "-";
            for (int i = 0; i < 6; i++)
            {
                code += characters[Random.Range(0, characters.Length)];
            }
            code += "-";
            for (int i = 0; i < 6; i++)
            {
                code += characters[Random.Range(0, characters.Length)];
            }
            return code;
        }

        public string GenerateBotUsername()
        {
            List<string> nameList = new List<string>{ "Milo", "Justin", "Pierce", "Ethan", "Emily", "Julian", "Brandon" };
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0; i < nameList.Count; i++)
            {
                for(int j = 0; j < players.Length; j++)
                {
                    if(nameList[i] == players[j].GetComponent<Player>().Username.Value)
                    {
                        nameList.Remove(nameList[i]);
                        i--;
                        break;
                    }
                }
            }
            return nameList[Random.Range(0, nameList.Count)];
        }
    }
}
