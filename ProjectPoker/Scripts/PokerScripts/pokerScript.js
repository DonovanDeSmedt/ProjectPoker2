viewModel = {
    init: function () {
        $.get("/Poker/GetIndexActivePlayer", function (data) {
            var botTurn = data;
            if (botTurn == "True") {
                $('.actionButton').click(function (e) {
                    e.preventDefault();
                });
                $.get("/Poker/BotDecision", function (data) {
                    $("#body").empty();
                    $("#body").html(data);
                });
                //$("#bot")[0].click();
            }
        });
        $("#checkButton").click(function (e) {
            e.preventDefault();
            $.get("/Poker/Check", function (data) {
                $("#body").empty();
                $("#body").html(data);
            });
        });
        $("#callButton").click(function (e) {
            e.preventDefault();
            $.get("/Poker/Check", function (data) {
                $("#body").empty();
                $("#body").html(data);
            });
        });
        $("#foldButton").click(function (e) {
            e.preventDefault();
            $.get("/Poker/Fold", function (data) {
                $("#body").empty();
                $("#body").html(data);
            });
        });
        $("form").submit(function () {
            $.post(this.action, $(this).serialize(), function (data) {
                $("#body").html(data);
            });
            return false;
        });
    }
},
$(document).ready(function () {
    $(function () {
        $("#slider").slider({
            range: "min",
            value: 37,
            min: 1,
            max: 1000,
            slide: function (event, ui) {
                $("#amount").val(ui.value);
            }
        });
        $("#amount").on('input', function () {
            $("#slider").slider("value", $(this).val());
        });
        $.get("/Poker/GetMinimumBet", function (data) {
            var array = data.split(":");
            $("#slider").slider("value", parseInt(array[0]));
            $("#slider").slider("option", "min", parseInt(array[0]));
            $("#slider").slider("option", "max", parseInt(array[1]));
        });

    });
    viewModel.init();
});