
var tableString = "<table id='tbla' border='1'>";

function getTableCellId(team,sprint) {
    return (team + "-" + sprint).replace(" ", "-");
}

tableString += "<tr>";
for (var i = 0; i < cols.length; i++) {
    if (cols[i] == currentSprint) {
        tableString += "<th><h2>" + cols[i] + "</h2></th>";
    } else {
        tableString += "<th>" + cols[i] + "</th>";
    }
}
tableString += "</tr>";


for (var j = 0; j < teams.length; j++) {

    tableString += "<tr>";
    tableString += "<td id='" + getTableCellId(teams[j], cols[0]) + "' style='color:" + getTeamColor(teams[j]) + "'><b>" + teams[j] + "</b></td>";
    for (var i = 1; i < cols.length; i++) {
        tableString += "<td id='" + getTableCellId(teams[j],cols[i]) + "'></td>";
    }
    tableString += "</tr>";
}
tableString += "</table>";
$("#canvas").append(tableString);