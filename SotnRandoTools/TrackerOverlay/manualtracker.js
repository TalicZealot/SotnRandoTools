const mainContainer = document.getElementById('tracker');
var slots = [];
let relicStates = [];
var bitFlags = [];
for (let i = 0; i < 30; i++) {
    bitFlags.push(Math.pow(2, i));
}

function updateRelic(element) {
    if ($(element).hasClass('uncollected')) {
        $(element).removeClass('uncollected');
    } else {
        $(element).addClass('uncollected');
    }
}

function setSLots() {
    slots = [];
    for (let i = 0; i < mainContainer.children.length; i++) {
        slots.push([]);
        let cells = Array.from(mainContainer.children[i].children);
        cells.forEach(cell => {
            let id = cell.dataset.index;
            if (id == null || id == undefined) {
                id = 0;
            }
            slots[slots.length - 1].push(id);
        });
    }
}

function initializeCells() {
    $(".column").sortable({
        connectWith: ".column",
        handle: ".relic",
        cancel: ".portlet-toggle",
        placeholder: "portlet-placeholder ui-corner-all"
    });

    $(".portlet").mouseup(function () {
        setSLots();
    });

    $(".relic").on("mousedown", function (e) {
        updateRelic(e.target);
    });

    $(".portlet")
        .addClass("ui-widget ui-widget-content ui-helper-clearfix ui-corner-all")
        .find(".relic")
        .addClass("ui-widget-header ui-corner-all")
        .prepend("<span class='ui-icon ui-icon-minusthick portlet-toggle'></span>");

}

// Initiate on document load
$(function () {
    initializeCells();
});