function GetText(element) {
    elem = document.getElementById(element);
    return elem.innerText;
}

function ClearText(element) {
    elem = document.getElementById(element);
    elem.innerHTML = "";
}

function EditResume(element, content) {
    elem = document.getElementById(element);
    elem.innerHTML = content;
    console.log(elem.innerHTML);
}

function SetFocus(element) {
    document.getElementById(element).focus();
}
