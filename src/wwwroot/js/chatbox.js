"use strict";

let connection = new signalR.HubConnection('/chat');
let path = $('#path').text();

/*
blocks: Array<Block> ;
class Block {
    userName: String;
    avatarPath: String;
    timeStamp: String;
    messages: {id: number => msg: String};
    hasEditRight: Boolean;
}
*/
connection.on('send', (blocks) => {
    let display = $('#DisplayMessages');
    display.empty();

    blocks.forEach((block, i) => {
        let date = new Date(block.timeStamp);
        let month = date.getMonth().toString().padStart(2, "0");
        let day = date.getDate().toString().padStart(2, "0");
        let hours = date.getHours().toString().padStart(2, "0");
        let min = date.getMinutes().toString().padStart(2, "0");
        let time = `<span class="w3-tiny w3-text-light-grey">${month}/${day} ${hours}:${min}</span>`;
        let usr = `<b class="w3-large">${block.userName}</b>`;
        let txt = '<div>';
        for (let i in block.messages) {
            let msg = block.messages[i];
            let x = "", y = "", len = 12;
            console.log(block.hasEditRight);
            if (block.hasEditRight) {
                x = `<span class="w3-col s1 m1 l1" onClick="deleteMessage(${i})"><i class="w3-button w3-right glyphicon glyphicon-remove w3-text-red"></i></span>`;
                y = `<span class="w3-col s1 m1 l1" onClick="editMessage(${i})"><i class="w3-button w3-right glyphicon glyphicon-edit w3-text-yellow"></i></span>`;
                len -= 2;
            }
            txt += `<div class="w3-row-padding"><span class="w3-col s${len} m${len} l${len} chatbox_message" id="chatbox_message_${i}">${msg}</span>${y}${x}</div>`;
        }
        txt += '</div>';
        let img = `<img src="${path}${block.avatarPath || 'images/no_avatar.jpg'}" class="w3-col" style="width: 75px; padding-top: 10px;"></img>`;
        let div = `<div class="w3-row-padding w3-padding">${img}<div class="w3-rest" style="padding-left:1em;"><p class="w3-bar-item" style="line-height: 1;">${usr} ${time}</p>${txt}</div></div><hr style='margin: 5px;'/>`;
        display.append(div);
    });

    setTimeout(() => display.scrollTop(display.prop('scrollHeight')), 100);
});

connection.start()
    .then(() => connection.invoke('RetrieveMessages', 1));
    
function SendMessage(){
    var msg = document.getElementById("txtMessage");
    connection.invoke('send', msg.value, 1);
    msg.value = "";
    return false;
}

function deleteMessage(id) {
    connection.invoke('DeleteMessage', 1, id);
}

function editMessage(id) {
    let newMsg = prompt("Here, you can edit your message: ", $(`#chatbox_message_${id}`).text());
    if (newMsg != null) {
        connection.invoke('EditMessage', 1, id, newMsg);
    }
}