using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRoidSDK;

namespace nkjzm.VMotion
{
    public class VRoidHubLogin : MonoBehaviour
    {
        [SerializeField]
        Button Login = null;
        [SerializeField]
        Button Register = null;
        [SerializeField]
        GameObject codeField;
        [SerializeField]
        InputField codeInputField;
        [SerializeField]
        SDKConfiguration sdkConfiguration;

        BrowserAuthorize browserAuthorize;

        private void Awake()
        {
            Authentication.Instance.Init(sdkConfiguration.AuthenticateMetaData);
            codeField.gameObject.SetActive(false);
            browserAuthorize = BrowserAuthorize.GenerateInstance(sdkConfiguration);
            Login.onClick.AddListener(OnLoginButtonClicked);
            Register.onClick.AddListener(OnRegisterCodeEnter);
        }

        void OnRegisterCodeEnter()
        {
            browserAuthorize.RegisterCode(codeInputField.text);
        }

        void AuthSuccessed()
        {
            Debug.Log("認証成功!");
            Destroy(browserAuthorize.gameObject);
            codeField.gameObject.SetActive(false);
            Login.gameObject.SetActive(false);
            FindObjectOfType<VRMLoader>().LoadFromVRoidHub();
        }

        void OnLoginButtonClicked()
        {
            Authentication.Instance.AuthorizeWithExistAccount((bool isAuthSuccess) =>
            {
                if (isAuthSuccess)
                {
                    AuthSuccessed();
                }
                else
                {
                    codeField.SetActive(true);
                    browserAuthorize.OpenBrowser(AfterBrowserAuthorize);
                }
            },
            (System.Exception e) =>
            {
                codeField.SetActive(true);
                browserAuthorize.OpenBrowser(AfterBrowserAuthorize);
            });
        }

        private void AfterBrowserAuthorize(bool isSuccess)
        {
            if (isSuccess)
            {
                AuthSuccessed();
            }
            else
            {
                codeField.SetActive(false);
            }
        }

    }
}