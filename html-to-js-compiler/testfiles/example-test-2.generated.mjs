
export default function baseCommands() {
    let Commands = document.createElement("div");
    Commands.setAttribute("class", "commands");
    Commands.setAttribute("data-component", "Commands");

    let text1 = document.createTextNode("\r\n    ");

    let div2 = document.createElement("div");
    div2.setAttribute("class", "commands__mode mode");
    div2.setAttribute("data-side", "top");

    let text3 = document.createTextNode("\r\n        ");

    let img4 = document.createElement("img");
    img4.setAttribute("class", "mode__icon");

    let text5 = document.createTextNode("\r\n        ");

    let ul6 = document.createElement("ul");
    ul6.setAttribute("class", "mode__content");
    ul6.setAttribute("data-getter", "ModeElement");

    let text7 = document.createTextNode("\r\n            ");

    let li8 = document.createElement("li");
    li8.setAttribute("class", "mode__option");
    li8.setAttribute("data-value", "add-mode");

    let img9 = document.createElement("img");
    img9.setAttribute("src", "./icons/plus.svg");
    img9.setAttribute("alt", "");
    img9.setAttribute("class", "mode__icon");

    li8.appendChild(img9);

    let text10 = document.createTextNode("\r\n            ");

    let li11 = document.createElement("li");
    li11.setAttribute("class", "mode__option");
    li11.setAttribute("data-value", "edit-mode");

    let img12 = document.createElement("img");
    img12.setAttribute("src", "./icons/pencil.svg");
    img12.setAttribute("alt", "");
    img12.setAttribute("class", "mode__icon");

    li11.appendChild(img12);

    let text13 = document.createTextNode("\r\n        ");

    ul6.appendChild(text7);
    ul6.appendChild(li8);
    ul6.appendChild(text10);
    ul6.appendChild(li11);
    ul6.appendChild(text13);

    let text14 = document.createTextNode("\r\n    ");

    div2.appendChild(text3);
    div2.appendChild(img4);
    div2.appendChild(text5);
    div2.appendChild(ul6);
    div2.appendChild(text14);

    let text15 = document.createTextNode("\r\n    ");

    let div16 = document.createElement("div");
    div16.setAttribute("class", "commands__input");
    div16.setAttribute("contenteditable", "true");

    let text17 = document.createTextNode("\r\n    ");

    let div18 = document.createElement("div");
    div18.setAttribute("class", "commands__send");
    div18.setAttribute("data-getter", "SendElement");

    let text19 = document.createTextNode("\r\n        ");

    let img20 = document.createElement("img");
    img20.setAttribute("src", "icons/arrow.svg");

    let text21 = document.createTextNode("\r\n    ");

    div18.appendChild(text19);
    div18.appendChild(img20);
    div18.appendChild(text21);

    let text22 = document.createTextNode("\r\n");

    Commands.appendChild(text1);
    Commands.appendChild(div2);
    Commands.appendChild(text15);
    Commands.appendChild(div16);
    Commands.appendChild(text17);
    Commands.appendChild(div18);
    Commands.appendChild(text22);

    return Commands;
}

export function getModeElement(commands) {
    return commands.childNodes[1].childNodes[3];
}

export function getSendElement(commands) {
    return commands.childNodes[5];
}


export default function baseButton() {
    let Button = document.createElement("button");
    Button.setAttribute("data-component", "Button");

    return Button;
}

