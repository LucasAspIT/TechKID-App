using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class FirebaseManager : MonoBehaviour
{
    // Firebase variables
    [Header("Firebase")]
    [SerializeField] DependencyStatus dependencyStatus;
    [SerializeField] FirebaseAuth auth;
    [SerializeField] FirebaseUser User;
    [SerializeField] DatabaseReference DBreference;

    // Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public Toggle saveLoginEmail;
    public Toggle saveLoginPassword;
    [SerializeField] TMP_Text warningLoginText;
    [SerializeField] TMP_Text confirmLoginText;

    // Register variables
    [Header("Register")]
    [SerializeField] TMP_InputField fullNameRegisterField;
    [SerializeField] TMP_InputField emailRegisterField;
    [SerializeField] TMP_InputField phoneRegisterField;
    [SerializeField] TMP_InputField birthdayRegisterField;
    [SerializeField] TMP_InputField streetnameRegisterField;
    [SerializeField] TMP_InputField cityRegisterField;
    [SerializeField] TMP_InputField zipcodeRegisterField;
    [SerializeField] TMP_InputField passwordRegisterField;
    [SerializeField] TMP_InputField passwordRegisterVerifyField;
    [SerializeField] TMP_Text warningRegisterText;

    private static FirebaseManager instance;
    public static FirebaseManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        // Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void Start()
    {
        PlayerPrefsControl.Instance.UseLoginInformation();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        // Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// Function for login button.
    /// </summary>
    public void LoginButton()
    {
        PlayerPrefsControl.Instance.SaveLoginToggleState();
        PlayerPrefsControl.Instance.SaveLoginInformation();
        // Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    /// <summary>
    /// Function for the register button.
    /// </summary>
    public void RegisterButton()
    {
        // Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, fullNameRegisterField.text, phoneRegisterField.text, birthdayRegisterField.text, streetnameRegisterField.text, cityRegisterField.text, zipcodeRegisterField.text));
    }

    /// <summary>
    /// Function for the sign out button.
    /// </summary>
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.Instance.DisableUserDashboard();
        UIManager.Instance.EnableStartScreen();
        UIManager.Instance.LoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
        Debug.Log("User signed out successfully.");
    }

    /// <summary>
    /// Function for button that saves the user's log content to the DB.
    /// </summary>
    public void SaveDataButton()
    {
        // StartCoroutine(UpdateUsernameAuth(usernameField.text)); // ### Change this to the relevant function when it has been created
    }

    /// <summary>
    /// Function for button that loads the data from the DB and goes to the user's log.
    /// </summary>
    public void UserLogButton()
    {
        // StartCoroutine(LoadUserLogData()); // ### Change this to the relevant function when it has been created
    }

    /// <summary>
    /// Clears the fields on the login UI.
    /// </summary>
    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    /// <summary>
    /// Clears the text fields on the Register UI.
    /// </summary>
    public void ClearRegisterFields()
    {
        fullNameRegisterField.text = "";
        emailRegisterField.text = "";
        phoneRegisterField.text = "";
        birthdayRegisterField.text = "";
        streetnameRegisterField.text = "";
        cityRegisterField.text = "";
        zipcodeRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    /// <summary>
    /// Sends the input information to Firebase to attempt a login.
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <returns></returns>
    private IEnumerator Login(string _email, string _password)
    {
        // Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        // Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            // User is now logged in
            // Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";

            yield return new WaitForSeconds(2);

            confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
            StartCoroutine(LoadUserData());
            UIManager.Instance.DisableStartScreen();
            UIManager.Instance.EnableUserDashboard();
        }
    }

    /// <summary>
    /// Sends the input information to Firebase for to attempt to register.
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <param name="_username"></param>
    /// <returns></returns>
    private IEnumerator Register(string _email, string _password, string _fullName, string _phone, string _birthday, string _streetname, string _city, string _zipcode)
    {
        if (_fullName == "")
        {
            // If the fullname field is blank show a warning
            warningRegisterText.text = "Missing full name";
        }
        else if (_birthday == "")
        {
            // If the birthday field is blank show a warning
            warningRegisterText.text = "Missing birthday";
        }
        else if (_phone == "")
        {
            // If the phone field is blank show a warning
            warningRegisterText.text = "Missing phone number";
        }
        else if (_streetname == "")
        {
            // If the streetname field is blank show a warning
            warningRegisterText.text = "Missing street name";
        }
        else if (_city == "")
        {
            // If the city field is blank show a warning
            warningRegisterText.text = "Missing city";
        }
        else if (_zipcode == "")
        {
            // If the zipcode field is blank show a warning
            warningRegisterText.text = "Missing zipcode";
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            // If the password does not match show a warning
            warningRegisterText.text = "Password does not match!";
        }
        else 
        {
            // Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            // Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                // If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email already in use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                // User has now been created
                // Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    // Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _fullName};

                    // Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    // Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        // If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Full name set failed!";
                    }
                    else
                    {
                        // Set the initial database values
                        StartCoroutine(SaveFullNameDatabase(_fullName));
                        StartCoroutine(SaveEmailDatabase(_email));
                        StartCoroutine(SavePhoneDatabase(_phone));
                        StartCoroutine(SaveBirthdayDatabase(_birthday));
                        StartCoroutine(SaveStreetnameDatabase(_streetname));
                        StartCoroutine(SaveCityDatabase(_city));
                        StartCoroutine(SaveZipcodeDatabase(_zipcode));
                        StartCoroutine(SaveRegisterDateDatabase());

                        // Now return to login screen
                        UIManager.Instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearLoginFields();
                        ClearRegisterFields();
                    }
                }
            }
        }
    }



    private IEnumerator SaveFullNameDatabase(string _fullName)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("fullname").SetValueAsync(_fullName);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database fullname is now registered.
        }
    }

    private IEnumerator SaveEmailDatabase(string _email)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("email").SetValueAsync(_email);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database email is now registered.
        }
    }

    private IEnumerator SavePhoneDatabase(string _phone)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("phone").SetValueAsync(_phone);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database phone is now registered.
        }
    }

    private IEnumerator SaveBirthdayDatabase(string _birthday)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("birthday").SetValueAsync(_birthday);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database birthday is now registered.
        }
    }

    private IEnumerator SaveStreetnameDatabase(string _streetname)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("streetname").SetValueAsync(_streetname);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database streetname is now registered.
        }
    }

    private IEnumerator SaveCityDatabase(string _city)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("city").SetValueAsync(_city);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database city is now registered.
        }
    }

    private IEnumerator SaveZipcodeDatabase(string _zipcode)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("zipcode").SetValueAsync(_zipcode);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database zipcode is now registered.
        }
    }

    private IEnumerator SaveRegisterDateDatabase()
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("registerdate").SetValueAsync(DateTime.Now.ToString("dd/MM/yyyy"));

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database register date is now registered.
        }
    }



    /// <summary>
    /// Loads the users data from the DB and saves it for later use.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadUserData()
    {
        // Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.LogWarning("LoadUserData() DBTask.Result.Value returned null!");
        }
        else
        {
            // Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            // Save the selected info
            UIManager.Instance.loadedFullName.text = $"Name: {snapshot.Child("fullname").Value}".ToString();
            UIManager.Instance.loadedEmail.text = $"E-mail: {snapshot.Child("email").Value}".ToString();
            UIManager.Instance.loadedPhone.text = $"Phone: {snapshot.Child("phone").Value}".ToString();
            UIManager.Instance.loadedBirthday.text = $"Birthday: {snapshot.Child("birthday").Value}".ToString();
            UIManager.Instance.loadedStreetname.text = $"Streetname: {snapshot.Child("streetname").Value}".ToString();
            UIManager.Instance.loadedCity.text = $"City: {snapshot.Child("city").Value}".ToString();
            UIManager.Instance.loadedZipcode.text = $"Zipcode: {snapshot.Child("zipcode").Value}".ToString();
        }
    }
}
