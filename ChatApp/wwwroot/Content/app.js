function getmessages() {
    $.ajax(
        {
            method: "GET",
            url: "https://localhost:44308/api/Chat/getallmessages",
        })
        .done(function (msg) {
            //console.log(msg);
            loadusers();
            loadmessages(msg.messages);
            updateScroll();
            getrecentmessages();
        })
 
}

function getrecentmessages() {
    var last = $('.chat__main').children().last().children().attr('id');
    //console.log(last);
    var lastid = parseInt(last.split("msg-")[1]);
    //console.log(lastid);
    $.ajax(
        {
            method: "GET",
            url: "https://localhost:44308/api/Chat/GetRecentMessage",
            data: { lastId: lastid },
        })
        .done(function (msg) {
            //console.log(msg)
            if (msg.messages != []) {
                loadmessages(msg.messages);
                updateScroll();
            }
            
            setTimeout(function () { getrecentmessages(); }, 1000);
        })
        
}

function loadmessages(msg) {
    var messages = "";

    $.each(msg, function (key,msg) {
        
        messages += msgDiv(key,msg);
        //console.log(msg);
        
    });

   
    $(".chat__main").append(messages); 
    
    
}


function msgDiv(id,message) {

    var msgDiv = "";
    if (message.status == "sent") {
        msgDiv += '<div id="dateStr-' + id + '">' + message.dateTimeString + '<div id="msg-' + message.id + '" class="row __chat__par__"> <div class="__chat__ receive__chat"> <p>'
            + message.user + ': ' + message.message + '</p> <p class="delivery-status">Delivered</p> </div> </div> </div>';
    }
    else {
        msgDiv += '<div id="dateStr-' + id + '">' + message.dateTimeString + '<div id="msg-' + message.id + '" class="row __chat__par__"> <div class="__chat__ from__chat"> <p>'
            + message.user + ': ' + message.message + '</p> <p class="delivery-status">Delivered</p> </div> </div> </div>';
    }
    return msgDiv;
}



$("#sendMessage").click(function () {
    var data = $("#msg_box").val();
    $.post("https://localhost:44308/api/Chat/post", { message: data, id: getCookie("userId") })
        .done(function (msg) {
            //console.log(msg);
            var message = "";
            $.each(msg.messages, function (key, msg) {
                message = msgDiv(parseInt($('.chat__main').children().last().attr('id').split("dateStr-")[1]), msg);
            });
            $(".chat__main").append(message);
            updateScroll();
    })
});

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

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function loadusers() {
    $.ajax(
        {
            method: "GET",
            url: "https://localhost:44308/api/Chat/getallusers",
        })
        .done(function (usr) {
            //console.log(usr)
            var usrDiv = "";
            $.each(usr.users, function(key,user) {
                //console.log(value); //this outputs response as Objects shown above
                if (user.id == getCookie("userId")) {
                    usrDiv += '<a class="user__item contact-' + user.id + '" >'
                        + '<li>'
                        + '<div class="avatar">'
                        + '<img src="/Content/no_avatar.png">'
                        + '</div>'
                        + '<span>' + user.name + '</span>'
                        + '<div class="status-bar active"></div>'
                        + '</li>'
                        + '</a>';
                }
                else {
                    usrDiv += '<a class="user__item contact-' + user.id + '" >'
                        + '<li>'
                        + '<div class="avatar">'
                        + '<img src="/Content/no_avatar.png">'
                        + '</div>'
                        + '<span>' + user.name + '</span>'
                        + '<div class="status-bar"></div>'
                        + '</li>'
                        + '</a>';
                }
                
            });

            $(".list_of_users").append(usrDiv);  // Append the results
        });
}

function updateScroll() {
    $(".chat__body").scrollTop(document.querySelector(".chat__body").scrollHeight)
}

getmessages();


