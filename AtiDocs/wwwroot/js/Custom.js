// Reference for Write class
var objRef;
var saved = false;

function Log(message) {
    console.log(message);
}

function GetText(element) {
    elem = document.getElementById(element);
    if (elem == null)
        return null;
    return elem.innerText;
}

function PutText(element, text) {
    elem = document.getElementById(element);
    elem.innerText = text;
}

function ClearText(element) {
    elem = document.getElementById(element);
    elem.innerHTML = "";
}

function EditResume(element, content) {
    elem = document.getElementById(element);
    elem.innerText = content;
    elem.addEventListener('keydown', handleEnter);
    elem.addEventListener('keyup', clearSave);
    //console.log("Register: " + elem.id);
    //console.log("Edit: " + elem.id);
}

function SetFocus(element) {
    document.getElementById(element).focus();
}

function RegisterObject(obj) {
    //console.log("Registered: " + obj)
    objRef = obj;
}

function RegisterEvent(element) {
    elem = document.getElementById(element);
    if (elem == null) {
        setTimeout(function () { RegisterEvent(element) }, 100);
        return;
    }
    elem.addEventListener('keydown', handleEnter);
    elem.addEventListener('keydown', handleSave);
    elem.addEventListener('keyup', clearSave);
    //console.log("Register: " + elem.id);
}

function RegisterEventForPage() {
    document.addEventListener('keydown', handleSave);
    document.addEventListener('keyup', clearSave);
}

function UnregisterEvent(element) {
    elem = document.getElementById(element);
    elem.removeEventListener('keydown', handleEnter);
    elem.removeEventListener('keydown', handleSave);
    elem.addEventListener('keyup', clearSave);
    //console.log("Unregister: " + elem.id);
}

function UnregisterEventForPage() {
    document.removeEventListener('keydown', handleSave);
    document.addEventListener('keyup', clearSave);
}

function handleEnter(evt) {
    // If Shift+Enter is pressed add new block
    if (evt.keyCode == 13 && evt.shiftKey) {
        //.log("Prevented combination");
        evt.preventDefault();
        objRef.invokeMethodAsync("AddNewBlock", this.id);
    }

    // If Shift+Del is pressed delete the current block
    if (evt.keyCode == 46 && evt.shiftKey) {
        //console.log("Block deleted");
        evt.preventDefault();
        objRef.invokeMethodAsync("DeleteBlock", this.id);
    }

    // If Ctrl+S is pressed save the document
    if (evt.keyCode == 83 && evt.ctrlKey) {
        elem = document.getElementById(this.id);
        if (elem != null) {
            elem.blur();
        }
        evt.preventDefault();
    }
}

function handleSave(evt) {
    // If Ctrl+S is pressed save the document
    if (evt.keyCode == 83 && evt.ctrlKey && saved == false) {
        saved = true;
        if(this.id != null) {
            elem = document.getElementById(this.id);
            if (elem != null) {
                elem.blur();
            }
        }
        evt.preventDefault();
        objRef.invokeMethodAsync("SaveContent", this.id, "save");
    }
}

function clearSave(evt) {
    saved = false;
}

function RegisterScroll() {
    window.onscroll = function() { handleScroll() };
}

function UnregisterScroll() {
    window.onscroll = null;
}

function handleScroll() {
    elem = document.getElementById("header-bar-bar");
    if (elem == null)
        return;

    title = document.getElementById("header-bar-bar-title");

    brandName = document.getElementById("header-brandname");

    if (document.body.scrollTop > 155 || document.documentElement.scrollTop > 155) {
        elem.style.position = "fixed";
        elem.style.top = "0";
        title.style.visibility = "visible";
        if (brandName != null) {
            brandName.style.width = "0";
            brandName.style.height = "0";
        }
    }
    else {
        elem.style.position = "relative";
        elem.style.top = "0";
        title.style.visibility = "hidden";
        if (brandName != null) {
            if (document.width > 1140) {
                brandName.style.width = "max-content";
                brandName.style.height = "auto";
            }
            else {
                brandName.style.width = "0";
                brandName.style.width = "0";
                brandName.style.visibility = "collapse";
            }
            
        }
    }
}

function ShowItem(element) {
    elem = document.getElementById(element);
    elem.style.visibility = "visible";
}

function HideItem(element) {
    elem = document.getElementById(element);
    elem.style.visibility = "collapse";
}