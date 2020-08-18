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

window.OpenModalForDisplayingImages = function () {
    $('#suspensionImagesModal').modal('show');
}

window.OpenModalForDisplayingImagesUserLookup = function () {
    $('#suspensionImagesUserLookupModal').modal('show');
}

window.OpenModalForCreatingNewRule = function () {
    $('#createRuleModal').modal('show');
}

window.CloseModalForCreatingNewRule = function () {
    $('#createRuleModal').modal('hide');
}

window.OpenModalForEditRule = function () {
    $('#editRuleModal').modal('show');
}

window.CloseModalForEditRule = function () {
    $('#editRuleModal').modal('hide');
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