﻿window.AntiHarassmentSetItem = function (key, value) {
    this.localStorage.setItem(key, value);
};

window.AntiHarassmentGetItem = function (key) {
    return this.localStorage.getItem(key);
};

window.CopyTextToClipboard = function (text) {
    navigator.clipboard.writeText(text);
}

window.DownloadAsFile = function (content, filename, contentType) {
    var file = new Blob([content], { type: contentType });
    var a = document.createElement("a");
    a.href = URL.createObjectURL(file);
    a.download = filename;
    a.click();
}

window.OpenModalForInvalidReasoning = function () {
    $('#invalidReasoningModal').modal('show');
}

window.CloseModalForInvalidReasoning = function () {
    $('#invalidReasoningModal').modal('hide');
}

window.OpenModalForUserLinking = function () {
    $('#accountLinkingModal').modal('show');
}

window.CloseModalForUserLinking = function () {
    $('#accountLinkingModal').modal('hide');
}

window.OpenModalForCreatingSuspension = function () {
    $('#createSuspensionModal').modal('show');
}

window.CloseModalForCreatingSuspension = function () {
    $('#createSuspensionModal').modal('hide');
}