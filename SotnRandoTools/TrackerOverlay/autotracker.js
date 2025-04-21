const mainContainer = document.getElementById('tracker');
var socket;
var slots;
let textureCoordinates = [
    { col: 0, row: 8 },
    { col: 1, row: 8 },
    { col: 2, row: 8 },
    { col: 3, row: 8 },
    { col: 4, row: 8 },
    { col: 5, row: 8 },
    { col: 6, row: 8 },
    { col: 0, row: 7 },
    { col: 1, row: 7 },
    { col: 2, row: 7 },
    { col: 3, row: 7 },
    { col: 4, row: 7 },
    { col: 5, row: 7 },
    { col: 6, row: 7 },
    { col: 0, row: 6 },
    { col: 1, row: 6 },
    { col: 2, row: 6 },
    { col: 3, row: 6 },
    { col: 4, row: 6 },
    { col: 5, row: 6 },
    { col: 6, row: 6 },
    { col: 0, row: 5 },
    { col: 1, row: 5 },
    { col: 2, row: 5 },
    { col: 3, row: 5 },
    { col: 4, row: 5 },
    { col: 5, row: 5 },
    { col: 6, row: 5 },
    { col: 0, row: 4 },
    { col: 1, row: 4 },
    { col: 2, row: 4 },
    { col: 3, row: 4 },
    { col: 4, row: 4 },
    { col: 5, row: 4 },
    { col: 6, row: 4 },
    { col: 0, row: 3 },
    { col: 1, row: 3 },
    { col: 2, row: 3 },
    { col: 3, row: 3 },
    { col: 4, row: 3 },
    { col: 5, row: 3 },
    { col: 6, row: 3 },
    { col: 0, row: 2 },
    { col: 1, row: 2 },
    { col: 2, row: 2 },
    { col: 3, row: 2 },
    { col: 4, row: 2 },
    { col: 5, row: 2 },
    { col: 6, row: 2 },
    { col: 0, row: 1 },
    { col: 1, row: 1 },
    { col: 2, row: 1 },
    { col: 3, row: 1 },
    { col: 4, row: 1 },
    { col: 5, row: 1 },
    { col: 6, row: 1 },
    { col: 0, row: 0 },
    { col: 1, row: 0 },
    { col: 2, row: 0 },
    { col: 3, row: 0 },
    { col: 4, row: 0 },
    { col: 5, row: 0 },
    { col: 6, row: 0 }
];
let objectStates = [];
var bitFlags = [];
for (let i = 0; i < 30; i++) {
    bitFlags.push(Math.pow(2, i));
}

let draggables;

function initializeCells() {
    let currentRow = null;
    let cells = 0;
    for (let i = 0; i < 13; i++) {
        currentRow = document.createElement("div");
        for (let j = 0; j < 15; j++) {
            const slot = document.createElement("div");
            slot.className = "slot";
            slot.id = "cell" + cells;
            slot.setAttribute("draggable", "true");
            slot.setAttribute("data-index", cells);
            slot.setAttribute("data-relic", -1);
            currentRow.appendChild(slot);
            cells++;
        }
        mainContainer.append(currentRow);
    }

    draggables = document.querySelectorAll('.slot');

    draggables.forEach(draggable => {
        draggable.addEventListener('dragstart', handleDragStart);
        draggable.addEventListener('dragover', handleDragOver);
        draggable.addEventListener('drop', handleDrop);
    });
}

function insertItems() {
    const byteArray = new Uint8Array(slots);
    for (let i = 1; i < byteArray.length; i++) {
        const cell = document.getElementById("cell" + byteArray[i]);
        if (!cell) {
            break;
        }
        cell.innerHTML = "";
        cell.setAttribute("data-relic", i);
        const col = textureCoordinates[i - 1].col;
        const row = textureCoordinates[i - 1].row;
        const trackedObject = document.createElement("div");
        trackedObject.id = 'object' + i;
        trackedObject.draggable = false;
        trackedObject.className = "relic uncollected";
        trackedObject.style.setProperty("--col", col);
        trackedObject.style.setProperty("--row", row);
        cell.appendChild(trackedObject);
    }
}

function updateStatus(relics, items, bosses) {
    for (let i = 0; i < 30; i++) {
        objectStates[i + 1] = relics & bitFlags[i];
    }

    for (let i = 0; i < 5; i++) {
        objectStates[i + 31] = items & bitFlags[i];
    }

    for (let i = 0; i < 23; i++) {
        objectStates[i + 36] = bosses & bitFlags[i];
    }

    for (let i = 1; i < objectStates.length; i++) {
        let currentRelic = document.getElementById('object' + i);
        if (objectStates[i] && currentRelic.classList.contains('uncollected')) {
            currentRelic.classList.remove('uncollected');
        } else if (!objectStates[i] && !currentRelic.classList.contains('uncollected')) {
            currentRelic.classList.add('uncollected');
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
            updateStatus(dataView.getInt32(1, true), dataView.getInt32(5, true), dataView.getInt32(9, true));
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
    if (draggedRelic > -1) {
        byteArray[draggedRelic] = targetIndex;
    }
    if (targetRelic > -1) {
        byteArray[targetRelic] = draggedIndex;
    }

    event.currentTarget.dataset.relic = draggedRelic;
    draggedElement.dataset.relic = targetRelic;

    draggedElement.innerHTML = '';
    event.currentTarget.innerHTML = '';

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

window.onload = (event) => {
    initializeCells();
    connectWebsocket();
};