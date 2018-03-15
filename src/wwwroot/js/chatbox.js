"use strict";

let connection = new signalR.HubConnection('/chat');
let path = $('#path').text();

/*
blocks: Array<Block> ;
class Block {
    userName: String;
    avatarPath: String;
    timeStamp: String;
    messages: Array<String>;
}
*/
connection.on('send', (blocks) => {
    console.log(blocks);
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
        block.messages.forEach((msg, i) => {
            txt += `<span style="display: block">${msg}</span>`;
        });
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