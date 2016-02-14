$(document).ready(function() {
    (function deleteMultiplayerSettings() {
        if (JSON.parse(window.sessionStorage.getItem("played"))) {
            window.sessionStorage.setItem("removeMultiplayer", JSON.stringify(true));
            window.sessionStorage.removeItem("played");
            window.sessionStorage.removeItem("GameStarted");
        }
    })();
});