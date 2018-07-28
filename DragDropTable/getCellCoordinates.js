function getScreenCoordinates(obj) {
    console.log(obj);
    var p = {};
    p.x = obj.offsetLeft;
    p.y = obj.offsetTop;
    while (obj.offsetParent) {
        p.x = p.x + obj.offsetParent.offsetLeft;
        p.y = p.y + obj.offsetParent.offsetTop;
        if (obj == document.getElementsByTagName("body")[0]) {
            break;
        }
        else {
            obj = obj.offsetParent;
        }
    }
    return p;
}

function calculateCellCoordinates(tableName) {
    console.log(tableName);
    var tableVar = document.getElementById(tableName);
    console.log(tableVar);
    var numOfRows = document.getElementById(tableName).rows.length;
    console.log(numOfRows);
    var numOfCols = document.getElementById(tableName).rows[0].cells.length;
    console.log(numOfCols);
    var i, j;
    var rowMarker = [];
    var colMarker = [];
    for (i = 0; i < numOfRows; i++) {
        var p = getScreenCoordinates(tableVar.rows[i]);
        rowMarker.push(p.y);
    }
    console.log("Row Marker or The y coordinates of rows of the table " + rowMarker);
    for (j = 0; j < numOfCols; j++) {
        var p = getScreenCoordinates(tableVar.rows[0].cells[j]);
        colMarker.push(p.x);
        // alert("Row:" + i + "Col:" + j + "X:" + p.x + " Y:" + p.y);
    }
    console.log("Col Marker or The x coordinates of columns of the table " + colMarker);
    return [colMarker, rowMarker];
}

function getCellIndexing(x, y) {
    console.log("Running Indexing function for " + x + " " + y);
    var markers = calculateCellCoordinates('tbla');
    var xMarker = markers[0];
    var yMarker = markers[1];
    var i, j;
    var xIndex = -1;
    var yIndex = -1;
    if (x < xMarker[0]) {
        xIndex = -1;
    } else {
        for (i = 0; i < xMarker.length - 1; i++) {
            if (x >= xMarker[i] && x < xMarker[i + 1]) {
                break;
            }
        }
        xIndex = i;
    }
    if (y < yMarker[0]) {
        yIndex = -1;
    } else {
        for (j = 0; j < yMarker.length - 1; j++) {
            if (y >= yMarker[j] && y < yMarker[j + 1]) {
                break;
            }
        }
        yIndex = j;
    }
    console.log("Column " + xIndex + " Row " + yIndex);
    return [xIndex, yIndex];
}

function getPlacementCoordinates(sprint, team, tableName) {
    var tableVar = document.getElementById(tableName);
    var numOfRows = document.getElementById(tableName).rows.length;
    var numOfCols = document.getElementById(tableName).rows[0].cells.length;

    var i = 0;

    for (i = 1; i < numOfCols; i++) {
        if (sprint == document.getElementById(tableName).rows[0].cells[i].innerText) {
            break;
        }
    }

    console.log("i= " + i);

    var j = 0;
    for (j = 1; j < numOfRows; j++) {
        if (team == document.getElementById(tableName).rows[j].cells[0].innerText) {
            break;
        }
    }

    console.log("j= " + j);

    var markers = calculateCellCoordinates('tbla');
    var xMarker = markers[0];
    var yMarker = markers[1];
    console.log(xMarker[i] + " " + yMarker[j]);
    return [xMarker[i], yMarker[j]];
}