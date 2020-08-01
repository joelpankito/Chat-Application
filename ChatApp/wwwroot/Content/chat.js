"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendMessage").disabled = true;

connection.on("ReceiveMessage", function (msg) {
    var message = "";
    $.each(msg.messages, function (key, msg) {
        console.log(msg);
        message += msgDiv(key, msg);
    });
    $(".chat__main").append(message);
    updateScroll();
});

connection.on("UserStatus", function (usr) {
    //console.log(usr)
    var usrDiv = "";
    $.each(usr.users, function (key, user) {
        //console.log(value); //this outputs response as Objects shown above

            usrDiv += '<a class="user__item contact-' + user.id + '" >'
                + '<li>'
                + '<div class="avatar">'
                + '<img src="/Content/no_avatar.png">'
                + '</div>'
                + '<span>' + user.name + '</span>'
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
    connection.invoke("LoadUsers", parseInt(getCookie("userId"))).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendMessage").addEventListener("click", function (event) {
    //var user = getCookie("userName");
    //var user_id = getCookie("userId");
    var message = $("#msg_box").val();
    connection.invoke("SendMessage", message, parseInt(1)).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


function msgDiv(id, message) {

    var msgDiv = "";
    if (message.user == getCookie("userName")) {
        msgDiv += '<div id="dateStr-' + id + '">' + message.dateTimeString + '<div id="msg-' + message.id + '" class="row __chat__par__"> <div class="__chat__ receive__chat"> <p>'
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
        //console.log(msg);

    });


    $(".chat__main").append(messages);


}

function updateScroll() {
    $(".chat__body").scrollTop(document.querySelector(".chat__body").scrollHeight)
}