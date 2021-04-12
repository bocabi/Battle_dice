using UnityEngine;
using UnityEngine.UI;
using Battlehub.Dispatcher;
using UnityEngine.SceneManagement;
public class LoginUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] CustomLogin = new GameObject[3];
    public GameObject[] CustomRegistar = new GameObject[3];
    private InputField[] inputFields = new InputField[4];
    private static LoginUI instance;
    private InputField NameField;
    public GameObject NickName;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        NickName.SetActive(false);
        NameField = NickName.GetComponent<InputField>();
        inputFields[0] = CustomLogin[0].GetComponent<InputField>();
        inputFields[1] = CustomLogin[1].GetComponent<InputField>();
        inputFields[2] = CustomRegistar[0].GetComponent<InputField>();
        inputFields[3] = CustomRegistar[1].GetComponent<InputField>();

    }

    public void _CustomLogin()
    {
        CustomLogin[0].SetActive(true);
        CustomLogin[1].SetActive(true);
        CustomLogin[2].SetActive(true);

    }
    public void _CustomLoginButton()
    {
        string id = inputFields[0].text;
        string pw = inputFields[1].text;

        ServerManager.GetInstance().CustomLogin(id, pw, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                  
                    return;
                }
                //ChangeLobbyScene();
            });
        });
    }

    public void _CustomRegistar()
    {
        CustomRegistar[0].SetActive(true);
        CustomRegistar[1].SetActive(true);
        CustomRegistar[2].SetActive(true);
    }
    public void _CustomRegistarButton()
    {

        string id = inputFields[2].text;
        string pw = inputFields[3].text;

        Debug.Log(id);
        Debug.Log(pw);
        ServerManager.GetInstance().CustomSignIn(id, pw, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    return;
                }
                //ChangeLobbyScene();
            });
        });
        CustomRegistar[0].SetActive(false);
        CustomRegistar[1].SetActive(false);
        CustomRegistar[2].SetActive(false);

    }
    public void GuestLogin()
    {

        ServerManager.GetInstance().GuestLogin((bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                Debug.Log("gd");

                if (!result)
                {

                    Debug.Log("로그인 에러");
                    return;
                }
                Debug.Log("gd");
            });
            //ChangeLobbyScene();
        });
    }

    public static LoginUI GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("LoginUI 인스턴스 없음");
            return null;
        }
        return instance;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void NickNameSet()
    {
        string nickname = NameField.text;
        if (nickname.Equals(string.Empty))
        {
            return;
        }

        ServerManager.GetInstance().UpdateNickname(nickname, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    return;
                }
                //ChangeLobbyScene();
            });
        });
    }
}
