import {
    initializeApp
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-app.js";
import {
    getAuth, createUserWithEmailAndPassword, signOut
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-auth.js";
import {
    getDatabase, ref, set, onValue, get, child, push
} from "https://www.gstatic.com/firebasejs/10.5.2/firebase-database.js";
import { Database, GetElementValue, FirebaseConfig, PatientAccount, IsNullOrEmpty, SetContentById } from "./main.js";

const SecondaryApp = initializeApp(FirebaseConfig, "SecondaryApp");
const SecondaryAuth = getAuth(SecondaryApp);

let hiddenKey = null;

const searchTextBoxElement = document.getElementById("ContentSearch");
const searchButtonElement = document.getElementById("SearchButton");
const tableBody = document.getElementById("table-body");
searchButtonElement.addEventListener("click", async (event) => {
    event.preventDefault();
    ShowLoading();
    tableBody.innerHTML = "";
    if (searchTextBoxElement.value.length == 0) {
        await PatientAccount();
        HideLoading();
        return;
    }
    SearchData(searchTextBoxElement.value.toLowerCase())
    HideLoading();
});

async function SearchData(text) {
    try {
        await get(child(ref(Database), 'users')).then(async (userSnapshot) => {
            if (await userSnapshot.exists()) {
                const data = userSnapshot.val();
                if (data) {
                    for (const [key, values] of Object.entries(data)) {
                        if (values.CompleteName.toLowerCase().startsWith(text) || values.CompleteName.toLowerCase().includes(text)) {
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
                            const viewButton = document.createElement("button");
                            viewButton.classList.add('Button-Blue-Icon', 'modal-trigger');
                            viewButton.title = "View";
                            viewButton.setAttribute('data-target', 'View-Modal');
                            viewButton.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" style="pointer-events: none;"><title>eye-outline</title><path d="M12,9A3,3 0 0,1 15,12A3,3 0 0,1 12,15A3,3 0 0,1 9,12A3,3 0 0,1 12,9M12,4.5C17,4.5 21.27,7.61 23,12C21.27,16.39 17,19.5 12,19.5C7,19.5 2.73,16.39 1,12C2.73,7.61 7,4.5 12,4.5M3.18,12C4.83,15.36 8.24,17.5 12,17.5C15.76,17.5 19.17,15.36 20.82,12C19.17,8.64 15.76,6.5 12,6.5C8.24,6.5 4.83,8.64 3.18,12Z" /></svg>';

                            viewButton.addEventListener('click', function () {
                                var modalId = this.dataset.target;
                                var modal = document.getElementById(modalId);
                                console.log(modal);
                                modal.style.display = 'flex';

                                // When the user clicks on the close button, close the modal
                                var closeBtn = modal.querySelector('.modal-close');
                                closeBtn.addEventListener('click', function () {
                                    modal.style.display = 'none';
                                });
                            });

                            const chatButton = document.createElement("button");
                            chatButton.classList.add('Button-Green-Icon', 'modal-trigger');
                            chatButton.title = "Chat";
                            chatButton.setAttribute('data-target', 'Chat-Modal');
                            chatButton.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" style="pointer-events: none;"><title>chat-processing-outline</title><path d="M12 3C6.5 3 2 6.58 2 11C2.05 13.15 3.06 15.17 4.75 16.5C4.75 17.1 4.33 18.67 2 21C4.37 20.89 6.64 20 8.47 18.5C9.61 18.83 10.81 19 12 19C17.5 19 22 15.42 22 11S17.5 3 12 3M12 17C7.58 17 4 14.31 4 11S7.58 5 12 5 20 7.69 20 11 16.42 17 12 17M17 12V10H15V12H17M13 12V10H11V12H13M9 12V10H7V12H9Z" /></svg>';
                            chatButton.style.marginLeft = "5px";

                            chatButton.addEventListener('click', function () {
                                var modalId = this.dataset.target;
                                var modal = document.getElementById(modalId);
                                console.log(modal);
                                modal.style.display = 'flex';

                                // When the user clicks on the close button, close the modal
                                var closeBtn = modal.querySelector('.modal-close');
                                closeBtn.addEventListener('click', function () {
                                    modal.style.display = 'none';
                                });
                            });
                            // const disableButton = document.createElement("button");
                            // disableButton.className = "Button-Red-Icon";
                            // disableButton.style = "margin-left: 4px;"
                            // disableButton.title = "Disable";
                            // disableButton.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 -960 960 960" width="24" style="pointer-events: none;"><path d="M538-538ZM424-424Zm56 264q51 0 98-15.5t88-44.5q-41-29-88-44.5T480-280q-51 0-98 15.5T294-220q41 29 88 44.5t98 15.5Zm106-328-57-57q5-8 8-17t3-18q0-25-17.5-42.5T480-640q-9 0-18 3t-17 8l-57-57q19-17 42.5-25.5T480-720q58 0 99 41t41 99q0 26-8.5 49.5T586-488Zm228 228-58-58q22-37 33-78t11-84q0-134-93-227t-227-93q-43 0-84 11t-78 33l-58-58q49-32 105-49t115-17q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 59-17 115t-49 105ZM480-80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-59 16.5-115T145-701L27-820l57-57L876-85l-57 57-615-614q-22 37-33 78t-11 84q0 57 19 109t55 95q54-41 116.5-62.5T480-360q38 0 76 8t74 22l133 133q-57 57-130 87T480-80Z"/></svg>';

                            actionsCell.appendChild(viewButton);
                            actionsCell.appendChild(chatButton);

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
            }
        }).catch((error) => {
            console.log(error);
        });
    } catch (error) {
        console.log(error);
    }
}

const table = document.getElementById("myTable");
table.addEventListener("click", async (event) => {
    if (event.target && event.target.matches(".Button-Blue-Icon")) {
        const row = event.target.closest("tr");
        const hiddenInput = row.querySelector("input[type='hidden']");
        const id = hiddenInput.value;
        ShowLoading();
        document.querySelector("#View-Modal .modal-body> :nth-child(3)").innerHTML = "";
        document.querySelector("#View-Modal .modal-body> :nth-child(4)").innerHTML = "";
        try {
            await get(child(ref(Database), 'users/' + id)).then(async (userSnapshot) => {
                const data = userSnapshot.val();
                SetContentById("CompleteName", data.CompleteName);
                SetContentById("Contact", data.Contact);
                SetContentById("Gender", data.Gender);
                SetContentById("Birthday", data.Birthday);
                SetContentById("Height", data.Height);
                SetContentById("Weight", data.Weight);
                onValue(ref(Database, "users/" + id + "/DataSensors"), (sensorSnapshot) => {
                    const sensor = sensorSnapshot.val();
                    SetContentById("HeartRateSensor", sensor.HeartRateSensor);
                    SetContentById("SmartWatchStatus", sensor.SmartWatchStatus);
                    SetContentById("StepSensor", sensor.StepSensor);
                });
                onValue(ref(Database, "sensors"), (sensorSnapshot) => {
                    const sensor = sensorSnapshot.val();
                    SetContentById("BloodOxygen", sensor.BloodOxygen);
                    SetContentById("BloodPressure", sensor.BloodPressure);
                    SetContentById("BodyComposition", sensor.BodyComposition);
                    SetContentById("Water", sensor.Water);
                    SetContentById("WorkOutSession", sensor.WorkOutSession);
                });

                for (const [keys, values] of Object.entries(data.Goals)) {
                    const goalItems = [
                        { label: 'Goal', value: values.TotalSteps },
                        { label: 'Status', value: values.Status },
                        { label: 'Created', value: values.Created },
                        { label: 'Progress', value: values.StepsTaken }
                    ];
                    CreateGoals(document.querySelector("#View-Modal .modal-body> :nth-child(3)"), goalItems, 'fitness-goal-container');
                }
                for (const [keys, values] of Object.entries(data.Calories)) {
                    const goalItems = [
                        { label: 'Goal', value: values.GoalCalorie },
                        { label: 'Status', value: values.Status },
                        { label: 'Created', value: values.Created },
                        { label: 'Progress', value: values.CalorieTaken }
                    ];
                    CreateGoals(document.querySelector("#View-Modal .modal-body> :nth-child(4)"), goalItems), 'calories-goal-container';
                }
            }).catch((error) => {
                console.log(error);
            });
        } catch (error) {
            console.log(error);
        }
        HideLoading();
    }
    if (event.target && event.target.matches(".Button-Green-Icon")) {
        const row = event.target.closest("tr");

        const hiddenInput = row.querySelector("input[type='hidden']");
        const id = hiddenInput.value;
        hiddenKey = id;
        ShowLoading();
        document.querySelector("#Chat-Modal .modal-body> :nth-child(1)").innerHTML = "";
        try {
            await get(child(ref(Database), 'users/' + id + '/Messages')).then(async (messageSnapshot) => {
                const data = messageSnapshot.val();
                for (const [keys, values] of Object.entries(data)) {
                    CreateMessages(document.querySelector("#Chat-Modal .modal-body> :nth-child(1)"), values)
                }
            }).catch((error) => {
                console.log(error);
            });
        } catch (error) {
            console.log(error);
        }
        HideLoading();
    }
});

document.getElementById("message-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const message = GetElementValue("message-box") ?? "";
    if (message == "") {
        return;
    }
    ShowLoading();
    const date = new Date();
    const data = {
        'CreatedDate': new Intl.DateTimeFormat('en-PH', { month: 'long', day: 'numeric', year: 'numeric' }).format(date),
        'CreatedTime': new Intl.DateTimeFormat('en-PH', { hour: 'numeric', minute: 'numeric', hour12: true }).format(date),
        'Message': message
    }
    await push(ref(Database, "users/" + hiddenKey + '/Messages'), data);
    document.getElementById("message-form").reset();
    CreateMessages(document.querySelector("#Chat-Modal .modal-body> :nth-child(1)"), data)
    HideLoading();
});

function CreateGoals(root, goalItems, classNameContainer) {
    // Create the outermost div with class "fitness-goal-container"
    const fitnessGoalContainer = document.createElement('div');
    fitnessGoalContainer.classList.add(classNameContainer);

    // Iterate through the goal items and create the corresponding HTML structure
    goalItems.forEach(item => {
        // Create a div for each goal item
        const goalItemDiv = document.createElement('div');

        // Create the first span for the label
        const labelSpan = document.createElement('span');
        labelSpan.textContent = item.label + ': ';

        // Create the second span for the value
        const valueSpan = document.createElement('span');
        valueSpan.textContent = item.value;

        // Append both spans to the goal item div
        goalItemDiv.appendChild(labelSpan);
        goalItemDiv.appendChild(valueSpan);

        // Append the goal item div to the fitness goal container
        fitnessGoalContainer.appendChild(goalItemDiv);
    });

    // Append the fitness goal container to the body
    root.appendChild(fitnessGoalContainer);
}

function CreateMessages(root, data) {
    const outerDiv = document.createElement('div');

    const span = document.createElement('span');
    span.textContent = data.CreatedDate + " " + data.CreatedTime;

    const paragraph = document.createElement('p');
    paragraph.textContent = data.Message;

    // Append the span and paragraph elements to the outer div
    outerDiv.appendChild(span);
    outerDiv.appendChild(paragraph);

    // Append the outer div to the document body or any other desired element
    root.appendChild(outerDiv);
}