const mainContainer = document.getElementById('tracker');
var socket;
var slots;
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

const draggables = document.querySelectorAll('.portlet');

draggables.forEach(draggable => {
    draggable.addEventListener('dragstart', handleDragStart);
    draggable.addEventListener('dragover', handleDragOver);
    draggable.addEventListener('drop', handleDrop);
});

function insertItems() {
    const byteArray = new Uint8Array(slots);
    for (let i = 1; i < byteArray.length; i++) {
        const cell = $("#cell" + (i - 1).toString());
        cell.empty();
        cell.attr("data-relic", byteArray[i]);
        if (byteArray[i] > 0) {
            cell.append(`<img class="relic uncollected" id="${relicIds[byteArray[i]]}" src="../Images/${relicIds[byteArray[i]]}.png" alt="" draggable="false">`);
        }
    }
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

    for (let i = 1; i < relicIds.length; i++) {
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
        socket.send(slots);
    } else {
        console.log("Socket is not open!");
    }
}

function connectWebsocket() {
    socket = new WebSocket("ws://localhost:9646");
    socket.binaryType = "arraybuffer";

    socket.onclose = function () {
        socket = null;
        setTimeout(connectWebsocket, 4000); //try to reconnect
    };

    socket.onerror = function (e) {
        socket.close();
        socket = null;
    };

    socket.onmessage = function (e) {
        const dataView = new DataView(e.data);
        if (dataView.getInt8(0) == 0) {
            updateStatus(dataView.getInt32(1, true), dataView.getInt32(5, true));
        } else {
            slots = e.data;
            insertItems();
        }
    };
}

function handleDragStart(event) {
    event.dataTransfer.setData('text/plain', event.target.id);
}

function handleDrop(event) {
    event.preventDefault();
    const draggedId = event.dataTransfer.getData('text/plain');
    const draggedElement = document.getElementById(draggedId);
    let draggedRelic = parseInt(draggedElement.dataset.relic, 10);
    let draggedIndex = parseInt(draggedElement.dataset.index, 10);
    let draggedImage = null;
    if (draggedElement.children.length > 0) {
        draggedImage = draggedElement.children[0].cloneNode();
    }

    let targetRelic = parseInt(event.currentTarget.dataset.relic, 10);
    let targetIndex = parseInt(event.currentTarget.dataset.index, 10);
    let targetImage = null;
    if (event.currentTarget.children.length > 0) {
        targetImage = event.currentTarget.children[0].cloneNode();
    }

    const byteArray = new Uint8Array(slots);
    byteArray[draggedIndex] = targetRelic;
    byteArray[targetIndex] = draggedRelic;

    event.currentTarget.dataset.relic = draggedRelic;
    draggedElement.dataset.relic = targetRelic;

    draggedElement.innerHTML = '';
    event.currentTarget.innerHTML = '';
    console.log(targetImage);

    if (targetImage !== null) {
        draggedElement.appendChild(targetImage);
    }
    if (draggedImage !== null) {  
        event.currentTarget.appendChild(draggedImage);
    }

    saveChanges();
}

function handleDragOver(event) {
    event.preventDefault();
}

// Initiate on document load
$(function () {
    connectWebsocket();
});