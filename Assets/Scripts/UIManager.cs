using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCanvas;
    [SerializeField] private GameObject userDashboardCanvas;
    [SerializeField] private GameObject userDashboard;
    [SerializeField] private GameObject userInventory;
    [SerializeField] private GameObject userInventoryAddItem;
    [SerializeField] private TMP_InputField userInventoryNameInput;
    [SerializeField] private TMP_InputField userInventoryTypeInput;
    [SerializeField] private GameObject userLog;

    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject registerUI;

    public TMP_Text loadedFullName;
    public TMP_Text loadedEmail;
    public TMP_Text loadedPhone;
    public TMP_Text loadedBirthday;
    public TMP_Text loadedStreetname;
    public TMP_Text loadedCity;
    public TMP_Text loadedZipcode;


    public static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    #region Login/Register screen controls

    /// <summary>
    /// Turn off all UI related to the login and register.
    /// </summary>
    public void ClearLoginScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }

    /// <summary>
    /// Opens the login screen.
    /// </summary>
    public void LoginScreen()
    {
        ClearLoginScreen();
        loginUI.SetActive(true);
    }

    /// <summary>
    /// Opens the register screen.
    /// </summary>
    public void RegisterScreen()
    {
        ClearLoginScreen();
        registerUI.SetActive(true);
    }

    #endregion

    #region Main application controls

    /// <summary>
    /// Clears the text in the personal log, but does not save it.
    /// </summary>
    public void ClearPersonalLog()
    {
        PlayerPrefsControl.Instance.personalLog.text = "";
    }



    /// <summary>
    /// Toggles the inventory's "add item" menu.
    /// </summary>
    public void OpenAddItemToggle()
    {
        if (userInventoryAddItem.activeSelf)
        {
            userInventoryAddItem.SetActive(false);
            ClearInventoryAddItemText();
        }
        else
        {
            userInventoryAddItem.SetActive(true);
        }
    }

    public void ClearInventoryAddItemText()
    {
        userInventoryNameInput.text = "";
        userInventoryTypeInput.text = "";
    }



    /// <summary>
    /// Turn off all UI related to the dashboard and personal log.
    /// </summary>
    public void ClearDashboardScreen()
    {
        userDashboard.SetActive(false);
        userInventory.SetActive(false);
        userLog.SetActive(false);
    }

    /// <summary>
    /// Opens the dashboard screen.
    /// </summary>
    public void DashboardScreen()
    {
        ClearDashboardScreen();
        userDashboard.SetActive(true);
    }

    /// <summary>
    /// Opens the inventory screen.
    /// </summary>
    public void InventoryScreen()
    {
        ClearDashboardScreen();
        userInventory.SetActive(true);
    }

    /// <summary>
    /// Opens the personal log screen.
    /// </summary>
    public void PersonalLogScreen()
    {
        ClearDashboardScreen();
        PlayerPrefsControl.Instance.LoadPersonalLog();
        userLog.SetActive(true);
    }

    #endregion

    #region Enabling and disabling of canvases

    /// <summary>
    /// Enables the start screen.
    /// </summary>
    public void EnableStartScreen()
    {
        ClearLoginScreen();
        startScreenCanvas.SetActive(true);
    }

    /// <summary>
    /// Disables the start screen.
    /// </summary>
    public void DisableStartScreen()
    {
        ClearLoginScreen();
        startScreenCanvas.SetActive(false);
    }

    /// <summary>
    /// Enable the user dashboard.
    /// </summary>
    public void EnableUserDashboard()
    {
        ClearLoginScreen();
        userDashboardCanvas.SetActive(true);
    }

    /// <summary>
    /// Disables the user dashboard.
    /// </summary>
    public void DisableUserDashboard()
    {
        ClearLoginScreen();
        userDashboardCanvas.SetActive(true);
    }
    #endregion
}
