using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    public MainManager mm;

    private List<QuestionModel> questionList = new List<QuestionModel>();

    public class QuestionModel
    {
        public string name;

        public QuestionModel()
        {
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onbtnGetStarted()
    {
        if (questionList.Count == 0)
        {
            mm.ShowAlertPopup("Please select favorite item.");
            return;
        }

        UpdateUser(questionList);
    }

    public void UpdateUser(List<QuestionModel> questionList)
    {
        string question_list = "";
        for(int i=0; i< questionList.Count; i++)
        {
            if (i > 0) question_list += ",";
            question_list += questionList[i].name;
        }

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_id", Global.m_user.id.ToString()));
        formData.Add(new MultipartFormDataSection("question_list", UnityWebRequest.EscapeURL(question_list)));

        string requestURL = Global.DOMAIN + "/API/SetQuestionList.aspx";
        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        StartCoroutine(ResponseUpdateUser(www, question_list));
    }

    IEnumerator ResponseUpdateUser(UnityWebRequest www, string question_list)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            mm.isQuit = true;
            mm.ShowAlertPopup("Please confirm internet.");
            yield break;
        }

        string resultData = www.downloadHandler.text;
        if (string.IsNullOrEmpty(resultData))
        {
            mm.ShowAlertPopup("Server api error!");
            yield break;
        }

        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            string resText = json["responseText"].ToString();
            mm.ShowAlertPopup(resText);
            yield break;
        }

        Global.m_user.question_list = question_list;

        PlayerPrefs.SetInt("auto_login", 1);
        PlayerPrefs.Save();
        Global.SaveUserInfo(Global.m_user);

        mm.gotoDashboard();
    }


    public void addQuestionItem(string name)
    {
        QuestionModel fn = new QuestionModel();
        fn.name = name;

        if(questionList.Find(x => x.name == name) == null)
        {
            questionList.Add(fn);
        }
    }

    public void removeQuestionItem(string name)
    {
        for(int i=0; i< questionList.Count; i++)
        {
            if(questionList[i].name == name)
            {
                questionList.RemoveAt(i);
                break;
            }
        }
    }
}
