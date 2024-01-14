import {
    initializeApp
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-app.js";
import {
    getAuth, createUserWithEmailAndPassword, signOut
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-auth.js";
import {
    getDatabase, ref, set, onValue, get, child
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-database.js";
import { Database, GetElementValue, FirebaseConfig, AdminAccount } from "./main.js";

const SecondaryApp = initializeApp(FirebaseConfig, "SecondaryApp");
const SecondaryAuth = getAuth(SecondaryApp);

document.getElementById("admin-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const name = GetElementValue("name") ?? "";
    const email = GetElementValue("email") ?? "";
    const password = GetElementValue("password") ?? "";
    if (name == "" || email == "" || password == "") {
        ShowNotification("Please fill up all the required data", Colors.Red);
        return;
    }
    const conpassword = GetElementValue('confirm-password');
    if (password != conpassword) {
        ShowNotification("Your password and confirm password didn't match", Colors.Red);
        return;
    }
    ShowLoading();
    await createUserWithEmailAndPassword(SecondaryAuth, email, password).then(async (userCredential) => {
        console.log(userCredential.uid);
        await set(ref(Database, "admins/" + userCredential.user.uid), {
            'Email': email,
            'Name': name,
            'Status': "active"
        });
        signOut(SecondaryAuth).then(() => {
        }).catch((error) => {
            console.log(error.message);
        });
        HideLoading();
        ShowPopup("You just created a new account");
        document.getElementById("admin-form").reset();
        document.querySelector('.modal-close').click();
        document.getElementById("table-body").innerHTML = "";
        await AdminAccount();
    }).catch((error) => {
        HideLoading();
        if (error.code == "auth/email-already-in-use") {
            ShowPopup("Email already in use");
        } else if (error.code == "auth/invalid-email") {
            ShowPopup("Invalid Email");
        } else if (error.code == "auth/weak-password") {
            ShowPopup("Your password is too weak");
        } else {
            ShowPopup(error.message);
        }
    });
    HideLoading();
});