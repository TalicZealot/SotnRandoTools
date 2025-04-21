const mainContainer = document.getElementById('tracker');
const OBJECT_COUNT = 63;
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
let slots = [1,
    0, 1, 2, 3, 4,
    15, 16, 17, 18, 19,
    30, 31, 32, 33, 34,
    45, 46, 47, 48, 49,
    60, 61, 62, 63, 64,
    75, 76, 77, 78, 79,
    90, 91, 92, 93, 94,
    6, 7, 8, 9, 10,
    21, 22, 23, 24, 25,
    36, 37, 38, 39, 40,
    51, 52, 53, 54, 55,
    66, 67, 68, 69, 70];
let draggables;
let relics;

function handleDragStart(event) {
    event.dataTransfer.setData('text/plain', event.target.id);
}

function handleDrop(event) {
    event.preventDefault();
    const draggedId = event.dataTransfer.getData('text/plain');
    const draggedElement = document.getElementById(draggedId);

    if (draggedElement.isEqualNode(event.currentTarget)) {
        return;
    }
    let draggedRelic = parseInt(draggedElement.dataset.relic, 10);
    let draggedImage = null;
    if (draggedElement.children.length > 0) {
        draggedImage = draggedElement.children[0].cloneNode();
    }

    let targetRelic = parseInt(event.currentTarget.dataset.relic, 10);
    let targetImage = null;
    if (event.currentTarget.children.length > 0) {
        targetImage = event.currentTarget.children[0].cloneNode();
    }

    event.currentTarget.dataset.relic = draggedRelic;
    draggedElement.dataset.relic = targetRelic;

    draggedElement.innerHTML = '';
    event.currentTarget.innerHTML = '';

    if (targetImage !== null) {
        draggedElement.appendChild(targetImage);
        targetImage.addEventListener('click', toggleRelic);
    }
    if (draggedImage !== null) {
        event.currentTarget.appendChild(draggedImage);
        draggedImage.addEventListener('click', toggleRelic);
    }
}

function handleDragOver(event) {
    event.preventDefault();
}

function toggleRelic(event) {
    if (event.target.classList.contains('uncollected')) {
        event.target.classList.remove('uncollected');
    } else {
        event.target.classList.add('uncollected');
    }
}

function initializeCells() {
    let currentRow = null;
    let cells = 0;

    for (let i = 0; i < 14; i++) {
        currentRow = document.createElement("div");
        for (let j = 0; j < 15; j++) {
            const slot = document.createElement("div");
            slot.className = "slot";
            slot.id = "cell" + cells;
            slot.setAttribute("draggable", "true");
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
    for (let i = 1; i < slots.length; i++) {
        const cell = document.getElementById("cell" + slots[i]);
        if (!cell) {
            break;
        }
        cell.innerHTML = "";
        const col = textureCoordinates[i-1].col;
        const row = textureCoordinates[i-1].row;
        const trackedObject = document.createElement("div");
        trackedObject.id = 'object' + slots[i];
        trackedObject.draggable = false;
        trackedObject.className = "relic uncollected";
        trackedObject.style.setProperty("--col", col);
        trackedObject.style.setProperty("--row", row);
        cell.appendChild(trackedObject);
    }
    relics = document.querySelectorAll('.relic');
    relics.forEach(relic => {
        relic.addEventListener('click', toggleRelic);
    });
}

window.onload = (event) => {
    initializeCells();
    insertItems();
};