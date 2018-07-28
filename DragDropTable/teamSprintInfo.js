
var spacing = 30;
var currentSprint = "1807-2";

var cols = ["Teams\Sprints", "1807-2", "1808-1", "1808-2", "1809-1", "1809-2"];
var teams = ["DareDevils", "Black Panthers", "Incredibles", "Dauntless"];
var teamColor = ["violet", "blue", "green", "orange", "magenta"];
function getTeamColor(team) {
    for (var i = 0; i < teams.length; i++) {
        if (teams[i] == team)
            return teamColor[i];
    }
}

function IsOldSprint(sprint) {
    for (var i = 1; i < cols.length; i++) {
        if (cols[i] == currentSprint)
            return false;
        if (cols[i] == sprint)
            return true;
    }
    return false;
}
