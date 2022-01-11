using UnityEngine;
using TMPro;

public class PlayerPrefsControl : MonoBehaviour
{
    public TMP_InputField personalLog;

    private static PlayerPrefsControl instance;

    public static PlayerPrefsControl Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Saves the text currently in the personal log.
    /// </summary>
    public void SavePersonalLog()
    {
        PlayerPrefs.SetString("SavedPersonalLog", personalLog.text);
    }

    public void LoadPersonalLog()
    {
        personalLog.text = PlayerPrefs.GetString("SavedPersonalLog", "");
    }

    /// <summary>
    /// Checks PlayerPrefs for saved login data, and inserts it if any has been saved:
    /// <para>Saved email and/or password.</para>
    /// <para>Saved toggle state for the checkboxes "Save email" and "Save password".</para>
    /// </summary>
    public void UseLoginInformation()
    {
        if (PlayerPrefs.GetInt("SavedEmailBool", 1) == 1)
        {
            FirebaseManager.Instance.emailLoginField.text = PlayerPrefs.GetString("SavedEmail", "");
            FirebaseManager.Instance.saveLoginEmail.isOn = true;
        }
        if (PlayerPrefs.GetInt("SavedPasswordBool", 1) == 1)
        {
            FirebaseManager.Instance.passwordLoginField.text = PlayerPrefs.GetString("SavedPassword", ""); // ########### THIS IS PROBABLY UNSAFE, BUT FINE FOR AN INTERNAL PRACTICE APP ONLY.
            FirebaseManager.Instance.saveLoginPassword.isOn = true;
        }
    }

    /// <summary>
    /// Saves the email and/or password locally via PlayerPrefs if the user has chosen to do so.
    /// </summary>
    public void SaveLoginInformation()
    {
        if (PlayerPrefs.GetInt("SavedEmailBool", 1) == 1)
        {
            PlayerPrefs.SetString("SavedEmail", FirebaseManager.Instance.emailLoginField.text);
        }
        else
        {
            PlayerPrefs.SetString("SavedEmail", "");
        }

        if (PlayerPrefs.GetInt("SavedPasswordBool", 1) == 1)
        {
            PlayerPrefs.SetString("SavedPassword", FirebaseManager.Instance.passwordLoginField.text); // ########### THIS IS PROBABLY UNSAFE, BUT FINE FOR AN INTERNAL PRACTICE APP ONLY.
        }
        else
        {
            PlayerPrefs.SetString("SavedPassword", "");
        }
    }

    /// <summary>
    /// Saves the login screen's checkboxes checked/unchecked states in PlayerPrefs.
    /// </summary>
    public void SaveLoginToggleState()
    {
        if (FirebaseManager.Instance.saveLoginEmail.isOn)
        {
            PlayerPrefs.SetInt("SavedEmailBool", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SavedEmailBool", 0);
        }

        if (FirebaseManager.Instance.saveLoginPassword.isOn)
        {
            PlayerPrefs.SetInt("SavedPasswordBool", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SavedPasswordBool", 0);
        }
    }
}
