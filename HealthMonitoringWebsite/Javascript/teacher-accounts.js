import {
    initializeApp
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-app.js";
import {
    getAuth, createUserWithEmailAndPassword, signOut
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-auth.js";
import {
    getDatabase, ref, set, onValue, get, child
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-database.js";
import { Database, GetElementValue, FirebaseConfig, TeacherAccount, IsNullOrEmpty } from "./main.js";

const SecondaryApp = initializeApp(FirebaseConfig, "SecondaryApp");
const SecondaryAuth = getAuth(SecondaryApp);

document.getElementById("teacher-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const gradeElement = document.getElementById("grade-level");
    const genderElement = document.getElementById("gender");

    const firstname = GetElementValue("first-name") ?? "";
    const lastname = GetElementValue("last-name") ?? "";
    const contact = GetElementValue("contact") ?? "";
    const email = GetElementValue("email") ?? "";
    const password = GetElementValue("password") ?? "";
    const birthday = GetElementValue("birthday") ?? "";
    const gradeLevel = (gradeElement === null) ? "None" : gradeElement.Value;
    const gender = (genderElement === null) ? "None" : genderElement.Value;
    if (IsNullOrEmpty(firstname) || IsNullOrEmpty(lastname) || IsNullOrEmpty(contact) || IsNullOrEmpty(email) || IsNullOrEmpty(password) || IsNullOrEmpty(birthday) || gradeLevel == "None" || gender == "None") {
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
        await set(ref(Database, "teachers/" + userCredential.user.uid), {
            'Email': email,
            'FirstName': firstname,
            'LastName': lastname,
            'Contact': contact,
            'Gender': gender,
            'GradeLevel': gradeLevel,
            'Birthday': birthday,
            'Status': "active"
        });
        signOut(SecondaryAuth).then(() => {
        }).catch((error) => {
            console.log(error.message);
        });
        HideLoading();
        ShowPopup("You just created a new account");
        document.getElementById("teacher-form").reset();
        document.querySelector('.modal-close').click();
        gradeElement.SelectedItem = gradeElement.Items[0];
        genderElement.SelectedItem = genderElement.Items[0];
        document.getElementById("table-body").innerHTML = "";
        await TeacherAccount();
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