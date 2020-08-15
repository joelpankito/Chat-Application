"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendMessage").disabled = true;

connection.on("ReceiveMessage", function (messages) {
    var message = "";
    
    $.each(messages.messages, function (key, msg) {
        console.log(msg);
        var status = messages.sendMsgStatus == null ? msg.loadMsgStatus : messages.sendMsgStatus;
        message += msgDiv(key, msg, status);
    });
    $(".chat__main").append(message);
    updateScroll();
});

connection.on("DisplayUsers", function (users) {
    var usrDiv = "";
    $(".list_of_users").empty();

    $.each(users, function (key, user) {

            usrDiv += '<a class="user__item contact-' + user + '" >'
                + '<li>'
                + '<div class="avatar">'
                + '<img src="/Content/no_avatar.png">'
                + '</div>'
                + '<span>' + user + '</span>'
                + '<div class="status-bar active"></div>'
                + '</li>'
                + '</a>';
    });

    $(".list_of_users").append(usrDiv);  // Append the results
});

connection.start().then(function () {
    document.getElementById("sendMessage").disabled = false;
    connection.invoke("LoadMessages").catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("LoadUsers").catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendMessage").addEventListener("click", function (event) {
    sendMessage();
    event.preventDefault();
});

$("#msg_box").on('keypress', function (e) {
    if (e.which == 13) {
        sendMessage();
        e.preventDefault();
    }
});

function sendMessage() {
    var message = $("#msg_box").val();

    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });

    $("#msg_box").val("");
}

function msgDiv(id, message, status) {

    var msgDiv = "";
    if (status == "sent") {
        msgDiv += '<div id="dateStr-' + id + '" style="display: contents;">' + message.dateTimeString + '<div id="msg-' + message.id + '" class="row __chat__par__"> <div class="__chat__ receive__chat"> <p>'
            + message.user + ': ' + message.message + '</p> <p class="delivery-status">Delivered</p> </div> </div> </div>';
    }
    else {
        msgDiv += '<div id="dateStr-' + id + '">' + message.dateTimeString + '<div id="msg-' + message.id + '" class="row __chat__par__"> <div class="__chat__ from__chat"> <p>'
            + message.user + ': ' + message.message + '</p> <p class="delivery-status">Delivered</p> </div> </div> </div>';
    }
    return msgDiv;
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function loadmessages(msg) {
    var messages = "";

    $.each(msg, function (key, msg) {
        messages += msgDiv(key, msg);
    });


    $(".chat__main").append(messages);
}

function updateScroll() {
    $(".chat__body").scrollTop(document.querySelector(".chat__body").scrollHeight)
}

