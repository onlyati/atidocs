function CopyLink(anchor) {
    var tempText = document.createElement("input");
    tempText.value = window.location.protocol + "//" + window.location.hostname + ':' + window.location.port + window.location.pathname + '#' + anchor;
    document.body.appendChild(tempText);
    tempText.select();
    document.execCommand("copy");
    document.body.removeChild(tempText);
}