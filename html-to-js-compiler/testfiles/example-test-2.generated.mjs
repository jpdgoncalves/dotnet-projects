
export function baseCommands() {
    let commands = document.createElement("div");
    commands.setAttribute("class", "commands");
    commands.setAttribute("data-component-name", "Commands");

    let text3 = document.createTextNode("\r\n    ");

    let div4 = document.createElement("div");
    div4.setAttribute("class", "commands__mode mode");
    div4.setAttribute("data-side", "top");

    let text5 = document.createTextNode("\r\n        ");

    let img6 = document.createElement("img");
    img6.setAttribute("class", "mode__icon");

    let text7 = document.createTextNode("\r\n        ");

    let ul8 = document.createElement("ul");
    ul8.setAttribute("class", "mode__content");
    ul8.setAttribute("data-getter", "ModeElement");

    let text9 = document.createTextNode("\r\n            ");

    let li10 = document.createElement("li");
    li10.setAttribute("class", "mode__option");
    li10.setAttribute("data-value", "add-mode");

    let img11 = document.createElement("img");
    img11.setAttribute("src", "./icons/plus.svg");
    img11.setAttribute("alt", "");
    img11.setAttribute("class", "mode__icon");

    li10.appendChild(img11);

    let text12 = document.createTextNode("\r\n            ");

    let li13 = document.createElement("li");
    li13.setAttribute("class", "mode__option");
    li13.setAttribute("data-value", "edit-mode");

    let img14 = document.createElement("img");
    img14.setAttribute("src", "./icons/pencil.svg");
    img14.setAttribute("alt", "");
    img14.setAttribute("class", "mode__icon");

    li13.appendChild(img14);

    let text15 = document.createTextNode("\r\n        ");

    ul8.appendChild(text9);
    ul8.appendChild(li10);
    ul8.appendChild(text12);
    ul8.appendChild(li13);
    ul8.appendChild(text15);

    let text16 = document.createTextNode("\r\n    ");

    div4.appendChild(text5);
    div4.appendChild(img6);
    div4.appendChild(text7);
    div4.appendChild(ul8);
    div4.appendChild(text16);

    let text17 = document.createTextNode("\r\n    ");

    let div18 = document.createElement("div");
    div18.setAttribute("class", "commands__input");
    div18.setAttribute("contenteditable", "true");

    let text19 = document.createTextNode("\r\n    ");

    let div20 = document.createElement("div");
    div20.setAttribute("class", "commands__send");
    div20.setAttribute("data-getter", "SendElement");

    let text21 = document.createTextNode("\r\n        ");

    let img22 = document.createElement("img");
    img22.setAttribute("src", "icons/arrow.svg");

    let text23 = document.createTextNode("\r\n    ");

    div20.appendChild(text21);
    div20.appendChild(img22);
    div20.appendChild(text23);

    let text24 = document.createTextNode("\r\n");

    commands.appendChild(text3);
    commands.appendChild(div4);
    commands.appendChild(text17);
    commands.appendChild(div18);
    commands.appendChild(text19);
    commands.appendChild(div20);
    commands.appendChild(text24);

    return commands;
}

export function getModeElement() {
    return commands.childNodes[1].childNodes[1].childNodes[3];
}

export function getSendElement() {
    return commands.childNodes[1].childNodes[5];
}

export function baseButton() {
    let button = document.createElement("button");
    button.setAttribute("data-component-name", "Button");

    return button;
}

