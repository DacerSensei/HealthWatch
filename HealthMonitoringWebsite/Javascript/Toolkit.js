const Colors = {
    Green: "#00d25b",
    Yellow: "#ffab00",
    Blue: "#0090e7",
    Red: "#fc424a",
    Purple: "#8f5fe8",
    Pink: "#f72585",
    Black: "#000000",
    White: "#FFFFFF",
    Ash: "#d5dee1",
    BorderColor: getComputedStyle(document.documentElement).getPropertyValue('--color-border'),
    PrimaryColor: getComputedStyle(document.documentElement).getPropertyValue('--color-primary')
}

const PopupType = {
    Alert: true,
    Prompt: false,
}

function ShowNotification(text, backgroundColor, textColor = Colors.White) {
    const notificationBar = document.getElementById("Notification-Bar");
    if (notificationBar) {
        notificationBar.textContent = text;
        notificationBar.style.color = textColor;
        notificationBar.style.display = "block";
        notificationBar.style.backgroundColor = backgroundColor;
        setTimeout(() => {
            notificationBar.style.display = "none";
        }, 3000);
    }
}

function SetErrorFocus(object) {
    Element.prototype.documentOffsetTop = function () {
        return this.offsetTop + (this.offsetParent ? this.offsetParent.documentOffsetTop() : 0);
    };

    var top = object.documentOffsetTop() - (window.innerHeight / 2);
    window.scrollTo(0, top);
    object.focus();

    var currentBorder = object.style.border;
    object.style.outline = '2px solid #da373c';
    setTimeout(() => {
        object.style.outline = currentBorder;
    }, 2000);
}

function ShowPopup(Message, Type = PopupType.Alert, Placeholder) {
    return new Promise((resolve) => {
        const popup = document.getElementById('Popup');
        const popupQuestion = document.getElementById('Popup-Question');
        const popupInput = document.getElementById('Popup-Input');
        const popupYes = document.getElementById('Popup-Yes');
        const popupNo = document.getElementById('Popup-No');

        popupQuestion.textContent = Message;

        if (Placeholder) {
            popupInput.placeholder = Placeholder;
            popupInput.style.display = "block";
        } else {
            popupInput.style.display = "none";
        }

        popupYes.onclick = () => {
            popup.style.display = 'none';
            resolve(true);
        };

        popupNo.onclick = () => {
            popup.style.display = 'none';
            resolve(false);
        };
        if (Type) {
            popupYes.textContent = "Ok";
            popupNo.style.display = "none";
        } else {
            popupYes.textContent = "Yes";
            popupNo.style.display = "flex";
        }
        popup.style.display = 'flex';
    });
}

function GetValidDate() {
    return new Promise(function (resolve, reject) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', "/MWSSS-Application/PHP-API/GetValidDate.php", true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                if (xhr.responseText == "Something went wrong") {
                    resolve("Something went wrong");
                } else {
                    resolve(xhr.responseText);
                }
            } else {
                reject('Something went wrong');
            }
        };
        xhr.send();
    });
}

function ValidateDate(event) {
    var inputTextbox = event.target;
    var input = event.target.value;
    var pattern = /^(0[1-9]|1[0-2])\/(0[1-9]|[12][0-9]|3[01])\/\d{4}$/;

    if (pattern.test(input)) {
        inputTextbox.setAttribute("validDate", "");
        inputTextbox.style.outline = "";
    } else {
        inputTextbox.removeAttribute("validDate");
        inputTextbox.style.outline = '2px solid #da373c';
    }
}

function DisableButton(id) {
    var button = document.getElementById(id);
    button.style.backgroundColor = Colors.Ash;
    button.style.color = Colors.Black;
    button.style.cursor = "context-menu";
    button.disabled = true;
}

function EnableButton(id, backgroundColor = Colors.Green, color = Colors.White) {
    var button = document.getElementById(id);
    button.style.backgroundColor = backgroundColor;
    button.style.color = color;
    button.style.cursor = "pointer";
    button.disabled = false;
}

function HideElement(id) {
    var element = document.getElementById(id);
    element.style.display = "none";
}

function ShowBlockElement(id) {
    var element = document.getElementById(id);
    element.style.display = "block";
}

// Sample how to use
// ShowPopup('Do you want to proceed?').then((result) => {
//     if (result) {
//         console.log('User clicked Yes');
//     } else {
//         console.log('User clicked No');
//     }
// });


//   Html

var modalButtons = document.querySelectorAll('.modal-trigger');
var modals = document.querySelectorAll('.modal-container');
for (var i = 0; i < modalButtons.length; i++) {
    modalButtons[i].addEventListener('click', function () {
        var modalId = this.dataset.target;
        var modal = document.getElementById(modalId);
        modal.style.display = 'flex';

        // When the user clicks on the close button, close the modal
        var closeBtn = modal.querySelector('.modal-close');
        closeBtn.addEventListener('click', function () {
            modal.style.display = 'none';
        });

        // When the user clicks anywhere outside of the modal, close the modal
        // window.addEventListener('click', function(event) {
        //     if (event.target == modal) {
        //         modal.style.display = 'none';
        //     }
        // });
    });
}

function ShowLoading() {
    const loadingOverlay = document.getElementById('loading-overlay');
    if (loadingOverlay) {
        loadingOverlay.style.display = 'flex';
        document.body.classList.add('loading');
    }
}

function HideLoading() {
    const loadingOverlay = document.getElementById('loading-overlay');
    if (loadingOverlay) {
        loadingOverlay.style.display = 'none';
        document.body.classList.remove('loading');
    }
}

// Custom Picker || Selected
/* Example of how to use the custom picker
<picker-component id="gender">
    <picker-pick-component></picker-pick-component>
    <picker-item-container-component>
        <picker-item-component value="None">Select Gender</picker-item-component>
        <picker-item-component value="Male">Male</picker-item-component>
        <picker-item-component value="Female">Female</picker-item-component>
    </picker-item-container-component>
</picker-component> 

// To get the value of selected
document.getElementById("gender").Value 

// To set the selected item programmatically
document.getElementById("gender").SelectedItem = document.getElementById("gender").Items[1] 

// To get the SelectedElement of picker
document.getElementById("gender").SelectedItem
*/
class PickerItem extends HTMLElement {
    constructor() {
        super();
        this.value = this.getAttribute("value");
    }
    connectedCallback() {
        this.addEventListener('click', () => {
            const picker = this.closest("picker-component");
            picker.removeAttribute("open");
            picker.selectItem(this);
        });
    }
}
customElements.define("picker-item-component", PickerItem);

class PickedItem extends HTMLElement {
    constructor() {
        super();
        this.Picked = "";
    }
    connectedCallback() {
        this.textContent = this.Picked;
    }
}
customElements.define("picker-pick-component", PickedItem);


class PickerItemContainer extends HTMLElement {
    constructor() {
        super();
    }
}
customElements.define("picker-item-container-component", PickerItemContainer);

class Picker extends HTMLElement {
    constructor() {
        super();
        this.Items = [];
        this._selectedItem = null;
        this.IsOpen = false;
    }

    get SelectedItem() {
        return this._selectedItem;
    }

    set SelectedItem(Item) {
        if (this._selectedItem !== Item) {
            this._selectedItem = Item;
            this.SelectedItemChanged();
        }
    }

    get Value(){
        return this._selectedItem ? this._selectedItem.getAttribute("value") : null;
    }

    SelectedItemChanged() {
        if (this._selectedItem) {
            if (this._selectedItem.getAttribute("value").toLowerCase() == "None".toLowerCase()) {
                this.querySelector("picker-pick-component").style.color = "#757575";
            } else {
                this.querySelector("picker-pick-component").style.color = "black";
            }
            this.querySelector("picker-pick-component").textContent = this._selectedItem.textContent
            this._selectedItem.setAttribute("selected", "");

            this.dispatchEvent(new CustomEvent("change", {
                bubbles: true,
                detail: {
                    value: this._selectedItem.getAttribute("value"),
                    text: this._selectedItem.textContent
                }
            }));
        }
    }

    togglePicker() {
        this.IsOpen ? this.closePicker() : this.openPicker();
    }

    openPicker() {
        this.IsOpen = true;
        this.setAttribute("open", "");
        document.addEventListener('click', this.clickOutsideHandler);
    }

    closePicker() {
        this.IsOpen = false;
        this.removeAttribute("open");
        document.removeEventListener('click', this.clickOutsideHandler);
    }

    clickOutsideHandler = (event) => {
        if (!this.contains(event.target)) {
            this.closePicker();
        }
    };

    connectedCallback() {
        this.addEventListener("click", () => {
            this.togglePicker();
        })

        this.Items = Array.from(this.querySelectorAll("picker-item-component"));
        if (this.Items.length > 0) {
            this.selectItem(this.Items[0]);
        }
    }

    selectItem(item) {
        if (this.SelectedItem) {
            this.SelectedItem.removeAttribute("selected");
        }

        this.SelectedItem = item;
    }
}
customElements.define("picker-component", Picker);
// End Custom Picker