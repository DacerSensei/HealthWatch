import {
    initializeApp
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-app.js";
import {
    getAuth, createUserWithEmailAndPassword, signOut
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-auth.js";
import {
    getDatabase, ref, set, onValue, get, child
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-database.js";
import { Database, GetElementValue, FirebaseConfig, StudentAccount, IsNullOrEmpty } from "./main.js";

const SecondaryApp = initializeApp(FirebaseConfig, "SecondaryApp");
const SecondaryAuth = getAuth(SecondaryApp);

document.getElementById("grade-level").addEventListener("change", async () => {
    const gradeElement = document.getElementById("grade-level");
    const teacherElement = document.getElementById("teacher");
    const teacherItems = teacherElement.querySelector("picker-item-container-component");

    teacherItems.innerHTML = "";

    const pickerItemDefault = document.createElement("picker-item-component");
    pickerItemDefault.setAttribute("value", "None");
    pickerItemDefault.textContent = "Select Teacher";
    teacherItems.append(pickerItemDefault);

    const gradeLevel = (gradeElement === null) ? "None" : gradeElement.SelectedItem.value;
    ShowLoading();
    try {
        await get(child(ref(Database), 'teachers')).then(async (teacherSnapshot) => {
            if (await teacherSnapshot.exists()) {
                const data = teacherSnapshot.val();
                console.log(data);
                if (data) {
                    for (const [key, values] of Object.entries(data)) {
                        if (values.GradeLevel == gradeLevel) {
                            const pickerItem = document.createElement("picker-item-component");
                            pickerItem.setAttribute("value", key);
                            pickerItem.textContent = values.LastName + ", " + values.FirstName;
                            teacherItems.appendChild(pickerItem);
                        }
                    }
                }
            }
        }).catch((error) => {
            console.log(error);
        });
    } catch (error) {
        console.log(error);
    }
    HideLoading();
});

document.getElementById("student-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const gradeElement = document.getElementById("grade-level");
    const genderElement = document.getElementById("gender");
    const teacherElement = document.getElementById("teacher");
    
    const firstName = GetElementValue("first-name") ?? "";
    const lastName = GetElementValue("last-name") ?? "";
    const email = GetElementValue("email") ?? "";
    const password = GetElementValue("password") ?? "";
    const birthday = GetElementValue("birthday") ?? "";
    const contact = GetElementValue("contact") ?? "";
    const gradeLevel = (gradeElement === null) ? "None" : gradeElement.Value;
    const gender = (genderElement === null) ? "None" : genderElement.Value;
    const teacher = (teacherElement === null) ? "None" : teacherElement.Value;
    if (IsNullOrEmpty(firstName) || IsNullOrEmpty(lastName) || IsNullOrEmpty(email) || IsNullOrEmpty(password) || IsNullOrEmpty(birthday) || IsNullOrEmpty(contact)  || gradeLevel == "None" || gender == "None" || teacher == "none") {
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
        await set(ref(Database, "users/" + userCredential.user.uid), {
            'Email': email,
            'FirstName': firstName,
            'LastName': lastName,
            'Gender': gender,
            'Contact': contact,
            'Teacher': teacher,
            'Birthday': birthday,
            'Status': "active"
        });
        signOut(SecondaryAuth).then(() => {
        }).catch((error) => {
            console.log(error.message);
        });
        HideLoading();
        ShowPopup("You just created a new account");
        document.getElementById("student-form").reset();
        document.querySelector('.modal-close').click();
        gradeElement.SelectedItem = gradeElement.Items[0];
        genderElement.SelectedItem = genderElement.Items[0];
        teacherElement.SelectedItem = genderElement.Items[0];
        document.getElementById("table-body").innerHTML = "";
        await StudentAccount();
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