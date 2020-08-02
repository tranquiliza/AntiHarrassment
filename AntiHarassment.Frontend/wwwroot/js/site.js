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
        overallChart.data.labels.push(valuePair.date);
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