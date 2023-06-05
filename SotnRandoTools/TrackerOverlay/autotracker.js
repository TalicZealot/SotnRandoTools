const mainContainer = document.getElementById('tracker');
var socket;
var slots = [];
let relicIds = [
    "blank",
    "SoulOfBat",
    "FireOfBat",
    "EchoOfBat",
    "ForceOfEcho",
    "SoulOfWolf",
    "PowerOfWolf",
    "SkillOfWolf",
    "FormOfMist",
    "PowerOfMist",
    "GasCloud",
    "CubeOfZoe",
    "SpiritOrb",
    "GravityBoots",
    "LeapStone",
    "HolySymbol",
    "FaerieScroll",
    "JewelOfOpen",
    "MermanStatue",
    "BatCard",
    "GhostCard",
    "FaerieCard",
    "DemonCard",
    "SwordCard",
    "SpriteCard",
    "NoseDevilCard",
    "HeartOfVlad",
    "ToothOfVlad",
    "RibOfVlad",
    "RingOfVlad",
    "EyeOfVlad",
    "GoldRing",
    "SilverRing",
    "SpikeBreaker",
    "HolyGlasses",
    "Claymore"
]
let relicStates = [];
var bitFlags = [];
for (let i = 0; i < 30; i++) {
    bitFlags.push(Math.pow(2, i));
}

function InsertItems() {
    $("#tracker").empty();
    let cellsHtml = "";

    for (let i = 0; i < slots.length; i++) {
        cellsHtml += '<div class="column">';
        let columnHtml = "";
        for (let j = 0; j < slots[i].length; j++) {
            if (slots[i][j] > 0) {
                columnHtml += (`<div class="portlet" data-index="${slots[i][j]}"><img class="relic uncollected" id="${relicIds[slots[i][j]]}" src="../Images/${relicIds[slots[i][j]]}.png" alt=""></div>`);
            } else {
                columnHtml += (`<div class="portlet" data-index="0"></div>`);
            }
        }
        cellsHtml += columnHtml + '</div>';
        columnHtml = "";
    }

    $("#tracker").append(cellsHtml);
}

function updateStatus(relics, items) {
    relicStates[1] = relics & bitFlags[0];
    relicStates[2] = relics & bitFlags[1];
    relicStates[3] = relics & bitFlags[2];
    relicStates[4] = relics & bitFlags[3];
    relicStates[5] = relics & bitFlags[4];
    relicStates[6] = relics & bitFlags[5];
    relicStates[7] = relics & bitFlags[6];
    relicStates[8] = relics & bitFlags[7];
    relicStates[9] = relics & bitFlags[8];
    relicStates[10] = relics & bitFlags[9];
    relicStates[11] = relics & bitFlags[10];
    relicStates[12] = relics & bitFlags[11];
    relicStates[13] = relics & bitFlags[12];
    relicStates[14] = relics & bitFlags[13];
    relicStates[15] = relics & bitFlags[14];
    relicStates[16] = relics & bitFlags[15];
    relicStates[17] = relics & bitFlags[16];
    relicStates[18] = relics & bitFlags[17];
    relicStates[19] = relics & bitFlags[18];
    relicStates[20] = relics & bitFlags[19];
    relicStates[21] = relics & bitFlags[20];
    relicStates[22] = relics & bitFlags[21];
    relicStates[23] = relics & bitFlags[22];
    relicStates[24] = relics & bitFlags[23];
    relicStates[25] = relics & bitFlags[24];
    relicStates[26] = relics & bitFlags[25];
    relicStates[27] = relics & bitFlags[26];
    relicStates[28] = relics & bitFlags[27];
    relicStates[29] = relics & bitFlags[28];
    relicStates[30] = relics & bitFlags[29];
    relicStates[31] = items & bitFlags[0];
    relicStates[32] = items & bitFlags[1];
    relicStates[33] = items & bitFlags[2];
    relicStates[34] = items & bitFlags[3];
    relicStates[35] = items & bitFlags[4];

    for (let i = 0; i < relicIds.length; i++) {
        let currentRelic = $('#' + relicIds[i]);
        if (relicStates[i] && currentRelic.hasClass('uncollected')) {
            currentRelic.removeClass('uncollected');
        } else if (!relicStates[i] && !currentRelic.hasClass('uncollected')) {
            currentRelic.addClass('uncollected');
        }
    }
}

function saveChanges() {
    if (socket != null && socket.readyState == 1) {
        let message = {
            event: "save-slots",
            slots: slots
        }

        socket.send(JSON.stringify(message));
        console.log("Saving slots");
    } else {
        console.log("Socket is not open!");
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

function connectWebsocket() {
    socket = new WebSocket("ws://localhost:9646");

    socket.onopen = function() {
        console.log("connected");
    };

    socket.onclose = function() {
        socket = null;
        setTimeout(connectWebsocket, 1000); //try to reconnect
    };

    socket.onmessage = function(message) {
        var socketMessage = JSON.parse(message.data);
        console.log(socketMessage);

        if (socketMessage.type == "relics") {
            updateStatus(socketMessage.relics, socketMessage.items);
        }

        if (socketMessage.type == "slots") {
            slots = socketMessage.slots;
            InsertItems();
            initializeCells();
        }
    };
}

function initializeCells() {
    $(".column").sortable({
        connectWith: ".column",
        handle: ".relic",
        cancel: ".portlet-toggle",
        placeholder: "portlet-placeholder ui-corner-all"
    });

    $(".portlet").mouseup(function() {
        setSLots();
        saveChanges();
    });

    $(".portlet")
        .addClass("ui-widget ui-widget-content ui-helper-clearfix ui-corner-all")
        .find(".relic")
        .addClass("ui-widget-header ui-corner-all")
        .prepend("<span class='ui-icon ui-icon-minusthick portlet-toggle'></span>");

}

// Initiate on document load
$(function() {
    connectWebsocket();
});