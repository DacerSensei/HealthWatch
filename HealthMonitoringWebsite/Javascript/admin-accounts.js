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


const searchTextBoxElement = document.getElementById("ContentSearch");
const searchButtonElement = document.getElementById("SearchButton");
const tableBody = document.getElementById("table-body");
searchButtonElement.addEventListener("click", async (event) => {
    event.preventDefault();
    ShowLoading();
    tableBody.innerHTML = "";
    if (searchTextBoxElement.value.length == 0) {
        await AdminAccount();
        HideLoading();
        return;
    }
    SearchData(searchTextBoxElement.value.toLowerCase())
    HideLoading();
});

async function SearchData(text) {
    try {
        await get(child(ref(Database), 'admins')).then(async (adminSnapshot) => {
            if (await adminSnapshot.exists()) {
                const data = adminSnapshot.val();
                if (data) {
                    for (const [key, values] of Object.entries(data)) {
                        if (values.Name.toLowerCase().startsWith(text) || values.Name.toLowerCase().includes(text)) {
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
            }
        }).catch((error) => {
            console.log(error);
        });
    } catch (error) {
        console.log(error);
    }
}