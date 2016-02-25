
function oncustomerDetailsEdit(e) {
    if (!e.model.isNew()) {
        e.container.find("[name=ProductID]").data("kendoDropDownList").enable(false);
    } else {
        e.container.find("td.k-hierarchy-cell").css({ visibility: "hidden" });
    }
}

function onDataBound(e) {
    var firstRow = this.tbody.find("tr.k-master-row").first();
    var model = this.dataItem(firstRow);
    if (!model.isNew()) {
        this.expandRow(firstRow);
    }
}

function oncustomerDetailsDataBound(e) {
    this.table.find("span[name=sum]").html(kendo.toString(sum, "c"));

    var firstRow = this.tbody.find("tr.k-master-row").first();
    var model = this.dataItem(firstRow);
    if (!model.isNew()) {
        this.expandRow(firstRow);
    }
}

function onError(e, gridName) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function(key, value) {
            if ('errors' in value) {
                $.each(value.errors, function() {
                    message += this + "\n";
                });
            }
        });
        var grid = $("#" + gridName).data("kendoGrid");
        grid.one("dataBinding", function(e) {
            e.preventDefault();
        });
        alert(message);
    }
}

function resizeGrid() {
    var gridElement = $("#customers"),
        dataArea = gridElement.find(".k-grid-content"),
        gridHeight = gridElement.innerHeight(),
        otherElements = gridElement.children().not(".k-grid-content"),
        otherElementsHeight = 0;
    otherElements.each(function() {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(gridHeight - otherElementsHeight);
}


function customersAutoComplete_onChange(e) {
    var event = e;
}

function customersAutoComplete_onAdditionalData() {
    return {
        text: $("#customersAutoComplete").val()
    };
}

function customersAutoComplete_onSelect(e) {
    var dataItem = this.dataItem(e.item.index());
    console.log(dataItem);

    $("#customers").data("kendoGrid").dataSource.filter({
        "field": "Id",
        "operator": "eq",
        "value": dataItem.Id
    });
}

function customersAutoComplete_onDataBound(e) {
    $("#customers").data("kendoGrid").dataSource.filter({});
}

