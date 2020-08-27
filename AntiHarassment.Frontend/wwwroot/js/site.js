window.AntiHarassmentSetItem = function (key, value) {
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

window.InitDropDowns = function () {
    var elems = document.querySelectorAll('.dropdown-trigger');
    var instances = M.Dropdown.init(elems, {});
}

window.InitDatePickers = function () {
    var elems = document.querySelectorAll('.datepicker');
    var instances = M.Datepicker.init(elems, {
    });
}

var createRuleModelInstance;
var editModalInstance;

window.InitModal = function () {
    var elems = document.querySelector('#createRuleModal');
    createRuleModelInstance = M.Modal.init(elems, {});

    var editRuleModal = document.querySelector('#editRuleModal');
    editModalInstance = M.Modal.init(editRuleModal, {});
}

window.OpenModalForEditRule = function () {
    editModalInstance.open();
}

window.CloseModalForEditRule = function () {
    editModalInstance.close();
}

window.CloseModalForCreateRule = function () {
    createRuleModelInstance.close();
}

window.InitSelectors = function () {
    var elems = document.querySelectorAll('select');
    var instances = M.FormSelect.init(elems, {});
}

window.InitCollapsibles = function () {
    var elems = document.querySelectorAll('.collapsible');
    var instances = M.Collapsible.init(elems, {});
}

var invalidReasonModalInstance;
window.OpenModalForInvalidReasoning = function () {
    var elems = document.querySelector('#invalidReasoningModal');
    invalidReasonModalInstance = M.Modal.init(elems, {});
    invalidReasonModalInstance.open();
}

window.CloseModalForInvalidReasoning = function () {
    invalidReasonModalInstance.close();
}

var modelForUserLinkingInstance;
window.OpenModalForUserLinking = function () {
    var elems = document.querySelector('#accountLinkingModal');
    modelForUserLinkingInstance = M.Modal.init(elems, {});
    modelForUserLinkingInstance.open();
}

window.CloseModalForUserLinking = function () {
    modelForUserLinkingInstance.close();
}

var modelForCreatingSuspensionInstance;
window.OpenModalForCreatingSuspension = function () {
    var elems = document.querySelector('#createSuspensionModal');
    modelForCreatingSuspensionInstance = M.Modal.init(elems, {});
    modelForCreatingSuspensionInstance.open();
}

window.CloseModalForCreatingSuspension = function () {
    modelForCreatingSuspensionInstance.close();
}

var modelForDisplayImageInstance;
window.OpenModalForDisplayingImages = function () {
    var elems = document.querySelector('#suspensionImagesModal');
    modelForDisplayImageInstance = M.Modal.init(elems, {});
    modelForDisplayImageInstance.open();
}

var modalForDisplayUserLookupImageInstance;
window.OpenModalForDisplayingImagesUserLookup = function () {
    var elems = document.querySelector('#suspensionImagesUserLookupModal');
    modalForDisplayUserLookupImageInstance = M.Modal.init(elems, {});
    modalForDisplayUserLookupImageInstance.open();
}

window.InitTooltips = function () {
    var elems = document.querySelectorAll('.tooltipped');
    var instances = M.Tooltip.init(elems, {});
}

window.InitImageBoxes = function () {
    var elems = document.querySelectorAll('.materialboxed');
    var instances = M.Materialbox.init(elems, {});
}

window.SendToast = function (textToDisplay) {
    M.toast({ html: textToDisplay, displayLength: 30000 });
    var audio = new Audio('quack.mp3'); // should probably remove it
    audio.play();
}

var overallChart;
var tagsChart;

window.InitializeStatisticsPage = function () {
    var overAllContext = document.getElementById('overallChart').getContext('2d');
    overallChart = new Chart(overAllContext, {
        type: 'line',

        data: {
            labels: [],
            datasets: [{
                label: 'Timeouts',
                borderColor: 'rgb(255, 255, 51)',
                data: []
            },
            {
                label: 'Bans',
                borderColor: 'rgb(255, 51, 51)',
                data: []
            },
            {
                label: 'Suspensions',
                borderColor: 'rgb(255, 51, 255)',
                data: []
            }
            ]
        },

        options: {}
    });

    var tagContext = document.getElementById('chartForTags').getContext('2d');
    tagsChart = new Chart(tagContext, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [{
                label: 'count',
                backgroundColor: 'rgb(255, 204, 204)',
                data: []
            }]
        },
        options: {}
    });
}

window.ClearGraph = function () {
    overallChart.destroy();
    tagsChart.destroy();

    window.InitializeStatisticsPage();
}

window.AddDataToGraph = function (suspensionsPerDay) {
    suspensionsPerDay.forEach((valuePair) => {
        let date = Date.parse(valuePair.date);
        const dateTimeFormat = new Intl.DateTimeFormat('en', { year: 'numeric', month: 'long', day: '2-digit' });
        const [{ value: month }, , { value: day }, , { value: year }] = dateTimeFormat.formatToParts(date);
        overallChart.data.labels.push(`${day}-${month}-${year}`);
        overallChart.data.datasets[0].data.push(valuePair.timeoutCount);
        overallChart.data.datasets[1].data.push(valuePair.bansCount);
        overallChart.data.datasets[2].data.push(valuePair.suspensionsCount);
    });

    overallChart.update();
}

window.AddDataToTagsGraph = function (data) {
    data.forEach((TagCountModel) => {
        tagsChart.data.labels.push(TagCountModel.tag.tagName);
        tagsChart.data.datasets[0].data.push(TagCountModel.count);
    });

    tagsChart.update();
}

var systemOverallChart;
var systemInvalidOverallChart;
var systemTagsChart;

window.ClearSystemGraphs = function () {
    systemOverallChart.destroy();
    systemInvalidOverallChart.destroy();
    systemTagsChart.destroy();

    window.InitializeSystemStatisticsPage();
}

window.InitializeSystemStatisticsPage = function () {
    var systemOverallContext = document.getElementById('systemOverallChart').getContext('2d');
    systemOverallChart = new Chart(systemOverallContext, {
        type: 'line',

        data: {
            labels: [],
            datasets: [{
                label: 'Timeouts',
                borderColor: 'rgb(255, 255, 51)',
                data: []
            },
            {
                label: 'Bans',
                borderColor: 'rgb(255, 51, 51)',
                data: []
            },
            {
                label: 'Suspensions',
                borderColor: 'rgb(255, 51, 255)',
                data: []
            }
            ]
        },

        options: {}
    });

    var systemInvalidOverallContext = document.getElementById('systemInvalidOverallChart').getContext('2d');
    systemInvalidOverallChart = new Chart(systemInvalidOverallContext, {
        type: 'line',

        data: {
            labels: [],
            datasets: [{
                label: 'Timeouts',
                borderColor: 'rgb(255, 255, 51)',
                data: []
            },
            {
                label: 'Bans',
                borderColor: 'rgb(255, 51, 51)',
                data: []
            },
            {
                label: 'Suspensions',
                borderColor: 'rgb(255, 51, 255)',
                data: []
            }
            ]
        },

        options: {}
    });

    var tagContext = document.getElementById('systemChartForTags').getContext('2d');
    systemTagsChart = new Chart(tagContext, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [{
                label: 'count',
                backgroundColor: 'rgb(255, 204, 204)',
                data: []
            }]
        },
        options: {}
    });
}

window.AddDataToSystemTagsGraph = function (data) {
    data.forEach((TagCountModel) => {
        systemTagsChart.data.labels.push(TagCountModel.tag.tagName);
        systemTagsChart.data.datasets[0].data.push(TagCountModel.count);
    });

    systemTagsChart.update();
}



window.AddDataToSystemOverallChart = function (suspensionsPerDay) {
    suspensionsPerDay.forEach((valuePair) => {
        let date = Date.parse(valuePair.date);
        const dateTimeFormat = new Intl.DateTimeFormat('en', { year: 'numeric', month: 'long', day: '2-digit' });
        const [{ value: month }, , { value: day }, , { value: year }] = dateTimeFormat.formatToParts(date);
        systemOverallChart.data.labels.push(`${day}-${month}-${year}`);
        systemOverallChart.data.datasets[0].data.push(valuePair.timeoutCount);
        systemOverallChart.data.datasets[1].data.push(valuePair.bansCount);
        systemOverallChart.data.datasets[2].data.push(valuePair.suspensionsCount);
    });

    systemOverallChart.update();
}

window.AddDataToSystemInvalidChart = function (suspensionsPerDay) {
    suspensionsPerDay.forEach((valuePair) => {
        let date = Date.parse(valuePair.date);
        const dateTimeFormat = new Intl.DateTimeFormat('en', { year: 'numeric', month: 'long', day: '2-digit' });
        const [{ value: month }, , { value: day }, , { value: year }] = dateTimeFormat.formatToParts(date);
        systemInvalidOverallChart.data.labels.push(`${day}-${month}-${year}`);
        systemInvalidOverallChart.data.datasets[0].data.push(valuePair.timeoutCount);
        systemInvalidOverallChart.data.datasets[1].data.push(valuePair.bansCount);
        systemInvalidOverallChart.data.datasets[2].data.push(valuePair.suspensionsCount);
    });

    systemInvalidOverallChart.update();
}