function getColorByWorkItemType(workItemType, workItemSprint, workItemState) {
    if (IsOldSprint(workItemSprint) && workItemState != 'closed') {
        return "#ffb2b2";
    }

    if (workItemState == "closed") {
        return "lightgreen";
    }

    if (workItemType.toLowerCase() == "feature")
        return "rgb(199, 178, 248)";
    if (workItemType.toLowerCase() == "dependency")
        return "#ffff88";
    if (workItemType.toLowerCase() == "userstory")
        return "orange";
    return "violet";
}

//relations = {rel:"reverse/forward",item:"workitem"}
var WorkItem = function (workItemId, workItemType, content, sprint, team, relations, workItemDueDate, workItemState) {
    this.workItemId = workItemId;
    this.sprint = sprint;
    this.workItemType = workItemType;
    this.team = team;
    this.content = content;
    this.workItemDueDate = workItemDueDate;
    this.workItemState = workItemState;

    if (!relations) relations = [];

    for (var i = 0; i < relations.length; i++) {
        relations[i].relatedItemDivId = function (j) {
            return 'item-' + relations[j].item;
        }
    }

    this.relations = relations;

    return {
        workItemid: this.workItemId,
        workItemType: this.workItemType,
        workItemState: this.workItemState,
        workItemDueDate: this.workItemDueDate,
        sprint: this.sprint,
        team: this.team,
        content: this.content,
        relations: this.relations,
        itemDivId: function () {
            return "item-" + this.workItemid;
        },
        appendWorkItem: function (tableName) {
            var topDivTag = "<div class='ui-item " + this.itemDivId() + " ui-draggable ui-draggable-handle ui-droppable' style='position: relative;background-color:" + getColorByWorkItemType(this.workItemType, this.sprint, this.workItemState) + "' >";
            var anchorTag = "<div class='con_anchor ui-draggable ui-draggable-handle'></div>";
            var relationData = "";
            if (this.relations) {
                for (var i = 0; i < this.relations.length; i++) {
                    relationData += "<br/>" + (i+1) + ". <a href='#'>" + this.relations[i].UserStory + "</a>";
                }
            }
            var content = "<div> <a href='http://" + this.workItemType + "'><b>" + this.workItemid + "</b></a><br/>" + this.content + "<br/>" + relationData + "</div>";
            var result = topDivTag + anchorTag + content + "</div>";
            $("#" + getTableCellId(this.team, this.sprint)).append(result);
            //this.setDefaultPosition();
        },
        setDefaultPosition: function (left, top) {
            var itm = $("." + this.itemDivId());
            var cellPos = $('#' + getTableCellId(this.team, this.sprint)).position();
            if (!left) {
                left = (cellPos.left + 25) + "px";
            }
            if (!top) {
                top = (cellPos.top + 25) + "px";
            }

            itm.css("top", top);
            itm.css("left", left);
        },
        connect: function () {
            for (var i = 0; i < this.relations.length; i++) {
                connect(this, this.itemDivId(), this.relations[i].relatedItemDivId(i));
            }
        }
    }
}

function connect(workItem, source, destination) {
    source = $('.' + source);
    destination = $('.' + destination);
    AddLine(workItem, source);
    $(destination).data('connected-item', source);
    source.data('lines').push(source.data('line'));

    if ($(destination).data('connected-lines')) {
        $(destination).data('connected-lines').push(source.data('line'));

        var y2_ = parseInt($(source.data('line')).attr('y2'));
        $(source.data('line')).attr('y2', y2_ + spacing + $(destination).data('connected-lines').length * 5);

    } else $(destination).data('connected-lines', [source.data('line')]);

    source.data('line', null);
    var con_lines = $(destination).data('connected-lines');
    if (con_lines) {
        con_lines.forEach(function (con_line, id) {
            $(con_line).attr('x2', $(destination).position().left + spacing)
                .attr('y2', $(destination).position().top + spacing + (parseInt($(destination).css('height')) / 2) + (id * 5));
        }.bind(destination));
    }
}

function AddLine(workItem, cur_ui_item) {
    var connector = $('#connector_canvas');
    var cur_con;

    if (!$(cur_ui_item).data('lines')) $(cur_ui_item).data('lines', []);

    if (!$(cur_ui_item).data('line')) {
        var start = cur_ui_item.position();
        cur_con = $(document.createElementNS('http://www.w3.org/2000/svg', 'line'));
        var cur_con = document.createElementNS('http://www.w3.org/2000/svg', 'line');
        cur_con.setAttribute('x1', start.left + spacing);
        cur_con.setAttribute('y1', start.top + spacing);
        cur_con.setAttribute('x2', start.left + spacing);
        cur_con.setAttribute('y2', start.top + spacing);
        cur_con.setAttribute('stroke', getTeamColor(workItem.team));
        cur_ui_item.data('line', cur_con);
    } else cur_con = cur_ui_item.data('line');

    connector.append(cur_con);
}
