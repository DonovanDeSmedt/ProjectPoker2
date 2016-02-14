viewModel = {
    con: null,
    hub: null,
    isSinglePlayer: false,
    isMultiPlayer: false,
    name: null,
    session: window.sessionStorage,
    init: function () {
        viewModel.name = viewModel.session.getItem("name");

        if (JSON.parse(viewModel.session.getItem("GameStarted"))) {
            viewModel.play();
        } else {
            $(function () {
                if (viewModel.con == null) {
                    viewModel.con = $.hubConnection();
                    viewModel.session.setItem("played", JSON.stringify(true));
                    viewModel.hub = viewModel.con.createHubProxy("pokerBox");
                }
                $("#startButton").click(function () {
                    viewModel.hub.invoke('findOpponent');
                });
                //Callback functions
                viewModel.hub.on('registerComplete', function (data) {
                    $("#info").empty();
                    $("#lobby").empty();
                    $("#lobby").append($("<tr>").append($("<th>").text("Name"))
                        .append($("<th>").text("Money"))
                        .append($("<th>").text("Time joined")));
                    data.forEach(function (player) {
                        $("#lobby").append("<tr><td>" + player.Name + "</td>" +
                            "<td>" + player.Money + "</td>" +
                            "<td>" + player.JoinDateTime + "</td>" +
                            "</tr>");
                    });
                });
                viewModel.hub.on('showButton', function () {
                    $("#startButton").show();
                });
                viewModel.hub.on('foundOpponent', function (data) {
                    $("#info").empty();
                    $("#lobby").empty();
                    $("#lobby").append($("<tr>").append($("<th>").text("Name"))
                        .append($("<th>").text("Money"))
                        .append($("<th>").text("Time joined")));
                    data.forEach(function (player) {
                        $("#lobby").append("<tr><td>" + player.Name + "</td>" +
                            "<td>" + player.Money + "</td>" +
                            "<td>" + player.JoinDateTime + "</td>" +
                            "</tr>");
                    });

                });
                viewModel.hub.on('noOpponents', function (data) {
                    $("#info").append("<h2>You need at least one more player!</h2>");
                    //$("#lobby").empty();
                    //$("#lobby").append($("<tr>").append($("<th>").text("Name"))
                    //    .append($("<th>").text("Money"))
                    //    .append($("<th>").text("Time joined")));
                    //data.forEach(function (player) {
                    //    $("#lobby").append("<tr><td>" + player.Name + "</td>" +
                    //        "<td>" + player.Money + "</td>" +
                    //        "<td>" + player.JoinDateTime + "</td>" +
                    //        "</tr>");
                    //});
                });
                viewModel.hub.on('newPokerGame', function (player) {
                    viewModel.hub.invoke('initGame', player);
                });
                viewModel.hub.on('onCheck', function (data) {
                    $.get("/Poker/CheckMultiplayer", { name: data }, function (result) {
                        $("#body").html(result);
                    });
                });
                viewModel.hub.on('onBet', function (data, number) {
                    $.get("/Poker/BetMultiplayer", { name: data, amount: number }, function (result) {
                        $("#body").html(result);
                    });
                });
                viewModel.hub.on('onFold', function (data) {
                    $.get("/Poker/FoldMultiplayer", { name: data }, function (result) {
                        $("#body").html(result);
                    });
                });
                viewModel.hub.on('onEndGame', function (data) {
                    $.get("/Poker/EndGame", { name: data }, function (result) {
                        $("#body").html(result);
                    });
                });
                viewModel.hub.on('nextRound', function (data) {
                    $.get("/Poker/NextRoundMultiplayer", { name: data }, function (result) {
                        $("#body").html(result);
                    });
                });
                viewModel.hub.on('startGame', function (data) {
                    console.log("initgame");
                    viewModel.session.setItem("name", data);
                    $.get("/Poker/NewMultiplayerGame", { name: data }, function (result) {
                        viewModel.session.setItem("GameStarted", JSON.stringify(true));
                        $("#body").html(result);
                    });
                });
                viewModel.hub.on('removeStorage', function (message) {
                    viewModel.session.removeItem("GameStarted");
                });
                viewModel.con.disconnected(function () {
                    console.log("Socket is disonnected");
                    setTimeout(function () {
                        viewModel.con.start();
                    }, 2000);
                });
                // Hub functions
                if (!JSON.parse(viewModel.session.getItem("GameStarted"))) {
                    viewModel.con.start(function () {
                        var name;
                        var money;
                        if (JSON.parse(viewModel.session.getItem("removeMultiplayer"))) {
                            viewModel.session.removeItem("removeMultiplayer");
                            viewModel.hub.invoke('removeMultiplayer');
                        }
                        $.getJSON("/Poker/GetNameActivePlayer", function (data) {
                            name = data.Name;
                            money = data.Money;
                            console.log(name);
                            console.log(money);
                            viewModel.hub.invoke('registerPlayer', name, money);
                            //viewModel.hub.invoke('findOpponent');
                        });
                    });
                }
            });
        }
    },
    play: function () {
        //$(function () {
        if (viewModel.con == null) {
            viewModel.con = $.hubConnection();
            viewModel.hub = viewModel.con.createHubProxy("pokerBox");
            viewModel.con.start();
        }
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
        $.get("/Poker/GetMinimumBet", { name: viewModel.name }, function (data) {
            var array = data.split(":");
            $("#slider").slider("value", parseInt(array[0]));
            $("#slider").slider("option", "min", parseInt(array[0]));
            $("#slider").slider("option", "max", parseInt(array[1]));
        });
        $("#checkButton").click(function (e) {
            e.preventDefault();
            viewModel.hub.invoke('check', viewModel.session.getItem("name"));
        });
        $("#callButton").click(function (e) {
            e.preventDefault();
            viewModel.hub.invoke('check', viewModel.session.getItem("name"));
        });
        $("#foldButton").click(function (e) {
            e.preventDefault();
            viewModel.hub.invoke('fold', viewModel.session.getItem("name"));
        });
        $("#betButton").click(function (e) {
            e.preventDefault();
            viewModel.hub.invoke('bet', viewModel.session.getItem("name"), parseInt($("#amount").val()));
        });
        $("#nextRoundButton").click(function (e) {
            e.preventDefault();
            viewModel.hub.invoke('nextRound', viewModel.session.getItem("name"));
        });
    }
}
$(document).ready(function () {
    viewModel.init();
});