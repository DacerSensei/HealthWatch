import {
    initializeApp
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-app.js";
import {
    getAuth, createUserWithEmailAndPassword, signInWithEmailAndPassword, sendPasswordResetEmail, onAuthStateChanged, signOut
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-auth.js";
import {
    getDatabase, ref, set, onValue, get, child
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-database.js";

const FirebaseConfig = {
    apiKey: "AIzaSyDt250uSsgG5HFI2eIWcQivHEgw2HtKi2A",
    authDomain: "healthwatch-f6e17.firebaseapp.com",
    databaseURL: "https://healthwatch-f6e17-default-rtdb.asia-southeast1.firebasedatabase.app",
    projectId: "healthwatch-f6e17",
    storageBucket: "healthwatch-f6e17.appspot.com",
    messagingSenderId: "928320262772",
    appId: "1:928320262772:web:74ba85a46a9eb3d0bff721"
};

const App = initializeApp(FirebaseConfig);
const Auth = getAuth(App);
const Database = getDatabase(App);


async function RenderElementInBody(link, elementType, isAppend = true) {
    const response = await fetch(link);
    const responseHTML = await response.text();
    const tempContainer = document.createElement(elementType);
    tempContainer.innerHTML = responseHTML;
    const body = document.body;
    if (isAppend) {
        body.append(...tempContainer.childNodes);
    } else {
        body.prepend(...tempContainer.childNodes);
    }
}

async function RenderElementById(link, elementType, id, isAppend = true) {
    const response = await fetch(link);
    const responseHTML = await response.text();
    const tempContainer = document.createElement(elementType);
    tempContainer.innerHTML = responseHTML;
    const element = document.getElementById(id);
    if (element) {
        if (isAppend) {
            element.append(...tempContainer.childNodes);
        } else {
            element.prepend(...tempContainer.childNodes);
        }
    }
}

(async () => {
    RenderElementInBody("/Library/loading.html", "div", false);
    RenderElementInBody("/Library/notification.html", "div");
    if (!window.location.pathname.includes("/index.html")) {
        await RenderElementInBody("/Library/header.html", "header", false);
        await RenderElementById("/Library/sidebar.html", "aside", "main", false)
        const links = document.querySelectorAll(".nav-links");
        const currentURL = window.location.href;
        links.forEach((link) => {
            if (link.href === currentURL) {
                link.classList.add("NavigationActive");
            } else {
                link.classList.remove("NavigationActive");
            }
        });
        Logout();
        const storedData = localStorage.getItem('adminData');
        if (storedData) {
            const parsedData = JSON.parse(storedData);
            SetContentByQuerySelector(".user-profile-name", parsedData.Name);
        }
        const HeaderButtons = document.getElementsByClassName("header-button");
        if (HeaderButtons) {
            if (HeaderButtons.length > 0) {
                for (let i = 0; i < HeaderButtons.length; i++) {
                    HeaderButtons[i].onclick = function () {

                        for (let j = 0; j < HeaderButtons.length; j++) {
                            let list = HeaderButtons[j].parentNode.getElementsByTagName("ul")[0].classList;
                            if (i == j) continue;
                            if (list.contains("Show")) {
                                list.remove("Show");
                            }
                        }
                        HeaderButtons[i].parentNode.getElementsByTagName("ul")[0].classList.toggle("Show");
                    }
                }
            }
        }
        const SideMenuButton = document.querySelector(".side-menu-collapse");
        if (SideMenuButton) {
            if (SideMenuButton != undefined) {
                SideMenuButton.onclick = SideBarMenuToggleEvent;
            }
        }
        const DropdownButton = document.querySelectorAll(".DropdownMenu");
        if (DropdownButton) {
            function AttachSideMenuEvent() {
                if (DropdownButton.length > 0) {
                    DropdownButton.forEach(button => {
                        let list = button.querySelector(".DropdownMenuList");
                        button.querySelector(".DropdownButton").onclick = function () {
                            list.classList.toggle("show")
                            if (list.classList.contains("show")) {
                                list.style.maxHeight = list.scrollHeight + "px";
                            } else {
                                list.style.maxHeight = 0 + "px";
                            }
                        }
                    });
                }
            }
            AttachSideMenuEvent();
        }
        ShowLoading();
        if (window.location.pathname.includes("/dashboard.html")) {
            await DashboardLoaded();
        }
        else if (window.location.pathname.includes("/admin-accounts.html")) {
            await AdminAccount();
        }
        else if (window.location.pathname.includes("/patient-accounts.html")) {
            await PatientAccount();
        }
    } else {
        Login();
    }
    HideLoading();
})();

function SideBarMenuToggleEvent() {
    const sideMenu = document.querySelector("#Side-Menu-Container")
    const logo = document.querySelector("#Logo")
    const root = document.querySelector(':root');
    const liElements = document.querySelectorAll("#MainNavigation > ul > li");
    if (getComputedStyle(document.documentElement).getPropertyValue('--sidebar-toggle') == '16rem') {
        root.style.setProperty('--sidebar-toggle', '4rem');
        document.querySelector("#MainNavigation > h4").style.display = "none";
        if (logo) {
            logo.lastElementChild.style.display = "none";
        }
        liElements.forEach(function (li) {
            li.querySelector("span").style.display = "none";
            if (li.classList.contains("DropdownMenu")) {
                const list = li.querySelector(".DropdownMenuList");
                if (li.querySelector(".DropdownMenuList").classList.contains("show")) {
                    li.querySelector(".DropdownMenuList").classList.remove("show");
                }
                li.querySelector(".DropdownButton").onclick = null;
                list.style.maxHeight = 0 + "px";
                list.classList.add("absolute");
                li.classList.add("flex");
                li.querySelector("svg:nth-of-type(2)").style.display = "none";
                li.onmouseover = function () {
                    list.style.maxHeight = list.scrollHeight + "px";
                }
                li.onmouseout = function () {
                    list.style.maxHeight = 0 + "px";
                }
            }
        });
    } else {
        root.style.setProperty('--sidebar-toggle', '16rem');
        document.querySelector("#MainNavigation > h4").style.display = "block";
        liElements.forEach(function (li) {
            if (li.classList.contains("DropdownMenu")) {
                let list = li.querySelector(".DropdownMenuList");
                list.classList.remove("absolute");
                li.classList.remove("flex");
                li.onmouseover = null;
                li.onmouseout = null;
            }
        });
        setTimeout(() => {
            liElements.forEach(function (li) {
                li.querySelector("span").style.display = "block";
                if (logo) {
                    logo.lastElementChild.style.display = "block";
                }
                if (li.classList.contains("DropdownMenu")) {
                    AttachSideMenuEvent();
                    li.querySelector("svg:nth-of-type(2)").style.display = "block";
                }
            });
        }, 300);
    }
}

async function DashboardLoaded() {
    try {
        await get(child(ref(Database), 'admins')).then(async (adminSnapshot) => {
            if (await adminSnapshot.exists()) {
                const data = adminSnapshot.val();
                if (data) {
                    SetContentById("AdminAccounts", Object.entries(data).length);
                }
            } else {
                ShowNotification("Something went wrong", Colors.Red);
            }
        }).catch((error) => {
            console.log(error);
        });
        await get(child(ref(Database), 'users')).then(async (studentSnapshot) => {
            if (await studentSnapshot.exists()) {
                const data = studentSnapshot.val();
                if (data) {
                    SetContentById("StudentAccounts", Object.entries(data).length);
                }
            } else {
                ShowNotification("Something went wrong", Colors.Red);
            }
        }).catch((error) => {
            console.log(error);
        });
    } catch (error) {
        console.log(error);
    }
}

async function AdminAccount() {
    try {
        await get(child(ref(Database), 'admins')).then(async (adminSnapshot) => {
            if (await adminSnapshot.exists()) {
                const data = adminSnapshot.val();
                if (data) {
                    const tableBody = document.getElementById("table-body");
                    for (const [key, values] of Object.entries(data)) {
                        const row = document.createElement("tr");
                        const hiddenInput = document.createElement("input");
                        hiddenInput.type = "hidden";
                        hiddenInput.value = key;

                        const nameCell = document.createElement("td");
                        nameCell.textContent = values.Name;

                        const emailCell = document.createElement("td");
                        emailCell.textContent = values.Email;

                        const statusCell = document.createElement("td");
                        const statusDiv = document.createElement("div");
                        statusDiv.textContent = values.Status.toUpperCase();
                        if (values.Status.toLowerCase() == "active") {
                            statusDiv.className = "Status-Green";
                        } else {
                            statusDiv.className = "Status-Red";
                        }

                        const actionsCell = document.createElement("td");
                        const editButton = document.createElement("button");
                        editButton.classList.add('Button-Blue-Icon', 'modal-trigger');
                        editButton.title = "Edit";
                        editButton.setAttribute('data-target', 'Edit-Modal');
                        editButton.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" style="pointer-events: none;"><path d="M14.06,9L15,9.94L5.92,19H5V18.08L14.06,9M17.66,3C17.41,3 17.15,3.1 16.96,3.29L15.13,5.12L18.88,8.87L20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18.17,3.09 17.92,3 17.66,3M14.06,6.19L3,17.25V21H6.75L17.81,9.94L14.06,6.19Z"/></svg>';

                        const disableButton = document.createElement("button");
                        disableButton.className = "Button-Red-Icon";
                        disableButton.style = "margin-left: 4px;"
                        disableButton.title = "Disable";
                        disableButton.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 -960 960 960" width="24" style="pointer-events: none;"><path d="M538-538ZM424-424Zm56 264q51 0 98-15.5t88-44.5q-41-29-88-44.5T480-280q-51 0-98 15.5T294-220q41 29 88 44.5t98 15.5Zm106-328-57-57q5-8 8-17t3-18q0-25-17.5-42.5T480-640q-9 0-18 3t-17 8l-57-57q19-17 42.5-25.5T480-720q58 0 99 41t41 99q0 26-8.5 49.5T586-488Zm228 228-58-58q22-37 33-78t11-84q0-134-93-227t-227-93q-43 0-84 11t-78 33l-58-58q49-32 105-49t115-17q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 59-17 115t-49 105ZM480-80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-59 16.5-115T145-701L27-820l57-57L876-85l-57 57-615-614q-22 37-33 78t-11 84q0 57 19 109t55 95q54-41 116.5-62.5T480-360q38 0 76 8t74 22l133 133q-57 57-130 87T480-80Z"/></svg>';

                        actionsCell.appendChild(editButton);
                        actionsCell.appendChild(disableButton);

                        statusCell.appendChild(statusDiv)

                        row.appendChild(hiddenInput);
                        row.appendChild(nameCell);
                        row.appendChild(emailCell);
                        row.appendChild(statusCell);
                        row.appendChild(actionsCell);

                        tableBody.appendChild(row);
                    }
                }
            }
        }).catch((error) => {
            console.log(error);
        });
    } catch (error) {
        console.log(error);
    }
}

async function PatientAccount() {
    try {
        await get(child(ref(Database), 'users')).then(async (userSnapshot) => {
            if (await userSnapshot.exists()) {
                const data = userSnapshot.val();
                if (data) {
                    const tableBody = document.getElementById("table-body");
                    for (const [key, values] of Object.entries(data)) {
                        const row = document.createElement("tr");
                        const hiddenInput = document.createElement("input");
                        hiddenInput.type = "hidden";
                        hiddenInput.value = key;

                        const nameCell = document.createElement("td");
                        nameCell.textContent = values.CompleteName;

                        const birthdayCell = document.createElement("td");
                        birthdayCell.textContent = values.Birthday;

                        const genderCell = document.createElement("td");
                        const genderDiv = document.createElement("div");
                        genderDiv.textContent = values.Gender;
                        if (values.Gender.toLowerCase() == "Male".toLowerCase()) {
                            genderDiv.className = "Status-Blue";
                        } else {
                            genderDiv.className = "Status-Pink";
                        }

                        const connectionCell = document.createElement("td");
                        const connectionDiv = document.createElement("div");
                        connectionDiv.textContent = "Unknown";
                        connectionDiv.className = "Status-Red";

                        const actionsCell = document.createElement("td");
                        const editButton = document.createElement("button");
                        editButton.classList.add('Button-Blue-Icon', 'modal-trigger');
                        editButton.title = "Edit";
                        editButton.setAttribute('data-target', 'Edit-Modal');
                        editButton.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" style="pointer-events: none;"><path d="M14.06,9L15,9.94L5.92,19H5V18.08L14.06,9M17.66,3C17.41,3 17.15,3.1 16.96,3.29L15.13,5.12L18.88,8.87L20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18.17,3.09 17.92,3 17.66,3M14.06,6.19L3,17.25V21H6.75L17.81,9.94L14.06,6.19Z"/></svg>';

                        // const disableButton = document.createElement("button");
                        // disableButton.className = "Button-Red-Icon";
                        // disableButton.style = "margin-left: 4px;"
                        // disableButton.title = "Disable";
                        // disableButton.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 -960 960 960" width="24" style="pointer-events: none;"><path d="M538-538ZM424-424Zm56 264q51 0 98-15.5t88-44.5q-41-29-88-44.5T480-280q-51 0-98 15.5T294-220q41 29 88 44.5t98 15.5Zm106-328-57-57q5-8 8-17t3-18q0-25-17.5-42.5T480-640q-9 0-18 3t-17 8l-57-57q19-17 42.5-25.5T480-720q58 0 99 41t41 99q0 26-8.5 49.5T586-488Zm228 228-58-58q22-37 33-78t11-84q0-134-93-227t-227-93q-43 0-84 11t-78 33l-58-58q49-32 105-49t115-17q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 59-17 115t-49 105ZM480-80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-59 16.5-115T145-701L27-820l57-57L876-85l-57 57-615-614q-22 37-33 78t-11 84q0 57 19 109t55 95q54-41 116.5-62.5T480-360q38 0 76 8t74 22l133 133q-57 57-130 87T480-80Z"/></svg>';

                        actionsCell.appendChild(editButton);
                        // actionsCell.appendChild(disableButton);

                        genderCell.appendChild(genderDiv)
                        connectionCell.appendChild(connectionDiv)

                        row.appendChild(hiddenInput);
                        row.appendChild(nameCell);
                        row.appendChild(birthdayCell);
                        row.appendChild(genderCell);
                        row.appendChild(connectionCell);
                        row.appendChild(actionsCell);

                        tableBody.appendChild(row);

                        onValue(ref(Database, "users/" + key + "/DataSensors"), (sensorSnapshot) => {
                            const data = sensorSnapshot.val();
                            connectionDiv.textContent = data["SmartWatchStatus"];
                            if (data["SmartWatchStatus"].toLowerCase() == "connected") {
                                connectionDiv.className = "Status-Green";
                            } else {
                                connectionDiv.className = "Status-Red";
                            }
                        });
                    }
                }
            }
        }).catch((error) => {
            console.log(error);
        });
    } catch (error) {
        console.log(error);
    }
}

document.addEventListener("DOMContentLoaded", function () {
    onAuthStateChanged(Auth, async (userCredentials) => {
        if (userCredentials) {
            if (window.location.pathname === "/index.html") {
                ShowLoading();
                if (!localStorage.getItem('adminData')) {
                    const userId = userCredentials.uid;
                    await get(child(ref(Database), 'admins/' + userId)).then(async (userSnapshot) => {
                        if (await userSnapshot.exists()) {
                            const userData = userSnapshot.val();
                            localStorage.setItem('adminData', JSON.stringify(userData));
                            window.location.href = "/dashboard.html";
                        } else {
                            ShowNotification("Your account is not admin", Colors.Red);
                            signOut(Auth).then(() => {
                            }).catch((error) => {
                                console.log(error.message);
                            });
                        }
                    }).catch((error) => {
                        console.log(error);
                    });
                }
                window.location.href = "/dashboard.html";
                HideLoading();
            }
        } else {
            if (window.location.pathname !== "/index.html") {
                window.location.href = "/index.html";
            }
        }
    });
});

function Login() {
    const loginForm = document.getElementById("login-form");
    if (loginForm) {
        loginForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const email = GetElementValue("email") ?? "";
            const password = GetElementValue("password") ?? "";

            if (email == "" || password == "") {
                ShowNotification("Email or password cannot be empty", Colors.Red);
                return;
            }
            ShowLoading();
            let isAdmin = false;
            await get(child(ref(Database), 'admins')).then(async (userSnapshot) => {
                if (await userSnapshot.exists()) {
                    const data = userSnapshot.val();
                    if (data) {
                        for (const [key, values] of Object.entries(data)) {
                            if (values.Email == email) {
                                isAdmin = true;
                                break;
                            }
                        }
                    }
                }
            }).catch((error) => {
                console.log(error);
            });
            if (isAdmin) {
                await signInWithEmailAndPassword(Auth, email, password).then(async (userCredential) => {
                }).catch((error) => {
                    console.log(error.message);
                    ShowNotification("Your email or password is incorrect.", Colors.Red);
                    HideLoading();
                });
            } else {
                ShowNotification("Your account is not admin", Colors.Red);
                HideLoading();
            }
        });
    }
}

function Logout() {
    const logoutButton = document.getElementById("LogoutButton");
    if (logoutButton) {
        logoutButton.addEventListener("click", () => {
            ShowLoading();
            signOut(Auth).then(() => {
                localStorage.clear();
            }).catch((error) => {
                console.log(error.message);
            });
            HideLoading();
        });
    }
}

function GetElementValue(id) {
    return document.getElementById(id).value;
}

function SetContentById(id, textContent) {
    return document.getElementById(id).textContent = textContent;
}

function SetContentByQuerySelector(querySelector, textContent) {
    if (querySelector) {
        return document.querySelector(querySelector).textContent = textContent;
    }
    return;
}

function IsNullOrEmpty(string) {
    if (string == "" || string == null || string == undefined) {
        return true;
    }
    return false;
}

export {
    Auth,
    Database,
    FirebaseConfig,
    GetElementValue,
    AdminAccount,
    PatientAccount,
    IsNullOrEmpty
};