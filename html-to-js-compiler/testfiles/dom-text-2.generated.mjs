
export function baseCommands() {
    let Commands = document.createElement("div");
    Commands.setAttribute("class", "commands");
    Commands.setAttribute("data-component", "Commands");

    let text1 = document.createTextNode("\r\n        ");

    let div2 = document.createElement("div");
    div2.setAttribute("class", "commands__mode mode");
    div2.setAttribute("data-side", "top");

    let text3 = document.createTextNode("\r\n            ");

    let img4 = document.createElement("img");
    img4.setAttribute("class", "mode__icon");

    let text5 = document.createTextNode("\r\n            ");

    let ul6 = document.createElement("ul");
    ul6.setAttribute("class", "mode__content");
    ul6.setAttribute("data-getter", "ModeElement");

    let text7 = document.createTextNode("\r\n                ");

    let li8 = document.createElement("li");
    li8.setAttribute("class", "mode__option");
    li8.setAttribute("data-value", "add-mode");

    let img9 = document.createElement("img");
    img9.setAttribute("src", "./icons/plus.svg");
    img9.setAttribute("alt", "");
    img9.setAttribute("class", "mode__icon");

    let text10 = document.createTextNode("\r\n                ");

    li8.appendChild(img9);
    li8.appendChild(text10);

    let text11 = document.createTextNode("\r\n                ");

    let li12 = document.createElement("li");
    li12.setAttribute("class", "mode__option");
    li12.setAttribute("data-value", "edit-mode");

    let img13 = document.createElement("img");
    img13.setAttribute("src", "./icons/pencil.svg");
    img13.setAttribute("alt", "");
    img13.setAttribute("class", "mode__icon");

    let text14 = document.createTextNode("\r\n                ");

    li12.appendChild(img13);
    li12.appendChild(text14);

    let text15 = document.createTextNode("\r\n            ");

    ul6.appendChild(text7);
    ul6.appendChild(li8);
    ul6.appendChild(text11);
    ul6.appendChild(li12);
    ul6.appendChild(text15);

    let text16 = document.createTextNode("\r\n        ");

    div2.appendChild(text3);
    div2.appendChild(img4);
    div2.appendChild(text5);
    div2.appendChild(ul6);
    div2.appendChild(text16);

    let text17 = document.createTextNode("\r\n        ");

    let div18 = document.createElement("div");
    div18.setAttribute("class", "commands__input");
    div18.setAttribute("contenteditable", "true");

    let text19 = document.createTextNode("\r\n        ");

    let div20 = document.createElement("div");
    div20.setAttribute("class", "commands__send");
    div20.setAttribute("data-getter", "SendElement");

    let text21 = document.createTextNode("\r\n            ");

    let img22 = document.createElement("img");
    img22.setAttribute("src", "icons/arrow.svg");

    let text23 = document.createTextNode("\r\n        ");

    div20.appendChild(text21);
    div20.appendChild(img22);
    div20.appendChild(text23);

    let text24 = document.createTextNode("\r\n    ");

    Commands.appendChild(text1);
    Commands.appendChild(div2);
    Commands.appendChild(text17);
    Commands.appendChild(div18);
    Commands.appendChild(text19);
    Commands.appendChild(div20);
    Commands.appendChild(text24);

    return Commands;
}

export function getModeElement(commands) {
    return commands.childNodes[1].childNodes[3];
}

export function getSendElement(commands) {
    return commands.childNodes[5];
}


export function baseButton() {
    let Button = document.createElement("button");
    Button.setAttribute("data-component", "Button");

    return Button;
}

