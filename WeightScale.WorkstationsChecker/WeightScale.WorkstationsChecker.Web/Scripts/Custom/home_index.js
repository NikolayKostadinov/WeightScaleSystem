var myTimer;
var localStorageSupport = (('localStorage' in window && window['localStorage'] !== null)),
data,
autorefresh,
html;
if (localStorageSupport) {
    //retrieve local storage data if such is available, else use the default order
    data = JSON.parse(localStorage.getItem("sortableData"));
    autorefresh = localStorage.getItem("autorefresh");
} else {
    alert("your browser does not support local storage");
    data = initialData || [];
}

var additionalData = function (idParam) {
    return {
        id: idParam,
        startPeriod: $("#startPeriod").data("kendoDateTimePicker").value(),
        endPeriod: $("#endPeriod").data("kendoDateTimePicker").value()
    };
}

function RefreshCharts() {
    var charts = $(".charts");
    startProgress(charts)
    //setTimeout(function () {
        var timeSpan = $("#endPeriod").data("kendoDateTimePicker").value() - $("#startPeriod").data("kendoDateTimePicker").value();
        for (var i = 0; i < charts.length; i++) {
            var currChart = $(charts[i]);
            if (timeSpan <= 10 * 60 * 1000) {
                currChart.data("kendoChart").options.categoryAxis.baseUnit = "seconds";
                currChart.data("kendoChart").options.categoryAxis.labels.step = 10;
            }
            else {
                currChart.data("kendoChart").options.categoryAxis.baseUnit = "minutes";
                currChart.data("kendoChart").options.categoryAxis.labels.step = 1;
            }
            currChart.data("kendoChart").dataSource.read(additionalData(currChart.data().id));
            currChart.data("kendoChart").redraw();
            currChart.data("kendoChart").refresh();
        }
   // }, 200);
}

function AutoRefresh() {
    if ($("#autorefresh")) {
        if ($("#autorefresh").is(':checked')) {
            localStorage.setItem("autorefresh", 'checked');
            myTimer = setInterval(AutorefreshCharts, 30000);
        }
        else {
            localStorage.setItem("autorefresh", null);
            clearInterval(myTimer);
        }
    }
}

function AutorefreshCharts() {
    $("#startPeriod").data("kendoDateTimePicker").value(new Date(new Date() - (1 * 1000 * 60 * 60)));//An hour before now
    $("#endPeriod").data("kendoDateTimePicker").value(new Date());
    RefreshCharts();
}

function dataBind(ev) {
    var currChart = '#' + $(ev.sender.element).attr('id');
   kendo.ui.progress($(currChart), false);
}

$(document).ready(function () {
    //set autorefresh
    if ($("#autorefresh")) {
        if ($("#autorefresh").is(':checked')) {
            myTimer = setInterval(AutorefreshCharts, 30000);
        }
    }

    //start progresses
    startProgress($(".charts"));

    //exapand
    $("div.body-content").on("click", "span.k-i-arrowhead-s", function (e) {
        var contentElement = $(e.target).closest(".widget").find(">div");
        $(e.target)
            .removeClass("k-i-arrowhead-s")
            .addClass("k-i-arrowhead-n");

        kendo.fx(contentElement).expand("vertical").stop().play();
    });

    //collapse
    $("div.body-content").on("click", "span.k-i-arrowhead-n", function (e) {
        var contentElement = $(e.target).closest(".widget").find(">div");
        $(e.target)
            .removeClass("k-i-arrowhead-n")
            .addClass("k-i-arrowhead-s");

        kendo.fx(contentElement).expand("vertical").stop().reverse();
    });

    //exapand
    $("div#navbar").on("click", "span.k-i-arrowhead-s", function (e) {
        if ($(this).prop('id') === "collapse-navbar") {
            var contentElement = $("#period");
            $(e.target)
                .removeClass("k-i-arrowhead-s")
                .addClass("k-i-arrowhead-n");

            kendo.fx(contentElement).expand("vertical").stop().play();
            $("#body-content").css("margin-top", "140px");
        } else if ($(this).prop('id') === "collapse-all") {
            $(this).next("Label").text("Collapse All");
            $(e.target)
                .removeClass("k-i-arrowhead-s")
                .addClass("k-i-arrowhead-n");
            $(".widget").find("span")
                .removeClass("k-i-arrowhead-s")
                .addClass("k-i-arrowhead-n");
            kendo.fx($(".widget").find(">div")).expand("vertical").stop().play();
        }
    });

    //collapse
    $("div#navbar").on("click", "span.k-i-arrowhead-n", function (e) {
        if ($(this).prop('id') === "collapse-navbar") {
            var contentElement = $("#period");
            $(e.target)
                .removeClass("k-i-arrowhead-n")
                .addClass("k-i-arrowhead-s");
            kendo.fx(contentElement).expand("vertical").stop().reverse();
            $("#body-content").css("margin-top", "70px");
        } else if ($(this).prop('id') === "collapse-all") {
            $(this).next("Label").text("Expand All");
            $(e.target)
                .removeClass("k-i-arrowhead-n")
                .addClass("k-i-arrowhead-s");
            $(".widget").find("span")
                .removeClass("k-i-arrowhead-n")
                .addClass("k-i-arrowhead-s");
            kendo.fx($(".widget").find(">div")).expand("vertical").stop().reverse();
        }
    });

    // stor charts in sortable
    var charts = $("div#body-content");
    $("#nav-wrapper").prev;
    charts.kendoSortable({
        filter: ">div.widget",
        cursor: "move",
        connectWith: "#navbar",
        placeholder: placeholder,
        hint: hint,
        change: function (e) {
            var item = data.splice(e.oldIndex, 1)[0]; //remove the item that has changed its order
            data.splice(e.newIndex, 0, item); //add the item back using the newIndex
            localStorage.setItem("sortableData", JSON.stringify(data)); //set the updated data in the local storage
        }
    });

    // get initial position of graphics
    if (!data) {
        data = InitializeData();
    }

    reorderSortable();

    //start progresses
    //startProgress($(".charts"));

    // set autorefresh checkbox
    autorefresh == 'null' ? $('#autorefresh').attr('checked', null) : $('#autorefresh').attr('checked', autorefresh);

    //rebing charts
    $(window).load(function () {
        startProgress($('.charts'));
        redrawCharts();
    });
});

function placeholder(element) {
    return element.clone().addClass("placeholder");
}

function hint(element) {
    return element.clone().addClass("hint")
        .height(element.height())
        .width(element.width());
}

var reorderSortable = function () {
    var wrapper = $("div#body-content");
    if (data) {
        wrapper.html("");
    }
    var length = data.length || 0; //render the HTML with the data
    for (var i = 0; i < length; i++) {
        wrapper.append($('<div class="widget"></div>').html(data[i]));
    }
    //append the HTML to the Sortable container
}

var InitializeData = function () {
    var initialData = []; //initial items order (used if there is no saved data
    var widgets = $('div.widget');
    var length = widgets.length;
    for (var i = 0; i < length; i++) {
        initialData[i] = placeholder($(widgets[i])).html();
    }
    return initialData;
}

var redrawCharts = function () {
    var charts = $(".charts");
    for (var i = 0; i < charts.length; i++) {
        var currChart = $(charts[i]);
        if (currChart.data("kendoChart")) {
            currChart.data("kendoChart").dataSource.read(additionalData(currChart.data().id));
            currChart.data("kendoChart").redraw();
            currChart.data("kendoChart").refresh();
        }
    }
}

var startProgress = function (charts) {
    for (var i = 0; i < charts.length; i++) {
        var currChart = $(charts[i])
        kendo.ui.progress(currChart, true);
    }
}
