window.AntiHarassmentSetItem = function (key, value) {
    this.localStorage.setItem(key, value);
};

window.AntiHarassmentGetItem = function (key) {
    return this.localStorage.getItem(key);
};

window.CopyTextToClipboard = function (text) {
    navigator.clipboard.writeText(text);
}