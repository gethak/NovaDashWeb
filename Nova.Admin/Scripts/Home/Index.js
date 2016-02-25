
var scale = chroma.scale(["#ade1fb", "#097dc6"]).domain([1, 100]),
selectedShape = null,
selectedLocations = "Arizona";

function resizeMap() {
    var map = $("#map").data("kendoMap");
    map.resize();
    map.center([50.000, -50]);
}

function setSparkLinesWidths() {
    var containerWidth = $(".sparkline-container").parent().width(),
        getSparkLines = $(".k-sparkline"),
        sparkLineWidth = (80 * containerWidth) / 100;

    getSparkLines.each(function () {
        $(this).data("kendoSparkline").setOptions({ chartArea: { width: sparkLineWidth } });
    });

    $("#MarketShare").data("kendoChart").resize();
    $("#GenderShare").data("kendoChart").resize();
    $("#CategoryShare").data("kendoChart").resize();
}

function onCategoryShareDataBound(e) {
    if (e.sender.dataSource.data().length > 0) {
        $("#RevenueNoData").hide();
    } else {
        $("#RevenueNoData").show();
    }
}

function onGenderShareDataBound(e) {
    if (e.sender.dataSource.data().length > 0) {
        $("#customersNoData").hide();
    } else {
        $("#customersNoData").show();
    }
}

function onTotalCustomersByAllStatesDataBound(e) {
    if (e.sender.dataSource.data().length > 0) {
        $("ProductsNoData").hide();
    } else {
        $("ProductsNoData").show();
    }
}

function onMarketShareDataBound(e) {
    var percentage = 0;
    if (this.dataSource.data().length == 2) {
        percentage = (this.dataSource.at(1).CustomersCount / this.dataSource.at(0).CustomersCount);
    }
    $("#MarketShareNoData").toggle(percentage === 0);
    //$("#marketShareContainer").toggle(percentage === 0);
    if (percentage == 0) {
        $("#marketShareContainer .sparkline-container").hide();
    }
    else {
        $("#marketShareContainer .sparkline-container").show();
    }
}

function onTopProductsDataBound() {
    var items = this.dataSource.data().length;
    $("#ProductsNoData").toggle(items === 0);
}

function onCriteriaChange() {
    var MarketShare = $("#MarketShare").data("kendoChart"),
        CategoryShare = $("#CategoryShare").data("kendoChart"),
        GenderShare = $("#GenderShare").data("kendoChart"),
        //Customers = $("#Customers").data("kendoSparkline"),
        //fromDate = $("#StartDate").data("kendoDatePicker").value(),
        fromDate = new Date();
    //toDate = $("#EndDate").data("kendoDatePicker").value();
    toDate = new Date();

    $.ajax({
        url: urlStateCustomersTotal,
        dataType: "json",
        data: { stateName: selectedLocations, FromDate: fromDate.toJSON(), ToDate: toDate.toJSON() },
        type: "POST",
        success: function (response) {
            $("#CustomersLabel").text(response.CustomersCount);
            $("#CustomersNoData").toggle(response.CustomersCount[0] === 0);
        }
    });

    MarketShare.dataSource.read({ stateName: selectedLocations, FromDate: fromDate, ToDate: toDate });
    CategoryShare.dataSource.read({ stateName: selectedLocations, FromDate: fromDate, ToDate: toDate });
    $("#countryName").text(selectedLocations);
    GenderShare.dataSource.read({ stateName: selectedLocations, FromDate: fromDate, ToDate: toDate });
}

function onShapeCreated(e) {
    var color = scale(e.shape.dataItem.properties.sales).hex();
    e.shape.fill(color);
}

function onShapeClick(e) {
    if (selectedShape) {
        var sales = selectedShape.dataItem.properties.sales;
        var color = scale(sales).hex();
        selectedShape.options.set("fill.color", color);
        selectedShape.options.set("stroke.color", "white");
        selectedShape.dataItem.properties.selected = false;
    }

    e.shape.options.set("fill.color", "#0c669f");
    e.shape.options.set("stroke.color", "black");
    e.shape.dataItem.properties.selected = true;
    selectedShape = e.shape;
    selectedLocations = selectedShape.dataItem.properties.name;

    onCriteriaChange();
}

function onShapeMouseEnter(e) {
    e.shape.options.set("fill.color", "#0c669f");
}

function onShapeMouseLeave(e) {
    if (!e.shape.dataItem.properties.selected) {
        var sales = e.shape.dataItem.properties.sales;
        var color = scale(sales).hex();
        e.shape.options.set("fill.color", color);
        e.shape.options.set("stroke.color", "white");
    }
}


function customersAutoComplete_onChange(e) {
    var event = e;
}

function customersAutoComplete_onAdditionalData() {
    return {
        text: $("#statesAutoComplete").val()
    };
}

function customersAutoComplete_onSelect(e) {
    var dataItem = this.dataItem(e.item.index());
    console.log(dataItem);

    selectedLocations = dataItem.Name;
    onCriteriaChange();
}

function customersAutoComplete_onDataBound(e) {
    //        $("#customers").data("kendoGrid").dataSource.filter({});
}


